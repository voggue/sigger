import fs from 'fs';
import path from 'path';
import { TsGeneration, TsHubGeneration } from './sigger-ts-generator.js';
import { hasFlag, isArray, isComplexOrEnum, KeepValueMode, TypeFlags } from './sigger-enums.js';
import * as indentation from './sigger-utils.js';
import { getRootDirectory } from './sigger-utils.js';

export async function generate(definition, output, flags) {
  // const gen = new TsGeneration(definition, output, flags);
  // await gen.generate();

  const rootDirectory = await getRootDirectory();

  const generator = new NgGeneration(rootDirectory, definition, output, flags);

  for (let hubIdx = 0; hubIdx < definition.hubs.length; hubIdx++) {
    const hub = definition.hubs[hubIdx];
    generator.generate(hub);
  }
}

class NgHubGeneration extends TsHubGeneration {
  /** true if either a class or an enum is present */
  get hasModels() {
    return this.hub.definitions?.types?.length > 0 && this.hub.definitions?.enums?.length > 0;
  }

  constructor(ngGeneration, hub) {
    super(ngGeneration, hub);

    this.moduleName = `${this.hub.exportedName}Module`;
    this.configName = `${this.hub.exportedName}Configuration`;
    this.configParamsName = `${this.hub.exportedName}ConfigurationParams`;
  }

  /**
   * generate the index.ts file for the hub-module
   */
  async generateModuleIndex() {
    const dir = await this.getServiceDirectory();
    const file = `index.ts`;

    let code = '';

    code += `export * from './${this.hub.exportedName}.configuration';\n`;
    code += `export * from './${this.hub.exportedName}.module';\n`;
    code += `export * from './${this.hub.exportedName}.service';\n`;
    if (this.hasModels) code += `export * as models from './models';\n`;

    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the Service
   */
  async generateService() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.service.ts`;
    var code = this.generateServiceCode();
    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the configuration for module
   */
  async generateModuleHubConfig() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.configuration.ts`;
    var code = this.generateConfigCode();
    this.writeToFile(dir, file, code);
  }

  /**
   * Generate the hub module
   */
  async generateModule() {
    const dir = await this.getServiceDirectory();
    const file = `${this.hub.exportedName}.module.ts`;
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

    code += `import { ${this.configName}, ${this.configParamsName} } from './${this.hub.exportedName}.configuration';\n`;
    code += `import { ModuleWithProviders, NgModule } from '@angular/core';\n`;

    code += '\n';
    code += `@NgModule({\n`;
    code += `})\n`;
    code += `export class ${this.moduleName} {\n\n`;

    code += `${indent1}static forRoot(params: ${this.configParamsName}): ModuleWithProviders<${this.moduleName}> {\n`;
    code += `${indent2}return {\n`;
    code += `${indent3}ngModule: ${this.moduleName},\n`;
    code += `${indent3}providers: [\n`;
    code += `${indent4}{\n`;
    code += `${indent5}provide: ${this.configName},\n`;
    code += `${indent5}useValue: params\n`;
    code += `${indent4}}\n`; // close config provider object
    code += `${indent3}]\n`; // close provider array
    code += `${indent2}}\n`; // close return
    code += `${indent1}}\n`; // close forRoot
    code += `}\n`; // Close module
    return code;
  }

