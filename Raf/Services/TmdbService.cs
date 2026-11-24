using System.Text.Json.Serialization;
using Raf.Models;

namespace Raf.Services;

public interface ITmdbService
{
    Task<List<HariciIcerik>> AraAsync(string sorgu);
    Task<HariciIcerik?> GetirAsync(string id);
}

public class TmdbService : ITmdbService
{
    private const string GorselAdresi = "https://image.tmdb.org/t/p/w500";

    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TmdbService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Tmdb:ApiKey"] ?? string.Empty;
    }

    public async Task<List<HariciIcerik>> AraAsync(string sorgu)
    {
        AnahtariKontrolEt();

        var url = $"search/movie?api_key={_apiKey}&language=tr-TR&include_adult=false&query={Uri.EscapeDataString(sorgu)}";
        var cevap = await _http.GetFromJsonAsync<TmdbAramaCevabi>(url);

        return cevap?.Results.Select(Donustur).ToList() ?? new List<HariciIcerik>();
    }

    public async Task<HariciIcerik?> GetirAsync(string id)
    {
        AnahtariKontrolEt();

        var url = $"movie/{Uri.EscapeDataString(id)}?api_key={_apiKey}&language=tr-TR";
        var film = await _http.GetFromJsonAsync<TmdbFilm>(url);

        return film == null ? null : Donustur(film);
    }

    private static HariciIcerik Donustur(TmdbFilm film) => new()
    {
        HariciId = film.Id.ToString(),
        Baslik = film.Title,
        CikisYili = HariciIcerik.YilAl(film.ReleaseDate),
        Aciklama = string.IsNullOrWhiteSpace(film.Overview) ? null : film.Overview,
        GorselUrl = film.PosterPath == null ? null : GorselAdresi + film.PosterPath,
        Turler = film.Genres?.Select(g => g.Name).ToList() ?? new List<string>()
    };

    private void AnahtariKontrolEt()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            throw new InvalidOperationException("TMDB API anahtarı tanımlı değil.");
        }
    }

    private record TmdbAramaCevabi(List<TmdbFilm> Results);

    private record TmdbFilm(
        int Id,
        string Title,
        [property: JsonPropertyName("release_date")] string? ReleaseDate,
        string? Overview,
        [property: JsonPropertyName("poster_path")] string? PosterPath,
        List<TmdbTur>? Genres);

    private record TmdbTur(string Name);
}
