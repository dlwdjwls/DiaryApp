namespace DiaryApp.Services;

public class ImageValidationException : Exception
{
    public ImageValidationException(string message) : base(message) { }
}

public class ImageStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp"
    };
    private const long MaxSizeBytes = 5 * 1024 * 1024;

    private readonly string _uploadRoot;

    public ImageStorageService(string uploadRoot)
    {
        _uploadRoot = uploadRoot;
    }

    public async Task<string> SaveAsync(Stream fileStream, string originalFileName, long fileSizeBytes)
    {
        var extension = Path.GetExtension(originalFileName);
        if (!AllowedExtensions.Contains(extension))
            throw new ImageValidationException($"지원하지 않는 이미지 형식입니다: {extension}");

        if (fileSizeBytes > MaxSizeBytes)
            throw new ImageValidationException("이미지 용량은 5MB를 초과할 수 없습니다.");

        Directory.CreateDirectory(_uploadRoot);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(_uploadRoot, fileName);

        using (var output = File.Create(fullPath))
        {
            await fileStream.CopyToAsync(output);
        }

        return $"uploads/{fileName}";
    }
}
