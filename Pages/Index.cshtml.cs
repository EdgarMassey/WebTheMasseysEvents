using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebTheMasseysEvents.Models;
using WebTheMasseysEvents.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebTheMasseysEvents.Pages;

public class IndexModel : PageModel
{
    private readonly EventStore _store;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(EventStore store, IWebHostEnvironment environment)
    {
        _store = store;
        _environment = environment;
    }

    public IReadOnlyList<EventItem> Events { get; private set; }
        = Array.Empty<EventItem>();

    public CurrentItem? LatestCurrent { get; private set; }

    public bool ShowEventsNewBadge { get; private set; }

    public string StartPictureUrl { get; private set; }
        = "/Photos/Us2026.jpg";

    public string StartPictureAlt { get; private set; }
        = "Family photo";

    public string StartPictureDescription { get; private set; }
        = "";

    public void OnGet()
    {
        var allEvents = _store.GetAll().ToList();

        Events = allEvents.Take(9).ToList();

        var newestEventDate = allEvents
            .OrderByDescending(e => e.Date)
            .Select(e => e.Date)
            .FirstOrDefault();

        ShowEventsNewBadge =
            newestEventDate != default &&
            DateTime.Today <= newestEventDate.AddDays(14);

        LatestCurrent = CurrentStore.LoadAll()
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();

        LoadStartPicture();
    }

    private void LoadStartPicture()
    {
        var path = Path.Combine(
            _environment.WebRootPath,
            "StartPicture.md");

        if (!System.IO.File.Exists(path))
            return;

        var markdown = System.IO.File.ReadAllText(path).Trim();

        // First line: ![Description](/Photos/picture.jpg)
        var match = Regex.Match(
            markdown,
            @"\A!\[(?<alt>[^\]\r\n]*)\]\((?<url>[^\r\n]+)\)[ \t]*(?:\r?\n|$)");

        if (!match.Success)
            return;

        var url = match.Groups["url"].Value.Trim();

        // Allow local site paths and full HTTP/HTTPS addresses.
        var isLocal = url.StartsWith("/") &&
                      !url.StartsWith("//") &&
                      !url.Contains('\\');

        var isWeb = Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp ||
                     uri.Scheme == Uri.UriSchemeHttps);

        if (!isLocal && !isWeb)
            return;

        StartPictureUrl = url;
        StartPictureAlt = match.Groups["alt"].Value;

        StartPictureDescription = markdown
            .Substring(match.Length)
            .Trim();
    }
}