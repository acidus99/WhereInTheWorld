using System;

using Gemini.Cgi;

namespace WhereInTheWorld.Cgi
{
    class Program
    {

        static void Main(string[] args)
        {
            var router = new CgiRouter();
            router.SetStaticRoot("static/");
            router.OnRequest("/play", RouteHandler.PlayGame);
            router.OnRequest("/png", RouteHandler.ShowPng);
            router.ProcessRequest();
        }

    }
}
