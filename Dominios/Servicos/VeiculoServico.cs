using Domain.DTO;
using Domain.Interface;
using Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Domain.Servicos;

public class VeiculoServico : IVeiculo
{

// Injeção de dependência do DbContexto
    private readonly DbContexto _context;
    public VeiculoServico(DbContexto context)
    {
        _context = context;
    }

// Implementação do método para obter um veículo por ID
    public async Task<List<VeiculoDTO>> ListarVeiculosAsync()
    {
        var veiculos = await _context.Veiculos.ToListAsync();
        return veiculos.Select(v => new VeiculoDTO{
            Id = v.Id,
            Marca = v.Marca,
            Nome = v.Nome,
            Ano = v.Ano
        }).ToList();
    }
// Implementação do método para obter um veículo por ID
    public async Task<VeiculoDTO> ObterVeiculoPorIdAsync(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado.");
        }
        return new VeiculoDTO
        {
            Id = veiculo.Id,
            Marca = veiculo.Marca,
            Nome = veiculo.Nome,
            Ano = veiculo.Ano
        };
    }

//Obter veículo por nome
    public async Task<VeiculoDTO> ObterVeiculoPorNomeAsync(string nome)
    {
        var veiculo = await _context.Veiculos.FirstOrDefaultAsync(v => EF.Functions.Like(v.Nome, $"%{nome}%"));
        if (veiculo == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado.");
        }
        return new VeiculoDTO
        {
            Id = veiculo.Id,
            Marca = veiculo.Marca,
            Nome = veiculo.Nome,
            Ano = veiculo.Ano
        };
    }
}