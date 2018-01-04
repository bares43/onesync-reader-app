using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class OpenUrl {
        public const string Name = "OpenUrl";

        public string Url { get; set; }
    }
}
