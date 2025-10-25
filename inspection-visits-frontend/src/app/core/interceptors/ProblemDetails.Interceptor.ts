import { Injectable } from '@angular/core';
import {
    HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse
} from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { extractProblemMessage } from '../utils/problem-details';

@Injectable()
export class ProblemDetailsInterceptor implements HttpInterceptor {
    constructor(private snack: MatSnackBar) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((err: HttpErrorResponse) => {
                // Only handle HTTP errors
                if (err instanceof HttpErrorResponse) {
                    const contentType = err.headers.get('content-type') || '';
                    // Detect RFC 7807 (or any body that looks like problem+json)
                    const looksLikeProblem = contentType.includes('application/problem+json')
                        || (err.error && (err.error.title || err.error.errors));

                    if (looksLikeProblem) {
                        const message = extractProblemMessage(err.error);
                        this.snack.open(message, 'Dismiss', { duration: 5000 });
                    } else if (err.status >= 400) {
                        // Fallback for non-problem JSON errors
                        const fallback = typeof err.error === 'string'
                            ? err.error
                            : err.message || `Request failed (${err.status})`;
                        this.snack.open(fallback, 'Dismiss', { duration: 5000 });
                    }
                }
                return throwError(() => err);
            })
        );
    }
}
