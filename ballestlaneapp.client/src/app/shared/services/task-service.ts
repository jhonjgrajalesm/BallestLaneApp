import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';

import { ApiResponse } from '../models/api-response';
import { TaskResponse } from '../models/task-response';

export interface CreateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
}

export interface UpdateTaskRequest {
  title: string;
  description: string;
  dueDate: string;
  status: number;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly apiUrl = '/api/tasks';

  constructor(private http: HttpClient) {
  }

  getMyTasks(): Observable<ApiResponse<TaskResponse[]>> {
    return this.http
      .get<ApiResponse<TaskResponse[]>>(this.apiUrl)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<TaskResponse[]>(error));
        })
      );
  }

  getById(id: string): Observable<ApiResponse<TaskResponse>> {
    return this.http
      .get<ApiResponse<TaskResponse>>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<TaskResponse>(error));
        })
      );
  }

  createTask(request: CreateTaskRequest): Observable<ApiResponse<TaskResponse>> {
    return this.http
      .post<ApiResponse<TaskResponse>>(this.apiUrl, request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<TaskResponse>(error));
        })
      );
  }

  updateTask(
    id: string,
    request: UpdateTaskRequest
  ): Observable<ApiResponse<TaskResponse>> {
    return this.http
      .put<ApiResponse<TaskResponse>>(`${this.apiUrl}/${id}`, request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<TaskResponse>(error));
        })
      );
  }

  deleteTask(id: string): Observable<ApiResponse<boolean>> {
    return this.http
      .delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          return throwError(() => this.mapHttpError<boolean>(error));
        })
      );
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
        return 'Invalid task information.';
      case 401:
        return 'Your session has expired. Please login again.';
      case 403:
        return 'You do not have permission to perform this action.';
      case 404:
        return 'The task was not found.';
      case 500:
        return 'Unexpected server error while processing the task.';
      default:
        return 'Something went wrong while processing the task.';
    }
  }
}
