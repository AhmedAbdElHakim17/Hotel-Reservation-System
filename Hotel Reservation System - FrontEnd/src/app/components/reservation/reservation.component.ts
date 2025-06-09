import { Component, OnInit } from '@angular/core';
import { finalize, Observable } from 'rxjs';
import { IReservation } from '../../models/IReservation';
import { ReservationService } from '../../services/reservation/reservation.service';
import { CommonModule } from '@angular/common';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../services/authentication/Auth.service';
import Swal from 'sweetalert2';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiResponse } from '../../models/apiResponse';
import { FormsModule } from '@angular/forms';
import { IFeedback } from '../../models/IFeedback';
import { FeedbackService } from '../../services/feedback/feedback.service';
import { PaymentComponent } from "../payment/payment/payment.component";
import { ErrorMessageService } from '../../services/error/error.service';

@Component({
  selector: 'app-reservation',
  imports: [CommonModule, NgxSkeletonLoaderModule, NgbDropdownModule, FormsModule, PaymentComponent],
  templateUrl: './reservation.component.html',
  styleUrl: './reservation.component.css'
})
export class ReservationComponent implements OnInit {
  Reservations: IReservation[] = {} as IReservation[];
  previousRes: IReservation[] = {} as IReservation[];
  confirmedRes: IReservation[] = {} as IReservation[];
  checkedOutRes: IReservation[] = {} as IReservation[];
  checkedInRes: IReservation[] = {} as IReservation[];
  expiredRes: IReservation[] = {} as IReservation[];
  pendingRes: IReservation[] = {} as IReservation[];
  cancelledRes: IReservation[] = {} as IReservation[];
  reservation!: IReservation;
  currentReservationId!: number;
  feedback!: IFeedback;
  ratingTouched = false;
  showPayment = false;
  role!: string;
  errorMessage!: string;
  isLoading!: boolean;
  isEditMode!: boolean;
  showModal!: boolean;
  isDownloading = false;
  isUpcoming = false;
  selectedReservation: IReservation = {} as IReservation;
  isDetailModalOpen = false;
  today = new Date(new Date());
  constructor(private reservationService: ReservationService, private authService: AuthService,
    private router: Router, private feedBackService: FeedbackService, private route: ActivatedRoute
    , private errorService: ErrorMessageService) { }
  ngOnInit(): void {
    this.role = localStorage.getItem('role') ?? '';
    if (this.role == "Guest")
      this.LoadReservations(this.reservationService.getMyReservations());
    else if (this.role == "Admin" || this.role == "HotelStaff")
      this.LoadReservations(this.reservationService.getAllReservations())
    else {
      Swal.fire('Oops!', 'Login First', 'error');
      this.router.navigateByUrl('/login')
    }
    this.route.queryParams.subscribe(params => {
      if (params['payment'] === 'success') {
        this.checkPaymentStatus();
      }
    });
    const savedId = localStorage.getItem('pendingPaymentReservationId');
    if (savedId) {
      this.currentReservationId = +savedId;
      localStorage.removeItem('pendingPaymentReservationId');
      this.checkPaymentStatus();
    }
  }
  LoadReservations(func: Observable<ApiResponse<IReservation[]>>) {
    this.isLoading = true;
    func.pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        this.Reservations = res.data.sort((a, b) => (new Date(b.checkInDate).getTime() - new Date(a.checkInDate).getTime()))
        this.confirmedRes = this.Reservations.filter(r => r.reservationStatus == "Confirmed")
        this.expiredRes = this.Reservations.filter(r => r.reservationStatus == "Expired")
        this.checkedOutRes = this.Reservations.filter(r => r.reservationStatus == "CheckedOut")
        this.pendingRes = this.Reservations.filter(r => r.reservationStatus == "Pending")
        this.cancelledRes = this.Reservations.filter(r => r.reservationStatus == "Cancelled")
        this.checkedInRes = this.Reservations.filter(r => r.reservationStatus == "CheckedIn")
        if (!res.isSuccess)
          Swal.fire('Fail!', res.message || 'Reservations Loading Process failed', 'error');
      },
      error: (err) => {
        this.errorService.showErrorMessage(err);
      }
    })
  }
  LoadUpcomingReservation() {
    this.isUpcoming = true;
    if (this.role == 'Guest') {
      this.LoadReservations(this.reservationService.getMyUpcomingReservations());
    } else {
      this.LoadReservations(this.reservationService.getAllUpcomingReservations())
    }
  }
  Cancel(id: number) {
    Swal.fire({
      title: 'Are you sure you want to cancel this reservation?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
      icon: 'question'
    }).then((res) => {
      if (res.isConfirmed) {
        this.reservationService.cancelReservation(id)
          .subscribe({
            next: res => {
              Swal.fire('Cancelled!', '', 'success');
              if (this.role == "Guest")
                this.LoadReservations(this.reservationService.getMyReservations());
              else
                this.LoadReservations(this.reservationService.getAllReservations());
            },
            error: err => {
              this.errorService.showErrorMessage(err);
            }
          });
      }
    })
  }
  confirm(id: number) {
    Swal.fire({
      title: 'Are you sure you want to confirm this reservation?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((res) => {
      if (res.isConfirmed) {
        this.reservationService.confirmReservation(id)
          .subscribe({
            next: res => {
              Swal.fire('Confirmed!', '', 'success');
              if (this.role == "Guest")
                this.LoadReservations(this.reservationService.getMyReservations());
              else
                this.LoadReservations(this.reservationService.getAllReservations());
            },
            error: err => {
              this.errorService.showErrorMessage(err);
            }
          });
      }
    })
  }
  checkIn(id: number) {
    Swal.fire({
      title: 'Are you sure you want to Check-In this reservation?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
      icon: 'question'
    }).then((res) => {
      if (res.isConfirmed) {
        this.reservationService.checkInReservation(id)
          .subscribe({
            next: res => {
              Swal.fire('Checked-In!', '', 'success');
              if (this.role == "Guest")
                this.LoadReservations(this.reservationService.getMyReservations());
              else
                this.LoadReservations(this.reservationService.getAllReservations());
            },
            error: err => {
              this.errorService.showErrorMessage(err);
            }
          });
      }
    })
  }
  checkOut(id: number) {
    Swal.fire({
      title: 'Are you sure you want to checkOut this reservation?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
      icon: 'question'
    }).then((res) => {
      if (res.isConfirmed) {
        this.reservationService.checkOutReservation(id)
          .subscribe({
            next: res => {
              Swal.fire('Checked-Out!', '', 'success');
              if (this.role == "Guest")
                this.LoadReservations(this.reservationService.getMyReservations());
              else
                this.LoadReservations(this.reservationService.getAllReservations());
            },
            error: err => {
              this.errorService.showErrorMessage(err);
            }
          });
      }
    })
  }
  downloadPaymentPdf(reservationId: number): void {
    this.isDownloading = true;
    this.reservationService.downloadPdf(reservationId).pipe(
      finalize(() => this.isDownloading = false)
    ).subscribe({
      next: res => {
        if (res && res.size > 0) {
          Swal.fire('Success!', 'Succeeded to download payment pdf', 'success');
        } else {
          Swal.fire('Fail!', 'Received empty PDF', 'error');
        }
      },
    });
  }
  viewDetails(reservationId: number) {
    this.selectedReservation = this.Reservations.find(r => r.id === reservationId) ?? {} as IReservation;
    if (this.selectedReservation) {
      this.isDetailModalOpen = true;
    } else {
      Swal.fire('Oops!', 'Reservation details not found', 'error');
    }
  }
  getNights() {
    if (!this.selectedReservation) return 0;
    const checkIn = new Date(this.selectedReservation.checkInDate);
    const checkOut = new Date(this.selectedReservation.checkOutDate);
    const diffTime = checkOut.getTime() - checkIn.getTime();
    return Math.floor(diffTime / (1000 * 60 * 60 * 24));
  }
  LoadPreviousReservations() {
    this.isUpcoming = false;
    if (this.role == 'Guest') {
      this.LoadReservations(this.reservationService.getMyReservations())
    } else {
      this.LoadReservations(this.reservationService.getAllReservations())
    }
  }
  openFeedbackModal(Feedback: IFeedback, resId: number) {
    if (this.authService.isAuthenticated()) {
      this.isEditMode = false;
      this.feedback = {
        ...Feedback,
        reservationId: resId
      }
      this.showModal = true;
    } else {
      Swal.fire('Oops!', 'Please Login first', 'info');
      this.router.navigateByUrl("/login")
    }
  }
  openEditModal(Feedback: IFeedback): void {
    this.isEditMode = true;
    this.feedback = Feedback
    this.showModal = true;
  }
  closeModal(): void {
    this.showModal = false;
  }
  submitForm(): void {
    if (this.isEditMode) {
      this.feedBackService.UpdateFeedback(this.feedback.id, this.feedback)
        .pipe(
          finalize(() => this.isLoading = false)
        )
        .subscribe({
          next: res => {
            this.closeModal();
          },
          error: err => {
            this.errorService.showErrorMessage(err);
          }
        });
    } else {
      this.feedBackService.AddFeedback(this.feedback)
        .pipe(
          finalize(() => this.isLoading = false)
        )
        .subscribe({
          next: res => {
            Swal.fire("Success!", res.message, 'success')
            this.closeModal();
          },
          error: err => {
            this.errorService.showErrorMessage(err);
          }
        });
    }
  }
  startPayment(reservation: IReservation) {
    this.selectedReservation = reservation;
    this.currentReservationId = reservation.id;
    this.showPayment = true;
    localStorage.setItem('pendingPaymentReservationId', reservation.id.toString());
  }

  onPaymentComplete(success: boolean) {
    this.showPayment = false;
    if (success) {
      Swal.fire('Started', 'Payment started', 'success');
      this.checkPaymentStatus();
    }
  }

  async checkPaymentStatus() {
    let attempts = 0;
    const maxAttempts = 10;
    const pollInterval = 2000; // 2 seconds

    const poll = setInterval(async () => {
      try {
        const res = await this.reservationService.getReservationById(this.currentReservationId).toPromise();
        const reservationData = res?.data as IReservation;

        if (reservationData?.reservationStatus === 'Confirmed') {
          clearInterval(poll);
          Swal.fire('Success!', 'Payment confirmed', 'success');
          this.LoadReservations(this.reservationService.getMyReservations());
        } else if (attempts >= maxAttempts) {
          clearInterval(poll);
        }

        attempts++;
      } catch (error) {
        clearInterval(poll);
        // console.error('Error checking payment status:', error);
        // Swal.fire('Error', 'Failed to check payment status', 'error');
      }
    }, pollInterval);
  }
  canCheckIn(reservation: IReservation): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const checkIn = new Date(reservation.checkInDate);
    checkIn.setHours(0, 0, 0, 0);
    return checkIn <= today && reservation.reservationStatus === 'Confirmed';
  }
  trackByReservationId(index: number, reservation: IReservation): number {
    return reservation.id;
  }
}

