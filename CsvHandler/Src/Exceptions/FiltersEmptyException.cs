namespace CsvHandler.Exceptions;

public class FiltersEmptyException : CsvApiException
{
    public FiltersEmptyException(string? message) : base(message) { }
}