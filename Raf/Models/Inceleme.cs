using System.ComponentModel.DataAnnotations;

namespace Raf.Models;

public class Inceleme
{
    public int Id { get; set; }

    public string KullaniciId { get; set; } = string.Empty;
    public Kullanici Kullanici { get; set; } = null!;

    public int IcerikId { get; set; }
    public Icerik Icerik { get; set; } = null!;

    [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
    public int Puan { get; set; }

    [StringLength(2000, ErrorMessage = "İnceleme en fazla 2000 karakter olabilir.")]
    public string? Metin { get; set; }

    public DateTime Tarih { get; set; } = DateTime.UtcNow;
}
