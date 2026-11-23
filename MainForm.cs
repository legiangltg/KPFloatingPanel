using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KeePass;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.Resources;
using KeePass.Util;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Cryptography.PasswordGenerator;
using KeePassLib.Security;
using KeePassLib.Utility;

using System.Runtime.InteropServices; // for SetWindowsPos

namespace KPFloatingPanel {
	internal partial class MainForm : Form {
		private string FLastName;
		private OptionsClass FOptions;
		internal IPluginHost Host;

		private const int SpaceForButtons = 220;
		private HotKeyboardHook kb_shortcut;
		private HotKeyboardHook kb_shortcut_quick;
		private HotKeyboardHook.ModifierKeys shortcut_last_mod;
		private string shortcut_last_key = "";
		private HotKeyboardHook.ModifierKeys shortcut_last_mod_quick;
		private string shortcut_last_key_quick = "";
		private int quick_anim = 0;

		private SearchPopup _searchPopup;
		private Point _dragStartPoint;
		private bool _isMouseDown;
		private Timer _autoTypeTimer;

		[DllImport("user32")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32")]
		private static extern bool AttachThreadInput(int nThreadId, int nThreadIdTo, bool bAttach);

		[DllImport("user32")]
		private static extern int GetWindowThreadProcessId(IntPtr hWnd, int unused);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HT_CAPTION = 0x2;

		public void CleanUp() {
			if (_autoTypeTimer != null) {
				_autoTypeTimer.Stop();
				_autoTypeTimer.Dispose();
				_autoTypeTimer = null;
			}
			if (kb_shortcut != null)
				kb_shortcut.Dispose();
			if (kb_shortcut_quick != null)
				kb_shortcut_quick.Dispose();
			if (_searchPopup != null)
				_searchPopup.Dispose();
		}

		public MainForm() {
			InitializeComponent();
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);

			FLastName = "";
			FOptions = new OptionsClass();

			_searchPopup = new SearchPopup();
			_searchPopup.EntryActionRequested += OnSearchEntryActionRequested;

			IconHelper.PopulateImageList(ilIcons);
			picSearchGlyph.Click += delegate(object s, EventArgs e) { txtSearch.Focus(); };
			pnlSearch.Click += delegate(object s, EventArgs e) { txtSearch.Focus(); };

			tmClock_Tick(null, null);
			ApplyOptions();

			Program.Translation.ApplyTo(this);
			Program.Translation.ApplyTo("KeePass.Forms.Plugins.PCFloatingPanel.MainForm.MainMenu", pmMainMenu.Items);
		}

		// prevent stealing focus
		protected override bool ShowWithoutActivation { get { return true; } }

		private void update_shortcut_key_quick() {
			HotKeyboardHook.ModifierKeys m_keys = 0;
			if (FOptions.shortcutShiftQuick)
				m_keys |= HotKeyboardHook.ModifierKeys.Shift;
			if (FOptions.shortcutAltQuick)
				m_keys |= HotKeyboardHook.ModifierKeys.Alt;
			if (FOptions.shortcutCntrlQuick)
				m_keys |= HotKeyboardHook.ModifierKeys.Control;
			if (FOptions.shortcutWinQuick)
				m_keys |= HotKeyboardHook.ModifierKeys.Win;

			if (kb_shortcut_quick != null && (shortcut_last_mod_quick != m_keys || shortcut_last_key_quick != FOptions.shortcutKeyQuick)) {
				kb_shortcut_quick.Dispose();
				kb_shortcut_quick = null;
			}
			if (kb_shortcut_quick == null && !string.IsNullOrEmpty(FOptions.shortcutKeyQuick) && m_keys != 0) {
				kb_shortcut_quick = new HotKeyboardHook();
				kb_shortcut_quick.KeyPressed += handle_keyboard_shortcut_quick;

				char key = FOptions.shortcutKeyQuick[0];
				try {
					kb_shortcut_quick.RegisterHotKey(m_keys, (Keys)key);
				} catch (Exception e) {
					MessageBox.Show("KPFloatingPanel: Unable to register QuickPass hotkey due to: " + e.Message);
				}
				shortcut_last_key_quick = FOptions.shortcutKeyQuick;
				shortcut_last_mod_quick = m_keys;
			}
		}

		private void update_shortcut_key() {
			HotKeyboardHook.ModifierKeys m_keys = 0;
			if (FOptions.shortcutShift)
				m_keys |= HotKeyboardHook.ModifierKeys.Shift;
			if (FOptions.shortcutAlt)
				m_keys |= HotKeyboardHook.ModifierKeys.Alt;
			if (FOptions.shortcutCntrl)
				m_keys |= HotKeyboardHook.ModifierKeys.Control;
			if (FOptions.shortcutWin)
				m_keys |= HotKeyboardHook.ModifierKeys.Win;

			if (kb_shortcut != null && (shortcut_last_mod != m_keys || shortcut_last_key != FOptions.shortcutKey)) {
				kb_shortcut.Dispose();
				kb_shortcut = null;
			}
			if (kb_shortcut == null && !string.IsNullOrEmpty(FOptions.shortcutKey) && m_keys != 0) {
				kb_shortcut = new HotKeyboardHook();
				kb_shortcut.KeyPressed += handle_keyboard_shortcut;

				char key = FOptions.shortcutKey[0];
				try {
					kb_shortcut.RegisterHotKey(m_keys, (Keys)key);
				} catch (Exception e) {
					MessageBox.Show("KPFloatingPanel: Unable to register shortcut hotkey due to: " + e.Message);
				}
				shortcut_last_key = FOptions.shortcutKey;
				shortcut_last_mod = m_keys;
			}
		}

		private bool IsDatabaseLocked() {
			if (Host != null && Host.MainWindow != null) {
				return Host.MainWindow.IsFileLocked(null);
			}
			return false;
		}

		private Color GetBorderColor() {
			bool isDark = FOptions.themeMode == 0;
			bool isLight = FOptions.themeMode == 1;
			if (isDark) return Color.FromArgb(55, 60, 72);
			if (isLight) return Color.FromArgb(203, 213, 225);
			return ControlPaint.Dark(BackColor);
		}

		private Color GetSearchBorderColor() {
			bool isDark = FOptions.themeMode == 0;
			bool isLight = FOptions.themeMode == 1;
			if (isDark) return Color.FromArgb(48, 54, 66);
			if (isLight) return Color.FromArgb(203, 213, 225);
			return ControlPaint.Dark(pnlSearch.BackColor);
		}

		private void ApplyTheme() {
			bool isDark = FOptions.themeMode == 0;
			bool isLight = FOptions.themeMode == 1;
			bool isCustom = FOptions.themeMode == 2;

			Color formBg, formFg, searchBg, searchFg;
			if (isDark) {
				formBg = Color.FromArgb(30, 32, 40);
				formFg = Color.FromArgb(226, 232, 240);
				searchBg = Color.FromArgb(20, 22, 28);
				searchFg = Color.FromArgb(241, 245, 249);
			} else if (isLight) {
				formBg = Color.FromArgb(248, 250, 252);
				formFg = Color.FromArgb(30, 41, 59);
				searchBg = Color.FromArgb(255, 255, 255);
				searchFg = Color.FromArgb(15, 23, 42);
			} else {
				formBg = FOptions.PanelColor;
				formFg = FOptions.FontColor;
				searchBg = ControlPaint.Dark(formBg);
				searchFg = formFg;
			}

			this.BackColor = formBg;
			lbClock.ForeColor = formFg;
			btnMenu.ForeColor = formFg;
			pnlSearch.BackColor = searchBg;
			txtSearch.BackColor = searchBg;
			txtSearch.ForeColor = searchFg;

			ModernMenuColorTable colorTable = new ModernMenuColorTable(isDark, isCustom, formBg, formFg);
			ModernMenuRenderer renderer = new ModernMenuRenderer(colorTable);
			pmPasswords.Renderer = renderer;
			pmMainMenu.Renderer = renderer;

			picSearchGlyph.Image = IconHelper.CreateSearchGlyph(14, isDark ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139));

