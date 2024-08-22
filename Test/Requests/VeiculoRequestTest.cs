using System.Net;
using System.Net.Http.Json;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
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
    public async Task IncluirVeiculo()
    {
        // Arrange
        var veiculoDto = new VeiculoDto
        {
            Nome = "X6",
            Marca = "BMW",
            Ano = 2022
        };
        
        await _setup.AddAuthorizationHeader();
        
        // Act
        var response = await _setup.HttpClient.PostAsJsonAsync("/veiculos", veiculoDto);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var veiculo = await response.Content.ReadFromJsonAsync<Veiculo>();
        
        Assert.IsNotNull(veiculo!.Id);
        Assert.IsNotNull(veiculo.Nome);
        Assert.IsNotNull(veiculo.Marca);
        Assert.IsNotNull(veiculo.Ano);
    }
    
    [TestMethod]
    public async Task ListarVeiculos()
    {
        // Arrange
        await _setup.AddAuthorizationHeader();
        
        // Act
        var response = await _setup.HttpClient.GetAsync("/veiculos");
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var veiculos = await response.Content.ReadFromJsonAsync<List<Veiculo>>();
        
        Assert.AreEqual(1, veiculos!.Count);
    }
    
    [TestMethod]
    public async Task BuscarVeiculoPorId()
    {
        // Arrange
        var id = 1;
        
        await _setup.AddAuthorizationHeader();
        
        // Act
        var response = await _setup.HttpClient.GetAsync($"/veiculos/{id}");
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var veiculo = await response.Content.ReadFromJsonAsync<Veiculo>();
        
        Assert.IsNotNull(veiculo!.Id);
        Assert.IsNotNull(veiculo.Nome);
        Assert.IsNotNull(veiculo.Marca);
        Assert.IsNotNull(veiculo.Ano);
    }
    
    [TestMethod]
    public async Task AtualizarVeiculo()
    {
        // Arrange
        var id = 1;
        var veiculoDto = new VeiculoDto
        {
            Nome = "X6",
            Marca = "BMW",
            Ano = 2022
        };
        
        await _setup.AddAuthorizationHeader();
        
        // Act
        var response = await _setup.HttpClient.PutAsJsonAsync($"/veiculos/{id}", veiculoDto);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var veiculo = await response.Content.ReadFromJsonAsync<Veiculo>();
        
        Assert.IsNotNull(veiculo!.Id);
        Assert.IsNotNull(veiculo.Nome);
        Assert.IsNotNull(veiculo.Marca);
        Assert.IsNotNull(veiculo.Ano);
    }
    
    [TestMethod]
    public async Task ApagaVeiculo()
    {
        // Arrange
        var id = 1;
        
        await _setup.AddAuthorizationHeader();
        
        // Act
        var response = await _setup.HttpClient.DeleteAsync($"/veiculos/{id}");
        
        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}