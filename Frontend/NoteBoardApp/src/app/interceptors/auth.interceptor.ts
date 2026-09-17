import { HttpErrorResponse, HttpEvent, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, switchMap, throwError } from 'rxjs';

import { AuthService } from '../shared/services/auth.service';

function isAuthEndpoint(url: string): boolean {
  const authEndpoints = [
    '/auth/login', 
    '/auth/refresh-token'
  ];

  return authEndpoints.some((endpoint) => url.endsWith(endpoint));
}

function handleUnauthorized(
  originalRequest: HttpRequest<unknown>,
  next: HttpHandlerFn,
  auth: AuthService,
  router: Router): Observable<HttpEvent<unknown>> {
  const refreshToken = auth.refreshToken;

  if (!refreshToken) {
    auth.logout();
    router.navigate(['/login']);
    return throwError(() => new Error('Unauthorized'));
  }

  return auth.refresh(refreshToken)
    .pipe(
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

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  let outgoingReq = req;

  if (!isAuthEndpoint(req.url)) {
    const token = authService.accessToken;

    if (token) {
      outgoingReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    }
  }

  return next(outgoingReq)
    .pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !isAuthEndpoint(req.url)) {
          return handleUnauthorized(req, next, authService, router);
        }
        return throwError(() => error);
      })
    );
};
