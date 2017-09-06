const {
    Chromeless
} = require("chromeless")

'use strict';

module.exports = class DevToolsLibrary {

    static init() {
        Chromeless.prototype.getReceivedMessages = async function () {
            return await this.evaluate(() => {
                return window.Api.receivedMessages
            })
        }

        Chromeless.prototype.getSentMessages = async function () {
            return await this.evaluate(() => {
                return window.Api.sentMessages
            })
        }

        Chromeless.prototype.getLastReceivedMessage = async function () {
            const messages = await this.getReceivedMessages();

            return messages[messages.length - 1]
        }

        Chromeless.prototype.getLastSentMessage = async function () {
            const messages = await this.getSentMessages();
            
            return messages[messages.length - 1]
        }

        Chromeless.prototype.goToDevTools = function () {
            return this.goto(__dirname + "/../DevTools/index.html")
        }

        Chromeless.prototype.sendInitMessage = function (width, height, fontSize, margin, wait = 500) {
            const wrapper = "#collapse-message-init"

            let chain = this
                .wait(wrapper)
                .type(width.toString(), wrapper + " [data-name=Width]")
                .type(height.toString(), wrapper + " [data-name=Height]")
                .type(fontSize.toString(), wrapper + " [data-name=FontSize]")
                .type(margin.toString(), wrapper + " [data-name=Margin]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.sendLoadHtmlMessage = function (html, wait = 500) {
            const wrapper = "#collapse-message-load-html"

            let chain = this
                .wait(wrapper)
                .type(html, wrapper + " [data-name=Html]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.sendResizeMessage = function (width, height, wait = 500) {
            const wrapepr = "#collapse-message-resize"

            let chain = this
                .wait(wrapepr)
                .type(width.toString(), wrapper + " [data-name=Width]")
                .type(height.toString(), wrapper + " [data-name=Height]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.sendChangeMarginMessage = function (margin, wait = 500) {
            const wrapper = "#collapse-message-change-margin"

            let chain = this
                .wait(wrapper)
                .type(margin.toString(), wrapper + " [data-name=Margin]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(500)
            }

            return chain
        }

        Chromeless.prototype.sendChangeFontSizeMessage = function (fontSize, wait = 500) {
            const wrapper = "#collapse-message-change-font"

            let chain = this
                .wait(wrapper)
                .type(fontSize.toString(), wrapper + " [data-name=FontSize]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.sendGoToStartOfPageMessage = function (page, wait = 500) {
            const wrapper = "#collapse-message-go-to-start-of-page"

            let chain = this
                .wait(wrapper)
                .type(page.toString(), wrapper + " [data-name=Page]")
                .click(wrapper + " button")

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.initDevTools = function (width, height, fontSize, margin, wait = 500) {
            let chain = this
                .goToDevTools()
                .sendInitMessage(width, height, fontSize, margin)

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.clickLeft = function (wait = 500) {
            let chain = this
                .evaluate(() => {
                    const iframe = document.getElementById('reader')
                    iframe.contentWindow.postMessage({
                        type: "clickToLeft"
                    }, "*")
                })

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.clickRight = function (wait = 500) {
            let chain = this
                .evaluate(() => {
                    const iframe = document.getElementById('reader')
                    iframe.contentWindow.postMessage({
                        type: "clickToRight"
                    }, "*")
                })

            if (wait > 0) {
                chain = chain.wait(wait)
            }

            return chain
        }

        Chromeless.prototype.getReaderJS = async function () {
            return await this
                .evaluate(() => {
                    return window.Api.readerJS
                })
        }
    }
}