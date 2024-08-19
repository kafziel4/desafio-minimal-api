var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDto loginDto) => 
    loginDto is { Email: "adm@test.com", Senha: "123456" }
        ? Results.Ok("Login com sucesso")
        : Results.Unauthorized());

app.Run();

public class LoginDto
{
    public required string Email { get; set; }
    public required string Senha { get; set; }
}
