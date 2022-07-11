using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sigger.Test.Parser.SharedModels;

#pragma warning disable CS8618

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Global


namespace Sigger.Test.Parser;

public partial class ParserGenericTests
{
    public class HubForGenericTests
    {
        public int TestField { get; set; }

        public double TestProperty { get; set; }

        public Task<GenericClass?> MyFunc1Async(EnumOne arg1, string arg2)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SimpleClass
    {
        public string? NullableText { get; set; }

        public string? Text { get; set; }
    }

    public class GenericClass
    {
        public NestedGenericClass<NestedGenericClass<NestedGenericClass<string[]>>>? TripleNestedGenericProperty { get; set; }
    }

    public class NestedGenericClass<T>
    {
        public Task<IEnumerable<string?>>? Prop1 { get; set; }

        public Task<IDictionary<string, IEnumerable<SimpleClass[]>>?> Prop2 { get; set; }

        public Task<IEnumerable<Task<EnumOne?>>> Method1(IEnumerable<byte[]?>? arg1, params string[] argsParams)
        {
            throw new NotSupportedException();
        }

        public T? GenericType { get; set; }
    }
}