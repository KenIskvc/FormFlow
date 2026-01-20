using System.Net.Http.Headers;
using System.Net.Http.Json;
using FormFlow.MobileApp.Contracts;
using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp.Services;

public class AnalysisApi : IAnalysisApi
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    // Creates a new API service instance with all required dependencies.
    // This class is the single place responsible for making HTTP calls
    // related to analyses.
    public AnalysisApi(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    // Retrieves all persisted analyses for the currently authenticated user.
    // If no access token is available, an empty list is returned.
    // This method maps the backend JSON response into AnalysisResponseDto objects.
    public async Task<IReadOnlyList<AnalysisResponseDto>> GetMyAnalysesAsync(
    CancellationToken ct)
    {
        var accessToken = await _tokenStore.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
            return Array.Empty<AnalysisResponseDto>();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync("api/analysis", ct);

        if (!response.IsSuccessStatusCode)
            return Array.Empty<AnalysisResponseDto>();

        var result =
            await response.Content.ReadFromJsonAsync<List<AnalysisResponseDto>>(ct);

        return result is null
            ? Array.Empty<AnalysisResponseDto>()
            : result;
    }

    // Uploads a video stream to the backend and starts a new analysis.
    // This method sends the video as multipart/form-data and returns
    // the analysis result as an AnalysisResponseDto.
    public async Task<AnalysisResponseDto> AnalyzeAsync(
    Stream videoStream,
    string fileName,
    CancellationToken ct)
    {
        using var content = new MultipartFormDataContent();

        var streamContent = new StreamContent(videoStream);
        streamContent.Headers.ContentType =
            new MediaTypeHeaderValue("video/mp4");

        content.Add(streamContent, "file", fileName);

        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await _httpClient.PostAsync("analyze", content, ct);
        response.EnsureSuccessStatusCode();

        var dto =
            await response.Content.ReadFromJsonAsync<AnalysisResponseDto>(ct);

        if (dto == null)
            throw new InvalidOperationException("Invalid analysis response.");

        return dto;
    }

    // Deletes a persisted analysis on the backend by its ID.
    public async Task DeleteAnalysisAsync(
        int analysisId,
        CancellationToken ct)
    {
        var accessToken = await _tokenStore.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
            throw new InvalidOperationException("User not authenticated.");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.DeleteAsync(
            $"api/analysis/{analysisId}",
            ct);

        response.EnsureSuccessStatusCode();
    }

}
