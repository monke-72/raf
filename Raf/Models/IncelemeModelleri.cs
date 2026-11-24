using System.ComponentModel.DataAnnotations;

namespace Raf.Models;

public class IncelemeFormModel
{
    public int IcerikId { get; set; }

    [Required(ErrorMessage = "Puan seçmelisin.")]
    [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
    [Display(Name = "Puan")]
    public int? Puan { get; set; }

    [StringLength(2000, ErrorMessage = "İnceleme en fazla 2000 karakter olabilir.")]
    [Display(Name = "İnceleme")]
    public string? Metin { get; set; }
}

public class IcerikDetayModel
{
    public Icerik Icerik { get; set; } = null!;
    public List<Inceleme> Incelemeler { get; set; } = new();
    public double? OrtalamaPuan { get; set; }
    public IncelemeFormModel Form { get; set; } = new();
    public bool KendiIncelemesiVar { get; set; }
}
