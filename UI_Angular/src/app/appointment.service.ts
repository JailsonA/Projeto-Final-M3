import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Doctor } from './doctor.model';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';



@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl_Appointment = 'http://localhost:5242/Appointments';
  private apiUrl_Users = 'http://localhost:5242/Users';
  private AppointId = null;


  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }


  
  createAppointment(doctorId: string, PatientMsg: string): Observable<any> {
    const params = new HttpParams()
        .set('doctorId', doctorId)
        .set('patientMessage', PatientMsg);
    
    return this.http.post(`${this.apiUrl_Appointment}/CreateAppointment`, {}, { headers: this.getHeaders(), params: params });
}

   getAppointmentsById(): Observable<any> {
    return this.http.post(`${this.apiUrl_Appointment}/GetAppointById`, { appointmentId: this.AppointId }, { headers: this.getHeaders() });
  }

  addMessageToAppointment(appointmentId: string, message: string): Observable<any> {
    return this.http.post(`${this.apiUrl_Appointment}/AddMessage`, { appointmentId, message }, { headers: this.getHeaders() });
  }

  getMessagesByAppointmentId(appointmentId: string): Observable<any> {
    return this.http.post(`${this.apiUrl_Appointment}/GetMessageByAppointId`, { appointmentId }, { headers: this.getHeaders() });
  }

  getDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.apiUrl_Users}/GetAllDoctor`, { headers: this.getHeaders() })
      .pipe(
        tap(doctors => console.log('Doctors received:', doctors)),
        catchError(error => {
          console.error('Error fetching doctors:', error);
          return throwError(error);
        })
      );
  }

  
  
  finishAppointment(appointmentId: number): Observable<any> {
    const params = new HttpParams()

      .set('appointmentId', appointmentId.toString());

    return this.http.post(`${this.apiUrl_Appointment}/FinishAppointment`, null, { headers: this.getHeaders(), params: params });
}


  uploadImage(image: File): Observable<any> {
    const formData = new FormData();
    formData.append('image', image);
    return this.http.post(`${this.apiUrl_Appointment}/upload`, formData);
  }
}

