import { Injectable } from '@angular/core';
import axios from 'axios';
import { AxiosRequestHeaders, InternalAxiosRequestConfig } from 'axios';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private axiosInstance = axios.create({
    baseURL: 'http://localhost:5242'
  });

  constructor() {
    this.axiosInstance.interceptors.request.use((config: InternalAxiosRequestConfig<any>) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers = {
          ...config.headers,
          Authorization: `Bearer ${token}`
        } as AxiosRequestHeaders;
      }
      return config;
    });
  }

  /* async login(user: any): Promise<any> {
    try {
      const apiUrl = '/Login/Login'; // Use o caminho relativo em relação à baseURL
      const response = await this.axiosInstance.post(apiUrl, user); // Use this.axiosInstance
      return response.data;
    } catch (error) {
      throw error;
    }
  } */


  async login(user: any): Promise<any> {
    try {
      const apiUrl = '/Login/Login';
      const response = await this.axiosInstance.post(apiUrl, user);
      console.log('Response:', response);
      localStorage.setItem('token', response.data);
      return response.data;
    } catch (error) {
      console.error('Error:', error);
      throw error;
    }
  }
  
}
