using CsvHandler.Entity;
using CsvHandler.Exceptions;

namespace CsvHandler.Validator;

public class ResultsValidator : IValidator<ResultsEntity>
{
    public void Validate(ResultsEntity results)
    {
        switch (results.RowsCount)
        {
            case < 1:
                throw new CsvValidationException("Количество строк не может быть меньше 1. " +
                                                 "Количество строк:" + results.RowsCount);
            case > 10_000:
                throw new CsvValidationException("Количество строк не может быть больше 10 000" +
                                                 "Количество строк:" + results.RowsCount);
        }
    }
}
