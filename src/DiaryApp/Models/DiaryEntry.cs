namespace DiaryApp.Models;

public class DiaryEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Content { get; set; } = string.Empty;
    public int? Mood { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public List<DiaryImage> Images { get; set; } = new();
}
