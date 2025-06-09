import { Component, HostListener, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RoomService } from '../../services/room/room.service';
import { ReservationService } from '../../services/reservation/reservation.service';
import { FeedbackService } from '../../services/feedback/feedback.service';
import { finalize } from 'rxjs';
import { AuthService } from '../../services/authentication/Auth.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  role!: string | undefined;
  isLoading: boolean = false;

  roomStats = {
    total: 0,
    available: 0,
    booked: 0
  };

  reservationStats = {
    total: 0,
    pending: 0,
    confirmed: 0,
    checkedIn: 0
  };

  feedbackStats = {
    total: 0,
    new: 0
  };

  constructor(
    private roomService: RoomService,
    private reservationService: ReservationService,
    private auth: AuthService) { }

  ngOnInit() {
    this.auth.authStatus$.subscribe(() => {
      const newRole = localStorage.getItem('role');
      if (this.auth.isLoggedIn() && newRole === 'Admin') {
        this.role = newRole;
        this.loadDashboardData();
      } else {
        this.role = newRole ?? '';
      }
    });
  }

  loadDashboardData() {
    this.isLoading = true;

    // Load room statistics
    this.roomService.getAllRooms().pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          const rooms = res.data;
          this.roomStats.total = rooms.length;
          this.roomStats.available = rooms.filter(r => r.isAvailable).length;
          this.roomStats.booked = rooms.filter(r => !r.isAvailable).length;
        }
      },
      error: (err) => {
        console.error('Error loading room data:', err);
      }
    });

    // Load reservation statistics
    this.reservationService.getAllReservations().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          const reservations = res.data;
          this.reservationStats.total = reservations.length;
          this.reservationStats.pending = reservations.filter(r => r.reservationStatus === 'Pending').length;
          this.reservationStats.confirmed = reservations.filter(r => r.reservationStatus === 'Confirmed').length;
          this.reservationStats.checkedIn = reservations.filter(r => r.reservationStatus === 'CheckedIn').length;
        }
      },
      error: (err) => {
        console.error('Error loading reservation data:', err);
      }
    });
  }
  updateRoleFromStorage() {
    if (this.auth.isLoggedIn()) {
      this.role = localStorage.getItem('role') ?? '';
      if (this.role === 'Admin') {
        this.loadDashboardData();
      }
    } else {
      this.role = undefined;
    }
  }
  ngDoCheck() {
    const currentRole = localStorage.getItem('role');
    if (this.auth.isLoggedIn() && currentRole !== this.role) {
      this.role = currentRole ?? '';
      if (this.role === 'Admin') {
        this.loadDashboardData();
      }
    }
  }
}
