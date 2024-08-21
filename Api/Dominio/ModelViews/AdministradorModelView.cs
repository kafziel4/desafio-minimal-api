using MinimalApi.Dominio.Enums;

namespace MinimalApi.Dominio.ModelViews;

public record AdministradorModelView
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Perfil { get; set; }
}