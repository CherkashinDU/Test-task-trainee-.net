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
                if (!url.StartsWith(Uri.UriSchemeHttp) && !url.StartsWith(Uri.UriSchemeHttps))
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
            var htmlWeb = new HtmlWeb();
            var document = htmlWeb.Load(uriResult);
            var htmlUrls = link.GetLinksWebsite(link.GetLinksHtml(document, url), url);
            var sitemapUrls = link.GetLinksSitemap(url);

            if (sitemapUrls.Count != 0)
            {
                Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site.");
                var uniqueSitemapUrls = sitemapUrls.Except(htmlUrls).ToList();
                link.ShowLinks(uniqueSitemapUrls);

                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
                var uniqueHtmlUrls = htmlUrls.Except(sitemapUrls).ToList();
                link.ShowLinks(uniqueHtmlUrls);

                link.ShowLinksWithTime(uniqueHtmlUrls.Concat(uniqueSitemapUrls).ToList());
            }
            else
            {
                Console.WriteLine("SITEMAP.XML NOT FOUND");
                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE");
                link.ShowLinksWithTime(htmlUrls.ToList());
            }

            Console.WriteLine("Urls found after crawling a website: " + htmlUrls.Count);
            Console.WriteLine("Urls found in sitemap: " + sitemapUrls.Count);
        }
    }
}

