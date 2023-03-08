using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CsvHandler.Entity;

public class ValuesEntity
{
    [Key] 
    [JsonIgnore]
    public long ValuesId { get; set; }
    [ForeignKey("CsvFileId")]
    [JsonIgnore]
    public CsvFileEntity? CsvFileEntity { get; set; }

    public DateTime DateTime { get; set; }
    public int Seconds { get; set; }
    public double Metric { get; set; }
}
