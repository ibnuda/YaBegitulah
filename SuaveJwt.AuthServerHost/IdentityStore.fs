module IdentityStore

open System.Security.Claims

let getClaim username =
    seq {
        yield (ClaimTypes.Name, username)
        if (username = "admin") then
            yield (ClaimTypes.Role, "admin")
        if (username = "tomi") then
            yield (ClaimTypes.Role, "super")
    } |> Seq.map (fun x -> new Claim(fst x, snd x)) |> async.Return

// TODO : at the moment, the book only check wheter if the username and the password
// are the same or not.
let isValidCredential username password =
    username = password |> async.Return