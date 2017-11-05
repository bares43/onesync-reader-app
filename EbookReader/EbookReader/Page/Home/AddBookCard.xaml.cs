using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Home {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddBookCard : StackLayout {
        public AddBookCard() {
            InitializeComponent();

            BindingContext = new {
                Width = Card.CardWidth,
                Height = Card.CardHeight,
            };

            var messageBus = IocManager.Container.Resolve<IMessageBus>();

            GestureRecognizers.Add(
                new TapGestureRecognizer {
                    Command = new Command(() => { messageBus.Send(new AddBookClicked()); }),
                }
            );

        }
    }
}