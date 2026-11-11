using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebTheMasseysEvents.Pages.Quotes
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public List<string> ImageUrls { get; private set; } = new();

        public void OnGet()
        {
            var dir = Path.Combine(_env.WebRootPath, "Photos", "Quotes");
            if (!Directory.Exists(dir))
                return;

            var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            ImageUrls = Directory.GetFiles(dir)
                .Where(file => extensions.Contains(Path.GetExtension(file)))
                .OrderByDescending(file => Path.GetFileName(file))
                .Select(file => "/Photos/Quotes/" + Uri.EscapeDataString(Path.GetFileName(file)))
                .ToList();
        }
    }
}