namespace DiaryApp.Services;

using DiaryApp.Data;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

public record RetrospectiveTier(string Label, List<DiaryEntry> Entries);

public class RetrospectiveService
{
    private readonly DiaryDbContext _db;

    public RetrospectiveService(DiaryDbContext db)
    {
        _db = db;
    }

    public async Task<List<RetrospectiveTier>> GetRetrospectiveAsync(DateTime referenceDate)
    {
        var tiers = new List<RetrospectiveTier>();

        var weekAgo = referenceDate.AddDays(-7);
        var weekAgoEntries = await _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .Where(e => e.Date.Year == weekAgo.Year && e.Date.Month == weekAgo.Month && e.Date.Day == weekAgo.Day)
            .ToListAsync();
        if (weekAgoEntries.Count > 0)
            tiers.Add(new RetrospectiveTier("7일 전", weekAgoEntries));

        var monthAgo = referenceDate.AddMonths(-1);
        var monthAgoEntries = await _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .Where(e => e.Date.Year == monthAgo.Year && e.Date.Month == monthAgo.Month && e.Date.Day == monthAgo.Day)
            .ToListAsync();
        if (monthAgoEntries.Count > 0)
            tiers.Add(new RetrospectiveTier("1개월 전", monthAgoEntries));

        var pastYearsEntries = await _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .Where(e => e.Date.Month == referenceDate.Month
                     && e.Date.Day == referenceDate.Day
                     && e.Date.Year < referenceDate.Year)
            .ToListAsync();

        foreach (var group in pastYearsEntries.GroupBy(e => e.Date.Year).OrderByDescending(g => g.Key))
        {
            var yearsAgo = referenceDate.Year - group.Key;
            tiers.Add(new RetrospectiveTier($"{yearsAgo}년 전", group.ToList()));
        }

        return tiers;
    }
}
