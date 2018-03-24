using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.EpubLoader;
using EbookReader.Model.Format;
using HtmlAgilityPack;

namespace EbookReader.Service {
    public class HtmlLoader : OneFileLoader {
        public HtmlLoader(IFileService fileService) : base(fileService) {
            Extensions = new string[] { "html", "xhtml", "htm", "xhtm" };
            EbookFormat = EbookFormat.Html;
        }
    }
}
