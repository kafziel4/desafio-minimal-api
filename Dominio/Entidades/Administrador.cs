using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Administrador
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(255)]
    public required string Email { get; set; }
    
    [StringLength(50)]
    public required string Senha { get; set; }
    
    [StringLength(10)]
    public required string Perfil { get; set; }
}