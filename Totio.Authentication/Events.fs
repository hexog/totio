namespace Totio.Authentication.Events

open Microsoft.Extensions.Logging
open Totio.Authentication.Client
open Totio.Core.Events
open Totio.Core

type UserEventProducer(logger: ILogger<UserEventProducer>, configuration) =
    inherit ProducerBase<UserEvent>(logger |> unsafeCast, configuration)

    override this.Topic =
        UserEventConsumerService.UserEventTopic
