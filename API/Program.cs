using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.DTO;
using Domain.Interface;
using Domain.Models;
using Domain.Servicos;
using Dominios.Entidades;
using Dominios.Servicos;
using Infra.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;


#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt:Key").Value;

if(string.IsNullOrEmpty(key)){
    throw new InvalidOperationException("A chave JWT não pode ser nula ou vazia. Verifique a configuração.");
}

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer (option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "veiculos_modelos",
        ValidAudience = "veiculos_modelos",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
    };
});
builder.Services.AddAuthorization();

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});



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

//Login
app.MapPost("/Usuario/Login", async ([FromBody] LoginDto loginDto, IUsuario usuarioService) =>
{
    if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Senha))
    {
        return Results.BadRequest("Email e senha são obrigatórios.");
    }

    var usuario = await usuarioService.Login(loginDto);

    if (usuario == null)
    {
        return Results.BadRequest("Email ou senha inválidos.");
    }

    // Gerar o Token JWT
    var token = GerarTokenJWT(usuario);

    return Results.Ok(new UsuarioLogado
    {
        Email = usuario.Email,
        Perfil = usuario.Perfil,
        Token = token
    });
})
.WithTags("Usuário");

//obeter por ID
app.MapGet("/Usuario/ObterPorId/{id}", async ([FromRoute]int id, IUsuario usuarioService) =>
{
    var usuario = await usuarioService.ObterUsuarioPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuario);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).RequireAuthorization().WithTags("Usuário");

//Listar todos
app.MapGet("/Usuario/Listar", async (IUsuario usuarioService) =>
{
    var usuarios = await usuarioService.ListarUsuariosAsync();
    return Results.Ok(usuarios);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).RequireAuthorization().WithTags("Usuário");

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
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).RequireAuthorization().WithTags("Veículo");

//Criar Veiculo
app.MapPost("/Veiculo/Criar", async ([FromBody] VeiculoModel veiculoModel, IVeiculo veiculoService) =>
{
    await veiculoService.CriarVeiculoAsync(veiculoModel);

    return Results.Ok("Veículo criado com sucesso.");
    
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).RequireAuthorization().WithTags("Veículo");

//Remover Veiculo
app.MapDelete("/Veiculo/Remover/{id}", async ([FromRoute] int id, IVeiculo veiculoService) =>
{
    var resultado = await veiculoService.RemoverVeiculoAsync(id);
    if (!resultado)
    {
        return Results.NotFound("Veículo não encontrado para remoção.");
    }
    return Results.Ok("Veículo removido com sucesso.");
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"}).RequireAuthorization().WithTags("Veículo");
#endregion


// ====================== Administrador ======================
#region Administrador
string GerarTokenJWT(Usuario usuario)
{
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>()
    {
        new Claim("Email", usuario.Email),
        new Claim("Perfil", usuario.Perfil),
        new Claim(ClaimTypes.Role, usuario.Perfil),
    };
    var token = new JwtSecurityToken(
        issuer: "veiculos_modelos",
        audience: "veiculos_modelos",
        claims : claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}


//Obter usuario por ID
app.MapGet("/Administrador/ObterUsuarioPorId/{id}", async ([FromRoute] int id, IAdministrador administradorService) =>
{
    var usuario = await administradorService.ObterUsuarioPorIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }
    return Results.Ok(usuario);
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).RequireAuthorization().WithTags("Administrador");

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
}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).RequireAuthorization().WithTags("Administrador");

//Remover usuario
app.MapDelete("/Administrador/RemoverUsuario/{id}", async ([FromRoute] int id, IAdministrador administradorService) =>
{
    await administradorService.RemoverUsuarioAsync(id);
    
    return Results.Ok("Usuário removido com sucesso.");

}).RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).RequireAuthorization().WithTags("Administrador");

#endregion


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();