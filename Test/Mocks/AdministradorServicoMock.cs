using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Test.Mocks;

public class AdministradorServicoMock : IAdministradorServico
{
    private readonly List<Administrador> _administradores =
    [
        new Administrador
        {
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },
        new Administrador
        {
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    ];

    public Task<Administrador?> Login(LoginDto loginDto)
    {
        return Task.FromResult(
            _administradores.Find(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha));
    }

    public Task<bool> Existe(string email)
    {
        return Task.FromResult(_administradores.Exists(a => a.Email == email));
    }

    public Task Incluir(Administrador administrador)
    {
        administrador.Id = _administradores.Count + 1;
        _administradores.Add(administrador);
        return Task.CompletedTask;
    }

    public Task<List<Administrador>> Todos(int? pagina = 1)
    {
        return Task.FromResult(_administradores);
    }

    public Task<Administrador?> BuscaPorId(int id)
    {
        return Task.FromResult(_administradores.Find(a => a.Id == id));
    }
}