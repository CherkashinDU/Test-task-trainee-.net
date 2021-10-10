using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;

namespace Crawler
{
    public class Links
    {
        public List<string> GetLinksHtml(HtmlDocument document, string baseUrl)
        {
            var links = new List<string>();

            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                var link = node.Attributes["href"].Value;
                if (!link.Contains("mailto") && !link.Contains("skype"))
                {
                    var absoluteUrl = GetAbsoluteUrlString(baseUrl, link);
                    if (absoluteUrl.StartsWith(baseUrl))
                        links.Add(absoluteUrl);
                }
            }
            return links.Distinct().ToList();
        }

        public List<string> GetLinksSitemap(string url)
        {
            var links = new List<string>();
            var sitemapUrl = url + "/sitemap.xml";
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            var sitemapString = wc.DownloadString(sitemapUrl);
            var document = new XmlDocument();
            document.LoadXml(sitemapString);

            foreach (XmlNode node in document.GetElementsByTagName("url"))
            {
                if (node["loc"] != null)
                {
                    links.Add(node["loc"].InnerText);
                }
            }
            return links;
        }

        public string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.ToString();
        }

        public void ShowLinks(List<string> links)
        {
            foreach (var link in links)
            {
                Console.WriteLine(link);
            }
            Console.WriteLine(Environment.NewLine);
        }

        public void ShowLinksWithTime(List<string> links)
        {
            foreach (var link in links)
            {
                Console.WriteLine(link);
                Console.WriteLine(GetResponseTime(link).Milliseconds + "ms");

            }
            Console.WriteLine(Environment.NewLine);
        }

        static TimeSpan GetResponseTime(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var timer = new Stopwatch();
            timer.Start();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            timer.Stop();

            return timer.Elapsed;
        }
    }
}
