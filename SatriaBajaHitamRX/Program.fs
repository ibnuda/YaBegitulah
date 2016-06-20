open Suave.Filters
open Suave

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 
    let webPart = pathScan "/api/profile/%s" ApiGateway.getProfile
    startWebServer defaultConfig webPart
    0 // return an integer exit code
