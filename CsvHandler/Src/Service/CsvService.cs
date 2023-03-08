using CsvHandler.Entity;
using CsvHandler.Exceptions;
using CsvHandler.Repository;
using CsvHandler.Utils;
using CsvHandler.Validator;
using Microsoft.EntityFrameworkCore;

namespace CsvHandler.Service;

public class CsvService
{
    private readonly CsvDbContext _dbContext;
    private readonly CsvServiceHelper _helper;
    
    public CsvService(CsvDbContext dbContext, 
        CsvServiceHelper helper)
    {
        _dbContext = dbContext;
        _helper = helper;
    }
    
    public void UploadCsv(IFormFile file)
    {
        var newCsvFileEntity = _helper
            .UploadCsv(file);
        
        var oldCsvFileEntity = _dbContext.Files
            .FirstOrDefault(f => 
                f.Name!.Equals(file.FileName));

        if (oldCsvFileEntity != null)
        {
            _dbContext.Files.Remove(oldCsvFileEntity);
        }
        _dbContext.Files.Add(newCsvFileEntity);
        _dbContext.SaveChanges();
    }

    public List<ValuesEntity> SearchValuesByFileName(string filename)
    {
        return _helper.SearchValuesByFileName(
            _dbContext.Values, filename);
    }

    public List<ResultsEntity> SearchResults(
        string? filename, 
        DateTime? startedFrom, DateTime? startedTo,
        double? metricAvgFrom, double? metricAvgTo,
        double? timeAvgFrom, double? timeAvgTo)
    {
        var select = _dbContext.Results
            .Include(e => e.CsvFileEntity)
            .AsEnumerable();

        var filters = _helper
            .CreateFilters(filename,
                startedFrom, startedTo,
                metricAvgFrom, metricAvgTo,
                timeAvgFrom, timeAvgTo);

        return _helper.SearchResults(select, filters);
    }
}




public class CsvServiceHelper
{
    private readonly ValuesValidator _valuesValidator;
    private readonly ResultsValidator _resultsValidator;

    public CsvServiceHelper(ValuesValidator valuesValidator,
        ResultsValidator resultsValidator)
    {
        _valuesValidator = valuesValidator;
        _resultsValidator = resultsValidator;
    }

    public CsvFileEntity UploadCsv(IFormFile file)
    {
        var csvFileEntity = new CsvFileEntity
        {
            Name = file.FileName,
            Values = new List<ValuesEntity>()
        };

        ResultsAggregator resultsAggregator = new();
        using var r = new StreamReader(file.OpenReadStream());
        while (!r.EndOfStream)
        {
            var lineData = r.ReadLine()?.Split(";");
            var values = new ValuesEntity
            {
                DateTime = Util.ParseDateTime(lineData?[0]!),
                Seconds = int.Parse(lineData?[1]!),
                Metric = Util.ParseDouble(lineData?[2]!)
            };
            csvFileEntity.Values.Add(values);
            _valuesValidator.Validate(values);
            resultsAggregator.Aggregate(values);
            _resultsValidator.Validate(resultsAggregator.Results);
        }
        _resultsValidator.Validate(resultsAggregator.Results);
        csvFileEntity.Results = resultsAggregator.Results;

        return csvFileEntity;
    }

    public List<ValuesEntity> SearchValuesByFileName(
        IEnumerable<ValuesEntity> source, string filename)
    {
        return source.Where(v => v
                .CsvFileEntity!.Name!.Equals(filename))
            .ToList();
    }
    
    public List<ResultsEntity> SearchResults(
        IEnumerable<ResultsEntity> source, 
        IReadOnlyCollection<Predicate<ResultsEntity>> filters)
    {
        return source.Where(e => 
            filters.All(f => f(e))
        ).ToList();
    }
    
    public List<Predicate<ResultsEntity>> CreateFilters(
        string? filename, 
        DateTime? startedFrom, DateTime? startedTo,
        double? metricAvgFrom, double? metricAvgTo,
        double? timeAvgFrom, double? timeAvgTo)
    {
        var filters = new List<Predicate<ResultsEntity>>();

        if (filename != null)
        {
            filters.Add(e => filename.Equals(e.CsvFileEntity?.Name));
        }
        if (startedFrom != null)
        {
            filters.Add(e => startedFrom <= e.TimeStarted);
        }
        if (startedTo != null)
        {
            filters.Add(e => e.TimeStarted <= startedTo);
        }
        if (metricAvgFrom != null)
        {
            filters.Add(e => metricAvgFrom <= e.MetricAvg);
        }
        if (metricAvgTo != null)
        {
            filters.Add(e => e.MetricAvg <= metricAvgTo);
        }
        if (timeAvgFrom != null)
        {
            filters.Add(e => timeAvgFrom <= e.SecondsAvg);
        }
        if (timeAvgTo != null)
        {
            filters.Add(e => e.SecondsAvg <= timeAvgTo);
        }

        if (filters.Count == 0)
        {
            throw new FiltersEmptyException("Заданы пустые параметры для поиска");
        }

        return filters;
    }
}