using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;




namespace NightOwlImageService.Classes
{
    public class ReadWeatherFeed
    {
        public string WeatherValue { get; set; }

        public string GetTheWeather()
        {
            string WeatherValue = "";
            string url = "http://fooblog.com/feed";
            XmlReader reader = XmlReader.Create(url);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();
            foreach (SyndicationItem item in feed.Items)
            {
                String subject = item.Title.Text;
                String summary = item.Summary.Text;
                WeatherValue += "{subject}{summary}";
            }
            return WeatherValue;
        }

    }
}

//public void ReadRSSFeed()
    //    {

    //        string url = "http://fooblog.com/feed";
    //        XmlReader reader = XmlReader.Create(url);
    //        SyndicationFeed feed = SyndicationFeed.Load(reader);
    //        reader.Close();
    //        foreach (SyndicationItem item in feed.Items)
    //        {
    //            String subject = item.Title.Text;
    //            String summary = item.Summary.Text;
    //            WeatherValue += "{subject}{summary}";
    //        }
    //    }
    

    //public class RssRead
    //{
    //    public string Title;
    //    public string PublicationDate;
    //    public string Description;
    //}

    //public class RssReader
    //{
    //    public List<RssRead> Read(string url)
    //    {
    //        var webResponse = WebRequest.Create(url).GetResponse();
    //        if (webResponse == null)
    //            return null;
    //        var ds = new DataSet();
    //        ds.ReadXml(webResponse.GetResponseStream());

    //        var news = (from row in ds.Tables["item"].AsEnumerable()
    //                    select new RssRead()
    //                    {
    //                        Title = row.Field<string>("title"),
    //                        PublicationDate = row.Field<string>("pubDate"),
    //                        Description = row.Field<string>("description")
    //                    }).ToList();
    //        return news;
    //    }
    //}

    //public static class ReadTemp
    //{
    //    public static string CovertRss(string url)
    //    {
    //        var s = new RssReader();

    //        StringBuilder sb = new StringBuilder();
    //        foreach (RssRead rs in s.Read(url))
    //        {
    //            sb.AppendLine(rs.Title);
    //            sb.AppendLine(rs.PublicationDate);
    //            sb.AppendLine(rs.Description);
    //        }
    //        return sb.ToString();
    //    }
    //}
