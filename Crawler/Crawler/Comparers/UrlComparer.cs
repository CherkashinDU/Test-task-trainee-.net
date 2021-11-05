using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crawler
{
    public class UrlComparer : IEqualityComparer<string>
    {
        public bool Equals(string url1, string url2)
        {
            return new Uri(url1).AbsolutePath == new Uri(url2).AbsolutePath;
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return new Uri(obj).LocalPath.GetHashCode();
        }
    }
}
