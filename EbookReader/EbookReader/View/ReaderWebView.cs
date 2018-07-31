using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Service;
using Xam.Plugin.WebView.Abstractions;

namespace EbookReader.View {
    public class ReaderWebView : FormsWebView {

        public WebViewMessages Messages { get; }

        private IAssetsManager _assetsManager;

        public static ReaderWebView Factory() {
            return IocManager.Container.Resolve<ReaderWebView>();
        }

        public ReaderWebView(IAssetsManager assetsManager) : base() {
            _assetsManager = assetsManager;

            this.Messages = new WebViewMessages(this);
        }
    }
}
