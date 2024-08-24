using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    Task<Administrador?> Login(LoginDto loginDto);
    Task<bool> Existe(string email);
    Task Incluir(Administrador administrador);
    Task<List<Administrador>> Todos(int? pagina = 1);
    Task<Administrador?> BuscaPorId(int id);
}