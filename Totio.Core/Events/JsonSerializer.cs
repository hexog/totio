using System.Runtime.Serialization;
using System.Text.Json;
using Confluent.Kafka;

namespace Totio.Core.Events;

public class JsonSerializer<TValue> : ISerializer<TValue>, IDeserializer<TValue>
{
	public byte[] Serialize(TValue data, SerializationContext context)
	{
		return JsonSerializer.SerializeToUtf8Bytes(data);
	}

	public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
	{
		return (TValue)(JsonSerializer.Deserialize(data, typeof(TValue))
					 ?? throw new SerializationException($"Could not deserialize value of type {typeof(TValue).Name}"));
	}
}
