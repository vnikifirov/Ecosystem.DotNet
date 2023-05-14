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

using System.IO;

using corelib;


namespace corelib
{

    public interface ICoordNavigation
    {
        void FlushSelection();
        Coords[] GetSelected();
        void SelectCoords(Coords[] c);
        bool SelectCoord(Coords c, bool set);
        bool SelectCoord(Coords c, bool set, bool update);
        
        bool EnableSelection
        {
            get;
            set;
        }

        bool ShowLegend
        {
            get;
            set;
        }

        event CoordsEventDelegate MovedToCoord;
        event CoordsEventDelegate LeftClickCoord;
        event CoordsEventDelegate RightClickCoord;
    }

    /// <summary>
    /// Summary for Graph
    /// </summary>
    public class CartView : UserControl
    {
        public CartView()
        {
            InitInternals();
        }

        class ExPanel : Panel
        {
            public ExPanel()
            {
#if !DOTNET_V11
                DoubleBuffered = true;
#endif
            }
        }

        #region Mouse Wheel with Ctrl button present sender
        bool _keyControl = false;

        protected override void OnKeyUp(KeyEventArgs e)
        {
            _keyControl = e.Control;
            base.OnKeyUp(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            _keyControl = e.Control;
            base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_keyControl)
            {
                _nav.MouseWheel(e);
            }
        }
        #endregion
        ExPanel p = new ExPanel();

        void InitInternals()
        {
            this.SuspendLayout();
            this.Resize += new System.EventHandler(this.CartView_Resize);

            _nav = new CoordNavigation(this);
            //SetLevelCart();

            AutoScroll = true;

            p.BackColor = Color.Empty;
            p.Location = new Point(borders / 2, borders / 2);
            p.Paint += new PaintEventHandler(CartView_Paint);
            p.MouseUp += new MouseEventHandler(p_MouseUp);
            p.MouseMove += new MouseEventHandler(p_MouseMove);
            this.Controls.Add(p);

            SetAutoScrollMargin(borders / 2, borders / 2);

            this.ResumeLayout(false);

            _plugins = new Plugins(typeof(DrawCartCell), "DrawCart");
        }

        void p_MouseMove(object sender, MouseEventArgs e)
        {
            _nav.MouseMove(e);
        }

        void p_MouseUp(object sender, MouseEventArgs e)
        {
            this.Focus();
            _nav.MouseUp(e);
        }

        void CartView_Paint(Object sender, PaintEventArgs e)
        {
            _nav.CoodsPainter(e.Graphics, _painter, _canvasSize);
        }

        public void Redraw()
        {
            CartView_Resize(null, null);
            p.Invalidate();
        }

        new public void Update()
        {
            p.Invalidate();
        }


        void CartView_Resize(Object sender, EventArgs e)
        {
            if (_autoSize && _painter != null)
            {
                Size canvasSize = _nav.CalculateAutoSize(Size, _painter);
                if (canvasSize != _canvasSize)
                {
                    _canvasSize = canvasSize;
                    p.Size = _canvasSize;

                    p.Invalidate();
                }
            }
            
        }


        public IDataCartogram GetCart()
        {
            return _cart;
        }



        public class CoordNavigation : ICoordNavigation
        {
            Coords GetCoordFormPoint(Point p)
            {
                //int x = p.X / _currentMultiplyer;
                //int y = 47 - p.Y / _currentMultiplyer;
                //return new Coords(y,x);

                int logicaly, logicalx;
                if (_presentation.SwitchXY)
                {
                    logicaly = _presentation.RevertY ? 47 - p.X / _currentMultiplyer : p.X / _currentMultiplyer;
                    logicalx = _presentation.RevertX ? 47 - p.Y / _currentMultiplyer : p.Y / _currentMultiplyer;
                }
                else
                {
                    logicalx = _presentation.RevertY ? 47 - p.X / _currentMultiplyer : p.X / _currentMultiplyer;
                    logicaly = _presentation.RevertX ? 47 - p.Y / _currentMultiplyer : p.Y / _currentMultiplyer;
                }
                return new Coords(logicalx, logicaly);
            }

