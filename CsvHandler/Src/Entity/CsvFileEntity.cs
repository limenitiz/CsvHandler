using System.ComponentModel.DataAnnotations;

namespace CsvHandler.Entity;

public class CsvFileEntity
{
    [Key] public long FileId { get; set; }
    public string? Name { get; set; }
    public ICollection<ValuesEntity>? Values { get; set; }
    public ResultsEntity? Results { get; set; }
}
