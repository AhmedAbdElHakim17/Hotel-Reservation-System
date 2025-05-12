import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { IReservation } from '../../../models/IReservation';
import { ReservationService } from '../../../services/reservation/reservation.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-myUpcomingReservations',
  imports: [CommonModule, NgxSkeletonLoaderModule],
  templateUrl: './myUpcomingReservations.component.html',
  styleUrls: ['./myUpcomingReservations.component.css']
})
export class MyUpcomingReservationsComponent implements OnInit {

  Reservations: IReservation[] = {} as IReservation[]
    errorMessage!: string;
    isLoading = false;
    constructor(private reservationService: ReservationService) { }
    ngOnInit(): void {
      this.LoadReservations();
    }
    LoadReservations() {
      this.isLoading = true;
      this.reservationService.getMyUpcomingReservations().pipe(
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
