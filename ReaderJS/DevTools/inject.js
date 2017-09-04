function csCallback(data) {
    window.parent.postMessage({
        type: "message",
        data: data,
        reader: JSON.stringify(Ebook)
    }, "*");
}

window.addEventListener('message', function (e) {
    Messages.parse(e.data);
});

window.onerror = function (msg, url, line, col, error) {

    var logMessage = "";
    if (msg.indexOf("Script error.") == -1) {
        logMessage = "line: " + line + ", col: " + col + ", url: " + url + ", error: " + error;
    }

    window.parent.postMessage({
        type: "error",
        title: msg,
        message: logMessage
    }, "*");
};