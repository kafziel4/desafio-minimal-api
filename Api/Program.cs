using MinimalApi.Endpoints;
using MinimalApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSwaggerConfiguration()
    .AddAuthenticationAndAndAuthorization(builder.Configuration)
    .AddDomainServices()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app
    .UseSwaggerMiddlewares()
    .UseAuthenticationAndAuthorization()
    .MapHomeEndpoints()
    .MapAdministradoresEndpoints(builder.Configuration)
    .MapVeiculosEndpoints();

app.Run();

public partial class Program;