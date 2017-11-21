/*global Ebook*/
/*global Messages*/
/*global Base64*/
/*global csCallback*/
/*global Hammer*/
/*global Gestures*/

$.fn.removeClassRegex = function(regex) {
  return $(this).removeClass(function(index, classes) {
    return classes
      .split(/\s+/)
      .filter(function(c) {
        return regex.test(c);
      })
      .join(' ');
  });
};

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
  init: function(width, height, margin, fontSize, scrollSpeed, clickEverywhere, doubleSwipe) {
    this.webViewWidth = width;
    this.webViewHeight = height;
    this.webViewMargin = margin;
    this.fontSize = fontSize;
    this.scrollSpeed = scrollSpeed;
    this.clickEverywhere = clickEverywhere;
    this.doubleSwipe = doubleSwipe;

    this.htmlHelper.setFontSize();
    this.htmlHelper.setWidth();
    this.htmlHelper.setHeight();
    this.htmlHelper.setMargin();

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

    var endOfChapterLeft = document.getElementById("js-ebook-end-of-chapter").offsetLeft;
    this.totalPages = Math.ceil(endOfChapterLeft / this.pageWidth);
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

      var length = Ebook.pagerHelper.getCachedItem(page);
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
  loadImages: function(images) {
    images.forEach(function(item) {
      $("[data-js-ebook-image-id=" + item.ID + "]").attr("src", item.Data);
    });
  },
  resizeImages: function() {
    $("img").css("max-width", (Ebook.webViewWidth - (2 * Ebook.webViewMargin)) + "px");
    $("img").css("max-height", (Ebook.webViewHeight - (2 * Ebook.webViewMargin)) + "px");
  },
  pagerHelper: {
    cache: [],
    invalideCache: function() {
      this.cache = [];
    },
    getCachedItem: function(page) {
      var cache = this.cache.filter(function(item) {
        return item.page === page;
      })[0];

      if (cache) {
        return cache.length;
      }

      return undefined;
    },
    startOfPage: function(page) {
      page = Math.min(Math.max(page, 1), Ebook.totalPages);

      Ebook.pagerHelper.computeLengthOfAllPages();

      var start = 0;

      for (var i = 1; i < page; i++) {
        var length = this.getCachedItem(i);
        if (length !== undefined) {
          start += length;
        }
      }

      if (page > 1) {
        ++start;
      }

      return start;
    },
    markStartOfPage: function() {
      var rect = {
        top: Ebook.webViewMargin,
        left: Ebook.webViewMargin,
        width: Ebook.webViewWidth - (2 * Ebook.webViewMargin),
        height: Ebook.webViewHeight - (2 * Ebook.webViewMargin),
      };

      if (document.caretRangeFromPoint) {
        var range = document.caretRangeFromPoint(rect.left, rect.top);

        if (range !== null) {
          var mark = document.createElement("span");
          mark.setAttribute("class", "js-ebook-page-begin");

          range.insertNode(mark);
        }
      }
    },
    markAllPages: function() {
      var currentPage = Ebook.currentPage;

      for (var i = 1; i <= Ebook.totalPages; i++) {
        Ebook.goToPageFast(i);
        this.markStartOfPage();
      }

      Ebook.goToPageFast(currentPage);
    },
    computeLengthOfAllPages: function() {
      if (this.cache.length > 0) { 
        return; 
      }

      this.markAllPages();

      var html = document.getElementById("content").innerHTML;
      var pages = html.split('<span class="js-ebook-page-begin"></span>');

      var result = [];

      for (var i = 1; i <= Ebook.totalPages; i++) {
        var partOfHtml = pages[i];
        var clearText = this.clearText($("<p>" + partOfHtml + "</p>").text());
        var length = clearText.length;
        result.push({
          page: i,
          length: length,
        });
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
    removeMark: function() {
      var mark = document.getElementById("js-ebook-mark");

      if (mark !== null) {
        var textNode = mark.firstChild;
        if (textNode !== null) {
          var previousNode = mark.previousSibling;
          var text = textNode.nodeValue;

          if (previousNode !== null && previousNode.nodeType === Node.TEXT_NODE) {
            text = previousNode.nodeValue + text;
            previousNode.remove();
          }

          mark.parentNode.replaceChild(document.createTextNode(text), mark);
        } else {
          mark.remove();
        }
      }
    },
    createMark: function(node, position) {
      var regex = /(\s*\S+)(\s+)?([\S\s]*)?/;
      var counter = 0;
      var found = false;

      var text = node.nodeValue;

      var textBefore = "";

      var mark = document.createElement("span");
      mark.setAttribute("id", "js-ebook-mark");

      while (regex.test(text) && !found) {
        var match = regex.exec(text);

        var currentText = match[1];
        var whiteCharacters = match[2];

        text = match[3];

        if (text === undefined) {
          text = "";
        }

        var length = this.clearText(currentText).length;

        counter += length;

        if (counter >= position) {
          found = true;
          mark.appendChild(document.createTextNode(currentText + whiteCharacters + text));
        } else {
          textBefore += currentText + whiteCharacters;
        }
      }

      var textBeforeNode = document.createTextNode(textBefore);

      var parent = node.parentNode;

      parent.replaceChild(mark, node);
      parent.insertBefore(textBeforeNode, mark);
    },
    clearText: function(text) {
      return text.replace(/\s/g, '');
    },
  },
  htmlHelper: {
    setFontSize: function() {
      $("body").removeClassRegex(/^reader-font-size-/);
      $("body").addClass("reader-font-size-" + Ebook.fontSize);
    },
    setWidth: function() {
      $("#columns-outer").css("width", (Ebook.webViewWidth - (2 * Ebook.webViewMargin)) + "px");
    },
    setHeight: function() {
      $("#columns-outer").css("height", (Ebook.webViewHeight - (2 * Ebook.webViewMargin)) + "px");
    },
    setMargin: function() {
      $("body").removeClassRegex(/^reader-margin-/);
      $("body").addClass("reader-margin-" + Ebook.webViewMargin);
    },
    showContent: function() {
      document.getElementById("content").style.opacity = 1;
    },
    hideContent: function() {
      document.getElementById("content").style.opacity = 0;
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
      Ebook.init(data.Width, data.Height, data.Margin, data.FontSize, data.ScrollSpeed, data.ClickEverywhere, data.DoubleSwipe);
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
        } else {
          Ebook.goToPageFast(1);
          Ebook.messagesHelper.sendPageChange();
        }
      }, 5);
      
      setTimeout(function() {
        Ebook.htmlHelper.showContent();
      }, 5);
    },
    goToStartOfPage: function(data) {
      Ebook.goToPosition(Ebook.pagerHelper.startOfPage(data.Page));
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
    var swipeleft = new Hammer.Swipe({
      event: "swipeleft",
      direction: Hammer.DIRECTION_LEFT,
    });
    var swiperight = new Hammer.Swipe({
      event: "swiperight",
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

    hammer.add([doubleTap, tap, press, swipeleft, swiperight, swipeleftdouble, swiperightdouble]);

    doubleTap.recognizeWith(tap);
    tap.requireFailure([doubleTap]);

    hammer.on("singletap", function(e) {
      Gestures.actions.tap(e.center.x, e.center.y);
    });

    hammer.on("doubletap", function() {
      Gestures.actions.doubleTap();
    });

    hammer.on("press", function() {
      Gestures.actions.press();
    });

    hammer.on("swipeleft", function() {
      Gestures.actions.swipeLeft();
    });

    hammer.on("swiperight", function() {
      Gestures.actions.swipeRight();
    });

    hammer.on("swipeleftdouble", function() {
      Gestures.actions.swipeLeftDouble();
    });

    hammer.on("swiperightdouble", function() {
      Gestures.actions.swipeRightDouble();
    });
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
    swipeLeft: function() {
      Ebook.goToNextPage();
    },
    swipeRight: function() {
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
  },
};