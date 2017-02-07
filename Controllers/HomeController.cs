using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using RSSReader.ViewModels;

namespace RSSReader.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string FeedUrl,
                                               bool DisplayTitle,
                                               bool DisplayLink,
                                               bool DisplayContent,
                                               bool DisplayDate)
        {
            RSSFeedItemList RSSFeedItems = null;
            var value = HttpContext.Session.GetString("RSSFeedItems");
            if (string.IsNullOrEmpty(value))
            {
                RSSFeedItems = new RSSFeedItemList
                {
                    FeedUrlList = new List<string>(),
                    ErrorMsg = "",
                    UrlIsValid = false,
                    DisplayTitle = true,
                    DisplayLink = false,
                    DisplayContent = true,
                    DisplayDate = false,
                    LastFeedUrl = "",
                };

                HttpContext.Session.SetObjectAsJson("RSSFeedItems", RSSFeedItems);
            }
            else
            {
                RSSFeedItems = HttpContext.Session.GetObjectFromJson<RSSFeedItemList>("RSSFeedItems");
            }

            RSSFeedItems.UrlIsValid = true;
            RSSFeedItems.DisplayTitle = DisplayTitle;
            RSSFeedItems.DisplayLink = DisplayLink;
            RSSFeedItems.DisplayContent = DisplayContent;
            RSSFeedItems.DisplayDate = DisplayDate;
            RSSFeedItems.LastFeedUrl = FeedUrl;

            if(RSSFeedItems.LastFeedUrl != null)
            {
                try{
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(RSSFeedItems.LastFeedUrl);
                        var getResponse = await client.GetAsync(RSSFeedItems.LastFeedUrl);
                        string getResponseString = await getResponse.Content.ReadAsStringAsync();

                        // Extract infos from XML
                        XDocument doc = XDocument.Parse(getResponseString);
                        RSSFeedItems.ItemList = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                            select new RSSFeedItem
                            {
                                TitleStr = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                LinkStr = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                ContentStr = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                DateStr = ExtractDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value)
                            };

                        //Add the Url to the list
                        RSSFeedItems.FeedUrlList.Add(RSSFeedItems.LastFeedUrl);
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

            // Save model
            HttpContext.Session.SetObjectAsJson("RSSFeedItems", RSSFeedItems);

            return View(RSSFeedItems);
        } 

        private DateTime ExtractDate(string date)
        {
            DateTime retVal;
            if (DateTime.TryParse(date, out retVal))
                return retVal;
            else
                return DateTime.MinValue;
        }

    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
