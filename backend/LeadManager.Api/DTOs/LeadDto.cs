using LeadManager.Api.Models;

namespace LeadManager.Api.DTOs;

public record LeadDto(
    int Id,
    string ContactFirstName,
    string ContactLastName,
    string ContactFullName,
    string ContactPhoneNumber,
    string ContactEmail,
    DateTime CreatedAt,
    string Suburb,
    string Category,
    string Description,
    decimal Price,
    LeadStatus Status)
{
    public static LeadDto FromModel(Lead lead)
    {
        var fullName = string.Join(" ", new[] { lead.ContactFirstName, lead.ContactLastName }
            .Where(part => !string.IsNullOrWhiteSpace(part)));

        return new LeadDto(
            lead.Id,
            lead.ContactFirstName,
            lead.ContactLastName,
            fullName,
            lead.ContactPhoneNumber,
            lead.ContactEmail,
            lead.CreatedAt,
            lead.Suburb,
            lead.Category,
            lead.Description,
            lead.Price,
            lead.Status);
    }
}
