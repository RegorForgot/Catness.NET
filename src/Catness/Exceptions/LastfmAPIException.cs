using Catness.Models.Lastfm;

namespace Catness.Exceptions;

public class LastfmAPIException : Exception
{
    public override string Message { get; }
    public LastfmErrorCode ErrorCode { get; }

    public LastfmAPIException()
    {
        ErrorCode = LastfmErrorCode.Unknown;
        Message = $"{ErrorCode.ToString()}";
    }

    public LastfmAPIException(string message, LastfmErrorCode code)
    {
        Message = $"{code.ToString()}: {message}";
        ErrorCode = code;
    }
}