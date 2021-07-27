namespace Shared

open System
open Thoth.Json

[<Struct>]
type Reporter =
    { CensusBlockGroups: string list
      Zipcode: string
      CountyName: string
      CityName: string
      Name: string
      Id: string }

[<Struct>]
type Report =
    { SubmittedOn: string
      Reporter: Reporter
      QueueTotal: int64
      PaidCount: int64
      TimespanDays: int64
      RejectedCount: int64
      Version: string
      Id: string }

module EvictionsReporting =

    let identifier () = Guid.NewGuid().ToString()

    let randomReporter () =
        { CensusBlockGroups =
              [ identifier ()
                identifier ()
                identifier () ]
          Zipcode = "00000"
          CountyName = "Randomtown"
          CityName = "Randomville"
          Name = "Jane Doe"
          Id = identifier () }

    type RequiredInfo =
        { Reporter: Reporter
          QueueTotal: int64
          PaidCount: int64
          RejectedCount: int64
          TimespanDays: int64 }

    let seed (reporter, queueTotal, paidCount, rejectedCount, timespanDays) =
        { Reporter = reporter
          QueueTotal = queueTotal
          PaidCount = paidCount
          RejectedCount = rejectedCount
          TimespanDays = timespanDays }

    let isValid s =
        try
            let _ = int64 s
            true
        with
        | _ -> false

type EvictionsSnapshot(required: EvictionsReporting.RequiredInfo) =

    member __.Version = "Alpha.01"
    member __.Id = EvictionsReporting.identifier ()
    member __.SubmittedOn = DateTime.Today.ToString()
    member __.QueueTotal = required.QueueTotal
    member __.PaidCount = required.PaidCount
    member __.RejectedCount = required.RejectedCount
    member __.TimespanDays = required.TimespanDays
    member __.Reporter = required.Reporter

    member __.Average() =
        ((__.RejectedCount + __.PaidCount)
         / int64 __.TimespanDays)

    member __.P99WaitTimeDays() =
        (float (
            (__.RejectedCount + __.PaidCount)
            / __.TimespanDays
        ))
        * 0.99

    member __.Report =
        { Reporter = __.Reporter
          QueueTotal = __.QueueTotal
          PaidCount = __.PaidCount
          TimespanDays = __.TimespanDays
          RejectedCount = __.RejectedCount
          SubmittedOn = __.SubmittedOn
          Version = __.Version
          Id = __.Id }

    member __.ToJson() : JsonValue =
        Encode.object [ "version", Encode.string __.Version
                        "id", Encode.string __.Id
                        "submitted_on", Encode.string __.SubmittedOn
                        "queue_total", Encode.int64 __.QueueTotal
                        "paid_count", Encode.int64 __.PaidCount
                        "rejected_count", Encode.int64 __.RejectedCount
                        "timespan_days", Encode.int64 __.TimespanDays
                        "reporter",
                        Encode.object [ "census_block_groups",
                                        Encode.list (List.map Encode.string __.Reporter.CensusBlockGroups)
                                        "zipcode", Encode.string __.Reporter.Zipcode
                                        "county_name", Encode.string __.Reporter.CountyName
                                        "city_name", Encode.string __.Reporter.CityName
                                        "name", Encode.string __.Reporter.Name
                                        "id", Encode.string __.Reporter.Id ] ]

    static member FromJson(json: JsonValue) : Decoder<EvictionsSnapshot> =
        let requiredInfo (get: Decode.IGetters) : EvictionsReporting.RequiredInfo =
            { Reporter =
                  { Id = get.Required.Field "id" Decode.string
                    Name = get.Required.Field "name" Decode.string
                    CityName = get.Required.Field "city_name" Decode.string
                    CountyName = get.Required.Field "county_name" Decode.string
                    CensusBlockGroups =
                        get.Required.Field "census_block_groups"
                        <| Decode.list Decode.string
                    Zipcode = get.Required.Field "zipcode" Decode.string }
              QueueTotal = get.Required.Field "queue_total" Decode.int64
              PaidCount = get.Required.Field "paid_count" Decode.int64
              RejectedCount = get.Required.Field "queue_total" Decode.int64
              TimespanDays = get.Required.Field "timespan_days" Decode.int64 }

        Decode.object (requiredInfo >> EvictionsSnapshot)

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type IReportsApi =
    { ShowReports: unit -> Async<EvictionsSnapshot list>
      SubmitReport: EvictionsReporting.RequiredInfo -> Async<EvictionsSnapshot> }
