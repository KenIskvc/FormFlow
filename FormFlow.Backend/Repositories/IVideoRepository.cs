using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IVideoRepository {
    Task<List<Video>> GetAll();
    Task<Video?> GetVideoAsync(int id);
    Task<IEnumerable<Video>> GetVideosByUserIdAsync(string userId);
    Task AddVideoAsync(Video video);
    Task UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(Video video);
}