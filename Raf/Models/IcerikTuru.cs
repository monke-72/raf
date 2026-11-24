namespace Raf.Models;

// Icerik <-> Tur çoka-çok ilişkisinin ara tablosu.
public class IcerikTuru
{
    public int IcerikId { get; set; }
    public Icerik Icerik { get; set; } = null!;

    public int TurId { get; set; }
    public Tur Tur { get; set; } = null!;
}
