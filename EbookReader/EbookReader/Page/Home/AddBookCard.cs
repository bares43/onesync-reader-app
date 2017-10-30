using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Page.Home {
    public class AddBookCard : Card {
        public AddBookCard() : base() {
            Children.Add(new Label { Text = "+" });

            var messageBus = IocManager.Container.Resolve<IMessageBus>();

            GestureRecognizers.Add(
                new TapGestureRecognizer {
                    Command = new Command(() => { messageBus.Send(new AddBookClicked()); }),
                }
            );
        }
    }
}
