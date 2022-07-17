module Totio.Core.FSharp.HostConfiguration

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection

let configureJson (services: IServiceCollection) =
    services.AddControllers()
        .AddJsonOptions(Json.configureSerializers)
    |> ignore
    services

let configureServices = configureJson
