open Encodings
open Secure
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave
// open System.IdentityModel.Claims
open System.Security.Claims

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 
    let base64Key = Base64String.FromString "sCHbhZlmfgciUZFVdgfy9y-YcPSM53iJBg2ffk0_T3U"
    let jwtConfig = {
        Issuer = "http://127.0.0.1:8083/suave"
        ClientId = "0f445642-7c24-453f-a041-90c5c46f155e4"
        SecurityKey = KeyStore.securityKey base64Key
    }

    let authorizeAdmin (claims : Claim seq) =
        // let isAdmin (c : Claim) =
        //     c.Type = ClaimTypes.Role && c.Value = "Admin"
        // match claims |> Seq.tryFind isAdmin with
        match claims |> Seq.tryFind (fun c -> c.Type = ClaimTypes.Role && c.Value = "Admin") with
        | Some _ -> Authorized |> async.Return
        | None -> UnAuthorized "Not an admin" |> async.Return

    let sample1 = path "/audience2/sample1" >=> jwtAuthenticate jwtConfig (OK "sample 2")
    let sample2 = path "/audience2/sample2" >=> jwtAuthenticate jwtConfig (OK "ini admin")

    let config = {
        defaultConfig with bindings = [HttpBinding.mkSimple HTTP "127.0.0.1" 8085]
    }

    let app = choose [ sample1;sample2 ]

    startWebServer config app
    // printfn "%A" argv
    0 // return an integer exit code
