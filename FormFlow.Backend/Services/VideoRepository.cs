using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Backend.Services;

public class VideoRepository : IVideoRepository
{
    private readonly FormFlowDbContext _context;

    // Fix für CS8618 & CS0649: Konstruktor zur Initialisierung des Contexts
    public VideoRepository(FormFlowDbContext context)
    {
        _context = context;
    }

    public async Task<Video?> GetVideoAsync(int id) =>
        await _context.Videos.Include(v => v.Analyses).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId) =>
        await _context.Videos.Where(v => v.UserId == userId).ToListAsync();

    public async Task AddVideoAsync(Video video)
    {
        await _context.Videos.AddAsync(video);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVideoAsync(Video video)
    {
        _context.Entry(video).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVideoAsync(Video video)
    {
        _context.Videos.Remove(video);
        await _context.SaveChangesAsync();
    }
}