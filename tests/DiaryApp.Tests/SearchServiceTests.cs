namespace DiaryApp.Tests;

using DiaryApp.Services;
using Xunit;

public class SearchServiceTests
{
    [Fact]
    public async Task SearchAsync_FiltersByKeyword()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var searchService = new SearchService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 7, 20), "오늘은 카페에 갔다", null, new List<string>());
        await entryService.CreateAsync(new DateTime(2026, 7, 19), "집에서 쉬었다", null, new List<string>());

        var results = await searchService.SearchAsync("카페", null);

        Assert.Single(results);
        Assert.Equal("오늘은 카페에 갔다", results[0].Content);
    }

    [Fact]
    public async Task SearchAsync_FiltersByTagUsingOrLogic()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var searchService = new SearchService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 7, 20), "글1", null, new List<string> { "행복" });
        await entryService.CreateAsync(new DateTime(2026, 7, 19), "글2", null, new List<string> { "피곤" });
        await entryService.CreateAsync(new DateTime(2026, 7, 18), "글3", null, new List<string> { "슬픔" });

        var results = await searchService.SearchAsync(null, new List<string> { "행복", "피곤" });

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAllOrderedByDateDescending()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var searchService = new SearchService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 7, 18), "오래된 글", null, new List<string>());
        await entryService.CreateAsync(new DateTime(2026, 7, 20), "최신 글", null, new List<string>());

        var results = await searchService.SearchAsync(null, null);

        Assert.Equal(2, results.Count);
        Assert.Equal("최신 글", results[0].Content);
    }

    [Fact]
    public async Task GetAllTagNamesAsync_ReturnsSortedNames()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var searchService = new SearchService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 7, 20), "글1", null, new List<string> { "행복", "일상" });
        await entryService.CreateAsync(new DateTime(2026, 7, 19), "글2", null, new List<string> { "일상" });

        var tags = await searchService.GetAllTagNamesAsync();

        Assert.Equal(new[] { "일상", "행복" }, tags);
    }
}
