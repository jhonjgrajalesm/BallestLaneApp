import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { TaskService } from '../../../../shared/services/task-service';
import { TaskResponse } from '../../../../shared/models/task-response';
import { ApiResponse } from '../../../../shared/models/api-response';

@Component({
  selector: 'app-task-list',
  standalone: false,
  templateUrl: './task-list.html',
  styleUrl: './task-list.css',
})
export class TaskList implements OnInit {
  tasks: TaskResponse[] = [];
  filteredTasks: TaskResponse[] = [];

  selectedStatus: number | null = null;

  isLoading = false;
  errorMessage = '';

  statuses = [
    { value: null, label: 'All' },
    { value: 1, label: 'Pending' },
    { value: 2, label: 'In Progress' },
    { value: 3, label: 'Completed' },
    { value: 4, label: 'Cancelled' }
  ];

  constructor(
    private taskService: TaskService,
    private router: Router,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.taskService.getMyTasks().subscribe({
      next: (response: ApiResponse<TaskResponse[]>) => {
        this.ngZone.run(() => {
          this.isLoading = false;

          if (!response.success || !response.data) {
            this.setErrorMessage(response);
            return;
          }

          this.tasks = response.data;
          this.applyStatusFilter();

          this.changeDetectorRef.detectChanges();
        });
      },
      error: (response: ApiResponse<TaskResponse[]>) => {
        this.ngZone.run(() => {
          this.isLoading = false;
          this.setErrorMessage(response);
        });
      }
    });
  }

  filterByStatus(status: number | null): void {
    this.selectedStatus = status;
    this.applyStatusFilter();
  }

  private applyStatusFilter(): void {
    if (this.selectedStatus === null) {
      this.filteredTasks = this.tasks;
      return;
    }

    this.filteredTasks = this.tasks.filter(
      task => Number(task.status) === Number(this.selectedStatus)
    );
  }

  createTask(): void {
    this.router.navigate(['/tasks/new']);
  }

  editTask(id: string): void {
    this.router.navigate(['/tasks/edit', id]);
  }

  deleteTask(id: string): void {
    const confirmed = confirm('Are you sure you want to delete this task?');

    if (!confirmed) {
      return;
    }

    this.taskService.deleteTask(id).subscribe({
      next: (response: ApiResponse<boolean>) => {
        this.ngZone.run(() => {
          if (!response.success) {
            this.setErrorMessage(response);
            return;
          }

          this.tasks = this.tasks.filter(task => task.id !== id);
          this.applyStatusFilter();

          this.changeDetectorRef.detectChanges();
        });
      },
      error: (response: ApiResponse<boolean>) => {
        this.ngZone.run(() => {
          this.setErrorMessage(response);
        });
      }
    });
  }

  getStatusLabel(status: number): string {
    switch (Number(status)) {
      case 1:
        return 'Pending';
      case 2:
        return 'In Progress';
      case 3:
        return 'Completed';
      case 4:
        return 'Cancelled';
      default:
        return 'Unknown';
    }
  }

  getStatusBadgeClass(status: number): string {
    switch (Number(status)) {
      case 1:
        return 'bg-secondary';
      case 2:
        return 'bg-primary';
      case 3:
        return 'bg-success';
      case 4:
        return 'bg-danger';
      default:
        return 'bg-dark';
    }
  }

  private setErrorMessage(response: any): void {
    this.errorMessage =
      response?.errors?.[0] ||
      response?.message ||
      response?.error?.errors?.[0] ||
      response?.error?.message ||
      'Something went wrong while loading tasks.';

    this.changeDetectorRef.detectChanges();
  }
}
