module ApiGateway

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Suave
open Suave.Successful
open Suave.Operators
open Gateway
open Suave.RequestErrors

let JSON v =
    let settings = new JsonSerializerSettings()
    settings.ContractResolver <- new CamelCasePropertyNamesContractResolver()

    JsonConvert.SerializeObject(v, settings)
    |> OK
    >=> Writers.setMimeType "application/json; charset=utf-8"

let getProfile username (httpContext : HttpContext) =
    async {
        let! profile = getProfile username
        match profile with
        | Some p -> return! JSON p httpContext
        | None ->
            let message = sprintf "username %s not found" username
            return! NOT_FOUND message httpContext
    }