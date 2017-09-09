const {
    Chromeless
} = require("chromeless")
const {
    expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")

describe('When resize', () => {

    it('should be totalPages set to correct value', async() => {
        const chromeless = new Chromeless()

        await chromeless
            .initDevTools(400, 800, 30, 45)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
            .sendResizeMessage(300, 300)

        const readerJS = await chromeless.getReaderJS()

        expect(readerJS.totalPages).to.be.equal(25)

        await chromeless.end()
    })

    it('should be PageChange message received with correct value', async() => {
        const chromeless = new Chromeless()

        await chromeless
            .initDevTools(400, 800, 30, 45)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
            .sendResizeMessage(300, 300)

        const lastReceivedMessage = await chromeless.getLastReceivedMessage()

        expect(lastReceivedMessage.data.TotalPages).to.be.equal(25)

        await chromeless.end()
    })

    describe('when page is 2', () => {
        it('should scroll to the same position', async() => {
            const chromeless = new Chromeless()

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
                .goToPageFast(2)
                .sendResizeMessage(300, 300)

            const currentContent = await chromeless.getReaderContent()

            expect(currentContent).to.be.equal('in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum')

            await chromeless.end()
        });
    });

    before(() => {
        DevToolsLibrary.init()
    })

})