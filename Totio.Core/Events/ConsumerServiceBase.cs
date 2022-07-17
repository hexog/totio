using System.Net;
using System.Reflection;
using Confluent.Kafka;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Totio.Core.Events;

public abstract class ConsumerServiceBase<TMessage> : BackgroundService
{
	private static readonly JsonSerializer<TMessage> Serializer = new();

	protected readonly ILogger<ConsumerServiceBase<TMessage>> Logger;
	private readonly IConfiguration configuration;

	protected ConsumerServiceBase(ILogger<ConsumerServiceBase<TMessage>> logger, IConfiguration configuration)
	{
		Logger = logger;
		this.configuration = configuration;
	}

	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(() => ExecuteLoopAsync(cancellationToken), cancellationToken);
		return Task.CompletedTask;
	}

	private async Task ExecuteLoopAsync(CancellationToken cancellationToken)
	{
		IConsumer<string, TMessage> consumer;

		try
		{
			consumer = GetConsumer();
		}
		catch (Exception e)
		{
			Logger.LogError("Could not start consumer loop: {Error}. Topic: {Topic}", e, Topic);
			throw;
		}

		while (cancellationToken.IsCancellationRequested == false)
		{
			try
			{
				var events = consumer.Consume(cancellationToken);
				consumer.Commit();
				await HandleEvent(events.Message.Value, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				Logger.LogError("Could not consume or process message: {Error}. Topic: {Topic}", e, Topic);
				await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
			}
		}

		consumer.Dispose();
	}

	protected abstract Task HandleEvent(TMessage message, CancellationToken? cancellationToken);
	protected abstract string Topic { get; }

	private IConsumer<string, TMessage> GetConsumer()
	{
		var config = new ConsumerConfig
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"],
			GroupId = configuration["Kafka:GroupId"],
			AutoOffsetReset = AutoOffsetReset.Earliest,
		};

		var consumerBuilder = new ConsumerBuilder<string, TMessage>(config);
		consumerBuilder.SetValueDeserializer(Serializer);
		var consumer = consumerBuilder.Build();
		consumer.Subscribe(Topic);
		return consumer;
	}
}
