const {
    Chromeless
} = require("chromeless")
const {
    expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")

describe('When change font size', () => {

    it('should be totalPages set to correct value', async() => {
        const chromeless = new Chromeless()

        await chromeless
            .initDevTools(300, 300, 30, 30)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
            .sendChangeFontSizeMessage(40)

        const readerJS = await chromeless.getReaderJS()

        expect(readerJS.totalPages).to.be.equal(36)

        await chromeless.end()
    })

    it('should be PageChange message received with correct value', async() => {
        const chromeless = new Chromeless()

        await chromeless
            .initDevTools(300, 300, 30, 30)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
            .sendChangeFontSizeMessage(40)

        const lastReceivedMessage = await chromeless.getLastReceivedMessage()

        expect(lastReceivedMessage.data.TotalPages).to.be.equal(36)

        await chromeless.end()
    })

    describe('when page is 2', () => {
        it('should scroll to the same position', async() => {
            const chromeless = new Chromeless()

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
                .goToPageFast(2)
                .sendChangeFontSizeMessage(40)

            const currentContent = await chromeless.getReaderContent()

            expect(currentContent).to.be.equal('reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem.Cras elementum.')

            await chromeless.end()
        });
    });

    before(() => {
        DevToolsLibrary.init()
    })

})