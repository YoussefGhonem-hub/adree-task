
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { InspectionVisit } from '../shared/models';

@Component({
  selector: 'app-add-violation-dialog',
  templateUrl: './add-violation-dialog.component.html',
  styleUrls: ['./add-violation-dialog.component.css']
})
export class AddViolationDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { visit: InspectionVisit },
    private dialogRef: MatDialogRef<AddViolationDialogComponent>,
    private fb: FormBuilder
  ) {}

  form = this.fb.group({
    code: ['', Validators.required],
    description: ['', Validators.required],
    severity: [1, Validators.required]
  });

  save() { if (this.form.invalid) return; this.dialogRef.close(this.form.value); }
  close() { this.dialogRef.close(); }
}
