import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import {ChatHubModule} from "../hubs/ChatHub";
import {HttpTransportType} from "@microsoft/signalr";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    ChatHubModule.forRoot({
      siggerUrl: 'https://localhost:7178/hubs/v1/chat',
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets
    })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