			bool isLocked = IsDatabaseLocked();
			picIcon.Image = IconHelper.CreateKeyIcon(22, isLocked, isDark);
		}

		public void UpdateShapeRegion() {
			if (Width <= 0 || Height <= 0) return;
			using (GraphicsPath path = IconHelper.CreateCapsule(0, 0, Width, Height)) {
				this.Region = new Region(path);
			}
			this.Invalidate();
		}

		private void UpdateLayout() {
			int h = 34;
			this.Height = h;

			picIcon.Location = new Point(6, 4);
			picIcon.Size = new Size(26, 26);

			bool showDirectSearch = FOptions.searchDirectOnPanel && !FOptions.compactMode;
			int currentX = picIcon.Right;

			if (showDirectSearch) {
				pnlSearch.Visible = true;
				pnlSearch.Location = new Point(currentX + 6, 4);
				pnlSearch.Size = new Size(160, 26);
				currentX = pnlSearch.Right;
			} else {
				pnlSearch.Visible = false;
			}

			if (!string.IsNullOrEmpty(lbClock.Text.Trim())) {
				lbClock.Visible = true;
				lbClock.Location = new Point(currentX + 8, 4);
				lbClock.Height = 26;
				currentX = lbClock.Right;
			} else {
				lbClock.Visible = false;
			}

			btnMenu.Location = new Point(currentX + 4, 4);
			btnMenu.Size = new Size(20, 26);

			int newWidth = btnMenu.Right + 8;
			if (this.Width != newWidth) {
				this.Width = newWidth;
			}

			UpdateShapeRegion();
		}

		private void ApplyOptions() {
			update_shortcut_key();
			update_shortcut_key_quick();

			ApplyTheme();
			Opacity = (double)FOptions.Transparency / 100;
			toolStripTextSearch.Visible = FOptions.showSearch && !FOptions.searchDirectOnPanel;

			UpdateLayout();

			try {
				pmPasswords_Opening(null, null);
			} catch (Exception e) {
				MessageBox.Show(e.Message + " -- " + e.StackTrace);
			}
			TopMost = true;
		}

		public void ResetPosition() {
			if (Host != null && Host.MainWindow != null) {
				Rectangle R = Screen.GetWorkingArea(Host.MainWindow);
				Top = R.Top + 6;
				Left = R.Left + R.Width - Width - SpaceForButtons;
			} else {
				Rectangle R = Screen.PrimaryScreen.WorkingArea;
				Top = R.Top + 6;
				Left = R.Left + R.Width - Width - 100;
			}
		}

		private bool last_show_clock;
		private bool last_show_dbname;
		private void tmClock_Tick(object sender, EventArgs e) {
			bool update_text = (FOptions.showClock != last_show_clock) || (FOptions.showDatabaseName != last_show_dbname);
			last_show_clock = FOptions.showClock;
			last_show_dbname = FOptions.showDatabaseName;

			string S = FOptions.showClock ? DateTime.Now.ToString("HH:mm") : "";
			string display_name = "";
			bool isLocked = false;

			if ((Host != null) && (Host.MainWindow != null) && (Host.Database != null)) {
				if (!Host.MainWindow.IsFileLocked(null)) {
					display_name = UrlUtil.StripExtension(UrlUtil.GetFileName(Host.Database.IOConnectionInfo.Path));
					if (Host.Database.Modified) display_name += "*";
				} else {
					isLocked = true;
					if (Host.MainWindow.DocumentManager != null && Host.MainWindow.DocumentManager.ActiveDocument != null)
						display_name = UrlUtil.StripExtension(UrlUtil.GetFileName(Host.MainWindow.DocumentManager.ActiveDocument.LockedIoc.Path));
					display_name += " [" + KPRes.Locked + "]";
				}
			}

			if (quick_anim != 0) {
				switch (quick_anim) {
					case 3: display_name += " :)"; break;
					case 2: display_name += " :|"; break;
					case 1: display_name += " :O"; break;
				}
				quick_anim--;
			}

			if (FLastName != display_name) {
				toolStripMenuSaveDatabase.Enabled = Host != null && Host.Database != null && Host.Database.IsOpen;
				FLastName = display_name;
				TopMost = true;
				update_text = true;

				bool isDark = FOptions.themeMode == 0;
				picIcon.Image = IconHelper.CreateKeyIcon(22, isLocked, isDark);

				if (isLocked) {
					ttTips.SetToolTip(picIcon, "KeePass: Cơ sở dữ liệu đang khóa. Nhấp để mở khóa.");
					ttTips.SetToolTip(lbClock, "KeePass: Cơ sở dữ liệu đang khóa. Nhấp để mở khóa.");
					if (_searchPopup != null) _searchPopup.HidePopup();
				} else {
					ttTips.SetToolTip(picIcon, "KeePass: " + FLastName + ". Nhấp để mở / Kéo để di chuyển.");
					ttTips.SetToolTip(lbClock, "KeePass: " + FLastName + ". Nhấp để mở / Kéo để di chuyển.");
				}
				ttTips.SetToolTip(btnMenu, "Menu tùy chọn & Cài đặt");
			}

			if (FOptions.showDatabaseName && !string.IsNullOrEmpty(display_name)) {
				if (S.Length > 0)
					S += " - " + display_name;
				else
					S = display_name;
			}

			if (update_text || lbClock.Text != S) {
				lbClock.Text = S;
				UpdateLayout();
			}
		}

		private void miHide_Click(object sender, EventArgs e) {
			if (_searchPopup != null) _searchPopup.HidePopup();
			Hide();
		}

		private void miSetup_Click(object sender, EventArgs e) {
			if (_searchPopup != null) _searchPopup.HidePopup();
			using (OptionsForm FForm = new OptionsForm()) {
				FForm.Host = Host;
				FForm.Options = FOptions;
				if (FForm.ShowDialog() == DialogResult.OK)
					ApplyOptions();
			}
		}

