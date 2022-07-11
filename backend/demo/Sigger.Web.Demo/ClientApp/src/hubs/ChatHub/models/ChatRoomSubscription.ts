import { User } from "./User";


/** ChatRoomSubscription */
/** generated from .net Type Sigger.Web.Demo.Hubs.ChatRoomSubscription */
export interface ChatRoomSubscription {
  
  /** Subscribed */
  subscribed: boolean;
  
  /** Uid */
  uid: string;
  
  /** Name */
  name: string | null;
  
  /** Members */
  members: User[] | null;
}

