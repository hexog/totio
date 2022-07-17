namespace Totio.Core;

public class ApiClientBase
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly IConfiguration configuration;

	protected ApiClientBase(IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{
		this.httpClientFactory = httpClientFactory;
		this.configuration = configuration;
	}

	protected async Task<TResponse> PostAsync<TResponse>(string path, object request)
	{
		using var client = httpClientFactory.CreateClient(this.GetType().Name);
		client.BaseAddress = new Uri(configuration["Services:Authentication:BaseAddress"]);

		var response = await client.PostAsync(path, JsonContent.Create(request)).ConfigureAwait(false);
		response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<TResponse>().ConfigureAwait(false)
			?? throw new Exception($"Could not deserialize response from request to {path}");
	}
}
