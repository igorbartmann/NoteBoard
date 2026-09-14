import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../environments/environment';
import { ApiResponse } from '../shared/models/ApiResponse';
import { UserCreateInputModel } from '../pages/register/models/UserCreateInputModel';
import { UserViewModel } from '../pages/register/models/UserViewModel';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/users`;

  register(input: UserCreateInputModel): Observable<UserViewModel> {
    return this.http
      .post<ApiResponse<UserViewModel>>(this.baseUrl, input)
      .pipe(map((response) => response.data as UserViewModel));
  }
}
