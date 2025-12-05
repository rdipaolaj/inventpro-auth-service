namespace invenpro.auth.common.Constants;

public static class RegexPattern
{
    public const string OnlyNumbers = "^[0-9]*$";
    public const string Auth0UserId = "^auth0\\|[a-z0-9]{1,50}$";
    public const string Ipv4 = "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
    public const string NoScript = @"^(?!.*<.*?>).*$";
}