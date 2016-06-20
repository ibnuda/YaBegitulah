open System
open Suave.Web
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
        TokenTimeSpan = TimeSpan.FromMinutes(9.)
    }

    let identityStore = {
        GetClaim = IdentityStore.getClaim
        IsValidCredential = IdentityStore.isValidCredential
        GetSecurityKey = KeyStore.securityKey
        GetSigningCredential = KeyStore.hmacSha256
    }

    let audienceWebPart' = audienceWebPart authorizationServerConfig identityStore

    // audienceWebPart authorizationServerConfig
    // |> startWebServer defaultConfig

    startWebServer defaultConfig audienceWebPart'
    0