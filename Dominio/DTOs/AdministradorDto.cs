using MinimalApi.Dominio.Enums;

namespace MinimalApi.Dominio.DTOs;

public class AdministradorDto
{
    public required string Email { get; set; }
    public required string Senha { get; set; }
    public Perfil Perfil { get; set; }
}