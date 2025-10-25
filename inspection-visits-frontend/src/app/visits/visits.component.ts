
import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder } from '@angular/forms';
import { VisitsService } from '../core/services/visits.service';
import { EntitiesService } from '../core/services/entities.service';
import { InspectorsService } from '../core/services/inspectors.service';
import { InspectionVisit, EntityToInspect, Inspector } from '../shared/models';
import { MatDialog } from '@angular/material/dialog';
import { ScheduleVisitDialogComponent } from './schedule-visit-dialog.component';
import { UpdateStatusDialogComponent } from './update-status-dialog.component';
import { AddViolationDialogComponent } from './add-violation-dialog.component';

@Component({
  selector: 'app-visits',
  templateUrl: './visits.component.html',
  styleUrls: ['./visits.component.css']
})
export class VisitsComponent implements OnInit {
  constructor(
    private service: VisitsService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private entitiesService: EntitiesService,
    private inspectorsService: InspectorsService
  ) { }

  ds = new MatTableDataSource<InspectionVisit>([]);
  cols = ['id', 'scheduledAt', 'status', 'inspectorId', 'score', 'actions'];
  total = 0;

  entities: EntityToInspect[] = [];
  inspectors: Inspector[] = [];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  inspectorsById: Record<number, string> = {};

  filters = this.fb.group({
    from: [null as Date | null],
    to: [null as Date | null],
    category: [''],
    status: [''],
    inspectorId: [null as number | null]
  });

  ngOnInit() {
    this.loadLookups();
    this.load();
  }

  loadLookups() {
    this.entitiesService.list().subscribe(e => this.entities = e);
    this.inspectorsService.list().subscribe(i => {
      this.inspectors = i;
      this.inspectorsById = {};
      for (const ins of i) this.inspectorsById[ins.id] = ins.fullName;
    });
  }

  load() {
    const f = this.filters.value;
    const from = f.from ? new Date(f.from).toISOString() : undefined;
    const to = f.to ? new Date(f.to).toISOString() : undefined;
    const pageIndex = this.paginator?.pageIndex ?? 0;
    const pageSize = this.paginator?.pageSize ?? 10;
    const sort = this.sort?.active ? `${this.sort.active} ${this.sort.direction}` : undefined;

    this.service.list({
      from, to,
      status: (f.status as any) || undefined,
      inspectorId: f.inspectorId || undefined,
      category: f.category || undefined,
      pageIndex, pageSize, sort
    }).subscribe(rows => {
      this.total = rows.length;
      this.ds.data = rows;
      if (this.paginator) this.ds.paginator = this.paginator;
      if (this.sort) this.ds.sort = this.sort;
    });
  }
  getInspectorName(id: number): string {
    return this.inspectorsById[id] || '';
  }
  openSchedule() {
    const ref = this.dialog.open(ScheduleVisitDialogComponent, { data: { entities: this.entities, inspectors: this.inspectors } });
    ref.afterClosed().subscribe(val => {
      if (!val) return;
      this.service.schedule(val.entityToInspectId, val.inspectorId, val.scheduledAt).subscribe(_ => this.load());
    });
  }

  openUpdateStatus(v: InspectionVisit) {
    const ref = this.dialog.open(UpdateStatusDialogComponent, { data: { visit: v } });
    ref.afterClosed().subscribe(val => { if (!val) return; this.service.updateStatus(v.id, val.status, val.score, val.notes).subscribe(_ => this.load()); });
  }

  openAddViolation(v: InspectionVisit) {
    const ref = this.dialog.open(AddViolationDialogComponent, { data: { visit: v } });
    ref.afterClosed().subscribe(val => { if (!val) return; this.service.addViolation(v.id, val.code, val.description, val.severity).subscribe(_ => this.load()); });
  }
}
