module Totio.Authentication.Services.Cryptography

open System
open System.Security.Cryptography
open System.Text

let getBytes (str: string) = Encoding.UTF8.GetBytes str

let generateSalt () = RandomNumberGenerator.GetBytes(10)

let hashPassword (password: byte array) (salt: byte array) =
    Rfc2898DeriveBytes.Pbkdf2(password, salt, 10_000, HashAlgorithmName.SHA256, 64)

let generateHash (password: string) =
    let salt = generateSalt ()
    let passwordBytes = getBytes password
    let hash = hashPassword passwordBytes salt

    Convert.ToBase64String hash, Convert.ToBase64String salt
