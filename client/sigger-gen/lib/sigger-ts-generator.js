import fs from 'fs';
import path from 'path';
import { hasFlag, isComplexOrEnum, KeepValueMode, TypeFlags } from './sigger-enums.js';
import * as indentation from './sigger-utils.js';
import { createTypeString, writeToFile } from './sigger-utils.js';

export class TsGeneration {
  constructor(rootDirectory, definition, output, flags) {
    this.rootDirectory = rootDirectory;
    this.definition = definition;
    this.output = output;
    this.flags = flags;
  }
}

export class TsHubGeneration {
  constructor(tsGeneration, hub) {
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
      if (this.flags.verbose) {
        console.log('Create directory: ' + fullPath + '...');
      }
      await fs.promises.mkdir(fullPath, { recursive: true });
    }
    return fullPath;
  }

  async generateModelsIndex() {
    let code = '';
    const hub = this.hub;
    for (let index = 0; index < this.generatedModels.length; index++) {
      const model = this.generatedModels[index];
      code += `export * from './${model}';\n`;
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
    this.verbose('generate angular models for hub ' + hub.exportedName + '...');
    for (let typeIdx = 0; typeIdx < types.length; typeIdx++) {
      const type = types[typeIdx];

      const def = this.createTypeEnumDefinition(type);
      const catalog = this.createTypeEnumCatalog(type);
      this.writeToFile(dir, `${type.exportedName}.ts`, def + '\n\n' + catalog);
      this.generatedModels.push(type.exportedName);
    }
  }

  writeToFile(dir, file, data) {
    if (this.flags.test) {
      console.log(def);
      return;
    }

    writeToFile(dir, file, data);
  }

  createTypeEnumDefinition(enumDef) {
    const indent = indentation.L1;
    let code = '';
    code += `/** ${enumDef.caption} */\n`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */\n`;
    code += `export enum ${enumDef.exportedName} {\n`;
    for (let itemIdx = 0; itemIdx < enumDef.items.length; itemIdx++) {
      const prop = enumDef.items[itemIdx];
      code += `\n`;
      code += `${indent}/** ${prop.caption} */\n`;
      code += `${indent}${prop.exportedName} = ${prop.value}${itemIdx == enumDef.items.length - 1 ? '' : ','}\n`;
    }
    code += `}\n\n`;
    return code;
  }

  createTypeEnumCatalog(enumDef) {
    let code = '';
    code += `/** ${enumDef.caption} Catalog */\n`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */\n`;
    code += `export const ${enumDef.exportedName}Catalog = {\n`;
    for (let itemIdx = 0; itemIdx < enumDef.items.length; itemIdx++) {
      const prop = enumDef.items[itemIdx];
      let indent = indentation.L1;
      code += `${indent}\n`;
      code += `${indent}/** ${prop.caption} */\n`;
      code += `${indent}${prop.exportedName}: {\n`;
      indent = indentation.L2;
      code += `${indent}caption: "${prop.caption}",\n`;
      if (prop.description) code += `${indent}description: "${prop.description}",\n`;
      code += `${indent}value: ${enumDef.exportedName}.${prop.exportedName},\n`;
      code += `${indent}intValue: ${prop.value}\n`;
      indent = indentation.L1;
      code += `${indent}}${itemIdx == enumDef.items.length - 1 ? '' : ','}\n`;
    }
    code += `}\n\n`;
    code += `/** ${enumDef.caption} Items List */\n`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */\n`;
    code += `export const ${enumDef.exportedName}List = Object.values(${enumDef.exportedName}Catalog);\n\n`;

    const enumMetaDef = `{ caption: string, value: ${enumDef.exportedName}, intValue: number, description?: string | null }`;

    code += `/** ${enumDef.caption} Items Record */\n`;
    code += `/** generated from .net Type ${enumDef.dotnetType} */\n`;
    code += `export const ${enumDef.exportedName}Record : Record<${enumDef.exportedName}, ${enumMetaDef}>\n`;
    code += `    = Object.assign({}, ...Object.values(${enumDef.exportedName}Catalog).map((x) => ({ [x.value]: x })));\n\n\n`;

    return code;
  }

  createTypeScriptClass(classDef) {
    let indent = indentation.L1;

    const imports = [];

    let code = '';
    code += `/** ${classDef.caption} */\n`;
    code += `/** generated from .net Type ${classDef.dotnetType} */\n`;
    code += `export interface ${classDef.exportedName} {\n`;
    for (let propIdx = 0; propIdx < classDef.properties.length; propIdx++) {
      const prop = classDef.properties[propIdx];
      code += `${indent}\n`;
      code += `${indent}/** ${prop.caption} */\n`;
      code += `${indent}${prop.exportedName}: ${createTypeString(prop.type)};\n`;

      // on arrays use the array-element type as type
      let type = prop.type;
      if (hasFlag(type.flagsValue, TypeFlags.IsArray)) type = type.arrayElement;

      if (isComplexOrEnum(type)) {
        imports.push(type.exportedType);
      }
    }
    code += `}\n\n`;

    var importCode = '';
    if (imports.length) {
      for (let index = 0; index < imports.length; index++) {
        const i = imports[index];
        importCode += `import { ${i} } from "./${i}";\n`;
      }
      importCode += `\n\n`;
    }

    return importCode + code;
  }

  verbose(message, ...optionalParams) {
    if (this.flags.verbose) {
      console.log(message, optionalParams);
    }
  }
}
