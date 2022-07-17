using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Totio.Core;

namespace Totio.Authentication.Client;

public class AccountClient : ApiClientBase
{
	public AccountClient(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory, configuration)
	{
	}

	public Task<Guid> RegisterUserAsync(RegisterUserRequest request)
	{
		return this.PostAsync<Guid>("api/account/register", request);
	}

	public Task<User> LoginAsync(LoginRequest request)
	{
		return this.PostAsync<User>("api/account/login", request);
	}
}
