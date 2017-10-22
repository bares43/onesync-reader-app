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

        public Content() {

            // IOC
            _messages = IocManager.Container.Resolve<IWebViewMessages>();

            var items = this.GenerateItems();
            this.stackLayout = this.GetStackLayout(items);

            this.scrollView = new ScrollView {
                Content = stackLayout,
            };

            Padding = new Thickness(25, 20);

            Device.BeginInvokeOnMainThread(() => {
                Children.Add(scrollView);
            });

        }
        
        private StackLayout GetStackLayout(List<Model.Navigation.Item> items) {
            var stackLayout = new StackLayout() {
                Orientation = StackOrientation.Vertical
            };

            foreach (var item in this.GetItems(items)) {
                stackLayout.Children.Add(item);
            }

            return stackLayout;
        }

        private List<Label> GetItems(List<Model.Navigation.Item> items, int depth = 0) {

            var labels = new List<Label>();

            foreach (var item in items) {
                var label = new Label {
                    StyleId = item.Id,
                    Text = item.Title,
                    Margin = new Thickness(depth * 20, 0),
                    FontSize = Device.GetNamedSize(Device.RuntimePlatform == Device.Android ? NamedSize.Large : NamedSize.Medium, typeof(Label))
                };

                var tgr = new TapGestureRecognizer();
                tgr.Tapped += (s, e) => this.ClickToItem(item.Id);
                label.GestureRecognizers.Add(tgr);

                labels.Add(label);

                if (item.Children != null && item.Children.Any()) {
                    labels.AddRange(this.GetItems(item.Children, depth + 1));
                }
            }

            return labels;
        }

        private void ClickToItem(string id) {
        }

        private List<Model.Navigation.Item> GenerateItems() {
            return new List<Model.Navigation.Item> {
                new Model.Navigation.Item {
                    Title = "Jedna"
                },
                new Model.Navigation.Item {
                    Title = "Dva",
                    Children = new List<Model.Navigation.Item> {
                        new Model.Navigation.Item {
                            Title = "Tři",
                            Children = new List<Model.Navigation.Item> {
                                new Model.Navigation.Item {
                                    Title = "Čtyři",
                                }
                            }
                        },
                        new Model.Navigation.Item {
                            Title = "Pět",
                        }
                    }
                },
                new Model.Navigation.Item {
                    Title = "Šest",
                },
                new Model.Navigation.Item {
                    Title = "Sedm",
                },
                new Model.Navigation.Item {
                    Title = "Osm",
                },
                new Model.Navigation.Item {
                    Title = "Devět",
                    Id = "scroll",
                },
                new Model.Navigation.Item {
                    Title = "Deset",
                },
                new Model.Navigation.Item {
                    Title = "Jedenact",
                },
                new Model.Navigation.Item {
                    Title = "Dvanact",
                },
            };
        }
    }
}
