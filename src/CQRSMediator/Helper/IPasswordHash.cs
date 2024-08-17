namespace CQRSMediator.Helper;

public interface IPasswordHash
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}