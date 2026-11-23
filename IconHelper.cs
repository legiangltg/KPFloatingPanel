using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KPFloatingPanel
{
    internal static class IconHelper
    {
        public static Bitmap CreateKeyIcon(int size, bool isLocked, bool isDark)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (isLocked)
                {
                    // Draw modern lock icon (coral / red accent)
                    Color lockBodyColor = Color.FromArgb(239, 68, 68);
                    Color shackleColor = isDark ? Color.FromArgb(203, 213, 225) : Color.FromArgb(71, 85, 105);

                    float shackleW = size * 0.46f;
                    float shackleH = size * 0.44f;
                    float shackleX = (size - shackleW) / 2f;
                    float shackleY = size * 0.12f;
                    float stroke = Math.Max(1.8f, size * 0.11f);

                    using (Pen shacklePen = new Pen(shackleColor, stroke))
                    {
                        shacklePen.StartCap = LineCap.Round;
                        shacklePen.EndCap = LineCap.Round;
                        using (GraphicsPath sp = new GraphicsPath())
                        {
                            sp.AddArc(shackleX, shackleY, shackleW, shackleH, 180, 180);
                            sp.AddLine(shackleX + shackleW, shackleY + shackleH / 2f, shackleX + shackleW, shackleY + shackleH);
                            sp.AddLine(shackleX, shackleY + shackleH / 2f, shackleX, shackleY + shackleH);
                            g.DrawPath(shacklePen, sp);
                        }
                    }

                    // Body
                    float bodyW = size * 0.68f;
                    float bodyH = size * 0.50f;
                    float bodyX = (size - bodyW) / 2f;
                    float bodyY = size * 0.42f;
                    float corner = size * 0.12f;

                    using (GraphicsPath bp = CreateRoundedRect(bodyX, bodyY, bodyW, bodyH, corner))
                    {
                        using (Brush b = new SolidBrush(lockBodyColor))
                            g.FillPath(b, bp);
                    }

                    // Keyhole
                    float holeSize = size * 0.16f;
                    float holeX = (size - holeSize) / 2f;
                    float holeY = bodyY + bodyH * 0.28f;
                    using (Brush kb = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(kb, holeX, holeY, holeSize, holeSize);
                        g.FillPolygon(kb, new PointF[] {
                            new PointF(size * 0.46f, holeY + holeSize * 0.6f),
                            new PointF(size * 0.54f, holeY + holeSize * 0.6f),
                            new PointF(size * 0.56f, bodyY + bodyH * 0.80f),
                            new PointF(size * 0.44f, bodyY + bodyH * 0.80f)
                        });
                    }
                }
                else
                {
                    // Draw modern KeePass key icon (golden gradient / vibrant blue accent)
                    Color keyColor = Color.FromArgb(245, 158, 11); // Golden Amber
                    Color keyRingColor = Color.FromArgb(251, 191, 36);

                    // Background soft glow circle
                    float pad = size * 0.08f;
                    float circleSize = size - (pad * 2f);
                    Color bgGlow = isDark ? Color.FromArgb(40, 59, 130, 246) : Color.FromArgb(25, 37, 99, 235);
                    using (Brush glowBrush = new SolidBrush(bgGlow))
                    {
                        g.FillEllipse(glowBrush, pad, pad, circleSize, circleSize);
                    }

                    // Draw diagonal key
                    // Transform to diagonal (45 degrees)
                    g.TranslateTransform(size / 2f, size / 2f);
                    g.RotateTransform(-45f);

                    float stemW = Math.Max(2.0f, size * 0.13f);
                    float bowR = size * 0.28f;
                    float bowHoleR = size * 0.13f;

                    // Bow (ring of the key)
                    float bowX = -size * 0.32f;
                    float bowY = -bowR / 2f;
                    using (Pen keyPen = new Pen(keyRingColor, stemW * 1.1f))
                    {
                        g.DrawEllipse(keyPen, bowX, bowY, bowR, bowR);
                    }

                    // Stem
                    float stemStart = bowX + bowR - (stemW * 0.4f);
                    float stemLength = size * 0.50f;
                    using (Pen stemPen = new Pen(keyColor, stemW))
                    {
                        stemPen.StartCap = LineCap.Round;
                        stemPen.EndCap = LineCap.Round;
                        g.DrawLine(stemPen, stemStart, 0, stemStart + stemLength, 0);

                        // Bit teeth
                        float tooth1X = stemStart + stemLength * 0.62f;
                        float tooth2X = stemStart + stemLength * 0.90f;
                        float toothH1 = size * 0.18f;
                        float toothH2 = size * 0.24f;

                        g.DrawLine(stemPen, tooth1X, 0, tooth1X, toothH1);
                        g.DrawLine(stemPen, tooth2X, 0, tooth2X, toothH2);
                    }

                    g.ResetTransform();
                }
            }
            return bmp;
        }

        public static Bitmap CreateSearchGlyph(int size, Color color)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                float r = size * 0.30f;
                float cx = size * 0.38f;
                float cy = size * 0.38f;
                float stroke = Math.Max(1.6f, size * 0.13f);

                using (Pen p = new Pen(color, stroke))
                {
                    g.DrawEllipse(p, cx - r, cy - r, r * 2f, r * 2f);

                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;

                    float handleStartX = cx + r * 0.72f;
                    float handleStartY = cy + r * 0.72f;
                    float handleEndX = size * 0.88f;
                    float handleEndY = size * 0.88f;
                    g.DrawLine(p, handleStartX, handleStartY, handleEndX, handleEndY);
                }
            }
            return bmp;
        }

        public static Bitmap CreateSaveIcon(int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Color bodyColor = Color.FromArgb(59, 130, 246);
                Color labelColor = Color.FromArgb(241, 245, 249);

                float p = size * 0.15f;
                float s = size * 0.70f;
                using (GraphicsPath path = CreateRoundedRect(p, p, s, s, size * 0.08f))
                {
                    using (Brush b = new SolidBrush(bodyColor))
                        g.FillPath(b, path);
                }

                // Inner label
                float lw = s * 0.55f;
                float lh = s * 0.40f;
                float lx = (size - lw) / 2f;
                using (Brush b = new SolidBrush(labelColor))
                {
                    g.FillRectangle(b, lx, p, lw, lh);
                }
            }
            return bmp;
        }

        public static Bitmap CreateActionIcon(string type, int size)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Color c = Color.FromArgb(59, 130, 246);

                switch (type.ToLowerInvariant())
                {
                    case "autotype":
                    case "keyboard":
                        // Draw mini keyboard
                        c = Color.FromArgb(16, 185, 129); // emerald
                        using (GraphicsPath p = CreateRoundedRect(size * 0.1f, size * 0.25f, size * 0.8f, size * 0.5f, size * 0.08f))
                        {
                            using (Pen pen = new Pen(c, 1.5f))
                                g.DrawPath(pen, p);
                            using (Brush b = new SolidBrush(c))
                            {
                                float dotSize = size * 0.09f;
                                g.FillEllipse(b, size * 0.25f, size * 0.38f, dotSize, dotSize);
                                g.FillEllipse(b, size * 0.45f, size * 0.38f, dotSize, dotSize);
                                g.FillEllipse(b, size * 0.65f, size * 0.38f, dotSize, dotSize);
                                g.FillRectangle(b, size * 0.32f, size * 0.55f, size * 0.36f, dotSize * 0.8f);
                            }
                        }
                        break;

                    case "url":
                    case "globe":
                        // Draw mini globe
                        c = Color.FromArgb(14, 165, 233); // sky blue
                        using (Pen pen = new Pen(c, 1.4f))
                        {
                            g.DrawEllipse(pen, size * 0.15f, size * 0.15f, size * 0.70f, size * 0.70f);
                            g.DrawEllipse(pen, size * 0.32f, size * 0.15f, size * 0.36f, size * 0.70f);
                            g.DrawLine(pen, size * 0.15f, size * 0.50f, size * 0.85f, size * 0.50f);
                        }
                        break;

                    case "username":
                    case "user":
                        // Draw mini user avatar
                        c = Color.FromArgb(168, 85, 247); // purple
                        using (Brush b = new SolidBrush(c))
                        {
                            g.FillEllipse(b, size * 0.35f, size * 0.15f, size * 0.30f, size * 0.30f);
                            using (GraphicsPath p = new GraphicsPath())
                            {
                                p.AddArc(size * 0.20f, size * 0.50f, size * 0.60f, size * 0.50f, 180, 180);
                                g.FillPath(b, p);
                            }
                        }
                        break;

                    case "password":
                    case "key":
                        // Mini key
                        c = Color.FromArgb(245, 158, 11); // amber
                        using (Pen pen = new Pen(c, 1.6f))
                        {
                            g.DrawEllipse(pen, size * 0.15f, size * 0.28f, size * 0.32f, size * 0.32f);
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, size * 0.47f, size * 0.44f, size * 0.82f, size * 0.44f);
                            g.DrawLine(pen, size * 0.68f, size * 0.44f, size * 0.68f, size * 0.64f);
                            g.DrawLine(pen, size * 0.82f, size * 0.44f, size * 0.82f, size * 0.68f);
                        }
                        break;

                    case "edit":
                        // Pencil
                        c = Color.FromArgb(234, 88, 12); // orange
                        using (Pen pen = new Pen(c, 2.0f))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, size * 0.22f, size * 0.78f, size * 0.78f, size * 0.22f);
                        }
                        break;

                    case "folder":
                        c = Color.FromArgb(234, 179, 8); // yellow-gold
                        using (Brush b = new SolidBrush(c))
                        {
                            PointF[] pts = new PointF[] {
                                new PointF(size * 0.12f, size * 0.28f),
                                new PointF(size * 0.42f, size * 0.28f),
                                new PointF(size * 0.52f, size * 0.38f),
                                new PointF(size * 0.88f, size * 0.38f),
                                new PointF(size * 0.88f, size * 0.78f),
                                new PointF(size * 0.12f, size * 0.78f)
                            };
                            g.FillPolygon(b, pts);
                        }
                        break;

                    case "lock":
                        return CreateKeyIcon(size, true, true);

                    case "expired":
                        // Red X
                        using (Pen pen = new Pen(Color.FromArgb(239, 68, 68), 2.2f))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.DrawLine(pen, size * 0.22f, size * 0.22f, size * 0.78f, size * 0.78f);
                            g.DrawLine(pen, size * 0.78f, size * 0.22f, size * 0.22f, size * 0.78f);
                        }
                        break;

                    default:
                        // Generic bullet / key
                        using (Brush b = new SolidBrush(c))
                        {
                            g.FillEllipse(b, size * 0.28f, size * 0.28f, size * 0.44f, size * 0.44f);
                        }
                        break;
                }
            }
            return bmp;
        }

        public static GraphicsPath CreateCapsule(float x, float y, float width, float height)
        {
            GraphicsPath path = new GraphicsPath();
            if (width <= 0 || height <= 0) return path;

            float r = height / 2f;
            if (r * 2f > width) r = width / 2f;
            float d = r * 2f;

            // Left semicircle: sweeps from bottom (90 deg) through left to top (270 deg)
            path.AddArc(x, y, d, d, 90, 180);
            // Top connecting line
            path.AddLine(x + r, y, x + width - r, y);
            // Right semicircle: sweeps from top (270 deg) through right to bottom (90 deg)
            path.AddArc(x + width - d, y, d, d, 270, 180);
            // Bottom connecting line
            path.AddLine(x + width - r, y + height, x + r, y + height);

            path.CloseFigure();
            return path;
        }

        public static GraphicsPath CreateRoundedRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (width <= 0 || height <= 0) return path;

            float maxR = Math.Min(width, height) / 2f;
            if (radius > maxR) radius = maxR;
            if (radius <= 0.1f)
            {
                path.AddRectangle(new RectangleF(x, y, width, height));
                return path;
            }

            float d = radius * 2f;

            // Top-left arc (180 to 270 deg)
            path.AddArc(x, y, d, d, 180, 90);
            // Top line
            path.AddLine(x + radius, y, x + width - radius, y);
            // Top-right arc (270 to 360/0 deg)
            path.AddArc(x + width - d, y, d, d, 270, 90);
            // Right line
            path.AddLine(x + width, y + radius, x + width, y + height - radius);
            // Bottom-right arc (0 to 90 deg)
            path.AddArc(x + width - d, y + height - d, d, d, 0, 90);
            // Bottom line
            path.AddLine(x + width - radius, y + height, x + radius, y + height);
            // Bottom-left arc (90 to 180 deg)
            path.AddArc(x, y + height - d, d, d, 90, 90);
            // Left line
            path.AddLine(x, y + height - radius, x, y + radius);

            path.CloseFigure();
            return path;
        }

        public static void PopulateImageList(ImageList il)
        {
            if (il == null) return;
            il.Images.Clear();
            il.ImageSize = new Size(16, 16);
            il.ColorDepth = ColorDepth.Depth32Bit;

            // 0: Main plugin icon
            il.Images.Add("0.ico", CreateKeyIcon(16, false, false));
            // 1: Edit entry
            il.Images.Add("1.ico", CreateActionIcon("edit", 16));
            // 2: Placeholder
            il.Images.Add("2.ico", CreateKeyIcon(16, false, true));
            // 3: Placeholder
            il.Images.Add("3.ico", CreateActionIcon("key", 16));
            // 4: Group / Folder
            il.Images.Add("4.ico", CreateActionIcon("folder", 16));
            // 5: Entry fallback
            il.Images.Add("5.ico", CreateActionIcon("key", 16));
            // 6: Lock
            il.Images.Add("B16x16_LockWorkspace.png", CreateKeyIcon(16, true, true));
            // 7: Expired
            il.Images.Add("x.png", CreateActionIcon("expired", 16));
            // 8: AutoType
            il.Images.Add("3.png", CreateActionIcon("autotype", 16));
            // 9: URL
            il.Images.Add("2.png", CreateActionIcon("url", 16));
            // 10: Username
            il.Images.Add("4.png", CreateActionIcon("username", 16));
            // 11: Password
            il.Images.Add("5.png", CreateActionIcon("password", 16));
            // 12: Generic
            il.Images.Add("1.png", CreateActionIcon("key", 16));
        }
    }
}
