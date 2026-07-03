import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AuthService } from '../../../../shared/services/auth-service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginForm: FormGroup;
  isSubmitting = false;
  errorMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.value).subscribe({
      next: response => {
        this.ngZone.run(() => {
          this.isSubmitting = false;

          if (!response.success || !response.data?.token) {
            this.setErrorMessage(response);
            return;
          }

          this.authService.saveToken(response.data.token);

          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');

          this.router.navigateByUrl(returnUrl || '/tasks');
        });
      },
      error: response => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.loginForm.get(controlName);

    return !!control &&
      control.hasError(errorName) &&
      (control.dirty || control.touched);
  }

  private setErrorMessage(response: any): void {
    this.errorMessage =
      response?.error?.errors?.[0] ||
      response?.error?.message ||
      response?.errors?.[0] ||
      response?.message ||
      'Login failed. Please check your credentials.';

    this.changeDetectorRef.detectChanges();
  }
}
