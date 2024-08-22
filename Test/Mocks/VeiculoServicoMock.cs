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
    
    public List<Veiculo> Todos(int pagina = 1, string? nome = null, string? marca = null)
    {
        return _veiculos;
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _veiculos.Find(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = _veiculos.Count + 1;
        _veiculos.Add(veiculo);
    }

    public void Atualizar(Veiculo veiculo)
    {
        _veiculos = _veiculos
            .Where(v => v.Id == veiculo.Id)
            .Select(_ => veiculo)
            .ToList();
    }

    public void Apagar(Veiculo veiculo)
    {
        var veiculoLista = BuscaPorId(veiculo.Id);
        _veiculos.Remove(veiculoLista!);
    }
}