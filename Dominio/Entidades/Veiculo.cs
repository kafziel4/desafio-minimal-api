using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    public required string Nome { get; set; }

    [StringLength(100)]
    public required string Marca { get; set; }

    public int Ano { get; set; }
}