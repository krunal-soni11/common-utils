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
            new Claim(JwtRegisteredClaimNames.NameId, createJwtRequestDto.UserId),
            new Claim(JwtRegisteredClaimNames.Name, createJwtRequestDto.Username)
        };
        //foreach (var role in createJwtRequestDto.Roles)
        //{
        //    claims.Add(new Claim(ClaimTypes.Role, role));
        //}
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            IssuedAt = DateTime.UtcNow,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };
        tokenDescriptor.Claims = new Dictionary<string, object>
        {
            { "roles", JsonSerializer.Serialize(createJwtRequestDto.Roles) }
        };
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

        var tokenValidationResult = await tokenHandler.ValidateTokenAsync(validateJwtRequestDto.AccessToken, validationParameters);
        validateTokenResponse.IsValid = tokenValidationResult.IsValid;
        if (validateTokenResponse.IsValid)
        {
            validateTokenResponse.IsValid = tokenValidationResult.ClaimsIdentity.IsAuthenticated;
            var principalClaims = tokenValidationResult.Claims.Select(kv => new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty)).ToList();
            if (principalClaims.Select(x => x.Type).Contains(JwtRegisteredClaimNames.NameId))
            {
                validateTokenResponse.UserId = principalClaims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value;
            }
            if (principalClaims.Select(x => x.Type).Contains(JwtRegisteredClaimNames.Name))
            {
                validateTokenResponse.Username = principalClaims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
            }
            if (principalClaims.Select(x => x.Type).Contains("roles"))
            {
                validateTokenResponse.Roles = JsonSerializer.Deserialize<List<string>>(principalClaims.First(x => x.Type == "roles").Value);
            }
            //validateTokenResponse.UserId = principalClaims.First(x => x.k string.Empty;
            //validateTokenResponse.Roles = string.Empty;
        }
        else 
        {
            validateTokenResponse.ErrorMessage = GetUserFriendlyErrorMessage(tokenValidationResult.Exception);
        }

        return validateTokenResponse;
    }

    private string GetUserFriendlyErrorMessage(Exception? ex)
    {
        if (ex == null)
            return "An unknown error occurred during token validation.";

        // Extract known IDX error code
        var errorCode = ex.Message.Split(':')[0].Trim();

        return errorCode switch
        {
            "IDX10223" => "Your session has expired. Please log in again.",
            "IDX10501" => "The token signature is invalid.",
            "IDX10214" => "The token audience is invalid.",
            "IDX10213" => "The token issuer is invalid.",
            "IDX10205" => "The token algorithm is not supported.",
            _ => "The authentication has failed."
        };
    }
}