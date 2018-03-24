using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class PanEvent {
        public const string Name = "PanEvent";

        public int X { get; set; }
        public int Y { get; set; }
    }
}
