using System.Linq;
using LeadManager.Api.DTOs;
using LeadManager.Api.Models;
using LeadManager.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeadManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetLeads([FromQuery] LeadStatus? status, CancellationToken cancellationToken)
    {
        var resolvedStatus = status ?? LeadStatus.Invited;
        var leads = await _leadService.GetLeadsAsync(resolvedStatus, cancellationToken);
        return Ok(leads.Select(LeadDto.FromModel));
    }

    [HttpPost("{id:int}/accept")]
    public async Task<ActionResult<LeadDto>> AcceptLead(int id, CancellationToken cancellationToken)
    {
        var lead = await _leadService.AcceptLeadAsync(id, cancellationToken);

        if (lead is null)
        {
            return NotFound();
        }

        return Ok(LeadDto.FromModel(lead));
    }

    [HttpPost("{id:int}/decline")]
    public async Task<ActionResult<LeadDto>> DeclineLead(int id, CancellationToken cancellationToken)
    {
        var lead = await _leadService.DeclineLeadAsync(id, cancellationToken);

        if (lead is null)
        {
            return NotFound();
        }

        return Ok(LeadDto.FromModel(lead));
    }
}
