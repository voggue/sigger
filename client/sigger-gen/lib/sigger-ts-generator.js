import fs from 'fs';
import path from 'path';
import { hasFlag, isComplexOrEnum, isDictionary, KeepValueMode, TypeFlags } from './sigger-enums.js';
import * as indentation from './sigger-utils.js';
import { createTypeString, writeToFile } from './sigger-utils.js';
import { EOL } from 'node:os';
export class TsGenerationBase {
  verbose(message, ...optionalParams) {
    if (this.flags.verbose) {
      if (optionalParams.length) console.log('NgGeneration | verbose | ' + message, optionalParams);
      else console.log('NgGeneration | verbose | ' + message);
    }
  }

  error(message, ...optionalParams) {
    if (optionalParams.length) console.error('NgGeneration | error | ' + message, optionalParams);
    else console.error('NgGeneration | error | ' + message);
  }

  writeToFile(dir, file, data) {
    if (this.flags.test) {
      console.log(def);
      return;
    }

    writeToFile(dir, file, data);
  }
}

export class TsGeneration extends TsGenerationBase {
  constructor(rootDirectory, definition, output, flags) {
    super();
    this.rootDirectory = rootDirectory;
    this.definition = definition;
    this.output = output;
    this.flags = flags;
  }
}

export class TsHubGeneration extends TsGenerationBase {
  constructor(tsGeneration, hub) {
    super();
    this.hub = hub;
    this.generation = tsGeneration;
    this.flags = tsGeneration.flags;
    this.rootDirectory = tsGeneration.rootDirectory;
    this.output = tsGeneration.output;
    this.flags = tsGeneration.flags;
    this.generatedModels = [];
  }

  async getModelsDirectory() {
    const hub = this.hub;
    if (this.flags.test) return '';

    const fullPath = path.resolve(path.join(this.rootDirectory, this.output, hub.exportedName, 'models'));
    const exists = fs.existsSync(fullPath);
    if (!exists) {
      this.verbose('create directory: ' + fullPath + '...');
      await fs.promises.mkdir(fullPath, { recursive: true });
    }
    return fullPath;
  }

  async generateModelsIndex() {
    let code = '';
    const hub = this.hub;
    for (let index = 0; index < this.generatedModels.length; index++) {
      const model = this.generatedModels[index];
      code += `export * from './${model}';${EOL}`;
    }

    const dir = await this.getModelsDirectory();
    this.writeToFile(dir, 'index.ts', code);
  }

  async generateModels() {
    const hub = this.hub;
    const types = hub.definitions.types;
    if (!types || !types.length) {
      this.verbose('no models found for hub ' + hub.exportedName + '...');
      return;
    }

    const dir = await this.getModelsDirectory(hub);
    this.verbose('generate angular models for hub ' + hub.exportedName + '...');
    for (let typeIdx = 0; typeIdx < types.length; typeIdx++) {
      const type = types[typeIdx];

      const def = this.createTypeScriptClass(type);
      this.writeToFile(dir, `${type.exportedName}.ts`, def);
      this.generatedModels.push(type.exportedName);
    }
  }

  async generateEnums() {
    const hub = this.hub;
    const types = hub.definitions.enums;
    if (!types || !types.length) {
      this.verbose('no enums found for hub ' + hub.exportedName + '...');
      return;
    }

    const dir = await this.getModelsDirectory(hub);
    this.verbose('generate angular enums for hub ' + hub.exportedName + '...');
    for (let typeIdx = 0; typeIdx < types.length; typeIdx++) {
      const type = types[typeIdx];

      const def = this.createTypeEnumDefinition(type);
      const catalog = this.createTypeEnumCatalog(type);
      this.writeToFile(dir, `${type.exportedName}.ts`, def + EOL + EOL + catalog);
      this.generatedModels.push(type.exportedName);
    }
  }

