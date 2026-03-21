using Application.Common.Models;
using Application.WorkItems;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/work-items")]
public sealed class WorkItemsController(WorkItemService workItemService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<WorkItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<WorkItemDto>>> ListAsync(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        PagedResult<WorkItemDto> result = await workItemService.ListAsync(skip, take, cancellationToken);
        return Ok(new PagedResponse<WorkItemDto>(result.Items, result.Skip, result.Take));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemDto>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        WorkItemDto workItem = await workItemService.GetAsync(id, cancellationToken);
        return Ok(workItem);
    }

    [HttpPost]
    [ProducesResponseType<WorkItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkItemDto>> CreateAsync(
        [FromBody] CreateWorkItemHttpRequest request,
        CancellationToken cancellationToken)
    {
        WorkItemDto created = await workItemService.CreateAsync(
            new CreateWorkItemRequest(request.Title, request.Description),
            cancellationToken);

        return CreatedAtAction(nameof(GetAsync), new { id = created.Id }, created);
    }
}
