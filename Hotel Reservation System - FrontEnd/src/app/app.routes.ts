import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { NotfoundComponent } from './components/notfound/notfound.component';
import { ReservationComponent } from './components/ReservationModule/allReservations/reservation.component';
import { FeedbackComponent } from './components/FeedbackModule/allFeedbacks/feedback.component';
import { AuthGuard } from './guards/auth.guard';
import { AllUpcomingReservationComponent } from './components/ReservationModule/allUpcomingReservation/allUpcomingReservation.component';
import { MyReservationsComponent } from './components/ReservationModule/myReservations/myReservations.component';
import { MyUpcomingReservationsComponent } from './components/ReservationModule/myUpcomingReservations/myUpcomingReservations.component';
import { OfferComponent } from './components/offer/offer.component';
import { RoomComponent } from './components/RoomModule/allRooms/room.component';

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

    {
        path: 'dashboard',
        loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
        canActivate: [AuthGuard]
    },
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: "Feedback", component: FeedbackComponent },
    { path: "Reservation/All", component: ReservationComponent },
    { path: "Reservation/allUpcoming", component: AllUpcomingReservationComponent },
    { path: "Reservation/My", component: MyReservationsComponent },
    { path: "Reservation/MyUpcoming", component: MyUpcomingReservationsComponent },
    { path: "Room/All", component: RoomComponent },
    { path: "**", component: NotfoundComponent }
];
