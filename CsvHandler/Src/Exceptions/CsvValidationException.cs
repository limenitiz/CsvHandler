namespace CsvHandler.Exceptions;

public class CsvValidationException : CsvApiException
{
    public CsvValidationException(string? s) : base(s) { }
}