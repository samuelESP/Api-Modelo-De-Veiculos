using Domain.DTO;
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

var app = builder.Build();
#endregion

// ====================== Criar Usuário ======================
#region UsuarioCriar
app.MapPost("/Usuario/Criar", async ([FromBody] UsuarioDTO usuarioDto, DbContexto context) =>
{

    // 1. Validações de Senha e Email Null ou Whitespace
    if (string.IsNullOrWhiteSpace(usuarioDto.Email) || 
        string.IsNullOrWhiteSpace(usuarioDto.Senha))
    {
        return Results.BadRequest("Email e Senha são obrigatórios.");
    }

    //2. Verificar se o email já existe
    if(await context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
    {
        return Results.BadRequest("Email já cadastrado.");
    }

    //3. Criar o usuário
    var usuario = new Usuario
    {
        Email = usuarioDto.Email,
        Senha = usuarioDto.Senha,
        Perfil = "Editor"
    };

    //4. Salvar no banco e Retornar o resultado
    context.Usuarios.Add(usuario);
    await context.SaveChangesAsync();
    return Results.Ok(usuario);
});
#endregion


app.Run();