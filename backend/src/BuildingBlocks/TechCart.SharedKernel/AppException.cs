namespace TechCart.SharedKernel;

// Tum moduller icin "is kurali ihlali" hatasi
public class AppException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    protected AppException(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode; // hatanin HTTP karsiligi
    }
}