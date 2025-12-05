namespace invenpro.auth.common.Settings;

public class SecretManagerSettings
{
    public bool Local { get; set; }
    public string Region { get; set; } = string.Empty;
    public bool UseLocalstack { get; set; }
    public string LocalStackServiceURL { get; set; } = string.Empty;
    public string AWSLocalStackKey { get; set; } = string.Empty;
    public string AWSLocalStackSecret { get; set; } = string.Empty;

    public string ArnAuthService { get; set; } = string.Empty;
}