namespace Services.Auth;
public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    public AuthService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }
    public string CreateJWT(int id, string username, string role)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            IssuedAt = DateTime.UtcNow,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        //tokenDescriptor.Claims = new Dictionary<string, object>
        //{
        //    { "userId", userId },
        //    { "role", role }
        //};
        var tokenHandler = new JsonWebTokenHandler();
        string token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }

    public async Task<string> ValidateJWT(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var tokenHandler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero // Remove delay of token when expire
        };
        try
        {
            var principal = await tokenHandler.ValidateTokenAsync(token, validationParameters);
            //var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            //var roleClaim = principal.FindFirst(ClaimTypes.Role);
            //username = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            //return userIdClaim?.Value ?? string.Empty;
            return string.Empty;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
}