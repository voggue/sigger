#pragma warning disable CS8618

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Sigger.Test.Parser.SharedModels;

namespace Sigger.Test.Parser;

public partial class ParserCoreTests
{
    public class HubBaseClass
    {
        public virtual Task<string> FunctionOfBaseClass(EnumOne enumArg, string textArg)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleHubWithOverload : HubBaseClass
    {
        public override Task<string> FunctionOfBaseClass(EnumOne enumArg, string textArg)
        {
            return Task.FromResult("Overridden");
        }

        public Task EmptyHubFunctionAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleHub : HubBaseClass
    {
        public int? TestField { get; set; }

        public Task EmptyHubFunctionAsync()
        {
            throw new NotImplementedException();
        }

        public double TestProperty { get; set; }

        public Task<SimpleClass?> MyFunc1Async(EnumOne[] enumArg1, string textArg1)
        {
            throw new NotImplementedException();
        }

        public void MyEmptyFunc()
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string?, Task<int>>> MyDictionaryFuncAsync()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class SimpleBaseClass
    {
        public List<KeyValuePair<int, ISimpleInterface>> PropertyOfBaseClass { get; set; }
    }

    public interface ISimpleInterface
    {
        public BindingFlags DotNetBindingFlags { get; set; }
    }

    [Display(Name = MODEL_NAME)]
    public class SimpleClass : SimpleBaseClass
    {
        public const string MODEL_NAME = "My Simple Model";

        public EnumOne TestEnums { get; set; }

        public DayOfWeek DotNetEnumType { get; set; }

        [Display(Name = "Nullable Text", Order = 5, ShortName = "Txt?", Description = "Test of nullable Text")]
        public string? NullableText { get; set; }

        [Display(Name = "Text", Order = 5, ShortName = "Txt", Description = "Test of not nullable Text")]
        public string? Text { get; set; }

        [Display(Name = "Array of Int", Order = 5, ShortName = "intArr", Description = "Test of int Array")]
        public int[] IntArray { get; } = Array.Empty<int>();


        [Display(Name = "List of bool", Order = 1, ShortName = "flagList", Description = "Test of boolean List")]
        public List<bool> FlagList { get; } = new();

        public Task<EnumOne> MyAsyncFunction(string? arg, int count, EnumOne defaultValue)
        {
            return arg == null
                ? Task.FromResult(EnumOne.FirstEnumValue)
                : Task.FromResult(count > 0 ? EnumOne.SecondEnumValue : defaultValue);
        }
    }
}