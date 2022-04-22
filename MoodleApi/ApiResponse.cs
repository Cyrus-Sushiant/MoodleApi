using System.Text.Json;

namespace MoodleApi
{
 /// <summary>
 /// Repressents the response from authentication
 /// </summary>
 /// <typeparam name="T"></typeparam>
    public class AuthentiactionResponse<T> where T : IDataModel
    {
        public T Data { get; private set; }

        public AuthenticationError Error { get; private set; }

        internal AuthentiactionResponse(AuthentiactionResponseRaw rawResponse)
        {
            this.Error = rawResponse.Error.Deserialize<AuthenticationError>();
            this.Data = rawResponse.Data.Deserialize<T>();
        }
    }

    internal class AuthentiactionResponseRaw
    {
        internal JsonElement Data { get; set; }
        internal JsonElement Error { get; set; }

        public AuthentiactionResponseRaw(JsonElement data)
        {
            Data = data;
            Error = data;
        }
    }

    /// <summary>
    /// Represents the API response.
    /// </summary>
    /// <typeparam name="T">The type of data that's going to be contained in the response.</typeparam>
    public class ApiResponse<T> where T : IDataModel
    {
        /// <summary>
        /// Gets the API response data.
        /// </summary>
        public string Status { get; private set; }

        public T[] DataArray { get; private set; }

        public T Data { get; private set; }

        public Error Error { get; private set; }
    
        internal ApiResponse(ApiResponseRaw rawResponse)
        {
            this.Error = rawResponse.Error.Deserialize<Error>();

            if (string.IsNullOrEmpty(Error.errorcode) && string.IsNullOrEmpty(Error.exception) && string.IsNullOrEmpty(Error.message))
                Status = "Succesful";
            else
                Status = "Failed";

            if (rawResponse.DataArray is not null)
                this.DataArray = rawResponse.DataArray.Select(d => d.Deserialize<T>()).ToArray();
            else
                this.Data = rawResponse.Data.Deserialize<T>();
        }
    }

    internal class ApiResponseRaw
    {
        public ApiResponseRaw(JsonElement data)
        {
            Data = data;
            Error = data;
        }

        public ApiResponseRaw(JsonElement[] data)
        {
            DataArray = data;
            Error = new JsonElement();
        }

        internal JsonElement[] DataArray { get; set; }
        internal JsonElement Data { get; set; }
        internal JsonElement Error { get; set; }
    }
}