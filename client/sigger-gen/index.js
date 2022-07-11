import * as parser from './lib/sigger-parser.js';
import * as generator from './lib/sigger-generator.js';

export async function siggerGen(source, destination, flags) {
  if (flags.verbose) {
    var flagsJson = JSON.stringify(flags);
    console.log(
      `Generate sigger from ${source} to ${destination} with flags ${flagsJson}`
    );
  }

  const sigger = await parser.parse(source, flags);
  await generator.generate(sigger, destination, flags);
}
