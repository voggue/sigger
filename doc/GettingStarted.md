# Sigger getting started

In this gettingSTarted we will build a simple chat app using Sigger.
 - The server is an ASP.net core application.
 - The client will be an Angular clientI

## Server

### Create a new asp.net 6 app

First we need a **.net6** ASP.net application. 
E.g. https://docs.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-aspnet-core.

Include the Sigger Library into your project:

```bash
Install-Package Voggue.Sigger
Install-Package Voggue.Sigger.UI
```

### Implement the chat-hub
Create a new file in your project e.g. `ChatHub`.
I always prefer the directory structure `/hubs/hubname without hub/hubname.cs`.
for my projects. (In this case `/hubs/chat/chathub.cs`)

```csharp
using Microsoft.AspNetCore.SignalR;

namespace Sigger.Web.GettingStarted.Hubs.Chat;

public interface IChatEvents
{
    Task OnMessageReceived(Message message);
}

public class ChatHub : Hub<IChatEvents>
{
    public async Task<Message> SendMessage(string message)
    {
        // Getting user works only with authentication
        var user = Context.User?.Identity?.Name ?? Context.UserIdentifier ?? Context.ConnectionId;
        
        await Clients.Others.OnMessageReceived(new Message(DateTime.Now, user, message, false));
        return new Message(DateTime.Now, user, message, true);
    }
}

/// <summary>
/// A Chat Message Object
/// </summary>
/// <param name="Time">Timestamp</param>
/// <param name="User">UserInfo</param>
/// <param name="Content">Content of the message</param>
/// <param name="Sent">True if the message was sent, false if the message was received</param>
public record Message(DateTime Time, string User, string Content, bool Sent);
```

### Configure sigger services and middleware

Add the Sigger Services to your Startup-Code 
(Program.cs or Startup.cs) and configure the 
chat hub and the sigger and sigger ui middleware.

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSigger(o => o
    .WithHub<ChatHub>("/hubs/v1/chat")
);
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(c => c
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    ));

var app = builder.Build();
app.UseCors();
app.UseSigger();
app.UseSiggerUi();
app.Run();
```

### Run the server

That was it on the server.

We can now also test the server right away by using the 
`https://localhost:7178/sigger/`
(the port must of course be adapted to your used port).

In the ui we can now open the `OnMessageReceived` event and 
register for these events via the *subscribe* button.

Now we open a second browser window, open the method 
`SendMessage` and enter a test message as parameter, 
then we should see the message in the first browser window 
after pressing the *Send* button.

