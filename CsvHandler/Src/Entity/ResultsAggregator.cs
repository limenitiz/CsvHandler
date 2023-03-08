namespace CsvHandler.Entity;

public class ResultsAggregator
{
    public ResultsEntity Results { get; } = new();

    private DateTime? _timeFinished;
    private double secondsSum;
    private double secondsCount;
    private double metricsSum;
    private double metricsCount;
    private readonly List<double> _metrics = new();

    public void Aggregate(ValuesEntity valuesEntity)
    {
        var dateTime = valuesEntity.DateTime;
        var seconds = valuesEntity.Seconds;
        var metric = valuesEntity.Metric;

        if (Results.TimeStarted == null || Results.TimeStarted > dateTime)
        {
            Results.TimeStarted = dateTime;
        }

        if (_timeFinished == null || _timeFinished < dateTime)
        {
            _timeFinished = dateTime;
        }

        if (_timeFinished != null)
        {
            var start = (DateTime)Results.TimeStarted;
            var finish = (DateTime)_timeFinished;
            Results.TimeTotal = new DateTime(
                finish.Subtract(start).Ticks);
        }

        secondsSum += seconds;
        secondsCount += 1;
        Results.SecondsAvg = secondsSum / secondsCount;

        metricsSum += metric;
        metricsCount += 1;
        Results.MetricAvg = metricsSum / metricsCount;

        _metrics.Add(metric);
        _metrics.Sort();
        Results.MetricMedian = _metrics[_metrics.Count / 2];

        if (Results.MetricMax == null || Results.MetricMax < metric)
        {
            Results.MetricMax = metric;
        }

        if (Results.MetricMin == null || Results.MetricMin > metric)
        {
            Results.MetricMin = metric;
        }

        Results.RowsCount += 1;
    }
}