            Point GetPointFromCoord(Coords crd)
            {
                //int x = crd.X * _currentMultiplyer;
                //int y = (47 - crd.Y) * _currentMultiplyer;
                // return new Point(x, y);

                int x, y;
                if (_presentation.SwitchXY)
                {
                    x = (_presentation.RevertY ? (47 - crd.X) : crd.X) * _currentMultiplyer;
                    y = (_presentation.RevertX ? (47 - crd.Y) : crd.Y) * _currentMultiplyer;
                }
                else
                {
                    y = (_presentation.RevertY ? (47 - crd.X) : crd.X) * _currentMultiplyer;
                    x = (_presentation.RevertX ? (47 - crd.Y) : crd.Y) * _currentMultiplyer;
                }

                return new Point(x, y);
            }

            void DrawSelection(Graphics g, Coords crd)
            {
                Point p = GetPointFromCoord(crd);
                if (_currentMultiplyer > 8)
                    g.DrawRectangle(Pens.Yellow, p.X + 3, p.Y + 3, _currentMultiplyer - 6, _currentMultiplyer - 6);
                g.DrawRectangle(Pens.Yellow, p.X + 2, p.Y + 2, _currentMultiplyer - 4, _currentMultiplyer - 4);
            }

            void DrawSingleCell(Graphics g, Coords crd)
            {
                Size s = new Size(_currentMultiplyer, _currentMultiplyer);
                Point p = GetPointFromCoord(crd);
                
                _cartDrawer.DrawCell(g, p, s, crd);
            }

            internal void CoodsPainter(Graphics g, DrawCartCell c, Size viewPortSize)
            {
                if (_parent._cart == null)
                    return;

                if (_validCoords != null)
                {
                    foreach (Coords crd in _validCoords)
                    {
                        if (crd.IsOk)
                            DrawSingleCell(g, crd);
                    }
                }
                else
                {
                    foreach (Coords crd in _parent._cart.AllCoords)
                    {
                        if (crd.IsOk)
                            DrawSingleCell(g, crd);
                    }
                }
                if (_selectMode)
                {
                    foreach (Coords crd in selectedCoords)
                        if (crd.IsOk)
                            DrawSelection(g, crd);
                }


                if (_drawLegend)
                {
                    int legendHeight = _currentMultiplyer * 48;

                    int legendX = _currentMultiplyer * 48;
                    int viewPortWidth = viewPortSize.Width - legendX;
                    int minWidth = c.GetLegendMinSize(viewPortSize).Width;

                    if ((viewPortWidth - minWidth) > 0)
                    {
                        int delta = viewPortWidth - minWidth;
                        if (delta > _currentMultiplyer)
                            delta = _currentMultiplyer;

                        legendX += delta;
                        viewPortWidth -= delta;
                    }

                    c.DrawLegend(g, new Point(legendX, 0), new Size(viewPortWidth, legendHeight));
                }
            }

            internal Size CalculateAutoSize(Size widget, DrawCartCell c)
            {
                Size legend = c.GetLegendMinSize(widget);
                int padd = 1;

                int widthAddon = 0;

                int edge;
                if (_drawLegend)
                {
                    widthAddon = legend.Width;
                    edge = Math.Min(widget.Width - borders - legend.Width, widget.Height - borders);
                }
                else
                    edge = Math.Min(widget.Width - borders, widget.Height - borders);

                int dedge = (edge / (step)) * step;
                if (dedge < step * minMultiplier)
                    dedge = step * minMultiplier;

                _origCurrentMultiplyer = dedge / step;
                if ((!_zoom) || (_origCurrentMultiplyer > _currentMultiplyer))
                {
                    _zoom = false;

                    _currentMultiplyer = _origCurrentMultiplyer;

                    if ((widget.Width - borders - 1 > dedge + padd + widthAddon) && (_drawLegend))
                        return new Size(widget.Width - borders-1, dedge + padd);
                    else
                        return new Size(dedge + padd + widthAddon, dedge + padd);
                }
                else
                {
                    dedge = _currentMultiplyer * step;

                    if (_drawLegend)
                    {
                        legend = c.GetLegendMinSize(new Size(dedge, dedge));
                        widthAddon = legend.Width;
                    }
                    return new Size(dedge + padd + widthAddon, dedge + padd);
                }
            }


            internal CoordNavigation(CartView c)
            {
                _parent = c;
            }

            bool IsCoordInCart(Coords c)
            {
                /////////////////////////////////////////////////////////////////////////////////////////
                // IMPROVE ME
                if (_parent._cart != null)
                    /* foreach (Coords crd in _parent._cart.AllCoords)
                         if (crd == c)
                             return true;*/
                    if (_validCoords != null)
                        return _validCoords[c] != CoordsConverter.InvalidIndex;
                    else
                        return _parent._cart.IsValidCoord(c);

                return false;
            }

