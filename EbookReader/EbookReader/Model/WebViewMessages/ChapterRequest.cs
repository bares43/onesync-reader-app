using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class ChapterRequest {
        public const string Name = "ChapterRequest";
        
        public string Chapter { get; set; }
    }
}
