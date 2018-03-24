using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Messages;
using EbookReader.Provider;
using EbookReader.Service;
using Autofac;

namespace EbookReader.Model.View {
    public class MarginVM : BaseVM {

        IMessageBus _messageBus;

        public MarginVM() {
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
        }

        public List<int> Items => MarginProvider.Items;

        public int Value {
            get => UserSettings.Reader.Margin;
            set {
                if (UserSettings.Reader.Margin == value)
                    return;

                UserSettings.Reader.Margin = value;
                OnPropertyChanged();
                _messageBus.Send(new ChangeMarginMessage { Margin = value });
                _messageBus.Send(new CloseQuickPanelMessage());
            }
        }
    }
}