            internal void MouseUp(MouseEventArgs e)
            {
                Coords n = GetCoordFormPoint(new Point(e.X, e.Y));

                if (IsCoordInCart(n))
                {
                    // throw event
                    if (e.Button == MouseButtons.Left)
                    {
                        if (LeftClickCoord != null)
                            LeftClickCoord(_parent, new CoodrsEventArgs(n, e.Button, e.Clicks, e.X, e.Y, e.Delta));

                        SelectClick(n);
                    }

                    if (e.Button == MouseButtons.Right)
                        if (RightClickCoord != null)
                            RightClickCoord(_parent, new CoodrsEventArgs(n, e.Button, e.Clicks, e.X, e.Y, e.Delta));
                }
                else
                {
                    _parent._painter.OnMouseUp(_parent, e);
                }
            }

            internal void MouseMove(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.None)
                {
                    Coords n = GetCoordFormPoint(new Point(e.X, e.Y));
                    if (n != _moving)
                    {
                        if (IsCoordInCart(n))
                        {
                            _moving = n;
                            // throw event
                            if (MovedToCoord != null)
                                MovedToCoord(_parent, new CoodrsEventArgs(n, e.Button, e.Clicks, e.X, e.Y, e.Delta));
                        }
                        else
                        {
                            if (_moving.IsOk)
                            {
                                _moving = Coords.incorrect;
                                if (MovedToCoord != null)
                                    MovedToCoord(_parent, new CoodrsEventArgs(_moving, e.Button, e.Clicks, e.X, e.Y, e.Delta));
                            }
                        }
                    }
                }
            }

            internal void MouseWheel(MouseEventArgs e)
            {
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

                int dx = (_currentMultiplyer * numberOfTextLinesToMove) / 25;
                if (dx == 0)
                    dx = (numberOfTextLinesToMove > 0) ? 1 : -1;

                _currentMultiplyer += dx;
                SetScaleIndex(_currentMultiplyer);
            }

            public void SetScaleIndex(int idx)
            {
                if (idx <= _origCurrentMultiplyer)
                {
                    _currentMultiplyer = _origCurrentMultiplyer;
                    _zoom = false;
                }
                else
                {
                    _currentMultiplyer = idx;
                    _zoom = true;
                }
                _parent.CartView_Resize(null, null);
                _parent.Redraw();
            }

            public event CoordsEventDelegate MovedToCoord;
            public event CoordsEventDelegate LeftClickCoord;
            public event CoordsEventDelegate RightClickCoord;



            void UnSelect(Coords c)
            {
                if (_selectMode)
                {
                    if (selectedCoords.Contains(c))
                    {
                        selectedCoords.Remove(c);
                    }
                }
            }

            void Select(Coords c)
            {
                if (_selectMode)
                {
                    if (!selectedCoords.Contains(c))
                    {
                        selectedCoords.Add(c);
                    }
                }
            }

            void SelectClick(Coords c)
            {
                if (_selectMode)
                {
                    using (Graphics g = Graphics.FromHwnd(_parent.p.Handle))
                    {
                        if (selectedCoords.Contains(c))
                        {
                            selectedCoords.Remove(c);
                            DrawSingleCell(g, c);
                        }
                        else
                        {
                            selectedCoords.Add(c);
                            DrawSelection(g, c);
                        }
                    }
                }
            }

            public void FlushSelection()
            {
                selectedCoords.Clear();
                _parent.Update();
            }
            public Coords[] GetSelected()
            {
                Coords[] c = new Coords[selectedCoords.Count];
                selectedCoords.CopyTo(c);
                return c;
            }
            public void SelectCoords(Coords[] c)
            {
                foreach (Coords s in c)
                {
                    selectedCoords.Add(s);
                }
                _parent.Update();
            }

            public bool SelectCoord(Coords c, bool set)
            {
                return SelectCoord(c, set, true);
            }

            public bool SelectCoord(Coords c, bool set, bool update)
            {
                if (_validCoords[c] >= 0)
                {
                    if (set)
                        selectedCoords.Add(c);
                    else
                        selectedCoords.Remove(c);
                    if (update)
                        _parent.Update();

                    return true;
                }
                return false;
            }

            public bool EnableSelection
            {
                get { return _selectMode; }
                set { _selectMode = value; FlushSelection(); }
            }
 
