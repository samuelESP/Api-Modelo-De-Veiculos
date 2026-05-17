using Domain.DTO;
using Domain.Interface;
using Dominios.Entidades;
using Dominios.Enums;
using Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Dominios.Servicos;

public class AdministradorServico : IAdministrador
{
    private readonly DbContexto _context;

    public AdministradorServico(DbContexto context)
    {
        _context = context;
    }

    private async Task<Usuario> BuscarUsuarioPorIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }
        return usuario;
    }

//Gerenciar o perfil do usuario
    public async Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id)
    {
        var usuario = await BuscarUsuarioPorIdAsync(id);

        if(usuario == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado.");
        }
        return new UsuarioDTO
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Perfil = Enum.Parse <Perfil>(usuario.Perfil) 
        };
    }

//Gerenciar do usuario
    public async Task GerenciarUsuariosAsync(int id, AtualizarUsuarioAdministradorDTO atualizarUsuarioAdministradorDTO)
    {
        var usuario = await BuscarUsuarioPorIdAsync(id);

        usuario.Email = atualizarUsuarioAdministradorDTO.Email;
        var emailExistente = await _context.Usuarios.AnyAsync(u => u.Email == atualizarUsuarioAdministradorDTO.Email && u.Id != id);
        if (emailExistente)
        {
            throw new ArgumentException("O email já está em uso por outro usuário.");
        }

        if (!Enum.IsDefined(typeof(Perfil), atualizarUsuarioAdministradorDTO.Perfil))
        {
            throw new ArgumentException("Perfil inválido.");
        }
        usuario.Perfil = atualizarUsuarioAdministradorDTO.Perfil.ToString();

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();        
    }

//Remoção de usuario
    public async Task<bool> RemoverUsuarioAsync(int id)
    {
        var usuario = await BuscarUsuarioPorIdAsync(id);

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return true;
    }

}