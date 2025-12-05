using invenpro.auth.common.Constants;
using invenpro.auth.common.Enums;
using invenpro.auth.common.Helper;
using System.Text.Json.Serialization;

namespace invenpro.auth.common.Responses;

public class ApiResponse<T>
{
    [JsonIgnore]
    public int? HttpStatus { get; private set; }

    public void SetHttpStatus(int statusCode)
    {
        HttpStatus = statusCode;
    }
    public Meta Meta { get; set; } = new Meta();

    [JsonPropertyName("datos")]
    public T Data { get; set; } = default!;

    public bool IsValid() => Meta is not null && Meta.Result;

    public void AddMessage(string codigo, string message)
    {
        Meta.AddMessage(codigo, message, nameof(TipoMensaje.INFORMATION));
    }

    public void AddMessage(string codigo, string message, string type)
    {
        Meta.AddMessage(codigo, message, type);
    }

    public void AddError(string code, string message)
    {
        Meta.AddError(code, message);
    }

    public void AddError(string message)
    {
        Meta.AddError(message);
    }

    public void AddMessages(List<ResponseMessage> messages)
    {
        Meta.Messages.AddRange(messages);

        if (messages.Exists(x => x.Type == nameof(TipoMensaje.ERROR)))
            Meta.Result = false;
    }

    public void AddSuccessMessage()
    {
        Meta.AddMessage(MessageCode.Success, MessageDescription.Success, nameof(TipoMensaje.INFORMATION));
    }

    public void SetData(T data)
    {
        Data = data;
    }

    public bool IsValidOrSetNewMeta<TNewApiResponseType>(ApiResponse<TNewApiResponseType> newApiResponse)
    {
        bool valid = IsValid();

        if (!valid)
            newApiResponse.Meta = Meta;

        return valid;
    }

    public ApiResponse<T> CreateResponeWithError(string code, string message)
    {
        ApiResponse<T> apiResponse = new();
        apiResponse.AddError(code, message);
        return apiResponse;
    }
}

public class Meta
{
    [JsonPropertyName("mensajes")]
    public List<ResponseMessage> Messages { get; set; } = [];

    [JsonPropertyName("idTransaccion")]
    public string IdTransaction { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("resultado")]
    public bool Result { get; set; } = true;

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = DatetimeHelper.Now().ToString("yyyyMMdd HHmmss");

    public void AddMessage(string code, string message, string type)
    {
        Messages.Add(new ResponseMessage(code, message, type));

        if (type == nameof(TipoMensaje.ERROR))
            Result = false;
    }

    public void AddError(string message)
    {
        Result = false;
        Messages.Add(new ResponseMessage(MessageCode.Error, message, nameof(TipoMensaje.ERROR)));
    }

    public void AddError(string code, string message)
    {
        Result = false;
        Messages.Add(new ResponseMessage(code, message, nameof(TipoMensaje.ERROR)));
    }
}

public class ResponseMessage(string code, string message, string type)
{
    [JsonPropertyName("codigo")]
    public string Code { get; set; } = code;

    [JsonPropertyName("mensaje")]
    public string Message { get; set; } = message;

    [JsonPropertyName("tipo")]
    public string Type { get; set; } = type;
}