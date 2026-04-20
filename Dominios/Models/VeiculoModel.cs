namespace Domain.Models;

public record VeiculoModel
{
    public string Marca { get; set; } = default!;
    public string Nome { get; set; } = default!;
    public int Ano { get; set; }
}