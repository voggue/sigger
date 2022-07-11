using Sigger.Generator.Parser;

namespace Sigger.Generator;

/// <summary>
/// Definition of primitive type
/// </summary>
/// <param name="Nullable"></param>
/// <param name="Flags"></param>
public record PrimitiveTypeMetaInfo(bool Nullable, TypeFlags Flags);
