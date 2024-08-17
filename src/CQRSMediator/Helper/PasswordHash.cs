
using System.Security.Cryptography;

namespace CQRSMediator.Helper;

public sealed class PasswordHash : IPasswordHash
{
    private const int saltSize = 16;
    private const int hashSize = 32;
    private const int iteration = 100000;
    private readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,salt, iteration, algorithm, hashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteration, algorithm, hashSize);

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);    
    }
}