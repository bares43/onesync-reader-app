using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Format {
    public class Ebook {

        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public List<Spine> Spines { get; set; }
        public IEnumerable<File> Files { get; set; }
        public string Folder { get; set; }
        public string ContentBasePath { get; set; }
        public List<Navigation.Item> Navigation { get; set; }
        public string Cover { get; set; }
    }
}
