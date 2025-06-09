import { Component } from '@angular/core';
import { AuthService } from '../../services/authentication/Auth.service';
import { OfferService } from '../../services/offer/offer.service';
import { IOffer } from '../../models/IOffer';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ErrorMessageService } from '../../services/error/error.service';

@Component({
  selector: 'app-offer',
  imports: [CommonModule, FormsModule],
  templateUrl: './offer.component.html',
  styleUrl: './offer.component.css'
})
export class OfferComponent {
  offers!: IOffer[];
  isEditMode = false;
  selectedOffer!: IOffer;
  selectedOfferId!: number;
  showModal!: boolean;
  roomTypes: string[] = [`Single`, `Double`, `Suite`, `Deluxe`];
  role!: string;
  today = new Date();
  constructor(private authService: AuthService, private offerService: OfferService, private errService: ErrorMessageService) { }
  ngOnInit() {
    this.role = localStorage.getItem('role') ?? '';
    this.LoadOffers()
  }
  LoadOffers() {
    this.offerService.getAllOffers().subscribe({
      next: res => {
        this.offers = res.data || [];
      }, error: err => {
        this.errService.showErrorMessage(err);
      }
    })
  }
  addOffer(offer: IOffer) {
    this.offerService.AddOffer(offer).subscribe({
      next: res => {
        Swal.fire('Success', res.message || 'Offer is added successfully', 'success');
        this.LoadOffers();
        this.closeModal();
      }, error: err => {
        this.errService.showErrorMessage(err);
      }
    })
  }
  updateOffer(offerId: number, offer: IOffer) {
    this.offerService.UpdateOffer(offerId, offer).subscribe({
      next: res => {
        Swal.fire('Success', res.message || 'Offer is updated successfully', 'success');
        this.LoadOffers();
        this.closeModal();
      }, error: err => {
        this.errService.showErrorMessage(err);
      }
    })
  }
  deleteOffer(offerId: number) {
    Swal.fire({
      title: 'Are you sure you want to delete this offer?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
      icon: 'question',
    }).then((res) => {
      if (res.isConfirmed) {
        this.offerService.DeleteOffer(offerId).subscribe({
          next: res => {
            Swal.fire('Success', res.message || 'Offer is deleted successfully', 'success');
            this.LoadOffers();
            this.closeModal();
          }, error: err => {
            this.errService.showErrorMessage(err);
          }
        })
      }
    })
  }
  openAddModal() {
    this.isEditMode = false;
    this.selectedOffer = {
      id: 0, description: "", discount: 0,
      endDate: new Date(), roomType: "",
      startDate: new Date(), title: ""
    }
    this.showModal = true;
  }
  openEditModal(Offer: IOffer): void {
    this.isEditMode = true;
    this.selectedOffer = { ...Offer };
    this.showModal = true;
  }
  closeModal(): void {
    this.showModal = false;
  }
  submitForm(): void {
    if (this.isEditMode) {
      this.updateOffer(this.selectedOffer.id, this.selectedOffer);
    } else {
      this.addOffer(this.selectedOffer);
    }
  }
}
