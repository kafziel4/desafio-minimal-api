using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;

    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome, $"%{nome}%"));
        }
        
        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => EF.Functions.Like(v.Marca, $"%{marca}%"));
        }

        var itensPorPagina = 10;

        query = query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _contexto.Veiculos.Find(id);
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }
}