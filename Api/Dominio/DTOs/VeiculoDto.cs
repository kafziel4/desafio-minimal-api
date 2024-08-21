namespace MinimalApi.Dominio.DTOs;

public record VeiculoDto
{
    public required string Nome { get; set; }
    public required string Marca { get; set; }
    public int Ano { get; set; }
}