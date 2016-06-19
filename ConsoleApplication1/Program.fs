module MiniSuave
open Suave.Http
open Suave.Console
open Suave.Successful
open Suave.Combinators
open Suave.Filters

[<EntryPoint>]
let main argv = 
    let request = {Route = ""; Type = Suave.Http.GET}
    let response = {Content = ""; StatusCode = 200}
    let context = {Request = request; Response = response}

    let app =
        Choose [
            GET >=> Path "/hello" >=> OK "got from /hello"
            POST >=> Path "/hello" >=> OK "posted to /hello"
            Path "/foo" >=> Choose [
                GET >=> OK "got from /foo"
                POST >=> OK "posted to /foo"
            ]
        ]
    executeInLoop context app

    0
