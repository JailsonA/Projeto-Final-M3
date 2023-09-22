import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {DashBoardComponent} from './dash-board/dash-board.component';
import {DashLandComponent} from './dash-land/dash-land.component';

const routes: Routes = [
    { path: '', component: DashLandComponent },
    { path: 'login', component: DashBoardComponent },
    { path: 'dashboard', component: DashLandComponent },
    { path: 'login', loadChildren: () => import('./dash-board/dash-board.module').then(m => m.DashboardModule) },
  ];

@NgModule({
    declarations: [],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})

export class AppRoutingModule { }