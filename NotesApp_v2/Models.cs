namespace NotesApp_v2;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#4A90E2";

    public override string ToString() => Name;
}

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Priority { get; set; } = "Обычный"; // Обычный, Средний, Высокий
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public bool IsReadOnly { get; set; }
    public string Tags { get; set; } = string.Empty;
    public bool HasReminder { get; set; }
    public string ReminderAt { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}
