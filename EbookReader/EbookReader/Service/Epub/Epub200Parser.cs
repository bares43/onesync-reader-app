using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EbookReader.Model.Navigation;
using PCLStorage;

namespace EbookReader.Service.Epub {
    public class Epub200Parser : EpubParser {

        private IFileService _fileService;

        public Epub200Parser(IFileService fileService, XElement package, IFolder folder, string contentBasePath) : base(package, folder, contentBasePath) {
            _fileService = fileService;
        }

        public override async Task<List<Item>> GetNavigation() {
            var navigation = new List<Item>();
            var tocFilename = this.GetTocFilename();

            if (!string.IsNullOrEmpty(tocFilename)) {
                var tocFile = await _fileService.OpenFile($"{ContentBasePath}{tocFilename}", Folder);
                var tocFileData = await tocFile.ReadAllTextAsync();
                var xmlContainer = XDocument.Parse(tocFileData);
                var items = xmlContainer.Root.Descendants().First(o => o.Name.LocalName == "navMap").Elements();

                navigation = this.LoadItems(items);
            }

            return navigation;
        }

        public override string GetCover() {
            var cover = string.Empty;

            var id = this.GetAttributeOnElementWithAttributeValue(this.GetMetadata(), "content", "name", "cover", "meta");

            if (!string.IsNullOrEmpty(id)) {
                cover = $"{ContentBasePath}{this.GetFiles().First(o => o.Id == id).Href}";
            }

            return cover;
        }

        private List<Item> LoadItems(IEnumerable<XElement> elements, int depth = 0) {
            var items = new List<Item>();

            var orderedElements = elements.Where(o => o.Name.LocalName == "navPoint").OrderBy(o => {
                var x = 0;
                var order = o.Attributes().FirstOrDefault(i => i.Name.LocalName == "playOrder");
                if (order != null) {
                    int.TryParse(order.Value, out x);
                }

                return x;
            });

            foreach (var element in orderedElements) {
                var descendants = element.Elements();

                var label = descendants
                    .First(o => o.Name.LocalName == "navLabel")
                    .Descendants()
                    .First(o => o.Name.LocalName == "text")
                    .Value;

                var id = descendants
                    .First(o => o.Name.LocalName == "content")
                    .Attributes()
                    .First(o => o.Name.LocalName == "src")
                    .Value;

                var item = new Item {
                    Id = id,
                    Title = label,
                    Depth = depth,
                };

                items.Add(item);

                items.AddRange(this.LoadItems(element.Elements(), depth + 1));
            }

            return items;
        }

        private string GetTocFilename() {
            var filename = string.Empty;

            var attr = Package
                .Descendants()
                .Where(o => o.Name.LocalName == "spine")
                .First()
                .Attributes()
                .Where(o => o.Name.LocalName == "toc")
                .Select(o => o.Value)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(attr)) {
                filename = this.GetFiles().Where(o => o.Id == attr).Select(o => o.Href).FirstOrDefault();
            }

            return filename;
        }
    }
}