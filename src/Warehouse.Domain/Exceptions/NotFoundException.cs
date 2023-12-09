namespace Warehouse.Domain.Exceptions;

public class NotFoundException : Exception
{
    private const string DefaultErrorMessage = "Entity was not found";

    public NotFoundException() : base(DefaultErrorMessage)
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(Exception innerException) : base(DefaultErrorMessage, innerException)
    {
    }
}
