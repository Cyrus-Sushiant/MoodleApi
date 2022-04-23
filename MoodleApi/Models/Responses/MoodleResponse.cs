using MoodleApi.Extensions;

namespace MoodleApi.Models.Responses;

/// <summary>
/// Represents the API response.
/// </summary>
/// <typeparam name="T">The type of data that's going to be contained in the response.</typeparam>
public class MoodleResponse<T> where T : IDataModel
{
    /// <summary>
    /// Gets the API response data.
    /// </summary>
    public bool Succeeded { get; }

    public T[]? DataArray { get; }

    public T? Data { get; }

    public Error? Error { get; }

    internal MoodleResponse(string stringJson)
    {
        Error = stringJson.ParseJson<Error>();

        Succeeded = Error is null || (Error.ErrorCode.HasNoValue() && Error.Exception.HasNoValue() && Error.Message.HasNoValue());

        if (Succeeded)
        {
            Error = null;
            Data = stringJson.ParseJson<T>();
        }
    }
}