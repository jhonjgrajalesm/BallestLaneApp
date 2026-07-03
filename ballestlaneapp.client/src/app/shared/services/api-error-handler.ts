import { HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';

import { ApiResponse } from '../models/api-response';

export class ApiErrorHandler {
  static handle<T>(error: HttpErrorResponse) {
    let response: ApiResponse<T>;

    if (error.error && typeof error.error === 'object') {
      response = {
        success: error.error.success ?? false,
        message: error.error.message ?? ApiErrorHandler.getDefaultMessage(error.status),
        data: error.error.data ?? null,
        errors: error.error.errors ?? [ApiErrorHandler.getDefaultMessage(error.status)]
      };
    } else {
      response = {
        success: false,
        message: ApiErrorHandler.getDefaultMessage(error.status),
        data: null,
        errors: [ApiErrorHandler.getDefaultMessage(error.status)]
      };
    }

    return throwError(() => response);
  }

  private static getDefaultMessage(status: number): string {
    switch (status) {
      case 0:
        return 'Cannot connect to the server.';
      case 400:
        return 'Bad request.';
      case 401:
        return 'Email or password is incorrect.';
      case 403:
        return 'You do not have permission to perform this action.';
      case 404:
        return 'The requested resource was not found.';
      case 409:
        return 'A conflict occurred while processing the request.';
      case 500:
        return 'An unexpected server error occurred.';
      default:
        return 'Unexpected error.';
    }
  }
}
