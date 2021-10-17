using System;
using System.Linq;
using HtmlAgilityPack;

namespace Crawler
{
    static class Program
    {
        static void Main(string[] args)
        {
            string url;
            Uri uriResult = null;
            bool result = false;
            do
            {
                Console.WriteLine("Please enter website:");
                url = Console.ReadLine();
                if (!url.StartsWith(Uri.UriSchemeHttp) || !url.StartsWith(Uri.UriSchemeHttps))
                {
                    Console.WriteLine("Please add scheme: http:// or https://");
                }
                else
                {
                    result = Uri.TryCreate(url, UriKind.Absolute, out uriResult);
                }
            }
            while (!result);

            var link = new Links();
            var document = new HtmlWeb().Load(uriResult);

            var linksHtml = link.GetLinksWebsite(link.GetLinksHtml(document, url), url);
            linksHtml.Sort();
            var linksSitemap = link.GetLinksSitemap(url);
            linksSitemap.Sort();
            Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site.");
            var uniqueSitemapUrls = linksSitemap.Except(linksHtml).ToList();
            link.ShowLinks(uniqueSitemapUrls);

            Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
            var uniqueHtmlUrls = linksHtml.Except(linksSitemap).ToList();
            link.ShowLinks(uniqueHtmlUrls);

            link.ShowLinksWithTime(uniqueHtmlUrls.Concat(uniqueSitemapUrls).ToList());
            Console.WriteLine("Urls found after crawling a website: " + linksHtml.Count);
            Console.WriteLine("Urls found in sitemap: " + linksSitemap.Count);
        }
    }
}

