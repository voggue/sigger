import * as angular from './sigger-ng-generator.js';

export async function generate(definition, output, flags) {
  if (flags.framework.toLowerCase() == 'angular') {
    if (flags.verbose) console.log('generate angular client...');
    await angular.generate(definition, output, flags);
  } else {
    console.error('Unknown framework ' + flags.framework);
  }
}
