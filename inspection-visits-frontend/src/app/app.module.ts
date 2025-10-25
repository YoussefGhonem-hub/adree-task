
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './root/app.component';
import { MaterialModule } from './core/material/material.module';
import { JwtInterceptor } from './core/interceptors/jwt.interceptor';
import { AuthErrorInterceptor } from './core/interceptors/auth-error.interceptor';

import { LoginComponent } from './login/login.component';
import { EntitiesComponent } from './entities/entities.component';
import { EntityDialogComponent } from './entities/entity-dialog.component';
import { VisitsComponent } from './visits/visits.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ScheduleVisitDialogComponent } from './visits/schedule-visit-dialog.component';
import { UpdateStatusDialogComponent } from './visits/update-status-dialog.component';
import { AddViolationDialogComponent } from './visits/add-violation-dialog.component';
import { ProblemDetailsInterceptor } from './core/interceptors/ProblemDetails.Interceptor';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    EntitiesComponent,
    EntityDialogComponent,
    VisitsComponent,
    DashboardComponent,
    ScheduleVisitDialogComponent,
    UpdateStatusDialogComponent,
    AddViolationDialogComponent
  ],
  imports: [
    BrowserModule, BrowserAnimationsModule, HttpClientModule, ReactiveFormsModule, FormsModule, MaterialModule, AppRoutingModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AuthErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ProblemDetailsInterceptor, multi: true }, // <= here

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
