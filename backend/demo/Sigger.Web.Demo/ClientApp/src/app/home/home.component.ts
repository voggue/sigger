import { BehaviorSubject, Observable } from 'rxjs';
import { ChatHub } from './../../hubs/ChatHub';
import { Component, OnInit } from '@angular/core';
import { User, MessageType, UserRole } from './../../hubs/ChatHub/models';
import { ChatService, COLORS, Message } from './chat.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  providers: [ChatService]
})
export class HomeComponent implements OnInit {

  messages$: Observable<Message[]>;
  currentUser$: Observable<User | null>;
  error$: Observable<any>;
  colors = COLORS;

  constructor(private _chatService: ChatService) {
    this.currentUser$ = _chatService.currentUser$;
    this.error$ = _chatService.error$;
    this.messages$ = _chatService.messages$;
  }

  ngOnInit(): void {
    this.login("Test dummy", COLORS[0]);
  }

  login(userName: string, color: string) {
    this._chatService.login(userName, color);
  }

  clear() {
    this._chatService.clearMessages();
  }

  logout() {
    this._chatService.logout();
  }

  send(message: string) {
    this._chatService.send(message);
  }



}
