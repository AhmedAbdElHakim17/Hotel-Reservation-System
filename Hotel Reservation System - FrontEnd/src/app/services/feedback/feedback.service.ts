import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';
import { IFeedback } from '../../models/IFeedback';
import { environment } from '../../environments/environment.development';
import { ApiResponse } from '../../models/apiResponse';
import { IReservation } from '../../models/IReservation';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {

  private readonly apiBaseUrl = `${environment.baseUrl}/api/Feedback`
  constructor(private httpClient: HttpClient) { }

  getAllFeedbacks(): Observable<ApiResponse<IFeedback[]>> {
    return this.httpClient.get<ApiResponse<IFeedback[]>>(`${this.apiBaseUrl}/All`)
  }
  GetMyFeedbacks(): Observable<ApiResponse<IFeedback[]>> {
    return this.httpClient.get<ApiResponse<IFeedback[]>>(`${this.apiBaseUrl}/MyOwn`)
  }
  AddFeedback(data: IFeedback): Observable<ApiResponse<IFeedback>> {
    return this.httpClient.post<ApiResponse<IFeedback>>(`${this.apiBaseUrl}/Add`, data)
  }
  UpdateFeedback(id: number, data: IFeedback): Observable<ApiResponse<IFeedback>> {
    return this.httpClient.put<ApiResponse<IFeedback>>(`${this.apiBaseUrl}/Update/${id}`, data)
  }
  DeleteFeedback(id: number): Observable<ApiResponse<IFeedback>> {
    return this.httpClient.delete<ApiResponse<IFeedback>>(`${this.apiBaseUrl}/Delete/${id}`, {})
  }
}
