
import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  constructor(private service: DashboardService) {}
  avg = 0; statusCounts: Record<string, number> = {};
  ngOnInit() { this.service.get().subscribe(d => { this.avg = d.averageScoreThisMonth ?? 0; this.statusCounts = d.countsByStatus ?? {}; }); }
}
