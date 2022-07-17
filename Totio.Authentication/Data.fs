namespace Totio.Authentication.Data

open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.FSharp.Core
open Totio.Core.Data
open Microsoft.EntityFrameworkCore

[<RequireQualifiedAccess>]
module Dbo =
    [<CLIMutable>]
    [<Table("users")>]
    type User =
        { [<Key; Column("id")>] Id: Guid
          [<Column("username")>] Username: string
          [<Column("email")>] Email: string
          [<Column("password_hash")>] PasswordHash: string
          [<Column("salt")>] Salt: string }

[<CustomDi>]
type UserRepository(dbContext) as this =
    inherit EntityHandlerBase<Dbo.User>(dbContext)

    let set = this.Set

    member this.FindAsync(id) =
        query {
            for e in set do
                where (e.Id = id)
                select e
        }
        |> Query.tryHeadAsync

    member this.FindByUsernameAsync(username) =
        query {
            for u in set do
                where (u.Username = username)
                select u
        }
        |> Query.tryHeadAsync

    member this.FindByEmailAsync(email) =
        query {
            for u in set do
                where (u.Email = email)
                select u
        }
        |> Query.tryHeadAsync

    member this.AddAsync(user: Dbo.User) =
        set.Add(user) |> ignore
        dbContext.SaveChangesAsync() :> Task
