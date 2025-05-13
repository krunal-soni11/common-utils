﻿namespace Services.Auth;

public class JwtSettings
{
    public string Key { get; set; } = "YourSuperSecretKey123!@#";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60; // optional
}