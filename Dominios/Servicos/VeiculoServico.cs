using Domain.DTO;
using Domain.Interface;
using Domain.Models;
using Dominios.Entidades;
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

    private async Task<Veiculo> BuscarEntidadePorIdAsync(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo == null)
        {
            throw new KeyNotFoundException("Veículo não encontrado.");
        }
        return veiculo;
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

//Atualizar Veiculo
    public async Task<VeiculoModel> AtualizarVeiculoAsync(int id, VeiculoModel veiculoModel)
    {
        var veiculo = await BuscarEntidadePorIdAsync(id);

        veiculo.Marca = veiculoModel.Marca;
        veiculo.Nome = veiculoModel.Nome;
        veiculo.Ano = veiculoModel.Ano;

        await _context.SaveChangesAsync();

        return new VeiculoModel
        {
            Marca = veiculo.Marca,
            Nome = veiculo.Nome,
            Ano = veiculo.Ano
        };
        
    }

    public async Task CriarVeiculoAsync (VeiculoModel veiculoModel)
    {

        if (string.IsNullOrEmpty(veiculoModel.Marca) || string.IsNullOrEmpty(veiculoModel.Nome) || veiculoModel.Ano <= 1885)
        {
            throw new ArgumentException("Dados do veículo inválidos. Verifique marca, nome e ano.");
        }

        var veiculo = new Veiculo
        {
            Marca = veiculoModel.Marca,
            Nome = veiculoModel.Nome,
            Ano = veiculoModel.Ano
        };

        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();

    }

    public async Task<bool> RemoverVeiculoAsync(int id)
    {
        var veiculo = await BuscarEntidadePorIdAsync(id);

        if (veiculo == null)
        {
            return false;
        }
        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();
        return true;
    }


}