
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { InspectionVisit } from '../shared/models';

@Component({
  selector: 'app-update-status-dialog',
  templateUrl: './update-status-dialog.component.html',
  styleUrls: ['./update-status-dialog.component.css']
})
export class UpdateStatusDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { visit: InspectionVisit },
    private dialogRef: MatDialogRef<UpdateStatusDialogComponent>,
    private fb: FormBuilder
  ) {}

  form = this.fb.group({
    status: [this.data.visit.status, Validators.required],
    score: [this.data.visit.score ?? null as number | null],
    notes: [this.data.visit.notes ?? '']
  });

  get showScore() { return this.form.value.status === 'Completed'; }

  save() {
    if (this.form.invalid) return;
    const v = this.form.value;
    this.dialogRef.close({ status: v.status, score: v.status === 'Completed' ? v.score : null, notes: v.notes });
  }
  close() { this.dialogRef.close(); }
}
