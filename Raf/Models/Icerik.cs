using System.ComponentModel.DataAnnotations;

namespace Raf.Models;

public class Icerik
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; } = string.Empty;

    [Display(Name = "Medya türü")]
    public MedyaTuru MedyaTuru { get; set; }

    [Range(1870, 2100, ErrorMessage = "Çıkış yılı 1870 ile 2100 arasında olmalıdır.")]
    [Display(Name = "Çıkış yılı")]
    public int? CikisYili { get; set; }

    [StringLength(4000)]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Url(ErrorMessage = "Geçerli bir adres girin.")]
    [StringLength(500)]
    [Display(Name = "Görsel adresi")]
    public string? GorselUrl { get; set; }

    // Elle eklenen içeriklerde bu iki alan boş kalır.
    public HariciKaynak? HariciKaynak { get; set; }

    [StringLength(50)]
    public string? HariciId { get; set; }

    public ICollection<IcerikTuru> IcerikTurleri { get; set; } = new List<IcerikTuru>();
}
