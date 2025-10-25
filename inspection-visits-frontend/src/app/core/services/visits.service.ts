
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { InspectionVisit, VisitStatus, Violation, ViolationSeverity } from '../../shared/models';

@Injectable({ providedIn: 'root' })
export class VisitsService {
  private base = `${environment.apiBaseUrl}/api/visits`;
  constructor(private http: HttpClient) {}
  list(opts: { from?: string, to?: string, status?: VisitStatus, inspectorId?: number, category?: string, pageIndex?: number, pageSize?: number, sort?: string } = {}) {
    let params = new HttpParams();
    if (opts.from) params = params.set('from', opts.from);
    if (opts.to) params = params.set('to', opts.to);
    if (opts.status) params = params.set('status', opts.status as any);
    if (opts.inspectorId) params = params.set('inspectorId', opts.inspectorId);
    if (opts.category) params = params.set('category', opts.category);
    if (opts.pageIndex !== undefined) params = params.set('pageIndex', opts.pageIndex);
    if (opts.pageSize !== undefined) params = params.set('pageSize', opts.pageSize);
    if (opts.sort) params = params.set('sort', opts.sort);
    return this.http.get<InspectionVisit[]>(this.base, { params });
  }
  schedule(entityToInspectId: number, inspectorId: number, scheduledAt: string) { return this.http.post<InspectionVisit>(`${this.base}/schedule`, { entityToInspectId, inspectorId, scheduledAt }); }
  updateStatus(id: number, newStatus: VisitStatus, score?: number, notes?: string) { return this.http.post<InspectionVisit>(`${this.base}/${id}/status`, { newStatus, score, notes }); }
  addViolation(visitId: number, code: string, description: string, severity: ViolationSeverity) { return this.http.post<Violation>(`${this.base}/${visitId}/violations`, { code, description, severity }); }
}
