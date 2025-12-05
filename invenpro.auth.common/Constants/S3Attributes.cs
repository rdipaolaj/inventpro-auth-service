namespace invenpro.auth.common.Constants;

public static class S3Typology
{
    public const string Consult = "Consultar";
    public const string Register = "Registrar";
    public const string Delete = "Eliminar";
    public const string Update = "Actualizar";
}

public static class S3Functionality
{
    //public const string ObtenerMfa = "ObtenerMfa";
}

public static class S3Context
{
    public const string Typology = "Typology";
    public const string Functionality = "Functionality";
    public const string BackendCode = "BackendCode";
}

public static class S3Microservice
{
    public const string AccountManagement = "Auth-Service";
}

public static class S3DataProperty
{
    public const string TransactionId = "transactionId";
    public const string Timestamp = "timeStamp";
    public const string ChannelCode = "channelCode";
    public const string IbsCode = "ibsCode";
    public const string Authorization = "authorization";
    public const string Typology = "typology";
    public const string BackendCode = "backendCode";
    public const string RoleName = "roleName";
    public const string ServerId = "serverid";
    public const string Module = "module";
    public const string Functionality = "functionality";
    public const string Request = "request";
    public const string Response = "response";
}

public static class S3Channel
{
    public const string App = "202";
}