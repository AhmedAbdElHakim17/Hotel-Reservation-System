import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from '../../services/authentication/Auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, NgbDropdownModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  constructor(private auth: AuthService, private router: Router) { }

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
    }
  }
}
