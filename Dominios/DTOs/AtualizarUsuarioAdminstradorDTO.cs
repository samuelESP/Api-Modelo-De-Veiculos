using Dominios.Enums;

namespace Domain.DTO;

public class AtualizarUsuarioAdministradorDTO
{
    public string Email { get; set; } = default!;
    public Perfil Perfil { get; set; } = default!;
}
