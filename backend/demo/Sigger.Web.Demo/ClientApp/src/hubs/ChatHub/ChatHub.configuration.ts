import { Injectable } from '@angular/core';
import { HttpTransportType, ITransport } from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class ChatHubConfiguration {
  /** Url of ChatHub endpoint; */
  siggerUrl: string = '';

  /** Negotiation can only be skipped when the IHttpConnectionOptions.transport property is set to 'HttpTransportType.WebSockets'.; */
  skipNegotiation?: boolean = false;

  /** An HttpTransportType value specifying the transport to use for the connection. */
  transport?: HttpTransportType | ITransport = HttpTransportType.WebSockets;

  /** 
   * Default value is 'true'.
   * This controls whether credentials such as cookies are sent in cross-site requests.
   */
  withCredentials?: boolean;

  /** Connect to server on startup. */
  autoConnect?: boolean = true;
}

/** Configuration of ChatHub; */
export interface ChatHubConfigurationParams {
  /** Url of ChatHub endpoint; */
  siggerUrl: string;

  /** Negotiation can only be skipped when the IHttpConnectionOptions.transport property is set to 'HttpTransportType.WebSockets'.; */
  skipNegotiation?: boolean;

  /** An HttpTransportType value specifying the transport to use for the connection. */
  transport?: HttpTransportType | ITransport;

  /** 
   * Default value is 'true'.
   * This controls whether credentials such as cookies are sent in cross-site requests.
   */
  withCredentials?: boolean;

  /** Connect to server on startup. */
  autoConnect?: boolean;
}
