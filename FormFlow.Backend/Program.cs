using FormFlow.Backend.Data;
using FormFlow.Backend.Models;
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
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<FormFlowUser>();

app.MapControllers();

app.Run();
