function csCallback(data) {
    window.parent.postMessage({
        type: "message",
        data: data,
        reader: JSON.stringify(Ebook)
    }, "*");
}

window.addEventListener('message', function (e) {

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
    }

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

function clickToLeft(){
    var event = $.Event('click');
    event.pageX = 1;
    $('#columns-outer').trigger(event);
}

function clickToRight(){
    var event = $.Event('click');
    event.pageX = $("#columns-outer").width() - 1;
    $('#columns-outer').trigger(event);
}