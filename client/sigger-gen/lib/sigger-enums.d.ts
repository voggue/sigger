type TypeFlagsLike = number | { flagsValue: number };

export function hasFlag(flags: number, testFlag: number): number;
export function hasAnyFlag(flags: TypeFlagsLike, ...testFlags: Array<number | number[]>): boolean;
export function isPrimitive(flags: TypeFlagsLike): boolean;
export function isNullable(flags: TypeFlagsLike): boolean;
export function isComplexOrEnum(flags: TypeFlagsLike): boolean;
export function isNumber(flags: TypeFlagsLike): boolean;
export function isBoolean(flags: TypeFlagsLike): boolean;
export function isComplex(flags: TypeFlagsLike): boolean;
export function isDictionary(flags: TypeFlagsLike): boolean;
export function isVoid(flags: TypeFlagsLike): boolean;
export function isText(flags: TypeFlagsLike): boolean;
export function isArray(flags: TypeFlagsLike): boolean;

export const KeepValueMode: Readonly<Record<string, number>>;
export const TypeFlags: Readonly<Record<string, number>>;
