import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dash-land',
  templateUrl: './dash-land.component.html',
  styleUrls: ['./dash-land.component.css']
})
export class DashLandComponent implements OnInit {
  title = 'eClinic';

  constructor(private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token === null) {
      this.router.navigate(['/']);
    }
  }

  goTodashland(): void {
    this.router.navigate(['/bashboard']);
  }
}
