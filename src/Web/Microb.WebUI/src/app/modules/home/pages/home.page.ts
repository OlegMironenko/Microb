import { Component } from '@angular/core';
import { AccountService } from 'src/app/shared';

@Component({ templateUrl: 'home.page.html' })
export class HomePage {
  account = this._accountService.accountValue;

  constructor(private _accountService: AccountService) {}
}
