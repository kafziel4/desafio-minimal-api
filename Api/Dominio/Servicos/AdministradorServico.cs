using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico(DbContexto contexto) : IAdministradorServico
{
    private const int ItensPorPagina = 10;

    public async Task<Administrador?> Login(LoginDto loginDto)
    {
        return await contexto.Administradores
            .FirstOrDefaultAsync(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha);
    }

    public async Task<bool> Existe(string email)
    {
        return await contexto.Administradores.AnyAsync(a => a.Email == email);
    }

    public async Task Incluir(Administrador administrador)
    {
        contexto.Administradores.Add(administrador);
        await contexto.SaveChangesAsync();
    }

    public async Task<List<Administrador>> Todos(int? pagina = 1)
    {
        if (pagina is null or <= 0)
        {
            pagina = 1;
        }

        return await contexto.Administradores
            .Skip((pagina.Value - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }

    public async Task<Administrador?> BuscaPorId(int id)
    {
        return await contexto.Administradores.FindAsync(id);
    }
}