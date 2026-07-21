namespace DiaryApp.Tests;

using DiaryApp.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DiaryEntryServiceTests
{
    [Fact]
    public async Task CreateAsync_SavesEntryWithNewTags()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);

        var entry = await service.CreateAsync(new DateTime(2026, 7, 20), "오늘은 날씨가 좋았다", 5, new List<string> { "행복", "일상" });

        Assert.NotEqual(0, entry.Id);
        Assert.Equal(2, entry.Tags.Count);
    }

    [Fact]
    public async Task CreateAsync_ReusesExistingTag()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);

        await service.CreateAsync(new DateTime(2026, 7, 19), "첫 일기", null, new List<string> { "일상" });
        await service.CreateAsync(new DateTime(2026, 7, 20), "둘째 일기", null, new List<string> { "일상" });

        Assert.Equal(1, await db.Context.Tags.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_ThrowsWhenContentIsEmpty()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(new DateTime(2026, 7, 20), "  ", null, new List<string>()));
    }

    [Fact]
    public async Task GetAllDescendingAsync_OrdersByDateDescending()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);

        await service.CreateAsync(new DateTime(2026, 7, 18), "오래된 글", null, new List<string>());
        await service.CreateAsync(new DateTime(2026, 7, 20), "최신 글", null, new List<string>());

        var all = await service.GetAllDescendingAsync();

        Assert.Equal("최신 글", all[0].Content);
        Assert.Equal("오래된 글", all[1].Content);
    }

    [Fact]
    public async Task AddImageAsync_AttachesImageToExistingEntry()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);
        var entry = await service.CreateAsync(new DateTime(2026, 7, 20), "사진이 있는 글", null, new List<string>());

        await service.AddImageAsync(entry.Id, "uploads/abc.jpg");

        var reloaded = await service.GetByIdAsync(entry.Id);
        Assert.NotNull(reloaded);
        Assert.Single(reloaded!.Images);
        Assert.Equal("uploads/abc.jpg", reloaded.Images[0].FilePath);
    }

    [Fact]
    public async Task AddImageAsync_ThrowsWhenEntryDoesNotExist()
    {
        using var db = new TestDb();
        var service = new DiaryEntryService(db.Context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddImageAsync(999, "uploads/x.jpg"));
    }
}
