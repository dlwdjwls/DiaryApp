namespace DiaryApp.Tests;

using DiaryApp.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DiaryDbContextTests
{
    [Fact]
    public async Task SavesAndReadsEntryWithTagsAndImages()
    {
        using var db = new TestDb();

        var entry = new DiaryEntry
        {
            Date = new DateTime(2026, 7, 20),
            Content = "테스트 일기 본문",
            Mood = 4,
            CreatedAt = DateTime.Now,
        };
        entry.Tags.Add(new Tag { Name = "일상" });
        entry.Images.Add(new DiaryImage { FilePath = "uploads/test.jpg" });

        db.Context.DiaryEntries.Add(entry);
        await db.Context.SaveChangesAsync();

        var reloaded = await db.Context.DiaryEntries
            .Include(e => e.Tags)
            .Include(e => e.Images)
            .SingleAsync(e => e.Id == entry.Id);

        Assert.Equal("테스트 일기 본문", reloaded.Content);
        Assert.Equal(4, reloaded.Mood);
        Assert.Single(reloaded.Tags);
        Assert.Equal("일상", reloaded.Tags[0].Name);
        Assert.Single(reloaded.Images);
    }

    [Fact]
    public async Task TagNameIsUnique()
    {
        using var db = new TestDb();

        db.Context.Tags.Add(new Tag { Name = "행복" });
        await db.Context.SaveChangesAsync();

        db.Context.Tags.Add(new Tag { Name = "행복" });

        await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.Context.SaveChangesAsync());
    }
}
