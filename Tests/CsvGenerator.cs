using Microsoft.AspNetCore.Http;

namespace CsvTests;

public static class CsvGenerator
{
    public static IFormFile NewFile(GeneratorParams param)
    {
        return param.IsRandom ? NewRandomFile(param) : NewNormalFile(param);
    }

    private static IFormFile NewRandomFile(GeneratorParams p)
    {
        var date = p.LatestDate ?? DateTime.Now;
        var ms = new MemoryStream(50);
        var w = new StreamWriter(ms);
        for (var i = 0; i < p.RowsCount; i++)
        {
            var dateTime = date.Subtract(TimeSpan.FromHours(24 * i * p.Shift));
            var s = $"{dateTime:yyyy-MM-dd_HH-mm-ss};" +
                    $"{p.SecondsValue ?? RandLong(p.MinSecondsValue, p.MaxSecondsValue)};" +
                    $"{p.MetricValue ?? RandDouble(p.MinMetricValue, p.MaxMetricValue):F3}";
            w.WriteLine(s);
        }
        w.Flush();
        return new FormFile(
            w.BaseStream,
            0,
            w.BaseStream.Length,
            p.Filename, p.Filename);
    }

    private static IFormFile NewNormalFile(GeneratorParams p)
    {
        var date = p.LatestDate ?? DateTime.Now;
        var ms = new MemoryStream(50);
        var w = new StreamWriter(ms);
        for (var i = 0; i < p.RowsCount; i++)
        {
            var dateTime = date.Subtract(TimeSpan.FromHours(24 * i * p.Shift));
            var s = $"{dateTime:yyyy-MM-dd_HH-mm-ss};" +
                    $"{p.SecondsValue ?? RandLong(p.MinSecondsValue, p.MaxSecondsValue)};" +
                    $"{p.MetricValue ?? RoundedDouble(i, p.MinMetricValue, p.MaxMetricValue):F3}";
            w.WriteLine(s);
        }
        w.Flush();
        return new FormFile(
            w.BaseStream,
            0,
            w.BaseStream.Length,
            p.Filename, p.Filename);
    }

    private static readonly Random Random = new();
    private static double RandDouble(int min, int max)
    {
        return Random.NextDouble() * (max - min) + min;
    }

    private static double RoundedDouble(int x, int min, int max)
    {
        return (min + x) % max;
    }

    private static long RandLong(long min, long max)
    {
        return Random.NextInt64(min, max);
    }
}


public record GeneratorParams
{
    public int RowsCount = 100;
    public int MinMetricValue = 1000;
    public int MaxMetricValue = 2000;
    public int? MetricValue = null;
    public int MinSecondsValue = 0;
    public int MaxSecondsValue = 9999;
    public int? SecondsValue = null;
    public string Filename = "test-file.csv";
    public DateTime? LatestDate = null;
    public double Shift = 0.5;
    public bool IsRandom = true;
}
