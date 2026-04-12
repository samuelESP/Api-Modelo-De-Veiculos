using Domain.DTOs;
using Dominios.Entidades;

namespace Domain.Interface;

public interface IEditor : IUsuario
{
    Task<VeiculoDTO> AtualizarVeiculoAsync(int id, VeiculoDTO veiculoDto);

    Task<VeiculoDTO> CriarVeiculoAsync(VeiculoDTO veiculoDto);

    Task<bool> RemoverVeiculoAsync(int id);
}