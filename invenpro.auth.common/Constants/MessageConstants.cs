namespace invenpro.auth.common.Constants;

public static class MessageCode
{
    public const string Success = "0";
    public const string Error = "999";

    public const string InvalidClaim = "4005";
    public const string InvalidJWTError = "4006";
    public const string ExpiredJWTError = "4007";
    public const string InvalidTransmitterJWTError = "4008";
    public const string InvalidAudienceJWTError = "4009";
    public const string UnknowJWTError = "4010";
    public const string CryptographicError = "4011";

    public const string MandatoryDecryptError = "1001";
    public const string InvalidDecryptError = "1002";
    public const string DecryptError = "1003";
    public const string DecodeError = "1004";
    public const string GenericDecryptError = "1005";
    public const string InvalidJsonBodyError = "1006";
    public const string DeserializeError = "1007";
    public const string MandatoryEncryptedBodyError = "1008";
    public const string InvalidEncryptedBodyError = "1009";

    public const string InvalidChannelCodeTypeFormatMessage = "Channel-code {0} no es válido.";

}

public static class MessageDescription
{
    public const string Success = "Ejecución con éxito.";
    public const string Error = "Ocurrió un error inesperado.";

    public const string InvalidClaim = "El JWT no pudo obtener el claim solicitado.";
    public const string InvalidJWTError = "El JWT es inválido o está mal formado.";
    public const string ExpiredJWTError = "El JWT ha expirado. La sesión ha caducado, por favor inicie sesión nuevamente.";
    public const string InvalidTransmitterJWTError = "El emisor del JWT es inválido.";
    public const string InvalidAudienceJWTError = "La audiencia del JWT es inválida.";
    public const string UnknowJWTError = "La validación del JWT falló por una razón desconocida";
    public const string CryptographicError = "Ocurrió un error criptográfico al intentar validar el token. Verifique que el certificado X509 sea válido.";


    public const string MandatoryDecryptError = "La lista de solicitudes encriptadas no puede ser nula o vacía.";
    public const string InvalidDecryptError = "Uno de los elementos encriptados es nulo o vacío.";
    public const string DecryptError = "Error durante la desencriptación.";
    public const string DecodeError = "Error durante la decodificación Base64: {0}";
    public const string GenericDecryptError = "Error inesperado: {0}";
    public const string InvalidJsonBodyError = "El cuerpo desencriptado no es un JSON válido.";
    public const string DeserializeError = "Error deserializando el JSON.";
    public const string MandatoryEncryptedBodyError = "El cuerpo de la solicitud está vacío o no está encriptado. Por favor, encripte el cuerpo antes de enviarlo.";
    public const string InvalidEncryptedBodyError = "El cuerpo de la solicitud contiene elementos que no están encriptados. Por favor, asegúrese de encriptar todo el cuerpo.";

    public const string MandatoryAuthorizationMessage = "Header authorization es obligatorio";
    public const string MandatoryChannelCodeMessage = "Header Channel-Code es obligatorio";
    public const string MandatoryTransactionIdMessage = "Header Transaction-Id es obligatorio";
    public const string MandatorTimestampMessage = "Header Timestamp es obligatorio";
    public const string InvalidChannelCodeTypeFormatMessage = "Channel-code {0} no es válido.";
    public const string InvalidTransactionIdTypeFormatMessage = "Transaction-Id {0} no es válido.";
    public const string InvalidTimestampTypeFormatMessage = "Timestamp {0} no es válido.";
    public const string InvalidAccessTokenMessage = "Token de authorización inválido";

}


public static class AuthMessageCode
{
    public const string InvalidCredentials = "AUTH00001";
    public const string MissingUserIdFromToken = "AUTH00002";
}

public static class AuthMessageDescription
{
    public const string InvalidCredentials = "Las credenciales proporcionadas son inválidas.";
    public const string MissingUserIdFromToken = "Usuario no identificado en el token.";
}