import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { ApiResponse } from '../../models/apiResponse';
import { IRoom } from '../../models/IRoom';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { AuthService } from '../authentication/Auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly apiBaseUrl = `${environment.baseUrl}/api/Room`
  constructor(private httpClient: HttpClient) { }
  getAllRooms(): Observable<ApiResponse<IRoom[]>> {
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/All`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: false
        });
      })
    )
  }
  getAvailableRooms(startDate: Date, endDate: Date): Observable<ApiResponse<IRoom[]>> {
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/Available`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: false
        });
      })
    )
  }
  getRoomById(id: number): Observable<ApiResponse<IRoom[]>> {
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/GetById/${id}`).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.message,
          data: error.data,
          isSuccess: false
        });
      })
    )
  }
  addRoom(data: IRoom): Observable<ApiResponse<IRoom>> {
    return this.httpClient.post<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Add`, data).pipe(
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
  updateRoom(id: number, data: IRoom): Observable<ApiResponse<IRoom>> {
    return this.httpClient.put<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Update/${id}`, data).pipe(
      map(res => res),
      catchError(error => {
        return of({
          message: error.error.errors[0],
          data: error.data,
          isSuccess: error.isSuccess
        });
      })
    )
  }

  deleteRoom(id: number): Observable<ApiResponse<IRoom>> {
    return this.httpClient.delete<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Delete/${id}`, {}).pipe(
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
}
