namespace invenpro.auth.common.Constants;

public static class ChannelConstant
{
    public const string PortalWeb = "PORTAL_WEB";
    public const string PortalMovil = "PORTAL_MOVIL";
    public const string PortalWebCode = "201";
    public const string PortalMovilCode = "202";
}

public static class ChannelCodeConstantExtensions
{
    private static readonly Dictionary<string, string> _channelCodes = new()
    {
        { ChannelConstant.PortalWebCode, ChannelConstant.PortalWeb },
        { ChannelConstant.PortalMovilCode, ChannelConstant.PortalMovil }
    };

    public static bool FindCode(this string value)
    {
        return _channelCodes.TryGetValue(value, out _);
    }

    public static string? GetDescripcion(this string value)
    {
        if (_channelCodes.TryGetValue(value, out string? description))
        {
            return description;
        }
        return null;
    }
}