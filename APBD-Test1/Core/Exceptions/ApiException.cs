using System.Net;

namespace APBD_8.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; set; }

    public ApiException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}