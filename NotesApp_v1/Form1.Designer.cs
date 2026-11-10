namespace NotesApp_v1;

partial class Form1
{
    private System.ComponentModel.IContainer components = null!;

    private ListBox lstNotes = null!;
    private TextBox txtTitle = null!;
    private TextBox txtContent = null!;
    private Button btnNew = null!;
    private Button btnSave = null!;
    private Button btnOpenNote = null!;
    private Button btnOpenFolder = null!;
    private Button btnDelete = null!;
    private Label lblNotes = null!;
    private Label lblTitle = null!;
    private Label lblContent = null!;
    private Label lblDate = null!;
    private Label lblStatus = null!;
    private Panel panelLeft = null!;
    private Panel panelRight = null!;

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
        this.panelLeft = new Panel();
        this.lblNotes = new Label();
        this.lstNotes = new ListBox();
        this.btnNew = new Button();
        this.btnDelete = new Button();
        this.panelRight = new Panel();
        this.lblTitle = new Label();
        this.txtTitle = new TextBox();
        this.lblDate = new Label();
        this.lblContent = new Label();
        this.txtContent = new TextBox();
        this.lblStatus = new Label();
        this.btnSave = new Button();
        this.btnOpenNote = new Button();
        this.btnOpenFolder = new Button();

        this.panelLeft.SuspendLayout();
        this.panelRight.SuspendLayout();
        this.SuspendLayout();

        // panelLeft
        this.panelLeft.BackColor = Color.FromArgb(245, 246, 248);
        this.panelLeft.Location = new Point(0, 0);
        this.panelLeft.Size = new Size(260, 520);
        this.panelLeft.Dock = DockStyle.Left;

        // lblNotes
        this.lblNotes.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.lblNotes.Location = new Point(12, 12);
        this.lblNotes.Size = new Size(236, 26);
        this.lblNotes.Text = "Мои заметки (v1.0)";

        // lstNotes
        this.lstNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.lstNotes.Font = new Font("Segoe UI", 10F);
        this.lstNotes.Location = new Point(12, 44);
        this.lstNotes.Size = new Size(236, 416);
        this.lstNotes.SelectedIndexChanged += this.lstNotes_SelectedIndexChanged;

        // btnNew
        this.btnNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnNew.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        this.btnNew.Location = new Point(12, 470);
        this.btnNew.Size = new Size(114, 36);
        this.btnNew.Text = "+ Новая";
        this.btnNew.UseVisualStyleBackColor = true;
        this.btnNew.Click += this.btnNew_Click;

        // btnDelete
        this.btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnDelete.Font = new Font("Segoe UI", 9.5F);
        this.btnDelete.Location = new Point(134, 470);
        this.btnDelete.Size = new Size(114, 36);
        this.btnDelete.Text = "Удалить";
        this.btnDelete.UseVisualStyleBackColor = true;
        this.btnDelete.Click += this.btnDelete_Click;

        this.panelLeft.Controls.Add(this.lblNotes);
        this.panelLeft.Controls.Add(this.lstNotes);
        this.panelLeft.Controls.Add(this.btnNew);
        this.panelLeft.Controls.Add(this.btnDelete);

        // panelRight
        this.panelRight.BackColor = Color.White;
        this.panelRight.Location = new Point(260, 0);
        this.panelRight.Size = new Size(580, 520);
        this.panelRight.Dock = DockStyle.Fill;

        // lblTitle
        this.lblTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        this.lblTitle.Location = new Point(16, 14);
        this.lblTitle.Size = new Size(120, 22);
        this.lblTitle.Text = "Заголовок:";

        // lblDate
        this.lblDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.lblDate.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        this.lblDate.ForeColor = Color.Gray;
        this.lblDate.Location = new Point(320, 14);
        this.lblDate.Size = new Size(244, 22);
        this.lblDate.TextAlign = ContentAlignment.MiddleRight;
        this.lblDate.Text = "";

        // txtTitle
        this.txtTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.txtTitle.Font = new Font("Segoe UI", 11F);
        this.txtTitle.Location = new Point(16, 40);
        this.txtTitle.Size = new Size(548, 28);

        // lblContent
        this.lblContent.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        this.lblContent.Location = new Point(16, 78);
        this.lblContent.Size = new Size(150, 22);
        this.lblContent.Text = "Текст заметки:";

        // txtContent
        this.txtContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.txtContent.Font = new Font("Segoe UI", 10.5F);
        this.txtContent.Location = new Point(16, 104);
        this.txtContent.Multiline = true;
        this.txtContent.ScrollBars = ScrollBars.Vertical;
        this.txtContent.Size = new Size(548, 326);

        // lblStatus
        this.lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.lblStatus.Font = new Font("Segoe UI", 8.5F);
        this.lblStatus.ForeColor = Color.FromArgb(33, 115, 70);
        this.lblStatus.Location = new Point(16, 438);
        this.lblStatus.Size = new Size(548, 22);
        this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        this.lblStatus.Text = "";

        // btnSave
        this.btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnSave.BackColor = Color.FromArgb(33, 115, 70);
        this.btnSave.ForeColor = Color.White;
        this.btnSave.FlatStyle = FlatStyle.Flat;
        this.btnSave.FlatAppearance.BorderSize = 0;
        this.btnSave.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        this.btnSave.Location = new Point(16, 470);
        this.btnSave.Size = new Size(150, 36);
        this.btnSave.Text = "Сохранить";
        this.btnSave.Click += this.btnSave_Click;

        // btnOpenNote
        this.btnOpenNote.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.btnOpenNote.Font = new Font("Segoe UI", 9.5F);
        this.btnOpenNote.Location = new Point(176, 470);
        this.btnOpenNote.Size = new Size(170, 36);
        this.btnOpenNote.Text = "Открыть заметку";
        this.btnOpenNote.UseVisualStyleBackColor = true;
        this.btnOpenNote.Click += this.btnOpenNote_Click;

        // btnOpenFolder
        this.btnOpenFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.btnOpenFolder.Font = new Font("Segoe UI", 9.5F);
        this.btnOpenFolder.Location = new Point(454, 470);
        this.btnOpenFolder.Size = new Size(110, 36);
        this.btnOpenFolder.Text = "Папка";
        this.btnOpenFolder.UseVisualStyleBackColor = true;
        this.btnOpenFolder.Click += this.btnOpenFolder_Click;

        this.panelRight.Controls.Add(this.lblTitle);
        this.panelRight.Controls.Add(this.lblDate);
        this.panelRight.Controls.Add(this.txtTitle);
        this.panelRight.Controls.Add(this.lblContent);
        this.panelRight.Controls.Add(this.txtContent);
        this.panelRight.Controls.Add(this.lblStatus);
        this.panelRight.Controls.Add(this.btnSave);
        this.panelRight.Controls.Add(this.btnOpenNote);
        this.panelRight.Controls.Add(this.btnOpenFolder);

        // Form1
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(840, 520);
        this.MinimumSize = new Size(740, 440);
        this.Controls.Add(this.panelRight);
        this.Controls.Add(this.panelLeft);
        this.Name = "Form1";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Заметки — Версия 1.0 (Базовая)";

        this.panelLeft.ResumeLayout(false);
        this.panelRight.ResumeLayout(false);
        this.panelRight.PerformLayout();
        this.ResumeLayout(false);
    }
}
