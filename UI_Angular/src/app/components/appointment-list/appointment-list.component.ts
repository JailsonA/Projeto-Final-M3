import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../appointment.service';
import { Router } from '@angular/router';
import { interval } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';



@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css']
})
export class AppointmentListComponent implements OnInit {
  appointmentId: string = '';
  appointments: any[] = [];
  AppointId: number = 0; // Inicialize a propriedade AppointId
  userType: string = 'Patient'; // Aqui você deve buscar o tipo de usuário (pode ser 'Patient' ou 'Doctor')
  currentUserFullName: string = 'Current User'; // Aqui você deve busca



  constructor(private appointmentService: AppointmentService, private router: Router) { }

  
  ngOnInit(): void {
    this.checkToken();
    setInterval(() => {
      this.fetchAppointments();
    }, 500); // Atualiza a cada 1 segundo
  }
  

  checkToken(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found!');
      this.router.navigate(['/login']);  // Redirecionar para a página de login
    } else {
      // Se um token válido foi encontrado, busca os appointments.
      this.fetchAppointments();
    }
  }
  


  onSubmit(): void {
    if (this.appointmentId) {
        const appointmentIdNumber = Number(this.appointmentId); // Convertendo string para número

        if (isNaN(appointmentIdNumber)) {
            alert('Appointment ID must be a valid number.');
            return;
        }

        this.finishAppointment(appointmentIdNumber); // Passando o ID como número
    } else {
        alert('Appointment ID is required.');
    }
  }
  

  fetchAppointments(): void {
    this.appointmentService.getAppointmentsById().subscribe(
      (appointments: any[]) => {
        this.appointments = appointments;
        if (appointments.length > 0) {
            this.AppointId = appointments[appointments.length - 1].id;
        }
      },
      error => console.error('Error fetching appointments:', error)
    );
  }



  finishAppointment(appointmentId: number): void { 
 
 
    this.appointmentService.finishAppointment(appointmentId).subscribe(
      () => {
        alert('Appointment finalizado com sucesso!');
        this.fetchAppointments();
      },
      error => {
        if (error.message.includes('You dont have permition')) {
          alert('Você não tem permissão para finalizar este compromisso. Apenas doutores podem fazer isso.');
        } else {
          alert('Erro ao finalizar o compromisso: ' + error.message);
        }
      }
    );
  } 

}