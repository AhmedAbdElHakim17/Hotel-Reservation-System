import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { IOffer } from '../../models/IOffer';
import { environment } from '../../environments/environment.development';
import { ApiResponse } from '../../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class OfferService {
  private readonly apiBaseUrl = `${environment.baseUrl}/api/Offer`
  constructor(private httpClient: HttpClient) { }
  getAllOffers(): Observable<ApiResponse<IOffer[]>> {
    return this.httpClient.get<ApiResponse<IOffer[]>>(`${this.apiBaseUrl}/All`)
  }
  AddOffer(data: IOffer): Observable<ApiResponse<IOffer>> {
    return this.httpClient.post<ApiResponse<IOffer>>(`${this.apiBaseUrl}/Add`, data)
  }
  UpdateOffer(id: number, data: IOffer): Observable<ApiResponse<IOffer>> {
    return this.httpClient.put<ApiResponse<IOffer>>(`${this.apiBaseUrl}/Update/${id}`, data)
  }
  DeleteOffer(id: number): Observable<ApiResponse<IOffer>> {
    return this.httpClient.delete<ApiResponse<IOffer>>(`${this.apiBaseUrl}/Delete/${id}`, {})
  }

}
