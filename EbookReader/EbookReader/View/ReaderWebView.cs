using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xam.Plugin.WebView.Abstractions;
using Autofac;

namespace EbookReader.View {
    public class ReaderWebView : FormsWebView {
        public static ReaderWebView Instance() {
            return IocManager.Container.Resolve<ReaderWebView>();
        }
    }
}
