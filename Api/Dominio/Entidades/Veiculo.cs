using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(100)]
    public string Marca { get; set; } = string.Empty;

    public int Ano { get; set; }
}