            public bool ShowLegend
            {
                get { return _drawLegend; }
                set { _drawLegend = value;  _parent.Redraw(); }
            }


            internal void SetDrawCart(DrawCartCell c, CartogramPresentationConfig presentation, CoordsConverter validCoords)
            {
                _cartDrawer = c;
                _presentation = presentation;
                _validCoords = validCoords;
            }

            CartogramPresentationConfig _presentation;
            DrawCartCell _cartDrawer;
            CoordsConverter _validCoords;
            
            Coords _moving = Coords.incorrect;
            CartView _parent;

            bool _zoom = false;
            int _origCurrentMultiplyer;
            int _currentMultiplyer;
            const int step = 48;
            const int minMultiplier = 8;

            bool _drawLegend = true;
            bool _selectMode = false;

#if !DOTNET_V11
            List<Coords> selectedCoords = new List<Coords>();
#else
            ArrayList selectedCoords = new ArrayList();
#endif
        }


        public ICoordNavigation Navigation
        {
            get { return _nav; }
        }


        public void SelectInRange(double start, double end, bool select)
        {
            foreach (Coords c in _cart.AllCoords)
            {
                if (c.IsOk)
                {
                    for (int l = 0; l < _cart.Layers; l++)
                    {
                        double v = _cart[c, l];
                        if ((start >= v) && (v > end))
                            _nav.SelectCoord(c, select, false);
                    }
                }
            }

            Update();
        }

        static DrawCartLevel _def = new DrawCartLevel();

        void SetCart(IDataCartogram c, CoordsConverter validCoords)
        {
            _cart = c;
            if (c != null)// && _cart.AllCoords != null)
                if (validCoords != null)
                    _painter.UpdateCart(_cart, validCoords);
                else
                    _painter.UpdateCart(_cart, _cart.AllCoords);
            else
            {
                _cart = null;
                _painter.UpdateCart(null, null);
            }

            Redraw();
        }

        public void SetCartogram(IDataCartogram c, CartogramPresentationConfig cnf, DrawCartCell classInst, CoordsConverter validCoords)
        {
            _painter = classInst;
            if (c != null)
                _nav.SetDrawCart(_painter, cnf, validCoords != null ? validCoords : c.AllCoords);
            SetCart(c, validCoords);
        }

        public void SetCartogram(IDataCartogram c, CartogramPresentationConfig cnf, CoordsConverter validCoords)
        {
            SetCartogram(c, cnf, _def, validCoords);
        }

        public void SetCartogram(IDataCartogram c, CartogramPresentationConfig cnf, string className, CoordsConverter validCoords)
        {
            SetCartogram(c, cnf, className, validCoords, null);
        }

        public void SetCartogram(IDataCartogram c, CartogramPresentationConfig cnf, string className, CoordsConverter validCoords, DataParamTable info)
        {
            SetCartogram(c, cnf, (DrawCartCell)_plugins.Create(className, info), validCoords);
        }


        public void SetCartogram(IDataCartogram c, IEnviromentEx gpi)
        {
            CartogramPresentationConfig cnf;
            string className;
            DataParamTable cinfo;

            if (gpi.GetCartPresentation(c, out cnf, out className, out cinfo))
                SetCartogram(c,
                    cnf,
                    (DrawCartCell)_plugins.Create(className, cinfo),
                    c.IsNative ? gpi.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, c.Info) : null);
            else
                SetCartogram(c, gpi.DefCartPresentation,
                   (c != null && c.IsNative) ? gpi.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, c.Info) : null);
        }

        public void SetCartogram(IDataCartogram c, IEnviromentEx gpi, string className)
        {
            SetCartogram(c, gpi.DefCartPresentation, className,
                (c != null && c.IsNative) ? gpi.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, c.Info) : null);
        }
        

        static Plugins _plugins;
        CoordNavigation _nav;
        
        DrawCartCell _painter;
        IDataCartogram _cart;

        const int borders = 10;

        Size _canvasSize;
        bool _autoSize = true;
    }

    public delegate void CoordsEventDelegate(object sender, CoodrsEventArgs e);
    public class CoodrsEventArgs : MouseEventArgs
    {
        public Coords Coords
        {
            get { return _crd; }
        }

        public CoodrsEventArgs(Coords c, MouseButtons button, int clicks, int x, int y, int delta)
            : base(button, clicks, x, y, delta)
        {
            _crd = c;

        }
        Coords _crd;
    }
}
