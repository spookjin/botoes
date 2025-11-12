using LeadManager.Api.Data;
using LeadManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManager.Api.Services;

public class LeadService : ILeadService
{
    private readonly LeadDbContext _dbContext;
    private readonly IEmailService _emailService;

    public LeadService(LeadDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(LeadStatus status, CancellationToken cancellationToken = default)
    {
        var leads = await _dbContext.Leads
            .AsNoTracking()
            .Where(lead => lead.Status == status)
            .OrderByDescending(lead => lead.CreatedAt)
            .ToListAsync(cancellationToken);

        return leads;
    }

    public async Task<Lead?> AcceptLeadAsync(int id, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(lead => lead.Id == id, cancellationToken);

        if (lead is null)
        {
            return null;
        }

        var wasAlreadyAccepted = lead.Status == LeadStatus.Accepted;

        if (!wasAlreadyAccepted)
        {
            lead.Status = LeadStatus.Accepted;

            if (lead.Price > 500m)
            {
                lead.Price = Math.Round(lead.Price * 0.9m, 2, MidpointRounding.AwayFromZero);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _emailService.SendLeadAcceptedNotificationAsync(lead, cancellationToken);
        }

        return lead;
    }

    public async Task<Lead?> DeclineLeadAsync(int id, CancellationToken cancellationToken = default)
    {
        var lead = await _dbContext.Leads.FirstOrDefaultAsync(lead => lead.Id == id, cancellationToken);

        if (lead is null)
        {
            return null;
        }

        lead.Status = LeadStatus.Declined;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return lead;
    }
}
