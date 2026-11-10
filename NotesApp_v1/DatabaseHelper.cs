using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace NotesApp_v1;

/// <summary>
/// Базовый класс работы с БД SQLite (Версия 1.0 — без оптимизаций транзакций и индексов)
/// </summary>
public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string? dbPath = null)
    {
        string resolvedPath = dbPath ?? Path.Combine(AppContext.BaseDirectory, "notes_v1.db");
        _connectionString = $"Data Source={resolvedPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );";
        cmd.ExecuteNonQuery();
    }

    public List<Note> GetAllNotes()
    {
        var list = new List<Note>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Title, Content, CreatedAt FROM Notes ORDER BY Id DESC;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Note
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Content = reader.GetString(2),
                CreatedAt = reader.GetString(3)
            });
        }
        return list;
    }

    public int AddNote(string title, string content)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Notes (Title, Content, CreatedAt)
            VALUES (@title, @content, @createdAt);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@content", content);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void UpdateNote(int id, string title, string content)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Notes
            SET Title = @title, Content = @content
            WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@content", content);
        cmd.ExecuteNonQuery();
    }

    public void DeleteNote(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Notes WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Простой вычислительный эксперимент v1.0:
    /// Построчная вставка без транзакций и поиск полным перебором без индексов.
    /// </summary>
    public (double insertMs, double searchMs, int foundCount) RunSimpleBenchmark(int count = 500)
    {
        string testDb = "benchmark_v1.db";
        if (File.Exists(testDb)) File.Delete(testDb);

        var sw = new Stopwatch();
        using var conn = new SqliteConnection($"Data Source={testDb}");
        conn.Open();

        using (var initCmd = conn.CreateCommand())
        {
            initCmd.CommandText = @"
                CREATE TABLE NotesTest (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT,
                    Content TEXT,
                    CreatedAt TEXT
                );";
            initCmd.ExecuteNonQuery();
        }

        // 1. Вставка по одной записи без транзакции (базовый неоптимизированный метод)
        sw.Start();
        for (int i = 0; i < count; i++)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO NotesTest (Title, Content, CreatedAt) VALUES (@t, @c, @d);";
            cmd.Parameters.AddWithValue("@t", $"Заметка #{i}");
            cmd.Parameters.AddWithValue("@c", $"Тестовое содержимое заметки номер {i}");
            cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();
        }
        sw.Stop();
        double insertMs = sw.Elapsed.TotalMilliseconds;

        // 2. Поиск без индекса (Full Table Scan)
        sw.Restart();
        int found = 0;
        using (var searchCmd = conn.CreateCommand())
        {
            searchCmd.CommandText = "SELECT COUNT(*) FROM NotesTest WHERE Title = @target;";
            searchCmd.Parameters.AddWithValue("@target", $"Заметка #{count / 2}");
            found = Convert.ToInt32(searchCmd.ExecuteScalar());
        }
        sw.Stop();
        double searchMs = sw.Elapsed.TotalMilliseconds;

        conn.Close();
        SqliteConnection.ClearAllPools();
        if (File.Exists(testDb)) File.Delete(testDb);

        return (insertMs, searchMs, found);
    }
}
