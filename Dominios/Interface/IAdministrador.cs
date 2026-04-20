using Domain.DTO;

namespace Domain.Interface;

public interface IAdministrador
{
    Task GerenciarUsuariosAsync();

    Task <List<UsuarioDTO>> ListarUsuariosAsync();

    Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);

    Task RemoverUsuarioAsync(int id);
    
}