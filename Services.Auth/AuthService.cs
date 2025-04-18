namespace Services.Auth;
public class AuthService : IAuthService
{
    public string GenerateJwtToken(string username)
    {
        var key = Encoding.UTF8.GetBytes("YourSuperSecretKey123!@#");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)//,
                //new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.Aes192CbcHmacSha384)
        };
        var tokenHandler = new JsonWebTokenHandler();
        string token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
}