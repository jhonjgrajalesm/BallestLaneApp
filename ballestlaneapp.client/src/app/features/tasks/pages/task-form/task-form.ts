import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import {
  CreateTaskRequest,
  TaskService,
  UpdateTaskRequest
} from '../../../../shared/services/task-service';

import { ApiResponse } from '../../../../shared/models/api-response';
import { TaskResponse } from '../../../../shared/models/task-response';

@Component({
  selector: 'app-task-form',
  standalone: false,
  templateUrl: './task-form.html',
  styleUrl: './task-form.css',
})
export class TaskForm implements OnInit {
  taskForm: FormGroup;
  taskId: string | null = null;
  isEditMode = false;
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  statuses = [
    { value: 1, label: 'Pending' },
    { value: 2, label: 'In Progress' },
    { value: 3, label: 'Completed' },
    { value: 4, label: 'Cancelled' }
  ];

  constructor(
    private formBuilder: FormBuilder,
    private taskService: TaskService,
    private route: ActivatedRoute,
    private router: Router,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.taskForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(150)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      dueDate: ['', [Validators.required]],
      status: [1, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.taskId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.taskId;

    if (this.isEditMode && this.taskId) {
      this.loadTask(this.taskId);
    }
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.isEditMode && this.taskId) {
      this.updateTask(this.taskId);
      return;
    }

    this.createTask();
  }

  

  cancel(): void {
    this.router.navigate(['/tasks']);
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.taskForm.get(controlName);

    return !!control &&
      control.hasError(errorName) &&
      (control.dirty || control.touched);
  }

  private loadTask(id: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getById(id).subscribe({
      next: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isLoading = false;

          if (!response.success || !response.data) {
            this.setErrorMessage(response);
            return;
          }

          this.taskForm.patchValue({
            title: response.data.title,
            description: response.data.description,
            dueDate: this.formatDateForInput(response.data.dueDate),
            status: response.data.status
          });

          this.changeDetectorRef.detectChanges();
        });
      },
      error: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isLoading = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  private createTask(): void {
    const request: CreateTaskRequest = {
      title: this.taskForm.value.title,
      description: this.taskForm.value.description,
      dueDate: this.taskForm.value.dueDate
    };

    this.taskService.createTask(request).subscribe({
      next: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;

          if (!response.success || !response.data) {
            this.setErrorMessage(response);
            return;
          }

          this.successMessage = 'Task created successfully.';
          this.changeDetectorRef.detectChanges();

          setTimeout(() => {
            this.router.navigate(['/tasks']);
          }, 600);
        });
      },
      error: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  private updateTask(id: string): void {
    const request: UpdateTaskRequest = {
      title: this.taskForm.value.title,
      description: this.taskForm.value.description,
      dueDate: this.taskForm.value.dueDate,
      status: Number(this.taskForm.value.status)
    };

    this.taskService.updateTask(id, request).subscribe({
      next: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;

          if (!response.success || !response.data) {
            this.setErrorMessage(response);
            return;
          }

          this.successMessage = 'Task updated successfully.';
          this.changeDetectorRef.detectChanges();

          setTimeout(() => {
            this.router.navigate(['/tasks']);
          }, 600);
        });
      },
      error: (response: ApiResponse<TaskResponse>) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  private setErrorMessage(response: any): void {
    this.errorMessage =
      response?.errors?.[0] ||
      response?.message ||
      response?.error?.errors?.[0] ||
      response?.error?.message ||
      'Something went wrong while saving the task.';

    this.changeDetectorRef.detectChanges();
  }

  private formatDateForInput(dateValue: string): string {
    if (!dateValue) {
      return '';
    }

    return dateValue.substring(0, 10);
  }
}
