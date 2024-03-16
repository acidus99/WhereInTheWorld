using Gemini.Cgi;

namespace WhereInTheWorld.Cgi;

class Program
{
    static void Main(string[] args)
    {
        var router = new CgiRouter();
        router.SetStaticRoot("static/");
        router.OnRequest("/play", RouteHandler.PlayToday);
        router.OnRequest("/game/", RouteHandler.PlayGame);
        router.OnRequest("/past", RouteHandler.ShowArchive);
        router.OnRequest("/png", RouteHandler.ShowPng);
        router.ProcessRequest();
    }
}
