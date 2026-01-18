using Microsoft.AspNetCore.Identity;

namespace FormFlow.Backend.Models;

public class FormFlowUser : IdentityUser {
    public virtual ICollection<Video>? Videos { get; set; } = [];
}
