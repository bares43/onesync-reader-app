const {
  Chromeless,
} = require('chromeless');

module.exports = class DevToolsLibrary {
  static get defaultWait() {
    return 2500;
  }

  static getChromeless() {
    return new Chromeless();
  }

  static init() {
    jest.setTimeout(50000);

    Chromeless.prototype.getReceivedMessages = async function() {
      return this.evaluate(() => {
        return window.Api.receivedMessages;
      });
    };

    Chromeless.prototype.getSentMessages = async function() {
      return this.evaluate(() => {
        return window.Api.sentMessages;
      });
    };

    Chromeless.prototype.getLastReceivedMessage = async function() {
      const messages = await this.getReceivedMessages();

      return messages[messages.length - 1];
    };

    Chromeless.prototype.getLastSentMessage = async function() {
      const messages = await this.getSentMessages();

      return messages[messages.length - 1];
    };

    Chromeless.prototype.goToDevTools = function() {
      return this.goto(__dirname + '/../DevTools/index.html');
    };

    Chromeless.prototype.sendInitMessage = function(width, height, fontSize, margin, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-init";

      return this
        .wait(wrapper)
        .type(width.toString(), wrapper + " [data-name=Width]")
        .type(height.toString(), wrapper + " [data-name=Height]")
        .type(fontSize.toString(), wrapper + " [data-name=FontSize]")
        .type(margin.toString(), wrapper + " [data-name=Margin]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.sendLoadHtmlMessage = function(html, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-load-html";

      return this
        .wait(wrapper)
        .type(html, wrapper + " [data-name=Html]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.sendResizeMessage = function(width, height, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-resize";

      return this
        .wait(wrapper)
        .click("[data-toggle=collapse][href='" + wrapper + "']")
        .type(width.toString(), wrapper + " [data-name=Width]")
        .type(height.toString(), wrapper + " [data-name=Height]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.sendChangeMarginMessage = function(margin, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-change-margin";

      return this
        .wait(wrapper)
        .click("[data-toggle=collapse][href='" + wrapper + "']")
        .type(margin.toString(), wrapper + " [data-name=Margin]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.sendChangeFontSizeMessage = function(fontSize, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-change-font";

      return this
        .wait(wrapper)
        .click("[data-toggle=collapse][href='" + wrapper + "']")
        .type(fontSize.toString(), wrapper + " [data-name=FontSize]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.sendGoToStartOfPageMessage = function(page, wait = DevToolsLibrary.defaultWait) {
      const wrapper = "#collapse-message-go-to-start-of-page";

      return this
        .wait(wrapper)
        .click("[data-toggle=collapse][href='" + wrapper + "']")
        .type(page.toString(), wrapper + " [data-name=Page]")
        .click(wrapper + " button")
        .wait(wait);
    };

    Chromeless.prototype.initDevTools = function(width, height, fontSize, margin, wait = DevToolsLibrary.defaultWait) {
      return this
        .goToDevTools()
        .sendInitMessage(width, height, fontSize, margin)
        .wait(wait);
    };

    Chromeless.prototype.clickLeft = function(wait = DevToolsLibrary.defaultWait + 400) {
      return this
        .evaluate(() => {
          const iframe = document.getElementById('reader');
          iframe.contentWindow.postMessage({
            type: 'clickToLeft',
          }, '*');
        })
        .wait(wait);
    };

    Chromeless.prototype.clickRight = async function(wait = DevToolsLibrary.defaultWait + 400) {
      return this
        .evaluate(() => {
          const iframe = document.getElementById('reader');
          iframe.contentWindow.postMessage({
            type: 'clickToRight',
          }, '*');
        })
        .wait(wait);
    };

    Chromeless.prototype.getReaderJS = async function() {
      return this
        .evaluate(() => {
          return window.Api.readerJS;
        });
    };

    Chromeless.prototype.getReaderContent = async function() {
      let chain = this
        .evaluate(() => {
          window.Api.currentContentRequest();
        });

      await this.wait(3000);

      var content = await chain.evaluate(() => {
        return window.Api.currentContent;
      });

      return content;
    };

    Chromeless.prototype.goToPageFast = function(page, wait = DevToolsLibrary.defaultWait) {
      return this
        .evaluate((page) => {
          window.Api.sendGoToPageFastMessage(page);
        }, page)
        .wait(wait);
    };
  }

  static generateLoremIpsum() {
    return '<p>Lorem ipsum dolor <strong>sit amet</strong>, consectetuer adipiscing elit.</p><p>Vivamus porttitor turpis ac leo. Fusce<br />tellus.</p><h1>LOREM IPSUM</h1><p>Nulla turpis magna, cursus sit amet, suscipit a, interdum id, felis.</p><p>Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem. Nullam rhoncus aliquam metus.</p><p>Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem.</p><p>Cras elementum. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Integer tempor. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna. Vestibulum erat nulla, ullamcorper nec, rutrum non, nonummy ac, erat. Nam quis nulla. Nullam justo enim, consectetuer nec, ullamcorper ac, vestibulum in, elit.</p>';
  }
};