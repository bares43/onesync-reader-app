using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Page.Reader.QuickPanelTab {
    public class Content : StackLayout {

        private IWebViewMessages _messages;
        private ScrollView scrollView;
        private StackLayout stackLayout;

        public event EventHandler<Model.Navigation.Item> OnChapterChange;

        public Content() {

            // IOC
            _messages = IocManager.Container.Resolve<IWebViewMessages>();

            this.stackLayout = new StackLayout {
                Orientation = StackOrientation.Vertical
            };

            this.scrollView = new ScrollView {
                Content = stackLayout,
            };

            Padding = new Thickness(25, 20);

            Device.BeginInvokeOnMainThread(() => {
                Children.Add(scrollView);
            });

        }
        
        public void SetNavigation(List<Model.Navigation.Item> items) {
            Device.BeginInvokeOnMainThread(() => {
                this.SetItems(items);
            });
        }

        private void SetItems(List<Model.Navigation.Item> items) {
            stackLayout.Children.Clear();

            foreach (var item in this.GetItems(items)) {
                stackLayout.Children.Add(item);
            }
        }

        private List<Label> GetItems(List<Model.Navigation.Item> items, string id = "", int depth = 0) {

            var labels = new List<Label>();

            foreach (var item in items) {
                var label = new Label {
                    StyleId = item.Id,
                    Text = item.Title,
                    Margin = new Thickness(depth * 20, 0),
                    FontSize = Device.GetNamedSize(Device.RuntimePlatform == Device.Android ? NamedSize.Large : NamedSize.Medium, typeof(Label))
                };

                var tgr = new TapGestureRecognizer();
                tgr.Tapped += (s, e) => this.ClickToItem(item);
                label.GestureRecognizers.Add(tgr);

                labels.Add(label);

                var children = items.Where(o => o.ParentID == item.Id).ToList();

                if (children.Any(o => o.ParentID == item.Id)) {
                    labels.AddRange(this.GetItems(items, item.Id, depth + 1));
                }
            }

            return labels;
        }

        private void ClickToItem(Model.Navigation.Item item) {
            this.OnChapterChange?.Invoke(this, item);
        }
        
    }
}
