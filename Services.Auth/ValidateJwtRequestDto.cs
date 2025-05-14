namespace Services.Auth;

public class ValidateJwtRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}