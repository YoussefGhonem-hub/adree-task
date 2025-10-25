export interface ProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    traceId?: string;
    errors?: Record<string, string[]>;
}

export function extractProblemMessage(pb: any): string {
    // Not strictly typed, be defensive
    const p = pb as ProblemDetails | undefined;

    if (!p) return 'Unexpected error';

    // If validation dictionary exists, flatten it
    const errs: string[] = [];
    if (p.errors && typeof p.errors === 'object') {
        for (const key of Object.keys(p.errors)) {
            const msgs = p.errors[key];
            if (Array.isArray(msgs)) {
                for (const m of msgs) errs.push(`${key}: ${m}`);
            }
        }
    }

    // Prefer validation messages if present
    if (errs.length) return errs.join('\n');

    // Otherwise fall back to title/detail, or status
    if (p.detail && p.title) return `${p.title}: ${p.detail}`;
    if (p.detail) return p.detail;
    if (p.title) return p.title;

    return `Request failed${p.status ? ` (${p.status})` : ''}`;
}
