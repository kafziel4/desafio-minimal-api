using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    public DbSet<Administrador> Administradores { get; set; }

    public DbContexto(DbContextOptions<DbContexto> options)
        : base(options)
    {
    }
}