  generateConfigCode() {
    let indent = indentation.L1;
    let code = '';
    code += `import { Injectable } from '@angular/core';\n`;
    code += `import { HttpTransportType, ITransport } from '@microsoft/signalr';\n\n`;

    code += `@Injectable({ providedIn: 'root' })\n`;
    code += `export class ${this.configName} {\n`;
    code += `${indent}/** Url of ${this.hub.exportedName} endpoint; */\n`;
    code += `${indent}siggerUrl: string = '';\n\n`;

    code += `${indent}/** Negotiation can only be skipped when the IHttpConnectionOptions.transport property is set to 'HttpTransportType.WebSockets'.; */\n`;
    code += `${indent}skipNegotiation?: boolean = false;\n\n`;

    code += `${indent}/** An HttpTransportType value specifying the transport to use for the connection. */\n`;
    code += `${indent}transport?: HttpTransportType | ITransport = HttpTransportType.WebSockets;\n\n`;

    code += `${indent}/** \n`;
    code += `${indent} * Default value is 'true'.\n`;
    code += `${indent} * This controls whether credentials such as cookies are sent in cross-site requests.\n`;
    code += `${indent} */\n`;
    code += `${indent}withCredentials?: boolean;\n\n`;

    code += `${indent}/** Connect to server on startup. */\n`;
    code += `${indent}autoConnect?: boolean = true;\n`;

    code += `}\n\n`;

    code += `/** Configuration of ${this.hub.exportedName}; */\n`;
    code += `export interface ${this.configParamsName} {\n`;
    code += `${indent}/** Url of ${this.hub.exportedName} endpoint; */\n`;
    code += `${indent}siggerUrl: string;\n\n`;

    code += `${indent}/** Negotiation can only be skipped when the IHttpConnectionOptions.transport property is set to 'HttpTransportType.WebSockets'.; */\n`;
    code += `${indent}skipNegotiation?: boolean;\n\n`;

    code += `${indent}/** An HttpTransportType value specifying the transport to use for the connection. */\n`;
    code += `${indent}transport?: HttpTransportType | ITransport;\n\n`;

    code += `${indent}/** \n`;
    code += `${indent} * Default value is 'true'.\n`;
    code += `${indent} * This controls whether credentials such as cookies are sent in cross-site requests.\n`;
    code += `${indent} */\n`;
    code += `${indent}withCredentials?: boolean;\n\n`;

    code += `${indent}/** Connect to server on startup. */\n`;
    code += `${indent}autoConnect?: boolean;\n`;

    code += `}\n`;
    return code;
  }

  generateServiceCode() {
    let code = '';
    code += this.generateImportsCode();

    code += `@Injectable({ providedIn: 'root' })\n`;
    code += `export class ${this.hub.exportedName} {\n\n`;

    code += this.generateSignlRFieldsCode();
    code += this.generateEventsCode();
    code += this.generateConstructorCode();
    code += this.generateSignalRMethodsCode();
    code += this.generateUtilMethodsCode();

    code += `}\n`;
    return code;
  }

  generateImportsCode() {
    let code = '';

    code += `import * as signalR from '@microsoft/signalr';\n`;
    if (this.hasModels) code += `import * as models from './models';\n`;
    code += `import { ${this.configName} } from './${this.hub.exportedName}.configuration';\n`;
    code += `import { Injectable } from '@angular/core';\n`;
    code += `import { BehaviorSubject, ReplaySubject, from, Observable, of, Subject, throwError } from 'rxjs';\n`;
    code += `import { catchError, filter, switchMap, take } from 'rxjs/operators';\n\n\n`;
    return code;
  }

