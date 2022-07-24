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

  /** Internal OnMessageReceived ReplaySubject */
  private readonly _onMessageReceived$ = new ReplaySubject<models.Message | null>();
  /** OnMessageReceived Observable */
  readonly onMessageReceived$ = this._onMessageReceived$.asObservable();

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
    this._connection.on("OnMessageReceived", (message) => this._onMessageReceived$.next(message));


    /** connect to hub-server */
    if (_config.autoConnect){
      this.tryConnect();
    }

  }

  /* -------------------------------------------------------------------------------- */
  /* signalR invoke Methods                                                           */
  /* -------------------------------------------------------------------------------- */

  /** SendMessage */
  sendMessage(message: string | null) {
    return this.invoke<models.Message | null>('SendMessage', message);
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
