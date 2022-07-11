import { Injectable, ɵAPP_ID_RANDOM_PROVIDER } from '@angular/core';
import { BehaviorSubject, Observable, ReplaySubject } from 'rxjs';
import { ChatHub } from 'src/hubs/ChatHub';
import { ChatRoom, UserRole, MessageType, User, ChatRoomSubscription } from 'src/hubs/ChatHub/models';

export interface Message {
  user: User | null;
  type: MessageType;
  room?: string | null;
  roomUid?: string | null;
  message: string | null
}

const SYSTEM_USER: User = { uid: "", color: "red", imageLink: null, name: "System", role: UserRole.ADMIN };
export const COLORS = ["#1abc9c", "#2ecc71", "#3498db", "#9b59b6", "#34495e", "#16a085", "#27ae60", "#2980b9", "#8e44ad", "#2c3e50", "#f1c40f", "#e67e22", "#e74c3c", "#ecf0f1", "#95a5a6", "#f39c12", "#d35400", "#c0392b", "#bdc3c7", "#7f8c8d"];

@Injectable()
export class ChatService {


  readonly error$: Observable<any>;

  private readonly _rooms$ = new ReplaySubject<ChatRoom[]>();
  readonly rooms$ = this._rooms$.asObservable();

  private readonly _messages$ = new BehaviorSubject<Message[]>([]);
  readonly messages$ = this._messages$.asObservable();

  private readonly _currentUser$ = new BehaviorSubject<User | null>(null);
  readonly currentUser$ = this._currentUser$.asObservable();

  private readonly _currentRoom$ = new BehaviorSubject<ChatRoom | null>(null);
  readonly currentRoom$ = this._currentRoom$.asObservable();

  constructor(private _chatHub: ChatHub) {
    this.error$ = this._chatHub.error$;

    this._chatHub.onUserLoggedIn$.subscribe(usr => this.addServiceMessage(usr, " logged in"));
    this._chatHub.onUserLoggedOut$.subscribe(usr => this.addServiceMessage(usr, " logged out"));

    this._chatHub.onUserEnteredChatRoom$.subscribe(subs => this.addServiceMessage(subs.user, ` entered room ${subs.room?.name}  (id:${subs.room?.uid?.slice(-5)})`));
    this._chatHub.onUserLeftChatRoom$.subscribe(subs => this.addServiceMessage(subs.user, ` left room ${subs.room?.name}  (id:${subs.room?.uid?.slice(-5)})`));

    this._chatHub.onMessageReceived$.subscribe(msg => this.addMessage(msg));
    this._chatHub.onChatRoomMessageReceived$.subscribe(msg => this.addRoomMessage(msg));

    this._chatHub.onChatRoomsChanged$.subscribe(() => this.reloadChatRooms());
  }

  login(userName: string, color: string) {
    this._chatHub.login(userName, color)
      .subscribe(usr => {
        this._currentUser$.next(usr);
        this.addServiceMessage(usr, " you are now logged in");
        this.reloadChatRooms();
      });
  }

  logout() {
    this._chatHub.logout().subscribe(
      () => {
        this._currentUser$.next(null);
        this.clearMessages();
      }
    )
  }


  send(message: string) {
    if (!this._currentUser$.value) {
      alert("User not logged in");
      return;
    }

    if (this._currentRoom$.value) {
      this._chatHub.sendMessageToChatRoom(this._currentRoom$.value.uid, message).subscribe();
    } else {
      this._chatHub.sendBroadcastMessage(message).subscribe();
    }
  }

  clearMessages() {
    this._messages$.next([]);
  }

  createChatRoom(room: string) {
    this._chatHub.createChatRoom(room)
      .subscribe(() => this.reloadChatRooms());
  }

  enterChatRoom(roomId: string) {
    this._chatHub.enterChatRoom(roomId)
      .subscribe(room => {
        this._currentRoom$.next(room);
        this.reloadChatRooms();
      });
  }

  leaveChatRoom(roomId: string) {
    this._chatHub.leaveChatRoom(roomId)
      .subscribe(() => {
        this._currentRoom$.next(null);
        this.reloadChatRooms();
      });
  }

  private addMessage(args: { user: User | null, type: MessageType, message: string | null }) {
    if (!args.message) return;
    this._messages$.value.push(args)
    this._messages$.next(this._messages$.value);
  }

  private addRoomMessage(args: { roomId: string, roomName: string | null, user: User | null, message: string | null }) {
    if (!args.message) return;
    this._messages$.value.push({
      room: args.roomName,
      roomUid: args.roomId,
      user: args.user,
      type: MessageType.PRIVATE,
      message: args.message
    });
    this._messages$.next(this._messages$.value);
  }

  private addServiceMessage(user: User | null, msg: string | null) {
    this._messages$.value.push({
      user: SYSTEM_USER,
      type: MessageType.PRIVATE,
      message: `User "${user?.name}" (id:${user?.uid?.slice(-3)}) ${msg}`
    });
    this._messages$.next(this._messages$.value);
  }

  private reloadChatRooms() {

    if (!this._currentUser$.value) {
      alert("User not logged in");
      return;
    }

    this._chatHub.getChatRooms()
      .subscribe((rooms) => {
        if (!rooms) {
          this._rooms$.next([]);
          return;
        }
        const usrId = this._currentUser$.value?.uid;
        const r = rooms.map(r => {
          const s: ChatRoomSubscription = {
            ...r,
            subscribed: r.members?.some(m => m.uid === usrId) ?? false
          };

          if (r.uid === this._currentRoom$.value?.uid) {
            this._currentRoom$.next(r);
          }

          return s;
        });
        this._rooms$.next(r);
      });
  }
}
