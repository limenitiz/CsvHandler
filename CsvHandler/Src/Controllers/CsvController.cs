using System.Text.Json;
using CsvHandler.Dto;
using CsvHandler.Entity;
using CsvHandler.Exceptions;
using CsvHandler.Service;
using Microsoft.AspNetCore.Mvc;

namespace CsvHandler.Controllers;

[ApiController]
[Route("csv")]
[Produces("application/json")]
public class CsvController : ControllerBase
{
    private readonly CsvService _csvService;
    
    public CsvController(CsvService csvService)
    {
        _csvService = csvService;
    }

    [HttpPost("upload")]
    public ActionResult UploadCsv(IFormFile file)
    {
        if (!file.ContentType.Equals("text/csv") ||
            !file.FileName.EndsWith(".csv"))
            return BadRequest();

        try
        {
            _csvService.UploadCsv(file);
            return Ok();
        }
        catch (CsvApiException e)
        {
            return BadRequest(JsonSerializer.SerializeToDocument(new ErrorDto()
            {
                TimeStamp = DateTime.UtcNow,
                Message = e.Message
            }));
        }
    }

    [HttpGet("search/results")]
    public ActionResult<List<ResultsEntity>> SearchResults(
        string? filename, 
        DateTime? startedFrom, DateTime? startedTo,
        double? metricAvgFrom, double? metricAvgTo,
        double? timeAvgFrom, double? timeAvgTo)
    {
        try
        {
            var output = _csvService.SearchResults(
                filename,
                startedFrom, startedTo,
                metricAvgFrom, metricAvgTo,
                timeAvgFrom, timeAvgTo);

            if (output.Count == 0) { return NotFound(); }
            return output;
        }
        catch (CsvApiException e)
        {
            return BadRequest(JsonSerializer.SerializeToDocument(new ErrorDto()
            {
                TimeStamp = DateTime.UtcNow,
                Message = e.Message
            }));
        }
    }

    [HttpGet("search/values/{filename}")]
    public ActionResult<List<ValuesEntity>> GetValuesGyFileName(string filename)
    {
        var output = _csvService.SearchValuesByFileName(filename);
        if (output.Count == 0) { return NotFound(); }

        return output;
    }
}


[ApiController]
[Route("/")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SwaggerRedirectController : ControllerBase
{
    [HttpGet]
    public async Task<RedirectResult> Redirect()
    {
        return await Task.Run(() => 
            new RedirectResult("https://localhost:7208/swagger/index.html"));
    }
}
