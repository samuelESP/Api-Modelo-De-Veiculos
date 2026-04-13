using Domain.DTO;
using Domain.DTOs;
using Dominios.Entidades;

namespace Domain.Interface;

public interface IUsuario
{

    Task<IResult> CriarUsuarioAsync(LoginDto loginDto);
    Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);

    Task<List<UsuarioDTO>> ListarUsuariosAsync();

   // Task <List<VeiculoDTO>> ListarVeiculosAsync();
    //Task <VeiculoDTO> ObterVeiculoPorIdAsync(int id);
    //Task <VeiculoDTO> ObterVeiculoPorNomeAsync(string nome);
}