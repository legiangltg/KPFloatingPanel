using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KPFloatingPanel
{
    internal class ModernMenuColorTable : ProfessionalColorTable
    {
        private readonly bool _isDark;
        private readonly Color _customBg;
        private readonly Color _customFg;
        private readonly bool _useCustom;

        public ModernMenuColorTable(bool isDark, bool useCustom, Color customBg, Color customFg)
        {
            _isDark = isDark;
            _useCustom = useCustom;
            _customBg = customBg;
            _customFg = customFg;
        }

        public override Color ToolStripDropDownBackground
        {
            get
            {
                if (_useCustom) return _customBg;
                return _isDark ? Color.FromArgb(30, 32, 40) : Color.FromArgb(255, 255, 255);
            }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return ToolStripDropDownBackground; }
        }

        public override Color ImageMarginGradientMiddle
        {
            get { return ToolStripDropDownBackground; }
        }

        public override Color ImageMarginGradientEnd
        {
            get { return ToolStripDropDownBackground; }
        }

        public override Color MenuBorder
        {
            get
            {
                if (_useCustom) return ControlPaint.Dark(_customBg);
                return _isDark ? Color.FromArgb(55, 60, 72) : Color.FromArgb(226, 232, 240);
            }
        }

        public override Color MenuItemSelected
        {
            get
            {
                if (_useCustom) return ControlPaint.Light(_customBg);
                return _isDark ? Color.FromArgb(48, 54, 66) : Color.FromArgb(241, 245, 249);
            }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return MenuItemSelected; }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return MenuItemSelected; }
        }

        public override Color MenuItemBorder
        {
            get { return MenuItemSelected; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return MenuItemSelected; }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get { return MenuItemSelected; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return MenuItemSelected; }
        }

        public override Color SeparatorDark
        {
            get
            {
                if (_useCustom) return ControlPaint.Dark(_customBg);
                return _isDark ? Color.FromArgb(48, 52, 63) : Color.FromArgb(226, 232, 240);
            }
        }

        public override Color SeparatorLight
        {
            get { return Color.Transparent; }
        }

        public Color TextColor
        {
            get
            {
                if (_useCustom) return _customFg;
                return _isDark ? Color.FromArgb(241, 245, 249) : Color.FromArgb(30, 41, 59);
            }
        }

        public Color SubTextColor
        {
            get
            {
                if (_useCustom) return ControlPaint.Dark(_customFg);
                return _isDark ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 116, 139);
            }
        }
    }

    internal class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly ModernMenuColorTable _colors;

        public ModernMenuRenderer(ModernMenuColorTable colors) : base(colors)
        {
            _colors = colors;
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using (Pen pen = new Pen(_colors.MenuBorder, 1.2f))
            {
                Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Enabled) return;

            if (e.Item.Selected || e.Item.Pressed)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle rect = new Rectangle(3, 1, e.Item.Width - 6, e.Item.Height - 2);
                using (GraphicsPath path = IconHelper.CreateRoundedRect(rect.X, rect.Y, rect.Width, rect.Height, 4f))
                {
                    using (SolidBrush brush = new SolidBrush(_colors.MenuItemSelected))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled ? _colors.TextColor : _colors.SubTextColor;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            int y = e.Item.Height / 2;
            using (Pen pen = new Pen(_colors.SeparatorDark, 1f))
            {
                g.DrawLine(pen, 28, y, e.Item.Width - 8, y);
            }
        }
    }
}
