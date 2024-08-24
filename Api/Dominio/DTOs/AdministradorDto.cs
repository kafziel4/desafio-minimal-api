using MinimalApi.Dominio.Enums;

namespace MinimalApi.Dominio.DTOs;

public record AdministradorDto(string Email, string Senha, Perfil Perfil);