import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-land-page',
  templateUrl: './land-page.component.html',
  styleUrls: ['./land-page.component.css']
})
export class LandPageComponent {
  title = 'eClinic';

  constructor(private router: Router) {}
  
  // LandPageComponent
goToDashboard(): void {
  console.log('Navegar para o Dashboard'); // Verifique se esta mensagem é registrada no console
  this.router.navigate(['/admin']);
}
}
