using Domain.DTO;
using Domain.Interface;
using Domain.Models;
using Domain.Servicos;
using Dominios.Entidades;
using Dominios.Servicos;
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
builder.Services.AddScoped<IAdministrador, AdministradorServico>();
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

//Atualizar Veiculo
app.MapPut("/Veiculo/Atualizar/{id}", async ([FromRoute] int id, [FromBody] VeiculoModel veiculoModel, IVeiculo veiculoService) =>
{
    var veiculoAtualizado = await veiculoService.AtualizarVeiculoAsync(id, veiculoModel);
    if (veiculoAtualizado == null)
    {
        return Results.NotFound("Veículo não encontrado para atualização.");
    }
    return Results.Ok(veiculoAtualizado);
}).WithTags("Veículo");

//Criar Veiculo
app.MapPost("/Veiculo/Criar", async ([FromBody] VeiculoModel veiculoModel, IVeiculo veiculoService) =>
{
    await veiculoService.CriarVeiculoAsync(veiculoModel);

    return Results.Ok("Veículo criado com sucesso.");
    
}).WithTags("Veículo");

//Remover Veiculo
app.MapDelete("/Veiculo/Remover/{id}", async ([FromRoute] int id, IVeiculo veiculoService) =>
{
    var resultado = await veiculoService.RemoverVeiculoAsync(id);
    if (!resultado)
    {
        return Results.NotFound("Veículo não encontrado para remoção.");
    }
    return Results.Ok("Veículo removido com sucesso.");
}).WithTags("Veículo");
#endregion

// ====================== Administrador ======================

//Obter usuario por ID
app.MapGet("/Administrador/ObterUsuarioPorId/{id}", async ([FromRoute] int id, IAdministrador administradorService) =>
{
    var usuario = await administradorService.ObterUsuarioPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuario);
}).WithTags("Administrador");

//Gerenciar usuario
app.MapPut("/Administrador/GerenciarUsuarios/{id}", async ([FromRoute] int id, [FromBody] AtualizarUsuarioAdministradorDTO atualizarUsuarioAdministradorDTO, IAdministrador administradorService) =>
{
   try
    {
        await administradorService.GerenciarUsuariosAsync(id, atualizarUsuarioAdministradorDTO);
        return Results.Ok("Usuário atualizado com sucesso.");
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound("Usuário não encontrado para atualização.");
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("Administrador");

//Remover usuario
app.MapDelete("/Administrador/RemoverUsuario/{id}", async ([FromRoute] int id, IAdministrador administradorService) =>
{
    await administradorService.RemoverUsuarioAsync(id);
    
    return Results.Ok("Usuário removido com sucesso.");

}).WithTags("Administrador");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();