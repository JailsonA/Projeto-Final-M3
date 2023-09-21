import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../api.service'; // Certifique-se de importar o serviço

@Component({
  selector: 'app-dash-board',
  templateUrl: './dash-board.component.html',
  styleUrls: ['./dash-board.component.css']
})
export class DashBoardComponent {
  title = 'eClinic';
  user = {
    email: null,
    password: null
  };

  // Injete o ApiService no construtor do componente
  constructor(private router: Router, private apiService: ApiService) {}

  goToDashboard(): void {
    this.router.navigate(['/admin']);
  }

  goToDashland(): void {
    this.router.navigate(['/dashland']);
  }

  async onSubmit(user: any) {
    try {
      const response = await this.apiService.login(user); // Use this.apiService para chamar o serviço
      if (response) {
        this.goToDashland();
      }
      //console.log(response);
      //localStorage.setItem('token', response);
    } catch (error) {
      console.error('Ocorreu um erro:', error);
    }
  }
}
