# Sigger

Sigger is essentially a code generator that generates client code from a SignalR hub. 
A schema file is generated for the application, which then generates client code via an npm application.
The name is derived from swagger (see https://github.com/domaindrivendev/Swashbuckle.AspNetCore) but for signalR.

## What is Sigger

>
> **Important: Sigger just supports .net6 or higher**
>

Sigger consists of several parts: 

 - One is the backend part, which is responsible for parsing the SignalR hubs and generating the schema 
   files and also provides some auxiliary functions for the SignalR interfaces.
   
 - The second part is the client, which is responsible for generating the client-side code. 
   This part will gradually be extended by various code generators.
  
 - Sigger Extensions provide extension functions and services that are often required for Sigger applications. For example, a user-topic registry.

 - Sigger UI Provides an interface to test the Sigger interface. 
 
 > Since I mainly use Angular, the first step only provides a generator for Angular clients. However, 
 > I will make sure that it can be adapted as easily as possible for other generators.
 
 ## Getting started
 
### Create the server

#### Create an asp.net 6 App
First you need a **.net6** ASP.net application. E.g. https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-aspnet-core.

#### Include the sigger nuget Package

```
TODO: nuget package is not currently deployed
```

#### Create a Hub
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


#### Configure Sigger Services and Middleware
Add the Sigger Services to your Startup-Code (Program.cs or Startup.cs) 

```
using Sigger.Generator;
using Sigger.Web.GettingStarted.Hubs.Chat;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);

var app = builder.Build();

app.UseSigger();

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


#### Check your generated schema file

When you call up the url '<https://yourHost:yourPort>/sigger/sigger.json' in your browser, you can now view the schema file for your hub definition.

![image](https://user-images.githubusercontent.com/17086780/177941855-c141d279-b99d-4a56-be9d-aee14cc9fb7a.png)


### Create the client

#### Create the angular app
In the next step we create an angular client to use the SignalR interface.

Open a terminal and navigate to the folder where you want to create the client. 
Enter the following commands to create the Angular application. 

> Note: A global installation of angular/cli is required to execute the `ng` commands. 
> When creating the application you will have to answer some questions, but for this tutorial you can confirm all of them with the default. 
> If you are > not that experienced in creating angular projects I can recommend the `tour of heros` on the angular website.
> https://angular.io/tutorial/toh-pt0  

```
> ng new your-ng-name
# ... wait a little eternity
> cd your-ng-name
> npm i @microsoft/signalr --save
> npm i sigger-gen --save-dev
```
Now you should be able to start the angular application
```
ng serve -o
```

#### Generate the sigger client

add the following script to your **packages.json**, adapting the url to that of your asp.net debug server.

```
...
 "scripts": {
  "sigger": "sigger-gen https://yourHost:yourPort/sigger/sigger.json ./src/hubs -v -f angular",
 }
 ...
```

store the file and execute following command in terminal:

```
> npm run sigger
```

If the command ran successfully, the ChatHub stub should now have been generated in the `./src/hubs` directory.

![image](https://user-images.githubusercontent.com/17086780/177948881-3edae479-fdab-41c6-a281-399c7169abaf.png)

#### Configure the hub

The ChatHub must now be informed of the endpoint with which it should communicate. In addition, for our first test, we do not yet have authentication, 
which can also be ignored.

Open the `./src/app/app.module.ts` file and add folowing configuration in the imports section

```
imports: [
    ...
    ChatHubModule.forRoot({
      siggerUrl: 'https://yourHost:yourPort/hubs/v1/chat',
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets
    }),
   ]
```

 > Note: It is a very bad practice to write urls for communication directly into the modules. 
 > For a proper project, these should be defined in the environments and imported. 
 > Help and more details can be found on the https://angular.io/ page.

#### Create a test page

In order to test our chat client, we open the file `./src/app/app.component.ts` and add the generated chat 
service and create an observable to store the messages in a list.
Since we also want to send messages, we need a function to send them.

```
import {Component} from '@angular/core';
import {ChatHub} from '../hubs/ChatHub';
import {BehaviorSubject} from "rxjs";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'sigger-getting-started';

  // never use subjects as public properties
  readonly messages$ = new BehaviorSubject<{ user: string, message: string }[]>([]);

  constructor(private _chatService: ChatHub) {
    _chatService.onMessageReceived$.subscribe(x => {
      if (x.user && x.message)
        this.messages$.next([...this.messages$.value, {user: x.user, message: x.message}]);
    });
    
    // The tryConnect is not always necessary, as the hub establishes a connection on the 1st request. 
    // In this case, however, we need the connection before the first call in order to receive message events.
    this._chatService.tryConnect();
  }

  sendMessage(message: string) {
    // don't forget to subscribe
    this._chatService.sendMessage(message)
     .subscribe();
  }
}
```

#### Create a html page

Of course, we also need a user interface for our client. To do this, we open the 
file `app.component.html` and replace the entire file with the following code:

> Remark: for the sake of simplicity, i have refrained from styling the chat's speech bubbles into own and incoming.

```
<style>
  :host {
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol";
    font-size: 14px;
    color: #333;
    box-sizing: border-box;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }

  .flex-column {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
  }

  .flex-row {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
  }

  .grow {
    flex-grow: 1;
  }

  .messages {
    width: 100%;
    background-color: #fff;
    border: 1px solid #e5e5ea;
    border-radius: 0.25rem;
    display: flex;
    flex-direction: column;
    font-size: 1.25rem;
    margin: 0 auto 1rem;
    max-width: 600px;
    padding: 0.5rem 1.5rem;
  }

  .message p {
    width: 400px;
    border-radius: 1.15rem;
    line-height: 1.25;
    max-width: 75%;
    padding: 0.5rem .875rem;
    position: relative;
    word-wrap: break-word;

    align-self: flex-end;
    background-color: #248bf5;
    color: #fff;
  }

  .message p::before,
  .message p::after {
    bottom: -0.1rem;
    content: "";
    height: 1rem;
    position: absolute;
  }

  .message p::before {
    border-bottom-left-radius: 0.8rem 0.7rem;
    border-right: 1rem solid #248bf5;
    right: -0.35rem;
    transform: translate(0, -0.1rem);
  }

  .message p::after {
    background-color: #fff;
    border-bottom-left-radius: 0.5rem;
    right: -40px;
    transform: translate(-30px, -2px);
    width: 10px;
  }

  .user {
    display: block;
    font-size: 0.7rem;
    font-weight: 100;
    margin-bottom: 0.5rem;
  }


</style>

<div class="flex-column">
  <div class="message">
    <p *ngFor="let m of messages$ | async">
      {{ m.message }}
      <span class="user"> {{ m.user}}</span>
    </p>
  </div>
  <div class="flex-row">
    <input (keydown.enter)="sendMessage(inpMessage.value); inpMessage.value = ''" class="grow" type="text" #inpMessage>
    <button (click)="sendMessage(inpMessage.value); inpMessage.value = ''">send</button>
  </div>
</div>

```

#### Test the app

now u should be able to chat

![image](https://user-images.githubusercontent.com/17086780/177962575-bc7ce886-131d-4594-8396-b98ea821e917.png)


