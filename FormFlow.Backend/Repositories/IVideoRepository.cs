using FormFlow.Backend.Models;

namespace FormFlow.Backend.Repositories;

public interface IVideoRepository {

    public Task<Video?> GetVideoAsync(int videoId);
}
