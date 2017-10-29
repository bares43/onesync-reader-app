using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model;

namespace EbookReader.Service {
    public interface IEpubLoader {
        Task<Model.Epub> GetEpub(string filename, byte[] filedata);
        Task<string> GetChapter(Model.Epub epub, Model.Navigation.Item chapter);
        Task<Model.EpubLoader.HtmlResult> PrepareHTML(string html, string epubFolderName);
    }
}
