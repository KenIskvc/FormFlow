using FormFlow.Backend.Contracts;
using FormFlow.Backend.DTOs;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormFlow.Backend.Controllers;

[ApiController]
[Route("api/video")]
public class VideoController : ControllerBase {
    private readonly IVideoRepository _videoRepository;
    private readonly IAnalysisRepository _analysisRepository;
    private readonly IPoseAnalysisServicecs _poseService;
    private readonly UserManager<FormFlowUser> _userManager;

    public VideoController(
        IVideoRepository videoRepository,
        IAnalysisRepository analysisRepository,
        IPoseAnalysisServicecs poseService,
        UserManager<FormFlowUser> userManager) {
        _videoRepository = videoRepository;
        _analysisRepository = analysisRepository;
        _poseService = poseService;
        _userManager = userManager;
    }
    // =========================
    // GET USER VIDEOS (Must 09)
    // =========================
    [Authorize]
    [HttpGet("my-videos")]
    public async Task<IActionResult> GetMyVideos(CancellationToken ct)
    {
        // 1. User identifizieren
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var videos = await _videoRepository.GetVideosByUserIdAsync(user.Id);

        var response = videos.Select(v => new {
            v.Id,
            v.FileName,
        });

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetVideos(CancellationToken ct) {
        var videos = await _videoRepository.GetAll();
        return Ok(videos);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] UploadRequest request,
        CancellationToken ct) {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file uploaded.");

        // Video in Bytes lesen
        byte[] videoBytes;
        using (var ms = new MemoryStream()) {
            await request.File.CopyToAsync(ms, ct);
            videoBytes = ms.ToArray();
        }

        // -------------------------
        // Analyse IMMER ausführen
        // -------------------------
        var reportJson = await _poseService.AnalyzeAsync(
            videoBytes,
            request.File.FileName,
            ct
        );

        // -------------------------
        // Eingeloggt → speichern
        // -------------------------
        if (User.Identity?.IsAuthenticated == true) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var video = new Video {
                FileName = request.File.FileName,
                FileData = videoBytes,
                UserId = user.Id
            };

            await _videoRepository.AddVideoAsync(video);

            var analysis = new Analysis {
                VideoId = video.Id,
                CreatedAt = DateTime.UtcNow,
                Report = reportJson
            };

            await _analysisRepository.AddAnalaysis(analysis);

            return Ok(new UploadResponseDto {
                VideoId = video.Id,
                Analysis = new AnalysisDto {
                    CreatedAt = analysis.CreatedAt,
                    Report = analysis.Report
                }
            });
        }


        // -------------------------
        // Gast → nur Analyse zurück
        // -------------------------
        return Ok(new UploadResponseDto {
            Analysis = new AnalysisDto {
                CreatedAt = DateTime.UtcNow,
                Report = reportJson
            }
        });
    }

    // =========================
    // RENAME
    // =========================
    [Authorize]
    [HttpPut("rename/{id:int}")]
    public async Task<IActionResult> Rename(int id, [FromBody] string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return BadRequest("Invalid name.");

        var video = await _videoRepository.GetVideoAsync(id);
        if (video == null)
            return NotFound();

        var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
        if (video.UserId != user!.Id)
            return Forbid();

        await _videoRepository.UpdateVideoNameAsync(id, newName);

        return Ok();
    }


    // =========================
    // DELETE
    // =========================
    [Authorize]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id) {
        var video = await _videoRepository.GetVideoAsync(id);
        if (video == null)
            return NotFound();

        var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
        if (video.UserId != user!.Id)
            return Forbid();

        await _videoRepository.DeleteVideoAsync(video);
        return NoContent();
    }
}
