import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../services/authentication/Auth.service';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, NgbDropdownModule, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  role!: string | undefined;
  authSub!: Subscription;
  constructor(private auth: AuthService) { }

  ngOnInit() {
    this.updateRoleFromStorage();
    this.authSub = this.auth.authStatus$.subscribe(() => {
      this.updateRoleFromStorage();
    });
  }

  // New method to update role from storage
  updateRoleFromStorage() {
    if (this.auth.isLoggedIn()) {
      this.role = localStorage.getItem('role') ?? '';
    } else {
      this.role = undefined;
    }
  }
  
  // Add this method to check for role changes
  ngDoCheck() {
    const currentRole = localStorage.getItem('role');
    if (this.auth.isLoggedIn() && currentRole !== this.role) {
      this.role = currentRole ?? '';
    }
  }

  async logout() {
    const result = await Swal.fire({
      title: 'Logout?',
      text: 'Are you sure you want to logout?',
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Yes, logout!',
      cancelButtonText: 'Cancel',
      allowOutsideClick: false,
      allowEscapeKey: false,
      showLoaderOnConfirm: true,
    })
    if (result.isConfirmed) {
      this.auth.logout();
      this.role = undefined;
    }
  }

  isAuthenticated() {
    return this.auth.isAuthenticated();
  }

  ngOnDestroy() {
    this.authSub?.unsubscribe();
  }
}
