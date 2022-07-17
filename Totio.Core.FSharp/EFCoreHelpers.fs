namespace Totio.Core.Data

open System.Linq
open Microsoft.EntityFrameworkCore

[<AutoOpen>]
module EFCoreHelpers =
    module Query =
        let inline toArrayAsync (xs: IQueryable<_>) = xs.ToArrayAsync()

        let inline tryHeadAsync (xs: IQueryable<_>) =
            task {
                let! head = xs.FirstOrDefaultAsync()
                match obj.ReferenceEquals(head, null) with
                | true -> return None
                | false -> return Some head
            }
