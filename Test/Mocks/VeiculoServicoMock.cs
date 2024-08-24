using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private List<Veiculo> _veiculos =
    [
        new Veiculo
        {
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2014
        }
    ];

    public Task<List<Veiculo>> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        return Task.FromResult(_veiculos);
    }

    public Task<Veiculo?> BuscaPorId(int id)
    {
        return Task.FromResult(_veiculos.Find(v => v.Id == id));
    }

    public Task Incluir(Veiculo veiculo)
    {
        veiculo.Id = _veiculos.Count + 1;
        _veiculos.Add(veiculo);
        return Task.CompletedTask;
    }

    public Task Atualizar(Veiculo veiculo)
    {
        _veiculos = _veiculos
            .Where(v => v.Id == veiculo.Id)
            .Select(_ => veiculo)
            .ToList();
        return Task.CompletedTask;
    }

    public Task Apagar(Veiculo veiculo)
    {
        _veiculos.Remove(veiculo);
        return Task.CompletedTask;
    }
}