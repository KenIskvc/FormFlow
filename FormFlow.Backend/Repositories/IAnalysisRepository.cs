using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IAnalysisRepository
{
    Task AddAnalaysis(Analysis analysis);

    Task<List<Analysis>> GetAnalysesForUserAsync(
        string userId,
        CancellationToken ct);

    Task<List<Analysis>> GetAllAnalysesAsync(CancellationToken ct);

}
