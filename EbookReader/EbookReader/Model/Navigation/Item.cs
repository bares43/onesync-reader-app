using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Navigation {
    public class Item {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Depth { get; set; }
    }
}
