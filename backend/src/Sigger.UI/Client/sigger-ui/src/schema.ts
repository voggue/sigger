//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.16.1.0 (NJsonSchema v10.7.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming



export interface ClassDefinition {
    clrType?: string | undefined;
    caption?: string | undefined;
    description?: string | undefined;
    order?: number | undefined;
    properties?: PropertyDefinition[] | undefined;
    exportedName?: string | undefined;
}

export interface PropertyDefinition {
    propertyType: TypeDefinition;
    exportedName: string;
    caption: string;
    description?: string | undefined;
    name: string;
    order?: number | undefined;
}

export interface TypeDefinition {
    exportedType?: string | undefined;
    flagsCaption?: string | undefined;
    flagsValue: number;
    caption?: string | undefined;
    description?: string | undefined;
    dictionaryKeyType?: TypeDefinition | undefined;
    dictionaryValueType?: TypeDefinition | undefined;
    arrayElementType?: TypeDefinition | undefined;
    genericTypes?: TypeDefinition[] | undefined;
}

export interface EnumDefinition {
    clrType?: string | undefined;
    caption?: string | undefined;
    exportedName?: string | undefined;
    description?: string | undefined;
    order?: number | undefined;
    items?: EnumItemDefinition[] | undefined;
}

export interface EnumItemDefinition {
    clrName?: string | undefined;
    caption?: string | undefined;
    description?: string | undefined;
    order?: number | undefined;
    intValue: number;
    stringValue?: string | undefined;
    exportedName?: string | undefined;
    exportedValue?: string | undefined;
}

export interface MethodDefinition {
    name?: string | undefined;
    exportedName?: string | undefined;
    caption?: string | undefined;
    description?: string | undefined;
    order?: number | undefined;
    arguments?: MethodArgumentDefinition[] | undefined;
    returnType?: TypeDefinition | undefined;
}

export interface EventDefinition extends MethodDefinition {
    keepValueMode: KeepValueMode;
}

export enum KeepValueMode {
    FireAndForget = "FireAndForget",
    KeepLastValue = "KeepLastValue",
    KeepWithSeedValue = "KeepWithSeedValue",
}

export interface MethodArgumentDefinition extends TypeDefinition {
    exportedName: string;
    clrName: string;
    order: number;
    type: TypeDefinition;
}

export interface HubDefinition {
    clrType?: string | undefined;
    caption?: string | undefined;
    path?: string | undefined;
    description?: string | undefined;
    name?: string | undefined;
    exportedName?: string | undefined;
    methods?: MethodDefinition[] | undefined;
    properties?: PropertyDefinition[] | undefined;
    events?: EventDefinition[] | undefined;
    definitions?: TypeDefinitions | undefined;
}

export interface TypeDefinitions {
    classDefinitions?: ClassDefinition[] | undefined;
    enumDefinitions?: EnumDefinition[] | undefined;
}

export interface SchemaInfo {
    title: string;
    description?: string | undefined;
    termsOfService?: string | undefined;
    version: string;
}

export interface SchemaDocument {
    schemaVersion: string;
    info: SchemaInfo;
    hubs: HubDefinition[];
}