import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { ApiResponse } from '../../models/apiResponse';
import { IRoom } from '../../models/IRoom';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class RoomService {
  private readonly apiBaseUrl = `${environment.baseUrl}/api/Room`
  constructor(private httpClient: HttpClient) { }
  getAllRooms(): Observable<ApiResponse<IRoom[]>> {
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/All`);
  }
  getAvailableRooms(startDate: Date, endDate: Date): Observable<ApiResponse<IRoom[]>> {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const params = new HttpParams()
      .set('from', start.toISOString())
      .set('to', end.toISOString());
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/Available`,
      { params }
    )
  }
  getRoomById(id: number): Observable<ApiResponse<IRoom[]>> {
    return this.httpClient.get<ApiResponse<IRoom[]>>(`${this.apiBaseUrl}/GetById/${id}`)
  }
  addRoom(data: IRoom): Observable<ApiResponse<IRoom>> {
    return this.httpClient.post<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Add`, data)
  }
  updateRoom(id: number, data: IRoom): Observable<ApiResponse<IRoom>> {
    return this.httpClient.put<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Update/${id}`, data)
  }
  deleteRoom(id: number): Observable<ApiResponse<IRoom>> {
    return this.httpClient.delete<ApiResponse<IRoom>>(`${this.apiBaseUrl}/Delete/${id}`, {})
  }
}
