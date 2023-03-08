namespace CsvHandler.Exceptions;

public class CsvApiException : Exception
{
    public CsvApiException(string? message) : base(message) { }
}