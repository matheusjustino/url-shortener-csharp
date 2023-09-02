namespace url_shortener.Api.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, Exception ex)
        : base(message, ex)
    {
    }
}
