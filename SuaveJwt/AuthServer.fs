module AuthServer

open System
open JwtToken
open SuaveJSON
open Suave
open Suave.Http
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

type AudienceCreateRequest = {
    Name : string
}

type AudienceCreateResponse = {
    ClientId : string
    Base64Secret : string
    Name : string
}

type Config = {
    AddAudienceUrlPath : string
    SaveAudience : Audience -> Async<Audience>
    CreateTokenUrlPath : string
    GetAudience : string -> Async<Audience option>
    Issuer : string
    TokenTimeSpan : TimeSpan
}

type TokenCreateCredential = {
    Username : string
    Password : string
    ClientId : string
}

let audienceWebPart config identityStore =
    let toAudienceCreateResponse (audience : Audience) = {
        Base64Secret = audience.Secret.ToString()
        ClientId = audience.ClientId
        Name = audience.Name
    }
    let tryCreateAudience (ctx : HttpContext) =
        match mapJsonPayload<AudienceCreateRequest> ctx.request with
        | Some audienceCreateRequest ->
            async {
                let! audience =
                    audienceCreateRequest.Name
                    |> createAudience
                    |> config.SaveAudience
                let audienceCreateResponse =
                    toAudienceCreateResponse audience
                return! JSON audienceCreateResponse ctx
            } 
        | None -> BAD_REQUEST "Invalid Audience Create Request" ctx

    let tryCreateToken (ctx : HttpContext) =
        match mapJsonPayload<TokenCreateCredential> ctx.request with
        | Some tokenCreateCredential ->
            async {
                let! audience = config.GetAudience tokenCreateCredential.ClientId
                match audience with
                | Some audience ->
                    let tokenRequest : TokenCreateRequest = {
                        Issuer = config.Issuer
                        Username = tokenCreateCredential.Username
                        Password = tokenCreateCredential.Password
                        TokenTimeSpan = config.TokenTimeSpan
                    }
                    let! token = createToken tokenRequest identityStore audience
                    match token with
                    | Some token -> return! JSON token ctx
                    | None -> return! BAD_REQUEST "Invalid Login Credential" ctx
                | None -> return! BAD_REQUEST "Invalid Client Id" ctx
            }
        | None -> BAD_REQUEST "Invalid Token Create Request" ctx

    choose [
        path config.AddAudienceUrlPath >=> POST >=> tryCreateAudience
        path config.CreateTokenUrlPath >=> POST >=> tryCreateToken
    ]
