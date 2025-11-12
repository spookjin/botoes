using LeadManager.Api.Models;

namespace LeadManager.Api.Services;

public interface IEmailService
{
    Task SendLeadAcceptedNotificationAsync(Lead lead, CancellationToken cancellationToken = default);
}
