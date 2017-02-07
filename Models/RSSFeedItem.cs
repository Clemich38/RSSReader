using System.Collections.Generic;
using System;

namespace RSSReader.ViewModels
{
  public class RSSFeedItemList
  {
    public IEnumerable<RSSFeedItem> ItemList { get; set; }
    public bool DisplayTitle { get; set; }
    public bool DisplayLink { get; set; }
    public bool DisplayContent { get; set; }
    public bool DisplayDate { get; set; }
    public string ErrorMsg { get; set; }
    public bool UrlIsValid { get; set; }
    public string LastFeedUrl { get; set; }
    public IList<string> FeedUrlList { get; set; }
  }

  public class RSSFeedItem
  {
    public string LinkStr { get; set; }
    public string TitleStr { get; set; }
    public string ContentStr { get; set; }
    public DateTime DateStr { get; set; }
  }
}