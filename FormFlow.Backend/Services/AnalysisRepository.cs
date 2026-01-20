using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Backend.Services;


public class AnalysisRepository : IAnalysisRepository
{
    private readonly FormFlowDbContext _context;

    public AnalysisRepository(FormFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAnalaysis(Analysis analysis)
    {
        await _context.Analyses.AddAsync(analysis);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Analysis>> GetAnalysesForUserAsync(
        string userId,
        CancellationToken ct)
    {
        return await _context.Analyses
            .Include(a => a.Video)
            .Where(a => a.Video.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Analysis>> GetAllAnalysesAsync(CancellationToken ct)
    {
        return await _context.Analyses
            .Include(a => a.Video)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Analysis?> GetByIdAsync(
    int analysisId,
    CancellationToken ct)
    {
        return await _context.Analyses
            .Include(a => a.Video)
            .FirstOrDefaultAsync(a => a.Id == analysisId, ct);
    }

    public async Task DeleteAsync(
        Analysis analysis,
        CancellationToken ct)
    {
        _context.Analyses.Remove(analysis);
        await _context.SaveChangesAsync(ct);
    }

}