import { Component, OnInit } from '@angular/core';
import { IRoom } from '../../../models/IRoom';
import { RoomService } from '../../../services/room/room.service';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/authentication/Auth.service';
import Swal from 'sweetalert2';
import { ReservationService } from '../../../services/reservation/reservation.service';
import { IReservation } from '../../../models/IReservation';
import { Router } from '@angular/router';

@Component({
  selector: 'app-room',
  imports: [CommonModule, FormsModule],
  templateUrl: './room.component.html',
  styleUrl: './room.component.css'
})
export class RoomComponent implements OnInit {
  Rooms: IRoom[] = []
  Reservation: IReservation = {} as IReservation;
  selectedRoom!: IRoom;
  roomTypes: string[] = [`Single`, `Double`, `Suite`, `Deluxe`];
  message!: string;
  isLoadingAll: boolean = false;
  isLoadingAvailable: boolean = false;
  startDate: Date = new Date();
  endDate: Date = new Date(new Date().setDate(new Date().getDate() - 1));
  showRoomModal = false;
  showReservationModal = false;
  isEditMode = false;
  role = "";
  constructor(private roomService: RoomService,
    private authService: AuthService, private resService: ReservationService, private router: Router) { }
  ngOnInit() {
    this.role = this.getUserRole();
    console.log(this.role);
    this.LoadRooms();
    if (this.startDate < this.endDate)
      this.loadAvailableRooms();
  }
  LoadRooms() {
    this.isLoadingAll = true;
    this.roomService.getAllRooms().pipe(
      finalize(() => this.isLoadingAll = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.message = res.message;
          this.Rooms = res.data;
          console.log(this.Rooms);
        } else {
          Swal.fire({
            title: "Fail!",
            text: "The backend Server doesn't work",
            icon: 'error'
          })
        }
      },
      error: (err) => {
        Swal.fire({
          title: "Fail!",
          text: err.error?.message,
          icon: 'error'
        })
        this.Rooms = []
      }
    })
  }
  loadAvailableRooms() {
    if (this.startDate >= this.endDate) {
      Swal.fire({
        title: "Oops...!",
        text: "Please select valid dates",
        icon: 'error'
      });
      return;
    }
    this.isLoadingAvailable = true;
    this.roomService.getAvailableRooms(this.startDate, this.endDate).pipe(
      finalize(() => this.isLoadingAvailable = false)
    ).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          Swal.fire({
            title: "Success!",
            text: res.message,
            icon: 'success'
          })
          this.Rooms = res.data;
        } else {
          Swal.fire({
            title: "Fail!",
            text: "The backend server doesn't work",
            icon: 'error'
          })
        }
      },
      error: (err) => {
        this.message = err.error?.message
        this.Rooms = []
      }
    })
  }
  openAddModal(type: string, room?: IRoom) {
    if (type == 'room') {
      this.isEditMode = false;
      this.selectedRoom = {
        id: 0, roomNum: 0, roomType: 1, isAvailable: true,
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
      else {
        Swal.fire('Oops!', 'Please Login first', 'info');
        this.router.navigateByUrl("/login")
      }
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
          finalize(() => this.isLoadingAll = false)
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
          finalize(() => this.isLoadingAll = false)
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
        this.roomService.deleteRoom(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Deleted!', '', 'success');
            this.LoadRooms();
          }
          else {
            Swal.fire('NotFound!', '', 'error');
          }
        });
      }
    })
  }
  getUserRole() {
    this.authService.getUserRole()
      .subscribe((res) => {
        this.role = res.data.name
        console.log(this.role);
      })
    return this.role;
  }
  submitReservation() {
    this.resService.addReservation(this.Reservation)
      .subscribe({
        next: ((res) => {
          if (res.isSuccess) {
            Swal.fire('Success!', "", "success")
            this.closeModal('res');
            this.router.navigateByUrl("/Reservation/MyUpcoming")
          } else {
            Swal.fire('Fail!', "", "error");
          }
        })
      })
  }
}
