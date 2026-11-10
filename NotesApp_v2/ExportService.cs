using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NotesApp_v2;

public static class ExportService
{
    public static void ExportNoteToTxt(Note note, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Название: {note.Title}");
        sb.AppendLine($"Категория: {note.CategoryName} | Приоритет: {note.Priority}");
        if (!string.IsNullOrWhiteSpace(note.Tags))
            sb.AppendLine($"Теги: {note.Tags}");
        sb.AppendLine($"Создано: {note.CreatedAt} | Обновлено: {note.UpdatedAt}");
        sb.AppendLine(new string('=', 45));
        sb.AppendLine();
        sb.AppendLine(note.Content);

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    public static void ExportAllToJson(List<Note> notes, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        string json = JsonSerializer.Serialize(notes, options);
        File.WriteAllText(filePath, json, Encoding.UTF8);
    }
}
