import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { LoginInputModel } from '../models/login-input-model'; 
import { LoginViewModel } from '../models/login-view-model'; 

const ACCESS_TOKEN_KEY = 'noteboard.accessToken';
const REFRESH_TOKEN_KEY = 'noteboard.refreshToken';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/auth`;

  login(input: LoginInputModel): Observable<LoginViewModel> {
    return this.httpClient.post<ApiResponse<LoginViewModel>>(`${this.baseUrl}/login`, input)
      .pipe(
        map((response) => response.data as LoginViewModel),
        tap((credentials) => this.persist(credentials))
      );
  }

  refresh(refreshToken: string): Observable<LoginViewModel> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${refreshToken}`
    });

    return this.httpClient.post<ApiResponse<LoginViewModel>>(`${this.baseUrl}/refresh-token`, null, { headers })
      .pipe(
        map((response) => response.data as LoginViewModel),
        tap((credentials) => this.persist(credentials))
      );
  }

  isAuthenticated(): boolean {
    return !!this.accessToken;
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

  private persist(credentials: LoginViewModel): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, credentials.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, credentials.refreshToken);
  }
}
