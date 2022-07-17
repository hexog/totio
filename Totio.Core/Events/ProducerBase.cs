using System.Net;
using System.Reflection;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Totio.Core.Events;

public abstract class ProducerBase<TMessage>
{
	private static readonly JsonSerializer<TMessage> Serializer = new();

	protected readonly ILogger<ProducerBase<TMessage>> Logger;
	private readonly IConfiguration configuration;

	protected ProducerBase(ILogger<ProducerBase<TMessage>> logger, IConfiguration configuration)
	{
		Logger = logger;
		this.configuration = configuration;
	}

	public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default)
	{
		IProducer<string, TMessage> producer;

		try
		{
			producer = GetProducer();
		}
		catch (Exception e)
		{
			Logger.LogError("Could not create producer: {Error}. Topic: {Topic}", e, Topic);
			throw;
		}

		try
		{
			await producer.ProduceAsync(Topic, new Message<string, TMessage>
				{
					Value = message,
				}, cancellationToken)
			   .ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Logger.LogError("Could not produce message: {Error}. Topic: {Topic}", e, Topic);
			throw;
		}
		finally
		{
			producer.Dispose();
		}
	}

	private IProducer<string, TMessage> GetProducer()
	{
		var config = new ProducerConfig()
		{
			BootstrapServers = configuration["Kafka:BootstrapServers"],
			ClientId = configuration["Kafka:ClientId"],
		};

		var producerBuilder = new ProducerBuilder<string, TMessage>(config);

		producerBuilder.SetValueSerializer(Serializer);

		var producer = producerBuilder.Build();
		return producer;
	}

	protected abstract string Topic { get; }
}
