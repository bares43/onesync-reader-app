const {
	expect,
} = require("chai");

const DevToolsLibrary = require("./devtoolslibrary");

describe('When go to start of page', () => {
	it('should be currentPage set to correct value', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45)
			.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
			.sendGoToStartOfPageMessage(2);

		const readerJS = await chromeless.getReaderJS();

		expect(readerJS.currentPage).to.be.equal(2);

		await chromeless.end();
	});
	
	it('should be PageChange message received with correct value', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45)
			.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
			.sendGoToStartOfPageMessage(2);

		const lastReceivedMessage = await chromeless.getLastReceivedMessage();

		expect(lastReceivedMessage.data.CurrentPage).to.be.equal(2);

		await chromeless.end();
	});

	it('should be shown correct content at second page', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45)
			.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
			.sendGoToStartOfPageMessage(2);

		const currentContent = await chromeless.getReaderContent();

		expect(currentContent).to.be.equal('porttitor ut, iaculis quis, sem. Nullam rhoncus aliquam metus.Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem.Cras elementum. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id');

		await chromeless.end();
	});

	before(() => {
		DevToolsLibrary.init();
	});
});