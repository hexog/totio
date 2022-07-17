using System.Text.Json;
using Totio.Authentication.Client;
using Totio.Core.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConsumer<UserEventPrinter, UserEvent>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class UserEventPrinter : UserEventConsumerService
{
	protected override Task HandleEvent(UserEvent message, CancellationToken? cancellationToken)
	{
		var serialize = JsonSerializer.Serialize(message);
		Console.WriteLine(serialize);
		return Task.CompletedTask;
	}

	public UserEventPrinter(ILogger<UserEventPrinter> logger, IConfiguration configuration) : base(logger, configuration)
	{
	}
}
