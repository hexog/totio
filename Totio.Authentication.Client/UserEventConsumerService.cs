using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Totio.Core.Events;

namespace Totio.Authentication.Client;

public record UserEvent(UserEventType EventType, User User);

public enum UserEventType
{
	Create = 1,
	Update = 2,
	Delete = 3,
}

public record User(Guid Id, string Username, string Email);

public abstract class UserEventConsumerService : ConsumerServiceBase<UserEvent>
{
	protected override string Topic => UserEventTopic;

	public static string UserEventTopic = "user-events";

	protected UserEventConsumerService(ILogger<UserEventConsumerService> logger, IConfiguration configuration) : base(logger, configuration)
	{
	}
}
