using LeadManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManager.Api.Data;

public class LeadDbContext : DbContext
{
    public LeadDbContext(DbContextOptions<LeadDbContext> options) : base(options)
    {
    }

    public DbSet<Lead> Leads => Set<Lead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Lead>()
            .Property(lead => lead.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Lead>().HasData(
            new Lead
            {
                Id = 1,
                ContactFirstName = "Maria",
                ContactLastName = "Silva",
                ContactEmail = "maria.silva@example.com",
                ContactPhoneNumber = "+55 11 99999-0001",
                CreatedAt = new DateTime(2024, 1, 12, 10, 30, 0, DateTimeKind.Utc),
                Suburb = "São Paulo",
                Category = "Pintura",
                Description = "Pintura de sala e quarto",
                Price = 450m,
                Status = LeadStatus.Invited
            },
            new Lead
            {
                Id = 2,
                ContactFirstName = "João",
                ContactLastName = "Pereira",
                ContactEmail = "joao.pereira@example.com",
                ContactPhoneNumber = "+55 21 98888-0002",
                CreatedAt = new DateTime(2024, 1, 18, 9, 0, 0, DateTimeKind.Utc),
                Suburb = "Rio de Janeiro",
                Category = "Eletricista",
                Description = "Instalação de luminárias",
                Price = 620m,
                Status = LeadStatus.Invited
            },
            new Lead
            {
                Id = 3,
                ContactFirstName = "Ana",
                ContactLastName = "Souza",
                ContactEmail = "ana.souza@example.com",
                ContactPhoneNumber = "+55 31 97777-0003",
                CreatedAt = new DateTime(2024, 1, 5, 15, 45, 0, DateTimeKind.Utc),
                Suburb = "Belo Horizonte",
                Category = "Jardinagem",
                Description = "Manutenção de jardim",
                Price = 300m,
                Status = LeadStatus.Accepted
            }
        );
    }
}
