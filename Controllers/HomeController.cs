using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

using RSSReader.ViewModels;

namespace RSSReader.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            IEnumerable<RSSReader.ViewModels.RSSFeedItem> RSSFeedItems = new List<RSSFeedItem>();
            var feedUrl = "https://blogs.msdn.microsoft.com/martinkearn/feed/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(feedUrl);
                var getResponse = await client.GetAsync(feedUrl);
                string getResponseString = await getResponse.Content.ReadAsStringAsync();

                // Extract infos from XML
                XDocument doc = XDocument.Parse(getResponseString);
                RSSFeedItems = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                                select new RSSFeedItem
                                {
                                    TitleStr = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                    LinkStr = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                    ContentStr = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                    // PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                };
            }

            return View(RSSFeedItems);
        } 

    }
}
