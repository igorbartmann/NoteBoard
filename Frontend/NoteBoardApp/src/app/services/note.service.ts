import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../environments/environment';
import { ApiResponse } from '../shared/models/ApiResponse';
import { NoteCreateInputModel } from '../pages/notes/models/NoteCreateInputModel';
import { NoteSetCompleteInputModel } from '../pages/notes/models/NoteSetCompleteInputModel';
import { NoteUpdateInputModel } from '../pages/notes/models/NoteUpdateInputModel';
import { NoteViewModel } from '../pages/notes/models/NoteViewModel';

@Injectable({ providedIn: 'root' })
export class NoteService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/notes`;

  list(): Observable<NoteViewModel[]> {
    return this.http
      .get<ApiResponse<NoteViewModel[]>>(this.baseUrl)
      .pipe(map((response) => response.data ?? []));
  }

  create(input: NoteCreateInputModel): Observable<NoteViewModel> {
    return this.http
      .post<ApiResponse<NoteViewModel>>(this.baseUrl, input)
      .pipe(map((response) => response.data as NoteViewModel));
  }

  update(input: NoteUpdateInputModel): Observable<NoteViewModel> {
    return this.http
      .put<ApiResponse<NoteViewModel>>(`${this.baseUrl}/${input.id}`, input)
      .pipe(map((response) => response.data as NoteViewModel));
  }

  complete(input: NoteSetCompleteInputModel): Observable<NoteViewModel> {
    return this.http
      .put<ApiResponse<NoteViewModel>>(
        `${this.baseUrl}/${input.id}/complete`,
        input
      )
      .pipe(map((response) => response.data as NoteViewModel));
  }

  delete(id: number): Observable<void> {
    return this.http
      .delete<ApiResponse<void>>(`${this.baseUrl}/${id}`)
      .pipe(map(() => void 0));
  }
}
