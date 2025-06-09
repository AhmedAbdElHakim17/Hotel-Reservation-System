import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { PaymentService } from '../../../services/payment/payment.service';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { IPayment } from '../../../models/IPayment';
import Swal from 'sweetalert2';
import { ErrorMessageService } from '../../../services/error/error.service';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  @Input() amount!: number;
  @Input() reservationId!: number;
  @Output() completed = new EventEmitter<boolean>();

  payments: IPayment[] = [];
  processing = false;
  loadError = false;
  error = '';
  role = '';

  constructor(private paymentService: PaymentService, private errService: ErrorMessageService) { }

  ngOnInit(): void {
    this.role = localStorage.getItem('role') ?? '';
    if (this.role === 'Admin' || this.role === 'HotelStaff')
      this.loadPayments();
  }

  loadPayments(): void {
    this.paymentService.getAllPayments().subscribe({
      next: res => {
        this.payments = res.data.sort((a, b) => new Date(b.transactionDate).getTime() - new Date(a.transactionDate).getTime()) || [];
        this.loadError = false;
      }, error: err => {
        this.errService.showErrorMessage(err);
        this.loadError = true;
      }
    });
  }

  pay(): void {
    this.processing = true;
    // Add console log for debugging
    console.log('Initiating payment for amount:', this.amount, 'reservation:', this.reservationId);

    this.paymentService.createPaymentIntent(this.amount, this.reservationId)
      .subscribe({
        next: (response) => {
          console.log('Payment response:', response); // Debug log
          if (response?.url) {
            window.location.href = response.url;
          } else {
            this.processing = false;
            this.completed.emit(true);
            Swal.fire('Success', 'Payment initiated successfully', 'success'); // Add success feedback
          }
        },
        error: (err) => {
          this.errService.showErrorMessage(err)
          this.completed.emit(false);
          this.loadError = true;
        }
      });
  }

  cancel(): void {
    this.completed.emit(false);
  }
}