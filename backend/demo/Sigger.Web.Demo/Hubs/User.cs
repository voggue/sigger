namespace Sigger.Web.Demo.Hubs;

public class User
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
    public User(Guid uid, string name, string? color, string? imageLink, UserRole role)
    {
        Uid = uid;
        Name = name;
        Color = color;
        ImageLink = imageLink;
        Role = role;
    }

    public Guid Uid { get; }

    public string Name { get; }

    public string? Color { get; }

    public string? ImageLink { get; }

    public UserRole Role { get; }
}