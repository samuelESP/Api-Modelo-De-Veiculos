using Domain.DTO;
using Domain.Interface;
using Infra.Db;
using Microsoft.EntityFrameworkCore;

namespace Domain.Servicos;

public class VeiculoServico : IVeiculo
{
    private readonly DbContexto _context;
    public VeiculoServico(DbContexto context)
    {
        _context = context;
    }


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
}