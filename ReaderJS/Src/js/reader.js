/*global Ebook*/
/*global Messages*/
/*global Base64*/
/*global csCallback*/
/*global Hammer*/
/*global Gestures*/
window.Ebook = {
  pageWidth: 0,
  totalPages: 0,
  currentPage: 1,
  fontSize: 0,
  webViewWidth: 0,
  webViewHeight: 0,
  webViewMargin: 0,
  scrollSpeed: 0,
  clickEverywhere: false,
  doubleSwipe: false,
  nightMode: false,
  panEventCounter: 0,
  init: function(width, height, margin, fontSize, scrollSpeed, clickEverywhere, doubleSwipe, nightMode) {
    this.webViewWidth = width;
    this.webViewHeight = height;
    this.webViewMargin = margin;
    this.fontSize = fontSize;
    this.scrollSpeed = scrollSpeed;
    this.clickEverywhere = clickEverywhere;
    this.doubleSwipe = doubleSwipe;
    this.nightMode = nightMode;

    this.htmlHelper.setFontSize();
    this.htmlHelper.setWidth();
    this.htmlHelper.setHeight();
    this.htmlHelper.setMargin();
    this.htmlHelper.setNightMode();

    this.setUpColumns();
    this.setUpEvents();
  },
  setUpEvents: function() {
    var wrapper = document.getElementsByTagName("body")[0];

    Gestures.init(wrapper);
  },
  setUpEbook: function() {
    this.resizeImages();

    this.pagerHelper.invalideCache();

    this.goToPageFast(1);

    this.totalPages = this.getPageOfMarker("js-ebook-end-of-chapter");

    this.setUpLinksListener();
  },
  setUpColumns: function() {
    var columnsInner = document.getElementById("columns-inner");
    this.pageWidth = columnsInner.getBoundingClientRect().width;
    columnsInner.style["column-width"] = this.pageWidth + "px";
  },
  resize: function(width, height) {
    Ebook.htmlHelper.hideContent();
    var position = Ebook.getCurrentPosition();

    Ebook.goToPageFast(1);
    Ebook.webViewWidth = width;
    Ebook.webViewHeight = height;
    Ebook.htmlHelper.setWidth();
    Ebook.htmlHelper.setHeight();

    Ebook.setUpColumns();
    Ebook.setUpEbook();

    setTimeout(function() {
      Ebook.goToPositionFast(position);
      Ebook.htmlHelper.showContent();
    }, 5);
  },
  changeFontSize: function(fontSize) {
    Ebook.htmlHelper.hideContent();
    var position = Ebook.getCurrentPosition();

    Ebook.goToPageFast(1);
    Ebook.fontSize = fontSize;
    Ebook.htmlHelper.setFontSize();

    Ebook.setUpColumns();
    Ebook.setUpEbook();

    setTimeout(function() {
      Ebook.goToPositionFast(position);
      Ebook.htmlHelper.showContent();
    }, 5);
  },
  changeMargin: function(margin) {
    Ebook.htmlHelper.hideContent();
    var position = Ebook.getCurrentPosition();

    Ebook.goToPageFast(1);
    Ebook.webViewMargin = margin;
    Ebook.htmlHelper.setWidth();
    Ebook.htmlHelper.setHeight();
    Ebook.htmlHelper.setMargin();

    Ebook.setUpColumns();
    Ebook.setUpEbook();

    setTimeout(function() {
      Ebook.goToPositionFast(position);
      Ebook.htmlHelper.showContent();
    }, 5);
  },
  goToNextPage: function() {
    var page = this.currentPage + 1;
    if (page <= this.totalPages) {
      this.goToPage(page);
    } else {
      this.messagesHelper.nextChapterRequest();
    }
  },
  goToPreviousPage: function() {
    var page = this.currentPage - 1;
    if (page >= 1) {
      this.goToPage(page);
    } else {
      this.messagesHelper.prevChapterRequest();
    }
  },
  goToPage: function(page, duration) {
    if (duration === undefined) {
      duration = Ebook.scrollSpeed;
    }

    this.goToPageInternal(page, duration);

    this.messagesHelper.sendPageChange();
  },
  goToPageFast: function(page) {
    this.goToPageInternal(page, 0);
  },
  goToPageInternal: function(page, duration) {
    this.currentPage = page;

    $('#columns-outer').animate({
      scrollLeft: (page - 1) * this.pageWidth,
    }, duration);
  },
  goToPosition: function(position, duration) {
    Ebook.pagerHelper.computeLengthOfAllPages();

    var page = 0;
    var currentPosition = 0;

    while (currentPosition < position) {
      page++;

      var length = Ebook.pagerHelper.cache.get(page);
      if (length !== undefined) {
        currentPosition += length;
      }
    }

    this.goToPage(page, duration);
  },
  goToPositionFast: function(position) {
    this.goToPosition(position, 0);
  },
  getCurrentPosition: function() {
    return this.pagerHelper.startOfPage(this.currentPage);
  },
  setUpLinksListener: function() {
    var links = document.getElementById("content").getElementsByTagName("a");
    for (var i = 0; i < links.length; i++) {
      links[i].addEventListener("click", function(e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
        e.cancelBubble = true;
        var link = this;
        var href = link.getAttribute("href");
        if (href) {
          if (href.startsWith("#")) {
            Ebook.goToMarker(href.slice(1));
          } else if (link.hostname) {
            Ebook.messagesHelper.sendOpenUrl(href);
          } else {
            Ebook.messagesHelper.sendChapterRequest(href);
          }
        }

        return false;
      }, false);
    }
  },
  getPageOfMarker: function(marker) {
    var currentPage = this.currentPage;
    this.goToPageFast(1);
    var position = document.getElementById(marker).getBoundingClientRect().left;
    this.goToPageFast(currentPage);
    return Math.ceil(position / this.pageWidth);
  },
  goToMarker: function(marker, duration) {
    var page = this.getPageOfMarker(marker);
    if (page > 0) {
      this.goToPage(page, duration);
    }
  },
  loadImages: function(images) {
    images.forEach(function(item) {
      $("[data-js-ebook-image-id=" + item.ID + "]").attr("src", item.Data);
    });
  },
  resizeImages: function() {
    $("img").css("max-width", (Ebook.webViewWidth - (2 * Ebook.webViewMargin)) + "px");
    $("img").css("max-height", (Ebook.webViewHeight - (2 * Ebook.webViewMargin)) + "px");
  },
  panEventHandler: function(x, y, isFinal) {
    if (isFinal) {
      Ebook.panEventCounter = 0;
    }

    if (Ebook.panEventCounter % 10 === 0) {
      Ebook.messagesHelper.sendPanEvent(x, y);
    }
    Ebook.panEventCounter++;
  },
  pagerHelper: {
    cache: new Map(),
    invalideCache: function() {
      this.cache = new Map();
    },
    startOfPage: function(page) {
      page = Math.min(Math.max(page, 1), Ebook.totalPages);

      Ebook.pagerHelper.computeLengthOfAllPages();

      var start = 0;

      for (var i = 1; i < page; i++) {
        var length = this.cache.get(i);
        if (length !== undefined) {
          start += length;
        }
      }

      if (page > 1) {
        ++start;
      }

      return start;
    },
    markAllPages: function() {
      var currentPage = Ebook.currentPage;

      var rect = {
        top: Ebook.webViewMargin,
        left: Ebook.webViewMargin,
      };

      var ranges = [];
      for (var i = 1; i <= Ebook.totalPages; i++) {
        Ebook.goToPageFast(i);

        var range = document.caretRangeFromPoint(rect.left, rect.top);
        
        if (range !== null) {
          ranges.push(range);
        }
      }

      ranges.forEach(function(range) {
        var mark = document.createElement("span");
        mark.setAttribute("class", "js-ebook-page-begin");

        range.insertNode(mark);
      });

      Ebook.goToPageFast(currentPage);
    },
    computeLengthOfAllPages: function() {
      if (this.cache.size > 0) { 
        return; 
      }

      this.markAllPages();

      var html = document.getElementById("content").innerHTML;
      var pages = html.split('<span class="js-ebook-page-begin"></span>');

      var result = new Map();

      for (var i = 1; i <= Ebook.totalPages; i++) {
        var clearText = this.clearText(this.stripHtmlTags(pages[i]));
        var length = clearText.length;
        result.set(i, length);
      }

      var mark = document.getElementsByClassName("js-ebook-page-begin")[0];

      while (mark !== undefined) {
        var previousNode = mark.previousSibling;
        var nextNode = mark.nextSibling;

        if (previousNode !== null && nextNode !== null && previousNode.nodeType === Node.TEXT_NODE && nextNode.nodeType === Node.TEXT_NODE) {
          var text = previousNode.nodeValue + nextNode.nodeValue;
          var textNode = document.createTextNode(text);

          var parent = mark.parentNode;
          parent.replaceChild(textNode, mark);

          previousNode.remove();
          nextNode.remove();
        } else {
          mark.remove();
        }

        mark = document.getElementsByClassName("js-ebook-page-begin")[0];
      }

      Ebook.pagerHelper.cache = result;
    },
    stripHtmlTags: function(html) {
      var div = document.createElement("div");
      div.innerHTML = html;
      return div.textContent;
    },
    clearText: function(text) {
      return text.replace(/\s/g, '');
    },
  },
  htmlHelper: {
    setFontSize: function() {
      var body = document.getElementsByTagName("body")[0];
      body.className = body.className.replace(/reader-font-size-\S+/, " ");
      body.classList.add("reader-font-size-" + Ebook.fontSize);
    },
    setWidth: function() {
      document.getElementById("columns-outer").style.width = (Ebook.webViewWidth - (2 * Ebook.webViewMargin)) + "px";
    },
    setHeight: function() {
      document.getElementById("columns-outer").style.height = (Ebook.webViewHeight - (2 * Ebook.webViewMargin)) + "px";
    },
    setMargin: function() {
      var body = document.getElementsByTagName("body")[0];
      body.className = body.className.replace(/reader-margin-\S+/, " ");
      body.classList.add("reader-margin-" + Ebook.webViewMargin);
    },
    showContent: function() {
      document.getElementById("content").style.opacity = 1;
    },
    hideContent: function() {
      document.getElementById("content").style.opacity = 0;
    },
    setNightMode: function() {
      var body = document.getElementsByTagName("body")[0];
      var className = "reader-night-mode";
      if (Ebook.nightMode) {
        body.classList.add(className);
      } else {
        body.classList.remove(className);
      }
    },
  },
  messagesHelper: {
    sendPageChange: function() {
      Messages.send("PageChange", {
        CurrentPage: Ebook.currentPage,
        TotalPages: Ebook.totalPages,
        Position: Ebook.getCurrentPosition(),
      });
    },
    nextChapterRequest: function() {
      Messages.send("NextChapterRequest", {});
    },
    prevChapterRequest: function() {
      Messages.send("PrevChapterRequest", {});
    },
    sendOpenQuickPanelRequest: function() {
      Messages.send("OpenQuickPanelRequest", {});
    },
    sendChapterRequest: function(chapter) {
      Messages.send("ChapterRequest", {
        Chapter: chapter,
      });
    },
    sendOpenUrl: function(url) {
      Messages.send("OpenUrl", {
        Url: url,
      });
    },
    sendPanEvent: function(x, y) {
      Messages.send("PanEvent", {
        X: x,
        Y: y,
      });
    },
  },
};
window.Messages = {
  send: function(action, data) {
    var json = JSON.stringify({
      action: action,
      data: data,
    });

    csCallback(Base64.encode(json));
  },
  parse: function(data) {
    var json = JSON.parse(Base64.decode(data));
    this.actions[json.Action](json.Data);
  },
  actions: {
    init: function(data) {
      Ebook.init(data.Width, data.Height, data.Margin, data.FontSize, data.ScrollSpeed, data.ClickEverywhere, data.DoubleSwipe, data.NightMode);
    },
    loadHtml: function(data) {
      Ebook.htmlHelper.hideContent();

      document.getElementById("content").innerHTML = data.Html;

      Ebook.loadImages(data.Images);
      Ebook.setUpEbook();

      setTimeout(function() {
        if (data.Position > 0) {
          Ebook.goToPositionFast(data.Position);
        } else if (data.LastPage) {
          Ebook.goToPageFast(Ebook.totalPages);
          Ebook.messagesHelper.sendPageChange();
        } else if (data.Marker) {
          Ebook.goToMarker(data.Marker);
        } else {
          Ebook.goToPageFast(1);
          Ebook.messagesHelper.sendPageChange();
        }
      }, 5);
      
      setTimeout(function() {
        Ebook.htmlHelper.showContent();
      }, 5);
    },
    goToPosition: function(data) {
      Ebook.goToPositionFast(data.Position);
    },
    changeFontSize: function(data) {
      Ebook.changeFontSize(data.FontSize);
    },
    resize: function(data) {
      Ebook.resize(data.Width, data.Height);
    },
    changeMargin: function(data) {
      Ebook.changeMargin(data.Margin);
    },
    goToPage: function(data) {
      if (data.Page > 0) {
        Ebook.goToPage(data.Page);
      } else if (data.Next) {
        Ebook.goToNextPage();
      } else if (data.Previous) {
        Ebook.goToPreviousPage();
      }
    },
  },
};

