using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LimsAuth.Api.Models;
using LimsAuth.Api.Services;

namespace LimsAuth.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InstitutionsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionsController(IInstitutionService institutionService)
    {
        _institutionService = institutionService;
    }

    [HttpGet]
    [Authorize(Policy = "Permission:institution:read")]
    public async Task<IActionResult> GetTree()
    {
        var result = await _institutionService.GetTreeAsync();
        return StatusCode(result.Code, result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Permission:institution:read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _institutionService.GetByIdAsync(id);
        return StatusCode(result.Code, result);
    }

    [HttpPost]
    [Authorize(Policy = "Permission:institution:create")]
    public async Task<IActionResult> Create([FromBody] CreateInstitutionRequest request)
    {
        var result = await _institutionService.CreateAsync(request);
        return StatusCode(result.Code, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Permission:institution:update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInstitutionRequest request)
    {
        var result = await _institutionService.UpdateAsync(id, request);
        return StatusCode(result.Code, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Permission:institution:delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _institutionService.DeleteAsync(id);
        return StatusCode(result.Code, result);
    }
}
