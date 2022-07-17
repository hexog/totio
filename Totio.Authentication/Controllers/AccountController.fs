namespace Totio.Authentication.Controllers

open System
open System.ComponentModel.DataAnnotations
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Totio.Authentication.Client
open Totio.Authentication.Services

[<CLIMutable>]
type LoginRequest =
    { Username: string option
      Email: string option // TODO: email validation when field is not required
      [<Required>]
      Password: string }

[<Route("api/account")>]
[<ApiController>]
type UserController(userService: UserService) =
    inherit ControllerBase()

    [<HttpPost("register")>]
    member this.RegisterUser([<FromBody>] request: RegisterUserRequest) : Task<IActionResult> =
        task {
            match this.ModelState.IsValid with
            | false -> return this.BadRequest(this.ModelState) :> IActionResult
            | true ->
                let! result =
                    userService.CreateUserAsync(
                        { Id =
                            Option.ofNullable request.Id
                            |> Option.defaultWith Guid.NewGuid
                          Username = request.Username
                          Email = request.Email
                          Password = request.Password }
                    )

                match result with
                | Error e -> return this.BadRequest(e) :> IActionResult
                | Ok userId -> return this.Ok(userId) :> IActionResult
        }

    [<HttpPost("login")>]
    member this.Login([<FromBody>] loginRequest: LoginRequest) =
        task {
            let handleLoginResult(result: Result<User, string>) : IActionResult =
                match result with
                | Ok user -> this.Ok(user)
                | Error e -> this.BadRequest(e)

            match this.ModelState.IsValid, loginRequest with
            | false, _ -> return this.BadRequest(this.ModelState) :> IActionResult
            | true, { Email = None; Username = None } -> return this.BadRequest("Username or email is rquired")
            | true,
              { Username = Some username
                Password = password } ->
                let! loginResult = userService.LoginWithUsernameAsync(username, password)
                return handleLoginResult loginResult
            | true,
              { Email = Some email
                Password = password } ->
                let! loginResult = userService.LoginWithEmailAsync(email, password)
                return handleLoginResult loginResult
        }