window.Gestures = {
  init: function(element) {
    var hammer = new Hammer.Manager(element);

    var tap = new Hammer.Tap({
      event: "singletap",
    });
    var doubleTap = new Hammer.Tap({
      event: "doubletap",
      taps: 2,
    });
    var press = new Hammer.Press({
      event: "press",
    });
    var panleft = new Hammer.Pan({
      event: "panleft",
      direction: Hammer.DIRECTION_LEFT,
    });
    var panright = new Hammer.Pan({
      event: "panright",
      direction: Hammer.DIRECTION_RIGHT,
    });
    var swipeleftdouble = new Hammer.Swipe({
      event: "swipeleftdouble",
      direction: Hammer.DIRECTION_LEFT,
      pointers: 2,
    });
    var swiperightdouble = new Hammer.Swipe({
      event: "swiperightdouble",
      direction: Hammer.DIRECTION_RIGHT,
      pointers: 2,
    });
    var panbottom = new Hammer.Pan({
      event: "panbottom",
      direction: Hammer.DIRECTION_DOWN,
    });
    var pantop = new Hammer.Pan({
      event: "pantop",
      direction: Hammer.DIRECTION_UP,
    });

    hammer.add([doubleTap, tap, press, panleft, panright, swipeleftdouble, swiperightdouble, panbottom, pantop]);

    doubleTap.recognizeWith(tap);
    tap.requireFailure([doubleTap]);

    hammer.on("singletap", function(e) {
      if (!Gestures.isLink(e)) {
        Gestures.actions.tap(e.center.x, e.center.y);
      }
    });

    hammer.on("doubletap", function(e) {
      if (!Gestures.isLink(e)) {
        Gestures.actions.doubleTap();
      }
    });

    hammer.on("press", function(e) {
      if (!Gestures.isLink(e)) {
        Gestures.actions.press();
      }
    });

    hammer.on("panleft", function(e) {
      if (e.isFinal) {
        Gestures.actions.panLeft();
      }
    });

    hammer.on("panright", function(e) {
      if (e.isFinal) {
        Gestures.actions.panRight();
      }
    });

    hammer.on("swipeleftdouble", function() {
      Gestures.actions.swipeLeftDouble();
    });

    hammer.on("swiperightdouble", function() {
      Gestures.actions.swipeRightDouble();
    });

    hammer.on("panbottom", function(e) {
      Gestures.actions.panBottom(e.center.x, e.center.y, e.isFinal);
    });

    hammer.on("pantop", function(e) {
      Gestures.actions.panTop(e.center.x, e.center.y, e.isFinal);
    });
  },
  isLink: function(e) {
    if (e && e.target) {
      var el = e.target;
      while (el !== null && el.id !== "content") {
        if (el.localName === "a") {
          return true;
        }

        el = el.parentElement;
      }
    }
    return false;
  },
  actions: {
    tap: function(x) {
      if (Ebook.clickEverywhere || x > Math.round(Ebook.pageWidth / 2)) {
        Ebook.goToNextPage();
      } else {
        Ebook.goToPreviousPage();
      }
    },
    doubleTap: function() {
      Ebook.messagesHelper.sendOpenQuickPanelRequest();
    },
    press: function() {
      Ebook.messagesHelper.sendOpenQuickPanelRequest();
    },
    panLeft: function() {
      Ebook.goToNextPage();
    },
    panRight: function() {
      Ebook.goToPreviousPage();
    },
    swipeLeftDouble: function() {
      if (Ebook.doubleSwipe) {
        Ebook.messagesHelper.nextChapterRequest();
      }
    },
    swipeRightDouble: function() {
      if (Ebook.doubleSwipe) {
        Ebook.goToPage(1);
      }
    },
    panBottom: function(x, y, isFinal) {
      Ebook.panEventHandler(x, y, isFinal);
    },
    panTop: function(x, y, isFinal) {
      Ebook.panEventHandler(x, y, isFinal);
    },
  },
};