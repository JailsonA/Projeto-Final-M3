// dash-board.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashBoardComponent } from './dash-board.component';
import { BrowserModule } from "@angular/platform-browser";


@NgModule({
  declarations: [DashBoardComponent],
  imports: [
    BrowserModule,
    FormsModule,
    BrowserAnimationsModule,
  ],
  providers: [],
  bootstrap: [DashboardModule]
})
export class DashboardModule {
}
