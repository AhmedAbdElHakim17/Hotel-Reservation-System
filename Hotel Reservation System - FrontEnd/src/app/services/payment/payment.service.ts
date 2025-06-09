import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ApiResponse } from '../../models/apiResponse';
import { IPayment } from '../../models/IPayment';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  apiBaseUrl = `${environment.baseUrl}/api/Payment`
  constructor(private http: HttpClient) { }

  getAllPayments(): Observable<ApiResponse<IPayment[]>> {
    return this.http.get<ApiResponse<IPayment[]>>(`${this.apiBaseUrl}/All`)
  }
  createPaymentIntent(amount: number, reservationId: number): Observable<any> {
    return this.http.post(`${this.apiBaseUrl}/Pay/${reservationId}`, amount);
  }
}