import * as signalR from '@microsoft/signalr';
import * as models from './models';
import { ChatHubConfiguration } from './ChatHub.configuration';
import { Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject, from, Observable, of, Subject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';


@Injectable({ providedIn: 'root' })
export class ChatHub {

  /* -------------------------------------------------------------------------------- */
  /* SignalR Fields                                                                   */
  /* -------------------------------------------------------------------------------- */

  /** signalR connection */
  private readonly _connection: signalR.HubConnection;

  /** Internal connection state subject */
  private readonly _connected$ = new BehaviorSubject<boolean>(false);
  /** Connection state observable */
  readonly connected$ = this._connected$.asObservable();

  /** Internal connection error subject */
  private readonly _error$ = new Subject<any>();
  /** Connection error observable */
  readonly error$ = this._error$.asObservable();

  /* -------------------------------------------------------------------------------- */
  /* SignalR Events                                                                   */
  /* -------------------------------------------------------------------------------- */

  /** Internal OnUserLoggedIn ReplaySubject */
  private readonly _onUserLoggedIn$ = new ReplaySubject<models.User | null>();
  /** OnUserLoggedIn Observable */
  readonly onUserLoggedIn$ = this._onUserLoggedIn$.asObservable();

  /** Internal OnUserLoggedOut ReplaySubject */
  private readonly _onUserLoggedOut$ = new ReplaySubject<models.User | null>();
  /** OnUserLoggedOut Observable */
  readonly onUserLoggedOut$ = this._onUserLoggedOut$.asObservable();

  /** Internal OnUserEnteredChatRoom ReplaySubject */
  private readonly _onUserEnteredChatRoom$ = new ReplaySubject<{room: models.ChatRoom | null, user: models.User | null}>();
  /** OnUserEnteredChatRoom Observable */
  readonly onUserEnteredChatRoom$ = this._onUserEnteredChatRoom$.asObservable();

  /** Internal OnUserLeftChatRoom ReplaySubject */
  private readonly _onUserLeftChatRoom$ = new ReplaySubject<{room: models.ChatRoom | null, user: models.User | null}>();
  /** OnUserLeftChatRoom Observable */
  readonly onUserLeftChatRoom$ = this._onUserLeftChatRoom$.asObservable();

  /** Internal OnChatRoomMessageReceived ReplaySubject */
  private readonly _onChatRoomMessageReceived$ = new ReplaySubject<{roomId: string, roomName: string | null, user: models.User | null, message: string | null}>();
  /** OnChatRoomMessageReceived Observable */
  readonly onChatRoomMessageReceived$ = this._onChatRoomMessageReceived$.asObservable();

  /** Internal OnMessageReceived ReplaySubject */
  private readonly _onMessageReceived$ = new ReplaySubject<{user: models.User | null, type: models.MessageType, message: string | null}>();
  /** OnMessageReceived Observable */
  readonly onMessageReceived$ = this._onMessageReceived$.asObservable();

  /** Internal OnChatRoomsChanged ReplaySubject */
  private readonly _onChatRoomsChanged$ = new ReplaySubject<string[] | null>();
  /** OnChatRoomsChanged Observable */
  readonly onChatRoomsChanged$ = this._onChatRoomsChanged$.asObservable();

  /* -------------------------------------------------------------------------------- */
  /* ChatHub service constructor                                                      */
  /* -------------------------------------------------------------------------------- */

  /** ChatHub service constructor */
  constructor(
    private _config: ChatHubConfiguration
  ){

    const signalrOptions: signalR.IHttpConnectionOptions = {
      skipNegotiation: _config.skipNegotiation,
      transport: _config.transport,
      withCredentials: _config.withCredentials
    };

    this._connection = new signalR.HubConnectionBuilder()
      .withUrl(this._config.siggerUrl, signalrOptions)
      .withAutomaticReconnect()
      // .configureLogging(signalR.LogLevel.Trace) /* for debug traces */
      .build();

    /** register events */
    this._connection.on("OnUserLoggedIn", (user) => this._onUserLoggedIn$.next(user));
    this._connection.on("OnUserLoggedOut", (user) => this._onUserLoggedOut$.next(user));
    this._connection.on("OnUserEnteredChatRoom", (room, user) => this._onUserEnteredChatRoom$.next({room, user}));
    this._connection.on("OnUserLeftChatRoom", (room, user) => this._onUserLeftChatRoom$.next({room, user}));
    this._connection.on("OnChatRoomMessageReceived", (roomId, roomName, user, message) => this._onChatRoomMessageReceived$.next({roomId, roomName, user, message}));
    this._connection.on("OnMessageReceived", (user, type, message) => this._onMessageReceived$.next({user, type, message}));
    this._connection.on("OnChatRoomsChanged", (chatRooms) => this._onChatRoomsChanged$.next(chatRooms));


    /** connect to hub-server */
    if (_config.autoConnect){
      this.tryConnect();
    }

  }

  /* -------------------------------------------------------------------------------- */
  /* signalR invoke Methods                                                           */
  /* -------------------------------------------------------------------------------- */

  /** SendMessageToChatRoom */
  sendMessageToChatRoom(chatRoomId: string, message: string | null) {
    return this.invoke<boolean>('SendMessageToChatRoom', chatRoomId, message);
  }

  /** SendBroadcastMessage */
  sendBroadcastMessage(message: string | null) {
    return this.invoke<boolean>('SendBroadcastMessage', message);
  }

  /** SendPrivateMessage */
  sendPrivateMessage(userId: string | null, message: string | null) {
    return this.invoke<boolean>('SendPrivateMessage', userId, message);
  }

  /** WhoAmI */
  whoAmI() {
    return this.invoke<models.User | null>('WhoAmI');
  }

  /** GetChatRooms */
  getChatRooms() {
    return this.invoke<models.ChatRoomSubscription[] | null>('GetChatRooms');
  }

  /** Login */
  login(userName: string | null, color: string | null) {
    return this.invoke<models.User | null>('Login', userName, color);
  }

  /** Logout */
  logout() {
    return this.invoke<boolean>('Logout');
  }

  /** CreateChatRoom */
  createChatRoom(roomName: string | null) {
    return this.invoke<models.ChatRoomSubscription | null>('CreateChatRoom', roomName);
  }

  /** EnterChatRoom */
  enterChatRoom(uid: string) {
    return this.invoke<models.ChatRoom | null>('EnterChatRoom', uid);
  }

  /** LeaveChatRoom */
  leaveChatRoom(roomId: string) {
    return this.invoke<boolean>('LeaveChatRoom', roomId);
  }

  /* -------------------------------------------------------------------------------- */
  /* ChatHub Helper                                                                   */
  /* -------------------------------------------------------------------------------- */

  /**
   * connect to server
   */
  tryConnect(){

    if (this._connection.state === signalR.HubConnectionState.Connected) {
      if (!this._connected$.value) this._connected$.next(true);
      return;
    }

    if (this._connection.state === signalR.HubConnectionState.Connecting ||
        this._connection.state === signalR.HubConnectionState.Reconnecting) {
      return;
    }

    if (this._connected$.value) this._connected$.next(false);

    /* helper to wait for a connection */
    const waitForConnection = () => {
      setTimeout(() => {
        if (this._connection && this._connection.state === signalR.HubConnectionState.Connected) {
          this._connected$.next(true);
        } else {
           waitForConnection();
        }
      }, 50);
    };

    /* connect */
    try {
      from(this._connection.start())
        .subscribe(() => waitForConnection());
    } catch (err) {
      this._error$.next(err);
    }
  }

  /**
   * Invoke helper (waits until a connection has been established)
   */
  private invoke<T>(method: string, ...args: any[]): Observable<T> {

    /* helper to wait for a connection */
    const call = () => {
      if (this._connection) {
        const pr = this._connection.invoke<T>(method, ...args);
        return from(pr).pipe(
          take(1),
          // tap(m => this.onMessageReceived(method, m)),
          catchError(err => {
            this._error$.next(err);
            return throwError(err);
          })
        );
      }

      this._error$.next('IMPORT SOCKET: Invoke Error (Invalid Hub Connection)');
      throw new Error('invalid Hub Connection');
    };

    /* Direct call when the connection is established */
    if (this._connection.state === signalR.HubConnectionState.Connected) {
      if (!this._connected$.value) this._connected$.next(true);
      return call();
    }

    /* Wait until the connection is established before calling up */
    this.tryConnect();
    return this._connected$.pipe(
      filter(c => c),
      take(1),
      switchMap(() => call()),
      catchError(err => {
        this._error$.next(err);
        return throwError(err);
      })
    );
  }

}
