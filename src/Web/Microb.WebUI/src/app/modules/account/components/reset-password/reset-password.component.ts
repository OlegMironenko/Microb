import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { AccountService, MustMatch } from 'src/app/shared';

enum TokenStatus {
  Validating,
  Valid,
  Invalid,
}

@Component({ templateUrl: 'reset-password.component.html' })
export class ResetPasswordComponent implements OnInit {
  TokenStatus = TokenStatus;
  tokenStatus = TokenStatus.Validating;
  token?: string;
  form!: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    private _formBuilder: FormBuilder,
    private _activeRoute: ActivatedRoute,
    private _router: Router,
    private _accountService: AccountService
  ) {}

  ngOnInit() {
    this.form = this._formBuilder.group(
      {
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      {
        validator: MustMatch('password', 'confirmPassword'),
      }
    );

    const token = this._activeRoute.snapshot.queryParams['token'];

    // remove token from url to prevent http referer leakage
    this._router.navigate([], {
      relativeTo: this._activeRoute,
      replaceUrl: true,
    });

    this._accountService
      .validateResetToken(token)
      .pipe(first())
      .subscribe({
        next: () => {
          this.token = token;
          this.tokenStatus = TokenStatus.Valid;
        },
        error: () => {
          this.tokenStatus = TokenStatus.Invalid;
        },
      });
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.form.controls;
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this._accountService
      .resetPassword(
        this.token!,
        this.f['password'].value,
        this.f['confirmPassword'].value
      )
      .pipe(first())
      .subscribe({
        next: () => {
          console.log('Password reset successful, you can now login', {
            keepAfterRouteChange: true,
          });
          this._router.navigate(['../login'], {
            relativeTo: this._activeRoute,
          });
        },
        error: (error) => {
          console.error(error);
          this.loading = false;
        },
      });
  }
}
