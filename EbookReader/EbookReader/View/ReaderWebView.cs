using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Service;
using HtmlAgilityPack;
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

            this.LoadWebViewLayout();
        }

        private async void LoadWebViewLayout() {
            var layout = await _assetsManager.GetFileContentAsync("layout.html");
            var js = await _assetsManager.GetFileContentAsync("reader.js");
            var css = await _assetsManager.GetFileContentAsync("reader.css");

            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode($"<script>{js}</script>"));
            doc.DocumentNode.Descendants("head").First().AppendChild(HtmlNode.CreateNode($"<style>{css}</style>"));

            Source = doc.DocumentNode.OuterHtml;
        }
    }
}
