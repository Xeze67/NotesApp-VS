using System.Diagnostics;

namespace NotesApp_v2;

public partial class Form1 : Form
{
    private readonly DatabaseService _db;
    private readonly string _notesFolder;
    private List<Category> _categories = new();
    private List<Note> _currentNotes = new();
    private Note? _selectedNote;
    private bool _isLoadingEditor = false;
    private bool _isDarkTheme = false;

    public Form1()
    {
        InitializeComponent();
        _db = new DatabaseService();
        _notesFolder = Path.Combine(Application.StartupPath, "Заметки");
        Directory.CreateDirectory(_notesFolder);
        SetupGridColumns();
        LoadCategories();
        RefreshNotesGrid();
        ApplyTheme(false);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.S))
        {
            btnSaveNote_Click(this, EventArgs.Empty);
            return true;
        }
        if (keyData == (Keys.Control | Keys.N))
        {
            btnNewNote_Click(this, EventArgs.Empty);
            return true;
        }
        if (keyData == (Keys.Control | Keys.F))
        {
            txtSearch.Focus();
            txtSearch.SelectAll();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void SetupGridColumns()
    {
        dgvNotes.Columns.Clear();
        dgvNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Pin", HeaderText = "📌", FillWeight = 10 });
        dgvNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Priority", HeaderText = "Приор.", FillWeight = 18 });
        dgvNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "Заголовок", FillWeight = 42 });
        dgvNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Категория", FillWeight = 20 });
        dgvNotes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Updated", HeaderText = "Обновлено", FillWeight = 22 });
    }

    private void LoadCategories()
    {
        _categories = _db.GetCategories();

        int prevFilterId = (cmbFilterCategory.SelectedItem is Category fc) ? fc.Id : 0;
        int prevEditId = (cmbCategory.SelectedItem is Category ec) ? ec.Id : 0;

        cmbFilterCategory.Items.Clear();
        cmbFilterCategory.Items.Add(new Category { Id = 0, Name = "Все категории" });
        foreach (var cat in _categories)
            cmbFilterCategory.Items.Add(cat);

        cmbFilterCategory.SelectedIndex = 0;
        for (int i = 0; i < cmbFilterCategory.Items.Count; i++)
        {
            if (cmbFilterCategory.Items[i] is Category c && c.Id == prevFilterId)
            {
                cmbFilterCategory.SelectedIndex = i;
                break;
            }
        }

        cmbCategory.Items.Clear();
        foreach (var cat in _categories)
            cmbCategory.Items.Add(cat);

        if (cmbCategory.Items.Count > 0)
        {
            cmbCategory.SelectedIndex = 0;
            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                if (cmbCategory.Items[i] is Category c && c.Id == prevEditId)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private void RefreshNotesGrid(int selectNoteId = -1)
    {
        int catId = (cmbFilterCategory.SelectedItem is Category fc) ? fc.Id : 0;
        string prio = cmbFilterPriority.SelectedItem?.ToString() ?? "Все";
        bool archived = chkShowArchive.Checked;
        string search = txtSearch.Text;

        btnArchiveNote.Text = archived ? "Восстановить" : "В архив";

        _currentNotes = _db.FilterNotes(search, catId, prio, archived);

        _isLoadingEditor = true;
        dgvNotes.Rows.Clear();
        foreach (var n in _currentNotes)
        {
            string pinIcon = n.IsPinned ? "★" : "";
            string titleDisplay = n.IsReadOnly ? $"🔒 {n.Title}" : n.Title;
            string shortDate = n.UpdatedAt.Length >= 16 ? n.UpdatedAt.Substring(5, 11) : n.UpdatedAt;
            int idx = dgvNotes.Rows.Add(pinIcon, n.Priority, titleDisplay, n.CategoryName, shortDate);
            dgvNotes.Rows[idx].Tag = n;

            if (n.Priority == "Высокий")
            {
                dgvNotes.Rows[idx].Cells["Priority"].Style.ForeColor = Color.Crimson;
                dgvNotes.Rows[idx].Cells["Priority"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }
        _isLoadingEditor = false;

        if (selectNoteId > 0)
        {
            foreach (DataGridViewRow row in dgvNotes.Rows)
            {
                if (row.Tag is Note n && n.Id == selectNoteId)
                {
                    row.Selected = true;
                    LoadNoteIntoEditor(n);
                    return;
                }
            }
        }

        if (dgvNotes.Rows.Count > 0)
        {
            dgvNotes.Rows[0].Selected = true;
            if (dgvNotes.Rows[0].Tag is Note first)
                LoadNoteIntoEditor(first);
        }
        else
        {
            ClearEditor();
        }
    }

    private void dgvNotes_SelectionChanged(object? sender, EventArgs e)
    {
        if (_isLoadingEditor) return;
        if (dgvNotes.SelectedRows.Count > 0 && dgvNotes.SelectedRows[0].Tag is Note note)
        {
            LoadNoteIntoEditor(note);
        }
    }

    private void LoadNoteIntoEditor(Note note)
    {
        _isLoadingEditor = true;
        _selectedNote = note;
        txtTitle.Text = note.Title;
        txtTags.Text = note.Tags;
        chkPinned.Checked = note.IsPinned;
        chkReadOnly.Checked = note.IsReadOnly;

        for (int i = 0; i < cmbCategory.Items.Count; i++)
        {
            if (cmbCategory.Items[i] is Category c && c.Id == note.CategoryId)
            {
                cmbCategory.SelectedIndex = i;
                break;
            }
        }

        int prioIdx = cmbPriority.Items.IndexOf(note.Priority);
        cmbPriority.SelectedIndex = prioIdx >= 0 ? prioIdx : 0;

        chkReminder.Checked = note.HasReminder;
        if (DateTime.TryParse(note.ReminderAt, out DateTime dt))
            dtpReminder.Value = dt;
        else
            dtpReminder.Value = DateTime.Now.AddHours(1);

        txtContent.Text = note.Content;
        ApplyReadOnlyState();
        _isLoadingEditor = false;
        UpdateWordCount();
    }

    private void ClearEditor()
    {
        _isLoadingEditor = true;
        _selectedNote = null;
        txtTitle.Text = string.Empty;
        txtTags.Text = string.Empty;
        chkPinned.Checked = false;
        chkReadOnly.Checked = false;
        chkReminder.Checked = false;
        dtpReminder.Value = DateTime.Now.AddHours(1);
        if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
        cmbPriority.SelectedIndex = 0;
        txtContent.Clear();
        ApplyReadOnlyState();
        _isLoadingEditor = false;
        UpdateWordCount();
    }

    private void ApplyReadOnlyState()
    {
        bool ro = chkReadOnly.Checked;
        txtTitle.ReadOnly = ro;
        txtContent.ReadOnly = ro;
        txtTags.ReadOnly = ro;
        cmbCategory.Enabled = !ro;
        cmbPriority.Enabled = !ro;
        chkReminder.Enabled = !ro;
        dtpReminder.Enabled = !ro && chkReminder.Checked;
        btnAddTask.Enabled = !ro;
        btnToggleTask.Enabled = !ro;
        btnInsertDate.Enabled = !ro;
        btnTemplate.Enabled = !ro;
    }

    private void chkReadOnly_CheckedChanged(object? sender, EventArgs e)
    {
        ApplyReadOnlyState();
        if (!_isLoadingEditor && _selectedNote != null)
        {
            _selectedNote.IsReadOnly = chkReadOnly.Checked;
            _db.UpdateNote(_selectedNote);
            RefreshNotesGrid(_selectedNote.Id);
        }
    }

    private void UpdateWordCount()
    {
        string text = txtContent.Text;
        int chars = text.Length;
        int words = string.IsNullOrWhiteSpace(text)
            ? 0
            : text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        string lockPrefix = chkReadOnly.Checked ? "🔒 Только чтение | " : "";
        string dates = _selectedNote != null
            ? $" | Изм.: {_selectedNote.UpdatedAt}"
            : " | Новая заметка";

        lblEditorStatus.Text = $"{lockPrefix}Слов: {words} | Симв.: {chars}{dates}";
    }

    private void btnNewNote_Click(object? sender, EventArgs e)
    {
        dgvNotes.ClearSelection();
        ClearEditor();
        txtTitle.Focus();
    }

    private void btnSaveNote_Click(object? sender, EventArgs e)
    {
        string title = txtTitle.Text.Trim();
        if (string.IsNullOrEmpty(title))
        {
            MessageBox.Show("Введите заголовок заметки!", "Предупреждение",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTitle.Focus();
            return;
        }

        int catId = (cmbCategory.SelectedItem is Category c) ? c.Id : 1;
        string prio = cmbPriority.SelectedItem?.ToString() ?? "Обычный";
        string remAt = chkReminder.Checked ? dtpReminder.Value.ToString("yyyy-MM-dd HH:mm") : "";

        int savedId;
        string catName = (cmbCategory.SelectedItem is Category catObj) ? catObj.Name : "";

        if (_selectedNote == null)
        {
            var newNote = new Note
            {
                Title = title,
                Content = txtContent.Text,
                CategoryId = catId,
                CategoryName = catName,
                Priority = prio,
                IsPinned = chkPinned.Checked,
                IsArchived = chkShowArchive.Checked,
                IsReadOnly = chkReadOnly.Checked,
                Tags = txtTags.Text.Trim(),
                HasReminder = chkReminder.Checked,
                ReminderAt = remAt,
                CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            savedId = _db.AddNote(newNote);
            SaveTxtCopyToFolder(savedId, newNote);
            RefreshNotesGrid(savedId);
        }
        else
        {
            string oldTitle = _selectedNote.Title;
            _selectedNote.Title = title;
            _selectedNote.Content = txtContent.Text;
            _selectedNote.CategoryId = catId;
            _selectedNote.CategoryName = catName;
            _selectedNote.Priority = prio;
            _selectedNote.IsPinned = chkPinned.Checked;
            _selectedNote.IsReadOnly = chkReadOnly.Checked;
            _selectedNote.Tags = txtTags.Text.Trim();
            _selectedNote.HasReminder = chkReminder.Checked;
            _selectedNote.ReminderAt = remAt;
            _selectedNote.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            _db.UpdateNote(_selectedNote);
            SaveTxtCopyToFolder(_selectedNote.Id, _selectedNote, oldTitle);
            RefreshNotesGrid(_selectedNote.Id);
        }

        lblEditorStatus.Text = $"✓ Сохранено в БД и .txt ({DateTime.Now:HH:mm:ss})";
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
        foreach (string pattern in new[] { $"{id}_*.txt", $"{id}_*.rtf" })
        {
            foreach (string file in Directory.GetFiles(_notesFolder, pattern))
            {
                try { File.Delete(file); } catch { }
            }
        }
    }

    private void SaveTxtCopyToFolder(int id, Note note, string? oldTitle = null)
    {
        Directory.CreateDirectory(_notesFolder);
        DeleteNoteFilesFromFolder(id, oldTitle);
        string filePath = GetNoteFilePath(note.Title);
        ExportService.ExportNoteToTxt(note, filePath);
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

    private void btnOpenFile_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null)
        {
            MessageBox.Show("Сначала выберите заметку из списка или сохраните текущую.", "Открытие файла",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Directory.CreateDirectory(_notesFolder);
        string existingFile = GetNoteFilePath(_selectedNote.Title);
        if (!File.Exists(existingFile))
        {
            SaveTxtCopyToFolder(_selectedNote.Id, _selectedNote);
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = existingFile,
            UseShellExecute = true
        });
        lblEditorStatus.Text = $"Открыт файл: {Path.GetFileName(existingFile)}";
    }

    // ===== ПОЛЕЗНЫЕ ИНСТРУМЕНТЫ РЕДАКТОРА =====
    private void btnAddTask_Click(object? sender, EventArgs e)
    {
        int pos = txtContent.SelectionStart;
        string prefix = (pos > 0 && txtContent.Text[pos - 1] != '\n') ? Environment.NewLine : "";
        string insert = prefix + "[ ] ";
        txtContent.Text = txtContent.Text.Insert(pos, insert);
        txtContent.SelectionStart = pos + insert.Length;
        txtContent.Focus();
    }

    private void btnToggleTask_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtContent.Text)) return;
        int lineIdx = txtContent.GetLineFromCharIndex(txtContent.SelectionStart);
        var lines = txtContent.Lines;
        if (lineIdx < 0 || lineIdx >= lines.Length) return;

        string line = lines[lineIdx];
        string trimmed = line.TrimStart();
        int indentLen = line.Length - trimmed.Length;
        string indent = line.Substring(0, indentLen);

        if (trimmed.StartsWith("[ ] "))
            lines[lineIdx] = indent + "[x] " + trimmed.Substring(4);
        else if (trimmed.StartsWith("[x] ", StringComparison.OrdinalIgnoreCase))
            lines[lineIdx] = indent + "[ ] " + trimmed.Substring(4);
        else
            lines[lineIdx] = indent + "[x] " + trimmed;

        int savedPos = txtContent.SelectionStart;
        txtContent.Lines = lines;
        txtContent.SelectionStart = Math.Min(savedPos, txtContent.TextLength);
        txtContent.Focus();
    }

    private void btnInsertDate_Click(object? sender, EventArgs e)
    {
        string stamp = $"[{DateTime.Now:dd.MM.yyyy HH:mm}] ";
        int pos = txtContent.SelectionStart;
        txtContent.Text = txtContent.Text.Insert(pos, stamp);
        txtContent.SelectionStart = pos + stamp.Length;
        txtContent.Focus();
    }

    private void btnTemplate_Click(object? sender, EventArgs e)
    {
        menuTemplates.Show(btnTemplate, new Point(0, btnTemplate.Height));
    }

    private void ApplyTemplate(string name)
    {
        string body = name switch
        {
            "План на день" =>
                $"План на {DateTime.Now:dd.MM.yyyy}\r\n" +
                "==============================\r\n" +
                "Главные задачи:\r\n" +
                "[ ] \r\n" +
                "[ ] \r\n\r\n" +
                "Заметки и итоги дня:\r\n- ",
            "Список дел" =>
                "Список покупок / дел:\r\n" +
                "[ ] \r\n" +
                "[ ] \r\n" +
                "[ ] \r\n",
            "Конспект" =>
                $"Тема: \r\nДата: {DateTime.Now:dd.MM.yyyy}\r\n" +
                "------------------------------\r\n" +
                "Основные тезисы:\r\n• \r\n• \r\n\r\n" +
                "Выводы / Вопросы:\r\n- ",
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(txtTitle.Text))
            txtTitle.Text = $"{name} ({DateTime.Now:dd.MM})";

        if (!string.IsNullOrWhiteSpace(txtContent.Text))
            txtContent.AppendText(Environment.NewLine + Environment.NewLine + body);
        else
            txtContent.Text = body;

        txtContent.Focus();
    }

    private void btnCopyText_Click(object? sender, EventArgs e)
    {
        string full = string.IsNullOrWhiteSpace(txtTitle.Text)
            ? txtContent.Text
            : $"{txtTitle.Text}\r\n\r\n{txtContent.Text}";
        if (string.IsNullOrWhiteSpace(full)) return;
        Clipboard.SetText(full);
        lblEditorStatus.Text = "✓ Текст скопирован в буфер обмена!";
    }

    private void btnDuplicate_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null)
        {
            MessageBox.Show("Выберите заметку для дублирования.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var copy = new Note
        {
            Title = $"{_selectedNote.Title} (копия)",
            Content = _selectedNote.Content,
            CategoryId = _selectedNote.CategoryId,
            CategoryName = _selectedNote.CategoryName,
            Priority = _selectedNote.Priority,
            IsPinned = false,
            IsArchived = _selectedNote.IsArchived,
            IsReadOnly = false,
            Tags = _selectedNote.Tags,
            HasReminder = false,
            ReminderAt = "",
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        int newId = _db.AddNote(copy);
        SaveTxtCopyToFolder(newId, copy);
        RefreshNotesGrid(newId);
        lblEditorStatus.Text = "✓ Создана копия заметки";
    }

    private void btnPinNote_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null) return;
        _db.SetPinState(_selectedNote.Id, !_selectedNote.IsPinned);
        RefreshNotesGrid(_selectedNote.Id);
    }

    private void btnArchiveNote_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null) return;
        bool targetArchive = !_selectedNote.IsArchived;
        _db.SetArchiveState(_selectedNote.Id, targetArchive);
        RefreshNotesGrid();
    }

    private void btnDeleteNote_Click(object? sender, EventArgs e)
    {
        if (_selectedNote == null)
        {
            MessageBox.Show("Выберите заметку для удаления.", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var res = MessageBox.Show(
            $"Безвозвратно удалить заметку «{_selectedNote.Title}»?",
            "Удаление заметки",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (res == DialogResult.Yes)
        {
            _db.DeleteNotePermanently(_selectedNote.Id);
            DeleteNoteFilesFromFolder(_selectedNote.Id, _selectedNote.Title);
            _selectedNote = null;
            RefreshNotesGrid();
        }
    }

    private void btnAddCategory_Click(object? sender, EventArgs e)
    {
        using var dlg = new Form
        {
            Width = 340,
            Height = 160,
            Text = "Новая категория",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };
        var lbl = new Label { Text = "Название категории:", Left = 16, Top = 16, Width = 280 };
        var tb = new TextBox { Left = 16, Top = 42, Width = 290 };
        var ok = new Button { Text = "Добавить", Left = 120, Top = 80, Width = 90, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = "Отмена", Left = 216, Top = 80, Width = 90, DialogResult = DialogResult.Cancel };
        dlg.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
        dlg.AcceptButton = ok;

        if (dlg.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(tb.Text))
        {
            try
            {
                _db.AddCategory(tb.Text.Trim());
                LoadCategories();
            }
            catch
            {
                MessageBox.Show("Такая категория уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void btnBackupJson_Click(object? sender, EventArgs e)
    {
        var all = _db.FilterNotes("", 0, "Все", false);
        all.AddRange(_db.FilterNotes("", 0, "Все", true));

        using var sfd = new SaveFileDialog
        {
            Filter = "JSON резервная копия (*.json)|*.json",
            FileName = $"notes_backup_{DateTime.Now:yyyyMMdd_HHmm}.json"
        };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            ExportService.ExportAllToJson(all, sfd.FileName);
            MessageBox.Show($"Создан бэкап {all.Count} заметок.", "Резервное копирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // ===== НАПОМИНАНИЯ =====
    private void reminderTimer_Tick(object? sender, EventArgs e)
    {
        var due = _db.GetDueReminders();
        foreach (var note in due)
        {
            _db.ClearReminder(note.Id);
            MessageBox.Show(
                $"Напоминание по заметке:\n«{note.Title}»\nВремя: {note.ReminderAt}",
                "Напоминание — Заметки Pro v2.0",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    // ===== ТЕМЫ ОФОРМЛЕНИЯ =====
    private void btnTheme_Click(object? sender, EventArgs e)
    {
        _isDarkTheme = !_isDarkTheme;
        ApplyTheme(_isDarkTheme);
    }

    private void ApplyTheme(bool dark)
    {
        btnTheme.Text = dark ? "Светлая тема" : "Темная тема";

        Color bg = dark ? Color.FromArgb(24, 24, 28) : Color.FromArgb(245, 246, 248);
        Color surface = dark ? Color.FromArgb(36, 36, 42) : Color.White;
        Color fg = dark ? Color.FromArgb(235, 235, 240) : Color.FromArgb(25, 25, 25);
        Color btnBg = dark ? Color.FromArgb(52, 52, 60) : Color.FromArgb(238, 240, 244);

        this.BackColor = bg;
        this.ForeColor = fg;
        panelTop.BackColor = bg;
        panelNotesLeft.BackColor = bg;
        panelEditor.BackColor = bg;
        panelNotesActions.BackColor = bg;
        panelEditorFooter.BackColor = bg;
        panelTools.BackColor = bg;

        foreach (Control c in panelTop.Controls)
            StyleControl(c, surface, fg, btnBg);
        foreach (Control c in panelEditor.Controls)
            StyleControl(c, surface, fg, btnBg);
        foreach (Control c in panelNotesActions.Controls)
            StyleControl(c, surface, fg, btnBg);
        foreach (Control c in panelTools.Controls)
            StyleControl(c, surface, fg, btnBg);
        foreach (Control c in panelEditorFooter.Controls)
            StyleControl(c, surface, fg, btnBg);

        btnSaveNote.BackColor = Color.FromArgb(33, 115, 70);
        btnSaveNote.ForeColor = Color.White;
        btnSaveNote.FlatAppearance.BorderSize = 0;

        StyleGrid(dgvNotes, dark, surface, fg);
    }

    private static void StyleControl(Control c, Color surface, Color fg, Color btnBg)
    {
        if (c is Button btn)
        {
            btn.BackColor = btnBg;
            btn.ForeColor = fg;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.Gray;
        }
        else if (c is TextBox or ComboBox or DateTimePicker)
        {
            c.BackColor = surface;
            c.ForeColor = fg;
        }
        else if (c is Label or CheckBox)
        {
            c.ForeColor = fg;
        }
    }

    private static void StyleGrid(DataGridView dgv, bool dark, Color surface, Color fg)
    {
        dgv.BackgroundColor = surface;
        dgv.GridColor = dark ? Color.FromArgb(60, 60, 68) : Color.LightGray;
        dgv.DefaultCellStyle.BackColor = surface;
        dgv.DefaultCellStyle.ForeColor = fg;
        dgv.DefaultCellStyle.SelectionBackColor = dark ? Color.FromArgb(65, 95, 145) : Color.FromArgb(215, 230, 250);
        dgv.DefaultCellStyle.SelectionForeColor = fg;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = dark ? Color.FromArgb(45, 45, 52) : Color.FromArgb(235, 237, 240);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = fg;
    }
}
