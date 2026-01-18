using FormFlow.Backend.Contracts;

namespace FormFlow.Backend.Services;

public class PoseAnalysisService : IPoseAnalysisServicecs {
    private readonly HttpClient _client;

    public PoseAnalysisService(HttpClient client) => _client = client;

    public async Task<string> AnalyzeAsync(byte[] videoData, string fileName, CancellationToken ct) {
        using var content = new MultipartFormDataContent {
            {
                new ByteArrayContent(videoData),
                "file",
                fileName
            }
        };

        var response = await _client.PostAsync("/analyze", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
