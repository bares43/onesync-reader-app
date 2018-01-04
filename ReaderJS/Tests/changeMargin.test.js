const DevToolsLibrary = require("./devtoolslibrary");

describe('When change margin', () => {
  test('should be totalPages set to correct value', async() => {
    const chromeless = DevToolsLibrary.getChromeless();

    await chromeless
      .initDevTools(300, 300, 30, 30)
      .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
      .sendChangeMarginMessage(45);

    const readerJS = await chromeless.getReaderJS();

    expect(readerJS.totalPages).toBe(13);

    await chromeless.end();
  });

  test('should be PageChange message received with correct value', async() => {
    const chromeless = DevToolsLibrary.getChromeless();

    await chromeless
      .initDevTools(300, 300, 30, 30)
      .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
      .sendChangeMarginMessage(45);

    const lastReceivedMessage = await chromeless.getLastReceivedMessage();

    expect(lastReceivedMessage.data.TotalPages).toBe(13);

    await chromeless.end();
  });

  describe('when page is 2', () => {
    test('should scroll to the same position', async() => {
      const chromeless = DevToolsLibrary.getChromeless();

      await chromeless
        .initDevTools(400, 800, 30, 15)
        .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
        .goToPageFast(2)
        .sendChangeMarginMessage(30);

      const currentContent = await chromeless.getReaderContent();

      expect(currentContent).toBe(
        'Nullam rhoncus aliquam metus.Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem.Cras elementum. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Integer tempor. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna.'
      );

      await chromeless.end();
    });
  });

  beforeAll(() => {
    DevToolsLibrary.init();
  });
});