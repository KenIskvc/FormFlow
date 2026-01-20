using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IAnalysisRepository
{
    Task AddAnalaysis(Analysis analysis);

    Task<List<Analysis>> GetAnalysesForUserAsync(
        string userId,
        CancellationToken ct);

    Task<List<Analysis>> GetAllAnalysesAsync(CancellationToken ct);

    Task<Analysis?> GetByIdAsync(int analysisId, CancellationToken ct);
    Task DeleteAsync(Analysis analysis, CancellationToken ct);


}
