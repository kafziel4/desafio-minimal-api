using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.ModelViews;
using System.Net;
using System.Net.Http.Json;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdministradorRequestTest
{
    private Setup _setup = default!;

    [TestInitialize]
    public void ClassInit()
    {
        _setup = new Setup();
    }

    [TestCleanup]
    public void ClassCleanup()
    {
        _setup.Dispose();
    }

    [TestMethod]
    public async Task TesteDeLogin()
    {
        // Arrange
        var loginDto = new LoginDto("adm@teste.com", "123456");

        // Act
        var response = await _setup.HttpClient.PostAsJsonAsync("/administradores/login", loginDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var admLogado = await response.Content.ReadFromJsonAsync<AdministradorLogado>();

        Assert.IsNotNull(admLogado?.Email);
        Assert.IsNotNull(admLogado.Perfil);
        Assert.IsNotNull(admLogado.Token);
    }

    [TestMethod]
    public async Task IncluirAdministrador()
    {
        // Arrange
        var admDto = new AdministradorDto("teste@teste.com", "123456", Perfil.Adm);

        await _setup.AddAuthorizationHeader();

        // Act
        var response = await _setup.HttpClient.PostAsJsonAsync("/administradores", admDto);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var admModelView = await response.Content.ReadFromJsonAsync<AdministradorModelView>();

        Assert.IsNotNull(admModelView?.Id);
        Assert.IsNotNull(admModelView.Email);
        Assert.IsNotNull(admModelView.Perfil);
    }

    [TestMethod]
    public async Task ListarAdministradores()
    {
        // Arrange
        await _setup.AddAuthorizationHeader();

        // Act
        var response = await _setup.HttpClient.GetAsync("/administradores");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var admsModelView = await response.Content.ReadFromJsonAsync<List<AdministradorModelView>>();

        Assert.AreEqual(2, admsModelView?.Count);
    }

    [TestMethod]
    public async Task BuscarAdministradorPorId()
    {
        // Arrange
        var id = 1;

        await _setup.AddAuthorizationHeader();

        // Act
        var response = await _setup.HttpClient.GetAsync($"/administradores/{id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var admModelView = await response.Content.ReadFromJsonAsync<AdministradorModelView>();

        Assert.AreEqual(id, admModelView?.Id);
    }
}