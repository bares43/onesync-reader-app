# OneSync Reader

E-book reader in Xamarin.Forms with synchronization via Dropbox or Firebase.

https://bares43.github.io/onesync-reader/

## Getting Started

### Prerequisites
You need Visual Studio with Xamarin support.
You need to install yarn.

### Installing

Go to ReaderJS folder and run:

```
yarn install
```
```
grunt build
```

Go to EbookReader/EbookReader folder, create copy of ReaderApp.config and name it ReaderApp.Release.config. Here is place for you production setting of app.

What you can configure:
* Firebase_BaseUrl
* Firebase_ApiKey
* Dropbox_ClientID
* AppCenter_Android
* AppCenter_UWP

## Authors

* **Jan Bareš** - *Initial work* - [bares43](https://github.com/bares43)

See also the list of [contributors](https://github.com/bares43/thesis-ebook-reader/graphs/contributors) who participated in this project.