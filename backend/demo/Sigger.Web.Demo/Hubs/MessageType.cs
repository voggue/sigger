using System.ComponentModel.DataAnnotations;

namespace Sigger.Web.Demo.Hubs;

public enum MessageType
{
    [Display(Name = "Broadcast Message", Description = "A broadcast message")]
    Broadcast,

    [Display(Name = "Private Message", Description = "A private message from another user")]
    Private
}