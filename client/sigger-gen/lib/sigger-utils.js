import { findUp } from 'find-up';
import { hasFlag, TypeFlags } from './sigger-enums.js';
import fs from 'fs';
import path from 'path';

const INDENT_SIZE = 2;

export const L1 = ''.padStart(INDENT_SIZE);
export const L2 = ''.padStart(2 * INDENT_SIZE);
export const L3 = ''.padStart(3 * INDENT_SIZE);
export const L4 = ''.padStart(4 * INDENT_SIZE);
export const L5 = ''.padStart(5 * INDENT_SIZE);
export const L6 = ''.padStart(6 * INDENT_SIZE);
export const L7 = ''.padStart(7 * INDENT_SIZE);
export const L8 = ''.padStart(8 * INDENT_SIZE);

/** formats a type-string  */
export function createTypeString(type) {
  if (hasFlag(type.flagsValue, TypeFlags.IsNullable)) return type.exportedType + ' | null';
  return type.exportedType;
}

/** get directory of package.json  */
export async function getRootDirectory({ cwd } = {}) {
  const rootPath = await findUp('package.json', { cwd });
  return rootPath ? path.dirname(rootPath) : './';
}

/** writes the source-code to file */
export function writeToFile(dir, file, data) {
  const filename = path.join(dir, file);
  fs.writeFile(filename, data, { encoding: 'utf-8' }, (err) => {
    if (err) throw err;
  });
}
