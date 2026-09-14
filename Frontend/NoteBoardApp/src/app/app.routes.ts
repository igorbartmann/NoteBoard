import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register.component').then(
        (m) => m.RegisterComponent
      )
  },
  {
    path: 'notes',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/notes/notes.component').then((m) => m.NotesComponent)
  },
  { path: '**', redirectTo: 'login' }
];
