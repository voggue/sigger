import fs from 'fs';
import path from 'path';
import { TsGeneration, TsHubGeneration } from './sigger-ts-generator.js';
import { hasFlag, isArray, isComplexOrEnum, isDictionary, isNullable, KeepValueMode, TypeFlags } from './sigger-enums.js';
import * as indentation from './sigger-utils.js';
import { getRootDirectory } from './sigger-utils.js';
import { EOL } from 'node:os';

export async function generate(definition, output, flags) {
  // const gen = new TsGeneration(definition, output, flags);
  // await gen.generate();

  const rootDirectory = await getRootDirectory();
  const generator = new NgGeneration(rootDirectory, definition, output, flags);

  await generator.generateGlobal();

  for (let hubIdx = 0; hubIdx < definition.hubs.length; hubIdx++) {
    const hub = definition.hubs[hubIdx];
    await generator.generate(hub);
  }
}

class NgHubGeneration extends TsHubGeneration {
  /** true if either a class or an enum is present */
  get hasModels() {
    return this.hub.definitions?.types?.length > 0 || this.hub.definitions?.enums?.length > 0;
  }

  constructor(ngGeneration, hub) {
    super(ngGeneration, hub);

    this.moduleName = `${this.hub.exportedName}Module`;
    this.configName = `${this.hub.exportedName}Configuration`;
  }

  /** Delete all generated files */
  async cleanup() {
    const fullPath = path.resolve(path.join(this.rootDirectory, this.output, this.hub.exportedName));
    if (!fs.existsSync(fullPath)) {
      this.verbose('directory does not exist: ' + fullPath);
      return;
    }

    try {
      this.verbose('remove directory: ' + fullPath);
      fs.rmSync(fullPath, { recursive: true });
    } catch (err) {
      console.error(err);
    }
  }

