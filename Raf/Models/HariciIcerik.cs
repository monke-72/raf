namespace Raf.Models;

public class HariciIcerik
{
    public string HariciId { get; set; } = string.Empty;
    public string Baslik { get; set; } = string.Empty;
    public int? CikisYili { get; set; }
    public string? Aciklama { get; set; }
    public string? GorselUrl { get; set; }
    public List<string> Turler { get; set; } = new();

    public static int? YilAl(string? tarih)
    {
        if (tarih != null && tarih.Length >= 4 && int.TryParse(tarih[..4], out var yil))
        {
            return yil;
        }

        return null;
    }
}
