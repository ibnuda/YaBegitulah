open Resting.Rest
open Resting.Db
open Suave.Web
open Suave.Http

[<EntryPoint>]
let main argv = 
    let personWebPart = rest "people" {
        GetAll = Db.getPeople
        Create = Db.createPerson
        Update = Db.updatePerson
        Delete = Db.deletePerson
        GetById = Db.getPerson
        UpdateById = Db.updatePersonById
        IsExist = Db.isPersonExists
    }

    startWebServer defaultConfig personWebPart

    0 // return an integer exit code
