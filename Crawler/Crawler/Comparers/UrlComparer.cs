using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Crawler
{
    public class UrlComparer : IEqualityComparer<string>
    {
        public bool Equals(string url1, string url2)
        {
            return new Uri(url1).AbsolutePath.TrimEnd('/') == new Uri(url2).AbsolutePath.TrimEnd('/');
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return new Uri(obj).AbsolutePath.TrimEnd('/').GetHashCode();
        }
    }
}
