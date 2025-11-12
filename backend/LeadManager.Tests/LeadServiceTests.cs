using System.Collections.Generic;
using System.Linq;
using LeadManager.Api.Data;
using LeadManager.Api.Models;
using LeadManager.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace LeadManager.Tests;

public class LeadServiceTests
{
    [Fact]
    public async Task AcceptLeadAsync_WithHighPrice_AppliesDiscountAndSendsNotification()
    {
        using var context = CreateContext(nameof(AcceptLeadAsync_WithHighPrice_AppliesDiscountAndSendsNotification));
        context.Leads.Add(new Lead
        {
            Id = 1,
            ContactFirstName = "Test",
            CreatedAt = DateTime.UtcNow,
            Suburb = "Cidade",
            Category = "Teste",
            Description = "Descrição",
            Price = 1000m,
            Status = LeadStatus.Invited
        });
        await context.SaveChangesAsync();

        var emailService = new TestEmailService();
        var service = new LeadService(context, emailService);

        var lead = await service.AcceptLeadAsync(1);

        Assert.NotNull(lead);
        Assert.Equal(LeadStatus.Accepted, lead!.Status);
        Assert.Equal(900m, lead.Price);
        Assert.Single(emailService.SentLeads);
        Assert.Equal(1, emailService.SentLeads.Single().Id);
    }

    [Fact]
    public async Task AcceptLeadAsync_WhenLeadDoesNotExist_ReturnsNull()
    {
        using var context = CreateContext(nameof(AcceptLeadAsync_WhenLeadDoesNotExist_ReturnsNull));
        var emailService = new TestEmailService();
        var service = new LeadService(context, emailService);

        var lead = await service.AcceptLeadAsync(42);

        Assert.Null(lead);
        Assert.Empty(emailService.SentLeads);
    }

    [Fact]
    public async Task AcceptLeadAsync_WhenLeadIsAlreadyAccepted_DoesNotSendNotificationAgain()
    {
        using var context = CreateContext(nameof(AcceptLeadAsync_WhenLeadIsAlreadyAccepted_DoesNotSendNotificationAgain));
        context.Leads.Add(new Lead
        {
            Id = 9,
            ContactFirstName = "Joana",
            CreatedAt = DateTime.UtcNow,
            Suburb = "Cidade",
            Category = "Teste",
            Description = "Descrição",
            Price = 540m,
            Status = LeadStatus.Accepted
        });
        await context.SaveChangesAsync();

        var emailService = new TestEmailService();
        var service = new LeadService(context, emailService);

        var lead = await service.AcceptLeadAsync(9);

        Assert.NotNull(lead);
        Assert.Equal(LeadStatus.Accepted, lead!.Status);
        Assert.Equal(540m, lead.Price);
        Assert.Empty(emailService.SentLeads);
    }

    [Fact]
    public async Task DeclineLeadAsync_UpdatesStatus()
    {
        using var context = CreateContext(nameof(DeclineLeadAsync_UpdatesStatus));
        context.Leads.Add(new Lead
        {
            Id = 5,
            ContactFirstName = "Maria",
            CreatedAt = DateTime.UtcNow,
            Suburb = "Cidade",
            Category = "Teste",
            Description = "Descrição",
            Price = 200m,
            Status = LeadStatus.Invited
        });
        await context.SaveChangesAsync();

        var emailService = new TestEmailService();
        var service = new LeadService(context, emailService);

        var lead = await service.DeclineLeadAsync(5);

        Assert.NotNull(lead);
        Assert.Equal(LeadStatus.Declined, lead!.Status);
        Assert.Empty(emailService.SentLeads);
    }

    private static LeadDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<LeadDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new LeadDbContext(options);
    }

    private sealed class TestEmailService : IEmailService
    {
        public List<Lead> SentLeads { get; } = new();

        public Task SendLeadAcceptedNotificationAsync(Lead lead, CancellationToken cancellationToken = default)
        {
            SentLeads.Add(lead);
            return Task.CompletedTask;
        }
    }
}
