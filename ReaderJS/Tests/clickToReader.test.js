const {
    Chromeless
} = require('chromeless')
const {
    expect
} = require('chai')

const DevToolsLibrary = require('./devtoolslibrary')


describe('When click on reader', () => {

    describe('to left', () => {
        it('should receive PrevChapterRequest message', async() => {
            const chromeless = new Chromeless();

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickLeft()

            const lastMessage = await chromeless.getLastReceivedMessage();

            expect(lastMessage.action).to.equal('PrevChapterRequest')

            await chromeless.end()
        })
    })

    describe('to right', () => {
        it('should receive NextChapterRequest message', async() => {
            const chromeless = new Chromeless();

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickRight()

            const lastMessage = await chromeless.getLastReceivedMessage();

            expect(lastMessage.action).to.equal('NextChapterRequest')

            await chromeless.end()
        })
    })

    before(() => {
        DevToolsLibrary.init()
    })

})