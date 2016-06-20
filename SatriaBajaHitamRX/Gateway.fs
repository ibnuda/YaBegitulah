module Gateway

open Github
open Http
open System.Reactive.Threading.Tasks
open FSharp.Control.Reactive
open ObservableExtension

let getProfile username =
    let userStream = username |> userUrl |> asyncResponseToObservable

    let toRepoWithLanguagesStream (repo : GithubRepo.Root) =
        username
        |> languagesUrl repo.Name
        |> asyncResponseToObservable
        |> Observable.map (languageResponseToRepoWithLanguages repo)

    let popularReposStream =
        username
        |> reposUrl
        |> asyncResponseToObservable
        |> Observable.map reposResponseToPopularRepos
        |> flatMap2 toRepoWithLanguagesStream

    async {
        return! popularReposStream
                |> Observable.zip userStream
                |> Observable.map toProfile
                |> TaskObservableExtensions.ToTask
                |> Async.AwaitTask
    }