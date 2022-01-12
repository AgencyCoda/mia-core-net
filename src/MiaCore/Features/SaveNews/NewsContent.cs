using System.Collections.Generic;

namespace MiaCore.Features.SaveNews
{
    internal class NewsContent
    {
        public List<NewsContentItem> Elements { get; set; }
    }

    internal class NewsContentItem
    {
        public int Type { get; set; }
        public string Text { get; set; }
        public string Filename { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string Caption { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

    }
}