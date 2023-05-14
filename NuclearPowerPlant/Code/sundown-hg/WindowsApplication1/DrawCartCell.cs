using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using System.Diagnostics;
using System.Xml;

namespace corelib
{
    public abstract class DrawCartCell
    {
        public abstract void UpdateCart(IDataCartogram c, CoordsConverter validCoords);
        public abstract Size GetLegendMinSize(Size canvasSize);
        public abstract void DrawCell(Graphics g, Point p, Size s, Coords o);
        public abstract void DrawLegend(Graphics g, Point p, Size s);

        public virtual void OnMouseUp(CartView obj, MouseEventArgs e)
        {
        }


        static readonly StringFormat _vertTicksSt;

        static DrawCartCell()
        {
            _vertTicksSt = new StringFormat();
            _vertTicksSt.Alignment = StringAlignment.Near;
            _vertTicksSt.LineAlignment = StringAlignment.Center;
            _vertTicksSt.FormatFlags = StringFormatFlags.NoWrap;
        }

        public static void DrawBarWithNotes(Graphics g, Rectangle bar, SizeF strf, Font fnt, int ticks, double min, double max, Color st, Color end)
        {
            int ticklen = bar.Width / 5;

            Brush b = (end != st) ?
                (Brush)(new LinearGradientBrush(new Point(0, bar.Y), new Point(0, bar.Height + bar.Y), st, end)) :
                (Brush)(new SolidBrush(st));

            g.FillRectangle(b, bar);

            g.DrawRectangle(Pens.Black, bar);
            int dorder = (int)Math.Ceiling(Math.Log10(Math.Max(Math.Abs(max), Math.Abs(min))));

            for (int i = 0; i < ticks; i++)
            {
                int my = bar.Y + i * bar.Height / (ticks - 1);
                int mx = bar.X + bar.Width;

                g.DrawLine(Pens.Black, new Point(mx, my), new Point(mx + ticklen, my));

                double val = (min + (max - min) * (((double)(ticks - 1 - i)) / ((double)(ticks - 1))));
                string str = FillIntellDouble(dorder, val);

                g.DrawString(str, fnt, Brushes.Black,
                    new RectangleF(new PointF((float)(mx + 2 * ticklen), (float)my - (strf.Height / 2.0f)), strf), _vertTicksSt);
            }
            b.Dispose();
        }

        public static string FillIntellDouble(int order, double val)
        {
            string str;

            if (order > 4)
            {
                if (val == 0.0)
                    str = "0";
                else
                    str = String.Format("{0:0.000E0}", val);
            }
            else if (order > 3)
                str = String.Format("{0}", (int)val);
            else if (order > 2)
                str = String.Format("{0:0.0}", val);
            else if (order > 1)
                str = String.Format("{0:0.00}", val);
            else if (order > 0)
                str = String.Format("{0:0.000}", val);
            else if (order > -1)
                str = String.Format("{0:0.0000}", val);
            else
            {
                if (val == 0.0)
                    str = "0";
                else
                    str = String.Format("{0:0.000E0}", val);
            }

            return str;
        }


        public static int[] CalcHystogram(IDataCartogram c, CoordsConverter validCoords, double from, double to, bool includeLowerBound, int bands, ref int totalFound, out double[] ranges)
        {
            if (bands < 0)
            {
                ranges = new double[0];
                return new int[0];
            }

            int[] res = new int[bands];

            ranges = new double[bands + 1];

            for (int i = 0; i < bands + 1; i++)
                ranges[i] = from + (to - from) * ((double)i / (double)bands);

            foreach (Coords crd in validCoords /*c.AllCoords*/)
            {
                if (!crd.IsOk)
                    continue;

                for (int l = 0; l < c.Layers; l++)
                {

                    double val = c[crd,l];
                    for (int i = 0; i < bands; i++)
                    {
                        if ((ranges[i] >= val) && (ranges[i + 1] < val))
                        {
                            res[i]++;
                            totalFound++;
                        }
                        if ((i + 1 == bands) && includeLowerBound && (ranges[i + 1] == val))
                        {
                            res[i]++;
                            totalFound++;
                        }
                    }
                }

            }
            return res;
        }
    }


