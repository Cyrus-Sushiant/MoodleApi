using MoodleApi.Extensions;

namespace MoodleApi.Models.Responses;

/// <summary>
/// Represents the API response.
/// </summary>
/// <typeparam name="T">The type of data that's going to be contained in the response.</typeparam>
public class MoodleResponse<T> where T : IDataModel
{
    private readonly Error? error;

    /// <summary>
    /// Gets the API response data.
    /// </summary>
    public bool Succeeded { get; }

    public T[]? DataArray { get; }

    public T? Data { get; }

    public Error? Error => error;
    internal MoodleResponse(string stringJson)
    {
        Succeeded = stringJson.TryParseJson(out error) is false || Error is null || (Error.ErrorCode.HasNoValue() && Error.Exception.HasNoValue() && Error.Message.HasNoValue());

        if (Succeeded)
        {
            error = null;
            if (stringJson.StartsWith("["))
            {
                DataArray = stringJson.ParseJson<T[]>();
            }
            else
            {
                Data = stringJson.ParseJson<T>();
            }
        }
    }
}