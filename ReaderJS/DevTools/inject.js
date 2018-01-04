/*global Ebook*/
/*global Messages*/
/*global Simulator*/
/* exported csCallback */

function clickToLeft() {
  Simulator.gestures.tap(document.getElementsByTagName('body')[0], {
    pos: [1, 1],
  });
}

function clickToRight() {
  Simulator.gestures.tap(document.getElementsByTagName('body')[0], {
    pos: [$("body").width() - 1, 1],
  });
}

function getCurrentContent() {
  var result = "";

  var rect = {
    top: Ebook.webViewMargin,
    left: Ebook.webViewMargin,
    width: Ebook.webViewWidth - (2 * Ebook.webViewMargin),
    height: Ebook.webViewHeight - (2 * Ebook.webViewMargin),
  };

  if (document.caretRangeFromPoint) {
    var caretRangeStart = document.caretRangeFromPoint(rect.left, rect.top);
    var caretRangeEnd = document.caretRangeFromPoint(rect.left + rect.width - 1, rect.top + rect.height - 1);

    if (caretRangeStart === null || caretRangeEnd === null) {
      return null;
    }

    var range = document.createRange();
    range.setStart(caretRangeStart.startContainer, caretRangeStart.startOffset);
    range.setEnd(caretRangeEnd.endContainer, caretRangeEnd.endOffset);

    result = range.toString();

    return result;
  }
  return null;
}

function csCallback(data) {
  window.parent.postMessage({
    type: "message",
    data: data,
    reader: JSON.stringify(Ebook),
    currentContent: getCurrentContent(),
  }, "*");
}

function handleCurrentContentRequest() {
  setTimeout(function() {
    var content = getCurrentContent();

    window.parent.postMessage({
      type: "currentContent",
      content: content,
    }, "*");
  }, 600);
}

window.addEventListener('message', function(e) {
  switch (e.data.type) {
  case "message":
    Messages.parse(e.data.data);
    break;
  case "clickToLeft":
    clickToLeft();
    break;
  case "clickToRight":
    clickToRight();
    break;
  case "currentContentRequest":
    handleCurrentContentRequest();
    break;
  case "goToPageFast":
    Ebook.goToPageFast(e.data.page);
    break;
  }
});

window.onerror = function(msg, url, line, col, error) {
  var logMessage = "";
  if (msg.indexOf("Script error.") === -1) {
    logMessage = "line: " + line + ", col: " + col + ", url: " + url + ", error: " + error;
  }

  window.parent.postMessage({
    type: "error",
    title: msg,
    message: logMessage,
  }, "*");
};