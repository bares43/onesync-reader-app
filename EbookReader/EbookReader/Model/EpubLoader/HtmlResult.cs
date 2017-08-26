using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.EpubLoader {
    public class HtmlResult {
        public string Html { get; set; }
        public IList<Image> Images { get; set; }
    }
}
