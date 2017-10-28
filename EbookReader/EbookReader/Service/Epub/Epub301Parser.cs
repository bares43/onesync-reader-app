using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EbookReader.Model.Navigation;
using PCLStorage;

namespace EbookReader.Service.Epub {
    public class Epub301Parser : EpubParser {
        public Epub301Parser(XElement package, IFolder folder) : base(package, folder) {
        }

        public override async Task<List<Item>> GetNavigation() {
            throw new NotImplementedException();
        }
    }
}
