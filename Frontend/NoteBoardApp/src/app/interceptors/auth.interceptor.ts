import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, switchMap, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';

const AUTH_ENDPOINTS = ['/auth/login', '/auth/refresh-token'];

function isAuthEndpoint(url: string): boolean {
  return AUTH_ENDPOINTS.some((endpoint) => url.endsWith(endpoint));
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  let outgoingReq = req;
  if (!isAuthEndpoint(req.url)) {
    const token = auth.accessToken;
    if (token) {
      outgoingReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
  }

  return next(outgoingReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint(req.url)) {
        return handleUnauthorized(req, next, auth, router);
      }
      return throwError(() => error);
    })
  );
};

function handleUnauthorized(
  originalRequest: HttpRequest<unknown>,
  next: HttpHandlerFn,
  auth: AuthService,
  router: Router
): Observable<HttpEvent<unknown>> {
  const refreshToken = auth.refreshToken;

  if (!refreshToken) {
    auth.logout();
    router.navigate(['/login']);
    return throwError(() => new Error('Unauthorized'));
  }

  return auth.refresh(refreshToken).pipe(
    switchMap((tokens) => {
      const retryRequest = originalRequest.clone({
        setHeaders: { Authorization: `Bearer ${tokens.accessToken}` }
      });
      return next(retryRequest);
    }),
    catchError((refreshError) => {
      auth.logout();
      router.navigate(['/login']);
      return throwError(() => refreshError);
    })
  );
}
