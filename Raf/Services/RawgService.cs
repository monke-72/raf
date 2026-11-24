using System.Text.Json.Serialization;
using Raf.Models;

namespace Raf.Services;

public interface IRawgService
{
    Task<List<HariciIcerik>> AraAsync(string sorgu);
    Task<HariciIcerik?> GetirAsync(string id);
}

public class RawgService : IRawgService
{
    private static readonly Dictionary<string, string> TurCevirileri = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Action"] = "Aksiyon",
        ["Adventure"] = "Macera",
        ["RPG"] = "Rol Yapma (RPG)",
        ["Platformer"] = "Platform",
        ["Simulation"] = "Simülasyon",
        ["Strategy"] = "Strateji",
        ["Shooter"] = "Nişancı",
        ["Puzzle"] = "Bulmaca",
        ["Racing"] = "Yarış",
        ["Sports"] = "Spor",
        ["Fighting"] = "Dövüş",
        ["Indie"] = "Bağımsız",
        ["Casual"] = "Gündelik",
        ["Family"] = "Aile",
        ["Educational"] = "Eğitici",
        ["Card"] = "Kart",
        ["Board Games"] = "Masa Oyunu",
        ["Massively Multiplayer"] = "Çok Oyunculu (MMO)"
    };

    private readonly HttpClient _http;
    private readonly string _apiKey;

    public RawgService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Rawg:ApiKey"] ?? string.Empty;
    }

    public async Task<List<HariciIcerik>> AraAsync(string sorgu)
    {
        AnahtariKontrolEt();

        var url = $"games?key={_apiKey}&page_size=20&search_precise=true&search={Uri.EscapeDataString(sorgu)}";
        var cevap = await _http.GetFromJsonAsync<RawgAramaCevabi>(url);

        return cevap?.Results
            .Where(o => !YetiskinIcerikMi(o))
            .Select(Donustur)
            .ToList() ?? new List<HariciIcerik>();
    }

    private static readonly string[] YetiskinEtiketleri = { "nsfw", "hentai", "sexual-content", "nudity", "erotic" };

    private static bool YetiskinIcerikMi(RawgOyun oyun) =>
        oyun.EsrbRating?.Slug == "adults-only"
        || (oyun.Tags?.Any(e => YetiskinEtiketleri.Contains(e.Slug)) ?? false);

    public async Task<HariciIcerik?> GetirAsync(string id)
    {
        AnahtariKontrolEt();

        var url = $"games/{Uri.EscapeDataString(id)}?key={_apiKey}";
        var oyun = await _http.GetFromJsonAsync<RawgOyun>(url);

        return oyun == null ? null : Donustur(oyun);
    }

    private static HariciIcerik Donustur(RawgOyun oyun) => new()
    {
        HariciId = oyun.Id.ToString(),
        Baslik = oyun.Name,
        CikisYili = HariciIcerik.YilAl(oyun.Released),
        Aciklama = string.IsNullOrWhiteSpace(oyun.DescriptionRaw) ? null : oyun.DescriptionRaw,
        GorselUrl = oyun.BackgroundImage,
        Turler = oyun.Genres?
            .Select(g => TurCevirileri.GetValueOrDefault(g.Name, g.Name))
            .ToList() ?? new List<string>()
    };

    private void AnahtariKontrolEt()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("RAWG API anahtarı tanımlı değil.");
        }
    }

    private record RawgAramaCevabi(List<RawgOyun> Results);

    private record RawgOyun(
        int Id,
        string Name,
        string? Released,
        [property: JsonPropertyName("background_image")] string? BackgroundImage,
        [property: JsonPropertyName("description_raw")] string? DescriptionRaw,
        List<RawgTur>? Genres,
        List<RawgEtiket>? Tags,
        [property: JsonPropertyName("esrb_rating")] RawgEtiket? EsrbRating);

    private record RawgTur(string Name);

    private record RawgEtiket(string Slug);
}
