module KeyStore

open System.IdentityModel.Tokens
open Encodings

let securityKey sharedKey : SecurityKey =
    let symmetricKey = sharedKey |> Base64String.Decode
    new InMemorySymmetricSecurityKey(symmetricKey) :> SecurityKey

let hmacSha256 secretKey =
    new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest)