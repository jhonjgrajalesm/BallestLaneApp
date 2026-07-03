import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { ApiResponse } from '../models/api-response';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  userId: string;
  fullName: string;
  email: string;
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly tokenKey = 'ballestlane_token';
  private readonly apiUrl = '/api';

  constructor(private http: HttpClient) {
  }

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.apiUrl}/auth/login`, request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<AuthResponse>(error));
        })
      );
  }

  register(request: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.apiUrl}/auth/register`, request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<AuthResponse>(error));
        })
      );
  }

  saveToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  private mapHttpError<T>(error: HttpErrorResponse): ApiResponse<T> {
    if (error.error && typeof error.error === 'object') {
      return {
        success: error.error.success ?? false,
        message: error.error.message ?? this.getDefaultErrorMessage(error.status),
        data: error.error.data ?? null,
        errors: error.error.errors ?? [this.getDefaultErrorMessage(error.status)]
      };
    }

    return {
      success: false,
      message: this.getDefaultErrorMessage(error.status),
      data: null,
      errors: [this.getDefaultErrorMessage(error.status)]
    };
  }

  private getDefaultErrorMessage(status: number): string {
    switch (status) {
      case 0:
        return 'Cannot connect to the server.';
      case 400:
        return 'Invalid request.';
      case 401:
        return 'Unauthorized request.';
      case 403:
        return 'You do not have permission to perform this action.';
      case 404:
        return 'The requested resource was not found.';
      case 409:
        return 'The email is already registered.';
      case 500:
        return 'Unexpected server error.';
      default:
        return 'Something went wrong. Please try again.';
    }
  }
}
