using CsvHandler.Exceptions;
using CsvHandler.Service;
using CsvHandler.Validator;

namespace CsvTests;

public class TestsUpload
{
    private CsvServiceHelper? _service;

    [SetUp]
    public void Setup()
    {
        _service = new CsvServiceHelper(
            new ValuesValidator(),
            new ResultsValidator());
    }

    [Test]
    public async Task ValidationRows()
    {
        var valid = new[]
        { 1, 100, 1000, 9999, 10_000 };

        foreach (var i in valid)
        {
            var entity = _service?.UploadCsv(
            CsvGenerator.NewFile(new GeneratorParams
                {
                    RowsCount = i
                }
            ));
            Assert.That(actual: entity!.Results!.RowsCount, 
                Is.EqualTo(i));
        }

        
        var invalid = new[]
        { 0, 10_001 };

        foreach (var i in invalid)
        {
            Assert.Catch<CsvApiException>(() => 
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        RowsCount = i
                    }
                )));
        }
    }

    [Test]
    public async Task ValidationDate()
    {
        var valid = new[]
        {
            DateTime.Now,
            DateTime.Now - TimeSpan.FromDays(1),
            DateTime.Now - TimeSpan.FromDays(365*1),
            DateTime.Now - TimeSpan.FromDays(365*2),
            DateTime.Now - TimeSpan.FromDays(365*3),
            DateTime.Parse("2000.01.01")
        };

        foreach (var i in valid)
        {
            Assert.DoesNotThrow(() =>
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        RowsCount = 1,
                        Shift = 0,
                        LatestDate = i
                    }
                )));
        }

        
        var invalid = new[]
        {
            DateTime.Now + TimeSpan.FromDays(1),
            DateTime.Now + TimeSpan.FromDays(365*1),
            DateTime.Now + TimeSpan.FromDays(365*2),
            DateTime.Now + TimeSpan.FromDays(365*3),
            DateTime.Parse("2000.01.01") - TimeSpan.FromDays(1),
            DateTime.Parse("2000.01.01") - TimeSpan.FromDays(2)
        };

        foreach (var i in invalid)
        {
            Assert.Catch<CsvApiException>(() => 
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        RowsCount = 1,
                        Shift = 0,
                        LatestDate = i
                    }
                )));
        }
    }

    [Test]
    public async Task ValidationSeconds()
    {
        var valid = new[]
            { 0, 1, 100, 1000, 9999, 10_000, 100_000 };

        foreach (var i in valid)
        {
            Assert.DoesNotThrow(() =>
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        SecondsValue = i
                    }
                )));
        }

        
        var invalid = new[]
            { -1, -100, -1000, -10_000, -100_000 };

        foreach (var i in invalid)
        {
            Assert.Catch<CsvApiException>(() => 
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        SecondsValue = i
                    }
                )));
        }
    }

    [Test]
    public async Task ValidationMetrics()
    {
        var valid = new[]
            { 0, 1, 100, 1000, 9999, 10_000, 100_000 };

        foreach (var i in valid)
        {
            Assert.DoesNotThrow(() =>
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        MetricValue = i
                    }
                )));
        }


        var invalid = new[]
            { -1, -100, -1000, -10_000, -100_000 };

        foreach (var i in invalid)
        {
            Assert.Catch<CsvApiException>(() =>
                _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        MetricValue = i
                    }
                )));
        }
    }
    
    [Test]
    public async Task TestResultsRowsCount()
    {
        for (var i = 1; i < 10_000; i+=100)
        {
            var csvFileEntity = _service?.UploadCsv(
            CsvGenerator.NewFile(new GeneratorParams()
                {
                    RowsCount = i
                }
            ));

            Assert.That(actual: csvFileEntity!.Results!.RowsCount, 
                Is.EqualTo(i));
        }
    }

    [Test]
    public async Task TestResultsMetrics()
    {
        for (var maxMetric = 100; maxMetric < 10_000; maxMetric+=100)
        {
            const double diff = 1.0;

            var csvFileEntity = _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        RowsCount = maxMetric,
                        MinMetricValue = 0,
                        MaxMetricValue = maxMetric,
                        IsRandom = false
                    }
                ));

            var csvFile = csvFileEntity!;
            var values = csvFile.Values!;
            var results = csvFile.Results!;

            var metricAvg = values.Sum(v => v.Metric) /
                            values.Count;

            Assert.Multiple(() =>
            {
                var leftInterval = 0 + diff;
                var rightInterval = maxMetric - diff;
                var median = maxMetric / 2.0;
                var medianLeftBorder = median - diff;
                var medianRightBorder = median + diff;
                Assert.That(metricAvg, Is.EqualTo(results.MetricAvg));
                Assert.That(leftInterval, Is.GreaterThanOrEqualTo(results.MetricMin));
                Assert.That(rightInterval, Is.LessThanOrEqualTo(results.MetricMax));
                Assert.That(medianLeftBorder < results.MetricMedian && 
                            results.MetricMedian < medianRightBorder, Is.True);
                Assert.That(medianLeftBorder < results.MetricAvg && 
                            results.MetricAvg < medianRightBorder, Is.True);
            });
        }
    }
    
    [Test]
    public async Task TestResultsSeconds()
    {
        for (var maxMetric = 100; maxMetric < 10_000; maxMetric+=100)
        {

            var csvFileEntity = _service?.UploadCsv(
                CsvGenerator.NewFile(new GeneratorParams()
                    {
                        RowsCount = maxMetric,
                        MinSecondsValue = 0,
                        MaxSecondsValue = maxMetric,
                        IsRandom = false
                    }
                ));

            var csvFile = csvFileEntity!;
            var values = csvFile.Values!;
            var results = csvFile.Results!;
            
            var secondsAvg = values.Sum(v => v.Seconds) /
                             (double) values.Count;

            Assert.That(secondsAvg, Is.EqualTo(results.SecondsAvg));
        }
    }
    
}
