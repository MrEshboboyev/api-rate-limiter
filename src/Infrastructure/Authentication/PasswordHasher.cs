using System.Security.Cryptography;
using Application.Abstractions.Security;

namespace Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    #region Private fields

    private const int SaltSize = 16;

    private const int HashSize = 32;

    private const int Iterations = 500000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    #endregion

    #region Implementations
    
    public string Hash(string password)
    {
        // Generate a new salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Hash the password with the salt
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations,
            Algorithm, HashSize);

        // Return the hash and salt as a single string
        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        // Split the hashed password into hash and salt parts
        var parts = passwordHash.Split('-');
        var hash = Convert.FromHexString(parts[0]);
        var salt = Convert.FromHexString(parts[1]);

        // Hash the input password with the original salt
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations,
            Algorithm, HashSize);

        // Compare the input hash with the original hash
        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }

    #endregion
}