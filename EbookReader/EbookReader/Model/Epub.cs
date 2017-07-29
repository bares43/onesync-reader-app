using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model {
    public class Epub {

        public EpubVersion Version { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public IEnumerable<EpubSpine> Spines { get; set; }
        public IEnumerable<EpubFile> Files { get; set; }
        public string Folder { get; set; }
    }

    public enum EpubVersion {
        V200,
        V300,
        V301
    }
}
