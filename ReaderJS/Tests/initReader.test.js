const {
  Chromeless
} = require("chromeless")
const {
  expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")

describe('When init reader', () => {

  it('should receive PageChange message', async() => {
    const chromeless = new Chromeless()

    await chromeless
      .initDevTools(400, 800, 30, 45)

    const lastSentMessage = await chromeless.getLastSentMessage()

    const lastMessage = await chromeless.getLastReceivedMessage()

    expect(lastMessage.action).to.equal('PageChange')

    const readrJS = await chromeless.getReaderJS()

    await chromeless.end()
  })

  before(() => {
    DevToolsLibrary.init()
  })

})