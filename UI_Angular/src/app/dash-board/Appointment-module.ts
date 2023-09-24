// src/app/dash-board/appointment/appointment.module.ts

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppointmentCreateComponent } from '../components/appointment-create/appointment-create.component';
import { AppointmentListComponent } from '../components/appointment-list/appointment-list.component';
import { AppointmentEditComponent } from '../components/appointment-edit/appointment-edit.component';
import { AppointmentDeleteComponent } from '../components/appointment-delete/appointment-delete.component';

const routes: Routes = [
  { path: '', redirectTo: 'list', pathMatch: 'full' },
  { path: 'list', component: AppointmentListComponent },
  { path: 'create', component: AppointmentCreateComponent },
  { path: 'edit/:id', component: AppointmentEditComponent },
  { path: 'delete/:id', component: AppointmentDeleteComponent }
];

@NgModule({
  declarations: [
    AppointmentCreateComponent,
    AppointmentListComponent,
    AppointmentEditComponent,
    AppointmentDeleteComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    AppointmentListComponent
  ]
})
export class AppointmentModule { }
