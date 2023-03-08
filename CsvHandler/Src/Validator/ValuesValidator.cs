using CsvHandler.Entity;
using CsvHandler.Exceptions;

namespace CsvHandler.Validator;

public class ValuesValidator : IValidator<ValuesEntity>
{
    public void Validate(ValuesEntity values)
    {
        if (values.DateTime >= DateTime.Now)
        {
            throw new CsvValidationException("Дата не может быть позже текущей." +
                                             "Дата: " + values.DateTime);
        }

        if (values.DateTime < DateTime.Parse("2000.01.01"))
        {
            throw new CsvValidationException("Дата не может быть раньше 01.01.2000." +
                                             "Дата: " + values.DateTime);
        }

        if (values.Seconds < 0)
        {
            throw new CsvValidationException("Время не может быть меньше 0. " +
                                             "Время: " + values.Seconds);
        }

        if (values.Metric < 0)
        {
            throw new CsvValidationException("Значение показателя не может быть меньше 0. " +
                                             "Значение показателя: " + values.Metric);
        }
    }
}
