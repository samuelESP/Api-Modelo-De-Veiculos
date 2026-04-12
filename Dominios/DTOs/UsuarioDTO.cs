using Dominios.Enums;

namespace Domain.DTO;

public class UsuarioDTO
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil Perfil { get; set; }
}