    public class Hystogram
    {
        public int Update(int[] var)
        {
            _maxInBands = 0;

            if (var != null)
            {
                foreach (int i in var)
                    if (i > _maxInBands)
                        _maxInBands = i;

                _magicZone = new Rectangle[var.Length];
                _ranges = var;
            }
            else
            {
                _magicZone = null;
                _ranges = null;
            }
            _rangemax = _maxInBands;
            return _maxInBands;
        }

        public int MaxInBands
        {
            get { return _maxInBands; }
        }

        public void SetRange(int newRange)
        {
            if (newRange > _maxInBands)
                _rangemax = newRange;
        }

        public void Draw(Graphics g, Rectangle r)
        {
            if ((_ranges == null))
                return;

            double multiplier = (double)(r.Width - 2 * _margin) / _rangemax;
            if (Double.IsInfinity(multiplier) || Double.IsNaN(multiplier))
                return;

            for (int i = 0; i < _ranges.Length; i++)
            {
                int cx = r.Height * i / _ranges.Length;
                int nx = r.Height * (i + 1) / _ranges.Length;
                int height = nx - cx - 2 * _margin + 1;

                Rectangle rec = new Rectangle(r.X + _margin, r.Y + _margin + cx,
                    (int)(multiplier * _ranges[i]), height);

                g.FillRectangle(_brush, rec);
                g.DrawRectangle(_pen, rec);

                _magicZone[i] = new Rectangle(r.X + _margin, r.Y + _margin + cx, r.Width, height);
            }
        }

        public void DrawNumbers(Graphics g, Rectangle r, int skipX)
        {
            if ((_ranges == null) || (_ranges.Length == 0) || (r.Height == 0))
                return;

            Font f = new Font(FontFamily.GenericSansSerif, Math.Max(4, (float)r.Height * 0.6f / _ranges.Length));
            double multiplier = (double)(r.Width - 2 * _margin) / _rangemax;

            for (int i = 0; i < _ranges.Length; i++)
            {
                int cx = r.Height * i / _ranges.Length;
                int nx = r.Height * (i + 1) / _ranges.Length;
                int height = nx - cx - 2 * _margin + 1;
                int barWidth = (int)(multiplier * _ranges[i]);
                string s = _ranges[i].ToString();

                Size sz = Size.Ceiling(g.MeasureString(s, f));

                PointF pnt = new PointF((float)(r.X + skipX + _margin), (float)(r.Y + _margin + cx));

                if (barWidth + sz.Width > r.Width)
                {
                    if (skipX + sz.Width < r.Width)
                    {
                        pnt.X = r.X + r.Width - 2 * _margin - sz.Width;
                        g.DrawString(s, f, Brushes.Black, pnt);
                    }
                }
                else
                {
                    if (barWidth > skipX)
                    {
                        pnt.X += barWidth - skipX;
                        g.DrawString(s, f, Brushes.Black, pnt);
                    }
                    else if (skipX + sz.Width < r.Width)
                    {
                        g.DrawString(s, f, Brushes.Black, pnt);
                    }
                }

            }
        }


        public int InRange(Point p)
        {
            if (_ranges == null)
                return -1;

            for (int i = 0; i < _magicZone.Length; i++)
            {
                Rectangle r = _magicZone[i];
                if ((r.X < p.X) && (r.Y < p.Y) && (r.Right > p.X) && (r.Bottom > p.Y))
                    return i;
            }
            return -1;
        }
        Brush _brush = Brushes.Bisque;
        Pen _pen = Pens.Red;

        int _margin = 1;
        int[] _ranges;
        int _maxInBands;
        int _rangemax;

        Rectangle[] _magicZone;


    }


    public class DrawCartZagr : DrawCartCell
    {
        IDataCartogram _cart;

        const int DKF_NOTHIG = 0x0000;
        const int DKF_DOT = 0x0010;
        const int DKF_STRIANGLE = 0x0020;
        const int DKF_SDIAMOND = 0x0030;
        const int DKF_DTRIANGLE = 0x0040;
        const int DKF_CIRCLE = 0x0050;
        const int DKF_CENT_TRI = 0x0060;
        const int DKF_CIRCLE_HAT = 0x0070;
        const int DKF_FULL = 0x0080;
        const int DKF_QUESTION = 0x00f0;

        const int DKF_FILLED = 0x0001;
        const int DKF_CROSSED = 0x0002;
        const int DKF_HFILLED = 0x0003;

