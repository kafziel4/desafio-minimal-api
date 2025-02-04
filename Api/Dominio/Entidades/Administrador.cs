﻿using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Dominio.Entidades;

public class Administrador
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [StringLength(50)]
    public string Senha { get; set; } = string.Empty;

    [StringLength(10)]
    public string Perfil { get; set; } = string.Empty;
}