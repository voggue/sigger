import fs from 'fs';
import path from 'path';
import url from 'url';
import { findUp } from 'find-up';

export async function parse(siggerFile, flags) {
  if (stringIsAValidUrl(siggerFile)) {
    if (flags.verbose)
      console.log('download definition from url ' + siggerFile + '...');
    return await readFromUrl(siggerFile, flags);
  }

  const packageRootPath = await packageDirectory();
  const fullPath = path.isAbsolute(siggerFile)
    ? siggerFile
    : path.join(packageRootPath ?? process.cwd(), siggerFile);
  if (flags.verbose)
    console.log('parse definition from file ' + fullPath + '...');
  return await readFromFile(fullPath, flags);
}

async function readFromUrl(siggerUrl, flags) {
  if (flags.insecureTls) {
    console.warn(
      'Warning: TLS certificate verification is disabled (--insecure-tls). Do not use in CI or production.'
    );
    return await readFromUrlInsecure(siggerUrl);
  }

  const res = await fetch(siggerUrl, { method: 'GET' });
  if (!res.ok) {
    throw new Error(`HTTP ${res.status} ${res.statusText} for ${siggerUrl}`);
  }
  return res.json();
}

/** HTTPS/HTTP GET with optional TLS verification off (only when --insecure-tls). */
function readFromUrlInsecure(siggerUrl) {
  const target = new url.URL(siggerUrl);
  const isHttps = target.protocol === 'https:';

  return new Promise((resolve, reject) => {
    const requestLib = isHttps
      ? // dynamic import keeps http path free of tls options
        import('node:https').then((m) => m.default)
      : import('node:http').then((m) => m.default);

    requestLib
      .then((httpOrHttps) => {
        const options = {
          hostname: target.hostname,
          port: target.port || (isHttps ? 443 : 80),
          path: target.pathname + target.search,
          method: 'GET',
          ...(isHttps ? { rejectUnauthorized: false } : {}),
        };

        const req = httpOrHttps.request(options, (res) => {
          if (res.statusCode && (res.statusCode < 200 || res.statusCode >= 300)) {
            reject(
              new Error(`HTTP ${res.statusCode} ${res.statusMessage ?? ''} for ${siggerUrl}`)
            );
            res.resume();
            return;
          }
          const chunks = [];
          res.on('data', (c) => chunks.push(c));
          res.on('end', () => {
            try {
              const text = Buffer.concat(chunks).toString('utf8');
              resolve(JSON.parse(text));
            } catch (e) {
              reject(e);
            }
          });
        });
        req.on('error', reject);
        req.end();
      })
      .catch(reject);
  });
}

async function readFromFile(fullPath, flags) {
  const data = await fs.promises.readFile(fullPath, 'utf8');
  if (flags.verbose) {
    const preview = data.length > 200 ? data.slice(0, 200) + '…' : data;
    console.log('schema preview:', preview);
  }
  return JSON.parse(data);
}

async function packageDirectory({ cwd } = {}) {
  const filePath = await findUp('package.json', { cwd });
  return filePath && path.dirname(filePath);
}

function stringIsAValidUrl(s) {
  if (/^[a-zA-Z]:[\\/]/.test(s)) {
    return false;
  }
  try {
    const u = new url.URL(s);
    return u.protocol === 'http:' || u.protocol === 'https:';
  } catch {
    return false;
  }
}
