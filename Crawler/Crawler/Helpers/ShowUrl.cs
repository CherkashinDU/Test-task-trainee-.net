using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler.Helpers
{
    public class ShowUrl
    {
        public void ShowUrls(List<string> links)
        {
            foreach (var link in links)
            {
                Console.WriteLine(link);
            }
            Console.WriteLine(Environment.NewLine);
        }

        public async Task ShowUrlsWithTime(List<string> links)
        {
            var urlService = new UrlService();
            var linksWithTime = await Task.WhenAll(links.Select(urlService.GetPageInfo));

            Console.WriteLine("Timing");
            foreach (var item in linksWithTime.OrderBy(r => r.ResponseTime))
            {
                Console.WriteLine($"{item.Link} {item.ResponseTime}ms");
            }
            Console.WriteLine(Environment.NewLine);
        }

    }
}
