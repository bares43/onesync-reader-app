const {
	expect,
} = require("chai");

const DevToolsLibrary = require("./devtoolslibrary");

describe('When init reader', () => {
	it('should receive PageChange message', async() => {
		const chromeless = DevToolsLibrary.getChromeless();

		await chromeless
			.initDevTools(400, 800, 30, 45);
			
		const lastMessage = await chromeless.getLastReceivedMessage();

		expect(lastMessage.action).to.equal('PageChange');

		await chromeless.end();
	});

	before(() => {
		DevToolsLibrary.init();
	});
});