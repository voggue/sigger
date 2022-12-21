using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Test.Generator;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class GenerationCoreTests
{
#pragma warning disable CS8618
    public class TestClasses
    {
        public interface IEventsDefinition
        {
            Task OnHandleDataChanged(string text, int value);
        }

        public class HubWithEnums : Hub
        {
            public Task ReceiveEnums(ClassWithEnums data)
            {
                return Task.CompletedTask;
            }
        }

        public class TestHub : Hub<IEventsDefinition>
        {
            public string Name { get; set; }

            public IEventsDefinition Events { get; set; }

            public Task<int> DoSomethingAsync(string[] args)
            {
                return Task.FromResult(-1);
            }
        }

        public class ComplexTestHub : Hub<IEventsDefinition>
        {
            public Task<IUnitReturnValue> DoSomethingComplexAsync(string[] args, UnitTestEnum failure, IEnumerable<int> numbers, string? nullableArg, DateTime? nullableStruct)
            {
                return Task.FromResult<IUnitReturnValue>(new UnitReturnValue("Hello from Unit test"));
            }
        }

        public class ClassWithEnums
        {
            public UnitTestEnum NonNullableEnum{ get; set; }
            
            public UnitTestEnum? NullableEnum{ get; set; }
            
            public int? NullableInt{ get; set; }
        }
        
        public interface IUnitReturnValue
        {
            public string Name { get; }
        }

        public class UnitReturnValue : IUnitReturnValue
        {
            public UnitReturnValue(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}