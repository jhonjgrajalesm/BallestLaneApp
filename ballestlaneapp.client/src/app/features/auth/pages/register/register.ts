import { ChangeDetectorRef, Component, NgZone } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiResponse } from '../../../../shared/models/api-response';

import {  
  AuthResponse,
  AuthService
} from '../../../../shared/services/auth-service';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  registerForm: FormGroup;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.register(this.registerForm.value).subscribe({
      next: (response: ApiResponse<AuthResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;

          if (!response.success || !response.data?.token) {
            this.setErrorMessage(response);
            return;
          }

          this.authService.saveToken(response.data.token);

          this.successMessage = 'Account created successfully. Redirecting...';

          this.changeDetectorRef.detectChanges();

          setTimeout(() => {
            this.router.navigate(['/tasks']);
          }, 700);
        });
      },
      error: (response: ApiResponse<AuthResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.registerForm.get(controlName);

    return !!control &&
      control.hasError(errorName) &&
      (control.dirty || control.touched);
  }

  private setErrorMessage(response: any): void {
    this.errorMessage =
      response?.errors?.[0] ||
      response?.message ||
      response?.error?.errors?.[0] ||
      response?.error?.message ||
      'Registration failed. Please try again.';

    this.changeDetectorRef.detectChanges();
  }
}