  createTypeEnumDefinition(enumDef) {
    const indent = indentation.L1;
    let code = '';
    code += `/** ${enumDef.caption} */${EOL}`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */${EOL}`;
    code += `export enum ${enumDef.exportedName} {${EOL}`;
    for (let itemIdx = 0; itemIdx < enumDef.items.length; itemIdx++) {
      const prop = enumDef.items[itemIdx];
      code += `${EOL}`;
      code += `${indent}/** ${prop.caption} */${EOL}`;
      code += `${indent}${prop.exportedName} = ${prop.value}${itemIdx == enumDef.items.length - 1 ? '' : ','}${EOL}`;
    }
    code += `}${EOL}${EOL}`;
    return code;
  }

  createTypeEnumCatalog(enumDef) {
    let code = '';
    code += `/** ${enumDef.caption} Catalog */${EOL}`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */${EOL}`;
    code += `export const ${enumDef.exportedName}Catalog = {${EOL}`;
    for (let itemIdx = 0; itemIdx < enumDef.items.length; itemIdx++) {
      const prop = enumDef.items[itemIdx];
      let indent = indentation.L1;
      code += `${indent}${EOL}`;
      code += `${indent}/** ${prop.caption} */${EOL}`;
      code += `${indent}${prop.exportedName}: {${EOL}`;
      indent = indentation.L2;
      code += `${indent}caption: "${prop.caption}",${EOL}`;
      if (prop.description) code += `${indent}description: "${prop.description}",${EOL}`;
      code += `${indent}value: ${enumDef.exportedName}.${prop.exportedName},${EOL}`;
      code += `${indent}intValue: ${prop.value}${EOL}`;
      indent = indentation.L1;
      code += `${indent}}${itemIdx == enumDef.items.length - 1 ? '' : ','}${EOL}`;
    }
    code += `}${EOL}${EOL}`;
    code += `/** ${enumDef.caption} Items List */${EOL}`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */${EOL}`;
    code += `export const ${enumDef.exportedName}List = Object.values(${enumDef.exportedName}Catalog);${EOL}${EOL}`;

    const enumMetaDef = `{ caption: string, value: ${enumDef.exportedName}, intValue: number, description?: string | null }`;

    code += `/** ${enumDef.caption} Items Record */${EOL}`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */${EOL}`;
    code += `export const ${enumDef.exportedName}Record : Record<${enumDef.exportedName}, ${enumMetaDef}>${EOL}`;
    code += `    = Object.assign({}, ...Object.values(${enumDef.exportedName}Catalog).map((x) => ({ [x.value]: x })));${EOL}${EOL}${EOL}`;

    return code;
  }

  createTypeScriptClass(classDef) {
    let indent = indentation.L1;
    let imports = [];
    let code = '';
    code += `/** ${classDef.caption} */${EOL}`;
    code += `/** generated from .net Type ${classDef.dotnetType} */${EOL}`;
    code += `export interface ${classDef.exportedName} {${EOL}`;
    for (let propIdx = 0; propIdx < classDef.properties.length; propIdx++) {
      const prop = classDef.properties[propIdx];
      code += `${indent}${EOL}`;
      code += `${indent}/** ${prop.caption} */${EOL}`;
      code += `${indent}${prop.exportedName}: ${createTypeString(prop.type)};${EOL}`;

      // on arrays use the array-element type as type
      let type = prop.type;
      if (hasFlag(type.flagsValue, TypeFlags.IsArray)) type = type.arrayElement;

      if (isComplexOrEnum(type) && classDef.exportedName !== type.exportedType) {
        imports.push(type.exportedType);
      } else if (isDictionary(type)) {
        // Handling for complex dictionary types (import is required)
        if (type.dictionaryValue) {
          if (isComplexOrEnum(type.dictionaryValue) && type.dictionaryValue.exportedName !== type.exportedType) {
            imports.push(type.dictionaryValue.exportedType);
          }
        } else {
          console.warn('Dictionary value is undefined', { classDef, prop, type });
        }
      }
    }
    code += `}${EOL}${EOL}`;

    // remove duplicates
    imports = imports.filter((v, i, a) => a.indexOf(v) === i);

    var importCode = '';
    if (imports.length) {
      for (let index = 0; index < imports.length; index++) {
        const i = imports[index];
        importCode += `import { ${i} } from "./${i}";${EOL}`;
      }
      importCode += `${EOL}${EOL}`;
    }

    return importCode + code;
  }

  verbose(message, ...optionalParams) {
    if (this.flags.verbose) {
      if (optionalParams.length) console.log(this.hub.exportedName + ' | verbose | ' + message, optionalParams);
      else console.log(this.hub.exportedName + ' | verbose | ' + message);
    }
  }

  error(message, ...optionalParams) {
    if (optionalParams.length) console.error(this.hub.exportedName + ' | error | ' + message, optionalParams);
    else console.error(this.hub.exportedName + ' | error | ' + message);
  }
}
