import { Component } from '@angular/core';
import { Account, AccountService, Role } from './shared';

@Component({ selector: 'app-root', templateUrl: 'app.component.html' })
export class AppComponent {
  Role = Role;
  account?: Account | null;

  constructor(private _accountService: AccountService) {
    this._accountService.account.subscribe((x) => (this.account = x));
  }

  logout() {
    this._accountService.logout();
  }
}
