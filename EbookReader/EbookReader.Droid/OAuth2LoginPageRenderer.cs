using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using EbookReader.Droid;
using EbookReader.Model;
using EbookReader.Model.Messages;
using EbookReader.Page;
using EbookReader.Service;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OAuth2LoginPage), typeof(OAuth2LoginPageRenderer))]
namespace EbookReader.Droid {
    public class OAuth2LoginPageRenderer : PageRenderer {
        bool done = false;

        public OAuth2LoginPageRenderer(Context context) : base(context) {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e) {
            base.OnElementChanged(e);

            if (!done) {

                var activity = this.Context as Activity;

                var OAuth2Data = Xamarin.Forms.Application.Current.Properties["OAuth2Data"] as OAuth2RequestData;

                var auth = new OAuth2Authenticator(
                    OAuth2Data.ClientID,
                    OAuth2Data.Scope,
                    new Uri(OAuth2Data.AuthorizeUrl),
                    new Uri(OAuth2Data.RedirectUrl)
                );

                auth.Completed += (sender, arg) => {
                    if (arg.IsAuthenticated) {
                        IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2AccessTokenObtainedMessage {
                            AccessToken = arg.Account.Properties["access_token"],
                            Provider = OAuth2Data.Provider,
                        });
                    }

                    IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2LoginPageClosed {
                        Provider = OAuth2Data.Provider,
                    });
                };

                auth.Error += (sender, arg) => {
                    IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2LoginPageClosed {
                        Provider = OAuth2Data.Provider,
                    });
                };

                activity.StartActivity(auth.GetUI(activity));
                done = true;
            }
        }
    }
}