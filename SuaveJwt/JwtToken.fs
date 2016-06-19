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

type TokenCreateRequest = {
    Issuer : string
    Username : string
    Password : string
    TokenTimeSpan : TimeSpan
}

type IdentityStore = {
    GetClaim : string -> Async<Claim seq>
    IsValidCredential : string -> string -> Async<bool>
    GetSecurityKey : Base64String -> SecurityKey
    GetSigningCredential : SecurityKey -> SigningCredentials
}

type Token = {
    AccessToken : string
    ExpiresIn : float
}

let createAudience audienceName =
    let clientId = Guid.NewGuid().ToString()
    let data = Array.zeroCreate 32
    RNGCryptoServiceProvider.Create().GetBytes(data)
    let secret = data |> Base64String.Create
    {ClientId = clientId; Secret = secret; Name = audienceName}

let createToken request identityStore audience =
    async {
        let username = request.Username
        let! isValidCredential = identityStore.IsValidCredential username request.Password
        if isValidCredential then
            let signingCredential =
                audience.Secret
                |> (identityStore.GetSecurityKey >> identityStore.GetSigningCredential)
            let issuedOn = Nullable DateTime.UtcNow
            let expiresBy = Nullable (DateTime.UtcNow.Add(request.TokenTimeSpan))
            let! claims = identityStore.GetClaim username
            let jwtSecurityToken =
                new JwtSecurityToken(request.Issuer, audience.ClientId, claims, issuedOn, expiresBy, signingCredential)
            let handler = new JwtSecurityTokenHandler()
            let accessToken = handler.WriteToken(jwtSecurityToken)

            return Some { AccessToken = accessToken; ExpiresIn = request.TokenTimeSpan.TotalSeconds }
        else
            return None
    }