  /**
   * generate the index.ts file for the hub-module
   */
  async generateModuleIndex() {
    const dir = await this.getServiceDirectory();
    const file = `index.ts`;
    this.verbose('generate index file: ' + file);
    let code = '';

    code += `export * from './${this.hub.exportedName}.configuration';${EOL}`;
    code += `export * from './${this.hub.exportedName}.module';${EOL}`;
    code += `export * from './${this.hub.exportedName}.service';${EOL}`;
    if (this.hasModels) code += `export * as models from './models';${EOL}`;

    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the Service
   */
  async generateService() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.service.ts`;
    this.verbose('generate angular service: ' + file);
    var code = this.generateServiceCode();
    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the configuration for module
   */
  async generateModuleHubConfig() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.configuration.ts`;
    this.verbose('generate angular configuration: ' + file);
    var code = this.generateConfigCode();
    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the hub module
   */
  async generateModule() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.module.ts`;
    this.verbose('generate angular module: ' + file);
    var code = this.generateModuleCode();
    this.writeToFile(dir, file, code);
  }

  generateModuleCode() {
    const indent1 = indentation.L1;
    const indent2 = indentation.L2;
    const indent3 = indentation.L3;
    const indent4 = indentation.L4;
    const indent5 = indentation.L5;

    let code = '';

    code += `import { ${this.configName} } from './${this.hub.exportedName}.configuration';${EOL}`;
    code += `import { ${this.generation.configParamsName} } from '../${this.generation.configParamsFile}';${EOL}`;
    code += `import { ModuleWithProviders, NgModule } from '@angular/core';${EOL}`;

    code += EOL;
    code += `@NgModule({${EOL}`;
    code += `})${EOL}`;
    code += `export class ${this.moduleName} {${EOL}${EOL}`;

    code += `${indent1}static forRoot(params: ${this.generation.configParamsName}): ModuleWithProviders<${this.moduleName}> {${EOL}`;
    code += `${indent2}return {${EOL}`;
    code += `${indent3}ngModule: ${this.moduleName},${EOL}`;
    code += `${indent3}providers: [${EOL}`;
    code += `${indent4}{${EOL}`;
    code += `${indent5}provide: ${this.configName},${EOL}`;
    code += `${indent5}useValue: params${EOL}`;
    code += `${indent4}}${EOL}`; // close config provider object
    code += `${indent3}]${EOL}`; // close provider array
    code += `${indent2}}${EOL}`; // close return
    code += `${indent1}}${EOL}`; // close forRoot
    code += `}${EOL}`; // Close module
    return code;
  }

  generateConfigCode() {
    let indent = indentation.L1;
    let code = '';
    code += `import { Injectable } from '@angular/core';${EOL}`;
    code += `import { ${this.generation.configParamsName} } from '../${this.generation.configParamsFile}';${EOL}`;
    code += `import { IHttpConnectionOptions } from '@microsoft/signalr';${EOL}${EOL}`;

    code += `@Injectable({ providedIn: 'root' })${EOL}`;
    code += `export class ${this.configName} {${EOL}${EOL}`;

    code += `${indent}/**Default Url of ${this.hub.exportedName} endpoint; */${EOL}`;
    code += `${indent}defaultHubPath = '${this.hub.path}';${EOL}${EOL}`;

    code += `${indent}/**Configured Url of ${this.hub.exportedName} endpoint; */${EOL}`;
    code += `${indent}hubPath?: string;${EOL}${EOL}`;

    code += `${indent}/** Url of ${this.hub.exportedName} endpoint; */${EOL}`;
    code += `${indent}siggerUrl = '';${EOL}${EOL}`;

    code += `${indent}/** Negotiation can only be skipped when the IHttpConnectionOptions.transport property is set to 'HttpTransportType.WebSockets'.; */${EOL}`;
    code += `${indent}connectionConfig: IHttpConnectionOptions = {};${EOL}${EOL}`;

    code += `${indent}/** Connect to server on startup. */${EOL}`;
    code += `${indent}autoConnect?: boolean = true;${EOL}`;

    code += `}${EOL}${EOL}`;

    code += `export function ${this.configName}Factory(params: ${this.generation.configParamsName}) {${EOL}${EOL}`;
    code += `${indent}const config = new ${this.configName}();${EOL}`;
    code += `${indent}if(params.siggerUrl) config.siggerUrl = params.siggerUrl;${EOL}`;
    code += `${indent}if(params.hubPath) config.hubPath = params.hubPath;${EOL}`;
    code += `${indent}if(params.connectionConfig) config.connectionConfig = params.connectionConfig ?? {};${EOL}`;
    code += `${indent}config.autoConnect = params.autoConnect;${EOL}`;
    code += `${indent}return config;${EOL}`;
    code += `}${EOL}${EOL}`;

    return code;
  }

  generateServiceCode() {
    let code = '';
    code += this.generateImportsCode();

    code += `@Injectable({ providedIn: 'root' })${EOL}`;
    code += `export class ${this.hub.exportedName} {${EOL}${EOL}`;

    code += this.generateSignlRFieldsCode();
    code += this.generateEventsCode();
    code += this.generateConstructorCode();
    code += this.generateSignalRMethodsCode();
    code += this.generateUtilMethodsCode();

    code += `}${EOL}`;
    return code;
  }

  generateImportsCode() {
    let code = '';

    code += `import * as signalR from '@microsoft/signalr';${EOL}`;
    if (this.hasModels) code += `import * as models from './models';${EOL}`;
    code += `import { ${this.configName} } from './${this.hub.exportedName}.configuration';${EOL}`;
    code += `import { Injectable } from '@angular/core';${EOL}`;
    code += `import { BehaviorSubject, ReplaySubject, from, Observable, of, Subject, throwError } from 'rxjs';${EOL}`;
    code += `import { catchError, filter, switchMap, take } from 'rxjs/operators';${EOL}${EOL}${EOL}`;
    return code;
  }

  generateSignlRFieldsCode() {
    let indent = indentation.L1;
    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}`;
    code += `${indent}/* ${'SignalR Fields'.padEnd(80)} */${EOL}`;
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}${EOL}`;
    code += `${indent}/** signalR connection */${EOL}`;
    code += `${indent}private readonly _connection: signalR.HubConnection;${EOL}${EOL}`;

    code += `${indent}/** Internal connection state subject */${EOL}`;
    code += `${indent}private readonly _connected$ = new BehaviorSubject<boolean>(false);${EOL}`;
    code += `${indent}/** Connection state observable */${EOL}`;
    code += `${indent}readonly connected$ = this._connected$.asObservable();${EOL}${EOL}`;

    code += `${indent}/** Internal connection error subject */${EOL}`;
    code += `${indent}private readonly _error$ = new Subject<any>();${EOL}`;
    code += `${indent}/** Connection error observable */${EOL}`;
    code += `${indent}readonly error$ = this._error$.asObservable();${EOL}${EOL}`;
    return code;
  }

  generateEventsCode() {
    let code = '';
    const events = this.hub.events;
    if (!events) return code;

    let indent = indentation.L1;
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}`;
    code += `${indent}/* ${'SignalR Events'.padEnd(80)} */${EOL}`;
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}${EOL}`;

    for (let evIdx = 0; evIdx < events.length; evIdx++) {
      const e = events[evIdx];
      const subject =
        e.keepValue === KeepValueMode.FireAndForget ? 'Subject' : e.keepValue === KeepValueMode.KeepLastValue ? 'ReplaySubject' : 'BehaviorSubject';

      let args = '';
      if (e.arguments.length) {
        const isNestedObject = e.arguments.length > 1;
        if (isNestedObject) args += '{';

        for (let argIndex = 0; argIndex < e.arguments.length; argIndex++) {
          const arg = e.arguments[argIndex];
          if (argIndex > 0) {
            args += ', ';
          }

          let type = arg.type.exportedType;

          if (isArray(arg.type) && isComplexOrEnum(arg.type.arrayElement)) {
            type = 'models.' + type;
          } else if (isComplexOrEnum(arg.type)) {
            type = 'models.' + type;
          }

          if (hasFlag(arg.type.flagsValue, TypeFlags.IsNullable)) {
            type += ' | null';
          }

          if (isNestedObject) args += `${arg.exportedName}: `;
          args += type;
        }

        if (isNestedObject) args += '}';
      }

      if (!args.length) {
        args = 'any';
      }

      code += `${indent}/** Internal ${e.caption} ${subject} */${EOL}`;
      code += `${indent}private readonly _${e.exportedName}$ = new ${subject}<${args}>();${EOL}`;
      code += `${indent}/** ${e.caption} Observable */${EOL}`;
      code += `${indent}readonly ${e.exportedName}$ = this._${e.exportedName}$.asObservable();${EOL}${EOL}`;
    }

    return code;
  }

  generateConstructorCode() {
    let indent = indentation.L1;
    let indent2 = indentation.L2;
    let indent3 = indentation.L3;
    let indent4 = indentation.L4;

    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}`;
    code += `${indent}/* ${`${this.hub.caption} service constructor`.padEnd(80)} */${EOL}`;
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}${EOL}`;

    code += `${indent}/** ${this.hub.caption} service constructor */${EOL}`;
    code += `${indent}constructor(${EOL}`;
    code += `${indent2}private _config: ${this.configName}${EOL}`;
    code += `${indent}){${EOL}${EOL}`;

    code += `${indent2}/** Full Url and Path of SignalR Endpoint */${EOL}`;
    code += `${indent2}const fullUrl = (this._config.siggerUrl?.replace(/\\\/+$/, '') ?? '') + '/' +${EOL}`;
    code += `${indent4}((this._config.hubPath ?? this._config.defaultHubPath)?.replace(/^\\\/+/, '') ?? '');${EOL}${EOL}`;

    code += `${indent2}this._connection = new signalR.HubConnectionBuilder()${EOL}`;
    code += `${indent3}.withUrl(fullUrl, this._config.connectionConfig)${EOL}`;
    code += `${indent3}.withAutomaticReconnect()${EOL}`;
    code += `${indent3}// .configureLogging(signalR.LogLevel.Trace) /* for debug traces */${EOL}`;
    code += `${indent3}.build();${EOL}${EOL}`;

    const events = this.hub.events;
    if (events) {
      code += `${indent2}/** register events */${EOL}`;
      for (let evIdx = 0; evIdx < events.length; evIdx++) {
        const e = events[evIdx];
        code += `${indent2}${this.generateEventHandlerCode(e)}`;
      }
      code += `${EOL}${EOL}`;
    }

    code += `${indent2}/** connect to hub-server */${EOL}`;
    code += `${indent2}if (_config.autoConnect){${EOL}`;
    code += `${indent3}this.getConnection();${EOL}`;
    code += `${indent2}}${EOL}${EOL}`; // close constructor

    code += `${indent}}${EOL}${EOL}`; // close constructor
    return code;
  }

  generateEventHandlerCode(e) {
    let eventArgs = '';
    let invokeArgs = '';
    if (e.arguments.length) {
      const isNestedObject = e.arguments.length > 1;
      if (isNestedObject) invokeArgs += '{';

      for (let argIndex = 0; argIndex < e.arguments.length; argIndex++) {
        const arg = e.arguments[argIndex];
        if (argIndex > 0) {
          eventArgs += ', ';
          invokeArgs += ', ';
        }
        eventArgs += arg.exportedName;
        invokeArgs += arg.exportedName;
      }
      if (isNestedObject) invokeArgs += '}';
    }

    return `this._connection.on("${e.name}", (${eventArgs}) => this._${e.exportedName}$.next(${invokeArgs}));${EOL}`;
  }

  generateSignalRMethodsCode() {
    const methods = this.hub.methods;
    if (!methods?.length) return '';

    let indent = indentation.L1;
    let indent2 = indentation.L2;

    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}`;
    code += `${indent}/* ${'signalR invoke Methods'.padEnd(80)} */${EOL}`;
    code += `${indent}/* ${''.padStart(80, '-')} */${EOL}${EOL}`;

    for (let methodIndex = 0; methodIndex < methods.length; methodIndex++) {
      const method = methods[methodIndex];

      let argsDef = '';
      let argsCall = '';
      if (method.arguments) {
        for (let argIndex = 0; argIndex < method.arguments.length; argIndex++) {
          const arg = method.arguments[argIndex];
          if (argIndex > 0) {
            argsDef += ', ';
          }

          argsCall += ', ';

          let type = this._getTypeDeclaration(arg.type);
          argsDef += `${arg.exportedName}: ${type}`;
          argsCall += arg.exportedName;
        }
      }

      const retType = this._getTypeDeclaration(method.returnType);

      code += `${indent}/** ${method.name} */${EOL}`;
      code += `${indent}${method.exportedName}(${argsDef}) {${EOL}`;
      code += `${indent2}return this.invoke<${retType}>('${method.name}'${argsCall});${EOL}`;
      code += `${indent}}${EOL}${EOL}`; // close method
    }

    return code;
  }

  _getTypeDeclaration(typeDefinition) {
    let type = typeDefinition.exportedType;

    if (isDictionary(typeDefinition) && typeDefinition.dictionaryKey && typeDefinition.dictionaryValue) {
      let keyType = typeDefinition.dictionaryKey.exportedType;
      let valueType = typeDefinition.dictionaryValue.exportedType;
      if (isComplexOrEnum(typeDefinition.dictionaryKey)) keyType = 'models.' + keyType;
      if (isComplexOrEnum(typeDefinition.dictionaryValue)) valueType = 'models.' + valueType;

      if (isNullable(typeDefinition.flagsValue)) {
        return `{ [key: ${keyType}]: ${valueType} } | null`;
      }

      return `{ [key: ${keyType}]: ${valueType} }`;
    }

    if (isArray(typeDefinition) && isComplexOrEnum(typeDefinition.arrayElement)) {
      type = 'models.' + type;
    } else if (isComplexOrEnum(typeDefinition)) {
      type = 'models.' + type;
    }

    if (hasFlag(typeDefinition.flagsValue, TypeFlags.IsNullable)) {
      type += ' | null';
    }

    return type;
  }

  generateUtilMethodsCode() {
    let indent = indentation.L1;

    const code = `