		private void StartDrag(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				_isMouseDown = true;
				_dragStartPoint = Cursor.Position;
			}
		}

		private void CheckDragMove(MouseEventArgs e) {
			if (_isMouseDown && e.Button == MouseButtons.Left) {
				int dx = Math.Abs(Cursor.Position.X - _dragStartPoint.X);
				int dy = Math.Abs(Cursor.Position.Y - _dragStartPoint.Y);
				if (dx > 4 || dy > 4) {
					_isMouseDown = false;
					if (_searchPopup != null) _searchPopup.HidePopup();
					ReleaseCapture();
					SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
				}
			}
		}

		private void EndDrag() {
			_isMouseDown = false;
		}

		private void picIcon_MouseDown(object sender, MouseEventArgs e) { StartDrag(e); }
		private void picIcon_MouseMove(object sender, MouseEventArgs e) { CheckDragMove(e); }
		private void picIcon_MouseUp(object sender, MouseEventArgs e) {
			if (_isMouseDown) {
				_isMouseDown = false;
				if (e.Button == MouseButtons.Left) {
					if (IsDatabaseLocked()) {
						miNoPasswords_Click(null, null);
					} else {
						TogglePasswordsMenu(new Point(picIcon.Left, Height + 8));
					}
				} else if (e.Button == MouseButtons.Right) {
					pmMainMenu.Show(picIcon, e.Location);
				}
			}
		}

		private void lbClock_MouseDown(object sender, MouseEventArgs e) { StartDrag(e); }
		private void lbClock_MouseMove(object sender, MouseEventArgs e) { CheckDragMove(e); }
		private void lbClock_MouseUp(object sender, MouseEventArgs e) {
			if (_isMouseDown) {
				_isMouseDown = false;
				if (e.Button == MouseButtons.Left) {
					if (IsDatabaseLocked()) {
						miNoPasswords_Click(null, null);
					} else {
						TogglePasswordsMenu(new Point(lbClock.Left, Height + 8));
					}
				} else if (e.Button == MouseButtons.Right) {
					pmMainMenu.Show(lbClock, e.Location);
				}
			}
		}

		private void pnlSearch_MouseDown(object sender, MouseEventArgs e) { StartDrag(e); }
		private void pnlSearch_MouseMove(object sender, MouseEventArgs e) { CheckDragMove(e); }
		private void pnlSearch_MouseUp(object sender, MouseEventArgs e) {
			if (_isMouseDown) {
				_isMouseDown = false;
				if (e.Button == MouseButtons.Left) {
					txtSearch.Focus();
				} else if (e.Button == MouseButtons.Right) {
					pmMainMenu.Show(pnlSearch, e.Location);
				}
			}
		}

		private void picSearchGlyph_MouseDown(object sender, MouseEventArgs e) {
			txtSearch.Focus();
		}

		private void btnMenu_MouseDown(object sender, MouseEventArgs e) { StartDrag(e); }
		private void btnMenu_MouseMove(object sender, MouseEventArgs e) { CheckDragMove(e); }
		private void btnMenu_MouseUp(object sender, MouseEventArgs e) {
			if (_isMouseDown) {
				_isMouseDown = false;
				if (_searchPopup != null) _searchPopup.HidePopup();
				pmMainMenu.Show(btnMenu, new Point(0, btnMenu.Height + 8));
			}
		}

		private void MainForm_Paint(object sender, PaintEventArgs e) {
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
			Color borderColor = GetBorderColor();
			using (Pen pen = new Pen(borderColor, 1.2f)) {
				using (GraphicsPath path = IconHelper.CreateCapsule(0.8f, 0.8f, Width - 1.6f, Height - 1.6f)) {
					e.Graphics.DrawPath(pen, path);
				}
			}
		}

		private void pnlSearch_Paint(object sender, PaintEventArgs e) {
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			Color formBg = this.BackColor;
			Color searchBg = pnlSearch.BackColor;
			Color borderColor = GetSearchBorderColor();

			// First fill panel background with form background so no square corners appear
			using (SolidBrush bgBrush = new SolidBrush(formBg)) {
				e.Graphics.FillRectangle(bgBrush, 0, 0, pnlSearch.Width, pnlSearch.Height);
			}

			// Draw the smooth capsule search pill background and border
			using (GraphicsPath path = IconHelper.CreateCapsule(0.5f, 0.5f, pnlSearch.Width - 1f, pnlSearch.Height - 1f)) {
				using (SolidBrush b = new SolidBrush(searchBg)) {
					e.Graphics.FillPath(b, path);
				}
				using (Pen pen = new Pen(borderColor, 1f)) {
					e.Graphics.DrawPath(pen, path);
				}
			}
		}

		private void TogglePasswordsMenu(Point location) {
			if (_searchPopup != null) _searchPopup.HidePopup();
			if (pmPasswords.Visible) {
				pmPasswords.Close();
			} else {
				is_searching = false;
				reload_menu();
				pmPasswords.Show(this, location);
			}
		}

		public bool SetForegroundWindow(Form form, bool force) {
			IntPtr window = form.Handle;
			IntPtr windowForeground = GetForegroundWindow();
			if (window == windowForeground || SetForegroundWindow(window))
				return true;

			if (force == false)
				return false;

			if (windowForeground == IntPtr.Zero)
				return false;

			if (!AttachThreadInput(System.Threading.Thread.CurrentThread.ManagedThreadId, GetWindowThreadProcessId(windowForeground, 0), true))
				return false;

			SetForegroundWindow(window);
			AttachThreadInput(System.Threading.Thread.CurrentThread.ManagedThreadId, GetWindowThreadProcessId(windowForeground, 0), false);
			form.BringToFront();
			form.Focus();
			return (GetForegroundWindow() == window);
		}

		public void handle_keyboard_shortcut_quick(object sender, KeyPressedEventArgs e) {
			ProtectedString str;
			PwGenerator.Generate(out str, Program.Config.PasswordGenerator.AutoGeneratedPasswordsProfile, null, Program.PwGeneratorPool);
			Clipboard.SetText(str.ReadString());
			quick_anim = 3;
		}

		public void handle_keyboard_shortcut(object sender, KeyPressedEventArgs e) {
			TopMost = true;
			BringToFront();
			SetForegroundWindow(this, true);

			if (FOptions.searchDirectOnPanel && pnlSearch.Visible) {
				txtSearch.Focus();
				txtSearch.SelectAll();
			} else {
				pmPasswords_Opening(null, null);
				pmPasswords.Show(this, new Point(0, Height + 8));
				pmPasswords.Focus();
				if (FOptions.showSearch && !IsDatabaseLocked())
					toolStripTextSearch.Focus();
			}
		}

		private void txtSearch_Enter(object sender, EventArgs e) {
		}

		private void txtSearch_Leave(object sender, EventArgs e) {
			if (_searchPopup != null && _searchPopup.Visible) {
				if (!_searchPopup.Bounds.Contains(Cursor.Position)) {
					_searchPopup.HidePopup();
				}
			}
		}

		private void txtSearch_TextChanged(object sender, EventArgs e) {
			string query = txtSearch.Text.Trim();
			if (string.IsNullOrEmpty(query)) {
				if (_searchPopup != null) _searchPopup.HidePopup();
				return;
			}
			PerformSearch(query);
		}

		private void txtSearch_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Down) {
				if (_searchPopup != null && _searchPopup.Visible) {
					_searchPopup.SelectNext();
					e.Handled = true;
					e.SuppressKeyPress = true;
				}
			} else if (e.KeyCode == Keys.Up) {
				if (_searchPopup != null && _searchPopup.Visible) {
					_searchPopup.SelectPrevious();
					e.Handled = true;
					e.SuppressKeyPress = true;
				}
			} else if (e.KeyCode == Keys.Enter) {
				if (_searchPopup != null && _searchPopup.Visible) {
					PwEntry entry = _searchPopup.SelectedEntry;
					if (entry != null) {
						Point screenPt = pnlSearch.PointToScreen(new Point(0, pnlSearch.Height + 8));
						ShowEntryContextMenu(entry, screenPt);
						e.Handled = true;
						e.SuppressKeyPress = true;
					}
				}
			} else if (e.KeyCode == Keys.Escape) {
				if (_searchPopup != null) _searchPopup.HidePopup();
				txtSearch.Text = "";
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void OnSearchEntryActionRequested(PwEntry entry, Point screenLocation, MouseButtons button) {
			if (entry == null) return;
			ShowEntryContextMenu(entry, screenLocation);
		}

		private void ShowEntryContextMenu(PwEntry entry, Point screenLocation) {
			if (entry == null) return;
			if (_searchPopup != null) _searchPopup.HidePopup();

			ContextMenuStrip ctx = new ContextMenuStrip();
			bool isDark = FOptions.themeMode == 0;
			bool isCustom = FOptions.themeMode == 2;
			ctx.Renderer = new ModernMenuRenderer(new ModernMenuColorTable(isDark, isCustom, BackColor, lbClock.ForeColor));

			// Header with Title & Group (bold, non-clickable)
			string title = entry.Strings.ReadSafe(PwDefs.TitleField);
			string groupName = entry.ParentGroup != null ? entry.ParentGroup.Name : "";
			string headerText = !string.IsNullOrEmpty(groupName) ? (title + "  [" + groupName + "]") : title;
			ToolStripMenuItem headerItem = new ToolStripMenuItem(headerText);
			headerItem.Font = new Font(headerItem.Font.FontFamily, 9.5f, FontStyle.Bold);
			headerItem.Enabled = false;
			headerItem.Image = GetEntryIcon(entry);
			ctx.Items.Add(headerItem);
			ctx.Items.Add(new ToolStripSeparator());

			// 1. Tự động gõ (Auto-Type)
			ToolStripMenuItem miAutoType = new ToolStripMenuItem(KPRes.AutoType);
			miAutoType.Image = ilIcons.Images[8];
			miAutoType.Click += delegate(object s, EventArgs e) {
				PerformAutoType(entry);
			};
			ctx.Items.Add(miAutoType);

			ctx.Items.Add(new ToolStripSeparator());

			// 2. Sao chép Tên người dùng (Username)
			string userName = entry.Strings.ReadSafe(PwDefs.UserNameField);
			string userText = KPRes.CopyUserName;
			if (!string.IsNullOrEmpty(userName)) {
				userText += ": " + userName;
			}
			ToolStripMenuItem miUser = new ToolStripMenuItem(userText);
			miUser.Image = ilIcons.Images[10];
			miUser.Click += delegate(object s, EventArgs e) {
				ClipboardUtil.CopyAndMinimize(entry.Strings.GetSafe(PwDefs.UserNameField),
					true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ? Host.MainWindow : null,
					entry, Host.MainWindow.DocumentManager.ActiveDatabase);
				Host.MainWindow.StartClipboardCountdown();
				SetLastOne(entry);
				ClearSearchState();
			};
			ctx.Items.Add(miUser);

			// 3. Sao chép Mật khẩu (Password)
			string pwdText = KPRes.CopyPasswordMenu != null ? KPRes.CopyPasswordMenu.Replace("&", "") : "Sao chép Mật khẩu";
			ToolStripMenuItem miPwd = new ToolStripMenuItem(pwdText);
			miPwd.Image = ilIcons.Images[11];
			miPwd.Click += delegate(object s, EventArgs e) {
				if (PwDefs.IsTanEntry(entry)) {
					entry.ExpiryTime = DateTime.Now;
					entry.Expires = true;
					Host.MainWindow.RefreshEntriesList();
					Host.MainWindow.UpdateUI(false, null, false, null, false, null, true);
				}
				ClipboardUtil.CopyAndMinimize(entry.Strings.GetSafe(PwDefs.PasswordField),
					true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ? Host.MainWindow : null,
					entry, Host.MainWindow.DocumentManager.ActiveDatabase);
				Host.MainWindow.StartClipboardCountdown();
				SetLastOne(entry);
				ClearSearchState();
			};
			ctx.Items.Add(miPwd);

			// 4. URL (Open URL & Copy URL if present)
			string url = entry.Strings.ReadSafe(PwDefs.UrlField);
			if (!string.IsNullOrEmpty(url)) {
				string openUrlText = KPRes.OpenUrl != null ? KPRes.OpenUrl.Replace("&", "") : "Mở URL";
				ToolStripMenuItem miOpenUrl = new ToolStripMenuItem(openUrlText);
				miOpenUrl.Image = ilIcons.Images[9];
				miOpenUrl.Click += delegate(object s, EventArgs e) {
					CustomOpenEntryUrl(entry);
					SetLastOne(entry);
					ClearSearchState();
				};
				ctx.Items.Add(miOpenUrl);

				string copyUrlText = KPRes.CopyUrls != null ? KPRes.CopyUrls.Replace("&", "") : "Sao chép URL";
				ToolStripMenuItem miCopyUrl = new ToolStripMenuItem(copyUrlText);
				miCopyUrl.Image = ilIcons.Images[9];
				miCopyUrl.Click += delegate(object s, EventArgs e) {
					ClipboardUtil.CopyAndMinimize(entry.Strings.GetSafe(PwDefs.UrlField),
						true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ? Host.MainWindow : null,
						entry, Host.MainWindow.DocumentManager.ActiveDatabase);
					Host.MainWindow.StartClipboardCountdown();
					SetLastOne(entry);
					ClearSearchState();
				};
				ctx.Items.Add(miCopyUrl);
			}

			// 5. Additional custom fields (if any)
			bool hasCustomFields = false;
			foreach (KeyValuePair<string, ProtectedString> kvp in entry.Strings) {
				if (!kvp.Key.Equals(PwDefs.TitleField) &&
					!kvp.Key.Equals(PwDefs.UserNameField) &&
					!kvp.Key.Equals(PwDefs.PasswordField) &&
					!kvp.Key.Equals(PwDefs.UrlField) &&
					!kvp.Key.Equals(PwDefs.NotesField)) {
					if (kvp.Value.Length > 0) {
						if (!hasCustomFields) {
							ctx.Items.Add(new ToolStripSeparator());
							hasCustomFields = true;
						}
						ProtectedString pVal = kvp.Value;
						string fKey = kvp.Key;
						ToolStripMenuItem miCustom = new ToolStripMenuItem(fKey + ": " + pVal.ReadString());
						miCustom.Image = ilIcons.Images[5];
						miCustom.Click += delegate(object s, EventArgs e) {
							ClipboardUtil.CopyAndMinimize(pVal,
								true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ? Host.MainWindow : null,
								entry, Host.MainWindow.DocumentManager.ActiveDatabase);
							Host.MainWindow.StartClipboardCountdown();
							SetLastOne(entry);
							ClearSearchState();
						};
						ctx.Items.Add(miCustom);
					}
				}
			}

			ctx.Items.Add(new ToolStripSeparator());

			// 6. Tinh chỉnh / Chỉnh sửa mục... (Edit Entry...)
			string editText = KPRes.EditEntry != null ? KPRes.EditEntry.Replace("&", "") : "Chỉnh sửa mục";
			ToolStripMenuItem miEdit = new ToolStripMenuItem(editText);
			miEdit.Image = ilIcons.Images[1];
			miEdit.Click += delegate(object s, EventArgs e) {
				PwEntryForm myForm = new PwEntryForm();
				myForm.InitEx(entry, PwEditMode.EditExistingEntry, Host.MainWindow.DocumentManager.ActiveDatabase, Host.MainWindow.ClientIcons, false, true);
				if (myForm.ShowDialog() == DialogResult.OK) {
					Host.MainWindow.UpdateUI(false, null, Host.MainWindow.DocumentManager.ActiveDatabase.UINeedsIconUpdate, null, true, null, true);
					Host.MainWindow.RefreshEntriesList();
				}
				ClearSearchState();
			};
			ctx.Items.Add(miEdit);

			ctx.Show(screenLocation);
		}

		private void ClearSearchState() {
			txtSearch.Text = "";
			if (_searchPopup != null) _searchPopup.HidePopup();
		}

		private Image GetEntryIcon(PwEntry entry) {
			if (entry == null) return ilIcons.Images[5];
			try {
				if (entry.CustomIconUuid != PwUuid.Zero && Host != null && Host.Database != null)
					return Host.Database.GetCustomIcon(entry.CustomIconUuid, 16, 16);
				else if (Host != null && Host.MainWindow != null && Host.MainWindow.ClientIcons != null)
					return Host.MainWindow.ClientIcons.Images[(int)entry.IconId];
			} catch { }
			return ilIcons.Images[5];
		}

		private void ExecuteDefaultAction(PwEntry entry) {
			if (entry == null) return;
			SetLastOne(entry);
			if (FOptions.URLAction == 0) {
				CustomOpenEntryUrl(entry);
			} else if (FOptions.URLAction == 1) {
				ClipboardUtil.CopyAndMinimize(entry.Strings.GetSafe(PwDefs.PasswordField),
					true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ?
					Host.MainWindow : null, entry, Host.MainWindow.DocumentManager.ActiveDatabase);
				Host.MainWindow.StartClipboardCountdown();
			}
		}

		private void pmPasswords_Opening(object sender, CancelEventArgs e) {
			is_searching = false;
			if (FOptions.showSearch && !FOptions.searchDirectOnPanel) {
				toolStripTextSearch.Text = "Click To Search";
				toolStripTextSearch.Width = 50;
			}
			if (Host == null)
				return;
			reload_menu();

			if (FOptions.showSearch && !FOptions.searchDirectOnPanel)
				toolStripTextSearch.Width = pmPasswords.DisplayRectangle.Width;
		}

		private void reload_menu() {
			int i = pmPasswords.Items.Count - 1;
			pmPasswords.SuspendLayout();
			while (i >= 5) {
				pmPasswords.Items.Remove(pmPasswords.Items[i]);
				i--;
			}
			miNoPasswords.Visible = true;
			miNoPasswords.Enabled = false;
			miNoPasswords.Image = null;

			if (IsDatabaseLocked()) {
				miNoPasswords.Text = KPRes.LockMenuUnlock;
				miNoPasswords.ToolTipText = "Cơ sở dữ liệu đang khóa. Nhấp để mở.";
				miNoPasswords.Enabled = true;
				toolStripTextSearch.Visible = false;
				miNoPasswords.Image = ilIcons.Images[6];

				LastOne.Visible = false;
				LastOne.Text = "LastOne";
				LastOne.DropDownItems.Clear();
			} else if ((Host.Database != null) && (Host.Database.IsOpen)) {
				toolStripTextSearch.Visible = FOptions.showSearch && !FOptions.searchDirectOnPanel;
				LastOne.Visible = FOptions.showLastOne && LastOne.Text != "LastOne";
				PwGroup start_group = Host.Database.RootGroup;
				if (start_group != null && !string.IsNullOrEmpty(FOptions.startGroupUUID)) {
					PwObjectList<PwGroup> all_groups = Host.Database.RootGroup.GetGroups(true);
					foreach (PwGroup group in all_groups) {
						if (group.Uuid.ToHexString() == FOptions.startGroupUUID) {
							start_group = group;
							break;
						}
					}
					if (start_group.Entries.UCount <= 0 && start_group.Groups.UCount <= 0)
						start_group = Host.Database.RootGroup;
				}

				PwEntry last_entry = LastOne.Tag as PwEntry;
				bool last_entry_ok = false;
				if (last_entry != null) {
					PwGroup tmp_grp = last_entry.ParentGroup;
					while (tmp_grp != null) {
						if (tmp_grp == start_group) {
							last_entry_ok = true;
							break;
						}
						if (tmp_grp == Host.Database.RootGroup)
							break;
						tmp_grp = tmp_grp.ParentGroup;
					}
				}
				if (!last_entry_ok)
					LastOne.Visible = false;

				if ((start_group != null) && ((start_group.Entries.UCount > 0) || (start_group.Groups.UCount > 0))) {
					AddGroupToMenu(null, start_group);
					miNoPasswords.Visible = false;
				} else {
					toolStripTextSearch.Visible = LastOne.Visible = false;
					miNoPasswords.Text = "-= Không có mật khẩu nào =-";
					miNoPasswords.ToolTipText = "Không có mật khẩu nào được định nghĩa.";
				}
			} else {
				toolStripTextSearch.Visible = LastOne.Visible = false;
				miNoPasswords.Text = "-= Chưa mở cơ sở dữ liệu =-";
				miNoPasswords.ToolTipText = "Vui lòng mở hoặc tạo cơ sở dữ liệu trong KeePass.";
			}

			pmPasswords.ResumeLayout();
		}

		private void miNoPasswords_Click(object sender, EventArgs e) {
			if (IsDatabaseLocked()) {
				// Unlock
				ToolStripItem[] items = Host.MainWindow.TrayContextMenu.Items.Find("m_ctxTrayLock", false);
				if (items != null && items.Length > 0)
					items[0].PerformClick();

				if (!Host.MainWindow.IsTrayed()) {
					ToolStripItem[] trayItems = Host.MainWindow.TrayContextMenu.Items.Find("m_ctxTrayTray", false);
					if (trayItems != null && trayItems.Length > 0)
						trayItems[0].PerformClick();
				}
			}
		}

		private void AddActionsToItem(ToolStripMenuItem Parent, PwEntry Entry) {
			ToolStripMenuItem Item;

			foreach (KeyValuePair<string, ProtectedString> kvp in Entry.Strings) {
				if (kvp.Key.Equals(PwDefs.UrlField)) {
					if (kvp.Value.Length > 0) {
						Item = new ToolStripMenuItem();
						Item.Tag = Entry;
						ToolStripMenuItem Tmp;
						if (FOptions.URLAction == 0)
							Tmp = (ToolStripMenuItem)Host.MainWindow.EntryContextMenu.Items.Find("m_ctxEntryOpenUrl", true)[0];
						else if (FOptions.URLAction == 1)
							Tmp = (ToolStripMenuItem)Host.MainWindow.EntryContextMenu.Items.Find("m_ctxEntryCopyUrl", true)[0];
						else
							throw new Exception("Internal error in AddActionsToItem (FOptions.URLAction)");
						Item.Text = Tmp.Text;
						Item.Image = ilIcons.Images[9];
						Item.Click += miItem_OpenURL;
						Parent.DropDownItems.Add(Item);
					}
					break;
				}
			}

			Item = new ToolStripMenuItem();
			Item.Tag = Entry;
			Item.Text = KPRes.AutoType;
			Item.Image = ilIcons.Images[8];
			Item.Click += miItem_AutoType;
			Parent.DropDownItems.Add(Item);

			foreach (KeyValuePair<string, ProtectedString> kvp in Entry.Strings) {
				if ((!kvp.Key.Equals(PwDefs.TitleField)) && (!kvp.Key.Equals(PwDefs.NotesField)) && (!kvp.Key.Equals(PwDefs.UrlField))) {
					if (kvp.Value.Length > 0) {
						Item = new ToolStripMenuItem();
						Item.Tag = Entry;
						Item.Name = kvp.Key;
						Item.Click += miItem_Click;

						if (kvp.Key.Equals(PwDefs.UserNameField)) {
							string uName = Entry.Strings.ReadSafe(PwDefs.UserNameField);
							Item.Text = KPRes.CopyUserName + (!string.IsNullOrEmpty(uName) ? (": " + uName) : "");
							Item.Image = ilIcons.Images[10];
							Parent.DropDownItems.Insert(Parent.DropDownItems.Count, Item);
						} else if (kvp.Key.Equals(PwDefs.PasswordField)) {
							string pText = KPRes.CopyPasswordMenu != null ? KPRes.CopyPasswordMenu.Replace("&", "") : "Sao chép Mật khẩu";
							Item.Text = pText;
							Item.Image = ilIcons.Images[11];
							Parent.DropDownItems.Insert(Parent.DropDownItems.Count, Item);
						} else {
							Item.Text = kvp.Key + ": " + kvp.Value.ReadString();
							Item.Image = ilIcons.Images[5];
							Parent.DropDownItems.Add(Item);
						}
					}
				}
			}

			Item = new ToolStripMenuItem();
			Item.Tag = Entry;
			Item.Text = KPRes.EditEntry != null ? KPRes.EditEntry.Replace("&", "") : "Chỉnh sửa mục";
			Item.Image = ilIcons.Images[1];
			Item.Click += miItem_Edit;
			Parent.DropDownItems.Add(Item);
		}

		private void AddItemsToMenu(ToolStripMenuItem Parent, PwGroup RootGroup) {
			List<PwEntry> myList = RootGroup.GetEntries(is_searching).CloneShallowToList();
			DateTime now = DateTime.Now;
			if (FOptions.sortAlphabetical) {
				myList.Sort((X, Y) => X.Strings.ReadSafe(PwDefs.TitleField).CompareTo(Y.Strings.ReadSafe(PwDefs.TitleField)));
			}

			foreach (PwEntry Entry in myList) {
				ToolStripMenuItem Item = new ToolStripMenuItem();
				Item.Tag = Entry;
				Item.Text = Entry.Strings.ReadSafe(PwDefs.TitleField);
				Item.BackColor = Entry.BackgroundColor;
				Item.DropDownOpening += miItem_DropDownOpening;
				Item.DoubleClick += miItem_OpenURL;
				Item.DoubleClickEnabled = true;

				PwIcon IconIndex = Entry.IconId;
				PwUuid CustomIconID = Entry.CustomIconUuid;

				if (CustomIconID != PwUuid.Zero)
					Item.Image = Host.Database.GetCustomIcon(CustomIconID, 16, 16);
				else
					Item.Image = Host.MainWindow.ClientIcons.Images[(int)IconIndex];
				if (Item.Image == null)
					Item.Image = ilIcons.Images[5];

				if (Entry.Expires) {
					if (Entry.ExpiryTime <= now) {
						Item.Image = ilIcons.Images[7];
						Item.Font = new Font(Item.Font, Item.Font.Style | FontStyle.Strikeout);
					}
					Item.ToolTipText = KPRes.ExpiryTime + ": " + Entry.ExpiryTime;
				}

				if (Parent == null)
					pmPasswords.Items.Add(Item);
				else
					Parent.DropDownItems.Add(Item);

				ToolStripMenuItem Dummy = new ToolStripMenuItem();
				Dummy.Tag = null;
				Dummy.Text = "dummy";
				Dummy.Enabled = false;
				Item.DropDownItems.Add(Dummy);
			}
		}

		private void AddGroupToMenu(ToolStripMenuItem Parent, PwGroup RootGroup) {
			if (!FOptions.foldersFirst)
				AddItemsToMenu(Parent, RootGroup);
			if (!is_searching) {
				DateTime now = DateTime.Now;
				foreach (PwGroup Group in RootGroup.Groups) {
					ToolStripMenuItem Item = new ToolStripMenuItem();
					Item.Tag = Group;
					Item.Text = Group.Name;
					Item.Name = Group.Name;
					Item.DropDownOpening += miGroup_DropDownOpening;

					PwIcon IconIndex = Group.IconId;
					PwUuid CustomIconID = Group.CustomIconUuid;

					if (CustomIconID != PwUuid.Zero)
						Item.Image = Host.Database.GetCustomIcon(CustomIconID, 16, 16);
					else
						Item.Image = Host.MainWindow.ClientIcons.Images[(int)IconIndex];
					if (Item.Image == null)
						Item.Image = ilIcons.Images[4];

					if (Group.Expires) {
						if (Group.ExpiryTime <= now) {
							Item.Image = ilIcons.Images[7];
							Item.Font = new Font(Item.Font, Item.Font.Style | FontStyle.Strikeout);
						}
						Item.ToolTipText = KPRes.ExpiryTime + ": " + Group.ExpiryTime;
					}

					bool bIsRecycleBin = (Host.Database.RecycleBinEnabled && Group.Uuid.Equals(Host.Database.RecycleBinUuid));
					if (!bIsRecycleBin && Group.Entries.UCount + Group.Groups.UCount > 0) {
						if (Parent == null)
							pmPasswords.Items.Add(Item);
						else
							Parent.DropDownItems.Add(Item);
					}

					if ((Group.Groups.UCount > 0) || (Group.Entries.UCount > 0)) {
						ToolStripMenuItem Dummy = new ToolStripMenuItem();
						Dummy.Tag = null;
						Dummy.Text = "dummy";
						Dummy.Enabled = false;
						Item.DropDownItems.Add(Dummy);
					}
				}

				if (RootGroup.Groups.UCount != 0) {
					ToolStripSeparator seperator = new ToolStripSeparator();
					if (Parent == null)
						pmPasswords.Items.Add(seperator);
					else
						Parent.DropDownItems.Add(seperator);
				}
			}
			if (FOptions.foldersFirst)
				AddItemsToMenu(Parent, RootGroup);
		}

		private void miGroup_DropDownOpening(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			if (Item.DropDownItems.Count != 1)
				return;
			ToolStripMenuItem pos_dummy = Item.DropDownItems[0] as ToolStripMenuItem;
			if (pos_dummy == null || pos_dummy.Tag != null)
				return;
			PwGroup Group = (PwGroup)Item.Tag;
			AddGroupToMenu(Item, Group);
			Item.DropDownItems.Remove(pos_dummy);
		}

		private void miItem_DropDownOpening(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			if (Item.DropDownItems.Count != 1)
				return;
			ToolStripMenuItem pos_dummy = Item.DropDownItems[0] as ToolStripMenuItem;
			if (pos_dummy == null || pos_dummy.Tag != null)
				return;
			PwEntry Entry = (PwEntry)Item.Tag;
			AddActionsToItem(Item, Entry);
			Item.DropDownItems.Remove(pos_dummy);
		}

		private void miItem_AutoType(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			PwEntry Entry = (PwEntry)Item.Tag;
			PerformAutoType(Entry);
		}

		private void PerformAutoType(PwEntry entry) {
			if (entry == null || Host == null || Host.Database == null || !Host.Database.IsOpen)
				return;

			SetLastOne(entry);
			ClearSearchState();

			if (_autoTypeTimer != null) {
				_autoTypeTimer.Stop();
				_autoTypeTimer.Dispose();
				_autoTypeTimer = null;
			}

			int delay = FOptions.autoTypeDelay;
			if (delay <= 0) {
				try {
					AutoType.PerformIntoPreviousWindow(this, entry, Host.Database);
				} catch (Exception ex) {
					MessageService.ShowWarning(ex);
				}
				return;
			}

			_autoTypeTimer = new Timer();
			_autoTypeTimer.Interval = delay;
			_autoTypeTimer.Tick += delegate(object s, EventArgs e) {
				if (_autoTypeTimer != null) {
					_autoTypeTimer.Stop();
					_autoTypeTimer.Dispose();
					_autoTypeTimer = null;
				}
				try {
					if (Host != null && Host.Database != null && Host.Database.IsOpen) {
						AutoType.PerformIntoPreviousWindow(this, entry, Host.Database);
					}
				} catch (Exception ex) {
					MessageService.ShowWarning(ex);
				}
			};
			_autoTypeTimer.Start();
		}

		private void SetLastOne(PwEntry entry) {
			if (!FOptions.showLastOne || entry == null)
				return;

			LastOne.Text = entry.Strings.ReadSafe(PwDefs.TitleField);
			LastOne.Tag = entry;
			LastOne.DropDownItems.Clear();
			AddActionsToItem(LastOne, entry);
			LastOne.Visible = true;
		}

		private void SetLastOne(ToolStripMenuItem item) {
			if (!FOptions.showLastOne || item == null)
				return;
			ToolStripMenuItem ParentItem;
			while (item != LastOne && (ParentItem = item.OwnerItem as ToolStripMenuItem) != null && ParentItem.Tag == item.Tag)
				item = ParentItem;

			if (item == LastOne)
				return;

			SetLastOne(item.Tag as PwEntry);
		}

		private void CustomOpenEntryUrl(PwEntry pe) {
			Debug.Assert(pe != null);
			if (pe == null) throw new ArgumentNullException("pe");
			string open_url;

			if (pe.OverrideUrl.Length > 0)
				open_url = pe.OverrideUrl;
			else {
				string strOverride = Program.Config.Integration.UrlOverride;
				open_url = strOverride.Length > 0 ? strOverride : pe.Strings.ReadSafe(PwDefs.UrlField);
			}
			WinUtil.OpenUrl(open_url, pe);
		}

		private void miItem_OpenURL(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			PwEntry Entry = Item.Tag as PwEntry;
			if (Entry == null)
				return;

			if (FOptions.URLAction == 0) {
				CustomOpenEntryUrl(Entry);
				SetLastOne(Item);
			} else if (FOptions.URLAction == 1) {
				FieldAction(Item, PwDefs.UrlField, FIELD_ACTION.CLIPBOARD_COPY);
			} else {
				throw new Exception("Internal error in miItem_OpenURL (FOptions.URLAction)");
			}
		}

		void miItem_Click(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			string fieldName = !string.IsNullOrEmpty(Item.Name) ? Item.Name : Item.Text;
			FieldAction(Item, fieldName, FIELD_ACTION.CLIPBOARD_COPY);
		}

		private enum FIELD_ACTION { CLIPBOARD_COPY, DRAG_DROP };
		private void FieldAction(ToolStripMenuItem Item, string FieldName, FIELD_ACTION action) {
			PwEntry Entry = null;
			if (Item != null)
				Entry = (PwEntry)Item.Tag;
			if (Entry == null)
				return;

			SetLastOne(Item);
			if (FieldName == PwDefs.PasswordField && PwDefs.IsTanEntry(Entry)) {
				Entry.ExpiryTime = DateTime.Now;
				Entry.Expires = true;
				Host.MainWindow.RefreshEntriesList();
				Host.MainWindow.UpdateUI(false, null, false, null, false, null, true);
			}

			if (action == FIELD_ACTION.CLIPBOARD_COPY) {
				ClipboardUtil.CopyAndMinimize(Entry.Strings.GetSafe(FieldName),
					true, Program.Config.MainWindow.MinimizeAfterClipboardCopy ?
					Host.MainWindow : null, Entry, Host.MainWindow.DocumentManager.ActiveDatabase);
				Host.MainWindow.StartClipboardCountdown();
			} else if (action == FIELD_ACTION.DRAG_DROP) {
				Item.DoDragDrop(Entry.Strings.ReadSafe(FieldName), DragDropEffects.Copy);
			}
		}

		private void miItem_Edit(object sender, EventArgs e) {
			ToolStripMenuItem Item = (ToolStripMenuItem)sender;
			PwEntry Entry = (PwEntry)Item.Tag;
			PwEntryForm myForm = new PwEntryForm();
			myForm.InitEx(Entry, PwEditMode.EditExistingEntry, Host.MainWindow.DocumentManager.ActiveDatabase, Host.MainWindow.ClientIcons, false, true);

			if (myForm.ShowDialog() == DialogResult.OK)
				Host.MainWindow.UpdateUI(false, null, Host.MainWindow.DocumentManager.ActiveDatabase.UINeedsIconUpdate, null, true, null, true);
			Host.MainWindow.RefreshEntriesList();
		}

		private void miAbout_Click(object sender, EventArgs e) {
			MessageBox.Show("Floating Panel plugin for KeePass\n" +
							"Modernized floating widget with instant search, smooth UI/UX and dark/light themes.\n\n" +
							"Original by Alexeev Alexander, updated by s², Mitch Capper.",
							"About KPFloatingPanel", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void toolStripMenuSaveDatabase_Click(object sender, EventArgs e) {
			Host.MainWindow.UIFileSave(false);
		}

		private void ToolStripMenuExitKeePass_Click(object sender, EventArgs e) {
			Host.MainWindow.TrayContextMenu.Items.Find("m_ctxTrayFileExit", false)[0].PerformClick();
		}

		private bool is_searching;

		void toolStripTextSearch_TextChanged(object sender, EventArgs e) {
			timer_search.Enabled = false;
			timer_search.Enabled = true;
		}

		private void timer_search_Tick(object sender, EventArgs e) {
			timer_search.Enabled = false;

			string query = toolStripTextSearch.Text;
			if (string.IsNullOrEmpty(query) || query == "Click To Search") {
				is_searching = false;
				reload_menu();
				return;
			}

			is_searching = true;
			PerformMenuSearch(query);
		}

		private void PerformMenuSearch(string query) {
			if (Host == null || Host.Database == null || !Host.Database.IsOpen)
				return;

			PwGroup start_group = Host.Database.RootGroup;
			if (start_group != null && !string.IsNullOrEmpty(FOptions.startGroupUUID)) {
				PwObjectList<PwGroup> all_groups = Host.Database.RootGroup.GetGroups(true);
				foreach (PwGroup group in all_groups) {
					if (group.Uuid.ToHexString() == FOptions.startGroupUUID) {
						start_group = group;
						break;
					}
				}
			}

			if (start_group == null) return;

			List<PwEntry> allEntries = start_group.GetEntries(true).CloneShallowToList();
			string[] search_arr = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			List<PwEntry> matched = new List<PwEntry>();
			foreach (PwEntry entry in allEntries) {
				string title = entry.Strings.ReadSafe(PwDefs.TitleField);
				string user = entry.Strings.ReadSafe(PwDefs.UserNameField);
				string url = entry.Strings.ReadSafe(PwDefs.UrlField);
				string notes = entry.Strings.ReadSafe(PwDefs.NotesField);
				string tags = entry.Tags != null ? string.Join(" ", entry.Tags) : "";
				string groupName = entry.ParentGroup != null ? entry.ParentGroup.Name : "";

				bool match = true;
				foreach (string key in search_arr) {
					if (title.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						user.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						url.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						notes.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						tags.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						groupName.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1) {
						match = false;
						break;
					}
				}
				if (match) matched.Add(entry);
			}

			if (FOptions.sortAlphabetical) {
				matched.Sort((X, Y) => X.Strings.ReadSafe(PwDefs.TitleField).CompareTo(Y.Strings.ReadSafe(PwDefs.TitleField)));
			}

			int i = pmPasswords.Items.Count - 1;
			pmPasswords.SuspendLayout();
			while (i >= 5) {
				pmPasswords.Items.Remove(pmPasswords.Items[i]);
				i--;
			}

			miNoPasswords.Visible = false;
			toolStripMenuSaveDatabase.Visible = false;
			LastOne.Visible = false;

			if (matched.Count == 0) {
				ToolStripMenuItem noMatches = new ToolStripMenuItem("(Không tìm thấy kết quả)");
				noMatches.Enabled = false;
				pmPasswords.Items.Add(noMatches);
			} else {
				DateTime now = DateTime.Now;
				foreach (PwEntry entry in matched) {
					ToolStripMenuItem item = new ToolStripMenuItem();
					item.Tag = entry;
					string title = entry.Strings.ReadSafe(PwDefs.TitleField);
					string user = entry.Strings.ReadSafe(PwDefs.UserNameField);
					if (!string.IsNullOrEmpty(user)) {
						item.Text = title + "  (" + user + ")";
					} else {
						item.Text = title;
					}
					item.BackColor = entry.BackgroundColor;
					item.DropDownOpening += miItem_DropDownOpening;
					item.DoubleClick += miItem_OpenURL;
					item.DoubleClickEnabled = true;
					item.Click += (s, e) => {
						miItem_OpenURL(item, null);
					};

					PwIcon iconIndex = entry.IconId;
					PwUuid customIconId = entry.CustomIconUuid;
					if (customIconId != PwUuid.Zero && Host.Database != null)
						item.Image = Host.Database.GetCustomIcon(customIconId, 16, 16);
					else if (Host.MainWindow != null && Host.MainWindow.ClientIcons != null)
						item.Image = Host.MainWindow.ClientIcons.Images[(int)iconIndex];
					if (item.Image == null)
						item.Image = ilIcons.Images[5];

					if (entry.Expires && entry.ExpiryTime <= now) {
						item.Image = ilIcons.Images[7];
						item.Font = new Font(item.Font, item.Font.Style | FontStyle.Strikeout);
					}

					pmPasswords.Items.Add(item);

					ToolStripMenuItem dummy = new ToolStripMenuItem();
					dummy.Tag = null;
					dummy.Text = "dummy";
					dummy.Enabled = false;
					item.DropDownItems.Add(dummy);
				}
			}

			pmPasswords.ResumeLayout();
		}

		private void PerformSearch(string query) {
			if (Host == null || Host.Database == null || !Host.Database.IsOpen) {
				if (_searchPopup != null) _searchPopup.HidePopup();
				return;
			}

			PwGroup start_group = Host.Database.RootGroup;
			if (start_group != null && !string.IsNullOrEmpty(FOptions.startGroupUUID)) {
				PwObjectList<PwGroup> all_groups = Host.Database.RootGroup.GetGroups(true);
				foreach (PwGroup group in all_groups) {
					if (group.Uuid.ToHexString() == FOptions.startGroupUUID) {
						start_group = group;
						break;
					}
				}
			}

			if (start_group == null) return;

			List<PwEntry> allEntries = start_group.GetEntries(true).CloneShallowToList();
			string[] search_arr = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			List<PwEntry> matched = new List<PwEntry>();
			foreach (PwEntry entry in allEntries) {
				string title = entry.Strings.ReadSafe(PwDefs.TitleField);
				string user = entry.Strings.ReadSafe(PwDefs.UserNameField);
				string url = entry.Strings.ReadSafe(PwDefs.UrlField);
				string notes = entry.Strings.ReadSafe(PwDefs.NotesField);
				string tags = entry.Tags != null ? string.Join(" ", entry.Tags) : "";
				string groupName = entry.ParentGroup != null ? entry.ParentGroup.Name : "";

				bool match = true;
				foreach (string key in search_arr) {
					if (title.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						user.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						url.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						notes.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						tags.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1 &&
						groupName.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) == -1) {
						match = false;
						break;
					}
				}
				if (match) matched.Add(entry);
			}

			if (FOptions.sortAlphabetical) {
				matched.Sort((X, Y) => X.Strings.ReadSafe(PwDefs.TitleField).CompareTo(Y.Strings.ReadSafe(PwDefs.TitleField)));
			}

			if (FOptions.searchDirectOnPanel && pnlSearch.Visible && _searchPopup != null) {
				int popWidth = Math.Max(pnlSearch.Width + 140, 320);
				int visibleCount = Math.Max(1, Math.Min(matched.Count, 7));
				int popHeight = visibleCount * 32 + 8;
				Point screenPos = pnlSearch.PointToScreen(new Point(0, pnlSearch.Height + 8));
				Rectangle screenArea = Screen.FromPoint(screenPos).WorkingArea;
				if (screenPos.Y + popHeight > screenArea.Bottom) {
					screenPos.Y = pnlSearch.PointToScreen(new Point(0, -popHeight - 8)).Y;
				}
				_searchPopup.SetResults(matched, FOptions.themeMode == 0, Host, ilIcons);
				_searchPopup.ShowNoActivate(screenPos.X, screenPos.Y, popWidth, popHeight);
			}
		}

		void toolStripTextSearch_GotFocus(object sender, EventArgs e) {
			if (toolStripTextSearch.Text == "Click To Search") {
				toolStripTextSearch.SelectAll();
			}
		}

		private void toolStripTextSearch_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up && e.KeyCode != Keys.Enter)
				return;
			ToolStrip parent = toolStripTextSearch.GetCurrentParent();
			ToolStripItem prev_item = null;
			ToolStripItem next_item = null;
			bool at_item = false;
			foreach (ToolStripItem itm in parent.Items) {
				if (itm == toolStripTextSearch)
					at_item = true;

				if (itm as ToolStripMenuItem == null)
					continue;

				if (!itm.Visible)
					continue;

				if (!at_item && itm != toolStripTextSearch)
					prev_item = itm;

				if (at_item) {
					next_item = itm;
					break;
				}
			}
			if (e.KeyCode == Keys.Enter && next_item != null) {
				miItem_OpenURL(next_item, null);
			}

			if (e.KeyCode == Keys.Down && next_item != null) {
				e.SuppressKeyPress = true;
				e.Handled = true;
				parent.Focus();
				next_item.Select();
			}
			if (e.KeyCode == Keys.Up && prev_item != null) {
				e.SuppressKeyPress = true;
				e.Handled = true;
				parent.Focus();
				prev_item.Select();
			}
		}

		void pmPasswords_Click(object sender, EventArgs e) {
			if (toolStripTextSearch.Focused)
				pmPasswords.Focus();
		}

		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
				return cp;
			}
		}
	}
}
