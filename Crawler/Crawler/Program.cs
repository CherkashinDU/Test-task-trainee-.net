using System;
using System.Linq;
using System.Threading.Tasks;
using Crawler.Helpers;

namespace Crawler
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            string url;
            Uri uriResult = null;
            bool result = false;
            do
            {
                Console.WriteLine("Please enter website:");
                url = Console.ReadLine()?.ToLowerInvariant();
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

            var urlService = new UrlService();
            var display = new ShowUrl();

            var htmlUrls = await urlService.GetUrlsWebsite(uriResult);
            var sitemapUrls = await urlService.GetUrlsSitemap(uriResult);

            if (sitemapUrls.Count != 0)
            {
                Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site.");
                var uniqueSitemapUrls = sitemapUrls.Except(htmlUrls, new UrlComparer()).ToList();
                display.ShowUrls(uniqueSitemapUrls);

                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
                var uniqueHtmlUrls = htmlUrls.Except(sitemapUrls, new UrlComparer()).ToList();
                display.ShowUrls(uniqueHtmlUrls);

                var allLinks = htmlUrls.Concat(sitemapUrls).ToList();
                await display.ShowUrlsWithTime(allLinks);
            }
            else
            {
                Console.WriteLine("SITEMAP.XML NOT FOUND");
                Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE");
                await display.ShowUrlsWithTime(htmlUrls);
            }

            Console.WriteLine("Urls found after crawling a website: " + htmlUrls.Count);
            Console.WriteLine("Urls found in sitemap: " + sitemapUrls.Count);
        }
    }
}

