using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EbookReader.Service.Epub {
    public abstract class EpubParser {

        protected XElement Package { get; set; }

        public EpubParser(XElement package) {
            this.Package = package;
        }

        public virtual string GetTitle() {
            return this.GetMandatoryElementValue("title", this.GetMetadata().Descendants());
        }

        public virtual string GetLanguage() {
            return this.GetMandatoryElementValue("language", this.GetMetadata().Descendants());
        }

        public virtual string GetAuthor() {
            return this.GetOptionalElementValue("creator", this.GetMetadata().Descendants());
        }

        public virtual string GetDescription() {
            return this.GetOptionalElementValue("description", this.GetMetadata().Descendants());
        }

        public virtual IEnumerable<Model.EpubSpine> GetSpines() {
            return this.GetSpine()
                .Descendants()
                .Where(o => o.Name.LocalName == "itemref")
                .Select(o => new Model.EpubSpine {
                    Idref = o.Attributes().Where(i => i.Name.LocalName == "idref").First().Value
                })
                .ToList();
        }

        public virtual IEnumerable<Model.EpubFile> GetFiles() {
            return this.GetManifest()
                .Descendants()
                .Where(o => o.Name.LocalName == "item")
                .Select(o => new Model.EpubFile {
                    Id = o.Attributes().Where(i => i.Name.LocalName == "id").First().Value,
                    Href = o.Attributes().Where(i => i.Name.LocalName == "href").First().Value,
                    MediaType = o.Attributes().Where(i => i.Name.LocalName == "media-type").First().Value
                })
                .ToList();
        }

        private XElement GetMetadata() {
            return Package.Descendants().Where(o => o.Name.LocalName == "metadata").First();
        }

        private XElement GetManifest() {
            return Package.Descendants().Where(o => o.Name.LocalName == "manifest").First();
        }

        private XElement GetSpine() {
            return Package.Descendants().Where(o => o.Name.LocalName == "spine").First();
        }

        private string GetMandatoryElementValue(string localName, IEnumerable<XElement> elements) {
            return elements.Where(o => o.Name.LocalName == localName).First().Value;
        }

        private string GetOptionalElementValue(string localName, IEnumerable<XElement> elements) {
            var element = elements.Where(o => o.Name.LocalName == localName).FirstOrDefault();
            return element != null ? element.Value : string.Empty;
        }
    }
}
