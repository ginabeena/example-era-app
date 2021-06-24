module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

let shared = testList "Shared" [
    testCase "TODO: data validation" <| (fun _ ->
        let expected = ""
        let actual = ""
        Expect.equal actual expected "Should be valid")
]