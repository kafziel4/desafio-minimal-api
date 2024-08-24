using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
    Task<List<Veiculo>> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    Task<Veiculo?> BuscaPorId(int id);
    Task Incluir(Veiculo veiculo);
    Task Atualizar(Veiculo veiculo);
    Task Apagar(Veiculo veiculo);
}