module JwtToken

open Encodings
open System
open System.Security.Claims
open System.IdentityModel.Tokens
open System.Security.Cryptography

type Audience = {
    ClientId : string
    Secret : Base64String
    Name : string
}

let createAudience audienceName =
    let clientId = Guid.NewGuid().ToString()
    let data = Array.zeroCreate 32
    RNGCryptoServiceProvider.Create().GetBytes(data)
    let secret = data |> Base64String.Create
    {ClientId = clientId; Secret = secret; Name = audienceName}



