const DevToolsLibrary = require("./devtoolslibrary");

describe('When init reader', () => {
	test('should receive PageChange message', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45);

		const lastMessage = await chromeless.getLastReceivedMessage();

		expect(lastMessage.action).toBe('PageChange');

		await chromeless.end();
	});

	beforeAll(() => {
		DevToolsLibrary.init();
	});
});