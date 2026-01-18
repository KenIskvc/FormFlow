using FormFlow.Backend.Contracts;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

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

        var analysis = new Analysis {
            VideoId = videoId,
            CreatedAt = DateTime.UtcNow,
            Report = reportAsJson
        };

        await _analysisRepository.AddAnalaysis(analysis);

        return Ok(analysis);
    }

}
