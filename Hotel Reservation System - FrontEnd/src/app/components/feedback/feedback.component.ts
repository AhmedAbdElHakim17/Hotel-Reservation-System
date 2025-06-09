import { Component, OnInit } from '@angular/core';
import { IFeedback } from '../../models/IFeedback';
import { FeedbackService } from '../../services/feedback/feedback.service';
import { finalize } from 'rxjs';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/authentication/Auth.service';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ErrorMessageService } from '../../services/error/error.service';

@Component({
  selector: 'app-feedback',
  imports: [MatProgressSpinner, MatCardModule, MatIconModule
    , MatButtonModule, CommonModule, FormsModule
  ],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.css'
})
export class FeedbackComponent implements OnInit {
  Feedbacks!: IFeedback[]
  feedback!: IFeedback
  isEditMode!: boolean;
  showModal!: boolean;
  errorMessage!: string;
  isLoading = false;
  role!: string;
  message!: string | undefined;
  extractedMessage!: string | undefined;
  showAllFeedbacks: boolean = true;
  constructor(private authService: AuthService, private feedBackService: FeedbackService,
    private router: Router, private errorMessageService: ErrorMessageService
  ) { }
  ngOnInit(): void {
    this.role = localStorage.getItem('role') ?? '';
    this.loadFeedbacks()
  }
  loadFeedbacks() {
    this.isLoading = true;
    this.feedBackService.getAllFeedbacks().pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        this.Feedbacks = res.data.sort((a, b) => (new Date(b.submittedAt).getTime() - new Date(a.submittedAt).getTime()));
      },
      error: (err) => {
        this.errorMessageService.showErrorMessage(err);
      }
    })
  }
  loadMyReservationFeedbacks() {
    this.isLoading = true;
    this.showAllFeedbacks = false;
    this.feedBackService.GetMyFeedbacks().pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        this.Feedbacks = res.data.sort((a, b) =>
          new Date(b.submittedAt).getTime() - new Date(a.submittedAt).getTime()
        );
      },
      error: (err) => {
        this.errorMessageService.showErrorMessage(err);
      }
    });
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
            Swal.fire("Success!", res.message, 'success')
            this.closeModal();
          },
          error: err => {
            this.errorMessageService.showErrorMessage(err);
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
            this.errorMessageService.showErrorMessage(err);
          }
        });
    }
  }
  deleteFeedback(id: number) {
    Swal.fire({
      title: 'Are you sure you want to delete this feedback?',
      icon: 'question',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((res) => {
      if (res.isConfirmed) {
        this.feedBackService.DeleteFeedback(id)
          .subscribe({
            next: res => {
              Swal.fire('Deleted!', '', 'success');
              this.loadFeedbacks();
            },
            error: err => {
              this.errorMessageService.showErrorMessage(err);
            }
          })
      }
    })
  }
  get averageRating(): number {
    if (!this.Feedbacks || this.Feedbacks.length === 0) return 0;
    return this.Feedbacks.reduce((sum, feedback) => sum + feedback.rating, 0) / this.Feedbacks.length;
  }
}
