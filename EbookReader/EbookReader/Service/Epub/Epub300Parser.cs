using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EbookReader.Model.Navigation;
using PCLStorage;

namespace EbookReader.Service.Epub {
    public class Epub300Parser : EpubParser {
        public Epub300Parser(XElement package, IFolder folder, string contentBasePath) : base(package, folder, contentBasePath) {
        }

        public override async Task<List<Item>> GetNavigation() {
            throw new NotImplementedException();
        }
    }
}
