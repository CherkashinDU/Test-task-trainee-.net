﻿using System;
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

        public List<string> GetLinksWebsite(List<string> links, string baseUrl)
        {
            var result = new List<string>();
            result.AddRange(links);
            foreach (var link in links)
            {
                var document = new HtmlWeb().Load(link);
                foreach (var item in GetLinksHtml(document, baseUrl))
                {
                    if (!result.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }
            result.Sort();

            return result;
        }

        public List<string> GetLinksHtml(HtmlDocument document, string baseUrl)
        {
            var links = new List<string>();
            document.OptionEmptyCollection = true;
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
            var sitemapString = GetSitemapContent(url);
            if (string.IsNullOrEmpty(sitemapString))
            {
                return links;
            }
            var document = new XmlDocument();
            document.LoadXml(sitemapString);

            foreach (XmlNode node in document.GetElementsByTagName("url"))
            {
                if (node["loc"] != null)
                {
                    links.Add(node["loc"].InnerText.Replace("//www.", "//"));
                }
            }
            links.Sort();

            return links;
        }

        public string GetSitemapContent(string url)
        {
            const string sitemapString = null;
            var sitemapUrl = url + "/sitemap.xml";
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers["User-Agent"] = "Mozilla/5.0";

            try
            {
                return wc.DownloadString(sitemapUrl);
            }
            catch
            {
                return sitemapString;
            }
        }

        public string GetAbsoluteUrlString(string baseUrl, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(baseUrl), uri);
            return uri.GetLeftPart(UriPartial.Query);
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
            var linksWithTime = new List<Page>();
            foreach (var link in links)
            {
                var page = new Page
                {
                    Link = link,
                    ResponseTime = GetResponseTime(link).Milliseconds
                };

                linksWithTime.Add(page);
            }

            Console.WriteLine("Timing");
            foreach (var item in linksWithTime.OrderBy(r => r.ResponseTime))
            {
                Console.WriteLine($"{item.Link} {item.ResponseTime}ms");
            }
            Console.WriteLine(Environment.NewLine);
        }

        static TimeSpan GetResponseTime(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.Headers.Add("User-Agent", "Mozilla / 5.0");
            var timer = new Stopwatch();
            timer.Start();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
            timer.Stop();

            return timer.Elapsed;
        }
    }
}
