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
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/All`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: [],
          isSuccess: false
        });
      }))
  }
  getAllUpcomingReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/AllUpcoming`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: [],
          isSuccess: false
        });
      }))
  }
  getMyUpcomingReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/MyOwn-Upcoming`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: [],
          isSuccess: false
        });
      }))
  }
  getMyReservations() {
    return this.httpClient.get<ApiResponse<IReservation[]>>(`${this.apiBaseUrl}/MyOwn`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: [],
          isSuccess: false
        });
      }))
  }
  addReservation(data: IReservation): Observable<ApiResponse<IReservation>> {
    return this.httpClient.post<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Add`, data).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }
  updateReservation(id: number, data: IReservation): Observable<ApiResponse<IReservation>> {
    return this.httpClient.put<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Update/${id}`, data).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }

  cancelReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.delete<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Cancel/${id}`, {}).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }

  confirmReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/Confirm/${id}`, {}).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }

  checkInReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/CheckIn/${id}`, {}).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }
  checkOutReservation(id: number): Observable<ApiResponse<IReservation>> {
    return this.httpClient.patch<ApiResponse<IReservation>>(`${this.apiBaseUrl}/CheckOut/${id}`, {}).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }

  downloadPdf(id: number): Observable<Blob> {
    return this.httpClient.get(`${this.apiBaseUrl}/invoice-pdf/${id}`, {
      responseType: 'blob', // Important for binary data
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
