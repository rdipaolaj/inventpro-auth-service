namespace invenpro.auth.common.Settings;

public class RedisSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public bool Local { get; set; }
    public bool Active { get; set; }
}