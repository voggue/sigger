namespace Sigger.Generator;

public enum FormatKind
{
    TypeName = 0,
    MethodName = 1,
    TypeDeclaration = 2,
    ClassName = 3,
    PropertyName,
    EnumItemName,
    EnumItemValue,
    EnumName,
    HubName,
    MethodArgumentName
}

public enum TypeKind
{
    MemberName,
    TypeName
}