/* -------------------------------------------------------------------------------- */
/* ImportPreviewHub Helper                                                          */
/* -------------------------------------------------------------------------------- */

/**
 *  get or create the connection and wait until the socket is in state connected
 */
private async getConnection() {
  /* already connected? */
  if (this.checkAndUpdateConnectedState()) {
    return this._connection;
  }
  if (this._connection.state === signalR.HubConnectionState.Disconnected ||
    this._connection.state === signalR.HubConnectionState.Disconnecting) {
    await this._connection.start();
  }

  const delay = 20;
  const timeout = 1500;
  let cnt = 0;
  return await new Promise<signalR.HubConnection>((resolve, reject) => {
    const waitInterval = setInterval(() => {
      if (this.checkAndUpdateConnectedState()) {
        clearInterval(waitInterval);
        resolve(this._connection);
      }
      cnt++;
      if (cnt * delay > timeout) {
        this.checkAndUpdateConnectedState();
        clearInterval(waitInterval);
        reject("Connection timeout: " + timeout + "ms");
      }
    }, delay);
  });
}

/** 
 * Async Invoker Helper 
 */
private async InvokeAsync<T>(method: string, ...args: any[]) {
  const connection = await this.getConnection();
  console.log("InvokeAsync: " + method);
  return await connection.invoke<T>(method, ...args);
}

