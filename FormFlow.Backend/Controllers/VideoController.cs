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
public class VideoController : ControllerBase
{
    private readonly IVideoRepository _videoRepository;
    private readonly IAnalysisRepository _analysisRepository;
    private readonly IPoseAnalysisServicecs _poseService;
    private readonly UserManager<FormFlowUser> _userManager;

    public VideoController(
        IVideoRepository videoRepository,
        IAnalysisRepository analysisRepository,
        IPoseAnalysisServicecs poseService,
        UserManager<FormFlowUser> userManager)
    {
        _videoRepository = videoRepository;
        _analysisRepository = analysisRepository;
        _poseService = poseService;
        _userManager = userManager;
    }

    /// <summary>
    /// Ruft alle Videos ab, die dem aktuell angemeldeten Benutzer gehören.
    /// </summary>
    /// <param name="ct">Integrierter Abbruchtoken für die asynchrone Operation.</param>
    /// <returns>Eine Liste von Video-Metadaten (Id und Dateiname) des Benutzers.</returns>
    
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

    /// <summary>
    /// Ruft eine Liste aller in der Datenbank gespeicherten Videos ab.
    /// </summary>
    /// <param name="ct">Integrierter Abbruchtoken für die asynchrone Operation.</param>
    /// <returns>Eine Liste aller Video-Entitäten.</returns>
    
    [HttpGet]
    public async Task<IActionResult> GetVideos(CancellationToken ct)
    {
        var videos = await _videoRepository.GetAll();
        return Ok(videos);
    }

    /// <summary>
    /// Verarbeitet einen Video-Upload, führt eine Pose-Analyse durch und speichert das Ergebnis optional für registrierte Benutzer.
    /// </summary>
    /// <param name="request">Das Formular-Objekt, das die Videodatei enthält.</param>
    /// <param name="ct">Integrierter Abbruchtoken für die asynchrone Operation.</param>
    /// <returns>Die Analyseergebnisse und bei angemeldeten Benutzern die Id des gespeicherten Videos.</returns>
    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        [FromForm] UploadRequest request,
        CancellationToken ct)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("No file uploaded.");

        // Video in Bytes lesen
        byte[] videoBytes;
        using (var ms = new MemoryStream())
        {
            await request.File.CopyToAsync(ms, ct);
            videoBytes = ms.ToArray();
        }


        var reportJson = await _poseService.AnalyzeAsync(
            videoBytes,
            request.File.FileName,
            ct
        );


        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var video = new Video
            {
                FileName = request.File.FileName,
                FileData = videoBytes,
                UserId = user.Id
            };

            await _videoRepository.AddVideoAsync(video);

            var analysis = new Analysis
            {
                VideoId = video.Id,
                CreatedAt = DateTime.UtcNow,
                Report = reportJson
            };

            await _analysisRepository.AddAnalaysis(analysis);

            return Ok(new UploadResponseDto
            {
                VideoId = video.Id,
                Analysis = new AnalysisDto
                {
                    CreatedAt = analysis.CreatedAt,
                    Report = analysis.Report
                }
            });
        }


        return Ok(new UploadResponseDto
        {
            Analysis = new AnalysisDto
            {
                CreatedAt = DateTime.UtcNow,
                Report = reportJson
            }
        });
    }

    /// <summary>
    /// Benennt eine vorhandene Videodatei um, sofern der Benutzer der Eigentümer ist.
    /// </summary>
    /// <param name="id">Die ID des umzubenennenden Videos.</param>
    /// <param name="newName">Der neue Dateiname.</param>
    /// <returns>Statuscode 200 bei Erfolg, 403 bei fehlender Berechtigung oder 404 falls nicht gefunden.</returns>
   
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


    /// <summary>
    /// Löscht ein Video aus der Datenbank, sofern der Benutzer der Eigentümer ist.
    /// </summary>
    /// <param name="id">Die ID des zu löschenden Videos.</param>
    /// <returns>Statuscode 204 (No Content) bei Erfolg.</returns>
   
    [Authorize]
    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
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