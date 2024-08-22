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
    
    public Administrador? Login(LoginDto loginDto)
    {
        return _administradores.Find(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha);
    }

    public void Incluir(Administrador administrador)
    {
        administrador.Id = _administradores.Count + 1;
        _administradores.Add(administrador);
    }

    public List<Administrador> Todos(int pagina = 1)
    {
        return _administradores;
    }

    public Administrador? BuscaPorId(int id)
    {
        return _administradores.Find(a => a.Id == id);
    }
}