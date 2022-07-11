using System.ComponentModel.DataAnnotations;

namespace Sigger.Web.Demo.Hubs;

public enum UserRole
{
    [Display(Name = "Unregistered user")] Guest,

    [Display(Name = "Registered user")] User,

    [Display(Name = "Administrator")] Admin
}