using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;

namespace FormFlow.Backend.Services;

public class VideoRepository : IVideoRepository {

    private readonly FormFlowDbContext _context;

    public VideoRepository(FormFlowDbContext context) => _context = context;

    public async Task<Video?> GetVideoAsync(int videoId) {
        var video = await _context.Videos.FindAsync(videoId);
        return video ?? null;
    }
}
