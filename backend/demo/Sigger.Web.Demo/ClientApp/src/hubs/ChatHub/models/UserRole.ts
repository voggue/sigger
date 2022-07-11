/** UserRole */
/** generated from .net Type Sigger.Web.Demo.Hubs.UserRole */
export enum UserRole {

  /** Unregistered user */
  GUEST = 0,

  /** Registered user */
  USER = 1,

  /** Administrator */
  ADMIN = 2
}



/** UserRole Catalog */
/** generated from .net Type Sigger.Web.Demo.Hubs.UserRole */
export const UserRoleCatalog = {
  
  /** Unregistered user */
  GUEST: {
    caption: "Unregistered user",
    value: UserRole.GUEST,
    intValue: 0
  },
  
  /** Registered user */
  USER: {
    caption: "Registered user",
    value: UserRole.USER,
    intValue: 1
  },
  
  /** Administrator */
  ADMIN: {
    caption: "Administrator",
    value: UserRole.ADMIN,
    intValue: 2
  }
}

/** UserRole Items List */
/** generated from .net Type Sigger.Web.Demo.Hubs.UserRole */
export const UserRoleList = Object.values(UserRoleCatalog);

/** UserRole Items Record */
/** generated from .net Type Sigger.Web.Demo.Hubs.UserRole */
export const UserRoleRecord : Record<UserRole, { caption: string, value: UserRole, intValue: number, description?: string | null }>
    = Object.assign({}, ...Object.values(UserRoleCatalog).map((x) => ({ [x.value]: x })));


