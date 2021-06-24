module Index

open Elmish
open Fable.Remoting.Client
open Shared

type Model =
    {
      UserPrompt: string
      Reports: EvictionsSnapshot list
      Reporter: reporter
      TotalQueueLength: string
      TotalPaidCount: string
      TotalRejectedCount: string
      TimeSpanInDays: string }

type ReportView =
    | GotReports of EvictionsSnapshot list
    | SetReporter of reporter
    | AddReport
    | TimeSpanInDaysSet of string
    | TotalQueueLengthSet of string
    | TotalPaidCountSet of string
    | TotalRejectedCountSet of string
    | SubmittedReport of EvictionsSnapshot
    | UserPromptSet of string

let reportsApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IReportsApi>

let init(): Model * Cmd<ReportView> =
    // Just for the demo
    let reporter = EvictionsReporting.randomReporter()
    let model =
        {
          Reporter = reporter
          UserPrompt = sprintf "Hi %s, please submit your report!" reporter.name
          Reports = []
          TotalQueueLength = ""
          TotalPaidCount = ""
          TotalRejectedCount = ""
          TimeSpanInDays = "" }
    let cmd = Cmd.OfAsync.perform reportsApi.showReports () GotReports
    model, cmd

let update (typing: ReportView) (model: Model): Model * Cmd<ReportView> =
    match typing with
    | GotReports reports ->
        { model with Reports = reports }, Cmd.none
    | TotalQueueLengthSet value ->
        { model with TotalQueueLength = value }, Cmd.none
    | TotalPaidCountSet value ->
        { model with TotalPaidCount = value }, Cmd.none
    | TotalRejectedCountSet value ->
        { model with TotalRejectedCount = value }, Cmd.none
    | TimeSpanInDaysSet value ->
        { model with TimeSpanInDays = value }, Cmd.none
    | SetReporter value ->
        { model with Reporter = value }, Cmd.none
    | UserPromptSet value ->
        { model with UserPrompt = value }, Cmd.none
    | AddReport ->
        let ql = int64 model.TotalQueueLength
        let pc = int64 model.TotalPaidCount
        let rc = int64 model.TotalRejectedCount
        let d = int64 model.TimeSpanInDays
        let reqs = EvictionsReporting.seed(model.Reporter,ql,pc,rc,d)
        let cmd = Cmd.OfAsync.perform reportsApi.submitReport reqs SubmittedReport
        { model with
            TotalQueueLength = ""
            TotalRejectedCount = ""
            TotalPaidCount = ""
            TimeSpanInDays = "" }, cmd
    | SubmittedReport value ->
        { model with
            Reports = [value]
            UserPrompt = sprintf "Successfully submitted report %s on %s" value.id value.submitted_on
            }, Cmd.none

let isReady (m:Model) =
    try
        let _ = int64 m.TotalQueueLength
        let _ = int64 m.TotalRejectedCount
        let _ = int64 m.TotalPaidCount
        let _ = int64 m.TimeSpanInDays
        true
    with _ -> false

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://home.treasury.gov/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Seal for the U.S. Department of the Treasury"
            ]
        ]
    ]

let containerBox (model : Model) (dispatch : ReportView -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Content.Ol.ol [ ] [
                for report in model.Reports do
                    li [ ] [ str report.submitted_on ]
                    li [ ] [ str report.reporter ]
                    li [ ] [ str (string report.queue_total) ]
                    li [ ] [ str (string report.paid_count) ]
                    li [ ] [ str (string report.timespan_days) ]
                    li [ ] [ str (string report.rejected_count) ]
                    li [ ] [ str (string (report.average())) ]
                    li [ ] [ str (string (report.p99_waittime_days())) ]
            ]
        ]

        Field.div [ Field.IsGrouped ] [
            h2 [] [ str model.UserPrompt ]
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.TotalQueueLength
                  Input.Placeholder "How many total rental assistance applications?"
                  Input.OnChange (fun x -> TotalQueueLengthSet x.Value |> dispatch) ]
            ]
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.TotalPaidCount
                  Input.Placeholder "How many paid-out rental assistance applications?"
                  Input.OnChange (fun x -> TotalPaidCountSet x.Value |> dispatch) ]

            ]
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.TotalRejectedCount
                  Input.Placeholder "How many rejected rental assistance applications?"
                  Input.OnChange (fun x -> TotalRejectedCountSet x.Value |> dispatch) ]
            ]
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.TimeSpanInDays
                  Input.Placeholder "In what timespan was this data collected? (in days)"
                  Input.OnChange (fun x -> TimeSpanInDaysSet x.Value |> dispatch) ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.Disabled (isReady model |> not)
                    Button.OnClick (fun _ -> dispatch AddReport)
                ] [
                    str "Submit Reporting"
                ]
            ]
        ]
    ]

let view (model : Model) (dispatch : ReportView -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "Emergency Rental Assistance Reporting" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
