using System.Net.Http.Headers;
using System.Net.Http.Json;
using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp.Services;

public class VideoService
{
    private readonly HttpClient _httpClient;

    public VideoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ======================
    // GET MY VIDEOS
    // ======================
    public async Task<List<VideoListDto>> GetMyVideosAsync(string token)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/video/my-videos");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new();

        return await response.Content
            .ReadFromJsonAsync<List<VideoListDto>>() ?? new();
    }

    // ======================
    // UPLOAD + ANALYZE
    // ======================
    public async Task<UploadResultDto> UploadAndAnalyzeAsync(
        FileResult video,
        string? token,
        CancellationToken ct)
    {
        if (video == null)
            throw new ArgumentNullException(nameof(video));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "api/video/upload");

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        using var fileStream = await video.OpenReadAsync();
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms, ct);

        var content = new MultipartFormDataContent();

        var byteContent = new ByteArrayContent(ms.ToArray());
        byteContent.Headers.ContentType =
            new MediaTypeHeaderValue(video.ContentType ?? "application/octet-stream");

        content.Add(byteContent, "File", video.FileName);

        request.Content = content;

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new Exception($"Upload fehlgeschlagen: {error}");
        }

        return await response.Content
            .ReadFromJsonAsync<UploadResultDto>(cancellationToken: ct)
            ?? throw new Exception("Ungültige Serverantwort");
    }

    // ======================
    // DELETE
    // ======================
    public async Task DeleteVideoAsync(int id, string token)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/video/delete/{id}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Delete fehlgeschlagen");
    }

    // ======================
    // RENAME
    // ======================
    public async Task RenameVideoAsync(int id, string newName, string token)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"api/video/rename/{id}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(newName);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Rename fehlgeschlagen");
    }

    // ======================
    // RE-ANALYZE
    // ======================
    public async Task<AnalysisResponseDto> ReAnalyzeAsync(
        int videoId,
        string token,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/analyze/{videoId}");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Re-Analyse fehlgeschlagen");

        return await response.Content
            .ReadFromJsonAsync<AnalysisResponseDto>(cancellationToken: ct)
            ?? throw new Exception("Ungültige Analyse-Antwort");
    }
}
