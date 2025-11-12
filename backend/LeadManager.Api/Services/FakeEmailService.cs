using System.Globalization;
using System.IO;
using LeadManager.Api.Models;

namespace LeadManager.Api.Services;

public class FakeEmailService : IEmailService
{
    private const string Recipient = "sales@test.com";

    public async Task SendLeadAcceptedNotificationAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "notifications");
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, "email-log.txt");

        var message = $"[{DateTime.UtcNow:O}] To: {Recipient} | Lead: {lead.Id} | Price: {lead.Price.ToString("C", CultureInfo.InvariantCulture)}{Environment.NewLine}";

        await File.AppendAllTextAsync(filePath, message, cancellationToken);
    }
}
