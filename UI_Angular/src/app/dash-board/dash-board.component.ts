import { Component } from '@angular/core';
import { Router } from '@angular/router';
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
  constructor(private router: Router) {}

  goToDashboard(): void {
    this.router.navigate(['/admin']);}

    onSubmit(user : any) {
      console.log(user);
    }
}
