namespace WhereInTheWorld.Cgi;

public static class RouteOptions
{
    private const string BaseCgiPath = "/cgi-bin/witw.cgi";
    
    public const string HelpUrl = BaseCgiPath + "/help.gmi";
    public const string FaqUrl = BaseCgiPath + "/faq.gmi";

    public static string PngUrl(int puzzleNumber)
        => BaseCgiPath + $"/png/game/{puzzleNumber}/country.png";

    public static string PlayUrl(int puzzleNumber)
        => BaseCgiPath + $"/game/{puzzleNumber}/";
}