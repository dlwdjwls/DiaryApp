namespace DiaryApp.Models;

public class DiaryImage
{
    public int Id { get; set; }
    public int DiaryEntryId { get; set; }
    public DiaryEntry? DiaryEntry { get; set; }
    public string FilePath { get; set; } = string.Empty;
}
