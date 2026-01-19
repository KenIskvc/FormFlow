using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Backend.Services;

public class VideoRepository : IVideoRepository {
    private readonly FormFlowDbContext _context;

    public VideoRepository(FormFlowDbContext context) => _context = context;

    public async Task<List<Video>> GetAll() => await _context.Videos.ToListAsync();

    public async Task<Video?> GetVideoAsync(int id) =>
        await _context.Videos.Include(v => v.Analyses).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId) =>
        await _context.Videos.Where(v => v.UserId == userId).ToListAsync();

    public async Task AddVideoAsync(Video video) {
        await _context.Videos.AddAsync(video);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateVideoNameAsync(int videoId, string newName)
    {
        await _context.Videos
            .Where(v => v.Id == videoId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(v => v.FileName, newName));
    }

    public async Task DeleteVideoAsync(Video video) {
        _context.Videos.Remove(video);
        await _context.SaveChangesAsync();
    }

}