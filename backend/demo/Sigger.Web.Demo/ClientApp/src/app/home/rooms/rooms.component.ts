import { switchMap } from 'rxjs/operators';
import { Component, OnInit } from '@angular/core';
import { combineLatest, Subject, Observable } from 'rxjs';
import { ChatHub } from 'src/hubs/ChatHub';
import { ChatRoom, ChatRoomSubscription } from 'src/hubs/ChatHub/models';
import { ChatService } from '../chat.service';



@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html'
})
export class RoomsComponent implements OnInit {

  readonly currentRoom$: Observable<ChatRoom | null>;
  rooms$: Observable<ChatRoom[]>;

  constructor(private _chatService: ChatService) {
    this.currentRoom$ = this._chatService.currentRoom$;
    this.rooms$ = this._chatService.rooms$;
  }

  ngOnInit(): void {

  }

  createRoom(room: string): void {
    this._chatService.createChatRoom(room);
  }

  subscribeForRoom(roomId: string): void {
    this._chatService.enterChatRoom(roomId);
  }

  unsubscribeForRoom(roomId: string): void {
    this._chatService.leaveChatRoom(roomId);
  }


}
