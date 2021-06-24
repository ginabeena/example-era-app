# Example ERA Prototype App

## Version 1: /submitReport Request:

```
 {
    "queue_total": int64
    "paid_count": int64
    "timespan_days": int64
    "rejected_count": int64
    "reporter": {
        "census_block_groups": string list;
        "zipcode": string;
        "county_name": string;
        "city_name": string;
        "name": string;
        "id": string; 
        }
}      
```

## Version 1: /submitReport Response:

```
{
    "submitted_on": string
    "reporter": {
        "census_block_groups": string list;
        "zipcode": string;
        "county_name": string;
        "city_name": string;
        "name": string;
        "id": string; 
        }
    "queue_total": int64
    "paid_count": int64
    "timespan_days": int64
    "rejected_count": int64
    "version": string
    "id": string
}

```

* Sample Type Information: https://github.com/ginabeena/example-era-app/blob/c32560a8439d928fc09c614c6bd8e378ffb0cc72/src/Shared/Shared.fs#L63
* Note: you can make all string information "optional" by passing blank strings, except the ids which are always required.

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
