using Domain.DTO;
using Domain.DTOs;
using Dominios.Entidades;

namespace Domain.Interface;

public interface IUsuario
{

    Task CriarUsuarioAsync(UsuarioDTO usuarioDto);
    Task<UsuarioDTO> ObterUsuarioPorIdAsync(int id);
    Task<UsuarioDTO> AlterarSenhaAsync(int id,string senhaAtual, string novaSenha);

    Task <List<VeiculoDTO>> ListarVeiculosAsync();
    Task <VeiculoDTO> ObterVeiculoPorIdAsync(int id);
    Task <VeiculoDTO> ObterVeiculoPorNomeAsync(string nome);
}