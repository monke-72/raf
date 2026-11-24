using Microsoft.AspNetCore.Identity;

namespace Raf.Models;

public class Kullanici : IdentityUser
{
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

    public ICollection<Inceleme> Incelemeler { get; set; } = new List<Inceleme>();
}
