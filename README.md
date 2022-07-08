# Sigger

Sigger is essentially a code generator that generates client code from a SignalR hub. 
A schema file is generated for the application, which then generates client code via an npm application.
The name is derived from swagger but for signalR.

## What is Sigger

>
> **Important: Sigger just supports .net6 or higher**
>

Sigger consists of two parts. 

 - One is the backend part, which is responsible for parsing the SignalR hubs and generating the schema 
   files and also provides some auxiliary functions for the SignalR interfaces.
   
 - The second part is the client, which is responsible for generating the client-side code. 
   This part will gradually be extended by various code generators.
 
 > Since I mainly use Angular, the first step only provides a generator for Angular clients. However, 
 > I will make sure that it can be adapted as easily as possible for other generators.
 
 ## Getting started
 

### Create an asp.net 6 App
First you need a **.net6** ASP.net application. E.g. https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-aspnet-core.

### Include the sigger nuget Package

```
TODO: nuget package is not currently deployed
```

### Create a Hub
Create a new file in your project e.g. `ChatHub`. 
I always prefer the directory structure `/hubs/hubname without hub/hubname.cs` for my projects.

```
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Web.GettingStarted.Hubs.Chat;

public interface IChatEvents
{
    Task OnMessageReceived(string user, string message);
}

public class ChatHub : Hub<IChatEvents>
{
    public async Task<bool> SendMessage(string message)
    {
        // Getting user works only with authentication
        var user = Context.User?.Identity?.Name ?? Context.UserIdentifier ?? Context.ConnectionId;
        await Clients.All.OnMessageReceived(user, message);
        return true;
    }
}
```


### Configure Sigger Services and Middleware
Add the Sigger Services to your Startup-Code (Program.cs or Startup.cs) 

```
using Sigger.Generator;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger();

var app = builder.Build();

app.UseSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);

app.Run();
```
or

```
using Sigger.Generator;
using Sigger.Web.GettingStarted.Hubs.Chat;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddSigger();
  }
  
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    app.UseSigger(o => o
      .WithHub<ChatHub>("/hubs/v1/chat")
  }
}
```


### Check your generated schema file

When you call up the url '<https://yourHost:yourPort>/sigger/sigger.json' in your browser, you can now view the schema file for your hub definition.

![image](https://user-images.githubusercontent.com/17086780/177941855-c141d279-b99d-4a56-be9d-aee14cc9fb7a.png)

