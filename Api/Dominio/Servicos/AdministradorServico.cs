using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? Login(LoginDto loginDto)
    {
        return _contexto.Administradores
            .FirstOrDefault(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha);
    }

    public void Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
    }

    public List<Administrador> Todos(int pagina = 1)
    {
        var query = _contexto.Administradores.AsQueryable();

        var itensPorPagina = 10;

        query = query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores.Find(id);
    }
}