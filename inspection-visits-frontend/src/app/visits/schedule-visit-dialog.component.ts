
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { EntityToInspect, Inspector } from '../shared/models';

@Component({
  selector: 'app-schedule-visit-dialog',
  templateUrl: './schedule-visit-dialog.component.html',
  styleUrls: ['./schedule-visit-dialog.component.css']
})
export class ScheduleVisitDialogComponent {
  minDate = new Date(); // prevent past dates (tweak if you need past allowed)

  form = this.fb.group({
    entityToInspectId: [
      this.data?.defaults?.entityToInspectId ?? null,
      Validators.required
    ],
    inspectorId: [
      this.data?.defaults?.inspectorId ?? null,
      Validators.required
    ],
    scheduledAt: [
      this.data?.defaults?.scheduledAt ?? null,
      Validators.required
    ],
    // Optional note shown in the UI only (not required by API)
    note: ['']
  });

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<ScheduleVisitDialogComponent>,
    private fb: FormBuilder
  ) { }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.value;

    // Convert date to ISO string (backend expects string)
    const scheduledAtIso = new Date(v.scheduledAt as Date).toISOString();

    this.dialogRef.close({
      entityToInspectId: v.entityToInspectId,
      inspectorId: v.inspectorId,
      scheduledAt: scheduledAtIso,
      note: v.note // only if you want to pass it back to parent
    });
  }

  close(): void {
    this.dialogRef.close();
  }
}