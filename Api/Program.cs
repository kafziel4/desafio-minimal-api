using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = HeaderNames.Authorization,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = JwtConstants.TokenType,
        In = ParameterLocation.Header,
        Description = "Insira o JWT aqui"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

var stringConexao = builder.Configuration.GetConnectionString("Mysql");

builder.Services.AddDbContext<DbContexto>(options => 
    options.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao)));

var key = builder.Configuration.GetValue<string>("Jwt:Key")!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

#region Home
app.MapGet("/", () => Results.Json(new Home()))
    .WithTags("Home");
#endregion

#region Administradores

string GerarTokenJwt(Administrador administrador)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.Email, administrador.Email),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };
    
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credential);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login", (
    [FromBody] LoginDto loginDto,
    IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDto);
    
    if (administrador is null)
    {
        return Results.Unauthorized();
    }

    var token = GerarTokenJwt(administrador);

    return Results.Ok(new AdministradorLogado
    {
        Email = administrador.Email,
        Perfil = administrador.Perfil,
        Token = token
    });
}).WithTags("Administradores");

app.MapPost("/administradores", (
    [FromBody] AdministradorDto administradorDto,
    IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(administradorDto.Email))
    {
        validacao.Mensagens.Add("Email não pode ficar em branco");
    }
    
    if (string.IsNullOrWhiteSpace(administradorDto.Senha))
    {
        validacao.Mensagens.Add("Senha não pode ficar em branco");
    }
    
    if (!Enum.IsDefined(typeof(Perfil), administradorDto.Perfil))
    {
        validacao.Mensagens.Add("Perfil inválido");
    }

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador
    {
        Email = administradorDto.Email,
        Senha = administradorDto.Senha,
        Perfil = administradorDto.Perfil.ToString()
    };
    
    administradorServico.Incluir(administrador);

    return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var administradores = pagina is > 0
        ? administradorServico.Todos(pagina.Value)
        : administradorServico.Todos();

    var administradoresModelView = administradores
        .Select(a => new AdministradorModelView
        {
            Id = a.Id,
            Email = a.Email,
            Perfil = a.Perfil
        }).ToList();
    
    return Results.Ok(administradoresModelView);
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);

    if (administrador is null)
    {
        return Results.NotFound();
    }
    
    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidacao ValidaDto(VeiculoDto veiculoDto)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = []
    };

    if (string.IsNullOrWhiteSpace(veiculoDto.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ficar em branco");
    }
    
    if (string.IsNullOrWhiteSpace(veiculoDto.Marca))
    {
        validacao.Mensagens.Add("A marca não pode ficar em branco");
    }
    
    if (veiculoDto.Ano < 1950)
    {
        validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");
    }

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDto(veiculoDto);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    
    var veiculo = new Veiculo
    {
        Nome = veiculoDto.Nome,
        Marca = veiculoDto.Marca,
        Ano = veiculoDto.Ano
    };
    
    veiculoServico.Incluir(veiculo);
    
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = pagina is > 0
        ? veiculoServico.Todos(pagina.Value)
        : veiculoServico.Todos();
    
    return Results.Ok(veiculos);
})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    
    return veiculo is null ? Results.NotFound() : Results.Ok(veiculo);
})
.RequireAuthorization()
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDto veiculoDto, IVeiculoServico veiculoServico) =>
{
    var validacao = ValidaDto(veiculoDto);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }
    
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo is null)
    {
        return Results.NotFound();
    }

    veiculo.Nome = veiculoDto.Nome;
    veiculo.Marca = veiculoDto.Marca;
    veiculo.Ano = veiculoDto.Ano;
    
    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo is null)
    {
        return Results.NotFound();
    }
    
    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
})
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Veiculos");
#endregion

app.Run();

public partial class Program;
