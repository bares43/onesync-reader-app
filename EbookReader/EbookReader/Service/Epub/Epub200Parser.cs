using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookReader.Service.Epub {
    public class Epub200Parser : EpubParser {
        public Epub200Parser(XElement package) : base(package) {
        }
    }
}
