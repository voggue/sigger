import { UserRole } from "./UserRole";


/** User */
/** generated from .net Type Sigger.Web.Demo.Hubs.User */
export interface User {
  
  /** Uid */
  uid: string;
  
  /** Name */
  name: string | null;
  
  /** Color */
  color: string | null;
  
  /** ImageLink */
  imageLink: string | null;
  
  /** Role */
  role: UserRole;
}

