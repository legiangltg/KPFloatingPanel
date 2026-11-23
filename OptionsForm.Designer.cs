namespace KPFloatingPanel
{
    partial class OptionsForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.pnButtons = new System.Windows.Forms.Panel();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.pcOptions = new System.Windows.Forms.TabControl();
            this.tsView = new System.Windows.Forms.TabPage();
            this.lbTheme = new System.Windows.Forms.Label();
            this.cbTheme = new System.Windows.Forms.ComboBox();
            this.cbxDirectSearch = new System.Windows.Forms.CheckBox();
            this.cbxShowDatabaseName = new System.Windows.Forms.CheckBox();
            this.cbxCompactMode = new System.Windows.Forms.CheckBox();
            this.pnFontColor = new System.Windows.Forms.Panel();
            this.pnPanelColor = new System.Windows.Forms.Panel();
            this.edPercent = new System.Windows.Forms.NumericUpDown();
            this.btChangeFontColor = new System.Windows.Forms.Button();
            this.btChangePanelColor = new System.Windows.Forms.Button();
            this.lbPercent = new System.Windows.Forms.Label();
            this.lbFontColor = new System.Windows.Forms.Label();
            this.lbPanelColor = new System.Windows.Forms.Label();
            this.lbTransp = new System.Windows.Forms.Label();
            this.labelAutoTypeDelay = new System.Windows.Forms.Label();
            this.edAutoTypeDelay = new System.Windows.Forms.NumericUpDown();
            this.labelAutoTypeDelayMs = new System.Windows.Forms.Label();
            this.tsBehavior = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxShortcutWinQuick = new System.Windows.Forms.CheckBox();
            this.cbxShortcutAltQuick = new System.Windows.Forms.CheckBox();
            this.cbxShortcutShiftQuick = new System.Windows.Forms.CheckBox();
            this.cbxShortcutCntrlQuick = new System.Windows.Forms.CheckBox();
            this.txtShortcutKeyQuick = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbxShowLastOne = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxShortcutWin = new System.Windows.Forms.CheckBox();
            this.cbxShortcutAlt = new System.Windows.Forms.CheckBox();
            this.cbxShortcutShift = new System.Windows.Forms.CheckBox();
            this.cbxShortcutCntrl = new System.Windows.Forms.CheckBox();
            this.txtShortcutKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxShowFoldersBeforeEntries = new System.Windows.Forms.CheckBox();
            this.cbxShowSearch = new System.Windows.Forms.CheckBox();
            this.cbxShowClock = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbStartGroup = new System.Windows.Forms.ComboBox();
            this.cbxSortAlphabetical = new System.Windows.Forms.CheckBox();
            this.cbURLAction = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ColorDialog = new System.Windows.Forms.ColorDialog();
            this.pnButtons.SuspendLayout();
            this.pcOptions.SuspendLayout();
            this.tsView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edAutoTypeDelay)).BeginInit();
            this.tsBehavior.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnButtons
            // 
            this.pnButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnButtons.Controls.Add(this.btCancel);
            this.pnButtons.Controls.Add(this.btOk);
            this.pnButtons.Location = new System.Drawing.Point(0, 373);
            this.pnButtons.Name = "pnButtons";
            this.pnButtons.Size = new System.Drawing.Size(320, 42);
            this.pnButtons.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(232, 8);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(76, 25);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOk
            // 
            this.btOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOk.Location = new System.Drawing.Point(148, 8);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(76, 25);
            this.btOk.TabIndex = 0;
            this.btOk.Text = "OK";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // pcOptions
            // 
            this.pcOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pcOptions.Controls.Add(this.tsView);
            this.pcOptions.Controls.Add(this.tsBehavior);
            this.pcOptions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pcOptions.Location = new System.Drawing.Point(0, 0);
            this.pcOptions.Name = "pcOptions";
            this.pcOptions.SelectedIndex = 0;
            this.pcOptions.Size = new System.Drawing.Size(320, 370);
            this.pcOptions.TabIndex = 1;
            // 
            // tsView
            // 
            this.tsView.Controls.Add(this.lbTheme);
            this.tsView.Controls.Add(this.cbTheme);
            this.tsView.Controls.Add(this.cbxDirectSearch);
            this.tsView.Controls.Add(this.cbxShowDatabaseName);
            this.tsView.Controls.Add(this.cbxCompactMode);
            this.tsView.Controls.Add(this.pnFontColor);
            this.tsView.Controls.Add(this.pnPanelColor);
            this.tsView.Controls.Add(this.edPercent);
            this.tsView.Controls.Add(this.btChangeFontColor);
            this.tsView.Controls.Add(this.btChangePanelColor);
            this.tsView.Controls.Add(this.lbPercent);
            this.tsView.Controls.Add(this.lbFontColor);
            this.tsView.Controls.Add(this.lbPanelColor);
            this.tsView.Controls.Add(this.lbTransp);
            this.tsView.Location = new System.Drawing.Point(4, 24);
            this.tsView.Name = "tsView";
            this.tsView.Padding = new System.Windows.Forms.Padding(3);
            this.tsView.Size = new System.Drawing.Size(312, 342);
            this.tsView.TabIndex = 0;
            this.tsView.Text = "Giao diện";
            this.tsView.UseVisualStyleBackColor = true;
            // 
            // lbTheme
            // 
            this.lbTheme.AutoSize = true;
            this.lbTheme.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lbTheme.Location = new System.Drawing.Point(14, 10);
            this.lbTheme.Name = "lbTheme";
            this.lbTheme.Size = new System.Drawing.Size(117, 15);
            this.lbTheme.TabIndex = 12;
            this.lbTheme.Text = "Chủ đề / Giao diện:";
            // 
            // cbTheme
            // 
            this.cbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTheme.FormattingEnabled = true;
            this.cbTheme.Location = new System.Drawing.Point(17, 28);
            this.cbTheme.Name = "cbTheme";
            this.cbTheme.Size = new System.Drawing.Size(265, 23);
            this.cbTheme.TabIndex = 13;
            // 
            // cbxDirectSearch
            // 
            this.cbxDirectSearch.AutoSize = true;
            this.cbxDirectSearch.Checked = true;
            this.cbxDirectSearch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxDirectSearch.Location = new System.Drawing.Point(17, 60);
            this.cbxDirectSearch.Name = "cbxDirectSearch";
            this.cbxDirectSearch.Size = new System.Drawing.Size(252, 19);
            this.cbxDirectSearch.TabIndex = 14;
            this.cbxDirectSearch.Text = "Nhập tìm kiếm ngay trên thanh trôi nổi";
            this.cbxDirectSearch.UseVisualStyleBackColor = true;
            // 
            // cbxShowDatabaseName
            // 
            this.cbxShowDatabaseName.AutoSize = true;
            this.cbxShowDatabaseName.Location = new System.Drawing.Point(17, 82);
            this.cbxShowDatabaseName.Name = "cbxShowDatabaseName";
            this.cbxShowDatabaseName.Size = new System.Drawing.Size(225, 19);
            this.cbxShowDatabaseName.TabIndex = 15;
            this.cbxShowDatabaseName.Text = "Hiển thị tên cơ sở dữ liệu trên thanh";
            this.cbxShowDatabaseName.UseVisualStyleBackColor = true;
            // 
            // cbxCompactMode
            // 
            this.cbxCompactMode.AutoSize = true;
            this.cbxCompactMode.Location = new System.Drawing.Point(17, 104);
            this.cbxCompactMode.Name = "cbxCompactMode";
            this.cbxCompactMode.Size = new System.Drawing.Size(225, 19);
            this.cbxCompactMode.TabIndex = 16;
            this.cbxCompactMode.Text = "Chế độ biểu tượng thu gọn (Compact)";
            this.cbxCompactMode.UseVisualStyleBackColor = true;
            // 
            // pnFontColor
            // 
            this.pnFontColor.BackColor = System.Drawing.SystemColors.InfoText;
            this.pnFontColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnFontColor.Location = new System.Drawing.Point(17, 238);
            this.pnFontColor.Name = "pnFontColor";
            this.pnFontColor.Size = new System.Drawing.Size(120, 22);
            this.pnFontColor.TabIndex = 11;
            this.pnFontColor.DoubleClick += new System.EventHandler(this.pnFontColor_DoubleClick);
            // 
            // pnPanelColor
            // 
            this.pnPanelColor.BackColor = System.Drawing.SystemColors.Info;
            this.pnPanelColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnPanelColor.Location = new System.Drawing.Point(17, 180);
            this.pnPanelColor.Name = "pnPanelColor";
            this.pnPanelColor.Size = new System.Drawing.Size(120, 22);
            this.pnPanelColor.TabIndex = 10;
            this.pnPanelColor.DoubleClick += new System.EventHandler(this.pnPanelColor_DoubleClick);
            // 
            // edPercent
            // 
            this.edPercent.Location = new System.Drawing.Point(17, 127);
            this.edPercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.edPercent.Name = "edPercent";
            this.edPercent.Size = new System.Drawing.Size(80, 23);
            this.edPercent.TabIndex = 9;
            this.edPercent.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btChangeFontColor
            // 
            this.btChangeFontColor.Location = new System.Drawing.Point(150, 237);
            this.btChangeFontColor.Name = "btChangeFontColor";
            this.btChangeFontColor.Size = new System.Drawing.Size(75, 24);
            this.btChangeFontColor.TabIndex = 8;
            this.btChangeFontColor.Text = "Đổi màu...";
            this.btChangeFontColor.UseVisualStyleBackColor = true;
            this.btChangeFontColor.Click += new System.EventHandler(this.btChangeFontColor_Click);
            // 
            // btChangePanelColor
            // 
            this.btChangePanelColor.Location = new System.Drawing.Point(150, 179);
            this.btChangePanelColor.Name = "btChangePanelColor";
            this.btChangePanelColor.Size = new System.Drawing.Size(75, 24);
            this.btChangePanelColor.TabIndex = 7;
            this.btChangePanelColor.Text = "Đổi màu...";
            this.btChangePanelColor.UseVisualStyleBackColor = true;
            this.btChangePanelColor.Click += new System.EventHandler(this.btChangePanelColor_Click);
            // 
            // lbPercent
            // 
            this.lbPercent.AutoSize = true;
            this.lbPercent.Location = new System.Drawing.Point(103, 129);
            this.lbPercent.Name = "lbPercent";
            this.lbPercent.Size = new System.Drawing.Size(17, 15);
            this.lbPercent.TabIndex = 4;
            this.lbPercent.Text = "%";
            // 
            // lbFontColor
            // 
            this.lbFontColor.AutoSize = true;
            this.lbFontColor.Location = new System.Drawing.Point(14, 217);
            this.lbFontColor.Name = "lbFontColor";
            this.lbFontColor.Size = new System.Drawing.Size(128, 15);
            this.lbFontColor.TabIndex = 2;
            this.lbFontColor.Text = "Màu chữ (Khi tùy biến):";
            // 
            // lbPanelColor
            // 
            this.lbPanelColor.AutoSize = true;
            this.lbPanelColor.Location = new System.Drawing.Point(14, 159);
            this.lbPanelColor.Name = "lbPanelColor";
            this.lbPanelColor.Size = new System.Drawing.Size(130, 15);
            this.lbPanelColor.TabIndex = 1;
            this.lbPanelColor.Text = "Màu nền (Khi tùy biến):";
            // 
            // lbTransp
            // 
            this.lbTransp.AutoSize = true;
            this.lbTransp.Location = new System.Drawing.Point(14, 108);
            this.lbTransp.Name = "lbTransp";
            this.lbTransp.Size = new System.Drawing.Size(83, 15);
            this.lbTransp.TabIndex = 0;
            this.lbTransp.Text = "Độ trong suốt:";
            // 
            // tsBehavior
            // 
            this.tsBehavior.Controls.Add(this.labelAutoTypeDelay);
            this.tsBehavior.Controls.Add(this.edAutoTypeDelay);
            this.tsBehavior.Controls.Add(this.labelAutoTypeDelayMs);
            this.tsBehavior.Controls.Add(this.label5);
            this.tsBehavior.Controls.Add(this.cbxShortcutWinQuick);
            this.tsBehavior.Controls.Add(this.cbxShortcutAltQuick);
            this.tsBehavior.Controls.Add(this.cbxShortcutShiftQuick);
            this.tsBehavior.Controls.Add(this.cbxShortcutCntrlQuick);
            this.tsBehavior.Controls.Add(this.txtShortcutKeyQuick);
            this.tsBehavior.Controls.Add(this.label6);
            this.tsBehavior.Controls.Add(this.cbxShowLastOne);
            this.tsBehavior.Controls.Add(this.label4);
            this.tsBehavior.Controls.Add(this.cbxShortcutWin);
            this.tsBehavior.Controls.Add(this.cbxShortcutAlt);
            this.tsBehavior.Controls.Add(this.cbxShortcutShift);
            this.tsBehavior.Controls.Add(this.cbxShortcutCntrl);
            this.tsBehavior.Controls.Add(this.txtShortcutKey);
            this.tsBehavior.Controls.Add(this.label3);
            this.tsBehavior.Controls.Add(this.cbxShowFoldersBeforeEntries);
            this.tsBehavior.Controls.Add(this.cbxShowSearch);
            this.tsBehavior.Controls.Add(this.cbxShowClock);
            this.tsBehavior.Controls.Add(this.label2);
            this.tsBehavior.Controls.Add(this.cbStartGroup);
            this.tsBehavior.Controls.Add(this.cbxSortAlphabetical);
            this.tsBehavior.Controls.Add(this.cbURLAction);
            this.tsBehavior.Controls.Add(this.label1);
            this.tsBehavior.Location = new System.Drawing.Point(4, 24);
            this.tsBehavior.Name = "tsBehavior";
            this.tsBehavior.Padding = new System.Windows.Forms.Padding(3);
            this.tsBehavior.Size = new System.Drawing.Size(312, 342);
            this.tsBehavior.TabIndex = 1;
            this.tsBehavior.Text = "Hành vi & Phím tắt";
            this.tsBehavior.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(198, 146);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 15);
            this.label5.TabIndex = 22;
            this.label5.Text = "+";
            // 
            // cbxShortcutWinQuick
            // 
            this.cbxShortcutWinQuick.AutoSize = true;
            this.cbxShortcutWinQuick.Location = new System.Drawing.Point(152, 145);
            this.cbxShortcutWinQuick.Name = "cbxShortcutWinQuick";
            this.cbxShortcutWinQuick.Size = new System.Drawing.Size(47, 19);
            this.cbxShortcutWinQuick.TabIndex = 21;
            this.cbxShortcutWinQuick.Text = "Win";
            this.cbxShortcutWinQuick.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutAltQuick
            // 
            this.cbxShortcutAltQuick.AutoSize = true;
            this.cbxShortcutAltQuick.Location = new System.Drawing.Point(111, 145);
            this.cbxShortcutAltQuick.Name = "cbxShortcutAltQuick";
            this.cbxShortcutAltQuick.Size = new System.Drawing.Size(41, 19);
            this.cbxShortcutAltQuick.TabIndex = 20;
            this.cbxShortcutAltQuick.Text = "Alt";
            this.cbxShortcutAltQuick.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutShiftQuick
            // 
            this.cbxShortcutShiftQuick.AutoSize = true;
            this.cbxShortcutShiftQuick.Location = new System.Drawing.Point(62, 145);
            this.cbxShortcutShiftQuick.Name = "cbxShortcutShiftQuick";
            this.cbxShortcutShiftQuick.Size = new System.Drawing.Size(50, 19);
            this.cbxShortcutShiftQuick.TabIndex = 19;
            this.cbxShortcutShiftQuick.Text = "Shift";
            this.cbxShortcutShiftQuick.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutCntrlQuick
            // 
            this.cbxShortcutCntrlQuick.AutoSize = true;
            this.cbxShortcutCntrlQuick.Location = new System.Drawing.Point(18, 145);
            this.cbxShortcutCntrlQuick.Name = "cbxShortcutCntrlQuick";
            this.cbxShortcutCntrlQuick.Size = new System.Drawing.Size(45, 19);
            this.cbxShortcutCntrlQuick.TabIndex = 18;
            this.cbxShortcutCntrlQuick.Text = "Ctrl";
            this.cbxShortcutCntrlQuick.UseVisualStyleBackColor = true;
            // 
            // txtShortcutKeyQuick
            // 
            this.txtShortcutKeyQuick.Location = new System.Drawing.Point(219, 143);
            this.txtShortcutKeyQuick.MaxLength = 1;
            this.txtShortcutKeyQuick.Name = "txtShortcutKeyQuick";
            this.txtShortcutKeyQuick.Size = new System.Drawing.Size(26, 23);
            this.txtShortcutKeyQuick.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(183, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "Phím tắt tạo mật khẩu nhanh:";
            // 
            // cbxShowLastOne
            // 
            this.cbxShowLastOne.Location = new System.Drawing.Point(18, 260);
            this.cbxShowLastOne.Name = "cbxShowLastOne";
            this.cbxShowLastOne.Size = new System.Drawing.Size(220, 20);
            this.cbxShowLastOne.TabIndex = 16;
            this.cbxShowLastOne.Text = "Hiển thị mục đã dùng gần nhất";
            this.cbxShowLastOne.UseVisualStyleBackColor = true;
            // 
            // labelAutoTypeDelay
            // 
            this.labelAutoTypeDelay.AutoSize = true;
            this.labelAutoTypeDelay.Location = new System.Drawing.Point(15, 290);
            this.labelAutoTypeDelay.Name = "labelAutoTypeDelay";
            this.labelAutoTypeDelay.Size = new System.Drawing.Size(107, 15);
            this.labelAutoTypeDelay.TabIndex = 24;
            this.labelAutoTypeDelay.Text = "Độ trễ tự động gõ:";
            // 
            // edAutoTypeDelay
            // 
            this.edAutoTypeDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.edAutoTypeDelay.Location = new System.Drawing.Point(135, 287);
            this.edAutoTypeDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.edAutoTypeDelay.Name = "edAutoTypeDelay";
            this.edAutoTypeDelay.Size = new System.Drawing.Size(65, 23);
            this.edAutoTypeDelay.TabIndex = 25;
            this.edAutoTypeDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // labelAutoTypeDelayMs
            // 
            this.labelAutoTypeDelayMs.AutoSize = true;
            this.labelAutoTypeDelayMs.Location = new System.Drawing.Point(205, 290);
            this.labelAutoTypeDelayMs.Name = "labelAutoTypeDelayMs";
            this.labelAutoTypeDelayMs.Size = new System.Drawing.Size(23, 15);
            this.labelAutoTypeDelayMs.TabIndex = 26;
            this.labelAutoTypeDelayMs.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(198, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "+";
            // 
            // cbxShortcutWin
            // 
            this.cbxShortcutWin.AutoSize = true;
            this.cbxShortcutWin.Location = new System.Drawing.Point(152, 97);
            this.cbxShortcutWin.Name = "cbxShortcutWin";
            this.cbxShortcutWin.Size = new System.Drawing.Size(47, 19);
            this.cbxShortcutWin.TabIndex = 14;
            this.cbxShortcutWin.Text = "Win";
            this.cbxShortcutWin.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutAlt
            // 
            this.cbxShortcutAlt.AutoSize = true;
            this.cbxShortcutAlt.Location = new System.Drawing.Point(111, 97);
            this.cbxShortcutAlt.Name = "cbxShortcutAlt";
            this.cbxShortcutAlt.Size = new System.Drawing.Size(41, 19);
            this.cbxShortcutAlt.TabIndex = 13;
            this.cbxShortcutAlt.Text = "Alt";
            this.cbxShortcutAlt.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutShift
            // 
            this.cbxShortcutShift.AutoSize = true;
            this.cbxShortcutShift.Location = new System.Drawing.Point(62, 97);
            this.cbxShortcutShift.Name = "cbxShortcutShift";
            this.cbxShortcutShift.Size = new System.Drawing.Size(50, 19);
            this.cbxShortcutShift.TabIndex = 12;
            this.cbxShortcutShift.Text = "Shift";
            this.cbxShortcutShift.UseVisualStyleBackColor = true;
            // 
            // cbxShortcutCntrl
            // 
            this.cbxShortcutCntrl.AutoSize = true;
            this.cbxShortcutCntrl.Location = new System.Drawing.Point(18, 97);
            this.cbxShortcutCntrl.Name = "cbxShortcutCntrl";
            this.cbxShortcutCntrl.Size = new System.Drawing.Size(45, 19);
            this.cbxShortcutCntrl.TabIndex = 11;
            this.cbxShortcutCntrl.Text = "Ctrl";
            this.cbxShortcutCntrl.UseVisualStyleBackColor = true;
            // 
            // txtShortcutKey
            // 
            this.txtShortcutKey.Location = new System.Drawing.Point(219, 95);
            this.txtShortcutKey.MaxLength = 1;
            this.txtShortcutKey.Name = "txtShortcutKey";
            this.txtShortcutKey.Size = new System.Drawing.Size(26, 23);
            this.txtShortcutKey.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(183, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Phím tắt mở thanh trôi nổi:";
            // 
            // cbxShowFoldersBeforeEntries
            // 
            this.cbxShowFoldersBeforeEntries.Location = new System.Drawing.Point(18, 239);
            this.cbxShowFoldersBeforeEntries.Name = "cbxShowFoldersBeforeEntries";
            this.cbxShowFoldersBeforeEntries.Size = new System.Drawing.Size(220, 20);
            this.cbxShowFoldersBeforeEntries.TabIndex = 8;
            this.cbxShowFoldersBeforeEntries.Text = "Hiển thị thư mục trước mục mật khẩu";
            this.cbxShowFoldersBeforeEntries.UseVisualStyleBackColor = true;
            // 
            // cbxShowSearch
            // 
            this.cbxShowSearch.Location = new System.Drawing.Point(18, 218);
            this.cbxShowSearch.Name = "cbxShowSearch";
            this.cbxShowSearch.Size = new System.Drawing.Size(220, 20);
            this.cbxShowSearch.TabIndex = 7;
            this.cbxShowSearch.Text = "Hiển thị ô tìm kiếm trong menu";
            this.cbxShowSearch.UseVisualStyleBackColor = true;
            // 
            // cbxShowClock
            // 
            this.cbxShowClock.Location = new System.Drawing.Point(18, 197);
            this.cbxShowClock.Name = "cbxShowClock";
            this.cbxShowClock.Size = new System.Drawing.Size(220, 20);
            this.cbxShowClock.TabIndex = 6;
            this.cbxShowClock.Text = "Hiển thị đồng hồ";
            this.cbxShowClock.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nhóm bắt đầu:";
            // 
            // cbStartGroup
            // 
            this.cbStartGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStartGroup.FormattingEnabled = true;
            this.cbStartGroup.Location = new System.Drawing.Point(114, 38);
            this.cbStartGroup.Name = "cbStartGroup";
            this.cbStartGroup.Size = new System.Drawing.Size(180, 23);
            this.cbStartGroup.TabIndex = 4;
            // 
            // cbxSortAlphabetical
            // 
            this.cbxSortAlphabetical.Location = new System.Drawing.Point(18, 176);
            this.cbxSortAlphabetical.Name = "cbxSortAlphabetical";
            this.cbxSortAlphabetical.Size = new System.Drawing.Size(220, 20);
            this.cbxSortAlphabetical.TabIndex = 3;
            this.cbxSortAlphabetical.Text = "Sắp xếp theo thứ tự bảng chữ cái";
            this.cbxSortAlphabetical.UseVisualStyleBackColor = true;
            // 
            // cbURLAction
            // 
            this.cbURLAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbURLAction.FormattingEnabled = true;
            this.cbURLAction.Items.AddRange(new object[] {
            "Mở URL trong trình duyệt",
            "Sao chép URL vào khay nhớ tạm"});
            this.cbURLAction.Location = new System.Drawing.Point(114, 9);
            this.cbURLAction.Name = "cbURLAction";
            this.cbURLAction.Size = new System.Drawing.Size(180, 23);
            this.cbURLAction.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Thao tác URL:";
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(320, 415);
            this.Controls.Add(this.pcOptions);
            this.Controls.Add(this.pnButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cài đặt KP Floating Panel";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.pnButtons.ResumeLayout(false);
            this.pcOptions.ResumeLayout(false);
            this.tsView.ResumeLayout(false);
            this.tsView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.edPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edAutoTypeDelay)).EndInit();
            this.tsBehavior.ResumeLayout(false);
            this.tsBehavior.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.CheckBox cbxSortAlphabetical;

        #endregion

        private System.Windows.Forms.Panel pnButtons;
        private System.Windows.Forms.TabControl pcOptions;
        private System.Windows.Forms.TabPage tsView;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.ColorDialog ColorDialog;
        private System.Windows.Forms.Label lbFontColor;
        private System.Windows.Forms.Label lbPanelColor;
        private System.Windows.Forms.Label lbTransp;
        private System.Windows.Forms.Button btChangeFontColor;
        private System.Windows.Forms.Button btChangePanelColor;
        private System.Windows.Forms.Label lbPercent;
        private System.Windows.Forms.Panel pnFontColor;
        private System.Windows.Forms.Panel pnPanelColor;
        private System.Windows.Forms.NumericUpDown edPercent;
        private System.Windows.Forms.TabPage tsBehavior;
        private System.Windows.Forms.ComboBox cbURLAction;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbStartGroup;
		private System.Windows.Forms.CheckBox cbxShowClock;
		private System.Windows.Forms.CheckBox cbxShowSearch;
		private System.Windows.Forms.TextBox txtShortcutKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox cbxShowFoldersBeforeEntries;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox cbxShortcutWin;
		private System.Windows.Forms.CheckBox cbxShortcutAlt;
		private System.Windows.Forms.CheckBox cbxShortcutShift;
		private System.Windows.Forms.CheckBox cbxShortcutCntrl;
		private System.Windows.Forms.CheckBox cbxShowLastOne;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbxShortcutWinQuick;
        private System.Windows.Forms.CheckBox cbxShortcutAltQuick;
        private System.Windows.Forms.CheckBox cbxShortcutShiftQuick;
        private System.Windows.Forms.CheckBox cbxShortcutCntrlQuick;
        private System.Windows.Forms.TextBox txtShortcutKeyQuick;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbTheme;
        private System.Windows.Forms.ComboBox cbTheme;
        private System.Windows.Forms.CheckBox cbxDirectSearch;
        private System.Windows.Forms.CheckBox cbxShowDatabaseName;
        private System.Windows.Forms.CheckBox cbxCompactMode;
        private System.Windows.Forms.Label labelAutoTypeDelay;
        private System.Windows.Forms.NumericUpDown edAutoTypeDelay;
        private System.Windows.Forms.Label labelAutoTypeDelayMs;
    }
}
