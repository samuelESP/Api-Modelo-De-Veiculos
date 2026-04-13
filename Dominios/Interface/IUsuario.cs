using Domain.DTO;

namespace Domain.Interface;

public interface IUsuario
{

    Task<IResult> CriarUsuarioAsync(LoginDto loginDto);
    Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);

    Task<List<UsuarioDTO>> ListarUsuariosAsync();
}