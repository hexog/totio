open System
open System.Reflection
open System.Threading.Tasks
open System.Timers
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Totio.Authentication.Client
open Totio.Authentication.Data
open Totio.Authentication.Events
open Totio.Authentication.Services
open Totio.Core
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore
open Microsoft.AspNetCore.Builder
open Totio.Core.Events
open Totio.Core.FSharp

[<EntryPoint>]
let rec main _ =
    App.Run(HostConfiguration.configureServices >> configureServices, configureApp)
    0

and configureServices services =
    services.AddControllers()
    |> ignore
    services.AddMvc()
    |> ignore

    services
        .AddProducer<UserEventProducer, UserEvent>()
//        .AddHostedService<HostedService>()
        .AddTotioDbContext(fun o -> o.UseSqlite("Data source=auth.db") |> ignore)
        .AddEntityHandler<UserRepository, Dbo.User>()
        .AddTypesFromAssembly(Assembly.GetExecutingAssembly())
    |> ignore

and configureApp app =
    app.MapControllers()
    |> ignore
