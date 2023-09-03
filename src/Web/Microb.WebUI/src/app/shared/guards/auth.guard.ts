import { Injectable } from '@angular/core';
import {
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';

import { AccountService } from '../services';

@Injectable({ providedIn: 'root' })
export class AuthGuard {
  constructor(
    private _router: Router,
    private _accountService: AccountService
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const account = this._accountService.accountValue;
    if (account) {
      // check if route is restricted by role

      const roles = route.data['roles'];

      if (roles && !roles.includes(account.role)) {
        // role not authorized so redirect to home page
        this._router.navigate(['/']);
        return false;
      }

      // authorized so return true
      return true;
    }

    // not logged in so redirect to login page with the return url
    this._router.navigate(['/account/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }
}
