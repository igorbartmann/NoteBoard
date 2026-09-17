import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response';
import { UserCreateInputModel } from './models/user-create-input-model';
import { UserViewModel } from './models/user-view-model';

@Injectable({ providedIn: 'root' })
export class RegisterService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/users`;

  register(input: UserCreateInputModel): Observable<UserViewModel> {
    return this.httpClient.post<ApiResponse<UserViewModel>>(this.baseUrl, input)
      .pipe(map((response) => response.data as UserViewModel));
  }
}
