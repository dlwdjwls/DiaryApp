using DiaryApp.Components;
using DiaryApp.Data;
using DiaryApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "diary.db");
builder.Services.AddDbContext<DiaryDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var uploadRoot = Path.Combine(builder.Environment.WebRootPath, "uploads");
builder.Services.AddScoped<DiaryEntryService>();
builder.Services.AddScoped<RetrospectiveService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddSingleton(new ImageStorageService(uploadRoot));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DiaryDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
