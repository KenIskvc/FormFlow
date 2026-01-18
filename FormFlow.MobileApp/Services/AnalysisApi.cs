using System.Net.Http.Headers;
using System.Net.Http.Json;
using FormFlow.MobileApp.Contracts;
using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp.Services;

public class AnalysisApi : IAnalysisApi
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    public AnalysisApi(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

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
}
