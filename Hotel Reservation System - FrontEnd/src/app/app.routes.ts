import { Routes } from '@angular/router';
import { AboutComponent } from './components/about/about.component';
import { HomeComponent } from './components/home/home.component';
import { NotfoundComponent } from './components/notfound/notfound.component';
import { FeedbackComponent } from './components/feedback/feedback.component';
import { AuthGuard } from './guards/auth.guard';
import { RoomComponent } from './components/Room/room.component';
import { OfferComponent } from './components/offer/offer.component';
import { PaymentComponent } from './components/payment/payment/payment.component';

export const routes: Routes = [
    { path: "", pathMatch: 'full', redirectTo: "/Home" },
    { path: "Home", component: HomeComponent },
    {
        path: 'login',
        loadComponent: () => import('./components/AuthModule/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'register',
        loadComponent: () => import('./components/AuthModule/register/register.component').then(m => m.RegisterComponent)
    },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: "Feedback", component: FeedbackComponent },
    {
        path: "Reservation",
        loadComponent: () => import('./components/reservation/reservation.component').then(m => m.ReservationComponent),
        canActivate: [AuthGuard]
    },
    { path: "Offer", component: OfferComponent },
    { path: "Room", component: RoomComponent },
    { path: 'About', component: AboutComponent },
    {
        path: 'Payment', component: PaymentComponent
        // loadComponent: () => import('./components/payment/payment/payment.component').then(m => m.PaymentComponent)
    },
    {
        path: 'reservation',
        loadComponent: () => import('./components/reservation/reservation.component').then(m => m.ReservationComponent)
    },
    { path: "**", component: NotfoundComponent },
];

