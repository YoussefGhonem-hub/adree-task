using InspectionVisits.Application.DTOs;
using InspectionVisits.Application.Features.Entities.Commands;
using InspectionVisits.Application.Features.Entities.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InspectionVisits.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class EntitiesController : ControllerBase
{
    private readonly IMediator _mediator;
    public EntitiesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public Task<List<EntityToInspectDto>> List([FromQuery] string? category) 
        => _mediator.Send(new ListEntitiesToInspect(category));

    public record UpsertEntityBody(string Name, string Address, string Category);

    [HttpPost]
    public Task<EntityToInspectDto> Create([FromBody] UpsertEntityBody body) 
        => _mediator.Send(new CreateEntityToInspect(body.Name, body.Address, body.Category));

    [HttpPut("{id:int}")]
    public Task<EntityToInspectDto> Update([FromRoute] int id, [FromBody] UpsertEntityBody body) 
        => _mediator.Send(new UpdateEntityToInspect(id, body.Name, body.Address, body.Category));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _mediator.Send(new DeleteEntityToInspect(id));
        return NoContent();
    }
}
