import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IUser } from '../../models/IUser';
import { ApiResponse } from '../../models/apiResponse';
import { ILogin } from '../../models/ILogin';
import { IRegister } from '../../models/IRegister';
import { IRole } from '../../models/IRole';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiBaseUrl = `${environment.baseUrl}/api/Account`
  private readonly TOKEN_KEY = 'access_token';
  private authChanged = new BehaviorSubject<boolean>(false);
  authStatus$ = this.authChanged.asObservable();
  isAuthenticated = signal(true);
  role!: string;

  constructor(private http: HttpClient, private router: Router) {
    this.isAuthenticated.set(!!this.getToken());
  }

  register(userData: IRegister): Observable<ApiResponse<IRegister>> {
    return this.http.post<ApiResponse<IRegister>>(`${this.apiBaseUrl}/Register`, userData);
  }

  login(user: IUser): Observable<ApiResponse<ILogin>> {
    if (this.isLoggedIn()) {
      this.router.navigate(['/Home']);
      return of<ApiResponse<ILogin>>({
        message: 'User is already logged in',
        data: {} as ILogin,
        isSuccess: false
      });
    }
    return this.http.post<ApiResponse<ILogin>>(`${this.apiBaseUrl}/Login`, user).pipe(
      tap(response => {
        localStorage.setItem('token', response.data.token)
        localStorage.setItem('tokenExpiry', response.data.expiration);
        this.isAuthenticated.set(true);
        this.getUserRole().subscribe();
        this.notifyAuthChanged();
      },
      ))
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('tokenExpiry');
    localStorage.removeItem('role');
    this.isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getTokenExpiry(): Date | null {
    const expiryStr = localStorage.getItem('tokenExpiry');
    return expiryStr ? new Date(expiryStr) : null;
  }
  isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry();
    return !expiry || new Date() > expiry;
  }
  isLoggedIn(): boolean {
    return !!this.getToken() && !this.isTokenExpired();
  }
  startAutoLogoutWatcher(): void {
    const expiry = this.getTokenExpiry();
    if (!expiry) return;

    const timeLeft = expiry.getTime() - Date.now();
    if (timeLeft <= 0) {
      this.logout();
      return;
    }

    setTimeout(() => {
      this.logout(); // auto logout after timeLeft ms
      alert('Session expired. Please log in again.');
    }, timeLeft);
  }

  get authStatus() {
    return this.isAuthenticated;
  }
  getUserRole(): Observable<ApiResponse<IRole>> {
    return this.http.get<ApiResponse<IRole>>(`${this.apiBaseUrl}/UserRole`).pipe(
      tap(res => {
        localStorage.setItem('role', res.data.name);
      })
    )
  }
  notifyAuthChanged() {
    this.authChanged.next(this.isAuthenticated());
  }
}
