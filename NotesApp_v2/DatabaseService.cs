using Microsoft.Data.Sqlite;

namespace NotesApp_v2;

/// <summary>
/// Оптимизированный сервис работы с базой данных SQLite (Версия 2.0)
/// </summary>
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string? dbPath = null)
    {
        string resolvedPath = dbPath ?? Path.Combine(AppContext.BaseDirectory, "notes_v2.db");
        _connectionString = $"Data Source={resolvedPath}";
        InitializeDatabase();
    }

    private SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var pragma = conn.CreateCommand();
        pragma.CommandText = @"
            PRAGMA journal_mode = WAL;
            PRAGMA synchronous = NORMAL;
            PRAGMA foreign_keys = ON;
            PRAGMA cache_size = 10000;";
        pragma.ExecuteNonQuery();
        return conn;
    }

    private void InitializeDatabase()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Categories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                ColorHex TEXT NOT NULL DEFAULT '#4A90E2'
            );

            CREATE TABLE IF NOT EXISTS Notes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                CategoryId INTEGER NOT NULL,
                Priority TEXT NOT NULL DEFAULT 'Обычный',
                IsPinned INTEGER NOT NULL DEFAULT 0,
                IsArchived INTEGER NOT NULL DEFAULT 0,
                IsReadOnly INTEGER NOT NULL DEFAULT 0,
                Tags TEXT NOT NULL DEFAULT '',
                HasReminder INTEGER NOT NULL DEFAULT 0,
                ReminderAt TEXT NOT NULL DEFAULT '',
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE RESTRICT
            );

            CREATE INDEX IF NOT EXISTS idx_notes_cat_arch ON Notes (CategoryId, IsArchived);
            CREATE INDEX IF NOT EXISTS idx_notes_pinned_upd ON Notes (IsPinned DESC, UpdatedAt DESC);
            CREATE INDEX IF NOT EXISTS idx_notes_title ON Notes (Title);
        ";
        cmd.ExecuteNonQuery();

        // Мягкая миграция для ранее созданной БД
        try
        {
            using var alter = conn.CreateCommand();
            alter.CommandText = "ALTER TABLE Notes ADD COLUMN IsReadOnly INTEGER NOT NULL DEFAULT 0;";
            alter.ExecuteNonQuery();
        }
        catch { }

        // Начальные категории
        using var checkCat = conn.CreateCommand();
        checkCat.CommandText = "SELECT COUNT(*) FROM Categories;";
        int catCount = Convert.ToInt32(checkCat.ExecuteScalar());
        if (catCount == 0)
        {
            var defaultCats = new (string name, string color)[]
            {
                ("Работа", "#2B579A"),
                ("Учеба", "#217346"),
                ("Личное", "#8E44AD"),
                ("Идеи", "#D35400"),
                ("Срочно", "#C0392B")
            };
            using var tx = conn.BeginTransaction();
            foreach (var (name, color) in defaultCats)
            {
                using var ins = conn.CreateCommand();
                ins.Transaction = tx;
                ins.CommandText = "INSERT INTO Categories (Name, ColorHex) VALUES (@n, @c);";
                ins.Parameters.AddWithValue("@n", name);
                ins.Parameters.AddWithValue("@c", color);
                ins.ExecuteNonQuery();
            }
            tx.Commit();

            // Приветственная заметка
            AddNote(new Note
            {
                Title = "Добро пожаловать в Заметки v2.0",
                Content = "Это многофункциональная версия приложения заметок.\r\n\r\n" +
                          "Полезные функции:\r\n" +
                          "[x] Категории, теги, приоритеты и закрепление заметок\r\n" +
                          "[x] Чек-листы задач ([ ] / [x]) прямо внутри текста\r\n" +
                          "[x] Быстрая вставка даты и готовые шаблоны заметок\r\n" +
                          "[x] Защита заметки от случайных правок («Только чтение»)\r\n" +
                          "[x] Поиск, фильтрация, напоминания и Архив\r\n" +
                          "[x] Сохранение в папку Заметки (.txt) и бэкап в JSON\r\n" +
                          "[ ] Попробуйте нажать «+ Задача» или «Шаблон» выше!",
                CategoryId = 2,
                Priority = "Высокий",
                IsPinned = true,
                Tags = "#практика, #пм03, #старт"
            });
        }
    }

    public List<Category> GetCategories()
    {
        var list = new List<Category>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, ColorHex FROM Categories ORDER BY Id;";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Category
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                ColorHex = r.GetString(2)
            });
        }
        return list;
    }

    public int AddCategory(string name, string colorHex = "#607D8B")
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Categories (Name, ColorHex) VALUES (@n, @c); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@n", name.Trim());
        cmd.Parameters.AddWithValue("@c", colorHex);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<Note> FilterNotes(string search, int categoryId, string priorityFilter, bool showArchived)
    {
        var list = new List<Note>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();

        var sql = @"
            SELECT n.Id, n.Title, n.Content, n.CategoryId, c.Name,
                   n.Priority, n.IsPinned, n.IsArchived, n.IsReadOnly, n.Tags,
                   n.HasReminder, n.ReminderAt, n.CreatedAt, n.UpdatedAt
            FROM Notes n
            JOIN Categories c ON n.CategoryId = c.Id
            WHERE n.IsArchived = @arch ";

        cmd.Parameters.AddWithValue("@arch", showArchived ? 1 : 0);

        if (categoryId > 0)
        {
            sql += " AND n.CategoryId = @catId ";
            cmd.Parameters.AddWithValue("@catId", categoryId);
        }

        if (!string.IsNullOrWhiteSpace(priorityFilter) && priorityFilter != "Все")
        {
            sql += " AND n.Priority = @prio ";
            cmd.Parameters.AddWithValue("@prio", priorityFilter);
        }

        sql += " ORDER BY n.IsPinned DESC, n.Id DESC;";
        cmd.CommandText = sql;

        string query = search?.Trim() ?? string.Empty;
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var note = new Note
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Content = r.GetString(2),
                CategoryId = r.GetInt32(3),
                CategoryName = r.GetString(4),
                Priority = r.GetString(5),
                IsPinned = r.GetInt32(6) == 1,
                IsArchived = r.GetInt32(7) == 1,
                IsReadOnly = r.GetInt32(8) == 1,
                Tags = r.GetString(9),
                HasReminder = r.GetInt32(10) == 1,
                ReminderAt = r.GetString(11),
                CreatedAt = r.GetString(12),
                UpdatedAt = r.GetString(13)
            };

            if (!string.IsNullOrEmpty(query))
            {
                bool match = note.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || note.Content.Contains(query, StringComparison.OrdinalIgnoreCase)
                          || note.Tags.Contains(query, StringComparison.OrdinalIgnoreCase);
                if (!match) continue;
            }

            list.Add(note);
        }
        return list;
    }

    public int AddNote(Note note)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Notes (Title, Content, CategoryId, Priority, IsPinned, IsArchived, IsReadOnly, Tags, HasReminder, ReminderAt, CreatedAt, UpdatedAt)
            VALUES (@t, @c, @cat, @prio, @pin, @arch, @ro, @tags, @rem, @remAt, @cr, @up);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@t", note.Title);
        cmd.Parameters.AddWithValue("@c", note.Content);
        cmd.Parameters.AddWithValue("@cat", note.CategoryId);
        cmd.Parameters.AddWithValue("@prio", note.Priority);
        cmd.Parameters.AddWithValue("@pin", note.IsPinned ? 1 : 0);
        cmd.Parameters.AddWithValue("@arch", note.IsArchived ? 1 : 0);
        cmd.Parameters.AddWithValue("@ro", note.IsReadOnly ? 1 : 0);
        cmd.Parameters.AddWithValue("@tags", note.Tags ?? "");
        cmd.Parameters.AddWithValue("@rem", note.HasReminder ? 1 : 0);
        cmd.Parameters.AddWithValue("@remAt", note.ReminderAt ?? "");
        cmd.Parameters.AddWithValue("@cr", string.IsNullOrEmpty(note.CreatedAt) ? now : note.CreatedAt);
        cmd.Parameters.AddWithValue("@up", now);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void UpdateNote(Note note)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Notes
            SET Title = @t, Content = @c, CategoryId = @cat,
                Priority = @prio, IsPinned = @pin, IsArchived = @arch, IsReadOnly = @ro,
                Tags = @tags, HasReminder = @rem, ReminderAt = @remAt, UpdatedAt = @up
            WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", note.Id);
        cmd.Parameters.AddWithValue("@t", note.Title);
        cmd.Parameters.AddWithValue("@c", note.Content);
        cmd.Parameters.AddWithValue("@cat", note.CategoryId);
        cmd.Parameters.AddWithValue("@prio", note.Priority);
        cmd.Parameters.AddWithValue("@pin", note.IsPinned ? 1 : 0);
        cmd.Parameters.AddWithValue("@arch", note.IsArchived ? 1 : 0);
        cmd.Parameters.AddWithValue("@ro", note.IsReadOnly ? 1 : 0);
        cmd.Parameters.AddWithValue("@tags", note.Tags ?? "");
        cmd.Parameters.AddWithValue("@rem", note.HasReminder ? 1 : 0);
        cmd.Parameters.AddWithValue("@remAt", note.ReminderAt ?? "");
        cmd.Parameters.AddWithValue("@up", now);
        cmd.ExecuteNonQuery();
    }

    public void SetArchiveState(int noteId, bool archived)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Notes SET IsArchived = @a, IsPinned = CASE WHEN @a = 1 THEN 0 ELSE IsPinned END, UpdatedAt = @up WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", noteId);
        cmd.Parameters.AddWithValue("@a", archived ? 1 : 0);
        cmd.Parameters.AddWithValue("@up", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    public void SetPinState(int noteId, bool pinned)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Notes SET IsPinned = @p, UpdatedAt = @up WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", noteId);
        cmd.Parameters.AddWithValue("@p", pinned ? 1 : 0);
        cmd.Parameters.AddWithValue("@up", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    public void DeleteNotePermanently(int noteId)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Notes WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", noteId);
        cmd.ExecuteNonQuery();
    }

    public List<Note> GetDueReminders()
    {
        var list = new List<Note>();
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, Title, ReminderAt FROM Notes
            WHERE IsArchived = 0 AND HasReminder = 1 AND ReminderAt != '' AND ReminderAt <= @now;";
        cmd.Parameters.AddWithValue("@now", now);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Note
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                ReminderAt = r.GetString(2)
            });
        }
        return list;
    }

    public void ClearReminder(int noteId)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Notes SET HasReminder = 0 WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", noteId);
        cmd.ExecuteNonQuery();
    }
}
