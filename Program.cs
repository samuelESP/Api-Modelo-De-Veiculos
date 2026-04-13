using Domain.DTO;
using Domain.Interface;
using Domain.Servicos;
using Dominios.Entidades;
using Infra.Db;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


#region Builder
var builder = WebApplication.CreateBuilder(args);

// ====================== DbContext ======================
#region DbContext
var connectionString = builder.Configuration.GetConnectionString("mysql");

builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
#endregion


builder.Services.AddScoped<IUsuario, UsuarioServico>();
builder.Services.AddScoped<IVeiculo, VeiculoServico>();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração para serializar enums como strings
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{options.SerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());});

var app = builder.Build();
#endregion

// ====================== Usuário ======================
#region Usuario

//Criar
app.MapPost("/Usuario/Criar", async ([FromBody] LoginDto loginDto, IUsuario usuarioService) =>
{
    await usuarioService.CriarUsuarioAsync(loginDto);
}).WithTags("Usuário");

//obeter por ID
app.MapGet("/Usuario/ObterPorId/{id}", async ([FromRoute]int id, IUsuario usuarioService) =>
{
    var usuario = await usuarioService.ObterUsuarioPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuario);
}).WithTags("Usuário");

//Listar todos
app.MapGet("/Usuario/Listar", async (IUsuario usuarioService) =>
{
    var usuarios = await usuarioService.ListarUsuariosAsync();
    return Results.Ok(usuarios);
}).WithTags("Usuário");

#endregion


// ====================== Veiculos ======================
#region  Veiculos

//Listar Veiculos
app.MapGet("/Veiculo/Listar", async (IVeiculo veiculoService) =>
{
    var veiculos = await veiculoService.ListarVeiculosAsync();
    return Results.Ok(veiculos);
}).WithTags("Veículo");

//Listar Veiculo por ID
app.MapGet("/Veiculo/ObterPorId/{id}", async ([FromRoute]int id, IVeiculo veiculoService) =>
{
    var veiculo = await veiculoService.ObterVeiculoPorIdAsync(id);
    if (veiculo == null)
    {
        return Results.NotFound("Veículo não encontrado.");
    }
    return Results.Ok(veiculo);
}).WithTags("Veículo");


//Listar Veiculo por nome
app.MapGet("/Veiculo/ObterPorNome", async ([FromQuery] string nome, IVeiculo veiculoService) =>
{
    var veiculo = await veiculoService.ObterVeiculoPorNomeAsync(nome);
    if (veiculo == null)
    {
        return Results.NotFound("Veículo não encontrado.");
    }
    return Results.Ok(veiculo);
}).WithTags("Veículo");
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.Run();