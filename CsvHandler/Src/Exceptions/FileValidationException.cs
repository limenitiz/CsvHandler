namespace CsvHandler.Exceptions;

public class FileEmptyException : CsvApiException
{
    public FileEmptyException(string? message) : base(message) { }
}