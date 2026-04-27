using Domain.DTO;

namespace Domain.Interface;

public interface IAdministrador
{
   Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);
   
    Task GerenciarUsuariosAsync(int id, AtualizarUsuarioAdministradorDTO atualizarUsuarioAdministradorDTO);

    Task<bool> RemoverUsuarioAsync(int id);
    
}