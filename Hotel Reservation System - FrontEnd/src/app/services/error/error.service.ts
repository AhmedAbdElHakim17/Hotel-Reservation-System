import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({ providedIn: 'root' })
export class ErrorMessageService {
  showErrorMessage(err: any): void {
    let errorMessage = 'An unexpected error occurred';

    if (err.error && err.error.errors) {
      const validationErrors = [];

      for (const field in err.error.errors) {
        if (err.error.errors[field] && Array.isArray(err.error.errors[field])) {
          validationErrors.push(...err.error.errors[field]);
        }
      }

      if (validationErrors.length > 0) {
        errorMessage = validationErrors.join(', ');
      }
    }
    else if (err.error && err.error.message) {
      errorMessage = err.error.message;
    }
    else if (err.error) {
      errorMessage = err.error;
    }

    Swal.fire('Error!', errorMessage, "error");
  }
}