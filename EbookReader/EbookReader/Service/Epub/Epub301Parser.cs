using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EbookReader.Model.Navigation;
using PCLStorage;

namespace EbookReader.Service.Epub {
    public class Epub301Parser : Epub300Parser {
        public Epub301Parser(IFileService fileService, XElement package, IFolder folder, string contentBasePath) : base(fileService, package, folder, contentBasePath) {
        }
    }
}
