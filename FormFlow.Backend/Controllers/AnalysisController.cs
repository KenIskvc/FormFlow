using FormFlow.Backend.Contracts;
using FormFlow.Backend.DTOs;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using FormFlow.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FormFlow.Backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AnalysisController : ControllerBase {

    private readonly IPoseAnalysisServicecs _analaysisService;
    private readonly IVideoRepository _videoRepository;
    private readonly IAnalysisRepository _analysisRepository;

    public AnalysisController(IPoseAnalysisServicecs analaysisService, IVideoRepository videoRepository, IAnalysisRepository analysisRepository) {
        _analaysisService = analaysisService;
        _videoRepository = videoRepository;
        _analysisRepository = analysisRepository;
    }

    [Authorize]
    [HttpPost("/analyze/{videoId}")]

    public async Task<IActionResult> AnalyzeVideo([FromRoute] int videoId, CancellationToken ct) {

        var video = await _videoRepository.GetVideoAsync(videoId);

        if (video == null)
            return NotFound("Video not found.");

        var reportAsJson = string.Empty;

        try {
            reportAsJson = await _analaysisService.AnalyzeAsync(
                video.FileData,
                video.FileName,
                ct
            );
        } catch (HttpRequestException) {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Pose analysis service is unavailable."
            );
        }

        if (string.IsNullOrWhiteSpace(reportAsJson)) {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                "Pose analysis returned no result."
            );
        }

        var errorCount = CountErrorsFromReport(reportAsJson);

        var analysis = new Analysis
        {
            VideoId = videoId,
            CreatedAt = DateTime.UtcNow,
            Report = reportAsJson,
            ErrorCount = errorCount
        };

        await _analysisRepository.AddAnalaysis(analysis);

        return Ok(new AnalysisResponseDto
        {
            CreatedAt = analysis.CreatedAt,
            AnalysisId = analysis.Id,
            ErrorCount = analysis.ErrorCount,
            Report = analysis.Report
        });

    }

    /*
     * This is how a possible call from the frontend could look like:
        public async Task<string> AnalyzeAsync(
        string filePath,
        CancellationToken ct)
        {
            using var content = new MultipartFormDataContent();

            await using var stream = File.OpenRead(filePath);

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");

            content.Add(fileContent, "file", Path.GetFileName(filePath));

            using var response = await _client.PostAsync("/analyze", content, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }

        EXAMPLE WITH FILEPICKER:


        var result = await FilePicker.Default.PickAsync(
        new PickOptions
        {
            PickerTitle = "Select video"
        });

        if (result != null)
        {
            var reportJson = await AnalyzeAsync(result.FullPath, ct);
        }
     */
    [AllowAnonymous]
    [HttpPost("/analyze")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AnalyzeUpload([FromForm] UploadRequest request, CancellationToken ct) {
        var file = request.File;

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var bytes = ms.ToArray();

        var reportAsJson = await _analaysisService.AnalyzeAsync(bytes, file.FileName, ct);

        if (string.IsNullOrWhiteSpace(reportAsJson))
            return StatusCode(502, "Pose analysis returned no result.");

        var errorCount = CountErrorsFromReport(reportAsJson);

        return Ok(new AnalysisResponseDto
        {
            CreatedAt = DateTime.UtcNow,
            AnalysisId = null,
            ErrorCount = errorCount,
            Report = reportAsJson
        });

        //or as simple json
        //return Ok(new {
        //create = DateTime.UtcNow,
        //report = reportAsJson
        //})
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<AnalysisResponseDto>>> GetMyAnalyses(
        CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var analyses =
            await _analysisRepository.GetAnalysesForUserAsync(userId, ct);

        var result = analyses.Select(a => new AnalysisResponseDto
        {
            AnalysisId = a.Id,
            CreatedAt = a.CreatedAt,
            ErrorCount = a.ErrorCount,
            Report = a.Report
        }).ToList();

        return Ok(result);
    }

    private static int CountErrorsFromReport(string reportJson)
    {
        if (string.IsNullOrWhiteSpace(reportJson))
            return 0;

        try
        {
            using var doc = JsonDocument.Parse(reportJson);

            if (doc.RootElement.TryGetProperty("technicalErrors", out var errors) &&
                errors.ValueKind == JsonValueKind.Array)
            {
                return errors.GetArrayLength();
            }
        }
        catch{}

        return 0;
    }

}
