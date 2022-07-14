/**
 * Test if the Flag is set
 */
export function hasFlag(flags, testFlag) {
  return flags & testFlag;
}

/**
 * Test if the Flag is set
 */
export function hasAnyFlag(flags, ...testFlags) {
  if (flags.flagsValue !== undefined) return hasAnyFlag(flags.flagsValue, testFlags);

  const args = Array.prototype.slice.call(arguments, 1);
  for (let argIdx = 0; argIdx < args.length; argIdx++) {
    const arg = args[argIdx];
    if (hasFlag(flags, arg)) return true;
  }
  return false;
}

/** Returns true if the type model must be imported */
export function isPrimitive(flags) {
  return hasAnyFlag(flags, TypeFlags.IsPrimitive);
}

/** Returns true if the type is Nullable*/
export function isNullable(flags) {
  return hasAnyFlag(flags, TypeFlags.IsNullable);
}

/** Returns true if the type model must be imported */
export function isComplexOrEnum(flags) {
  return hasAnyFlag(flags, TypeFlags.IsComplex, TypeFlags.IsEnum);
}

/** Returns true if the type is a complex type*/
export function isComplex(flags) {
  return hasAnyFlag(flags, TypeFlags.IsComplex);
}

/** Returns true if the type is a void type*/
export function isVoid(flags) {
  return hasAnyFlag(flags, TypeFlags.IsVoid);
}

/** Returns true if the type is a text type*/
export function isText(flags) {
  return hasAnyFlag(flags, TypeFlags.IsString);
}

/** returns true if the type is an */
export function isArray(flags) {
  return hasAnyFlag(flags, TypeFlags.IsArray);
}

export const KeepValueMode = {
  /// <summary>
  /// Discard the value after notification
  /// </summary>
  FireAndForget: 0,

  /// <summary>
  /// Keep last value
  /// </summary>
  KeepLastValue: 1,

  /// <summary>
  /// Keep last value with initial Value
  /// </summary>
  KeepWithSeedValue: 2,
};

export const TypeFlags = {
  None: 0x00000,
  IsRoot: 0x00001,
  IsArray: 0x00002,
  IsByRef: 0x00004,
  IsTask: 0x00008,
  IsDictionary: 0x00010,
  IsNullable: 0x00100,
  IsNumber: 0x00200,
  IsString: 0x00400,
  IsBoolean: 0x00800,
  IsAny: 0x01000,
  IsEnum: 0x02000,
  IsComplex: 0x04000,
  IsPrimitive: 0x08000,
  IsBase64: 0x10000,
  IsVoid: 0x20000,
  IsParameter: 0x40000,
  IsReturn: 0x80000,
};