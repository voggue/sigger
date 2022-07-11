/** MessageType */
/** generated from .net Type Sigger.Web.Demo.Hubs.MessageType */
export enum MessageType {

  /** Broadcast Message */
  BROADCAST = 0,

  /** Private Message */
  PRIVATE = 1
}



/** MessageType Catalog */
/** generated from .net Type Sigger.Web.Demo.Hubs.MessageType */
export const MessageTypeCatalog = {
  
  /** Broadcast Message */
  BROADCAST: {
    caption: "Broadcast Message",
    description: "A broadcast message",
    value: MessageType.BROADCAST,
    intValue: 0
  },
  
  /** Private Message */
  PRIVATE: {
    caption: "Private Message",
    description: "A private message from another user",
    value: MessageType.PRIVATE,
    intValue: 1
  }
}

/** MessageType Items List */
/** generated from .net Type Sigger.Web.Demo.Hubs.MessageType */
export const MessageTypeList = Object.values(MessageTypeCatalog);

/** MessageType Items Record */
/** generated from .net Type Sigger.Web.Demo.Hubs.MessageType */
export const MessageTypeRecord : Record<MessageType, { caption: string, value: MessageType, intValue: number, description?: string | null }>
    = Object.assign({}, ...Object.values(MessageTypeCatalog).map((x) => ({ [x.value]: x })));


