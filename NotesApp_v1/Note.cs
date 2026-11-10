namespace NotesApp_v1;

/// <summary>
/// Простая модель заметки (Версия 1.0)
/// </summary>
public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Title) ? "(Без названия)" : Title;
    }
}
