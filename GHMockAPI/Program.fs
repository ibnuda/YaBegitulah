open System
open Suave
open Suave.Operators
open Suave.Successful
open Suave.Filters
open System.IO

[<EntryPoint>]
let main argv = 
    
    let json fileName =
        let content = File.ReadAllText fileName
        content
        |> OK >=> Writers.setMimeType "application/json"

    let user = pathScan "/users/%s" (fun _ -> "Users.json" |> json)
    let repo = pathScan "/users/%s/repos" (fun _ -> "Repos.json" |> json)
    let mockApi = choose [user; repo]

    startWebServer defaultConfig mockApi
    0 // return an integer exit code
