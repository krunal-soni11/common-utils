namespace Services.Auth;

public class ValidateApiKeyRequestDto
{
    public string ApiKeyHeaderName { get; set; } = "X-API-Key";
    public string ApiKeyExpected { get; set; } = "coffee-is-my-auth-token";
    public required string ApiKeyProvided { get; set; }
}