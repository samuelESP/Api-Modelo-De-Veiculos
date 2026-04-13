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
app.MapGet("/Usuario/ObterPorId/{id}", async (int id, IUsuario usuarioService) =>
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




app.UseSwagger();
app.UseSwaggerUI();

app.Run();