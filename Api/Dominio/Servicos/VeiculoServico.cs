using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico(DbContexto contexto) : IVeiculoServico
{
    private const int ItensPorPagina = 10;

    public async Task<List<Veiculo>> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        if (pagina is null or <= 0)
        {
            pagina = 1;
        }

        var query = contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome, $"%{nome}%"));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => EF.Functions.Like(v.Marca, $"%{marca}%"));
        }

        return await query
            .Skip((pagina.Value - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }

    public async Task<Veiculo?> BuscaPorId(int id)
    {
        return await contexto.Veiculos.FindAsync(id);
    }

    public async Task Incluir(Veiculo veiculo)
    {
        contexto.Veiculos.Add(veiculo);
        await contexto.SaveChangesAsync();
    }

    public async Task Atualizar(Veiculo veiculo)
    {
        contexto.Veiculos.Update(veiculo);
        await contexto.SaveChangesAsync();
    }

    public async Task Apagar(Veiculo veiculo)
    {
        contexto.Veiculos.Remove(veiculo);
        await contexto.SaveChangesAsync();
    }
}