using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Totio.Authentication.Client;

#nullable disable

public class RegisterUserRequest
{
	public Guid? Id { get; set; }
	[Required] public string Username { get; set; }
	[Required, EmailAddress] public string Email { get; set; }
	[Required] public string Password { get; set; }
}

public abstract class LoginRequest
{
	[Required] public string Password { get; set; }
}

public class UsernameLoginRequest : LoginRequest
{
	[Required] public string Username { get; set; }
}

public class EmailLoginRequest : LoginRequest
{
	[Required, EmailAddress] public string Email { get; set; }
}
