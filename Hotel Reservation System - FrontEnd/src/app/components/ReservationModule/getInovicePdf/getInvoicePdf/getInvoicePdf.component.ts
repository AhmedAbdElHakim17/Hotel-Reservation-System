import { Component, OnInit } from '@angular/core';
import { ReservationService } from '../../../../services/reservation/reservation.service';
import { finalize, identity } from 'rxjs';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-getInvoicePdf',
  imports: [MatSnackBarModule],
  templateUrl: './getInvoicePdf.component.html',
  styleUrls: ['./getInvoicePdf.component.css']
})
export class GetInvoicePdfComponent {

  constructor(private reservationService: ReservationService, private snackBar: MatSnackBar) { }
  isDownloading = false;

  downloadPaymentPdf(reservationId: number): void {
    this.isDownloading = true;
    this.reservationService.downloadPdf(reservationId).pipe(
      finalize(() => this.isDownloading = false)
    ).subscribe({
      error: (err) => {
        this.snackBar.open('Failed to download PDF', 'Close');
      }
    });
  }
}
