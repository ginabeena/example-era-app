# Example ERA Prototype App

## Version 1: POST /submitReport Request:

```json
{
   "queue_total":100,
   "paid_count":30,
   "timespan_days":30,
   "rejected_count":10,
   "reporter":{
      "census_block_groups":[
         "01-234-567890-1"
      ],
      "zipcode":"32168",
      "county_name":"Volusia County",
      "city_name":"New Smyrna Beach",
      "name":"Gina Maini",
      "id":"3bf8788f-6872-43be-b2f7-cb46a0ac8aeb"
   }
}  
```

* See also: [census block maps](https://www.census.gov/geographies/reference-maps/2010/geo/2010-census-block-maps.html)

## Version 1: /submitReport Response:

```json
{
   "submitted_on":"6/24/2021 12:00:00 AM",
   "reporter":{
      "census_block_groups":[
         "01-234-567890-1"
      ],
      "zipcode":"32168",
      "county_name":"Volusia County",
      "city_name":"New Smyrna Beach",
      "name":"Gina Maini",
      "id":"3bf8788f-6872-43be-b2f7-cb46a0ac8aeb"
   },
   "queue_total":100,
   "paid_count":30,
   "timespan_days":30,
   "rejected_count":4,
   "version":"Alpha.01",
   "id":"c2ec7ea9-dae1-498e-b301-cac4adef97c9",
   "avg_waittime_30_days":15.2,
   "p99_waittime_30_days":13.7
}
```

## Version 1: GET /showReports Response:

```json
{
   "snapshots":[
      {
         "submitted_on":"6/24/2021 12:00:00 AM",
         "reporter":{
            "census_block_groups":[
               "01-234-567890-1"
            ],
            "zipcode":"32168",
            "county_name":"Volusia County",
            "city_name":"New Smyrna Beach",
            "name":"Gina Maini",
            "id":"3bf8788f-6872-43be-b2f7-cb46a0ac8aeb"
         },
         "queue_total":100,
         "paid_count":30,
         "timespan_days":30,
         "rejected_count":4,
         "version":"Alpha.01",
         "id":"c2ec7ea9-dae1-498e-b301-cac4adef97c9"
      }
   ],
   "p99_waittime_30_days":13.7,
   "avg_waittime_30_days":15.2
}
```

* Sample Type Information: https://github.com/ginabeena/example-era-app/blob/c32560a8439d928fc09c614c6bd8e378ffb0cc72/src/Shared/Shared.fs#L63
* Note: you can make all string information "optional" by passing blank strings, except the ids which are always required. You can makes the lists optional by passing an empty list (converted to a JavaScript Array). 
* All integers are 64bit in code, but needs to be updated for float only. 

## Tech Stack

This project uses [SAFE Stack](https://safe-stack.github.io/). It was created using the dotnet [SAFE Template](https://safe-stack.github.io/docs/template-overview/). If you want to learn more about the template why not start with the [quick start](https://safe-stack.github.io/docs/quickstart/) guide?

## Install pre-requisites
You'll need to install the following pre-requisites in order to build SAFE applications

* The [.NET Core SDK](https://www.microsoft.com/net/download) 3.1 or higher.
* [npm](https://nodejs.org/en/download/) package manager.
* [Node LTS](https://nodejs.org/en/download/).

## Starting the application
Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet fake build -t run
```

Then open `http://localhost:8080` in your browser.

To run concurrently server and client tests in watch mode (run in a new terminal):

```bash
dotnet fake build -t runtests
```

Client tests are available under `http://localhost:8081` in your browser and server tests are running in watch mode in console.

## SAFE Stack Documentation
If you want to know more about the full Azure Stack and all of it's components (including Azure) visit the official [SAFE documentation](https://safe-stack.github.io/docs/).

You will find more documentation about the used F# components at the following places:

* [Saturn](https://saturnframework.org/docs/)
* [Fable](https://fable.io/docs/)
* [Elmish](https://elmish.github.io/elmish/)
