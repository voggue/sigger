# Sigger

Sigger is essentially a code generator that generates client code from a SignalR hub. 
A schema file is generated for the application, which then generates client code via an npm application.
The name is derived from swagger but for signalR.

## What is Sigger

Sigger consists of two parts. 

 - One is the backend part, which is responsible for parsing the SignalR hubs and generating the schema 
   files and also provides some auxiliary functions for the SignalR interfaces.
   
 - The second part is the client, which is responsible for generating the client-side code. 
   This part will gradually be extended by various code generators.
 
 > Since I mainly use Angular, the first step only provides a generator for Angular clients. However, 
 > I will make sure that it can be adapted as easily as possible for other generators.
 
 ## Getting started
 
 > **Important: Sigger just supports .net6 or higher**

### Create an asp.net 6 App
First you need a **.net6** ASP.net application. E.g. https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-aspnet-core.

### Include the sigger nuget Package

```
TODO: nuget package is not currently deployed
```

### Configure Sigger Services
Add the Sigger Services to your Startup-Code (Program.cs or Startup.cs) 

```
using Sigger.Generator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger();
```
or

```
using Sigger.Generator;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddSigger();
  }
}
```

### Configure Middleware
```
var app = builder.Build();
app.UseSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);

```
or

```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
  app.UseSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
}
```

### Create the Hub

```
public interface IChatEvents
{
    Task OnMessageReceived(string user, string message);
}

public class ChatHub : Hub<IChatEvents>
{
    public async Task<bool> SendMessage(string message)
    {
        // Works only with authentication
        var user = Context.User.Identity?.Name ?? Context.Current.ConnectionId;
        await Clients.All.OnMessageReceived(user, message);
        return true;
    }
}
```

### check the schema file

When you call up the url '<https://host:port>/sigger/sigger.json' in your browser, you can now view the schema file for your hub definition.
