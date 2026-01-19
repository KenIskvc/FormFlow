namespace FormFlow.Backend.Contracts;

public interface IPoseAnalysisServicecs {

    public Task<string> AnalyzeAsync(byte[] videoData, string fileName, CancellationToken ct);
}
