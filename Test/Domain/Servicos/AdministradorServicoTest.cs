using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto _context = default!;

    [TestInitialize]
    public void Setup()
    {
        _context = CriarContextoDeTeste();
        _context.Database.Migrate();
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
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
    public async Task TestandoSalvarAdministrador()
    {
        // Arrange
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorServico(_context);

        // Act
        await administradorServico.Incluir(adm);

        // Assert
        var administradores = await administradorServico.Todos();
        Assert.AreEqual(1, administradores.Count);
    }

    [TestMethod]
    public async Task TestandoBuscaPorId()
    {
        // Arrange
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorServico(_context);
        await administradorServico.Incluir(adm);

        // Act
        var admDoBanco = await administradorServico.BuscaPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }

    [TestMethod]
    public async Task TestandoLogin()
    {
        // Arrange
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var loginDto = new LoginDto(adm.Email, adm.Senha);

        var administradorServico = new AdministradorServico(_context);
        await administradorServico.Incluir(adm);

        // Act
        var admDoBanco = await administradorServico.Login(loginDto);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }
}