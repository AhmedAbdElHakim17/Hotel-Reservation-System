import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/authentication/Auth.service';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { IUser } from '../../../models/IUser';
import { finalize } from 'rxjs';
import Swal from 'sweetalert2';
import { ErrorMessageService } from '../../../services/error/error.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, FormsModule, NgxSkeletonLoaderModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  user: IUser = {} as IUser;
  isLoading = false;
  error = '';

  constructor(private auth: AuthService, private router: Router, private errorService: ErrorMessageService) { }

  onSubmit() {
    this.isLoading = true;
    this.error = '';
    this.auth.login(this.user).pipe(
      finalize(() => this.isLoading = false)
    ).subscribe({
      next: () => {
        this.auth.startAutoLogoutWatcher(); // Optional
        this.auth.notifyAuthChanged();
        this.router.navigate(['/Home']);
      },
      error: (err) => {
        this.errorService.showErrorMessage(err);
      }
    });
  }
}
