namespace Raf.Models;

public enum MedyaTuru
{
    Film,
    Oyun,
    Album
}

public static class MedyaTuruExtensions
{
    public static string Ad(this MedyaTuru tur) => tur switch
    {
        MedyaTuru.Film => "Film",
        MedyaTuru.Oyun => "Oyun",
        MedyaTuru.Album => "Albüm",
        _ => tur.ToString()
    };

    public static string CogulAd(this MedyaTuru tur) => tur switch
    {
        MedyaTuru.Film => "Filmler",
        MedyaTuru.Oyun => "Oyunlar",
        MedyaTuru.Album => "Albümler",
        _ => tur.ToString()
    };
}
