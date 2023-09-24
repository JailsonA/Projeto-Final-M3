import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../api.service'; // Certifique-se de importar o serviço

@Component({
  selector: 'app-dash-board',
  templateUrl: './dash-board.component.html',
  styleUrls: ['./dash-board.component.css']
})
export class DashBoardComponent implements OnInit {
  
  ngOnInit(): void {
    // Verifique o token do localStorage aqui e redirecione conforme necessário
    if (localStorage.getItem('token') != null) {
      this.goToDashland(); // Corrigido para redirecionar para '/dashboard'
    }
  }
  
  title = 'eClinic';
  user = {
    email: null,
    password: null
  };

  // Injete o ApiService no construtor do componente
  constructor(private router: Router, private apiService: ApiService) {}

  goToDashboard(): void {
    this.router.navigate(['/dashboard']); // Corrigido para redirecionar para '/dashboard'
  }

  goToDashland(): void {
    this.router.navigate(['/appointments']);
  }

  async onSubmit(user: any) {
    try {
      const response = await this.apiService.login(user);
      if (response) {
        this.goToDashland();
      }
    } catch (error) {
      console.error('Ocorreu um erro:', error);
    }
  }
}
