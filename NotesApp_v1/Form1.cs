using System.Diagnostics;
using System.Text;

namespace NotesApp_v1;

public partial class Form1 : Form
{
    private readonly DatabaseHelper _db;
    private readonly string _notesFolder;
    private Note? _selectedNote;

    public Form1()
    {
        InitializeComponent();
        _db = new DatabaseHelper();
        _notesFolder = Path.Combine(Application.StartupPath, "Заметки");
        Directory.CreateDirectory(_notesFolder);
        LoadNotes();
    }

    private void LoadNotes()
    {
        lstNotes.Items.Clear();
        var notes = _db.GetAllNotes();
        foreach (var note in notes)
        {
            lstNotes.Items.Add(note);
        }

        if (lstNotes.Items.Count > 0 && _selectedNote == null)
        {
            lstNotes.SelectedIndex = 0;
        }
        else if (lstNotes.Items.Count == 0)
        {
            ClearEditor();
        }
    }

    private void ClearEditor()
    {
        _selectedNote = null;
        txtTitle.Text = string.Empty;
        txtContent.Text = string.Empty;
        lblDate.Text = "Новая заметка";
        lblStatus.Text = string.Empty;
    }

    private void lstNotes_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstNotes.SelectedItem is Note note)
        {
            _selectedNote = note;
            txtTitle.Text = note.Title;
            txtContent.Text = note.Content;
            lblDate.Text = $"Создано: {note.CreatedAt}";
            lblStatus.Text = string.Empty;
        }
    }

    private void btnNew_Click(object? sender, EventArgs e)
    {
        lstNotes.ClearSelected();
        ClearEditor();
        txtTitle.Focus();
    }

    private string BuildNoteFileText(string title, string content)
    {
        return $"Название: {title}\r\nДата: {DateTime.Now:dd.MM.yyyy HH:mm}\r\n{new string('-', 40)}\r\n{content}";
    }

    private string GetNoteFilePath(string title)
    {
        string safeName = string.Join("_", title.Split(Path.GetInvalidFileNameChars())).Trim();
        if (string.IsNullOrWhiteSpace(safeName)) safeName = "Заметка";
        return Path.Combine(_notesFolder, $"{safeName}.txt");
    }

    private void DeleteNoteFilesFromFolder(int id, string? oldTitle = null)
    {
        if (!Directory.Exists(_notesFolder)) return;
        if (!string.IsNullOrWhiteSpace(oldTitle))
        {
            string oldPath = GetNoteFilePath(oldTitle);
            try { if (File.Exists(oldPath)) File.Delete(oldPath); } catch { }
        }
        foreach (string file in Directory.GetFiles(_notesFolder, $"{id}_*.txt"))
        {
            try { File.Delete(file); } catch { }
        }
    }

    private string SaveTxtCopyToFolder(int id, string title, string content, string? oldTitle = null)
    {
        Directory.CreateDirectory(_notesFolder);
        DeleteNoteFilesFromFolder(id, oldTitle);
        string filePath = GetNoteFilePath(title);
        File.WriteAllText(filePath, BuildNoteFileText(title, content), Encoding.UTF8);
        return filePath;
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        string title = txtTitle.Text.Trim();
        string content = txtContent.Text;

        if (string.IsNullOrEmpty(title))
        {
            MessageBox.Show("Введите заголовок заметки!", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTitle.Focus();
            return;
        }

        int savedId;
        string? oldTitle = null;
        if (_selectedNote == null)
        {
            savedId = _db.AddNote(title, content);
        }
        else
        {
            savedId = _selectedNote.Id;
            oldTitle = _selectedNote.Title;
            _db.UpdateNote(savedId, title, content);
        }

        string txtPath = SaveTxtCopyToFolder(savedId, title, content, oldTitle);
        LoadNotes();
        SelectNoteById(savedId);
        lblStatus.Text = $"Сохранено в Заметки\\{Path.GetFileName(txtPath)}";
    }

    private void btnOpenNote_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null)
        {
            MessageBox.Show("Сначала выберите заметку из списка или сохраните текущую.", "Открытие заметки",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Directory.CreateDirectory(_notesFolder);
        string existingFile = GetNoteFilePath(_selectedNote.Title);
        if (!File.Exists(existingFile))
        {
            existingFile = SaveTxtCopyToFolder(_selectedNote.Id, _selectedNote.Title, _selectedNote.Content);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = existingFile,
            UseShellExecute = true
        });
        lblStatus.Text = $"Открыта заметка: {Path.GetFileName(existingFile)}";
    }

    private void btnOpenFolder_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(_notesFolder);
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"\"{_notesFolder}\"",
            UseShellExecute = true
        });
    }

    private void SelectNoteById(int id)
    {
        for (int i = 0; i < lstNotes.Items.Count; i++)
        {
            if (lstNotes.Items[i] is Note n && n.Id == id)
            {
                lstNotes.SelectedIndex = i;
                break;
            }
        }
    }

    private void btnDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null)
        {
            MessageBox.Show("Выберите заметку для удаления.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Удалить заметку «{_selectedNote.Title}»?",
            "Подтверждение",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            _db.DeleteNote(_selectedNote.Id);
            DeleteNoteFilesFromFolder(_selectedNote.Id, _selectedNote.Title);
            _selectedNote = null;
            LoadNotes();
            lblStatus.Text = "Заметка удалена из БД и папки";
        }
    }
}
