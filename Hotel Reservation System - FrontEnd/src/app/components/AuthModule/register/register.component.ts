import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { IRegister } from '../../../models/IRegister';
import { AuthService } from '../../../services/authentication/Auth.service';
import { ErrorMessageService } from '../../../services/error/error.service';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  userData: IRegister = {} as IRegister;
  isLoading = false;
  error = '';
  constructor(private auth: AuthService, private router: Router, private errorService: ErrorMessageService) { }

  onSubmit() {
    this.isLoading = true;
    this.error = '';
    this.auth.register(this.userData).subscribe({
      next: () => {
        this.isLoading = false,
          alert('Registered successfully!'),
          this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorService.showErrorMessage(err);
      }
    });
  }
}
