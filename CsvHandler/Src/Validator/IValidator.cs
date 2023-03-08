namespace CsvHandler.Validator;

public interface IValidator <in T>
{
    public void Validate(T t);
}
