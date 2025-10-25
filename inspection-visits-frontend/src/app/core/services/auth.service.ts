
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private tokenKey = 'jwt_token';
  private rolesKey = 'roles';
  constructor(private http: HttpClient) {}
  login(email: string, password: string): Observable<{ token: string, roles: string[], expiresUtc?: string }> {
    return this.http.post<{ token: string, roles: string[], expiresUtc?: string }>(`${environment.apiBaseUrl}/api/auth/login`, { email, password });
  }
  setSession(token: string, roles: string[]) { localStorage.setItem(this.tokenKey, token); localStorage.setItem(this.rolesKey, JSON.stringify(roles)); }
  getToken(): string | null { return localStorage.getItem(this.tokenKey); }
  getRoles(): string[] { const raw = localStorage.getItem(this.rolesKey); return raw ? JSON.parse(raw) : []; }
  isAuthenticated(): boolean { return !!this.getToken(); }
  logout() { localStorage.removeItem(this.tokenKey); localStorage.removeItem(this.rolesKey); }
}
