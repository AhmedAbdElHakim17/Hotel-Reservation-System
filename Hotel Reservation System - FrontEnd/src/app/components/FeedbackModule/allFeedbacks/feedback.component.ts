import { Component, OnInit } from '@angular/core';
import { IFeedback } from '../../../models/IFeedback';
import { FeedbackService } from '../../../services/feedback/feedback.service';
import { finalize } from 'rxjs';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-feedback',
  imports: [MatProgressSpinner, MatCardModule, MatIconModule
    , MatButtonModule, CommonModule
  ],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.css'
})
export class FeedbackComponent implements OnInit {
  Feedbacks!: IFeedback[]
  errorMessage!: string;
  isLoading = false;
  constructor(private feedBackService: FeedbackService) { }
  ngOnInit(): void {
    this.loadFeedbacks()
  }
  loadFeedbacks() {
    this.isLoading = true;
    this.feedBackService.getAllFeedbacks().pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: (res) => {
        this.Feedbacks = res.data;
        if (!res.isSuccess)
          this.errorMessage = res.message;
      },
      error: (err) => {
        this.errorMessage = err.error?.message
        this.Feedbacks = []
      }
    })
  }
}
