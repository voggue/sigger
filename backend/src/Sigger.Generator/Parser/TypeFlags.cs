namespace Sigger.Generator.Parser;

[Flags]
public enum TypeFlags
{
    None = 0x00000,
    IsRoot = 0x00001,
    IsArray = 0x00002,
    IsByRef = 0x00004,
    IsTask = 0x00008,
    IsDictionary = 0x00010,
    IsNullable = 0x00100,
    IsNumber = 0x00200,
    IsString = 0x00400,
    IsBoolean = 0x00800,
    IsAny = 0x01000,
    IsEnum = 0x02000,
    IsComplex = 0x04000,
    IsPrimitive = 0x08000,
    IsBase64 = 0x10000,
    IsVoid = 0x20000,
    IsParameter = 0x40000,
    IsReturn = 0x80000,
    
}