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
            var articles = new List<RSSFeedItem>();
            var feedUrl = "https://blogs.msdn.microsoft.com/martinkearn/feed/";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(feedUrl);
                var responseMessage = await client.GetAsync(feedUrl);
                var responseString = await responseMessage.Content.ReadAsStringAsync();

                //extract feed items
                XDocument doc = XDocument.Parse(responseString);
                var feedItems = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                                select new RSSFeedItem
                                {
                                    TitleStr = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                    LinkStr = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                    ContentStr = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                    // PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                };
                articles = feedItems.ToList();
            }

            return View(articles);
        } 

    }
}
