using FormFlow.Backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DB (PostgreSQL)
builder.Services.AddDbContext<FormFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (no UI) + API endpoints
builder.Services.AddIdentityCore<IdentityUser>()
       .AddEntityFrameworkStores<FormFlowDbContext>()
       .AddApiEndpoints();

// Token auth for mobile/API clients
builder.Services.AddAuthentication()
       .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();

app.MapControllers();

app.Run();
