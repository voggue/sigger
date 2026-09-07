import assert from 'node:assert/strict';
import os from 'node:os';
import path from 'node:path';
import { mkdtemp, rm, writeFile } from 'node:fs/promises';
import test from 'node:test';

import { parse } from './sigger-parser.js';

test('parse reads sigger schema from a local file', async () => {
  const tempDir = await mkdtemp(path.join(os.tmpdir(), 'sigger-gen-'));
  const schemaPath = path.join(tempDir, 'schema.json');

  try {
    const expected = { hubs: [{ name: 'ChatHub' }] };
    await writeFile(schemaPath, JSON.stringify(expected), 'utf8');

    const actual = await parse(schemaPath, { verbose: false, insecureTls: false });
    assert.deepEqual(actual, expected);
  } finally {
    await rm(tempDir, { recursive: true, force: true });
  }
});
