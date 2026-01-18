using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;

namespace FormFlow.Backend.Services;

public class AnalysisRepository : IAnalysisRepository {
    private readonly FormFlowDbContext? _context;

    public AnalysisRepository(FormFlowDbContext? context) => _context = context;

    public async Task AddAnalaysis(Analysis analysis) {
        await _context.Analyses.AddAsync(analysis);
        await _context.SaveChangesAsync();
    }
}
