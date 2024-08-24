namespace MinimalApi.Dominio.ModelViews;

public record Home
{
    public string Mensagem => "Bem-vindo à API de veículos - Minimal API";
    public string Doc => "/swagger";
}