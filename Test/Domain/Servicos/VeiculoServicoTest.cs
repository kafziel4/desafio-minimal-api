using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto _context = default!;

    private DbContexto CriarContextoDeTeste()
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        return new DbContexto(options);
    }

    [TestInitialize]
    public void Setup()
    {
        _context = CriarContextoDeTeste();
        _context.Database.Migrate();
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");
    }

    [TestMethod]
    public void TestandoSalvarVeiculo()
    {
        // Arrange
        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2014,
        };

        var veiculoServico = new VeiculoServico(_context);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.Todos().Count);
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2014,
        };

        var veiculoServico = new VeiculoServico(_context);
        veiculoServico.Incluir(veiculo);

        // Act
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id)!;

        // Assert
        Assert.AreEqual(1, veiculoDoBanco.Id);
    }

    [TestMethod]
    public void TestandoAtualizar()
    {
        // Arrange
        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2014,
        };

        var veiculoServico = new VeiculoServico(_context);
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id)!;

        veiculoDoBanco.Nome = "Fiesta Sedan";

        // Act
        veiculoServico.Atualizar(veiculoDoBanco);

        // Assert
        Assert.AreEqual("Fiesta Sedan", veiculoServico.BuscaPorId(veiculo.Id)!.Nome);
    }

    [TestMethod]
    public void TestandoApagar()
    {
        // Arrange
        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Fiesta",
            Marca = "Ford",
            Ano = 2014,
        };

        var veiculoServico = new VeiculoServico(_context);
        veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = veiculoServico.BuscaPorId(veiculo.Id)!;

        // Act
        veiculoServico.Apagar(veiculoDoBanco);

        // Assert
        Assert.AreEqual(null, veiculoServico.BuscaPorId(veiculo.Id));
    }
}
