# Sigger-Client

## Abstract
Generates SignalR client from Sigger-Definition file. See https://github.com/voggue/sigger

## Install
```
npm i @microsoft/signalr --save
npm i sigger-gen --save
```

## Usage

### Add script
Extend the script section in your package.json
```
 "scripts": {
    ...
    "sigger": "sigger-gen https://localhost:7291/sigger/sigger.json ./src/hubs -v -f angular"
  },
```
where `https://localhost:7291/sigger/sigger.json` is the url of your sigger endpoint. and `./src/hubs`
is the destinition directory of the signalR client stub

> -f angular is optional because this is the default value

### Generate Client stub

Now you are able to call
```
# for npm 
npm run sigger

# for yarn
yarn sigger
```

This will generate the client stub files in the destination folder

### Configure the generated client stub in your app

In your `app.module.ts` add the generated hub (replace `ChatHub` with your gnerated Hub Module(s)).

```
import { ChatHubModule } from 'src/hubs/ChatHub';

@NgModule({
    imports: [
        ChatHubModule.forRoot({
            siggerUrl: 'https://localhost:7291/hubs/v1/chat' // Endpoint of SignalR Hub
        }),
    ]
})
export class AppModule { }
```

### Use the generated hub service

Create a new component e.g. HomeComponent

`home.component.ts`
```
private readonly _messages$ = new BehaviorSubject<Message[]>([]);
readonly messages$ = this._messages$.asObservable();

constructor(private _chatHub: ChatHub) {
    this._chatHub.onMessageReceived$.subscribe(msg => this.addMessage(msg));
}

private addMessage(args: { user: User | null, type: MessageType, message: string | null }) {
    if (!args.message) return;
    this._messages$.value.push(args)
    this._messages$.next(this._messages$.value);
}
```