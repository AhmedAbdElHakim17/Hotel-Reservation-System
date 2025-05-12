import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { IRoom } from '../../../models/IRoom';
import { RoomService } from '../../../services/room/room.service';
import { finalize } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-availableRooms',
  imports: [CommonModule, FormsModule],
  templateUrl: './availableRooms.component.html',
  styleUrls: ['./availableRooms.component.css']
})
export class AvailableRoomsComponent implements OnInit {

  availableRooms: IRoom[] = []
  roomTypes: string[] = [`Single`, `Double`, `Suite`, `Deluxe`];
  message!: string;
  isLoadingAvailable: boolean = false;
  startDate: Date = new Date();
  endDate: Date = new Date(new Date().setDate(new Date().getDate() + 1));
  constructor(private roomService: RoomService) { }

  ngOnInit() {
    if (this.startDate || this.endDate)
      this.loadAvailableRooms();
  }

  loadAvailableRooms() {
    if (!this.startDate || !this.endDate) {
      this.message = 'Please select valid dates';
      return;
    }
    this.isLoadingAvailable = true;
    this.roomService.getAvailableRooms(this.startDate, this.endDate).pipe(
      finalize(() => this.isLoadingAvailable = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message
          this.availableRooms = res.data;
        } else {
          this.message = res.message;
        }
      },
      error: (err) => {
        this.message = err.error?.message
        this.availableRooms = []
      }
    })
  }
}
