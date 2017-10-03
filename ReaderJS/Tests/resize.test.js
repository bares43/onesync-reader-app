const DevToolsLibrary = require("./devtoolslibrary");

describe('When resize', () => {
	test('should be totalPages set to correct value', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45)
			.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
			.sendResizeMessage(300, 300);

		const readerJS = await chromeless.getReaderJS();

		expect(readerJS.totalPages).toBe(13);

		await chromeless.end();
	});

	test('should be PageChange message received with correct value', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45)
			.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
			.sendResizeMessage(300, 300);

		const lastReceivedMessage = await chromeless.getLastReceivedMessage();

		expect(lastReceivedMessage.data.TotalPages).toBe(13);

		await chromeless.end();
	});

	describe('when page is 2', () => {
		test('should scroll to the same position', async() => {
			const chromeless = DevToolsLibrary.getChromeless();

			await chromeless
				.initDevTools(400, 800, 30, 45)
				.sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
				.goToPageFast(2)
				.sendResizeMessage(300, 300);

			const currentContent = await chromeless.getReaderContent();

			expect(currentContent).toBe(
				'in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum'
			);

			await chromeless.end();
		});
	});

	beforeAll(() => {
		DevToolsLibrary.init();
	});
});