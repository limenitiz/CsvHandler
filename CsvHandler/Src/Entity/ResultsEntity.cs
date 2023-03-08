using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CsvHandler.Entity;

public class ResultsEntity
{
    [JsonIgnore] [Key]
    public long ResultsId { get; set; }
    [JsonIgnore] [ForeignKey("CsvFileId")]
    public CsvFileEntity? CsvFileEntity { get; set; }
    public DateTime? TimeTotal { get; set; }
    public DateTime? TimeStarted { get; set; }
    public double SecondsAvg { get; set; }
    public double MetricAvg { get; set; }
    public double MetricMedian { get; set; }
    public double? MetricMax { get; set; }
    public double? MetricMin { get; set; }
    public int RowsCount { get; set; }
}
