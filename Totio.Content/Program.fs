open System
open System.Reflection
open System.Text.Json
open System.Threading.Tasks
open System.Timers
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Totio.Authentication.Client
open Totio.Core
open Microsoft.Extensions.DependencyInjection
open Totio.Core.Events

type UserEventContentPrinter(logger: ILogger<UserEventContentPrinter>, configuration) as this =
    inherit UserEventConsumerService(logger |> unsafeCast, configuration)

    let logger = this.Logger

    override this.HandleEvent(message, cancellationToken) =
        task { logger.LogInformation("Received user: {User}", JsonSerializer.Serialize(message)) }

type HostedService(accountClient: AccountClient) =
    inherit BackgroundService()

    override this.ExecuteAsync(stoppingToken) =
        task {
            let! id =
                accountClient.RegisterUserAsync(
                    RegisterUserRequest(Username = "myusername", Email = "mem@example.com", Password = "mypassword")
                )

            printfn "%A" id
        }

App.Run (fun services ->
    services
        .AddConsumer<UserEventContentPrinter, _>()
        .AddHostedService<HostedService>()
        .AddTypesFromAssembly(Assembly.GetExecutingAssembly())
        .AddTotioAuthenticationClients()
    |> ignore)
