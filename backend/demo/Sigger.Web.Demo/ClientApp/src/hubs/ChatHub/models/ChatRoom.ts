import { User } from "./User";


/** ChatRoom */
/** generated from .net Type Sigger.Web.Demo.Hubs.ChatRoom */
export interface ChatRoom {
  
  /** Uid */
  uid: string;
  
  /** Name */
  name: string | null;
  
  /** Members */
  members: User[] | null;
}

