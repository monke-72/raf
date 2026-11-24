using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Raf.Models;

namespace Raf.Services;

public interface IDiscogsService
{
    Task<List<HariciIcerik>> AraAsync(string sorgu);
    Task<HariciIcerik?> GetirAsync(string id);
}

public class DiscogsService : IDiscogsService
{
    private static readonly Dictionary<string, string> TurCevirileri = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Electronic"] = "Elektronik",
        ["Jazz"] = "Caz",
        ["Classical"] = "Klasik",
        ["Folk, World, & Country"] = "Folk ve Dünya Müziği",
        ["Stage & Screen"] = "Film ve Sahne Müziği",
        ["Children's"] = "Çocuk",
        ["Non-Music"] = "Müzik Dışı"
    };

    private readonly HttpClient _http;
    private readonly string _token;

    public DiscogsService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _token = config["Discogs:Token"] ?? string.Empty;
    }

    public async Task<List<HariciIcerik>> AraAsync(string sorgu)
    {
        var url = $"database/search?type=master&per_page=20&q={Uri.EscapeDataString(sorgu)}";
        var cevap = await GonderAsync<DiscogsAramaCevabi>(url);

        return cevap?.Results.Select(s => new HariciIcerik
        {
            HariciId = s.Id.ToString(),
            Baslik = s.Title,
            CikisYili = HariciIcerik.YilAl(s.Year),
            GorselUrl = GercekGorselMi(s.CoverImage) ? s.CoverImage : null
        }).ToList() ?? new List<HariciIcerik>();
    }

    public async Task<HariciIcerik?> GetirAsync(string id)
    {
        var album = await GonderAsync<DiscogsMaster>($"masters/{Uri.EscapeDataString(id)}");
        if (album == null)
        {
            return null;
        }

        var sanatcilar = string.Join(", ", album.Artists?.Select(a => SanatciAdiniTemizle(a.Name)) ?? []);
        var gorsel = album.Images?.FirstOrDefault(g => g.Type == "primary") ?? album.Images?.FirstOrDefault();

        return new HariciIcerik
        {
            HariciId = album.Id.ToString(),
            Baslik = string.IsNullOrEmpty(sanatcilar) ? album.Title : $"{sanatcilar} – {album.Title}",
            CikisYili = album.Year > 0 ? album.Year : null,
            Aciklama = SarkiListesi(album.Tracklist),
            GorselUrl = gorsel?.Uri,
            Turler = album.Genres?
                .Select(g => TurCevirileri.GetValueOrDefault(g, g))
                .ToList() ?? new List<string>()
        };
    }

    private async Task<T?> GonderAsync<T>(string url)
    {
        if (string.IsNullOrWhiteSpace(_token))
        {
            throw new InvalidOperationException("Discogs token'ı tanımlı değil.");
        }

        using var istek = new HttpRequestMessage(HttpMethod.Get, url);
        istek.Headers.Authorization = new AuthenticationHeaderValue("Discogs", $"token={_token}");

        using var cevap = await _http.SendAsync(istek);
        cevap.EnsureSuccessStatusCode();

        return await cevap.Content.ReadFromJsonAsync<T>();
    }

    private static string? SarkiListesi(List<DiscogsSarki>? sarkilar)
    {
        var liste = sarkilar?
            .Where(s => !string.IsNullOrWhiteSpace(s.Title))
            .Select((s, sira) => $"{sira + 1}. {s.Title}")
            .ToList();

        return liste == null || liste.Count == 0 ? null : "Şarkılar:\n" + string.Join("\n", liste);
    }

    private static string SanatciAdiniTemizle(string ad) => Regex.Replace(ad, @"\s\(\d+\)$", "");

    private static bool GercekGorselMi(string? url) =>
        !string.IsNullOrEmpty(url) && !url.Contains("spacer.gif");

    private record DiscogsAramaCevabi(List<DiscogsSonuc> Results);

    private record DiscogsSonuc(
        int Id,
        string Title,
        string? Year,
        [property: JsonPropertyName("cover_image")] string? CoverImage);

    private record DiscogsMaster(
        int Id,
        string Title,
        int Year,
        List<DiscogsSanatci>? Artists,
        List<string>? Genres,
        List<DiscogsGorsel>? Images,
        List<DiscogsSarki>? Tracklist);

    private record DiscogsSanatci(string Name);

    private record DiscogsGorsel(string Type, string Uri);

    private record DiscogsSarki(string Title);
}
