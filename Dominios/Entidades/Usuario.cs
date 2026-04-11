using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set;} = default!;
    
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(30)]
    public string Senha { get; set; } = default!;

    [Required]
    [StringLength(10)]
    public string Perfil { get; set; } = default!;
}