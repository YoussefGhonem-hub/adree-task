
import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loading = false;
  form = this.fb.group({
    email: ['admin@demo.local', [Validators.required, Validators.email]],
    password: ['P@ssw0rd!', [Validators.required, Validators.minLength(3)]],
  });

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  submit() {
    if (this.form.invalid) return;
    this.loading = true;
    const { email, password } = this.form.value;
    this.auth.login(email!, password!).subscribe({
      next: res => { this.auth.setSession(res.token, res.roles || []); this.router.navigateByUrl('/dashboard'); },
      error: _ => { this.loading = false; },
      complete: () => { this.loading = false; }
    });
  }
}
