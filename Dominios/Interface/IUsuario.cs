using Domain.DTO;
using Dominios.Entidades;

namespace Domain.Interface;

public interface IUsuario
{

    Task<Usuario> Login(LoginDto loginDto);
    Task<IResult> CriarUsuarioAsync(LoginDto loginDto);
    Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);

    Task<List<UsuarioDTO>> ListarUsuariosAsync();
}