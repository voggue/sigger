import assert from 'node:assert/strict';
import { once } from 'node:events';
import { mkdtemp, rm, writeFile } from 'node:fs/promises';
import { createServer } from 'node:http';
import { tmpdir } from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { parse } from '../lib/sigger-parser.js';

const schema = { hubs: [{ name: 'ChatHub', methods: [] }] };

async function fixture(t, contents) {
  const directory = await mkdtemp(path.join(tmpdir(), 'sigger-parser-'));
  t.after(() => rm(directory, { recursive: true, force: true }));
  const filename = path.join(directory, 'sigger.json');
  await writeFile(filename, contents);
  return filename;
}

test('reads a schema from an absolute file path', async (t) => {
  const filename = await fixture(t, JSON.stringify(schema));
  assert.deepEqual(await parse(filename, {}), schema);
});

test('rejects invalid JSON in a file', async (t) => {
  const filename = await fixture(t, '{invalid');
  await assert.rejects(parse(filename, {}), SyntaxError);
});

test('rejects a missing file', async (t) => {
  const filename = await fixture(t, '{}');
  await assert.rejects(parse(filename + '.missing', {}), { code: 'ENOENT' });
});

async function endpoint(t, status, body) {
  const server = createServer((request, response) => {
    assert.equal(request.url, '/sigger.json?version=1');
    response.writeHead(status, { 'Content-Type': 'application/json' });
    response.end(body);
  });
  t.after(() => new Promise((resolve, reject) => {
    server.close((error) => error ? reject(error) : resolve());
    server.closeAllConnections();
  }));
  server.listen(0, '127.0.0.1');
  await once(server, 'listening');
  return `http://127.0.0.1:${server.address().port}/sigger.json?version=1`;
}

test('downloads and parses a schema over HTTP', async (t) => {
  const address = await endpoint(t, 200, JSON.stringify(schema));
  assert.deepEqual(await parse(address, {}), schema);
});

test('rejects unsuccessful HTTP responses', async (t) => {
  const address = await endpoint(t, 404, '{}');
  await assert.rejects(parse(address, {}), /HTTP 404/);
});

test('rejects invalid JSON in an HTTP response', async (t) => {
  const address = await endpoint(t, 200, '{invalid');
  await assert.rejects(parse(address, {}), SyntaxError);
});
