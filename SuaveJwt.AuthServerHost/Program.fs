open Suave
open System
open AuthServer
open JwtToken

[<EntryPoint>]
let main argv = 
    let authorizationServerConfig = {
        AddAudienceUrlPath = "/api/audience"
        CreateTokenUrlPath = "/oauth2/token"
        SaveAudience = AudienceStorage.saveAudience
        GetAudience = AudienceStorage.getAudience
        Issuer = "http://127.0.0.1:8083/suave"
        TokenTimeSpan = TimeSpan.FromMinutes(1.)
    }

    let identityStore = {
        GetClaim = IdentityStore.GetClaim
        IsValidCredential = IdentityStore.IsValidCredential
        GetSecurityKey = IdentityStore.GetSecurityKey
        GetSigningCredential = IdentityStore.GetSigningCredential
    }

    let audienceWebPart' = audienceWebPart authorizationServerConfig identityStore

    // audienceWebPart authorizationServerConfig
    // |> startWebServer defaultConfig

    startWebServer defaultConfig audienceWebPart'
    0