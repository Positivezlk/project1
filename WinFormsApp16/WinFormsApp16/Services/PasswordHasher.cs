using System.Security.Cryptography;
namespace CertDesk.Services;
public static class PasswordHasher
{
    private const int Iterations=120000; private const int SaltSize=16; private const int KeySize=32;
    public static string HashPassword(string password){ byte[] salt=RandomNumberGenerator.GetBytes(SaltSize); byte[] hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,Iterations,HashAlgorithmName.SHA256,KeySize); return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}"; }
    public static bool VerifyPassword(string password,string storedHash){ var p=storedHash.Split('.'); if(p.Length!=3||!int.TryParse(p[0],out var it)) return false; var salt=Convert.FromBase64String(p[1]); var expected=Convert.FromBase64String(p[2]); var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,it,HashAlgorithmName.SHA256,expected.Length); return CryptographicOperations.FixedTimeEquals(actual,expected); }
}
