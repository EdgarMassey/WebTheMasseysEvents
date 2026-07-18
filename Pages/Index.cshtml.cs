using Microsoft.AspNetCore.Mvc.RazorPages;
using WebTheMasseysEvents.Models;
using WebTheMasseysEvents.Services;
using System.Linq;

namespace WebTheMasseysEvents.Pages;

public class IndexModel : PageModel
{
    private readonly EventStore _store;

    public IndexModel(EventStore store)
    {
        _store = store;
    }

    public IReadOnlyList<EventItem> Events { get; private set; } = Array.Empty<EventItem>();

    public CurrentItem? LatestCurrent { get; private set; }

    public bool ShowEventsNewBadge { get; private set; }

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
    }
}