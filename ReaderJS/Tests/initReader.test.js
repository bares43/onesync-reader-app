const {
  Chromeless
} = require("chromeless")

const DevToolsLibrary = require("./devtoolslibrary")

describe('When init reader', () => {

  test('should receive PageChange message', async() => {
    const chromeless = DevToolsLibrary.getChromeless()

    await chromeless
      .initDevTools(400, 800, 30, 45)

    const lastSentMessage = await chromeless.getLastSentMessage()

    const lastMessage = await chromeless.getLastReceivedMessage()

    expect(lastMessage.action).toBe('PageChange')

    const readrJS = await chromeless.getReaderJS()

    await chromeless.end()
  })

  beforeAll(() => {
    DevToolsLibrary.init()
  })

})