  generateSignlRFieldsCode() {
    let indent = indentation.L1;
    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */\n`;
    code += `${indent}/* ${'SignalR Fields'.padEnd(80)} */\n`;
    code += `${indent}/* ${''.padStart(80, '-')} */\n\n`;
    code += `${indent}/** signalR connection */\n`;
    code += `${indent}private readonly _connection: signalR.HubConnection;\n\n`;

    code += `${indent}/** Internal connection state subject */\n`;
    code += `${indent}private readonly _connected$ = new BehaviorSubject<boolean>(false);\n`;
    code += `${indent}/** Connection state observable */\n`;
    code += `${indent}readonly connected$ = this._connected$.asObservable();\n\n`;

    code += `${indent}/** Internal connection error subject */\n`;
    code += `${indent}private readonly _error$ = new Subject<any>();\n`;
    code += `${indent}/** Connection error observable */\n`;
    code += `${indent}readonly error$ = this._error$.asObservable();\n\n`;
    return code;
  }

  generateEventsCode() {
    let code = '';
    const events = this.hub.events;
    if (!events) return code;

    let indent = indentation.L1;
    code += `${indent}/* ${''.padStart(80, '-')} */\n`;
    code += `${indent}/* ${'SignalR Events'.padEnd(80)} */\n`;
    code += `${indent}/* ${''.padStart(80, '-')} */\n\n`;

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

      code += `${indent}/** Internal ${e.caption} ${subject} */\n`;
      code += `${indent}private readonly _${e.exportedName}$ = new ${subject}<${args}>();\n`;
      code += `${indent}/** ${e.caption} Observable */\n`;
      code += `${indent}readonly ${e.exportedName}$ = this._${e.exportedName}$.asObservable();\n\n`;
    }

    return code;
  }

  generateConstructorCode() {
    let indent = indentation.L1;
    let indent2 = indentation.L2;
    let indent3 = indentation.L3;

    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */\n`;
    code += `${indent}/* ${`${this.hub.caption} service constructor`.padEnd(80)} */\n`;
    code += `${indent}/* ${''.padStart(80, '-')} */\n\n`;

    code += `${indent}/** ${this.hub.caption} service constructor */\n`;
    code += `${indent}constructor(\n`;
    code += `${indent2}private _config: ${this.configName}\n`;
    code += `${indent}){\n\n`;

    code += `${indent2}const signalrOptions: signalR.IHttpConnectionOptions = {\n`;
    code += `${indent3}skipNegotiation: _config.skipNegotiation,\n`;
    code += `${indent3}transport: _config.transport,\n`;
    code += `${indent3}withCredentials: _config.withCredentials\n`;
    code += `${indent2}};\n\n`;

    code += `${indent2}this._connection = new signalR.HubConnectionBuilder()\n`;
    code += `${indent3}.withUrl(this._config.siggerUrl, signalrOptions)\n`;
    code += `${indent3}.withAutomaticReconnect()\n`;
    code += `${indent3}// .configureLogging(signalR.LogLevel.Trace) /* for debug traces */\n`;
    code += `${indent3}.build();\n\n`;

    const events = this.hub.events;
    if (events) {
      code += `${indent2}/** register events */\n`;
      for (let evIdx = 0; evIdx < events.length; evIdx++) {
        const e = events[evIdx];
        code += `${indent2}${this.generateEventHandlerCode(e)}`;
      }
      code += `\n\n`;
    }

    code += `${indent2}/** connect to hub-server */\n`;
    code += `${indent2}if (_config.autoConnect){\n`;
    code += `${indent3}this.tryConnect();\n`;
    code += `${indent2}}\n\n`; // close constructor

    code += `${indent}}\n\n`; // close constructor
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

    return `this._connection.on("${e.name}", (${eventArgs}) => this._${e.exportedName}$.next(${invokeArgs}));\n`;
  }

