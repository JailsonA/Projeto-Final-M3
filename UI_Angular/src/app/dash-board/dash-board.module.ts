// src/app/dash-board/dash-board.module.ts

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DashBoardComponent } from './dash-board.component';
import { AppointmentModule } from './Appointment-module';
// Adicione isto ao topo de dash-board.module.ts
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// E adicione o FormsModule


const routes: Routes = [
  { path: '', component: DashBoardComponent },
  { path: 'appointments', loadChildren: () => AppointmentModule }
];

@NgModule({
  declarations: [DashBoardComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    AppointmentModule, // Importe o AppointmentModule
    FormsModule,
    BrowserAnimationsModule
  ]
})
export class DashboardModule { }


