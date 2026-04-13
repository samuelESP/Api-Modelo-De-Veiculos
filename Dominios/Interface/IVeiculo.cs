using Domain.DTO;

namespace Domain.Interface;

public interface IVeiculo
{
    
    Task <List<VeiculoDTO>> ListarVeiculosAsync();
    //Task <VeiculoDTO> ObterVeiculoPorIdAsync(int id);
    //Task <VeiculoDTO> ObterVeiculoPorNomeAsync(string nome);
}