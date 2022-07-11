using System.ComponentModel.DataAnnotations;

namespace Sigger.Test.Parser.SharedModels;

public enum EnumOne
{
    [Display(Name = "Value 1", Order = 1, ShortName = "V1", Description = "Test of Enum-Value 1")]
    FirstEnumValue = 0,

    [Display(Name = "Value 2", Order = 5, ShortName = "V2", Description = "Test of Enum-Value 2")]
    SecondEnumValue = 8,

    [Display(Name = "Value 3", Order = 3, ShortName = "V3", Description = "Test of Enum-Value 3")]
    ThirdEnumValue = 16
}