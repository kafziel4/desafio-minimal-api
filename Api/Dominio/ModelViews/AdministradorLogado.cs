namespace MinimalApi.Dominio.ModelViews;

public record AdministradorLogado
{
    public required string Email { get; set; }
    public required string Perfil { get; set; }
    public required string Token { get; set; }
}