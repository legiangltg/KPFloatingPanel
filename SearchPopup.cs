using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Security;

namespace KPFloatingPanel
{
    internal class SearchPopup : Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_SHOWWINDOW = 0x0040;
        public const uint SWP_HIDEWINDOW = 0x0080;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOSIZE = 0x0001;

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 3;

        private List<PwEntry> _results = new List<PwEntry>();
        private int _selectedIndex = -1;
        private int _hoverIndex = -1;
        private int _scrollOffset = 0;
        private bool _isDark = true;
        private IPluginHost _host;
        private ImageList _fallbackIcons;

        private const int ItemHeight = 32;
        private const int MaxVisibleItems = 7;
        private const int PaddingTopBottom = 4;

        public event Action<PwEntry, Point, MouseButtons> EntryActionRequested;

        public PwEntry SelectedEntry
        {
            get
            {
                if (_selectedIndex >= 0 && _selectedIndex < _results.Count)
                    return _results[_selectedIndex];
                if (_results.Count > 0)
                    return _results[0];
                return null;
            }
        }

        public SearchPopup()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            this.Cursor = Cursors.Hand;
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE (Never steal focus)
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW (Do not show in Alt-Tab)
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        public void ShowNoActivate(int x, int y, int width, int height)
        {
            if (!this.IsHandleCreated)
            {
                this.CreateHandle();
            }
            this.Left = x;
            this.Top = y;
            this.Width = width;
            this.Height = height;
            UpdateShapeRegion();
            SetWindowPos(this.Handle, HWND_TOPMOST, x, y, width, height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
            this.Visible = true;
        }

        private void UpdateShapeRegion()
        {
            if (Width <= 0 || Height <= 0) return;
            using (GraphicsPath path = IconHelper.CreateRoundedRect(0, 0, Width, Height, 8f))
            {
                this.Region = new Region(path);
            }
        }

        public void HidePopup()
        {
            this.Visible = false;
            if (this.IsHandleCreated)
            {
                SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_HIDEWINDOW | SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
            }
        }

        public void SetResults(List<PwEntry> results, bool isDark, IPluginHost host, ImageList fallbackIcons)
        {
            _results = results != null ? results : new List<PwEntry>();
            _isDark = isDark;
            _host = host;
            _fallbackIcons = fallbackIcons;

            _scrollOffset = 0;
            _selectedIndex = _results.Count > 0 ? 0 : -1;
            _hoverIndex = -1;

            this.Invalidate();
        }

        public void SelectNext()
        {
            if (_results.Count == 0) return;
            _selectedIndex++;
            if (_selectedIndex >= _results.Count)
                _selectedIndex = 0;

            EnsureVisible(_selectedIndex);
            this.Invalidate();
        }

        public void SelectPrevious()
        {
            if (_results.Count == 0) return;
            _selectedIndex--;
            if (_selectedIndex < 0)
                _selectedIndex = _results.Count - 1;

            EnsureVisible(_selectedIndex);
            this.Invalidate();
        }

        private void EnsureVisible(int index)
        {
            if (index < _scrollOffset)
                _scrollOffset = index;
            else if (index >= _scrollOffset + MaxVisibleItems)
                _scrollOffset = index - MaxVisibleItems + 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color bgColor = _isDark ? Color.FromArgb(24, 26, 33) : Color.FromArgb(255, 255, 255);
            Color borderColor = _isDark ? Color.FromArgb(55, 60, 72) : Color.FromArgb(203, 213, 225);
            Color titleColor = _isDark ? Color.FromArgb(241, 245, 249) : Color.FromArgb(15, 23, 42);
            Color userColor = _isDark ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139);
            Color groupColor = _isDark ? Color.FromArgb(100, 116, 139) : Color.FromArgb(148, 163, 184);
            Color selectBg = _isDark ? Color.FromArgb(45, 52, 66) : Color.FromArgb(226, 232, 240);
            Color hoverBg = _isDark ? Color.FromArgb(34, 38, 48) : Color.FromArgb(241, 245, 249);

            // Fill background
            using (SolidBrush b = new SolidBrush(bgColor))
            {
                g.FillRectangle(b, 0, 0, Width, Height);
            }

            if (_results.Count == 0)
            {
                using (Font italicFont = new Font("Segoe UI", 9f, FontStyle.Italic))
                using (SolidBrush textBrush = new SolidBrush(userColor))
                {
                    g.DrawString("Không tìm thấy mật khẩu nào", italicFont, textBrush, new PointF(16, (Height - 18) / 2f));
                }
            }
            else
            {
                using (Font titleFont = new Font("Segoe UI", 9.25f, FontStyle.Bold))
                using (Font subFont = new Font("Segoe UI", 8.5f, FontStyle.Regular))
                using (Font groupFont = new Font("Segoe UI", 8f, FontStyle.Regular))
                using (SolidBrush titleBrush = new SolidBrush(titleColor))
                using (SolidBrush userBrush = new SolidBrush(userColor))
                using (SolidBrush grpBrush = new SolidBrush(groupColor))
                {
                    int visibleCount = Math.Min(_results.Count - _scrollOffset, MaxVisibleItems);
                    for (int i = 0; i < visibleCount; i++)
                    {
                        int dataIndex = _scrollOffset + i;
                        if (dataIndex >= _results.Count) break;

                        PwEntry entry = _results[dataIndex];
                        int itemY = PaddingTopBottom + (i * ItemHeight);

                        // Highlight background
                        if (dataIndex == _selectedIndex)
                        {
                            using (GraphicsPath selPath = IconHelper.CreateRoundedRect(4, itemY, Width - 8, ItemHeight, 4f))
                            using (SolidBrush selBrush = new SolidBrush(selectBg))
                            {
                                g.FillPath(selBrush, selPath);
                            }
                        }
                        else if (dataIndex == _hoverIndex)
                        {
                            using (GraphicsPath hoverPath = IconHelper.CreateRoundedRect(4, itemY, Width - 8, ItemHeight, 4f))
                            using (SolidBrush hBrush = new SolidBrush(hoverBg))
                            {
                                g.FillPath(hBrush, hoverPath);
                            }
                        }

                        // Icon
                        Image icon = null;
                        if (entry.CustomIconUuid != PwUuid.Zero && _host != null && _host.Database != null)
                            icon = _host.Database.GetCustomIcon(entry.CustomIconUuid, 16, 16);
                        else if (_host != null && _host.MainWindow != null && _host.MainWindow.ClientIcons != null)
                            icon = _host.MainWindow.ClientIcons.Images[(int)entry.IconId];

                        if (icon == null && _fallbackIcons != null && _fallbackIcons.Images.Count > 5)
                            icon = _fallbackIcons.Images[5];

                        if (icon != null)
                        {
                            g.DrawImage(icon, new Rectangle(10, itemY + 8, 16, 16));
                        }

                        // Text: Title
                        string title = entry.Strings.ReadSafe(PwDefs.TitleField);
                        string user = entry.Strings.ReadSafe(PwDefs.UserNameField);
                        string groupName = entry.ParentGroup != null ? entry.ParentGroup.Name : "";

                        float textX = 32f;
                        g.DrawString(title, titleFont, titleBrush, textX, itemY + 6);

                        SizeF titleSize = g.MeasureString(title, titleFont);
                        float userX = textX + titleSize.Width + 4;

                        // Username
                        if (!string.IsNullOrEmpty(user) && userX < Width - 80)
                        {
                            string userStr = "(" + user + ")";
                            g.DrawString(userStr, subFont, userBrush, userX, itemY + 7);
                        }

                        // Group name (right aligned)
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            SizeF grpSize = g.MeasureString(groupName, groupFont);
                            float grpX = Width - grpSize.Width - 12;
                            if (grpX > userX + 30)
                            {
                                g.DrawString(groupName, groupFont, grpBrush, grpX, itemY + 8);
                            }
                        }
                    }
                }

                // Scrollbar indicator
                if (_results.Count > MaxVisibleItems)
                {
                    float total = _results.Count;
                    float thumbHeight = Math.Max(16f, (MaxVisibleItems / total) * (Height - 12));
                    float thumbY = 6 + (_scrollOffset / (total - MaxVisibleItems)) * (Height - 12 - thumbHeight);
                    Color scrollColor = _isDark ? Color.FromArgb(70, 78, 92) : Color.FromArgb(180, 190, 205);
                    using (SolidBrush sb = new SolidBrush(scrollColor))
                    using (GraphicsPath sp = IconHelper.CreateRoundedRect(Width - 6, thumbY, 3, thumbHeight, 1.5f))
                    {
                        g.FillPath(sb, sp);
                    }
                }
            }

