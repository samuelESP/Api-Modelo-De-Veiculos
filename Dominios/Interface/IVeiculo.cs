using Domain.DTO;
using Domain.Models;
using Dominios.Entidades;

namespace Domain.Interface;

public interface IVeiculo
{
    
    Task <List<VeiculoDTO>> ListarVeiculosAsync();
    Task <VeiculoDTO> ObterVeiculoPorIdAsync(int id);
    Task <VeiculoDTO> ObterVeiculoPorNomeAsync(string nome);

    Task<VeiculoModel> AtualizarVeiculoAsync(int id, VeiculoModel veiculoModel);

    Task CriarVeiculoAsync(VeiculoModel veiculoModel);

    Task<bool> RemoverVeiculoAsync(int id);
}