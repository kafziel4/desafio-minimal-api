using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var stringConexao = configuration.GetConnectionString("Mysql");

        services.AddDbContext<DbContexto>(options =>
            options.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao)));

        return services;
    }
}