  generateSignalRMethodsCode() {
    const methods = this.hub.methods;
    if (!methods?.length) return;

    let indent = indentation.L1;
    let indent2 = indentation.L2;

    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */\n`;
    code += `${indent}/* ${'signalR invoke Methods'.padEnd(80)} */\n`;
    code += `${indent}/* ${''.padStart(80, '-')} */\n\n`;

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

      code += `${indent}/** ${method.name} */\n`;
      code += `${indent}${method.exportedName}(${argsDef}) {\n`;
      code += `${indent2}return this.invoke<${retType}>('${method.name}'${argsCall});\n`;
      code += `${indent}}\n\n`; // close method
    }

    return code;
  }

  _getTypeDeclaration(typeDefinition) {
    let type = typeDefinition.exportedType;

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
    let indent2 = indentation.L2;
    let indent3 = indentation.L3;
    let indent4 = indentation.L4;
    let indent5 = indentation.L5;
    let indent6 = indentation.L6;

    let code = '';
    code += `${indent}/* ${''.padStart(80, '-')} */\n`;
    code += `${indent}/* ${`${this.hub.caption} Helper`.padEnd(80)} */\n`;
    code += `${indent}/* ${''.padStart(80, '-')} */\n\n`;

    code += `${indent}/**\n`;
    code += `${indent} * connect to server\n`;
    code += `${indent} */\n`;
    code += `${indent}tryConnect(){\n\n`;

    code += `${indent2}if (this._connection.state === signalR.HubConnectionState.Connected) {\n`;
    code += `${indent3}if (!this._connected$.value) this._connected$.next(true);\n`;
    code += `${indent3}return;\n`;
    code += `${indent2}}\n\n`;

    code += `${indent2}if (this._connection.state === signalR.HubConnectionState.Connecting ||\n`;
    code += `${indent2}    this._connection.state === signalR.HubConnectionState.Reconnecting) {\n`;
    code += `${indent3}return;\n`;
    code += `${indent2}}\n\n`;

    code += `${indent2}if (this._connected$.value) this._connected$.next(false);\n\n`;

    code += `${indent2}/* helper to wait for a connection */\n`;
    code += `${indent2}const waitForConnection = () => {\n`;
    code += `${indent3}setTimeout(() => {\n`;
    code += `${indent4}if (this._connection && this._connection.state === signalR.HubConnectionState.Connected) {\n`;
    code += `${indent5}this._connected$.next(true);\n`;
    code += `${indent4}} else {\n`;
    code += `${indent5} waitForConnection();\n`;
    code += `${indent4}}\n`; // Close if connected
    code += `${indent3}}, 50);\n`; // Close setTimeout
    code += `${indent2}};\n\n`; // Close waitForConnection

    code += `${indent2}/* connect */\n`;
    code += `${indent2}try {\n`;
    code += `${indent3}from(this._connection.start())\n`;
    code += `${indent4}.subscribe(() => waitForConnection());\n`;
    code += `${indent2}} catch (err) {\n`;
    code += `${indent3}this._error$.next(err);\n`;
    code += `${indent2}}\n`; // close catch
    code += `${indent}}\n\n`; // Close tryConnect

    // Invoke helper
    code += `${indent}/**\n`;
    code += `${indent} * Invoke helper (waits until a connection has been established)\n`;
    code += `${indent} */\n`;
    code += `${indent}private invoke<T>(method: string, ...args: any[]): Observable<T> {\n\n`;

    code += `${indent2}/* helper to wait for a connection */\n`;
    code += `${indent2}const call = () => {\n`;
    code += `${indent3}if (this._connection) {\n`;
    code += `${indent4}const pr = this._connection.invoke<T>(method, ...args);\n`;
    code += `${indent4}return from(pr).pipe(\n`;
    code += `${indent5}take(1),\n`;
    code += `${indent5}// tap(m => this.onMessageReceived(method, m)),\n`;
    code += `${indent5}catchError(err => {\n`;
    code += `${indent6}this._error$.next(err);\n`;
    code += `${indent6}return throwError(err);\n`;
    code += `${indent5}})\n`; // Close catchError pipe
    code += `${indent4});\n`; // close pipe
    code += `${indent3}}\n\n`; // close if
    code += `${indent3}this._error$.next('IMPORT SOCKET: Invoke Error (Invalid Hub Connection)');\n`;
    code += `${indent3}throw new Error('invalid Hub Connection');\n`;
    code += `${indent2}};\n\n`; // close call helper

    code += `${indent2}/* Direct call when the connection is established */\n`;
    code += `${indent2}if (this._connection.state === signalR.HubConnectionState.Connected) {\n`;
    code += `${indent3}if (!this._connected$.value) this._connected$.next(true);\n`;
    code += `${indent3}return call();\n`;
    code += `${indent2}}\n\n`; // close if connected

    code += `${indent2}/* Wait until the connection is established before calling up */\n`;
    code += `${indent2}this.tryConnect();\n`;
    code += `${indent2}return this._connected$.pipe(\n`;
    code += `${indent3}filter(c => c),\n`;
    code += `${indent3}take(1),\n`;
    code += `${indent3}switchMap(() => call()),\n`;
    code += `${indent3}catchError(err => {\n`;
    code += `${indent4}this._error$.next(err);\n`;
    code += `${indent4}return throwError(err);\n`;
    code += `${indent3}})\n`; // Close catchError pipe
    code += `${indent2});\n`; // close pipe
    code += `${indent}}\n\n`; // Close invoker method

    return code;
  }

  async getServiceDirectory() {
    if (this.flags.test) return '';

    const fullPath = path.resolve(path.join(this.rootDirectory, this.output, this.hub.exportedName));
    const exists = fs.existsSync(fullPath);
    if (!exists) {
      if (this.flags.verbose) {
        console.log('Create directory: ' + fullPath);
      }
      const result = await fs.promises.mkdir(fullPath, { recursive: true });
      if (this.flags.verbose) console.log('Path created', fullPath);
    }
    return fullPath;
  }
}

class NgGeneration extends TsGeneration {
  constructor(rootDirectory, definition, output, flags) {
    super(rootDirectory, definition, output, flags);
  }

  async generate(hub) {
    const hubGenerator = new NgHubGeneration(this, hub);

    try {
      if (hubGenerator.hasModels) {
        await hubGenerator.generateModels();
        await hubGenerator.generateEnums();
        await hubGenerator.generateModelsIndex();
      }

      await hubGenerator.generateModuleHubConfig();
      await hubGenerator.generateModule();
      await hubGenerator.generateService();

      await hubGenerator.generateModuleIndex();
    } catch (err) {
      console.error('ERROR Generation details:', {
        rootDirectory: this.rootDirectory,
        definition: this.definition,
        output: this.output,
        flags: this.flags,
        hub,
        err,
      });
    }
  }
}
