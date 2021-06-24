module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Shared

type Storage () =
    let mutable reporter = EvictionsReporting.randomReporter()
    member __.reports = ResizeArray<EvictionsSnapshot>()

    member __.ShowReports() =
        List.ofSeq __.reports

    member __.SubmitReport(e:EvictionsSnapshot) =
        __.reports.Add(e)
        Ok (e)

    member __.SetReporter
        with get () = reporter
        and set (updated) = reporter <- updated


let storage = Storage()

let reportsApi =
    { showReports = fun () -> async { return storage.ShowReports() }
      submitReport =
        fun (required) -> async {
            let snap = EvictionsSnapshot(required)
            let submission = storage.SubmitReport(snap)
            match submission with
            | Ok r -> return r
            | Error e -> return failwith e
        }}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue reportsApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
