using System.ComponentModel.DataAnnotations;

namespace Raf.Models;

public class IncelemeDto
{
    public int Id { get; set; }
    public int IcerikId { get; set; }
    public string IcerikBaslik { get; set; } = string.Empty;
    public string KullaniciAdi { get; set; } = string.Empty;
    public int Puan { get; set; }
    public string? Metin { get; set; }
    public DateTime Tarih { get; set; }
}

public class IncelemeOlusturDto
{
    [Required(ErrorMessage = "İçerik numarası zorunludur.")]
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir içerik numarası girin.")]
    public int? IcerikId { get; set; }

    [Required(ErrorMessage = "Puan zorunludur.")]
    [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
    public int? Puan { get; set; }

    [StringLength(2000, ErrorMessage = "İnceleme en fazla 2000 karakter olabilir.")]
    public string? Metin { get; set; }
}

public class IncelemeGuncelleDto
{
    [Required(ErrorMessage = "Puan zorunludur.")]
    [Range(1, 10, ErrorMessage = "Puan 1 ile 10 arasında olmalıdır.")]
    public int? Puan { get; set; }

    [StringLength(2000, ErrorMessage = "İnceleme en fazla 2000 karakter olabilir.")]
    public string? Metin { get; set; }
}
