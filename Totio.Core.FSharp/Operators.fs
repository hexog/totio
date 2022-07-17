namespace Totio.Core

[<AutoOpen>]
module Operators =
    let inline unsafeCast<'t> (object: obj) = unbox<'t> object
