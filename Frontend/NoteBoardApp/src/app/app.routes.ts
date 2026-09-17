import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((c) => c.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.component').then((c) => c.RegisterComponent)
  },
  {
    path: 'notes',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/notes/notes.component').then((c) => c.NotesComponent)
  },
  { path: '**', redirectTo: 'login' }
];