            // Draw border
            using (GraphicsPath borderPath = IconHelper.CreateRoundedRect(0.8f, 0.8f, Width - 1.6f, Height - 1.6f, 7.5f))
            using (Pen borderPen = new Pen(borderColor, 1.2f))
            {
                g.DrawPath(borderPen, borderPath);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Y < PaddingTopBottom)
            {
                if (_hoverIndex != -1)
                {
                    _hoverIndex = -1;
                    this.Invalidate();
                }
                return;
            }
            int idx = (e.Y - PaddingTopBottom) / ItemHeight + _scrollOffset;
            if (idx >= 0 && idx < _results.Count)
            {
                if (_hoverIndex != idx)
                {
                    _hoverIndex = idx;
                    this.Invalidate();
                }
            }
            else
            {
                if (_hoverIndex != -1)
                {
                    _hoverIndex = -1;
                    this.Invalidate();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverIndex != -1)
            {
                _hoverIndex = -1;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Y < PaddingTopBottom) return;
            int idx = (e.Y - PaddingTopBottom) / ItemHeight + _scrollOffset;
            if (idx >= 0 && idx < _results.Count)
            {
                _selectedIndex = idx;
                PwEntry entry = _results[idx];
                this.Invalidate();

                if (EntryActionRequested != null)
                {
                    EntryActionRequested(entry, Cursor.Position, e.Button);
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_results.Count > MaxVisibleItems)
            {
                if (e.Delta < 0)
                {
                    if (_scrollOffset < _results.Count - MaxVisibleItems)
                        _scrollOffset++;
                }
                else
                {
                    if (_scrollOffset > 0)
                        _scrollOffset--;
                }
                this.Invalidate();
            }
        }
    }
}
