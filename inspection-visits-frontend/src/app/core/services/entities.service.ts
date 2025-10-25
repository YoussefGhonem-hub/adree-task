
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { EntityToInspect } from '../../shared/models';
import { Observable, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class EntitiesService {
  private base = `${environment.apiBaseUrl}/api/entities`;
  constructor(private http: HttpClient) {}
  list(category?: string) { let params = new HttpParams(); if (category) params = params.set('category', category); return this.http.get<EntityToInspect[]>(this.base, { params }); }
  create(body: Omit<EntityToInspect, 'id'>) { return this.http.post<EntityToInspect>(this.base, body); }
  update(id: number, body: Omit<EntityToInspect, 'id'>) { return this.http.put<EntityToInspect>(`${this.base}/${id}`, body); }
  delete(id: number) { return this.http.delete<void>(`${this.base}/${id}`); }
  existsByName(name: string): Observable<boolean> { return this.list().pipe(map(items => items.some(e => e.name.toLowerCase() === name.toLowerCase()))); }
}
