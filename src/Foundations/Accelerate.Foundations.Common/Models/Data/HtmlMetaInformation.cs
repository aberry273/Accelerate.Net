using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Data
{
    public class HtmlMetaInformation
    {
        public bool HasData { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Image { get; set; }
        public string SiteName { get; set; }

        public HtmlMetaInformation(string url)
        {
            Url = url;
            HasData = false;
        }

        public HtmlMetaInformation(string url, string title, string description, string keywords, string image, string siteName)
        {
            Url = url;
            Title = title;
            Description = description;
            Keywords = keywords;
            Image = image;
            SiteName = siteName;
        }
    }
}
