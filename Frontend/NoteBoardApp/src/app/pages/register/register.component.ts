import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import { RegisterService } from './register.service';
import { UserCreateInputModel } from './models/user-create-input-model';

const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[.!@#$&*\-]).{1,20}$/;

function noWhiteSpaceEdges(control: AbstractControl): ValidationErrors | null {
  const value = control.value as string | null;

  if (!value) {
    return null;
  }

  return value.trim().length !== value.length 
    ? { whitespace: true } 
    : null;
}

@Component({
  selector: 'noteboard-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly registerService = inject(RegisterService);
  private readonly router = inject(Router);

  protected readonly registerForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
    password: ['', [Validators.required, Validators.maxLength(20), Validators.pattern(PASSWORD_PATTERN), noWhiteSpaceEdges]]
  });

  protected loading = false;
  protected errorMessage: string | null = null;
  protected successMessage: string | null = null;

  protected submit(): void {
    if (this.registerForm.invalid || this.loading) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const input: UserCreateInputModel = this.registerForm.getRawValue();
    this.loading = true;
    this.errorMessage = null;
    this.successMessage = null;

    this.registerService.register(input).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Account created. Redirecting to sign in...';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        this.errorMessage = err.error?.message ?? 'Could not create the account.';
      }
    });
  }
}
