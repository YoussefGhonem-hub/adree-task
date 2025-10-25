
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EntitiesService } from '../core/services/entities.service';
import { EntityToInspect } from '../shared/models';
import { EntityDialogComponent } from './entity-dialog.component';

@Component({
  selector: 'app-entities',
  templateUrl: './entities.component.html',
  styleUrls: ['./entities.component.css']
})
export class EntitiesComponent implements OnInit {
  rows: EntityToInspect[] = [];
  cols = ['id','name','address','category','actions'];

  constructor(private service: EntitiesService, private dialog: MatDialog) {}
  ngOnInit() { this.load(); }
  load() { this.service.list().subscribe(res => this.rows = res); }

  openCreate() { const ref = this.dialog.open(EntityDialogComponent, { data: {} }); ref.afterClosed().subscribe(val => { if (!val) return; this.service.create(val).subscribe(_ => this.load()); }); }
  openEdit(r: EntityToInspect) { const ref = this.dialog.open(EntityDialogComponent, { data: r }); ref.afterClosed().subscribe(val => { if (!val) return; this.service.update(r.id, val).subscribe(_ => this.load()); }); }
  remove(r: EntityToInspect) { if (!confirm('Delete this entity?')) return; this.service.delete(r.id).subscribe(_ => this.load()); }
}
