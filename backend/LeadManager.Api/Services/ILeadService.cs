using LeadManager.Api.Models;

namespace LeadManager.Api.Services;

public interface ILeadService
{
    Task<IReadOnlyCollection<Lead>> GetLeadsAsync(LeadStatus status, CancellationToken cancellationToken = default);
    Task<Lead?> AcceptLeadAsync(int id, CancellationToken cancellationToken = default);
    Task<Lead?> DeclineLeadAsync(int id, CancellationToken cancellationToken = default);
}
