using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BuildingsController : ControllerBase
{
    private readonly IBuildingService _buildingService;

    public BuildingsController(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:building:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _buildingService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:building:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _buildingService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:building:create")]
    public async Task<IActionResult> Create([FromBody] CreateBuildingRequest request)
    {
        var result = await _buildingService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:building:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBuildingRequest request)
    {
        var result = await _buildingService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:building:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _buildingService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:room:read")]
    public async Task<IActionResult> GetList([FromQuery] PagedQueryRequest query)
    {
        var result = await _roomService.GetListAsync(query);
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:room:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _roomService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:room:create")]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        var result = await _roomService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:room:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoomRequest request)
    {
        var result = await _roomService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:room:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}
