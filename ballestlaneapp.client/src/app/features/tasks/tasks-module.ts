import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { TasksRoutingModule } from './tasks-routing-module';

import { TaskList } from './pages/task-list/task-list';
import { TaskForm } from './pages/task-form/task-form';

@NgModule({
  declarations: [
    TaskList,
    TaskForm
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TasksRoutingModule
  ]
})
export class TasksModule {
}
