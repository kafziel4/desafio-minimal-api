using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;

namespace MinimalApi.Extensions;

public static class DomainServicesExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        return services;
    }
}