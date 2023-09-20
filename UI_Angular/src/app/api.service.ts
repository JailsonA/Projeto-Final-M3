import { Injectable } from '@angular/core';
import axios from 'axios';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor() {}

  getDataFromApi(): Observable<any> {
    const apiUrl = 'http://localhost:5242/Clinic/GetAll';
    return new Observable((observer) => {
      axios
        .get(apiUrl)
        .then((response) => {
          observer.next(response.data);
          observer.complete();
        })
        .catch((error) => {
          observer.error(error);
        });
    });
  }
}
