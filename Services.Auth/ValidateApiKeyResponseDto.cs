namespace Services.Auth;

public class ValidateApiKeyResponseDto
{
    public bool IsValid { get; set; } = false;
    public string? ErrorMessage { get; set; } = "Unauthorized access. Invalid API key.";
}