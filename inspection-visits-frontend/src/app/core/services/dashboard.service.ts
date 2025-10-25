
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { DashboardVm } from '../../shared/models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(private http: HttpClient) {}
  get() { return this.http.get<DashboardVm>(`${environment.apiBaseUrl}/api/dashboard`); }
}
