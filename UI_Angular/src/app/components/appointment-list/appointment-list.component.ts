import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../appointment.service';

@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css']
})
export class AppointmentListComponent implements OnInit {
  
  appointments: any[] = [];
  AppointId: number = 0; // Inicialize a propriedade AppointId

  constructor(private appointmentService: AppointmentService) { }

  ngOnInit(): void {
    this.fetchAppointments();
  }

  fetchAppointments(): void {
    // Se você tem um método que busca todos os compromissos, use-o. Caso contrário, ajuste conforme necessário.
    this.appointmentService.getAppointmentsById().subscribe(
      (appointments: any[]) => { // Adicione o tipo aqui
        this.appointments = appointments;

        // Supondo que o retorno seja um array e você quer pegar o ID do último compromisso
        if (appointments.length > 0) {
            this.AppointId = appointments[appointments.length - 1].id;
        }
      },
      (error: any) => console.error('Error fetching appointments:', error) // Adicione o tipo aqui
    );
  }

}
