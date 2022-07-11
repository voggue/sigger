import { RoomsComponent } from './home/rooms/rooms.component';
import { FormatUserRolePipe } from './home/format-user-role.pipe';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { ChatHubModule } from 'src/hubs/ChatHub';
import { HttpTransportType } from '@microsoft/signalr';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    RoomsComponent,
    FormatUserRolePipe
  ],
  imports: [
    BrowserModule, // .withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    // FormsModule,
    // ApiAuthorizationModule,
    ChatHubModule.forRoot({
      siggerUrl: 'https://localhost:7291/hubs/v1/chat',
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets
    }),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    ])
  ],
  providers: [
    // { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
