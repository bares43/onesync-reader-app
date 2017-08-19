using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class PageChange {
        public const string Name = "PageChange";

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
