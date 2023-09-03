import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/shared';

@Component({ templateUrl: 'account.page.html' })
export class AccountPage {
  constructor(
    private _router: Router,
    private _accountService: AccountService
  ) {
    // redirect to home if already logged in
    if (this._accountService.accountValue) {
      this._router.navigate(['/']);
    }
  }
}
