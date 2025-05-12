/// <reference types="@angular/localize" />

import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    // Essential providers for standalone apps:
    provideRouter(routes), // If using router
    provideAnimations(),   // If using Material/animations

    // Include your appConfig providers
    ...(appConfig?.providers || [])
  ]
})
  .catch((err) => console.error(err));
