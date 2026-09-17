import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response';
import { NoteCreateInputModel } from './models/note-create-input-model';
import { NoteSetCompleteInputModel } from './models/note-set-complete-input-model';
import { NoteUpdateInputModel } from './models/nite-update-input-model';
import { NoteViewModel } from './models/note-view-model';

@Injectable({ providedIn: 'root' })
export class NotesService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/notes`;

  list(): Observable<NoteViewModel[]> {
    return this.httpClient.get<ApiResponse<NoteViewModel[]>>(this.baseUrl)
      .pipe(map((response) => response.data ?? []));
  }

  create(input: NoteCreateInputModel): Observable<NoteViewModel> {
    return this.httpClient.post<ApiResponse<NoteViewModel>>(this.baseUrl, input)
      .pipe(map((response) => response.data as NoteViewModel));
  }

  update(input: NoteUpdateInputModel): Observable<NoteViewModel> {
    return this.httpClient.put<ApiResponse<NoteViewModel>>(`${this.baseUrl}/${input.id}`, input)
      .pipe(map((response) => response.data as NoteViewModel));
  }

  complete(input: NoteSetCompleteInputModel): Observable<NoteViewModel> {
    return this.httpClient.put<ApiResponse<NoteViewModel>>(`${this.baseUrl}/${input.id}/complete`, input)
      .pipe(map((response) => response.data as NoteViewModel));
  }

  delete(id: number): Observable<void> {
    return this.httpClient.delete<ApiResponse<void>>(`${this.baseUrl}/${id}`)
      .pipe(map(() => undefined));
  }
}
