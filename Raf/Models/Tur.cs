using System.ComponentModel.DataAnnotations;

namespace Raf.Models;

public class Tur
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tür adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Tür adı en fazla 50 karakter olabilir.")]
    [Display(Name = "Tür adı")]
    public string Ad { get; set; } = string.Empty;

    public ICollection<IcerikTuru> IcerikTurleri { get; set; } = new List<IcerikTuru>();
}
