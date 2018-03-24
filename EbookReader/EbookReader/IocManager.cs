using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Service;
using EbookReader.Service.Epub;
using EbookReader.View;
using EbookReader.Provider;
using EbookReader.Model.Format.EpubFormat;
using EbookReader.Model.Format;
using EbookReader.Repository;

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
            if(_container == null) {
                _container = ContainerBuilder.Build();
            }
        }

        private static void SetUpIoc() {
            ContainerBuilder.RegisterType<EpubLoader>().Keyed<IBookLoader>(EbookFormat.Epub);
            ContainerBuilder.RegisterType<TxtLoader>().Keyed<IBookLoader>(EbookFormat.Txt);
            ContainerBuilder.RegisterType<HtmlLoader>().Keyed<IBookLoader>(EbookFormat.Html);
            ContainerBuilder.RegisterType<FileService>().As<IFileService>();
            ContainerBuilder.RegisterType<Epub200Parser>().Keyed<EpubParser>(EpubVersion.V200);
            ContainerBuilder.RegisterType<Epub300Parser>().Keyed<EpubParser>(EpubVersion.V300);
            ContainerBuilder.RegisterType<Epub301Parser>().Keyed<EpubParser>(EpubVersion.V301);
            ContainerBuilder.RegisterType<MessageBus>().As<IMessageBus>().SingleInstance();
            ContainerBuilder.RegisterType<BookshelfService>().As<IBookshelfService>();
            ContainerBuilder.RegisterType<ReaderWebView>().As<ReaderWebView>();
            ContainerBuilder.RegisterType<SyncService>().As<ISyncService>();
            ContainerBuilder.RegisterType<DumbCloudStorageService>().Keyed<ICloudStorageService>(SynchronizationServicesProvider.Dumb);
            ContainerBuilder.RegisterType<DropboxCloudStorageService>().Keyed<ICloudStorageService>(SynchronizationServicesProvider.Dropbox);
            ContainerBuilder.RegisterType<FirebaseCloudStorageService>().Keyed<ICloudStorageService>(SynchronizationServicesProvider.Firebase);
            ContainerBuilder.RegisterType<DatabaseService>().As<IDatabaseService>().SingleInstance();
            ContainerBuilder.RegisterType<BookRepository>().As<IBookRepository>();
            ContainerBuilder.RegisterType<BookmarkRepository>().As<IBookmarkRepository>();
            ContainerBuilder.RegisterType<BookmarkService>().As<IBookmarkService>();
        }

    }
}
