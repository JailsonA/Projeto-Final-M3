import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../api.service'; 

@Component({
  selector: 'app-dash-land',
  templateUrl: './dash-land.component.html',
  styleUrls: ['./dash-land.component.css']
})
export class DashLandComponent implements OnInit {
  title = 'eClinic';
  
  appoint = {
    doctorId: null,
    message: null
  };
  constructor(private router: Router, private apiService: ApiService) {}

  ngOnInit() {}

  async onSubmit(appoint: any) {
    try {
      debugger;
      const resp = this.appoint;
      const response = await this.apiService.CreateAppoint(appoint);
      if (resp) {
        console.log(resp);
      }
    } catch (error) {
      console.error('Ocorreu um erro:', error);
    }
  }
}
