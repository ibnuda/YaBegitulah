open Suave
open AuthServer

[<EntryPoint>]
let main argv = 
    let authorizationServerConfig = {
        AddAudienceUrlPath = "/api/audience"
        SaveAudience = AudienceStorage.saveAudience
    }
    audienceWebPart authorizationServerConfig
    |> startWebServer defaultConfig
    0