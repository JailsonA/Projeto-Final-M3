import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {LandPageComponent} from './land-page/land-page.component';
import {DashBoardComponent} from './dash-board/dash-board.component';
import {DashLandComponent} from './dash-land/dash-land.component';

const routes: Routes = [
    { path: '', component: LandPageComponent },
    { path: 'admin', component: DashBoardComponent },
    { path: 'dashland', component: DashLandComponent },
    { path: 'admin', loadChildren: () => import('./dash-board/dash-board.module').then(m => m.DashboardModule) },
  ];

@NgModule({
    declarations: [],
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})

export class AppRoutingModule { }