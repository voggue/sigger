import { UserRoleCatalog, UserRoleRecord } from './../../hubs/ChatHub/models/UserRole';
import { Pipe, PipeTransform } from '@angular/core';
import { User } from 'src/hubs/ChatHub/models';

@Pipe({
  name: 'formatUserRole'
})
export class FormatUserRolePipe implements PipeTransform {

  transform(user: User | null): string {
    if(!user) return UserRoleCatalog.GUEST.caption;
    return UserRoleRecord[user.role].caption;
  }

}
