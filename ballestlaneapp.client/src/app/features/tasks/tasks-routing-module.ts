import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TaskList } from './pages/task-list/task-list';
import { TaskForm } from './pages/task-form/task-form';

const routes: Routes = [
  {
    path: '',
    component: TaskList
  },
  {
    path: 'new',
    component: TaskForm
  },
  {
    path: 'edit/:id',
    component: TaskForm
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class TasksRoutingModule {
}