/** 
 * return true if the socket is connected and updates the connected$ observable 
 */
private checkAndUpdateConnectedState() {
  const connected = this._connection.state === signalR.HubConnectionState.Connected;
  if (this._connected$.value !== connected) this._connected$.next(connected);
  return connected;
}

/**
 * Invoke helper (waits until a connection has been established)
 */
private invoke<T>(method: string, ...args: any[]): Observable<T> {
  return from(this.InvokeAsync<T>(method, ...args));
}
`;
    return code
      .split('\n')
      .map((l) => indent + l.replaceAll('\r', ''))
      .join(EOL);
  }

  async getServiceDirectory() {
    if (this.flags.test) return '';

    const fullPath = path.resolve(path.join(this.rootDirectory, this.output, this.hub.exportedName));
    const exists = fs.existsSync(fullPath);
    if (!exists) {
      this.verbose('create directory: ' + fullPath);
      const result = await fs.promises.mkdir(fullPath, { recursive: true });
      if (this.flags.verbose) console.log('Path created', fullPath);
    }
    return fullPath;
  }
}

class NgGeneration extends TsGeneration {
  constructor(rootDirectory, definition, output, flags) {
    super(rootDirectory, definition, output, flags);

    this.configParamsName = `SiggerConfigurationParams`;
    this.configParamsFile = `sigger.config`;
  }

  async generateGlobal() {
    const dir = await this.getServiceDirectory();
    this.verbose('generate angular configuration: ' + this.configParamsFile);

    const code = this.generateConfigParamsCode();
    this.writeToFile(dir, `${this.configParamsFile}.ts`, code);
  }

  async generate(hub) {
    const hubGenerator = new NgHubGeneration(this, hub);

    try {
      if (this.flags.rm) {
        await hubGenerator.cleanup();
      }

      if (hubGenerator.hasModels) {
        await hubGenerator.generateModels();
        await hubGenerator.generateEnums();
        await hubGenerator.generateModelsIndex();
      }

      await hubGenerator.generateModuleHubConfig();
      await hubGenerator.generateModule();
      await hubGenerator.generateService();

      await hubGenerator.generateModuleIndex();
      hubGenerator.verbose('generation completed');
    } catch (err) {
      hubGenerator.error('generation failed', this.rootDirectory, this.output, err);
    }
  }

  async getServiceDirectory() {
    if (this.flags.test) return '';

    const fullPath = path.resolve(path.join(this.rootDirectory, this.output));
    const exists = fs.existsSync(fullPath);
    if (!exists) {
      this.verbose('create directory: ' + fullPath);
      const result = await fs.promises.mkdir(fullPath, { recursive: true });
      if (this.flags.verbose) console.log('Path created', fullPath);
    }
    return fullPath;
  }

  generateConfigParamsCode() {
    let indent = indentation.L1;
    let code = '';
    code += `import { IHttpConnectionOptions } from '@microsoft/signalr';${EOL}${EOL}`;

    code += `/** Sigger Configuration */${EOL}`;
    code += `export interface ${this.configParamsName} {${EOL}`;
    code += `${indent}/**Root Url of SignalR endpoint; */${EOL}`;
    code += `${indent}siggerUrl: string;${EOL}${EOL}`;

    code += `${indent}/**Path of SignalR endpoint; */${EOL}`;
    code += `${indent}hubPath?: string;${EOL}${EOL}`;

    code += `${indent}/** Connect to server on startup. */${EOL}`;
    code += `${indent}autoConnect?: boolean;${EOL}`;

    code += `${indent}/** Url of Sigger endpoint; */${EOL}`;
    code += `${indent}connectionConfig: IHttpConnectionOptions;${EOL}${EOL}`;

    code += `}${EOL}`;
    return code;
  }
}
