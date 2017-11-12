using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Provider;
using EbookReader.Service;

namespace EbookReader.Model.View {
    public class FontSizeVM : BaseVM {

        IMessageBus _messageBus;

        public FontSizeVM() {
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
        }

        public List<int> Items => FontSizeProvider.Items;

        public int Value {
            get => UserSettings.Reader.FontSize;
            set {
                if (UserSettings.Reader.FontSize == value)
                    return;

                UserSettings.Reader.FontSize = value;
                OnPropertyChanged();
                _messageBus.Send(new ChangeFontSize { FontSize = value });
                _messageBus.Send(new CloseQuickPanel());
            }
        }
    }
}