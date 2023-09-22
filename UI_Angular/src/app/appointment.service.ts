import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = 'http://localhost:5242/Appointments'; // Base URL da sua API
  private AppointId = null;
  
  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('auth_token');
   // console.log("Token:", token); // imprime o token para verificar
    return new HttpHeaders({
      'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1RTZXJ2aWNlQWNjZXNzVG9rZW4iLCJqdGkiOiIzMWQwNzU5NS1kYzIzLTQ2MmMtYTU1ZS03NTJhMTQzNTMwNDkiLCJ1bmlxdWVfbmFtZSI6IndhZ25lci5oMkBob3RtYWlsLmNvbSIsIlVzZXJJZCI6IjEiLCJGdWxsTmFtZSI6InN0cmluZyIsIlVzZXJUeXBlIjoiUGF0aWVudCIsImV4cCI6MTY5NTI5NDEyOSwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.-TOLSOSSYlMjHkhWXSS3fTjL9nSL1muDsxDv_M9h7Bg` // token fixo para testes
    });
  }

  createAppointment(doctorId: string, patientMessage: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/CreateAppointment`, { doctorId, patientMessage }, { headers: this.getHeaders() });
  }

  getAppointmentsById(): Observable<any> {
    return this.http.post(`${this.apiUrl}/GetAppointById`, { appointmentId: this.AppointId }, { headers: this.getHeaders() });
  }

  addMessageToAppointment(appointmentId: string, message: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddMessage`, { appointmentId, message }, { headers: this.getHeaders() });
  }

  getMessagesByAppointmentId(appointmentId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/GetMessageByAppointId`, { appointmentId }, { headers: this.getHeaders() });
  }

  finishAppointment(appointmentId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/FinishAppointment`, { appointmentId }, { headers: this.getHeaders() });
  }

  uploadImage(image: File): Observable<any> {
    const formData = new FormData();
    formData.append('image', image);
    return this.http.post(`${this.apiUrl}/upload`, formData);
  }
}
