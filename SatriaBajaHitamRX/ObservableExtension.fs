module ObservableExtension

open FSharp.Control.Reactive

let flatMap2 f observable =
    observable
    |> Observable.flatmap (Array.map f >> Observable.mergeArray)
    |> Observable.toArray