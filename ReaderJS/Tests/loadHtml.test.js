const {
    Chromeless
} = require("chromeless")
const {
    expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")

describe('When loadHtml', () => {

    it('should be shown correct content at first page', async() => {
        const chromeless = DevToolsLibrary.getChromeless()

        await chromeless
            .initDevTools(400, 800, 30, 45)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())

        const currentContent = await chromeless.getReaderContent()

        expect(currentContent).to.be.equal('Lorem ipsum dolor sit amet, consectetuer adipiscing elit.Vivamus porttitor turpis ac leo. Fuscetellus.LOREM IPSUMNulla turpis magna, cursus sit amet, suscipit a, interdum id, felis.Sed vel lectus. Donec odio tempus molestie,')

        await chromeless.end()
    })

    it('should be shown correct content at second page', async() => {
        const chromeless = DevToolsLibrary.getChromeless()

        await chromeless
            .initDevTools(400, 800, 30, 45)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())
            .goToPageFast(2)

        const currentContent = await chromeless.getReaderContent()

        expect(currentContent).to.be.equal('porttitor ut, iaculis quis, sem. Nullam rhoncus aliquam metus.Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem.Cras elementum. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id')

        await chromeless.end()
    })

    it('should be totalPages set to correct value', async() => {
        const chromeless = DevToolsLibrary.getChromeless()

        await chromeless
            .initDevTools(400, 800, 30, 45)
            .sendLoadHtmlMessage(DevToolsLibrary.generateLoremIpsum())

        const readerJS = await chromeless.getReaderJS()

        expect(readerJS.totalPages).to.be.equal(5)

        await chromeless.end()
    });

    before(() => {
        DevToolsLibrary.init()
    })

})