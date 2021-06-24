module Client.Tests

open Fable.Mocha

open Index
open Shared

let client = testList "Client" [
    testCase "Added todo" <| fun _ ->

        Expect.equal "" "" "TODO: fix and add client tests"
]

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all