module Totio.Core.FSharp.Json

open System.Text.Json.Serialization
open Microsoft.AspNetCore.Mvc

let configureSerializers (options: JsonOptions) =
    options.JsonSerializerOptions.Converters.Add(JsonFSharpConverter())
