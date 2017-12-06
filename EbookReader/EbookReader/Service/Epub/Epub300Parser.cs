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

        IFileService _fileService;

        public Epub300Parser(IFileService fileService, XElement package, IFolder folder, string contentBasePath) : base(package, folder, contentBasePath) {
            _fileService = fileService;
        }

        public override async Task<List<Item>> GetNavigation() {

            var navigation = new List<Item>();
            var tocFilename = this.GetAttributeOnElementWithAttributeValue(this.GetManifest(), "href", "properties", "nav", "item");

            if (!string.IsNullOrEmpty(tocFilename)) {
                var tocFile = await _fileService.OpenFile($"{ContentBasePath}{tocFilename}", Folder);
                var tocFileData = await tocFile.ReadAllTextAsync();
                var xmlContainer = XDocument.Parse(tocFileData);

                var navItem = xmlContainer.Root.Descendants()
                    .FirstOrDefault(o => o.Name.LocalName == "nav" && o.Attributes().Any(i => i.Name.LocalName == "type" && i.Value == "toc"));

                if (navItem != null) {
                    var olItem = navItem.Elements().FirstOrDefault(o => o.Name.LocalName == "ol");

                    if (olItem != null) {
                        foreach (var item in olItem.Elements().Where(o => o.Name.LocalName == "li")) {
                            var a = item.Elements().FirstOrDefault(o => o.Name.LocalName == "a");
                            if (a != null) {
                                var href = a.Attributes().FirstOrDefault(o => o.Name.LocalName == "href");

                                if (href != null) {
                                    navigation.Add(new Item {
                                        Id = href.Value,
                                        Title = a.Value
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return navigation;
        }

        public override string GetCover() {
            var cover = string.Empty;

            var href = this.GetAttributeOnElementWithAttributeValue(this.GetManifest(), "href", "properties", "cover-image", "item");

            if (!string.IsNullOrEmpty(href)) {
                cover = $"{ContentBasePath}{href}";
            }

            return cover;
        }
    }
}
