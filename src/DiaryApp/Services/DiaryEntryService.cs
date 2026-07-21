namespace DiaryApp.Services;

using DiaryApp.Data;
using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;

public class DiaryEntryService
{
    private readonly DiaryDbContext _db;

    public DiaryEntryService(DiaryDbContext db)
    {
        _db = db;
    }

    public async Task<DiaryEntry> CreateAsync(DateTime date, string content, int? mood, List<string> tagNames)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("본문은 비어 있을 수 없습니다.", nameof(content));

        var entry = new DiaryEntry
        {
            Date = date,
            Content = content,
            Mood = mood,
            CreatedAt = DateTime.Now,
        };

        foreach (var name in tagNames.Select(t => t.Trim()).Where(t => t.Length > 0).Distinct())
        {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == name);
            if (tag == null)
            {
                tag = new Tag { Name = name };
                _db.Tags.Add(tag);
            }
            entry.Tags.Add(tag);
        }

        _db.DiaryEntries.Add(entry);
        await _db.SaveChangesAsync();
        return entry;
    }

    public async Task<List<DiaryEntry>> GetAllDescendingAsync()
    {
        return await _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<DiaryEntry?> GetByIdAsync(int id)
    {
        return await _db.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddImageAsync(int entryId, string filePath)
    {
        var entry = await _db.DiaryEntries.FindAsync(entryId);
        if (entry == null)
            throw new InvalidOperationException($"Id {entryId}에 해당하는 일기 항목이 없습니다.");
        _db.DiaryImages.Add(new DiaryImage { DiaryEntryId = entryId, FilePath = filePath });
        await _db.SaveChangesAsync();
    }
}
