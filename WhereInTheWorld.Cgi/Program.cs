using System;

using Gemini.Cgi;

namespace WhereInTheWorld.Cgi
{
    class Program
    {

        static void Main(string[] args)
        {
            var cgiRouter = new CgiRouter();
            cgiRouter.OnRequest("", RouteHandler.PlayGame);
            cgiRouter.OnRequest("/png", RouteHandler.ShowPng);
            cgiRouter.ProcessRequest();
        }

    }
}
