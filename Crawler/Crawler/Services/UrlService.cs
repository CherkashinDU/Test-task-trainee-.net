using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace Crawler
{
    public class UrlService
    {
        public Task<List<string>> GetUrlsWebsite(Uri domain)
        {
            var links = GetUrlsHtml(domain.ToString(), domain, new List<string>());
            links.Sort();
            return Task.FromResult(links);
        }

        public async Task<List<string>> GetUrlsSitemap(Uri url)
        {
            var links = new List<string>();
            var sitemapString = await GetSitemapContent(url);
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
                    var link = node["loc"].InnerText;
                    links.Add(link);
                }
            }

            return links.Distinct(new UrlComparer()).OrderBy(i => i).ToList();
        }

        public async Task<Page> GetPageInfo(string link)
        {
            var page = new Page
            {
                Link = link,
            };

            try
            {
                page.ResponseTime = await GetResponseTime(link);
            }
            catch
            {
                page.ResponseTime = 0;
            }

            return page;
        }

        private List<string> GetUrlsHtml(string url, Uri domain, List<string> links)
        {
            var document = new HtmlWeb().Load(url);
            document.OptionEmptyCollection = true;
            var nodes = document.DocumentNode.SelectNodes("//a[@href]");
            if (nodes.Count == 0)
            {
                return new List<string>();
            }
            else
            {
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a[@href]"))
                {
                    var link = node.Attributes["href"].Value;
                    if (!link.Contains("mailto") && !link.Contains("skype") && !link.Contains(".pdf") && link != "/" && !link.Contains("ssh:"))
                    {
                        var absoluteUrl = GetAbsoluteUrlString(domain, link);
                        if (absoluteUrl.StartsWith(domain.OriginalString) && !links.Contains(absoluteUrl, new UrlComparer()))
                        {

                            links.Add(absoluteUrl);
                            var childrenLinks = GetUrlsHtml(absoluteUrl, domain, links)
                                .Where(i => !links.Contains(i, new UrlComparer()))
                                .ToList();
                            links.AddRange(childrenLinks);
                        }
                    }
                }
                return links;
            }
        }

        private async Task<string> GetSitemapContent(Uri url)
        {
            string sitemapString = null;

            try
            {
                var sitemapUrl = new Uri(url + "sitemap.xml");
                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36";

                return await wc.DownloadStringTaskAsync(sitemapUrl);
            }
            catch
            {
                return sitemapString;
            }
        }

        private string GetAbsoluteUrlString(Uri domain, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri(domain.OriginalString), uri);

            return uri.GetLeftPart(UriPartial.Query);
        }

        private static async Task<int> GetResponseTime(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            var timer = new Stopwatch();
            timer.Start();
            using var response = await request.GetResponseAsync();
            timer.Stop();

            return timer.Elapsed.Milliseconds;
        }
    }
}
