#! /usr/bin/env node

import meow from 'meow';
import { siggerGen } from '../index.js';

const cli = meow(
  `
  Usage
    $ sigger-gen <sigger.json> <destinationFolder>
  Examples
	  $ sigger-gen ./output/sigger.json ./generated/
`,
  {
    importMeta: import.meta,
    flags: {
      test: {
        type: 'boolean',
        alias: 't',
      },
      verbose: {
        type: 'boolean',
        alias: 'v',
      },
      framework: {
        type: 'string',
        alias: 'f',
        default: 'angular',
      },
    },
  }
);

if (cli.input.length < 2) {
  console.error('Missing: file and/or desination folder.');
  cli.showHelp(1);
}

if (cli.flags.verbose) {
  console.log(`Generate Sigger Client...`);
  console.log(` - Siggerfile: ${cli.input[0]}`);
  console.log(` - Destination: ${cli.input[1]}`);
}

siggerGen(cli.input[0], cli.input[1], cli.flags);
