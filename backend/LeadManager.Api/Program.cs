using System.Text.Json.Serialization;
using LeadManager.Api.Data;
using LeadManager.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<LeadDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddSingleton<IEmailService, FakeEmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaClient", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("SpaClients").Get<string[]>() ??
                            new[] { "http://localhost:3000", "http://localhost:5173" })
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeadDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("SpaClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
