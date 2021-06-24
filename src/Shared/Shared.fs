namespace Shared

open System
open Thoth.Json

[<Struct>]
type report = {
  submitted_on: string
  reporter: string
  queue_total: int64
  paid_count: int64
  timespan_days: int64
  rejected_count: int64
  version: string
  id: string
}

[<Struct>]
type reporter = {
  census_block_groups: string list;
  zipcode: string;
  county_name: string;
  city_name: string;
  name: string;
  id: string;
}

module EvictionsReporting =

    let identifier () = Guid.NewGuid().ToString()

    let randomReporter () = {
        census_block_groups = [identifier(); identifier(); identifier()];
        zipcode = "00000";
        county_name = "Randomtown";
        city_name = "Randomville";
        name = "Jane Doe";
        id = identifier();
    }

    type required_info = {
        reporter:reporter
        queue_total:int64
        paid_count:int64
        rejected_count:int64
        timespan_days:int64
    }

    let seed (reporter,queue_total,paid_count,rejected_count,timespan_days) = {
            reporter=reporter
            queue_total=queue_total
            paid_count=paid_count
            rejected_count=rejected_count
            timespan_days=timespan_days
        }

    let isValid s =
        try
            let _ = int64 s
            true
        with _ -> false

type EvictionsSnapshot(required:EvictionsReporting.required_info) =

    member __.version = "Alpha.01"
    member __.id = EvictionsReporting.identifier()
    member __.submitted_on = DateTime.Today.ToString()
    member __.queue_total = required.queue_total;
    member __.paid_count = required.paid_count;
    member __.rejected_count = required.rejected_count;
    member __.timespan_days =  required.timespan_days;
    member __.reporter = required.reporter.id;
    member __.average() =
        ((__.rejected_count + __.paid_count) / int64 __.timespan_days)
    member __.p99_waittime_days() =
        (float ((__.rejected_count + __.paid_count)/__.timespan_days)) * 0.99
    member __.report = {
      reporter = __.reporter;
      queue_total = __.queue_total;
      paid_count = __.paid_count;
      timespan_days = __.timespan_days;
      rejected_count = __.rejected_count;
      submitted_on = __.submitted_on;
      version = __.version;
      id = __.id;
    }
    member __.ToJson() : JsonValue =
        Encode.object
            [
              "version", Encode.string __.version
              "id", Encode.string __.id
              "submitted_on", Encode.string __.submitted_on
              "queue_total", Encode.int64 __.queue_total
              "paid_count", Encode.int64 __.paid_count
              "rejected_count", Encode.int64 __.rejected_count
              "timespan_days", Encode.int64 __.timespan_days
              "reporter", Encode.object
                            [ "census_block_groups", Encode.array (List.map Encode.string __.reporter.census_block_groups)
                              "zipcode", Encode.string __.reporter.zipcode
                              "county_name", Encode.string __.reporter.county_name
                              "city_name", Encode.string __.reporter.city_name
                              "name", Encode.string __.reporter.name
                              "id", Encode.string __.reporter.id
                            ]
            ]
    static member FromJson (json:JsonValue) : Decoder<EvictionsSnapshot> =
        Decode.object
            (fun get ->
                {
                    version = get.Required.Field "version" Decode.string
                    id = get.Required.Field "id" Decode.string
                    submitted_on = get.Required.Field "submitted_on" Decode.string
                    queue_total = get.Required.Field "queue_total" Decode.int64
                    paid_count = get.Required.Field "paid_count" Decode.int64
                    rejected_count = get.Required.Field "queue_total" Decode.int64
                    reporter = {
                        id = get.Required.Field "id" Decode.string
                        name = get.Required.Field  "name" Decode.string
                        city_name = get.Required.Field "city_name" Decode.string
                        county_name = get.Required.Field "county_name" Decode.string
                        census_block_groups = get.Required.Field "census_block_groups" Decode.array |> List.map Decode.string }
                }
            )



module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IReportsApi =
    { showReports : unit -> Async<EvictionsSnapshot list>
      submitReport : EvictionsReporting.required_info -> Async<EvictionsSnapshot> }
