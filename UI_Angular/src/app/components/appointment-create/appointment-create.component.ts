// Imports
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AppointmentService } from '../../appointment.service';

// Interface Doctor
interface Doctor {
  user: {
    userId: number;
    fullName: string;
    // ... outras propriedades do usuário ...
  };
  // ... outras propriedades do médico ...
}

@Component({
  selector: 'app-appointment-create',
  templateUrl: './appointment-create.component.html',
  styleUrls: ['./appointment-create.component.css']
})


export class AppointmentCreateComponent implements OnInit {

  patientMessage: string = '';
  selectedDoctorUserId: number | null = null;
  doctors: Doctor[] = [];
  selectedDoctorFullName: string = '';

  constructor(private appointmentService: AppointmentService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadDoctors();
  }

  onDoctorSelected() {
    const selectedDoctor = this.doctors.find(doc => doc.user.userId === this.selectedDoctorUserId);
    if (selectedDoctor) {
      this.selectedDoctorFullName = selectedDoctor.user.fullName;
      console.log("Doctor selected:", this.selectedDoctorFullName);
    } else {
      console.log("No doctor found with the given ID.");
    }
  }

  

  loadDoctors(): void {
    this.appointmentService.getDoctors().subscribe(
      (doctorsList: Doctor[]) => {
        this.doctors = doctorsList;
        console.log('Doctors set in the component:', this.doctors);
      },
      error => console.error('Error fetching doctors:', error)
    );
  }

onSubmit(): void {
  console.log('Selected Doctor ID:', this.selectedDoctorUserId);
  console.log('Patient Message:', this.patientMessage);

  if (this.selectedDoctorUserId && this.patientMessage) {
    this.createApp(this.selectedDoctorUserId);
  } else {
    alert('Both doctor and patientMessage are required.');
  }
}


  createApp(doctorId: number): void {
    this.appointmentService.createAppointment(doctorId.toString(), this.patientMessage)
      .subscribe(
        () => {
          alert('Appointment created successfully!');
        },
        (error) => alert('Error creating appointment: ' + error.message)
      );
  }

}