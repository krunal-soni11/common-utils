namespace Services.Auth;
public interface IAuthService
{
    string GenerateJwtToken(string username);
}