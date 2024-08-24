using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServicoTest
{
    private DbContexto _context = default!;

    [TestInitialize]
    public void Setup()
    {
        _context = CriarContextoDeTeste();
        _context.Database.Migrate();
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");
    }

    private DbContexto CriarContextoDeTeste()
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        return new DbContexto(options);
    }

    [TestMethod]
    public async Task TestandoSalvarVeiculo()
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
        await veiculoServico.Incluir(veiculo);

        // Assert
        var veiculos = await veiculoServico.Todos();
        Assert.AreEqual(1, veiculos.Count);
    }

    [TestMethod]
    public async Task TestandoBuscaPorId()
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
        await veiculoServico.Incluir(veiculo);

        // Act
        var veiculoDoBanco = await veiculoServico.BuscaPorId(veiculo.Id);

        // Assert
        Assert.AreEqual(1, veiculoDoBanco?.Id);
    }

    [TestMethod]
    public async Task TestandoAtualizar()
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
        await veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = await veiculoServico.BuscaPorId(veiculo.Id);

        veiculoDoBanco!.Nome = "Fiesta Sedan";

        // Act
        await veiculoServico.Atualizar(veiculoDoBanco);

        // Assert
        var veiculoAtualizado = await veiculoServico.BuscaPorId(veiculo.Id);
        Assert.AreEqual("Fiesta Sedan", veiculoAtualizado?.Nome);
    }

    [TestMethod]
    public async Task TestandoApagar()
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
        await veiculoServico.Incluir(veiculo);
        var veiculoDoBanco = await veiculoServico.BuscaPorId(veiculo.Id);

        // Act
        await veiculoServico.Apagar(veiculoDoBanco!);

        // Assert
        var veiculoApagado = await veiculoServico.BuscaPorId(veiculo.Id);
        Assert.AreEqual(null, veiculoApagado);
    }
}
