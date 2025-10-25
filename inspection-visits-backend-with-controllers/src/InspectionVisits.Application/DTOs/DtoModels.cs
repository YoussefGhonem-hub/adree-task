using InspectionVisits.Domain.Enums;

namespace InspectionVisits.Application.DTOs;

public record EntityToInspectDto(int Id, string Name, string Address, string Category);
public record InspectorDto(int Id, string FullName, string Email, string Phone, string Role);
public record ViolationDto(int Id, string Code, string Description, ViolationSeverity Severity);
public record InspectionVisitDto(int Id, int EntityToInspectId, int InspectorId, DateTime ScheduledAt, VisitStatus Status, int? Score, string? Notes, List<ViolationDto> Violations);
