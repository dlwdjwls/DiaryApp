namespace DiaryApp.Services;

using DiaryApp.Data;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

public class SearchService
{
    private readonly DiaryDbContext _db;

    public SearchService(DiaryDbContext db)
    {
        _db = db;
    }

    public async Task<List<DiaryEntry>> SearchAsync(string? keyword, List<string>? tagNames)
    {
        var query = _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(e => e.Content.Contains(keyword));
        }

        if (tagNames != null && tagNames.Count > 0)
        {
            query = query.Where(e => e.Tags.Any(t => tagNames.Contains(t.Name)));
        }

        return await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllTagNamesAsync()
    {
        return await _db.Tags.OrderBy(t => t.Name).Select(t => t.Name).ToListAsync();
    }
}
