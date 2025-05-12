import { Component, signal } from '@angular/core';
import { IReservation } from '../../../models/IReservation';
import { ReservationService } from '../../../services/reservation/reservation.service';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-addReservation',
  standalone: true,
  imports: [FormsModule, MatCardModule, MatProgressSpinnerModule,
    MatIconModule, MatFormFieldModule, CommonModule],
  templateUrl: './addReservation.component.html',
  styleUrls: ['./addReservation.component.css']
})
export class AddReservationComponent {

  reservation: IReservation = {} as IReservation
  isLoading = false;
  message!: string
  isSuccess!: boolean;
  constructor(private reservationService: ReservationService) { }

  Save() {
    this.isLoading = true;
    const save = this.reservation.id
      ? this.reservationService.updateReservation(this.reservation.id, this.reservation)
      : this.reservationService.addReservation(this.reservation)
    save.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.reservation = res.data;
          this.isSuccess = res.isSuccess
        } else {
          this.message = res.message
          this.isSuccess = res.isSuccess
        }
      },
      error: (err) => {
        this.message = err.error?.message
        this.isSuccess = err.error?.isSuccess
      }
    })
  }

  cancel() {
    if (!this.reservation.id) return;
    this.isLoading = true;
    this.reservationService.cancelReservation(this.reservation.id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.reservation = res.data;
          this.isSuccess = res.isSuccess
        } else {
          this.isSuccess = res.isSuccess
          this.message = res.message
        }
      },
      error: (err) => {
        this.isSuccess = err.error?.isSuccess
        this.message = err.error?.message
      }
    })
  }
  confirm() {
    if (!this.reservation.id) return;
    this.reservationService.confirmReservation(this.reservation.id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.reservation = res.data;
          this.isSuccess = res.isSuccess
        } else {
          this.isSuccess = res.isSuccess
          this.message = res.message
        }
      },
      error: (err) => {
        this.isSuccess = err.error?.isSuccess
        this.message = err.error?.message
      }
    })
  }

  checkIn() {
    if (!this.reservation.id) return;
    this.reservationService.checkInReservation(this.reservation.id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.reservation = res.data;
          this.isSuccess = res.isSuccess
        } else {
          this.isSuccess = res.isSuccess
          this.message = res.message
        }
      },
      error: (err) => {
        this.isSuccess = err.error?.isSuccess
        this.message = err.error?.message
      }
    })
  }

  checkOut() {
    if (!this.reservation.id) return;
    this.reservationService.checkOutReservation(this.reservation.id).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.reservation = res.data;
          this.isSuccess = res.isSuccess
        } else {
          this.isSuccess = res.isSuccess
          this.message = res.message
        }
      },
      error: (err) => {
        this.isSuccess = err.error?.isSuccess
        this.message = err.error?.message
      }
    })
  }


}
