
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, Validators, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { EntitiesService } from '../core/services/entities.service';
import { map, of, switchMap, timer } from 'rxjs';

export interface EntityDialogData { id?: number; name?: string; address?: string; category?: string; }

@Component({
  selector: 'app-entity-dialog',
  templateUrl: './entity-dialog.component.html',
  styleUrls: ['./entity-dialog.component.css']
})
export class EntityDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EntityDialogData,
    private dialogRef: MatDialogRef<EntityDialogComponent>,
    private fb: FormBuilder,
    private entities: EntitiesService
  ) {}

  uniqueNameValidator(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      const name = (control.value || '').trim();
      if (!name) return of(null);
      return timer(300).pipe(
        switchMap(() => this.entities.existsByName(name)),
        map(exists => exists && name.toLowerCase() !== (this.data?.name || '').toLowerCase() ? { nameTaken: true } : null)
      );
    };
  }

  form = this.fb.group({
    name: [this.data?.name ?? '', { validators: [Validators.required], asyncValidators: [this.uniqueNameValidator()], updateOn: 'blur' }],
    address: [this.data?.address ?? '', [Validators.required]],
    category: [this.data?.category ?? '', [Validators.required]],
  });

  save() { if (this.form.invalid) return; this.dialogRef.close(this.form.value); }
  close() { this.dialogRef.close(); }
}
