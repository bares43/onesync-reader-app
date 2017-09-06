const {
    Chromeless
} = require("chromeless")
const {
    expect
} = require("chai")

const DevToolsLibrary = require("./devtoolslibrary")


describe("When click to reader", function () {

    describe("click left", function () {
        it("should send PrevChapterRequest message", async function () {
            this.timeout(5000);
            const chromeless = new Chromeless();

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickLeft()

            const lastMessage = await chromeless.getLastReceivedMessage();

            expect(lastMessage.action).to.equal("PrevChapterRequest")

            await chromeless.end()
        })
    })

    describe("click right", function () {
        it("should send NextChapterRequest message", async function () {
            this.timeout(5000);
            const chromeless = new Chromeless();

            await chromeless
                .initDevTools(400, 800, 30, 45)
                .clickRight()

            const lastMessage = await chromeless.getLastReceivedMessage();

            expect(lastMessage.action).to.equal("NextChapterRequest")

            await chromeless.end()
        })
    })

    before(function () {
        DevToolsLibrary.init()
    })

})