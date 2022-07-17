namespace Totio.Authentication.Services

open System
open System.Text
open Microsoft.Extensions.Logging
open Totio.Authentication.Client
open Totio.Authentication.Data
open Totio.Authentication.Events
open Totio.Authentication.Services

type UserCreateParameters =
    { Id: Guid
      Username: string
      Email: string
      Password: string }

module UserServiceImpl =
    let validateUser (userRepository: UserRepository) (parameters: UserCreateParameters) =
        backgroundTask {
            let errorList = ResizeArray()
            let! existingUsernameUser = userRepository.FindByUsernameAsync(parameters.Username)

            match existingUsernameUser with
            | Some _ -> errorList.Add("User with this username already exists")
            | None -> ()

            let! existingEmailUser = userRepository.FindByEmailAsync(parameters.Email)

            match existingEmailUser with
            | Some _ -> errorList.Add("User with this email already exists")
            | None -> ()

            match errorList with
            | x when x.Count > 0 -> return Error (errorList.ToArray ())
            | _ -> return Ok parameters
        }

    let createUser (userRepository: UserRepository) (parameters: UserCreateParameters) =
        backgroundTask {
            let passwordHash, salt = Cryptography.generateHash parameters.Password

            let userDbo: Dbo.User =
                { Id = parameters.Id
                  Email = parameters.Email
                  Username = parameters.Username
                  PasswordHash = passwordHash
                  Salt = salt }

            do! userRepository.AddAsync(userDbo)

            return userDbo
        }

open UserServiceImpl

type UserService(userRepository: UserRepository, logger: ILogger<UserService>, userEventProducer: UserEventProducer) =

    member this.NotifyCreatedUserAsync(user: Dbo.User) =
        let message = UserEvent(UserEventType.Create,
                                User(user.Id,
                                     user.Username,
                                     user.Email))
        userEventProducer.ProduceAsync(message)

    member this.CreateUserAsync(parameters) =
        backgroundTask {
            let! validationResult = validateUser userRepository parameters
            match validationResult with
            | Error e -> return Error e
            | Ok parameters ->
                let! createdUser = createUser userRepository parameters

                do! this.NotifyCreatedUserAsync(createdUser)

                return Ok createdUser.Id
        }

    member this.LoginWithEmailAsync(email: string, password: string) =
        backgroundTask {
            let! user = userRepository.FindByEmailAsync email
            match user with
            | None -> return Error "User not found"
            | Some user -> return this.LoginInner(user, password)
        }

    member this.LoginWithUsernameAsync(username: string, password: string) =
        backgroundTask {
            let! user = userRepository.FindByUsernameAsync username
            match user with
            | None -> return Error "User not found"
            | Some user -> return this.LoginInner(user, password)
        }

    member private this.LoginInner(user: Dbo.User, password: string) =
        let salt = user.Salt
        let originalPasswordHash = Convert.FromBase64String user.PasswordHash
        let saltBytes = Convert.FromBase64String salt
        let passwordBytes = Cryptography.getBytes password
        let hashed = Cryptography.hashPassword passwordBytes saltBytes

        let isSame = Array.fold2 (fun isSame l r -> isSame && l = r) true hashed originalPasswordHash

        if isSame
        then Ok (User(user.Id,
                      user.Username,
                      user.Email))
        else Error "Invalid username or password"
