using InspectionVisits.Application.DTOs;
using InspectionVisits.Application.Features.Visits.Commands;
using InspectionVisits.Application.Features.Visits.Queries;
using InspectionVisits.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspectionVisits.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VisitsController : ControllerBase
{
    private readonly IMediator _mediator;
    public VisitsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public Task<List<InspectionVisitDto>> List(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] VisitStatus? status,
        [FromQuery] int? inspectorId,
        [FromQuery] string? category)
        => _mediator.Send(new ListVisits(from, to, status, inspectorId, category));

    public record ScheduleVisitBody(int EntityToInspectId, int InspectorId, DateTime ScheduledAt);
    public record UpdateStatusBody(VisitStatus NewStatus, int? Score, string? Notes);
    public record AddViolationBody(string Code, string Description, ViolationSeverity Severity);

    [HttpPost("schedule")]
    [Authorize(Roles = "Admin,Inspector")]
    public Task<InspectionVisitDto> Schedule([FromBody] ScheduleVisitBody body)
        => _mediator.Send(new ScheduleVisit(body.EntityToInspectId, body.InspectorId, body.ScheduledAt));

    [HttpPost("{id:int}/status")]
    [Authorize(Roles = "Admin,Inspector")]
    public Task<InspectionVisitDto> UpdateStatus([FromRoute] int id, [FromBody] UpdateStatusBody body)
        => _mediator.Send(new UpdateVisitStatus(id, body.NewStatus, body.Score, body.Notes));

    [HttpPost("{id:int}/violations")]
    [Authorize(Roles = "Admin,Inspector")]
    public Task<ViolationDto> AddViolation([FromRoute] int id, [FromBody] AddViolationBody body)
        => _mediator.Send(new AddViolation(id, body.Code, body.Description, body.Severity));
}
