using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public interface IWebViewMessages {

        event EventHandler<Model.WebViewMessages.PageChange> OnPageChange;
        event EventHandler<Model.WebViewMessages.NextChapterRequest> OnNextChapterRequest;
        event EventHandler<Model.WebViewMessages.PrevChapterRequest> OnPrevChapterRequest;
        event EventHandler<Model.WebViewMessages.OpenQuickPanelRequest> OnOpenQuickPanelRequest;

        void Send(string action, object data);
        void Parse(string data);
    }
}
