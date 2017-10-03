/*global Ebook*/
/*global Messages*/
/*global Base64*/
/*global csCallback*/

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
  scrollStep: 0,
  totalPages: 0,
  currentPage: 1,
  fontSize: 0,
  webViewWidth: 0,
  webViewHeight: 0,
  webViewMargin: 0,
  init: function(width, height, margin, fontSize) {
    this.webViewWidth = width;
    this.webViewHeight = height;
    this.webViewMargin = margin;
    this.fontSize = fontSize;

    this.htmlHelper.setFontSize();
    this.htmlHelper.setWidth();
    this.htmlHelper.setHeight();
    this.htmlHelper.setMargin();

    this.setUpColumns();
    this.setUpEvents();
    this.setUpEbook();
  },
  setUpEvents: function() {
    $("#columns-outer").on('click', function(e) {
      if (e.pageX > Math.round(Ebook.pageWidth / 2)) {
        Ebook.goToNextPage();
      } else {
        Ebook.goToPreviousPage();
      }
    });
  },
  setUpEbook: function() {
    this.resizeImages();

    this.pagerHelper.invalideCache();

    var endOfChapterLeft = $("#js-ebook-end-of-chapter").position().left;
    this.totalPages = Math.ceil(endOfChapterLeft / this.pageWidth);

    this.messagesHelper.sendPageChange();
  },
  setUpColumns: function() {
    this.pageWidth = $("#columns-inner").width();
    $("#columns-inner").css("column-width", this.pageWidth + "px");
    this.scrollStep = this.pageWidth + parseInt($("#columns-inner").css("column-gap"));
  },
  resize: function(width, height) {
    this.doWithSpinner(function() {
      var position = Ebook.getCurrentPosition();

      Ebook.goToPageFast(1);
      Ebook.webViewWidth = width;
      Ebook.webViewHeight = height;
      Ebook.htmlHelper.setWidth();
      Ebook.htmlHelper.setHeight();

      Ebook.setUpColumns();
      Ebook.setUpEbook();

      Ebook.goToPositionFast(position);
    });
  },
  changeFontSize: function(fontSize) {
    this.doWithSpinner(function() {
      var position = Ebook.getCurrentPosition();

      Ebook.goToPageFast(1);
      Ebook.fontSize = fontSize;
      Ebook.htmlHelper.setFontSize();

      Ebook.setUpEbook();
      Ebook.goToPositionFast(position);
    });
  },
  changeMargin: function(margin) {
    this.doWithSpinner(function() {
      var position = Ebook.getCurrentPosition();

      Ebook.goToPageFast(1);
      Ebook.webViewMargin = margin;
      Ebook.htmlHelper.setWidth();
      Ebook.htmlHelper.setHeight();
      Ebook.htmlHelper.setMargin();

      Ebook.setUpColumns();
      Ebook.setUpEbook();
      Ebook.goToPositionFast(position);
    });
  },
  doWithSpinner: function(callback) {
    this.htmlHelper.showSpinner();
    setTimeout(function() {
      callback();
    }, 10);
    setTimeout(function() {
      Ebook.htmlHelper.hideSpinner();
    }, 100);
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
      duration = 500;
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
      scrollLeft: (page - 1) * this.scrollStep,
    }, duration);
  },
  pageOfElement: function(el) {
    var left = $(el).position().left + 1;
    return Math.ceil(left / this.pageWidth);
  },
  goToPosition: function(position, duration) {
    var currentPage = Ebook.currentPage;
    this.goToPageFast(1);

    var result = this.pagerHelper.findNodeAtPosition(position, $("#content").get(0));
    if (result.node !== null) {
      this.pagerHelper.createMark(result.node, position - result.positionCounter);
      var page = this.pageOfElement($("#js-ebook-mark"));

      this.goToPageFast(currentPage);
      this.goToPage(page, duration);
    }

    this.pagerHelper.removeMark();
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
    startOfPage: function(page) {
      page = Math.min(Math.max(page, 1), Ebook.totalPages);

      if (this.cache.length === 0) {
        Ebook.pagerHelper.getLengthOfAllPages();
      }

      var start = 0;

      for (var i = 1; i < page; i++) {
        var cache = this.cache.filter(function(item) {
          return item.page === i;
        })[0];

        start += cache.length;
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
    getLengthOfAllPages: function() {
      this.markAllPages();

      var html = $("#content").html();
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
    findNodeAtPosition: function(position, currentNode, positionCounter) {
      if (positionCounter === undefined) {
        positionCounter = 0;
      }

      var result = {
        node: null,
        positionCounter: 0,
      };

      var nodeLength = this.getNodeLength(currentNode);
      var nodeLengthPlusCounter = nodeLength + positionCounter;

      if (nodeLengthPlusCounter >= position) {
        if (currentNode.nodeType === Node.ELEMENT_NODE) {
          result = this.findNodeAtPosition(position, currentNode.firstChild, positionCounter);
        } else if (currentNode.nodeType === Node.TEXT_NODE) {
          result.node = currentNode;
          result.positionCounter = positionCounter;
        }
      } else if (currentNode.nextSibling !== null) {
        result = this.findNodeAtPosition(position, currentNode.nextSibling,
          nodeLengthPlusCounter);
      }

      return result;
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
    getNodeLength: function(node) {
      var rawText = "";

      if (node.nodeType === Node.ELEMENT_NODE) {
        rawText = node.innerText;
      } else if (node.nodeType === Node.TEXT_NODE) {
        rawText = node.nodeValue;
      }

      return this.clearText(rawText).length;
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
    showSpinner: function() {
      $(".js-ebook-overlay").addClass("show");
    },
    hideSpinner: function() {
      $(".js-ebook-overlay").removeClass("show");
    },
  },
  messagesHelper: {
    sendPageChange: function() {
      Messages.send("PageChange", { CurrentPage: Ebook.currentPage,
        TotalPages: Ebook.totalPages });
    },
    nextChapterRequest: function() {
      Messages.send("NextChapterRequest", {});
    },
    prevChapterRequest: function() {
      Messages.send("PrevChapterRequest", {});
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
      Ebook.init(data.Width, data.Height, data.Margin, data.FontSize);
    },
    loadHtml: function(data) {
      $("#content").html(data.Html);

      Ebook.loadImages(data.Images);
      Ebook.setUpEbook();

      if (data.Page === 'last') {
        Ebook.goToPageFast(Ebook.totalPages);
      } else {
        Ebook.goToPageFast(1);
      }

      Ebook.messagesHelper.sendPageChange();
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