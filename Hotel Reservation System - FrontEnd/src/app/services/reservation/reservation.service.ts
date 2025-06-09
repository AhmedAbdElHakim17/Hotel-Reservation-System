import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { IReservation } from '../../models/IReservation';
import { environment } from '../../environments/environment.development';
import { ApiResponse } from '../../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  apiBaseUrl = `${environment.baseUrl}/api/Reservation`
  isLoading: boolean = false;
  constructor(private httpClient: HttpClient) { }
  getAllReservations(): Observable<ApiResponse<IReservation[]>> {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/All`);
  }
  getAllUpcomingReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/AllUpcoming`);
  }
  getMyUpcomingReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/MyOwn-Upcoming`);
  }
  getMyReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/MyOwn`);
  }
  getReservationById(id: number) {
    return this.httpClient.get<ApiResponse<IReservation>>(`${this.apiBaseUrl}/GetById/${id}`);
  }
  addReservation(data: IReservation): Observable<ApiResponse<IReservation>> {
    return this.httpClient.post<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Add`, data);
  }
  updateReservation(id: number, data: IReservation): Observable<ApiResponse<IReservation>> {
    return this.httpClient.put<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Update/${id}`, data);
  }
  cancelReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.delete<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Cancel/${id}`);
  }
  confirmReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Confirm/${id}`, {});
  }
  checkInReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/CheckIn/${id}`, {});
  }
  checkOutReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/CheckOut/${id}`, {});
  }
  downloadPdf(id: number): Observable<Blob> {
    return this.httpClient.get(`${this.apiBaseUrl}/invoice-pdf/${id}`, {
      responseType: 'blob',
      headers: new HttpHeaders({
        'Accept': 'application/pdf'
      })
    }).pipe(
      tap((blob: Blob) => {
        this.handlePdfDownload(blob, id)
      })
    );
  }
  private handlePdfDownload(blob: Blob, reservationId: number): void {
    // Create download link
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `Payment-${reservationId}.pdf`;
    document.body.appendChild(a);
    a.click();

    // Cleanup
    setTimeout(() => {
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    }, 100);
  }
}
