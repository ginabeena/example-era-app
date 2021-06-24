module Server.Tests

open Expecto

open Shared
open Server

let rand = System.Random()

let server = testList "Server" [
    testCase "Adding valid report" <| fun _ ->
        let rej = int64 <| rand.Next()
        let paid = int64 <| rand.Next()
        let total = rej + paid
        let days = int64 <| rand.Next()
        let reporter = EvictionsReporting.randomReporter()

        let storage = Storage()
        let required_info = EvictionsReporting.seed(reporter, total, paid, rej, days)
        let snap = EvictionsSnapshot(required_info)
        let result = storage.SubmitReport snap
        let expected = Ok <| snap

        Expect.equal result expected "Storage should be ok"
        Expect.contains (storage.ShowReports()) snap "And you should see what you store"
]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all