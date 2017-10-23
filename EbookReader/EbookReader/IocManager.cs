using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model;
using EbookReader.Service;
using EbookReader.Service.Epub;
using Xam.Plugin.WebView.Abstractions;

namespace EbookReader {
    public static class IocManager {

        private static ContainerBuilder _containerBuilder;
        public static ContainerBuilder ContainerBuilder {
            get {
                if (_containerBuilder == null) {
                    _containerBuilder = new ContainerBuilder();

                    SetUpIoc();
                }

                return _containerBuilder;
            }
        }

        private static IContainer _container;
        public static IContainer Container {
            get {
                return _container;
            }
        }

        public static void Build() {
            _container = ContainerBuilder.Build();
        }

        private static void SetUpIoc() {
            ContainerBuilder.RegisterType<EpubLoader>().As<IEpubLoader>();
            ContainerBuilder.RegisterType<FileService>().As<IFileService>();
            ContainerBuilder.RegisterType<FormsWebView>().As<FormsWebView>().SingleInstance();
            ContainerBuilder.RegisterType<WebViewMessages>().As<IWebViewMessages>().SingleInstance();
            ContainerBuilder.RegisterType<Epub200Parser>().Keyed<EpubParser>(EpubVersion.V200);
            ContainerBuilder.RegisterType<Epub300Parser>().Keyed<EpubParser>(EpubVersion.V300);
            ContainerBuilder.RegisterType<Epub301Parser>().Keyed<EpubParser>(EpubVersion.V301);
        }

    }
}
