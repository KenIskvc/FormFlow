using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IAnalysisRepository {

    public Task AddAnalaysis(Analysis analysis);
}
