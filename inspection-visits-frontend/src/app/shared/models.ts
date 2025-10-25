
export type VisitStatus = 'Planned' | 'InProgress' | 'Completed' | 'Cancelled';
export type ViolationSeverity = 0 | 1 | 2;
export interface EntityToInspect { id: number; name: string; address: string; category: string; }
export interface Inspector { id: number; fullName: string; email: string; phone: string; role: string; }
export interface Violation { id: number; code: string; description: string; severity: ViolationSeverity; }
export interface InspectionVisit {
  id: number; entityToInspectId: number; inspectorId: number;
  scheduledAt: string; status: VisitStatus; score?: number; notes?: string; violations: Violation[];
}
export interface DashboardVm { countsByStatus: Record<string, number>; averageScoreThisMonth: number; }
