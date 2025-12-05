namespace invenpro.auth.common.Constants;

public static class HeaderConstant
{
    public const string ChannelCode = "ChannelCode";
    public const string DeviceId = "DeviceId";
    public const string Timestamp = "Timestamp";
    public const string TransactionId = "TransactionId";
    public const string DeviceIp = "DeviceIp";
}

/// <summary>
/// Clase de formatos para los headers
/// </summary>
public static class HeaderFormat
{
    public const string TransactionIdFormat = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";
    public const string TimestampFormat = @"^\d{4}(0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])(2[0-3]|[01][0-9])([0-5][0-9]){2}$";
}