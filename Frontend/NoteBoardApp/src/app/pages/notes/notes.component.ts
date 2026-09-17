import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { AuthService } from '../../shared/services/auth.service';
import { NotesService } from './notes.service';
import { NoteColor } from './models/note-color';
import { NoteCreateInputModel } from './models/note-create-input-model';
import { NoteSetCompleteInputModel } from './models/note-set-complete-input-model';
import { NoteUpdateInputModel } from './models/nite-update-input-model';
import { NoteViewModel } from './models/note-view-model';

interface ColorOption {
  value: NoteColor;
  label: string;
  cssClass: string;
}

@Component({
  selector: 'noteboard-notes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css'
})
export class NotesComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly notesService = inject(NotesService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly notes = signal<NoteViewModel[]>([]);
  protected readonly loading = signal(false);
  protected readonly showForm = signal(false);
  protected readonly editingId = signal<number | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly colorOptions: ColorOption[] = [
    { value: NoteColor.Yellow, label: 'Yellow', cssClass: 'note--yellow' },
    { value: NoteColor.Green,  label: 'Green',  cssClass: 'note--green'  },
    { value: NoteColor.Red,    label: 'Red',    cssClass: 'note--red'    },
    { value: NoteColor.White,  label: 'White',  cssClass: 'note--white'  }
  ];

  protected readonly noteForm = this.formBuilder.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(50)]],
    content: ['', [Validators.required, Validators.maxLength(350)]],
    color: [NoteColor.Yellow, [Validators.required]]
  });

  ngOnInit(): void {
    this.loadNotes();
  }

  protected loadNotes(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.notesService.list().subscribe({
      next: (result) => {
        this.notes.set(result);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message ?? 'Could not load the notes.'
        );
      }
    });
  }

  protected openCreateForm(): void {
    this.editingId.set(null);
    this.noteForm.reset({ title: '', content: '', color: NoteColor.Yellow });
    this.showForm.set(true);
  }

  protected openEditForm(note: NoteViewModel): void {
    this.editingId.set(note.id);
    this.noteForm.setValue({
      title: note.title,
      content: note.content,
      color: note.color
    });
    this.showForm.set(true);
  }

  protected cancelForm(): void {
    this.showForm.set(false);
    this.editingId.set(null);
    this.errorMessage.set(null);
  }

  protected submit(): void {
    if (this.noteForm.invalid) {
      this.noteForm.markAllAsTouched();
      return;
    }

    const values = this.noteForm.getRawValue();
    const editingId = this.editingId();

    if (editingId === null) {
      const input: NoteCreateInputModel = {
        title: values.title,
        content: values.content,
        color: values.color
      };

      this.notesService.create(input).subscribe({
        next: (created) => {
          this.notes.update((list) => [...list, created]);
          this.cancelForm();
        },
        error: (err: HttpErrorResponse) => this.handleError(err)
      });
    } 
    else {
      const note = this.notes().find((n) => n.id === editingId);

      if (!note) {
        return;
      }

      const input: NoteUpdateInputModel = {
        id: editingId,
        title: values.title,
        content: values.content,
        color: values.color,
        updatedAt: note.updatedAt
      };

      this.notesService.update(input).subscribe({
        next: (updatedNote) => {
          this.notes.update((list) => list.map((n) => (n.id === updatedNote.id ? updatedNote : n)));
          this.cancelForm();
        },
        error: (err: HttpErrorResponse) => this.handleError(err)
      });
    }
  }

  protected toggleComplete(note: NoteViewModel): void {
    if (note.isCompleted) {
      return;
    }

    const input: NoteSetCompleteInputModel = {
      id: note.id,
      updatedAt: note.updatedAt
    };

    this.notesService.complete(input).subscribe({
      next: (updated) => {
        this.notes.update((list) => list.map((n) => (n.id === updated.id ? updated : n)));
      },
      error: (err: HttpErrorResponse) => this.handleError(err)
    });
  }

  protected deleteNote(note: NoteViewModel): void {
    if (!confirm(`Delete "${note.title}"?`)) {
      return;
    }

    this.notesService.delete(note.id).subscribe({
      next: () => {
        this.notes.update((list) => list.filter((n) => n.id !== note.id));
      },
      error: (err: HttpErrorResponse) => this.handleError(err)
    });
  }

  protected logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  protected colorClass(color: NoteColor): string {
    return (
      this.colorOptions.find((opt) => opt.value === color)?.cssClass ?? 'note--yellow'
    );
  }

  private handleError(err: HttpErrorResponse): void {
    this.errorMessage.set(err.error?.message ?? 'Something went wrong. Please try again.');
  }
}
