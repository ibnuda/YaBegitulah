module Github

open Http
open FSharp.Data

type GithubUser = JsonProvider<"Users.json">
type GithubRepo = JsonProvider<"Repos.json">

type Profile = {
    Name : string
    AvatarUrl : string
    PopularRepositories : Repository seq
} and Repository = {
    Name : string
    Stars : int
    Languages : string []
}

let parseUser = GithubUser.Parse
let parseUserRepos = GithubRepo.Parse

let host = "https://api.github.com"
let userUrl = sprintf "%s/users/%s" host
let reposUrl = sprintf "%s/users/%s/repos" host
let languagesUrl repoName userName = sprintf "%s/repos/%s/%s/languages" host userName repoName

let parseLanguage languagesJson =
    languagesJson
    |> JsonValue.Parse
    |> JsonExtensions.Properties
    |> Array.map fst

let popularRepos (repos : GithubRepo.Root []) =
    let ownRepos = repos |> Array.filter (fun repo -> not repo.Fork)
    let takeCount = if ownRepos.Length > 3 then 3 else ownRepos.Length

    ownRepos
    |> Array.sortBy (fun r -> r.StargazersCount)
    |> Array.toSeq
    |> Seq.take takeCount
    |> Seq.toArray

let reposResponseToPopularRepos = function
    | Ok(r) -> r |> parseUserRepos |> popularRepos
    | _ -> [||]

let languageResponseToRepoWithLanguages (repo : GithubRepo.Root) = function
    | Ok(l) ->
        { Name = repo.Name
          Languages = (parseLanguage l)
          Stars = repo.StargazersCount }
    | _ ->
        { Name = repo.Name
          Languages = Array.empty
          Stars = repo.StargazersCount }

let toProfile = function
    | Ok(u), repos ->
        let user = parseUser u
        { Name = user.Name
          PopularRepositories = repos
          AvatarUrl = user.AvatarUrl } |> Some
    | _ -> None 