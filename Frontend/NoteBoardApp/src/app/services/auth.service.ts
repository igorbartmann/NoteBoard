import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, tap } from 'rxjs';

import { environment } from '../../environments/environment';
import { ApiResponse } from '../shared/models/ApiResponse';
import { LoginInputModel } from '../pages/login/models/LoginInputModel';
import { LoginViewModel } from '../pages/login/models/LoginViewModel';

const ACCESS_TOKEN_KEY = 'noteboard.accessToken';
const REFRESH_TOKEN_KEY = 'noteboard.refreshToken';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  login(input: LoginInputModel): Observable<LoginViewModel> {
    return this.http
      .post<ApiResponse<LoginViewModel>>(`${this.baseUrl}/login`, input)
      .pipe(
        map((response) => response.data as LoginViewModel),
        tap((tokens) => this.persist(tokens))
      );
  }

  refresh(refreshToken: string): Observable<LoginViewModel> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${refreshToken}`
    });

    return this.http
      .post<ApiResponse<LoginViewModel>>(
        `${this.baseUrl}/refresh-token`,
        null,
        { headers }
      )
      .pipe(
        map((response) => response.data as LoginViewModel),
        tap((tokens) => this.persist(tokens))
      );
  }

  logout(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  }

  get accessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  get refreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    return !!this.accessToken;
  }

  private persist(tokens: LoginViewModel): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
  }
}
