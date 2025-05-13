namespace Services.Auth;
public interface IAuthService
{
    string CreateJWT(int id, string username, string role);
    Task<string> ValidateJWT(string token);
}