namespace DiaryApp.Tests;

using DiaryApp.Services;
using Xunit;

public class RetrospectiveServiceTests
{
    [Fact]
    public async Task ReturnsEntryFromSevenDaysAgo()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var retroService = new RetrospectiveService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 7, 13), "일주일 전 기록", null, new List<string>());

        var tiers = await retroService.GetRetrospectiveAsync(new DateTime(2026, 7, 20));

        var weekTier = Assert.Single(tiers, t => t.Label == "7일 전");
        Assert.Single(weekTier.Entries);
        Assert.Equal("일주일 전 기록", weekTier.Entries[0].Content);
    }

    [Fact]
    public async Task ReturnsEntryFromOneMonthAgo()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var retroService = new RetrospectiveService(db.Context);

        await entryService.CreateAsync(new DateTime(2026, 6, 20), "한달 전 기록", null, new List<string>());

        var tiers = await retroService.GetRetrospectiveAsync(new DateTime(2026, 7, 20));

        var monthTier = Assert.Single(tiers, t => t.Label == "1개월 전");
        Assert.Single(monthTier.Entries);
    }

    [Fact]
    public async Task ReturnsEntriesFromMultipleYearsAgoOrderedNewestFirst()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var retroService = new RetrospectiveService(db.Context);

        await entryService.CreateAsync(new DateTime(2024, 7, 20), "2년 전 기록", null, new List<string>());
        await entryService.CreateAsync(new DateTime(2025, 7, 20), "1년 전 기록", null, new List<string>());

        var tiers = await retroService.GetRetrospectiveAsync(new DateTime(2026, 7, 20));

        var labels = tiers.Select(t => t.Label).ToList();
        Assert.Equal(new[] { "1년 전", "2년 전" }, labels);
    }

    [Fact]
    public async Task SkipsTiersWithNoMatchingEntries()
    {
        using var db = new TestDb();
        var retroService = new RetrospectiveService(db.Context);

        var tiers = await retroService.GetRetrospectiveAsync(new DateTime(2026, 7, 20));

        Assert.Empty(tiers);
    }

    [Fact]
    public async Task HandlesLeapDayReferenceCorrectly()
    {
        using var db = new TestDb();
        var entryService = new DiaryEntryService(db.Context);
        var retroService = new RetrospectiveService(db.Context);

        await entryService.CreateAsync(new DateTime(2024, 2, 29), "윤년 기록", null, new List<string>());

        var tiers = await retroService.GetRetrospectiveAsync(new DateTime(2028, 2, 29));

        var yearTier = Assert.Single(tiers, t => t.Label == "4년 전");
        Assert.Single(yearTier.Entries);
    }
}
