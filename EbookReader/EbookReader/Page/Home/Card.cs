using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Page.Home {
    public abstract class Card : StackLayout {

        public Card() {
            WidthRequest = 300;
            HeightRequest = 300;
            BackgroundColor = Color.LightGray;
        }

    }
}
