import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-data-display',
  templateUrl: './data-display.component.html',
  styleUrls: ['./data-display.component.css']
})
export class DataDisplayComponent implements OnInit {
  data: any;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getDataFromApi().subscribe(
      (result) => {
        this.data = result;
      },
      (error) => {
        console.error('Erro ao buscar dados da API:', error);
      }
    );
  }
}



