using FormFlow.Backend.Contracts;
using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
using FormFlow.Backend.Repositories;
using FormFlow.Backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DB (PostgreSQL)
builder.Services.AddDbContext<FormFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (no UI) + API endpoints
builder.Services.AddIdentityCore<FormFlowUser>()
       .AddEntityFrameworkStores<FormFlowDbContext>()
       .AddApiEndpoints();

builder.Services.AddHttpClient("PoseDetectionClient", client => {
    client.BaseAddress = new Uri("http://10.123.177.51:8000");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddTransient(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("PoseDetectionClient"));

builder.Services.Configure<IdentityOptions>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

// Token auth for mobile/API clients
builder.Services.AddAuthentication()
       .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddScoped<IPoseAnalysisServicecs, PoseAnalysisService>();
builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<FormFlowUser>();

app.MapControllers();

app.Run();
