namespace Raf.Models;

public class IcerikTuru
{
    public int IcerikId { get; set; }
    public Icerik Icerik { get; set; } = null!;

    public int TurId { get; set; }
    public Tur Tur { get; set; } = null!;
}
