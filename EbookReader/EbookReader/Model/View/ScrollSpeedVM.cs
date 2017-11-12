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
    public class ScrollSpeedVM : BaseVM {

        IMessageBus _messageBus;

        public ScrollSpeedVM() {
            _messageBus = IocManager.Container.Resolve<IMessageBus>();
        }

        public List<int> Items => ScrollSpeedProvider.Items;

        public int Value {
            get => UserSettings.Reader.ScrollSpeed;
            set {
                if (UserSettings.Reader.ScrollSpeed == value)
                    return;

                UserSettings.Reader.ScrollSpeed = value;
                OnPropertyChanged();
            }
        }
    }
}
