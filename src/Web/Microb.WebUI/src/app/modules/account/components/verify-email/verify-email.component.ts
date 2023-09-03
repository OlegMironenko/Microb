import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';
import { AccountService } from 'src/app/shared';

enum EmailStatus {
  Verifying,
  Failed,
}

@Component({ templateUrl: 'verify-email.component.html' })
export class VerifyEmailComponent implements OnInit {
  EmailStatus = EmailStatus;
  emailStatus = EmailStatus.Verifying;

  constructor(
    private _activeRoute: ActivatedRoute,
    private _router: Router,
    private _accountService: AccountService
  ) {}

  ngOnInit() {
    const token = this._activeRoute.snapshot.queryParams['token'];

    // remove token from url to prevent http referer leakage
    this._router.navigate([], {
      relativeTo: this._activeRoute,
      replaceUrl: true,
    });

    this._accountService
      .verifyEmail(token)
      .pipe(first())
      .subscribe({
        next: () => {
          console.log('Verification successful, you can now login', {
            keepAfterRouteChange: true,
          });
          this._router.navigate(['../login'], {
            relativeTo: this._activeRoute,
          });
        },
        error: () => {
          this.emailStatus = EmailStatus.Failed;
        },
      });
  }
}
