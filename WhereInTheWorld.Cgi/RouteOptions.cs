﻿namespace WhereInTheWorld.Cgi;

public static class RouteOptions
{
    private const string BaseCgiPath = "/cgi-bin/witw.cgi";
    
    public const string PngUrl = BaseCgiPath + "/png";
    public const string PlayUrl = BaseCgiPath + "/play";
    public const string HelpUrl = BaseCgiPath + "/help.gmi";
    public const string FaqUrl = BaseCgiPath + "/faq.gmi";
}