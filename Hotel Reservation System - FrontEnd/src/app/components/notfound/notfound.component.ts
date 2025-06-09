import { Component } from '@angular/core';
import { RouterLink } from '@angular/router'; // Import RouterLink
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-notfound',
  standalone: true, 
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './notfound.component.html',
  styleUrl: './notfound.component.css'
})
export class NotfoundComponent {

}
