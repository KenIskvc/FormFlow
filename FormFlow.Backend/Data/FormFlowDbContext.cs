using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Backend.Data;

public class FormFlowDbContext : IdentityDbContext<IdentityUser> {
    public FormFlowDbContext(DbContextOptions<FormFlowDbContext> options)
        : base(options) {
    }
}