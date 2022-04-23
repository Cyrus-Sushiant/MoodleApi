namespace MoodleApi.Models.Responses;

public class AuthentiactionResult
{
    public AuthentiactionResult(AuthenticationError? error = null)
    {
        Error = error;
        Succeeded = error is null;
    }

    public bool Succeeded { get; }
    public AuthenticationError? Error { get; }
}