namespace NotesApp_v2;

partial class Form1
{
    private System.ComponentModel.IContainer components = null!;

    // Верхняя панель инструментов
    private Panel panelTop = null!;
    private TextBox txtSearch = null!;
    private Label lblSearch = null!;
    private ComboBox cmbFilterCategory = null!;
    private ComboBox cmbFilterPriority = null!;
    private CheckBox chkShowArchive = null!;
    private Button btnTheme = null!;
    private Button btnAddCategory = null!;
    private Button btnBackupJson = null!;

    // Левая панель списка заметок
    private Panel panelNotesLeft = null!;
    private DataGridView dgvNotes = null!;
    private Panel panelNotesActions = null!;
    private Button btnNewNote = null!;
    private Button btnPinNote = null!;
    private Button btnArchiveNote = null!;
    private Button btnDeleteNote = null!;

    // Правая панель редактора
    private Panel panelEditor = null!;
    private TextBox txtTitle = null!;
    private ComboBox cmbCategory = null!;
    private ComboBox cmbPriority = null!;
    private TextBox txtTags = null!;
    private CheckBox chkPinned = null!;
    private CheckBox chkReadOnly = null!;
    private CheckBox chkReminder = null!;
    private DateTimePicker dtpReminder = null!;
    private Panel panelTools = null!;
    private Button btnAddTask = null!;
    private Button btnToggleTask = null!;
    private Button btnInsertDate = null!;
    private Button btnTemplate = null!;
    private Button btnCopyText = null!;
    private Button btnDuplicate = null!;
    private ContextMenuStrip menuTemplates = null!;
    private TextBox txtContent = null!;
    private Panel panelEditorFooter = null!;
    private Label lblEditorStatus = null!;
    private Button btnOpenFolder = null!;
    private Button btnOpenFile = null!;
    private Button btnSaveNote = null!;

    private System.Windows.Forms.Timer reminderTimer = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.panelTop = new Panel();
        this.lblSearch = new Label();
        this.txtSearch = new TextBox();
        this.cmbFilterCategory = new ComboBox();
        this.cmbFilterPriority = new ComboBox();
        this.chkShowArchive = new CheckBox();
        this.btnAddCategory = new Button();
        this.btnBackupJson = new Button();
        this.btnTheme = new Button();

        this.panelNotesLeft = new Panel();
        this.dgvNotes = new DataGridView();
        this.panelNotesActions = new Panel();
        this.btnNewNote = new Button();
        this.btnPinNote = new Button();
        this.btnArchiveNote = new Button();
        this.btnDeleteNote = new Button();

        this.panelEditor = new Panel();
        this.txtTitle = new TextBox();
        this.cmbCategory = new ComboBox();
        this.cmbPriority = new ComboBox();
        this.txtTags = new TextBox();
        this.chkPinned = new CheckBox();
        this.chkReadOnly = new CheckBox();
        this.chkReminder = new CheckBox();
        this.dtpReminder = new DateTimePicker();
        this.panelTools = new Panel();
        this.btnAddTask = new Button();
        this.btnToggleTask = new Button();
        this.btnInsertDate = new Button();
        this.btnTemplate = new Button();
        this.btnCopyText = new Button();
        this.btnDuplicate = new Button();
        this.menuTemplates = new ContextMenuStrip(this.components);
        this.txtContent = new TextBox();
        this.panelEditorFooter = new Panel();
        this.lblEditorStatus = new Label();
        this.btnOpenFolder = new Button();
        this.btnOpenFile = new Button();
        this.btnSaveNote = new Button();

        this.reminderTimer = new System.Windows.Forms.Timer(this.components);

