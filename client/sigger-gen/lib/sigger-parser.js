import fs from 'fs';
import path from 'path';
import url from 'url';
import fetch from 'node-fetch';
import https from 'https';
import { findUp } from 'find-up';

export async function parse(siggerFile, flags) {
  if (stringIsAValidUrl(siggerFile)) {
    if (flags.verbose)
      console.log('download definition from url ' + siggerFile + '...');
    return await readFromUrl(siggerFile);
  }

  const packageRootPath = await packageDirectory();
  const fullPath = path.join(packageRootPath, siggerFile);
  if (flags.verbose)
    console.log('parse definition from file ' + fullPath + '...');
  return await readFromFile(fullPath);
}

async function readFromUrl(siggerUrl) {
  const httpsAgent = new https.Agent({
    rejectUnauthorized: false,
  });

  var f = await fetch(siggerUrl, {
    method: 'GET',
    agent: httpsAgent,
  });

  return f.json();
}

async function readFromFile(fullPath) {
  try {
    const data = await fs.readFile(fullPath, 'utf8');
    console.log(data);
  } catch (err) {
    console.error(err);
    throw err;
  }
}

async function packageDirectory({ cwd } = {}) {
  const filePath = await findUp('package.json', { cwd });
  return filePath && path.dirname(filePath);
}

function stringIsAValidUrl(s) {
  try {
    new url.URL(s);
    return true;
  } catch (err) {
    return false;
  }
}
