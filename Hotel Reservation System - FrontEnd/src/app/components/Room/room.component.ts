import { Component, OnInit } from '@angular/core';
import { IRoom } from '../../models/IRoom';
import { RoomService } from '../../services/room/room.service';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/authentication/Auth.service';
import Swal from 'sweetalert2';
import { ReservationService } from '../../services/reservation/reservation.service';
import { IReservation } from '../../models/IReservation';
import { ActivatedRoute, Router } from '@angular/router';
import { ErrorMessageService } from '../../services/error/error.service';

@Component({
  selector: 'app-room',
  imports: [CommonModule, FormsModule],
  templateUrl: './room.component.html',
  styleUrl: './room.component.css'
})
export class RoomComponent implements OnInit {
  Rooms: IRoom[] = []
  availableRooms: IRoom[] = [];
  bookedRooms: IRoom[] = [];
  Reservation: IReservation = {} as IReservation;
  selectedRoom!: IRoom;
  roomTypes: string[] = [`Single`, `Double`, `Suite`, `Deluxe`];
  message!: string;
  isLoadingAll: boolean = false;
  isLoadingAvailable: boolean = false;
  processing: boolean = false;
  startDate: Date = new Date();
  today = new Date();
  endDate: Date = new Date(new Date().setDate(new Date().getDate() - 1));
  showRoomModal = false;
  showReservationModal = false;
  isEditMode = false;
  role = "";
  constructor(private roomService: RoomService,
    private authService: AuthService, private resService: ReservationService,
    private router: Router, private route: ActivatedRoute, private errorService: ErrorMessageService) { }
  ngOnInit() {
    this.role = localStorage.getItem('role') ?? '';
    this.route.queryParams.subscribe(params => {
      if (params['startDate']) {
        const startDate = new Date(params['startDate']);
        const endDate = new Date(params['endDate']);
        this.loadAvailableRooms(startDate, endDate);
      } else {
        this.LoadRooms();
      }
    })
  }
  LoadRooms() {
    this.isLoadingAll = true;
    this.roomService.getAllRooms().pipe(
      finalize(() => this.isLoadingAll = false)
    ).subscribe({
      next: (res) => {
        this.message = res.message;
        this.Rooms = res.data;
        this.availableRooms = this.Rooms.filter(r => r.isAvailable == true);
        this.bookedRooms = this.Rooms.filter(r => r.isAvailable == false);
      },
      error: (err) => {
        this.errorService.showErrorMessage(err);
      }
    })
  }
  loadAvailableRooms(startDate: Date, endDate: Date) {
    this.isLoadingAvailable = true;
    this.roomService.getAvailableRooms(startDate, endDate).pipe(
      finalize(() => this.isLoadingAvailable = false)
    ).subscribe({
      next: (res) => {
        this.Rooms = res.data;
        this.availableRooms = this.Rooms.filter(r => r.isAvailable == true);
        this.bookedRooms = this.Rooms.filter(r => r.isAvailable == false);
        this.router.navigate([], {
          queryParams: {
            startDate: startDate.toISOString,
            endDate: endDate.toISOString
          },
          replaceUrl: true
        });
      },
      error: (err) => {
        this.errorService.showErrorMessage(err);
      }
    })
  }
  openAddModal(type: string, room?: IRoom) {
    if (this.authService.isAuthenticated()) {
      if (type == 'room') {
        this.isEditMode = false;
        this.selectedRoom = {
          id: 0, roomNum: 0, roomType: "", isAvailable: true,
          pricePerNight: 0, imageUrl: "", facilities: ""
        }
        this.showRoomModal = true;
      } else {
        if (this.authService.isAuthenticated() && room) {
          this.Reservation = {
            ...this.Reservation,
            roomNum: room.roomNum,
            createdAt: new Date()
          }
          this.showReservationModal = true;
        }
      }
    }
    else {
      Swal.fire('Oops!', 'Please Login first', 'info');
      this.router.navigateByUrl("/login")
    }
  }
  openEditRoomModal(room: IRoom): void {
    this.isEditMode = true;
    this.selectedRoom = { ...room };
    this.showRoomModal = true;
  }
  Reserve() {
    if (this.authService.isAuthenticated()) {
      this.Reservation = {
        checkInDate: new Date(), checkOutDate: new Date(),
        userName: "", createdAt: new Date(),
        totalAmount: 0, roomNum: 0, reservationStatus: "", id: 0
      }
      this.showReservationModal = true;
    }
    else {
      Swal.fire('Oops!', 'Please Login first', 'info');
      this.router.navigateByUrl("/login")
    }
  }
  closeModal(type: string): void {
    if (type == 'room')
      this.showRoomModal = false;
    else
      this.showReservationModal = false;
  }
  submitRoomForm(): void {
    if (this.isEditMode) {
      this.roomService.updateRoom(this.selectedRoom.id, this.selectedRoom)
        .pipe(
          finalize(() => this.processing = false)
        )
        .subscribe(res => {
          if (res.isSuccess) {
            this.LoadRooms();
            this.closeModal('room');
          }
          else {
            Swal.fire("Fail!", res.message, "error")
          }
        });
    } else {
      this.roomService.addRoom(this.selectedRoom)
        .pipe(
          finalize(() => this.processing = false)
        )
        .subscribe(res => {
          if (res.isSuccess) {
            this.LoadRooms();
            this.closeModal('room');
          }
        });
    }
  }
  deleteRoom(id: number) {
    Swal.fire({
      title: 'Are you sure you want to delete this room?',
      showCancelButton: true,
      cancelButtonText: 'No',
      confirmButtonText: 'Yes',
    }).then((res) => {
      if (res.isConfirmed) {
        this.roomService.deleteRoom(id).subscribe({
          next: (res) => {
            Swal.fire('Deleted!', '', 'success');
            this.LoadRooms();
          },
          error: (err) => {
            this.errorService.showErrorMessage(err);
          }
        });
      }
    })
  }
  submitReservation() {
    this.processing = true;
    this.resService.addReservation(this.Reservation).pipe(
      finalize(() => this.processing = false)
    ).subscribe({
        next: (res) => {
          Swal.fire('Success!', "", "success")
          this.closeModal('res');
          this.router.navigateByUrl("/Reservation")
        },
        error: (err) => {
          this.errorService.showErrorMessage(err);
        }
      })
  }
}
