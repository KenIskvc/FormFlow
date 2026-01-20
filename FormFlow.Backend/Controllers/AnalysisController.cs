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

    // Initializes the controller with all required dependencies.
    // The controller itself contains no business logic;
    // it only orchestrates services and repositories.
    public AnalysisController(IPoseAnalysisServicecs analaysisService, IVideoRepository videoRepository, IAnalysisRepository analysisRepository) {
        _analaysisService = analaysisService;
        _videoRepository = videoRepository;
        _analysisRepository = analysisRepository;
    }

    // Analyzes a previously uploaded video identified by its ID.
    // This endpoint requires authentication and persists the analysis result.
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
        };

        await _analysisRepository.AddAnalaysis(analysis);

        return Ok(new AnalysisResponseDto
        {
            CreatedAt = analysis.CreatedAt,
            AnalysisId = analysis.Id,
            ErrorCount = errorCount,
            Report = analysis.Report,
            VideoTitle = video.FileName
        });

    }

    // Analyzes a video uploaded directly via multipart/form-data.
    // This endpoint does NOT persist the analysis result and can be
    // used without authentication (session analysis).
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
            Report = reportAsJson,
            VideoTitle = file.FileName
        });
    }

    // Retrieves all analyses for the authenticated user.

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<AnalysisResponseDto>>> GetMyAnalyses(
        CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        List<Analysis> analyses;

        analyses = await _analysisRepository.GetAnalysesForUserAsync(userId, ct);

        var result = analyses.Select(a => new AnalysisResponseDto
        {
            AnalysisId = a.Id,
            CreatedAt = a.CreatedAt,
            ErrorCount = CountErrorsFromReport(a.Report),
            Report = a.Report,
            VideoTitle = a.Video.FileName
        }).ToList();

        return Ok(result);
    }

    // Counts the number of technical errors in an analysis report.
    // This method parses the JSON report and extracts the size
    // of the "technicalErrors" array.
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

    // Deletes a persisted analysis by its ID.
    [Authorize]
    [HttpDelete("{analysisId}")]
    public async Task<IActionResult> DeleteAnalysis(
    int analysisId,
    CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var analysis = await _analysisRepository.GetByIdAsync(analysisId, ct);

        if (analysis == null)
            return NotFound();

        // Sicherheitscheck: nur Besitzer
        if (analysis.Video.UserId != userId)
            return Forbid();

        await _analysisRepository.DeleteAsync(analysis, ct);

        return NoContent(); // 204
    }

}