![image](https://user-images.githubusercontent.com/17086780/180666183-47593bb0-175b-4006-a274-e4122b5df82b.png)


If you don't want to use the Sigger UI, you can also check 
the functioning of the Sigger interface by entering the URL:
`https://localhost:7178/sigger/sigger.json`.

![image](https://user-images.githubusercontent.com/17086780/177941855-c141d279-b99d-4a56-be9d-aee14cc9fb7a.png)


## Client
In the next step we create an angular client to use the SignalR interface.

Open a terminal and navigate to the folder where you want to create the client.
Enter the following commands to create the Angular application.

> Note: A global installation of angular/cli is required to execute the `ng` commands.
> When creating the application you will have to answer some questions, but for this tutorial you can confirm all of them with the default.
> If you are not that experienced in creating angular projects I can recommend the `tour of heros` on the angular website.
> https://angular.io/tutorial/toh-pt0

```bash
> ng new your-ng-name
# ... wait a little eternity
> cd your-ng-name
> npm i @microsoft/signalr --save
> npm i sigger-gen --save-dev
```

Now we should be able to start the angular application
```bash
ng serve -o
```

### Add sigger generation script

add the following script to your **packages.json**, 
adapting the url to that of your asp.net debug server.

```json
 "scripts": {
  "sigger": "sigger-gen https://yourHost:yourPort/sigger/sigger.json ./src/hubs -v -f angular",
 }
```


store the file and execute following command in terminal:

```bash
> npm run sigger
```


If the command ran successfully, the ChatHub stub should 
now have been generated in the `./src/hubs` directory.

![image](https://user-images.githubusercontent.com/17086780/177948881-3edae479-fdab-41c6-a281-399c7169abaf.png)

### Configure the hub client


The ChatHub must now be informed of the endpoint with 
which it should communicate. In addition, for our first test, 
we do not yet have authentication,
which can also be ignored.

Open the `./src/app/app.module.ts` file and add 
following configuration in the imports section

```js
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

### Create a test page

In order to test our chat client, we open the file `./src/app/app.component.ts` 
and add the generated chat
service and create an observable to store the messages in a list.
Since we also want to send messages, we need a function to send them.


```typescript
import { Component } from '@angular/core';
import { ChatHub } from '../hubs/ChatHub';
import { BehaviorSubject } from 'rxjs';
import { Message } from 'src/hubs/ChatHub/models';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
})
export class AppComponent {
    title = 'sigger-getting-started';

    // never use subjects as public properties
    readonly messages$ = new BehaviorSubject<Message[]>([]);

    constructor(private _chatService: ChatHub) {
        _chatService.onMessageReceived$.subscribe((msg) => this.add(msg));
        this._chatService.tryConnect();
    }

    sendMessage(message: string) {
        if (message === 'clear') {
            this.messages$.next([]);
            return;
        }
        this._chatService.sendMessage(message).subscribe((msg) => this.add(msg));
    }

    private add(msg: Message | null) {
        if (msg?.user && msg?.content) {
            const m = this.messages$.value;
            m.push(msg);
            this.messages$.next([...m]);
        }
    }
}
```

### Add component styling

We take simple style classes from here: https://stackoverflow.com/questions/71154905/css-for-chat-room-speech-bubble-position

```css

.chat {
  --rad: 20px;
  --rad-sm: 3px;
  font: 16px/1.5 sans-serif;
  display: flex;
  flex-direction: column;
  padding: 20px;
  max-width: 500px;
  margin: auto;
}

.msg {
  position: relative;
  max-width: 75%;
  padding: 7px 15px;
  margin-bottom: 2px;
}

.msg.sent {
  border-radius: var(--rad) var(--rad-sm) var(--rad-sm) var(--rad);
  background: #42a5f5;
  color: #fff;
  /* moves it to the right */
  margin-left: auto;
}

.msg.rcvd {
  border-radius: var(--rad-sm) var(--rad) var(--rad) var(--rad-sm);
  background: #f1f1f1;
  color: #555;
  /* moves it to the left */
  margin-right: auto;
}

/* Improve radius for messages group */

.msg.sent:first-child,
.msg.rcvd+.msg.sent {
  border-top-right-radius: var(--rad);
}

.msg.rcvd:first-child,
.msg.sent+.msg.rcvd {
  border-top-left-radius: var(--rad);
}


/* time */

.msg::before {
  content: attr(data-time);
  font-size: 0.8rem;
  position: absolute;
  bottom: 100%;
  color: #888;
  white-space: nowrap;
  /* Hidden by default */
  display: none;
}

.msg.sent::before {
  right: 15px;
}

.msg.rcvd::before {
  left: 15px;
}


/* Show time only for first message in group */

.msg:first-child::before,
.msg.sent+.msg.rcvd::before,
.msg.rcvd+.msg.sent::before {
  /* Show only for first message in group */
  display: block;
}
```

### Create the html component page

Of course, we also need a user interface for our client. To do this, we open the
file `app.component.html` and replace the entire file with the following code:

```html
<div class="flex-column">
  <div class="chat">
    <div *ngFor="let m of messages$ | async" class="msg"
    [attr.data-time]="m.user + ' ' + (m.time | date: 'HH:mm:ss')"
    [class]="m.sent ? 'sent' : 'rcvd'">
      {{ m.content }}
  </div>
  <div class="flex-row">
    <input (keydown.enter)="sendMessage(inpMessage.value); inpMessage.value = ''" class="grow" type="text" #inpMessage>
    <button (click)="sendMessage(inpMessage.value); inpMessage.value = ''">send</button>
  </div>
</div>

```


#### Let's chat

now we should be able to chat with our client.

> I realize that it is very unattractive that instead of a 
> user in the chat, the connection ID is displayed. However, 
> this is accepted due to the simplicity for these first steps.

![image](https://user-images.githubusercontent.com/17086780/180666201-b5096df0-f35a-49b8-96c2-9da2de2c4d2f.png)
