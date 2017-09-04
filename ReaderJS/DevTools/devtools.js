var iframe = document.getElementById('reader');
var logCnt = 0;

function sendMessage(action, data) {
    var json = JSON.stringify({
        Action: action,
        Data: data
    });

    iframe.contentWindow.postMessage(Base64.encode(json), "*");

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
            addReceiveLog(msg.action, JSON.stringify(msg.data));
            $("#ebookjs").html(e.data.reader)
            break;
    }
});

function beforeMessageSend(action, data) {
    if (action == "init" || action == "resize") {
        var $iframe = $(iframe);
        $iframe.show();
        $iframe.width(data.Width);
        $iframe.height(data.Height);
    }else if (action == "loadHtml") {
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

    addSendLog(action, JSON.stringify(data));
    sendMessage(action, data)

    form.find("input, textarea").val("");
    form.find(".collapse").collapse("hide");

    return false;
});

$("#clear-logs").on("click", function () {
    clearLogs();
})