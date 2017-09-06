const {
  Chromeless
} = require("chromeless")
const {
  expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")

describe("When init reader", function () {

  it("should receive page change message", async function () {
    this.timeout(5000)
    const chromeless = new Chromeless()

    await chromeless
      .initDevTools(400, 800, 30, 45)

    const lastSentMessage = await chromeless.getLastSentMessage()

    const lastMessage = await chromeless.getLastReceivedMessage()

    expect(lastMessage.action).to.equal("PageChange")

    const readrJS = await chromeless.getReaderJS()

    await chromeless.end()
  })

  before(function () {
    DevToolsLibrary.init()
  })

})