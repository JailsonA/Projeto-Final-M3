import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LandPageComponent } from './land-page/land-page.component';
import { DashLandModule } from './dash-land/dash-land.module'; // Certifique-se de importar o DashLandModule

@NgModule({
  declarations: [
    AppComponent,
    LandPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    DashLandModule, // Adicione o DashLandModule aqui
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
