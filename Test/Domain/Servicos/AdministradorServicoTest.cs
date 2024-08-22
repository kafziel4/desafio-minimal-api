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
        _context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
    }

    [TestMethod]
    public void TestandoSalvarAdministrador()
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
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos().Count);
    }

    [TestMethod]
    public void TestandoBuscaPorId()
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
        administradorServico.Incluir(adm);

        // Act
        var admDoBanco = administradorServico.BuscaPorId(adm.Id)!;

        // Assert
        Assert.AreEqual(1, admDoBanco.Id);
    }

    [TestMethod]
    public void TestandoLogin()
    {
        // Arrange
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var loginDto = new LoginDto
        {
            Email = adm.Email,
            Senha = adm.Senha
        };

        var administradorServico = new AdministradorServico(_context);
        administradorServico.Incluir(adm);

        // Act
        var admDoBanco = administradorServico.Login(loginDto)!;

        // Assert
        Assert.AreEqual(1, admDoBanco.Id);
    }
}