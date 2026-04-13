using Domain.DTO;
using Domain.Interface;
using Dominios.Entidades;
using Dominios.Enums;
using Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Domain.Servicos;

public class UsuarioServico : IUsuario
{

// Conexão com o banco
    private readonly DbContexto _context;
    public UsuarioServico(DbContexto context)
    {
        _context = context;
    }


//Criar Usuário
    public async Task<IResult> CriarUsuarioAsync(LoginDto loginDto)
    {
        // 1. Validações de Senha e Email Null ou Whitespace
        if (string.IsNullOrWhiteSpace(loginDto.Email) ||
            string.IsNullOrWhiteSpace(loginDto.Senha))
        {
            throw new ArgumentException("Email e Senha são obrigatórios.");
        }

        //2. Verificar se o email já existe
        if (await _context.Usuarios.AnyAsync(u => u.Email == loginDto.Email))
        {
            throw new ArgumentException("Email já cadastrado.");
        }

        //3. Criar o usuário
        var usuario = new Usuario
        {
            Email = loginDto.Email,
            Senha = loginDto.Senha,
            Perfil = "Editor"
        };

        //4. Salvar no banco
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return Results.Ok(usuario);
    }

//Obter usuario pelo ID

    public async Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Perfil = Enum.Parse<Perfil>(usuario.Perfil) 
        };
    }

//Listar Usuarios
    public async Task<List<UsuarioDTO>> ListarUsuariosAsync()
    {
        var usuarios = await _context.Usuarios.ToListAsync();
        return usuarios.Select(u => new UsuarioDTO
        {
            Id = u.Id,
            Email = u.Email,
            Perfil = Enum.Parse<Perfil>(u.Perfil) 
        }).ToList();
    }
}