        const int DKF_FILL_MASK = 0x0003;

        const int DKF_ROT90 = 0x0004;
        const int DKF_ROT180 = 0x0008;
        const int DKF_ROT270 = 0x000C;

        public struct NodeColorInfo
        {
            public int _drawCode;
            public Brush _brush1;
            public Pen _pen1;
            public string _name;
            public string _descr;

            public void SetDefault()
            {
                _drawCode = DKF_NOTHIG;
                _brush1 = Brushes.Black;
                _pen1 = Pens.Black;

                _name = null;
                _descr = null;
            }

            public void reset(int drawCode, Color color, string name, string descr)
            {
                _drawCode = drawCode;
                _brush1 = new SolidBrush(color);
                _pen1 = new Pen(color);

                _name = name;
                _descr = descr;
            }
        }

        public DrawCartZagr(DataParamTable tup)
            : this((System.Xml.XmlElement)tup["DrawZagrCart"].Value)
        {
        }

        public DrawCartZagr(XmlNode root)
        {
            _ni = new NodeColorInfo[256];
            for (int i = 0; i < _ni.Length; i++)
            {
                _ni[i].SetDefault();
            }

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.Name == "#comment")
                    continue;

                if (n.Name == "item")
                {
                    string id = n.Attributes["id"].Value;
                    string name = n.Attributes["name"].Value;
                    string descr = n.Attributes["descr"].Value;
                    string draw = n.Attributes["draw"].Value;
                    string color = n.Attributes["color"].Value;

                    int nId = Convert.ToInt32(id, 16);
                    int nDraw = Convert.ToInt32(draw, 16);
                    int nColor = Convert.ToInt32(color, 16);

                    if ((nId >= 0) && (nId < 256))
                    {
                        _ni[nId].reset(nDraw, Color.FromArgb(255, Color.FromArgb(nColor)),
                            name, descr);
                    }
                }
            }

        }

        NodeColorInfo[] _ni;

        public override void UpdateCart(IDataCartogram c, CoordsConverter validCoords)
        {
            _cart = c;
        }

        public override Size GetLegendMinSize(Size canvasSize)
        {
            return new Size();
        }

        protected void DrawObject(Graphics g, Point p, Size s, NodeColorInfo _ni)
        {
            Size s1 = new Size(s.Width / 3, s.Height / 3);
            Point p1 = new Point(p.X + s1.Width, p.Y + s1.Height);

            switch (_ni._drawCode & 0x0f0)
            {
                case DKF_NOTHIG:
                    g.FillRectangle(_ni._brush1, p.X + 1, p.Y + 1, s.Width - 1, s.Height - 1);
                    break;
                case DKF_DOT:
                    if ((_ni._drawCode & DKF_FILLED) == DKF_FILLED)
                        g.FillEllipse(_ni._brush1, new Rectangle(p1, s1));
                    else
                        g.DrawEllipse(_ni._pen1, new Rectangle(p1, s1));
                    break;
                case DKF_STRIANGLE:
                    Point[] triangle = {
                            new Point(p1.X + s1.Width/2, p1.Y),
                            new Point(p1.X, p1.Y + s1.Height),
                            new Point(p1.X + s1.Width, p1.Y + s1.Height)
                        };
                    if ((_ni._drawCode & DKF_FILLED) == DKF_FILLED)
                        g.DrawLines(_ni._pen1, triangle);
                    else
                        g.FillPolygon(_ni._brush1, triangle);
                    break;
                case DKF_SDIAMOND:
                    Point[] diamond = {
                            new Point(p1.X + s1.Width/2, p1.Y),
                            new Point(p1.X, p1.Y + s1.Height/2),
                            new Point(p1.X + s1.Width/2, p1.Y + s1.Height),
                            new Point(p1.X + s1.Width, p1.Y + s1.Height/2)
                        };
                    if ((_ni._drawCode & DKF_FILLED) == DKF_FILLED)
                        g.DrawLines(_ni._pen1, diamond);
                    else
                        g.FillPolygon(_ni._brush1, diamond);
                    break;
                case DKF_CIRCLE:
                    {
                        Size sc = new Size(2 * s.Width / 3, 2 * s.Height / 3);
                        Point pc = new Point(p.X + sc.Width / 4, p.Y + sc.Height / 4);
                        Point pec = new Point(p.X + s.Width - sc.Width / 4, p.Y + s.Height - sc.Height / 4);
                        Size perfSize = new Size(pec.X - pc.X, pec.Y - pc.Y);

                        if ((_ni._drawCode & DKF_FILL_MASK) == DKF_FILLED)
                            g.FillEllipse(_ni._brush1, new Rectangle(pc, perfSize));
                        else
                        {
                            g.DrawEllipse(_ni._pen1, new Rectangle(pc, perfSize));

                            if ((_ni._drawCode & DKF_FILL_MASK) == DKF_CROSSED)
                            {
                                int r1 = (int)(Math.Sqrt(2) * (double)sc.Width / 2);
                                g.DrawLine(_ni._pen1,
                                    new Point(p.X + (s.Width - r1) / 2, p.Y + (s.Height - r1) / 2),
                                    new Point(p.X + (s.Width + r1) / 2, p.Y + (s.Height + r1) / 2));
                                g.DrawLine(_ni._pen1,
                                    new Point(p.X + (s.Width + r1) / 2, p.Y + (s.Height - r1) / 2),
                                    new Point(p.X + (s.Width - r1) / 2, p.Y + (s.Height + r1) / 2));
                            }
                            else if ((_ni._drawCode & DKF_FILL_MASK) == DKF_HFILLED)
                            {
                                switch (_ni._drawCode & 0xc)
                                {
                                    case 0:
                                        g.FillPie(_ni._brush1, new Rectangle(pc, perfSize), 270, 180);
                                        break;
                                    case DKF_ROT90:
                                        g.FillPie(_ni._brush1, new Rectangle(pc, perfSize), 0, 180);
                                        break;
                                    case DKF_ROT180:
                                        g.FillPie(_ni._brush1, new Rectangle(pc, perfSize), 90, 180);
                                        break;
                                    case DKF_ROT270:
                                        g.FillPie(_ni._brush1, new Rectangle(pc, perfSize), 180, 180);
                                        break;
                                }
                            }
                        }
                        break;
                    }
                case DKF_CIRCLE_HAT:
                    {
                        Size sc = new Size(2 * s.Width / 3, 2 * s.Height / 3);
                        Point pc = new Point(p.X + sc.Width / 4, p.Y + sc.Height / 4);
                        Point pec = new Point(p.X + s.Width - sc.Width / 4, p.Y + s.Height - sc.Height / 4);
                        Size perfSize = new Size(pec.X - pc.X, pec.Y - pc.Y);

                        g.DrawEllipse(_ni._pen1, new Rectangle(pc, perfSize));

                        int r1 = (int)(Math.Sqrt(2) * (double)sc.Width / 2);
                        g.DrawLine(_ni._pen1,
                            new Point(p.X + (s.Width - r1) / 2, p.Y + (s.Height + r1) / 2),
                            new Point(p.X + (s.Width) / 2, p.Y + (s.Height - r1) / 2));
                        g.DrawLine(_ni._pen1,
                            new Point(p.X + (s.Width + r1) / 2, p.Y + (s.Height + r1) / 2),
                            new Point(p.X + (s.Width) / 2, p.Y + (s.Height - r1) / 2));

                        break;
                    }
                case DKF_DTRIANGLE:
                    {
                        Point[] fp = {
                            new Point(p.X + 1 + s.Width - 1, p.Y + 1),
                            new Point(p.X + 1 + s.Width - 1, p.Y + 1 + s.Height - 1),
                            new Point(p.X + 1, p.Y + 1 + s.Height - 1),
                            new Point(p.X + 1, p.Y + 1),
                            new Point(p.X + 1 + s.Width - 1, p.Y + 1),
                            new Point(p.X + 1 + s.Width - 1, p.Y + 1 + s.Height - 1)
                            };

                        switch (_ni._drawCode & 0xc)
                        {
                            case 0:
                                {
                                    Point[] rpoints = { fp[0], fp[1], fp[2] };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT90:
                                {
                                    Point[] rpoints = { fp[1], fp[2], fp[3] };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT180:
                                {
                                    Point[] rpoints = { fp[2], fp[3], fp[4] };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT270:
                                {
                                    Point[] rpoints = { fp[3], fp[4], fp[5] };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                        }
                    }
                    break;
                case DKF_CENT_TRI:
                    {
                        Size sc = new Size(2 * s.Width / 3, 2 * s.Height / 3);
                        Point pc = new Point(p.X + sc.Width / 4, p.Y + sc.Height / 4);
                        Point pec = new Point(p.X + s.Width - sc.Width / 4, p.Y + s.Height - sc.Height / 4);

                        Point[] fp = {
                                new Point(p.X + 1 + s.Width - 1, p.Y + 1),
                                new Point(p.X + 1 + s.Width - 1, p.Y + 1 + s.Height - 1),
                                new Point(p.X + 1, p.Y + 1 + s.Height - 1),
                                new Point(p.X + 1, p.Y + 1)
                            };
                        Point center = new Point(p.X + 1 + (s.Width - 1) / 2, p.Y + 1 + (s.Height - 1) / 2);

                        g.DrawEllipse(_ni._pen1, new Rectangle(pc,
                            new Size(pec.X - pc.X, pec.Y - pc.Y)));

                        switch (_ni._drawCode & 0xc)
                        {
                            case 0:
                                {
                                    Point[] rpoints = { fp[0], fp[1], center };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT90:
                                {
                                    Point[] rpoints = { fp[1], fp[2], center };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT180:
                                {
                                    Point[] rpoints = { fp[2], fp[3], center };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                            case DKF_ROT270:
                                {
                                    Point[] rpoints = { fp[3], fp[0], center };
                                    g.FillPolygon(_ni._brush1, rpoints);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public override void DrawCell(Graphics g, Point p, Size s, Coords o)
        {
            //int i = (int)_cart.ScaleToDouble(o);
            int i = (int)_cart[o, 0];
            Debug.Assert((i < 256) && (i >= 0));

            if (i < _ni.Length)
            {
                DrawObject(g, p, s, _ni[i]);
            }
        }

        public override void DrawLegend(Graphics g, Point p, Size s)
        {

        }
    }


    public class DrawCartLevel : DrawCartCell
    {
        public override void UpdateCart(IDataCartogram c, CoordsConverter validCoords)
        {
            if (c != null)
                c.GetScaledMaxMin(out _min, out _max);

            _cart = c;
            _validCoords = validCoords;
        }

        public override void DrawCell(Graphics g, Point p, Size s, Coords c)
        {
            if (_cart == null)
                return;
            if (!_cart.IsValidCoord(c))
                return;

            Brush b;

            double delta = (s.Height) / _cart.Layers;

            for (int i = 0; i < _cart.Layers; i++)
            {
                double v = _cart[c, i];

                int level = (int)Math.Round(255 * (v - _min) / (_max - _min));

                if ((level >= 0) && (level < 256))
                    b = _levelBrush[level];
                else
                    b = _incorect;

                int height = (int)((i + 1) * delta) - (int)(i * delta);
                g.FillRectangle(b, p.X + 1, p.Y + 1 + (int)(i * delta), s.Width - 1, height - 1);
            }


            if (border)
                g.DrawRectangle(_border, p.X, p.Y, s.Width, s.Height);
        }


        public DrawCartLevel()
        {
            _border = Pens.Black;

            _levelBrush = new Brush[256];
            for (int i = 0; i < 256; i++)
                _levelBrush[i] = new SolidBrush(Color.FromArgb(0, i, 0));

            _incorect = Brushes.LightGray;
        }


        bool border = false;

        IDataCartogram _cart;
        CoordsConverter _validCoords;

        double _min;
        double _max;

        Pen _border;
        Brush _incorect;
        Brush[] _levelBrush;

        bool drawHystrogramm = true;
        Hystogram _hist = new Hystogram();
        double[] _magicInts;

        public override Size GetLegendMinSize(Size canvasSize)
        {
            int y = canvasSize.Height;
            if (y > canvasSize.Width)
                y = canvasSize.Width;

            int width = 30 + y / 9;
            return new Size(width, 100);
        }

        public override void DrawLegend(Graphics g, Point p, Size s)
        {
            int minTextWidth = 20;
            int height = s.Height;
            int width = Math.Min(height / 32, s.Width - minTextWidth);
            int ticklen = width / 5;

            Point pr = p;
            pr.Y += height / 32;
            pr.X += 10;
            Size sr = s;
            sr.Height -= 2 * height / 32;
            sr.Width = width;

            int textWidth = s.Width - sr.Width - 10 - ticklen;

            Font fnt = new Font(FontFamily.GenericSansSerif, Math.Max(height / 64, 8));
            int ticks = Math.Max(height / 32, 20);
            float textHeight = sr.Height / (ticks + 5);


            if ((drawHystrogramm) && (_cart.Layers == 1))
            {
                int maxWidth = textWidth;

                int totalFound = 0;

                int[] ints = CalcHystogram(_cart, _validCoords, _max, _min, true, 2*(ticks - 1), ref totalFound, out _magicInts);

                _hist.SetRange(_hist.Update(ints));

                _hist.Draw(g, new Rectangle(pr.X + sr.Width, pr.Y, maxWidth, sr.Height));

                _hist.DrawNumbers(g, new Rectangle(pr.X + sr.Width, pr.Y, maxWidth, sr.Height), (int)textHeight * 5);
            }

            DrawBarWithNotes(g, new Rectangle(pr, sr), new SizeF(textWidth, textHeight), fnt, ticks, _min, _max, Color.FromArgb(0, 255, 0), Color.FromArgb(0, 0, 0));
        }

        public override void OnMouseUp(CartView obj, MouseEventArgs e)
        {
            int v;

            if ((v = _hist.InRange(new Point(e.X, e.Y))) != -1 && drawHystrogramm)
            {
                double end = _magicInts[v + 1];
                if (_magicInts.Length == v + 2)
                    end -= 0.01;

                if (e.Button == MouseButtons.Left)
                    obj.SelectInRange(_magicInts[v], end, true);
                else if (e.Button == MouseButtons.Right)
                    obj.SelectInRange(_magicInts[v], end, false);
            }            
        }
    }


    public class DrawCartDiffLevel : DrawCartCell
    {
        public override void UpdateCart(IDataCartogram c, CoordsConverter validCoords)
        {
            if (c != null)
                _amax = c.GetScaledMaxMin(out _min, out _max);
            _cart = c;
            _validCoords = validCoords;
        }

        public override void DrawCell(Graphics g, Point p, Size s, Coords c)
        {
            if (_cart == null)
                return;
            if (!_cart.IsValidCoord(c))
                return;

            Brush b;

            double delta = (s.Height) / _cart.Layers;
            for (int i = 0; i < _cart.Layers; i++)
            {
                double v = _cart[c, i];

                int level = (int)Math.Round(127 * v / (_amax));

                if ((level >= 0) && (level < 128))
                    b = _levelBrushMax[level];
                else if ((level < 0) && (level > -128))
                    b = _levelBrushMin[-level];
                else
                    b = _incorect;

                int height = (int)((i + 1) * delta) - (int)(i * delta);
                g.FillRectangle(b, p.X + 1, p.Y + 1 + (int)(i * delta), s.Width - 1, height - 1);
            }

            if (border)
                g.DrawRectangle(_border, p.X, p.Y, s.Width, s.Height);
        }


        public DrawCartDiffLevel()
        {
            _border = Pens.Black;

            _levelBrushMin = new Brush[128];
            for (int i = 0; i < 128; i++)
                _levelBrushMin[i] = new SolidBrush(Color.FromArgb(255 - 2 * i, 255 - 2 * i, 255));

            _levelBrushMax = new Brush[128];
            for (int i = 0; i < 128; i++)
                _levelBrushMax[i] = new SolidBrush(Color.FromArgb(255, 255 - 2 * i, 255 - 2 * i));

            _incorect = Brushes.LightGray;
        }

        bool border = false;

        IDataCartogram _cart;
        CoordsConverter _validCoords;

        double _amax;
        double _min;
        double _max;

        Pen _border;
        Brush _incorect;
        Brush[] _levelBrushMin;
        Brush[] _levelBrushMax;

        bool drawHystrogramm = true;
        bool doubleBar = true;

        double[] _magicMin;
        double[] _magicMax;

        Hystogram _hmin = new Hystogram();
        Hystogram _hmax = new Hystogram();

        public override Size GetLegendMinSize(Size canvasSize)
        {
            int width = 30 + canvasSize.Height / 9;
            return new Size(width, 100);
        }

        public override void DrawLegend(Graphics g, Point p, Size s)
        {
            int minTextWidth = 20;
            int height = s.Height;
            int width = Math.Min(height / 32, s.Width - minTextWidth);
            int ticklen = width / 5;

            Point pr = p;
            pr.Y += height / 32;
            pr.X += 10;
            Size sr = s;
            sr.Height -= 2 * height / 32;
            sr.Width = width;

            int textWidth = s.Width - sr.Width - 10 - ticklen;

            Font fnt = new Font(FontFamily.GenericSansSerif, Math.Max(height / 64, 8));
            int total_ticks = Math.Max(height / 32, 20);
            float textHeight = sr.Height / (total_ticks + 5);


            double alpha = (Math.Abs(_max) / Math.Abs(_max - _min));

            if (alpha > 1.0)
                alpha = 1.0;

            int up = (int)Math.Round(alpha * total_ticks);
            int down = total_ticks - up;
            int upx = 0;
            if (_max > 0)
            {
                if (up < down)
                    up++;
                if (up < 2)
                    up = 2;

                upx = (int)(sr.Height * alpha);
            }
            if (_min < 0)
            {
                if (down < up)
                    down++;
                if (down < 2)
                    down = 2;
            }
            if ((drawHystrogramm) && (!Double.IsNaN(alpha)))
            {
                int maxWidth = textWidth;

                int totalFound = 0;
                int maxes = ((doubleBar) ? 2 : 1) * (up - 1);
                int minimums = ((doubleBar) ? 2 : 1) * (down - 1);
                int[] max = CalcHystogram(_cart, _validCoords, _max, 0, (_min == 0.0), maxes, ref totalFound, out _magicMax);
                int[] min = CalcHystogram(_cart, _validCoords, 0, _min, true, minimums, ref totalFound, out _magicMin);

                int rangemax = Math.Max(_hmax.Update(max), _hmin.Update(min));
                _hmax.SetRange(rangemax);
                _hmin.SetRange(rangemax);

                _hmax.Draw(g, new Rectangle(pr.X + sr.Width, pr.Y, maxWidth, upx));
                _hmin.Draw(g, new Rectangle(pr.X + sr.Width, pr.Y + upx, maxWidth, sr.Height - upx));

                _hmax.DrawNumbers(g, new Rectangle(pr.X + sr.Width, pr.Y, maxWidth, upx), (int)textHeight * 5);
                _hmin.DrawNumbers(g, new Rectangle(pr.X + sr.Width, pr.Y + upx, maxWidth, sr.Height - upx), (int)textHeight * 5);

            }
            if (_max > 0)
            {
                int q = (int)Math.Abs(255 * _max / _amax);
                DrawBarWithNotes(g, new Rectangle(pr, new Size(sr.Width, upx)), new SizeF(textWidth, textHeight), fnt, up, 0, _max,
                    Color.FromArgb(255, 255 - q, 255 - q), Color.FromArgb(255, 255, 255));
            }
            if (_min < 0)
            {
                int q = (int)Math.Abs(255 * _min / _amax);
                DrawBarWithNotes(g, new Rectangle(new Point(pr.X, pr.Y + upx), new Size(sr.Width, sr.Height - upx)),
                    new SizeF(textWidth, textHeight), fnt, down, _min, 0,
                    Color.FromArgb(255, 255, 255), Color.FromArgb(255 - q, 255 - q, 255));
            }
        }

        public override void OnMouseUp(CartView obj, MouseEventArgs e)
        {
            int v;

            if ((v = _hmax.InRange(new Point(e.X, e.Y))) != -1)
            {
                double end = _magicMax[v + 1];
                if ((_magicMax.Length == v + 2) && (_magicMin.Length == 0))
                    end -= 1.1;

                if (e.Button == MouseButtons.Left)
                    obj.SelectInRange(_magicMax[v], end, true);
                else if (e.Button == MouseButtons.Right)
                    obj.SelectInRange(_magicMax[v], end, false);
            }
            else if ((v = _hmin.InRange(new Point(e.X, e.Y))) != -1)
            {
                double end = _magicMin[v + 1];
                if (_magicMin.Length == v + 2)
                    end *= 1.1;

                if (e.Button == MouseButtons.Left)
                    obj.SelectInRange(_magicMin[v], end, true);
                else if (e.Button == MouseButtons.Right)
                    obj.SelectInRange(_magicMin[v], end, false);

            }
        }
    }



}
