namespace DiaryApp.Tests;

using DiaryApp.Services;
using Xunit;

public class ImageStorageServiceTests : IDisposable
{
    private readonly string _uploadRoot;

    public ImageStorageServiceTests()
    {
        _uploadRoot = Path.Combine(Path.GetTempPath(), "DiaryAppTests_" + Guid.NewGuid().ToString("N"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_uploadRoot))
            Directory.Delete(_uploadRoot, recursive: true);
    }

    [Fact]
    public async Task SaveAsync_SavesFileAndReturnsRelativePath()
    {
        var service = new ImageStorageService(_uploadRoot);
        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        var relativePath = await service.SaveAsync(stream, "photo.jpg", stream.Length);

        Assert.StartsWith("uploads/", relativePath);
        var savedFileName = relativePath.Substring("uploads/".Length);
        Assert.True(File.Exists(Path.Combine(_uploadRoot, savedFileName)));
    }

    [Fact]
    public async Task SaveAsync_RejectsDisallowedExtension()
    {
        var service = new ImageStorageService(_uploadRoot);
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        await Assert.ThrowsAsync<ImageValidationException>(
            () => service.SaveAsync(stream, "document.pdf", stream.Length));
    }

    [Fact]
    public async Task SaveAsync_RejectsFileOverSizeLimit()
    {
        var service = new ImageStorageService(_uploadRoot);
        using var stream = new MemoryStream(new byte[10]);
        var overLimitSize = 5 * 1024 * 1024 + 1;

        await Assert.ThrowsAsync<ImageValidationException>(
            () => service.SaveAsync(stream, "big.jpg", overLimitSize));
    }
}
