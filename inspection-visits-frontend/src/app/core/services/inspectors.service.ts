
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Inspector } from '../../shared/models';

@Injectable({ providedIn: 'root' })
export class InspectorsService {
  private base = `${environment.apiBaseUrl}/api/inspectors`;
  constructor(private http: HttpClient) {}
  list() { return this.http.get<Inspector[]>(this.base); }
}
