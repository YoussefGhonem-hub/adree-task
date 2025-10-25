using InspectionVisits.Application.DTOs;
using InspectionVisits.Application.Features.Inspectors.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspectionVisits.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public InspectorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin,Inspector")]
    public async Task<ActionResult<IReadOnlyList<InspectorDto>>> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetInspectorsQuery(), ct));
}