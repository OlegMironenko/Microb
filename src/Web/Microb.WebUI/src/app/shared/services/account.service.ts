import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, finalize, map } from 'rxjs';
import { Account } from '../models';
import { Router } from '@angular/router';

const baseUrl = `${environment.apiUrl}/api/auth`;

@Injectable({ providedIn: 'root' })
export class AccountService {
  private _accountSubject: BehaviorSubject<Account | null>;
  public account: Observable<Account | null>;

  constructor(private _router: Router, private _httpClient: HttpClient) {
    this._accountSubject = new BehaviorSubject<Account | null>(null);
    this.account = this._accountSubject.asObservable();
  }

  public get accountValue() {
    return this._accountSubject.value;
  }

  login(email: string, password: string) {
    return this._httpClient
      .post<any>(
        `${baseUrl}/authenticate`,
        { email, password },
        { withCredentials: true }
      )
      .pipe(
        map((account) => {
          this._accountSubject.next(account);
          this.startRefreshTokenTimer();
          return account;
        })
      );
  }

  logout() {
    this._httpClient
      .post<any>(`${baseUrl}/revoke-token`, {}, { withCredentials: true })
      .subscribe();
    this.stopRefreshTokenTimer();
    this._accountSubject.next(null);
    this._router.navigate(['/account/login']);
  }

  refreshToken() {
    return this._httpClient
      .post<any>(`${baseUrl}/refresh-token`, {}, { withCredentials: true })
      .pipe(
        map((account) => {
          this._accountSubject.next(account);
          this.startRefreshTokenTimer();
          return account;
        })
      );
  }

  register(account: Account) {
    return this._httpClient.post(`${baseUrl}/register`, account);
  }

  verifyEmail(token: string) {
    return this._httpClient.post(`${baseUrl}/verify-email`, { token });
  }

  forgotPassword(email: string) {
    return this._httpClient.post(`${baseUrl}/forgot-password`, { email });
  }

  validateResetToken(token: string) {
    return this._httpClient.post(`${baseUrl}/validate-reset-token`, { token });
  }

  resetPassword(token: string, password: string, confirmPassword: string) {
    return this._httpClient.post(`${baseUrl}/reset-password`, {
      token,
      password,
      confirmPassword,
    });
  }

  getAll() {
    return this._httpClient.get<Account[]>(baseUrl);
  }

  getById(id: string) {
    return this._httpClient.get<Account>(`${baseUrl}/${id}`);
  }

  create(params: any) {
    return this._httpClient.post(baseUrl, params);
  }

  update(id: string, params: any) {
    return this._httpClient.put(`${baseUrl}/${id}`, params).pipe(
      map((account: any) => {
        // update the current account if it was updated
        if (account.id === this.accountValue?.id) {
          // publish updated account to subscribers
          account = { ...this.accountValue, ...account };
          this._accountSubject.next(account);
        }
        return account;
      })
    );
  }

  delete(id: string) {
    return this._httpClient.delete(`${baseUrl}/${id}`).pipe(
      finalize(() => {
        // auto logout if the logged in account was deleted
        if (id === this.accountValue?.id) this.logout();
      })
    );
  }

  // helper methods

  private refreshTokenTimeout?: any;

  private startRefreshTokenTimer() {
    // parse json object from base64 encoded jwt token
    const jwtBase64 = this.accountValue!.jwtToken!.split('.')[1];
    const jwtToken = JSON.parse(atob(jwtBase64));

    // set a timeout to refresh the token a minute before it expires
    const expires = new Date(jwtToken.exp * 1000);
    const timeout = expires.getTime() - Date.now() - 60 * 1000;
    this.refreshTokenTimeout = setTimeout(
      () => this.refreshToken().subscribe(),
      timeout
    );
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
