module AudienceStorage

open System.Collections.Generic
open JwtToken

let private audienceStorage =
    new Dictionary<string, Audience>()

let saveAudience (audience : Audience) =
    audienceStorage.Add(audience.ClientId, audience)
    audience |> async.Return