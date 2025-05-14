namespace Services.Auth;
public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    public AuthService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }
    public CreateJwtResponseDto CreateJWT(CreateJwtRequestDto createJwtRequestDto)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, createJwtRequestDto.UserId),
            new Claim(ClaimTypes.Name, createJwtRequestDto.Username)
        };
        foreach (var role in createJwtRequestDto.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
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
        var tokenResponse = new CreateJwtResponseDto
        {
            AccessToken = token,
            ExpiresAt = tokenDescriptor.Expires ?? DateTime.UtcNow.AddHours(1)
        };
        return tokenResponse;
    }

    public async Task<ValidateJwtResponseDto> ValidateJWT(ValidateJwtRequestDto validateJwtRequestDto)
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
        var validateTokenResponse = new ValidateJwtResponseDto();
        try
        {
            var principal = await tokenHandler.ValidateTokenAsync(validateJwtRequestDto.AccessToken, validationParameters);
            //var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            //var roleClaim = principal.FindFirst(ClaimTypes.Role);
            //username = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            //return userIdClaim?.Value ?? string.Empty;
            validateTokenResponse.IsValid = true;
        }
        catch (Exception ex)
        {
            validateTokenResponse.ErrorMessage = ex.Message;
            validateTokenResponse.IsValid = false;
        }
        return validateTokenResponse;
    }
}