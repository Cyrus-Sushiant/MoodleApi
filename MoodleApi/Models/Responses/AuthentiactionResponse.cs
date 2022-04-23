using MoodleApi.Extensions;

namespace MoodleApi.Models.Responses;

/// <summary>
/// Repressents the response from authentication
/// </summary>
/// <typeparam name="T"></typeparam>
public class AuthentiactionResponse<T> where T : IDataModel
{
    public T? Data { get; }
    public AuthenticationError? Error { get; }

    internal AuthentiactionResponse(string stringJson)
    {
        Data = stringJson.ParseJson<T>();
        Error = stringJson.ParseJson<AuthenticationError>();
    }
}