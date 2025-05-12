import { Component, OnInit } from '@angular/core';
import { finalize } from 'rxjs';
import { IReservation } from '../../../models/IReservation';
import { ReservationService } from '../../../services/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-reservation',
  imports: [CommonModule, NgxSkeletonLoaderModule, NgbDropdownModule],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.css'
})
export class ReservationComponent implements OnInit {
  Reservations: IReservation[] = {} as IReservation[]
  errorMessage!: string;
  isLoading = false;
  constructor(private reservationService: ReservationService) { }
  ngOnInit(): void {
    this.LoadReservations();
  }
  LoadReservations() {
    this.isLoading = true;
    this.reservationService.getAllReservations().pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        this.Reservations = res.data;
        if (!res.isSuccess)
          this.errorMessage = res.message;
      },
      error: (err) => {
        this.errorMessage = err.error?.message
        this.Reservations = []
      }
    })
  }
}
