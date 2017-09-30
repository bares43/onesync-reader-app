const {
    Chromeless
} = require('chromeless')

const DevToolsLibrary = require('./devtoolslibrary')

describe('When click on reader', () => {

    describe('to left', () => {
        test('should receive PrevChapterRequest message', async() => {
            const chromeless = DevToolsLibrary.getChromeless()

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickLeft()

            const lastMessage = await chromeless.getLastReceivedMessage()

            expect(lastMessage.action).toBe('PrevChapterRequest')

            await chromeless.end()
        })
    })

    describe('to right', () => {
        test('should receive NextChapterRequest message', async() => {
            const chromeless = DevToolsLibrary.getChromeless()

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickRight()

            const lastMessage = await chromeless.getLastReceivedMessage()

            expect(lastMessage.action).toBe('NextChapterRequest')

            await chromeless.end()
        })
    })

    beforeAll(() => {
        DevToolsLibrary.init()
    })

})