        this.panelTop.SuspendLayout();
        this.panelNotesLeft.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.dgvNotes).BeginInit();
        this.panelNotesActions.SuspendLayout();
        this.panelEditor.SuspendLayout();
        this.panelTools.SuspendLayout();
        this.panelEditorFooter.SuspendLayout();
        this.SuspendLayout();

        // ===== panelTop =====
        this.panelTop.Size = new Size(1140, 52);
        this.panelTop.Dock = DockStyle.Top;
        this.panelTop.Padding = new Padding(10, 10, 10, 8);

        this.lblSearch.Location = new Point(12, 15);
        this.lblSearch.Size = new Size(56, 22);
        this.lblSearch.Text = "Поиск:";
        this.lblSearch.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        this.txtSearch.Location = new Point(70, 12);
        this.txtSearch.Size = new Size(215, 26);
        this.txtSearch.Font = new Font("Segoe UI", 9.5F);
        this.txtSearch.PlaceholderText = "Заголовок, текст, тег...";
        this.txtSearch.TextChanged += (s, e) => RefreshNotesGrid();

        this.cmbFilterCategory.Location = new Point(293, 12);
        this.cmbFilterCategory.Size = new Size(145, 26);
        this.cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbFilterCategory.Font = new Font("Segoe UI", 9.5F);
        this.cmbFilterCategory.SelectedIndexChanged += (s, e) => RefreshNotesGrid();

        this.cmbFilterPriority.Location = new Point(446, 12);
        this.cmbFilterPriority.Size = new Size(115, 26);
        this.cmbFilterPriority.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbFilterPriority.Font = new Font("Segoe UI", 9.5F);
        this.cmbFilterPriority.Items.AddRange(new object[] { "Все", "Высокий", "Средний", "Обычный" });
        this.cmbFilterPriority.SelectedIndex = 0;
        this.cmbFilterPriority.SelectedIndexChanged += (s, e) => RefreshNotesGrid();

        this.chkShowArchive.Location = new Point(570, 14);
        this.chkShowArchive.Size = new Size(76, 24);
        this.chkShowArchive.Text = "Архив";
        this.chkShowArchive.Font = new Font("Segoe UI", 9.5F);
        this.chkShowArchive.CheckedChanged += (s, e) => RefreshNotesGrid();

        this.btnAddCategory.Location = new Point(652, 11);
        this.btnAddCategory.Size = new Size(100, 28);
        this.btnAddCategory.Text = "+ Категория";
        this.btnAddCategory.Font = new Font("Segoe UI", 8.5F);
        this.btnAddCategory.Click += this.btnAddCategory_Click;

        this.btnBackupJson.Location = new Point(760, 11);
        this.btnBackupJson.Size = new Size(106, 28);
        this.btnBackupJson.Text = "Бэкап JSON";
        this.btnBackupJson.Font = new Font("Segoe UI", 8.5F);
        this.btnBackupJson.Click += this.btnBackupJson_Click;

        this.btnTheme.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnTheme.Location = new Point(1018, 11);
        this.btnTheme.Size = new Size(110, 28);
        this.btnTheme.Text = "Темная тема";
        this.btnTheme.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        this.btnTheme.Click += this.btnTheme_Click;

        this.panelTop.Controls.AddRange(new Control[]
        {
            this.lblSearch, this.txtSearch, this.cmbFilterCategory, this.cmbFilterPriority,
            this.chkShowArchive, this.btnAddCategory,
            this.btnBackupJson, this.btnTheme
        });

        // ===== panelNotesLeft =====
        this.panelNotesLeft.Size = new Size(416, 608);
        this.panelNotesLeft.Dock = DockStyle.Left;
        this.panelNotesLeft.Padding = new Padding(8, 6, 4, 8);

        this.panelNotesActions.Size = new Size(404, 46);
        this.panelNotesActions.Dock = DockStyle.Bottom;
        this.panelNotesActions.Padding = new Padding(0, 6, 0, 0);

        this.btnNewNote.Location = new Point(0, 8);
        this.btnNewNote.Size = new Size(96, 34);
        this.btnNewNote.Text = "+ Новая";
        this.btnNewNote.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.btnNewNote.Click += this.btnNewNote_Click;

        this.btnPinNote.Location = new Point(102, 8);
        this.btnPinNote.Size = new Size(96, 34);
        this.btnPinNote.Text = "Закрепить";
        this.btnPinNote.Font = new Font("Segoe UI", 9F);
        this.btnPinNote.Click += this.btnPinNote_Click;

        this.btnArchiveNote.Location = new Point(204, 8);
        this.btnArchiveNote.Size = new Size(98, 34);
        this.btnArchiveNote.Text = "В архив";
        this.btnArchiveNote.Font = new Font("Segoe UI", 9F);
        this.btnArchiveNote.Click += this.btnArchiveNote_Click;

        this.btnDeleteNote.Location = new Point(308, 8);
        this.btnDeleteNote.Size = new Size(94, 34);
        this.btnDeleteNote.Text = "Удалить";
        this.btnDeleteNote.Font = new Font("Segoe UI", 9F);
        this.btnDeleteNote.Click += this.btnDeleteNote_Click;

        this.panelNotesActions.Controls.AddRange(new Control[]
        {
            this.btnNewNote, this.btnPinNote, this.btnArchiveNote, this.btnDeleteNote
        });

        this.dgvNotes.Dock = DockStyle.Fill;
        this.dgvNotes.AllowUserToAddRows = false;
        this.dgvNotes.AllowUserToDeleteRows = false;
        this.dgvNotes.ReadOnly = true;
        this.dgvNotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvNotes.MultiSelect = false;
        this.dgvNotes.RowHeadersVisible = false;
        this.dgvNotes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvNotes.BackgroundColor = Color.White;
        this.dgvNotes.BorderStyle = BorderStyle.FixedSingle;
        this.dgvNotes.SelectionChanged += this.dgvNotes_SelectionChanged;

        this.panelNotesLeft.Controls.Add(this.dgvNotes);
        this.panelNotesLeft.Controls.Add(this.panelNotesActions);

        // ===== panelEditor =====
        this.panelEditor.Size = new Size(724, 608);
        this.panelEditor.Dock = DockStyle.Fill;
        this.panelEditor.Padding = new Padding(14, 6, 14, 8);

        var lblT = new Label { Text = "Заголовок:", Location = new Point(14, 10), Size = new Size(80, 20), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        this.txtTitle.Location = new Point(14, 32);
        this.txtTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.txtTitle.Size = new Size(696, 28);
        this.txtTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.txtTitle.PlaceholderText = "Введите заголовок заметки...";

        var lblCat = new Label { Text = "Категория:", Location = new Point(14, 68), Size = new Size(76, 20), Font = new Font("Segoe UI", 9F) };
        this.cmbCategory.Location = new Point(92, 65);
        this.cmbCategory.Size = new Size(135, 25);
        this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbCategory.Font = new Font("Segoe UI", 9F);

        var lblPrio = new Label { Text = "Приоритет:", Location = new Point(236, 68), Size = new Size(78, 20), Font = new Font("Segoe UI", 9F) };
        this.cmbPriority.Location = new Point(316, 65);
        this.cmbPriority.Size = new Size(105, 25);
        this.cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;
        this.cmbPriority.Font = new Font("Segoe UI", 9F);
        this.cmbPriority.Items.AddRange(new object[] { "Обычный", "Средний", "Высокий" });
        this.cmbPriority.SelectedIndex = 0;

        this.chkPinned.Location = new Point(432, 67);
        this.chkPinned.Size = new Size(96, 22);
        this.chkPinned.Text = "Закрепить";
        this.chkPinned.Font = new Font("Segoe UI", 9F);

        this.chkReadOnly.Location = new Point(534, 67);
        this.chkReadOnly.Size = new Size(140, 22);
        this.chkReadOnly.Text = "Только чтение";
        this.chkReadOnly.Font = new Font("Segoe UI", 9F);
        this.chkReadOnly.CheckedChanged += this.chkReadOnly_CheckedChanged;

        var lblTags = new Label { Text = "Теги:", Location = new Point(14, 100), Size = new Size(46, 20), Font = new Font("Segoe UI", 9F) };
        this.txtTags.Location = new Point(62, 97);
        this.txtTags.Size = new Size(265, 25);
        this.txtTags.Font = new Font("Segoe UI", 9F);
        this.txtTags.PlaceholderText = "#работа, #идея, #важно";

        this.chkReminder.Location = new Point(340, 99);
        this.chkReminder.Size = new Size(118, 22);
        this.chkReminder.Text = "Напоминание:";
        this.chkReminder.Font = new Font("Segoe UI", 9F);
        this.chkReminder.CheckedChanged += (s, e) => { this.dtpReminder.Enabled = this.chkReminder.Checked && !this.chkReadOnly.Checked; };

        this.dtpReminder.Location = new Point(460, 97);
        this.dtpReminder.Size = new Size(175, 25);
        this.dtpReminder.Format = DateTimePickerFormat.Custom;
        this.dtpReminder.CustomFormat = "dd.MM.yyyy HH:mm";
        this.dtpReminder.Font = new Font("Segoe UI", 9F);
        this.dtpReminder.Enabled = false;

        // Панель полезных инструментов (вместо форматирования)
        this.panelTools.Location = new Point(14, 130);
        this.panelTools.Size = new Size(696, 34);
        this.panelTools.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        this.btnAddTask.Location = new Point(0, 3);
        this.btnAddTask.Size = new Size(106, 28);
        this.btnAddTask.Text = "+ Задача [ ]";
        this.btnAddTask.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        this.btnAddTask.Click += this.btnAddTask_Click;

        this.btnToggleTask.Location = new Point(112, 3);
        this.btnToggleTask.Size = new Size(92, 28);
        this.btnToggleTask.Text = "✓ Готово";
        this.btnToggleTask.Font = new Font("Segoe UI", 8.5F);
        this.btnToggleTask.Click += this.btnToggleTask_Click;

        this.btnInsertDate.Location = new Point(210, 3);
        this.btnInsertDate.Size = new Size(80, 28);
        this.btnInsertDate.Text = "+ Дата";
        this.btnInsertDate.Font = new Font("Segoe UI", 8.5F);
        this.btnInsertDate.Click += this.btnInsertDate_Click;

        this.btnTemplate.Location = new Point(296, 3);
        this.btnTemplate.Size = new Size(104, 28);
        this.btnTemplate.Text = "Шаблоны ▼";
        this.btnTemplate.Font = new Font("Segoe UI", 8.5F);
        this.btnTemplate.Click += this.btnTemplate_Click;

        this.btnCopyText.Location = new Point(406, 3);
        this.btnCopyText.Size = new Size(96, 28);
        this.btnCopyText.Text = "Копировать";
        this.btnCopyText.Font = new Font("Segoe UI", 8.5F);
        this.btnCopyText.Click += this.btnCopyText_Click;

        this.btnDuplicate.Location = new Point(508, 3);
        this.btnDuplicate.Size = new Size(102, 28);
        this.btnDuplicate.Text = "Дублировать";
        this.btnDuplicate.Font = new Font("Segoe UI", 8.5F);
        this.btnDuplicate.Click += this.btnDuplicate_Click;

        this.panelTools.Controls.AddRange(new Control[]
        {
            this.btnAddTask, this.btnToggleTask, this.btnInsertDate,
            this.btnTemplate, this.btnCopyText, this.btnDuplicate
        });

        // Меню шаблонов
        this.menuTemplates.Items.Add("План на день", null, (s, e) => ApplyTemplate("План на день"));
        this.menuTemplates.Items.Add("Список дел / покупок", null, (s, e) => ApplyTemplate("Список дел"));
        this.menuTemplates.Items.Add("Конспект / Учеба", null, (s, e) => ApplyTemplate("Конспект"));

        // Подвал редактора
        this.panelEditorFooter.Size = new Size(696, 48);
        this.panelEditorFooter.Dock = DockStyle.Bottom;
        this.panelEditorFooter.Padding = new Padding(0, 6, 0, 4);

        this.lblEditorStatus.Location = new Point(0, 12);
        this.lblEditorStatus.Size = new Size(284, 24);
        this.lblEditorStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.lblEditorStatus.Font = new Font("Segoe UI", 8.5F);
        this.lblEditorStatus.Text = "Слов: 0 | Символов: 0";

        this.btnOpenFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnOpenFolder.Location = new Point(292, 6);
        this.btnOpenFolder.Size = new Size(80, 36);
        this.btnOpenFolder.Text = "Папка";
        this.btnOpenFolder.Font = new Font("Segoe UI", 9F);
        this.btnOpenFolder.Click += this.btnOpenFolder_Click;

        this.btnOpenFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnOpenFile.Location = new Point(378, 6);
        this.btnOpenFile.Size = new Size(116, 36);
        this.btnOpenFile.Text = "Открыть файл";
        this.btnOpenFile.Font = new Font("Segoe UI", 9F);
        this.btnOpenFile.Click += this.btnOpenFile_Click;

        this.btnSaveNote.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.btnSaveNote.Location = new Point(500, 6);
        this.btnSaveNote.Size = new Size(196, 36);
        this.btnSaveNote.BackColor = Color.FromArgb(33, 115, 70);
        this.btnSaveNote.ForeColor = Color.White;
        this.btnSaveNote.FlatStyle = FlatStyle.Flat;
        this.btnSaveNote.FlatAppearance.BorderSize = 0;
        this.btnSaveNote.Text = "Сохранить (Ctrl+S)";
        this.btnSaveNote.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnSaveNote.Click += this.btnSaveNote_Click;

        this.panelEditorFooter.Controls.Add(this.lblEditorStatus);
        this.panelEditorFooter.Controls.Add(this.btnOpenFolder);
        this.panelEditorFooter.Controls.Add(this.btnOpenFile);
        this.panelEditorFooter.Controls.Add(this.btnSaveNote);

        // Редактор TextBox
        this.txtContent.Location = new Point(14, 168);
        this.txtContent.Size = new Size(696, 380);
        this.txtContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.txtContent.Font = new Font("Segoe UI", 11F);
        this.txtContent.Multiline = true;
        this.txtContent.ScrollBars = ScrollBars.Vertical;
        this.txtContent.AcceptsTab = true;
        this.txtContent.BorderStyle = BorderStyle.FixedSingle;
        this.txtContent.TextChanged += (s, e) => UpdateWordCount();

        this.panelEditor.Controls.AddRange(new Control[]
        {
            lblT, this.txtTitle, lblCat, this.cmbCategory, lblPrio, this.cmbPriority,
            this.chkPinned, this.chkReadOnly, lblTags, this.txtTags, this.chkReminder, this.dtpReminder,
            this.panelTools, this.txtContent, this.panelEditorFooter
        });

        // Timer
        this.reminderTimer.Interval = 30000;
        this.reminderTimer.Enabled = true;
        this.reminderTimer.Tick += this.reminderTimer_Tick;

        // Form1
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1140, 660);
        this.MinimumSize = new Size(980, 560);
        this.KeyPreview = true;
        this.Controls.Add(this.panelEditor);
        this.Controls.Add(this.panelNotesLeft);
        this.Controls.Add(this.panelTop);
        this.Name = "Form1";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Заметки Pro — Версия 2.0 (Многофункциональная)";

        this.panelTop.ResumeLayout(false);
        this.panelTop.PerformLayout();
        this.panelNotesLeft.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.dgvNotes).EndInit();
        this.panelNotesActions.ResumeLayout(false);
        this.panelEditor.ResumeLayout(false);
        this.panelEditor.PerformLayout();
        this.panelTools.ResumeLayout(false);
        this.panelEditorFooter.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}
