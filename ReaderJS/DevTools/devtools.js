var iframe = document.getElementById('reader');
var logCnt = 0;

function sendMessage(action, data) {
    addSendLog(action, JSON.stringify(data));

    var msg = {
        Action: action,
        Data: data
    };

    window.Api.sentMessages.push(msg);

    var json = JSON.stringify(msg);

    iframe.contentWindow.postMessage({
        type: "message",
        data: Base64.encode(json)
    }, "*");
}

function clearLogs() {
    logCnt = 0;
    $("#logs").html("");
}

function addLog(message, type, detail) {
    ++logCnt;

    var log = $("<div></div>").addClass("alert").addClass("alert-" + type);

    log.text(logCnt + ": " + message);

    if (detail) {
        var detailBtn = $("<span></span>").text("Detail").attr("data-toggle", "collapse").attr("href", "#log-" + logCnt).css("display", "block");
        var detail = $("<div></div>").addClass("collapse").attr("id", "log-" + logCnt).html(detail);

        log.append(detailBtn);
        log.append(detail);
    }

    $("#logs").prepend(log);
}

function addErrorLog(message, detail) {
    addLog(message, "danger", detail);
}

function addSendLog(message, detail) {
    addLog(message, "secondary", detail);
}

function addReceiveLog(message, detail) {
    addLog(message, "primary", detail);
}

window.addEventListener('message', function (e) {

    switch (e.data.type) {
        case "error":
            addErrorLog(e.data.title, e.data.message);
            break;
        case "message":
            var msg = JSON.parse(Base64.decode(e.data.data));
            window.Api.receivedMessages.push(msg);
            addReceiveLog(msg.action, JSON.stringify(msg.data));
            $("#ebookjs").html(e.data.reader);
            window.Api.readerJS = JSON.parse(e.data.reader);
            break;
        case "currentContent":
            window.Api.currentContent = e.data.content;
            break;
    }
});

function beforeMessageSend(action, data) {
    if (action == "init" || action == "resize") {
        var $iframe = $(iframe);
        $iframe.show();
        $iframe.width(data.Width);
        $iframe.height(data.Height);
    } else if (action == "loadHtml") {
        data["Images"] = [];
    }

    return data;
}

$(".message-form").on("submit", function (e) {
    e.preventDefault();
    e.stopImmediatePropagation();

    var form = $(this);
    var data = {};

    form.find("[data-name]").each(function (i, v) {
        var name = $(v).attr("data-name");
        var value = $(v).val();

        var type = $(v).attr("data-type");
        if (type == "int") {
            value = parseInt(value);
        }

        data[name] = value;
    });

    var action = form.find("[data-action]").attr("data-action");

    data = beforeMessageSend(action, data);

    sendMessage(action, data);

    form.find("input, textarea").val("");
    form.find(".collapse").collapse("hide");

    return false;
});

$("#clear-logs").on("click", function () {
    clearLogs();
})

window.Api = {
    receivedMessages: [],
    sentMessages: [],
    readerJS: null,
    currentContent: "",
    currentContentRequest: function () {
        iframe.contentWindow.postMessage({
            type: "currentContentRequest",
        }, "*");
    },
    sendGoToPageFastMessage: function (page) {
        iframe.contentWindow.postMessage({
            type: "goToPageFast",
            page: page,
        }, "*");
    },
}