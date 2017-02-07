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
        public async Task<IActionResult> Index(string FeedUrl,
                                               bool DisplayTitle,
                                               bool DisplayLink,
                                               bool DisplayContent)
        {
            RSSFeedItemList RSSFeedItems = new RSSFeedItemList();
            RSSFeedItems.ErrorMsg = "";
            RSSFeedItems.UrlIsValid = true;
            RSSFeedItems.DisplayTitle = DisplayTitle;
            RSSFeedItems.DisplayLink = DisplayLink;
            RSSFeedItems.DisplayContent = DisplayContent;
            var feedUrl = FeedUrl;//"https://blogs.msdn.microsoft.com/martinkearn/feed/";

            if(feedUrl != null)
            {
                try{
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(feedUrl);
                        var getResponse = await client.GetAsync(feedUrl);
                        string getResponseString = await getResponse.Content.ReadAsStringAsync();

                        // Extract infos from XML
                        XDocument doc = XDocument.Parse(getResponseString);
                        RSSFeedItems.ItemList = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                                        select new RSSFeedItem
                                        {
                                            TitleStr = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                            LinkStr = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                            ContentStr = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                            // PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                        };
                    }
                }
                catch (Exception e){
                    RSSFeedItems.ErrorMsg = e.Message;
                    RSSFeedItems.UrlIsValid = false;
                }
            }
            else
            {
                RSSFeedItems.ErrorMsg = "Invalid Url";
                RSSFeedItems.UrlIsValid = false;
            }

            return View(RSSFeedItems);
        } 

    }
}
