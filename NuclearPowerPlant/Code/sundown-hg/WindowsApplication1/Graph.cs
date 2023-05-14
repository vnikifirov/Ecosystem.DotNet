using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using corelib;

namespace corelib
{
    /// <summary>
    /// Summary for Graph
    /// </summary>
    public class Graph : System.Windows.Forms.UserControl
    {
        public Graph()
        {
            InitializeComponent();
            //
            //TODO: Add the constructor code here
            //
            InitInternals();
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Graph
            // 
#if !DOTNET_V11
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
#endif
            this.Name = "Graph";
            this.Size = new System.Drawing.Size(176, 160);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Graph_Paint);
            this.Resize += new System.EventHandler(this.Graph_Resize);
            this.MouseUp += new MouseEventHandler(Graph_MouseUp);
            this.ResumeLayout(false);

            this.MouseWheel += new MouseEventHandler(Graph_MouseWheel);
            this.KeyUp += new KeyEventHandler(Graph_KeyUp);
            this.KeyDown += new KeyEventHandler(Graph_KeyDown);
            this.MouseMove += new MouseEventHandler(Graph_MouseMove);
            this.MouseDown += new MouseEventHandler(Graph_MouseDown);
        }

        int _mx;
        int _my;
        float _fx;
        float _fy;

        void Graph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mx = e.X;
                _my = e.Y;

                _fx = logicalViewport.X;
                _fy = logicalViewport.Y;
            }
        }

        void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                float py = (float)(e.Y - _my) / drawArea.Height;
                float px = -(float)(e.X - _mx) / drawArea.Width;

                logicalViewport.Y = _fy + py * logicalViewport.Height;
                logicalViewport.X = _fx + px * logicalViewport.Width;

                if (logicalViewport.Y + logicalViewport.Height > 1)
                    logicalViewport.Y = 1 - logicalViewport.Height;
                else if (logicalViewport.Y < 0)
                    logicalViewport.Y = 0;

                if (logicalViewport.X + logicalViewport.Width > 1)
                    logicalViewport.X = 1 - logicalViewport.Width;
                else if (logicalViewport.X < 0)
                    logicalViewport.X = 0;

                ReCalcViewPort();
                Refresh();
            }
        }

        bool _keyControl = false;
        bool _keyShift = false;
        bool _keyAlt = false;

        void Graph_KeyUp(object sender, KeyEventArgs e)
        {
            _keyControl = e.Control;
            _keyShift = e.Shift;
            _keyAlt = e.Alt;
        }

        void Graph_KeyDown(object sender, KeyEventArgs e)
        {
            _keyControl = e.Control;
            _keyShift = e.Shift;
            _keyAlt = e.Alt;
        }

        void Graph_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta / 600.0f;
            if (delta < 0)
                delta = 1 / (1 + Math.Abs(delta));
            else
                delta = 1 + delta;

            
            float rh = delta * logicalViewport.Height;
            float rw = delta * logicalViewport.Width;

            //logicalViewport.Left = rh/2

            float py = 1-(float)(e.Y - drawArea.Y) / drawArea.Height;
            float px = (float)(e.X - drawArea.X) / drawArea.Width;

            if (px < 0)
                px = 0;
            if (py < 0)
                py = 0;
            if (px > 1f)
                px = 1f;
            if (py > 1f)
                py = 1f;

            bool doHeight =!_keyShift;
            bool doWidth = !_keyControl;

            
            if (doHeight)
            {
                logicalViewport.Y = logicalViewport.Y + py * (logicalViewport.Height - rh);
                logicalViewport.Height = rh;

                if (logicalViewport.Y < 0)
                    logicalViewport.Y = 0;

                if (logicalViewport.Height + logicalViewport.Y > 1)
                {
                    //logicalViewport.Y = 0;
                    logicalViewport.Height = 1 - logicalViewport.Y;
                }


            }

            if (doWidth)
            {
                logicalViewport.X = logicalViewport.X + px * (logicalViewport.Width - rw);
                logicalViewport.Width = rw;

                if (logicalViewport.X < 0)
                    logicalViewport.X = 0;

                if (logicalViewport.Width + logicalViewport.X > 1)
                {
                    //logicalViewport.X = 0;
                    logicalViewport.Width = 1 - logicalViewport.X;
                }


            }


            ReCalcViewPort();
            Refresh();
            
        }


        #endregion

        void Graph_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    GraphData graphData = (GraphData)data[i];
                    if ((graphData.hide) || (graphData.legend == null))
                        continue;

                    if (graphData.posBas.Contains(new Point(e.X, e.Y)))
                    {
                        graphData.dontDraw = !graphData.dontDraw;
                        Invalidate();
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Если нажата правая кнопка на ячейку которая не перечеркнута и все 
                // остальные перечеркнуты, то сделать активными все
                // Иначе все кроме нажатой ячейки не отрисовывать
                GraphData graphDataFound = null;
                bool otherHidden = true;
                for (int i = 0; i < data.Count; i++)
                {
                    GraphData graphData = (GraphData)data[i];
                    if ((graphData.hide) || (graphData.legend == null))
                        continue;

                    if (graphData.posBas.Contains(new Point(e.X, e.Y)))
                    {
                        graphDataFound = graphData;
                    }
                    else
                    {
                        otherHidden &= graphData.dontDraw;
                    }
                }

                if (graphDataFound == null)
                    return;

                bool riseUp = (otherHidden)  && (graphDataFound.dontDraw != true);
                for (int i = 0; i < data.Count; i++)
                {
                    GraphData graphData = (GraphData)data[i];
                    if ((graphData.hide) || (graphData.legend == null))
                        continue;

                    graphData.dontDraw = !riseUp;
                }
                graphDataFound.dontDraw = false;
                Invalidate();
                return;
            }
        }



        string ComposeTextDouble(double f)
        {
            return f.ToString("0.#####");
        }

        void Graph_Paint(Object sender, PaintEventArgs e)
        {

            int i, j, k;
            int cx, cy;

            if ((data.Count == 0)  || (areaBrush == null))
            {
                e.Graphics.DrawRectangle(borderPen, drawArea);
                return;
            }
            Size subSpace = new Size();

            if (drawValues)
            {
                SizeF max = new SizeF();
                double f;
                f = (ymin + legStepValY) / legStepValY;
                if (Math.Floor(f) != f)
                    f = Math.Floor(f) * legStepValY;
                else
                    f = (Math.Floor(f) - 1) * legStepValY;
                for (; f <= ymax; f += legStepValY)
                {
                    cy = (int)(tr_ay * f + tr_by);

                    if (cy < drawArea.Top)
                        continue;
                    if (cy > drawArea.Bottom)
                        continue;


                    SizeF l = e.Graphics.MeasureString(ComposeTextDouble(f), Font);
                    if (l.Width > max.Width)
                        max = l;

                    if (legStepValY == 0.0)
                        break;
                }
                subSpace.Width = (int)Math.Ceiling(max.Width) + 7;
                subSpace.Height = (int)Math.Ceiling(max.Height) + 3;

                maximumYTicks = Math.Max(6, drawArea.Height / (2 * subSpace.Height));
                maximumYTicks = Math.Min(maximumYTicks, 19);
                ReCalcLegendScale();
            }

            if (drawArea.X < subSpace.Width)
            {
                int delta = subSpace.Width - drawArea.X;

                drawArea.X += delta;
                drawArea.Width -= delta;

                UpdateTRIndex();
            }

            e.Graphics.FillRectangle(areaBrush, drawArea);



            for (i = 0; i < data.Count; i++)
            {
                GraphData graphData = (GraphData)data[i];

                if (graphData.hide)
                    continue;

                if (graphData.dontDraw)
                    continue;

                int oCx = 0;
                int oCy = 0;
                int ofy = 0;

                bool wasDowned = false;

                Point[] points = graphData.LinePoints; //new Point[2 * graphData.data.Length];

                k = 0;

                if ((graphData.PolygonDraw))
                {
                    points[0] = new Point((int)(tr_ax * 0 + tr_bx), drawArea.Bottom); k++;
                }

                for (j = 0; j < graphData.data.Length; j++)
                {
                    int pcx = cx = (int)Math.Round(tr_ax * j + tr_bx);
                    int pcy = cy = (int)Math.Round(tr_ay * graphData.data[j] + tr_by);

                    if (cx < drawArea.Left)
                    {
                        int ncx = (int)(tr_ax * (j + 1) + tr_bx);                        
                        if (ncx < drawArea.Left)
                        {
                            goto End;
                        }
                        int ncy = j < graphData.data.Length ? (int)(tr_ay * graphData.data[j + 1] + tr_by) : pcy;

                        double p = ((double)drawArea.Left - pcx) / (ncx - pcx);
                        cy = (int)Math.Round(pcy + p * (ncy - pcy));
                        cx = drawArea.Left;

                        if (cy > drawArea.Bottom)
                        {
                            wasDowned = true;
                            ofy = drawArea.Bottom;
                        }
                        else if (cy < drawArea.Top)
                        {
                            wasDowned = true;
                            ofy = drawArea.Top;
                        }

                        pcx = cx;
                        pcy = cy;
                    }
                    else if (cx > drawArea.Right)
                    {
                        if (oCx > drawArea.Right)
                            break;

                        double p = ((double)drawArea.Right - oCx) / (pcx - oCx);
                        cy = (int)Math.Round(oCy + p * (pcy - oCy));
                        cx = drawArea.Right;

                        pcx = cx;
                        pcy = cy;
                    }

                    if ((wasDowned) &&
                        ((cy < drawArea.Bottom && cy > drawArea.Top) ||
                        (cy >= drawArea.Bottom && ofy == drawArea.Top) ||
                        (cy <= drawArea.Top && ofy == drawArea.Bottom)
                        ))
                    {
                        double p = ((double)ofy - oCy) / (pcy - oCy);
                        if (pcy == oCy)
                        {
                            p = 1.0;
                        }
                        cx = (int)Math.Round(oCx + p * (pcx - oCx));

                        if (cx < drawArea.Left)
                            cx = drawArea.Left;
                        else if (cx > drawArea.Right)
                            cx = drawArea.Right;

                        points[k++] = new Point(cx, ofy);
                        cx = pcx;

                        wasDowned = false;
                    }


                    if (cy > drawArea.Bottom)
                    {
                        double p = 1.0;
                        if (!wasDowned)
                        {
                            p = ((double)drawArea.Bottom - oCy) / (pcy - oCy);
                            if (pcy == oCy)
                            {                                
                                p = 1.0;
                            }
                        }

                        cy = drawArea.Bottom;
                        cx = (int)Math.Round(oCx + p * (pcx - oCx));

                        if (cx < drawArea.Left)
                            cx = drawArea.Left;
                        else if (cx > drawArea.Right)
                            cx = drawArea.Right;

                        if (!wasDowned)
                            points[k++] = new Point(cx, cy);
                        wasDowned = true;
                    }
                    else if (cy < drawArea.Top)
                    {
                        double p = 1.0;
                        if (!wasDowned)
                        {
                            p = ((double)drawArea.Top - oCy) / (pcy - oCy);
                            if (pcy == oCy)
                            {
                                p = 1.0;
                            }
                        }

                        cy = drawArea.Top;
                        cx = (int)Math.Round(oCx + p * (pcx - oCx));

                        if (cx < drawArea.Left)
                            cx = drawArea.Left;
                        else if (cx > drawArea.Right)
                            cx = drawArea.Right;

                        if (!wasDowned)
                            points[k++] = new Point(cx, cy);

                        wasDowned = true;
                    }
                    else
                    {
                        points[k++] = new Point(cx, cy);
                    }

                    
                End:
                    //graphData.LinePoints[j] = new Point((int)Math.Round(tr_ax * j + tr_bx),
                    //    (int)Math.Round(tr_ay * graphData.data[j] + tr_by));

                    oCx = pcx;
                    oCy = pcy;
                    ofy = cy;
                }

                for (; k < points.Length; k++)
                {
                    points[k] = new Point(oCx, ofy);
                }

                if (graphData.PolygonDraw)
                {
                    points[points.Length-1] = new Point(graphData.LinePoints[k - 1].X, graphData.LinePoints[0].Y);
                    //graphData.LinePoints[k] = new Point(graphData.LinePoints[k - 1].X, graphData.LinePoints[0].Y);
                    e.Graphics.FillPolygon(graphData.BrushLine, graphData.LinePoints);
                }
                else
                {
                    //e.Graphics.DrawLines(Pens.Green, graphData.LinePoints);
                    e.Graphics.DrawLines(graphData.PenLine, points);

                    //e.Graphics.DrawLines(graphData.PenLine, graphData.LinePoints);
                }
            }

            //Draw Mark Line
            if ((lineMarkEnabled) && (xmax >= xPos) && (xmin <= xPos))
            {
                cx = (int)(tr_ax * xPos + tr_bx);
                int dx = Math.Min(drawArea.Height, drawArea.Width) / 60;
                if (dx < 5)
                    dx = 5;

                Point[] points = new Point[3] {
                     new Point (cx, drawArea.Top),
                     new Point (cx + dx, drawArea.Top - dx),
                     new Point (cx - dx, drawArea.Top - dx)};

                e.Graphics.FillPolygon(markBrush, points);

                points = new Point[3] {
                     new Point (cx, drawArea.Bottom),
                     new Point (cx + dx, drawArea.Bottom + dx),
                     new Point (cx - dx, drawArea.Bottom + dx)};

                e.Graphics.FillPolygon(markBrush, points);

                e.Graphics.DrawLine(markPen, new Point(cx, drawArea.Top), new Point(cx, drawArea.Bottom));
            }

            if (drawValues)
            {
                int lentick = (int)Math.Sqrt((drawArea.Bottom - drawArea.Top) * (drawArea.Bottom - drawArea.Top) +
                   (drawArea.Right - drawArea.Left) * (drawArea.Right - drawArea.Left)) / 150;
                if (lentick < 3)
                    lentick = 3;

                //Draw X ticks
                double f = (xmin + legStepValX) / legStepValX;
                if (Math.Floor(f) != f)
                    f = Math.Floor(f) * legStepValX;
                else
                    f = (Math.Floor(f) - 1) * legStepValX;

                for (; f <= xmax; f += legStepValX)
                {
                    cx = (int)(tr_ax * f + tr_bx);

                    if (cx < drawArea.Left)
                        continue;
                    if (cx > drawArea.Right)
                        continue;


                    if (drawGrid)
                        e.Graphics.DrawLine(gridPen, new Point(cx, drawArea.Top),
                           new Point(cx, drawArea.Bottom));

                    e.Graphics.DrawLine(borderPen, new Point(cx, drawArea.Bottom - lentick)
                       , new Point(cx, drawArea.Bottom));

                    e.Graphics.DrawString(ComposeTextDouble(f), Font, textBrush,
                       new RectangleF(cx - (float)(tr_ax * legStepValX), drawArea.Bottom + (float)lentick / 2,
                       2 * (float)(tr_ax * legStepValX), Bottom - drawArea.Bottom - (float)lentick / 2), horzTicksSt);
                }

                //Draw Y ticks
                f = (ymin + legStepValY) / legStepValY;
                if (Math.Floor(f) != f)
                    f = Math.Floor(f) * legStepValY;
                else
                    f = (Math.Floor(f) - 1) * legStepValY;

                for (; f <= ymax; f += legStepValY)
                {
                    cy = (int)(tr_ay * f + tr_by);

                    if (cy < drawArea.Top)
                        continue;
                    if (cy > drawArea.Bottom)
                        continue;

                    if (drawGrid)
                        e.Graphics.DrawLine(gridPen, new Point(drawArea.Right, cy),
                           new Point(drawArea.Left, cy));

                    e.Graphics.DrawLine(borderPen, new Point(drawArea.Left + lentick, cy),
                       new Point(drawArea.Left, cy));

                    e.Graphics.DrawString(ComposeTextDouble(f), Font, textBrush,
                       new RectangleF(0, cy + (float)(tr_ay * legStepValY), (float)drawArea.Left - lentick,
                                  -2 * (float)(tr_ay * legStepValY)), vertTicksSt);
                }
            }

            //Draw border
            e.Graphics.DrawRectangle(borderPen, drawArea);

            //Darw bars
            if ((drawBars) &&
                ((logicalViewport.Width < 1) || (logicalViewport.Height < 1)))
            {
                int barw =3;

                e.Graphics.DrawRectangle(borderPen,
                    new Rectangle(drawArea.Left, drawArea.Top - barw, drawArea.Width, barw));

                e.Graphics.FillRectangle(Brushes.Gray,
                    new Rectangle(
                    (int)(drawArea.Left + logicalViewport.X * (drawArea.Width)),
                    drawArea.Top - barw + 1,
                    (int)(logicalViewport.Width * (drawArea.Width))+1,
                     barw - 1));


                e.Graphics.DrawRectangle(borderPen,
                    new Rectangle(drawArea.Right, drawArea.Top, barw, drawArea.Height));

                e.Graphics.FillRectangle(Brushes.Gray,
                    new Rectangle(
                    drawArea.Right + 1,
                    (int)(drawArea.Top + logicalViewport.Y * (drawArea.Height))+1,
                    barw - 1,
                    (int)(logicalViewport.Height * (drawArea.Height))));

            }

            //Draw legend
            if ((legendHeight > 0) && (legendWidth > 0))
            {
                e.Graphics.FillRectangle(legendBrush, legendArea);
                e.Graphics.DrawRectangle(borderPen, legendArea);

                for (int ii = 0, p = 0; ii < data.Count; ii++)
                {
                    GraphData graphData = (GraphData)data[ii];

                    graphData.posBas = new Rectangle(legendArea.Left + barSize, legendArea.Top + barSize + p * vertSkip, barSize, barSize);

                    if ((graphData.hide) || (graphData.legend == null))
                        continue;
                    

                    e.Graphics.FillRectangle(graphData.BrushLine, graphData.posBas);
                       //new Rectangle(legendArea.Left + barSize, legendArea.Top + barSize + p * vertSkip, barSize, barSize));
                    e.Graphics.DrawRectangle(borderPen, graphData.posBas);
                       //new Rectangle(legendArea.Left + barSize, legendArea.Top + barSize + p * vertSkip, barSize, barSize));

                    if (graphData.dontDraw)
                    {
                        e.Graphics.DrawLine(borderPen,
                            graphData.posBas.Left, graphData.posBas.Top, graphData.posBas.Right, graphData.posBas.Bottom);
                        e.Graphics.DrawLine(borderPen,
                            graphData.posBas.Left, graphData.posBas.Bottom, graphData.posBas.Right, graphData.posBas.Top);
                    }

                    e.Graphics.DrawString(graphData.legend, Font, textBrush,
                       new RectangleF(legendArea.Left + (float)3 * barSize, legendArea.Top + barSize + (float)p * vertSkip,
                       legendArea.Right - legendArea.Left - (float)3 * barSize, (float)barSize));
                    p++;
                }
            }
        }

        void Graph_Resize(System.Object sender, System.EventArgs e)
        {
            ReCalcViewPort();
			if (areaBrush != null) 
			{
				areaBrush.Dispose();
				areaBrush = null;
			}

            areaBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, Size.Height),
              Color.White, Color.AntiqueWhite);

            //   areaBrush = new SolidBrush(Color.White);

            Refresh();
        }

        void UpdateTRIndex()
        {
            tr_ax = (drawArea.Right - drawArea.Left) / (v_qx - v_px);
            tr_bx = drawArea.Left - tr_ax * v_px;

            tr_ay = (drawArea.Top - drawArea.Bottom) / (v_qy - v_py);
            tr_by = drawArea.Bottom - tr_ay * v_py;
        }

        void ReCalcViewPort()
        {
            int px = (int)(Size.Width * 0.03) + 10;
            int py = (int)(Size.Height * 0.03) + 10;

            if ((legendHeight == 0) || (legendWidth == 0))
            {
                drawArea = new Rectangle(px, py, Size.Width - 2 * px, Size.Height - 2 * py);
            }
            else
            {
                drawArea = new Rectangle(px, py, Size.Width - 2 * px - legendWidth, Size.Height - 2 * py);

                legendArea = new Rectangle(px / 2 + drawArea.Right, (Size.Height - legendHeight) / 2, legendWidth, legendHeight);
            }

            // Recall transform coeff 
            v_px = xmin + (xmax - xmin) * logicalViewport.Left;
            v_qx = xmin + (xmax - xmin) * logicalViewport.Right;
            v_py = ymin + (ymax - ymin) * logicalViewport.Top;
            v_qy = ymin + (ymax - ymin) * logicalViewport.Bottom;

            UpdateTRIndex();
        }

        void ReCalcLegend()
        {
            barSize = 0;
            maxTextWidth = 0;
            maxTextHeight = 0;
            legendHeight = 0;
            legendWidth = 0;

            if (drawLegend)
            {
                int i, cnt = 0;
                Graphics g = CreateGraphics();
                for (i = 0; i < data.Count; i++)
                {
                    GraphData graphData = (GraphData)data[i];

                    if ((graphData.hide) || (graphData.legend == null))
                        continue;

                    SizeF sz;
                    sz = g.MeasureString(graphData.legend, Font);
                    graphData.legendText = new Size((int)sz.Width, (int)sz.Height);

                    if (graphData.legendText.Width > maxTextWidth)
                        maxTextWidth = graphData.legendText.Width;
                    if (graphData.legendText.Height > maxTextHeight)
                        maxTextHeight = graphData.legendText.Height;

                    barSize = (int)(maxTextHeight * 0.85);
                    vertSkip = (int)(maxTextHeight * 1.15);
                    cnt++;
                }
                if ((maxTextWidth > 0) && (maxTextHeight > 0))
                {
                    //Update legend place
                    legendWidth = barSize * 4 + maxTextWidth;
                    legendHeight = barSize * 2 + vertSkip * cnt;
                }
                g.Dispose();
            }
            else
            {
                legendArea = new Rectangle(0, 0, 0, 0);
            }
        }

        void ReCalcLegendScale()
        {
            int i;

            legStepValX = legStepValY = 0;

            double visibleXmax = xmin + (xmax - xmin) * (logicalViewport.X + logicalViewport.Width);
            double visibleXmin = xmin + (xmax - xmin) * logicalViewport.X;
            double visibleYmax = ymin + (ymax - ymin) * (logicalViewport.Y + logicalViewport.Height);
            double visibleYmin = ymin + (ymax - ymin) * logicalViewport.Y;

            double valXc = Math.Log10((visibleXmax - visibleXmin));
            double valYc = Math.Log10((visibleYmax - visibleYmin));
            double[] scales = { 5, 2, 1, 0.5, 0.2 };
            double valX = Math.Pow(10, (int)Math.Floor(valXc));
            double valY = Math.Pow(10, (int)Math.Floor(valYc));
            double scaleYgood = 0;
            for (i = 0; i < scales.Length; i++)
            {
                int xticks = (int)((visibleXmax - visibleXmin) / (scales[i] * valX));
                if (scales[i] * valX < minimumXStep)
                {
                    legStepValX = (i > 0) ? scales[i - 1] * valX : scales[i] * valX;
                    break;
                }
                if (5 < xticks && xticks < maximumXTicks)
                    legStepValX = scales[i] * valX;
            }
            for (i = 0; i < scales.Length; i++)
            {
                int yticks = (int)((visibleYmax - visibleYmin) / (scales[i] * valY));
                if (4 <= yticks && yticks < maximumYTicks)
                    legStepValY = scales[i] * valY;

                if ((3 <= yticks) && (scaleYgood == 0))
                    scaleYgood = scales[i];
            }
            if (legStepValX == 0)
                legStepValX = scales[i - 1] * valX;
            if (legStepValY == 0)
                legStepValY = ((scaleYgood != 0) ? scaleYgood : scales[i - 1] ) * valY;
        }

        void ReCalcArea()
        {
            if (data.Count > 0)
            {
                int i = 0;
                GraphData graphData = (GraphData)data[i];

                ymin = graphData.min_val;
                ymax = graphData.max_val;
                xmin = graphData.min_X;
                xmax = graphData.max_X;

                for (i = 1; i < data.Count; i++)
                {
                    graphData = (GraphData)data[i];

                    if (graphData.min_val < ymin)
                        ymin = graphData.min_val;
                    if (graphData.min_X < xmin)
                        xmin = graphData.min_X;

                    if (graphData.max_val > ymax)
                        ymax = graphData.max_val;
                    if (graphData.max_X > xmax)
                        xmax = graphData.max_X;
                }

                ReCalcLegendScale();

            }
            else
            {
                ymin = ymax = xmin = xmax = 0;
            }
        }


        static Graph()
        {
            horzTicksSt = new StringFormat();
            horzTicksSt.Alignment = StringAlignment.Center;
            horzTicksSt.LineAlignment = StringAlignment.Near;
            horzTicksSt.FormatFlags = StringFormatFlags.NoWrap;

            vertTicksSt = new StringFormat();
            vertTicksSt.Alignment = StringAlignment.Far;
            vertTicksSt.LineAlignment = StringAlignment.Center;
            vertTicksSt.FormatFlags = StringFormatFlags.NoWrap;

            strLegendFormat = new StringFormat();
            strLegendFormat.Alignment = StringAlignment.Near;
            strLegendFormat.LineAlignment = StringAlignment.Center;
            strLegendFormat.FormatFlags = StringFormatFlags.NoWrap;
        }

        void InitInternals()
        {
            borderPen = Pens.Black;
            logicalViewport = new RectangleF(0, 0, 1.0f, 1.0f);
            textBrush = Brushes.Black;

            gridPen = new Pen(Color.Black);
            gridPen.DashStyle = DashStyle.Dot;

            drawValues = true;
            drawLegend = true;
            drawGrid = true;

#if !DOTNET_V11
            DoubleBuffered = true;
#endif
        }



        //////////////////////////////////////////////////////////////////////////
        // PUBLIC METHODS
        //////////////////////////////////////////////////////////////////////////


        public bool LegendEnabled
        {
            get
            {
                return (drawLegend && (legendHeight > 0) && (legendWidth > 0));
            }
            set
            {
                if (drawLegend != value)
                {
                    drawLegend = value;

                    ReCalcLegend();
                    ReCalcViewPort();
                }
            }
        }

        public bool GridEnabled
        {
            get { return (drawGrid); }
            set { drawGrid = value; }
        }
        public double MarkX
        {
            get { return xPos; }
            set { xPos = value; }
        }

        public bool MarkXEnabled
        {
            get
            {
                return lineMarkEnabled;
            }
            set
            {
                lineMarkEnabled = value;
                if (value != false)
                {
                    if (markBrush == null)
                        markBrush = new SolidBrush(Color.DeepSkyBlue);

                    if (markPen == null)
                        markPen = new Pen(Color.DeepSkyBlue);
                }
            }
        }

        public void ClearData()
        {
            data.Clear();
        }

        public GraphData AddLinearData(int[] dt)
        {
            double[] data = new double[dt.Length];
            for (int i = 0; i < dt.Length; i++)
                data[i] = (double)dt[i];

            return AddLinearData(data);
        }

        public GraphData AddLinearData(double[] dt)
        {
            if (dt.Length == 0)
                return null;

            GraphData gd = new GraphData(this);

            gd.data = dt;

            gd.max_val = MathOp.max(dt);
            gd.min_val = MathOp.min(dt);

            gd.min_X = 0;
            gd.max_X = dt.Length - 1;

            gd.color = DefColors.colors[data.Count % DefColors.colors.Length];

            data.Add(gd);
            ReCalcArea();
            ReCalcViewPort();

            return gd;
        }

        struct DefColors
        {
            static internal readonly Color[] colors = 
               {
                  Color.Red,
                  Color.Green,
                  Color.DarkCyan,
                  Color.Chocolate,
                  Color.DarkGoldenrod,
                  Color.DarkOliveGreen,
                  Color.DeepPink,
                  Color.DarkTurquoise,

                  Color.DarkKhaki,
                  Color.CornflowerBlue,
                  Color.DarkSalmon,
                  Color.Blue,
                  Color.DarkViolet,
                  Color.DarkOrange,
                  Color.DarkGray,
                  Color.DarkSlateGray
               };
        }

        //int style;
#if !DOTNET_V11
        List<GraphData> data = new List<GraphData>();
#else
        ArrayList data = new ArrayList();
#endif
        Rectangle drawArea;
        Brush areaBrush;
        Brush textBrush;
        Pen borderPen;
        RectangleF logicalViewport;

        double minimumXStep = 1.0;
        int maximumYTicks = 11;
        int maximumXTicks = 15;

        // Transformation of coords and logical view port
        double ymin, ymax, xmin, xmax;
        double v_px, v_py, v_qx, v_qy;
        double tr_ax, tr_bx, tr_ay, tr_by;

        // Vert line mark
        bool lineMarkEnabled;
        double xPos;
        Brush markBrush;
        Pen markPen;

        // Grid and ticks
        static StringFormat horzTicksSt;
        static StringFormat vertTicksSt;
        double legStepValX, legStepValY;
        bool drawValues;
        Pen gridPen;
        bool drawGrid;

        // Legend
        bool drawLegend;
        int barSize;
        int vertSkip;
        int maxTextWidth; int legendWidth;
        int maxTextHeight; int legendHeight;
        Rectangle legendArea;
        Brush legendBrush = Brushes.White;
        static StringFormat strLegendFormat;

        // scroll
        bool drawBars = true;

        public bool DrawViewportBars
        {
            get { return drawBars; }
            set
            {
                drawBars = value;
                Refresh();
            }
        }

        public RectangleF LogicalViewport
        {
            get { return logicalViewport; }
            set
            {
                logicalViewport = value;
                if (logicalViewport.X < 0)
                    logicalViewport.X = 0;
                if (logicalViewport.Width > 1)
                    logicalViewport.Width = 1;
                if (logicalViewport.Width + logicalViewport.X > 1)
                    logicalViewport.X = 1 - logicalViewport.Width;

                if (logicalViewport.Y < 0)
                    logicalViewport.Y = 0;
                if (logicalViewport.Height > 1)
                    logicalViewport.Height = 1;
                if (logicalViewport.Height + logicalViewport.Y > 1)
                    logicalViewport.Y = 1 - logicalViewport.Height;

                ReCalcViewPort();
                Refresh();
            }
        }

        public class GraphData
        {
            internal bool hide;
            internal bool dontDraw = false;
            //internal Object[] legendX;
            //internal double[] dataX;
            internal double min_X;
            internal double max_X;

            internal double[] data;
            internal double min_val;
            internal double max_val;
            internal String legend;

            //internal bool solidDraw;
            internal Color color;

            internal Rectangle posBas = new Rectangle(0,0,0,0);

            // Properties
            internal Brush BrushLine
            {
                get
                {
                    if (brushLn == null)
                        brushLn = new SolidBrush(color);
                    return brushLn;
                }
            }

            internal Pen PenLine
            {
                get
                {
                    if (line == null)
                        line = new Pen(color);
                    return line;
                }
            }

            internal Point[] LinePoints
            {
                get
                {
                    if (cpts == null)
                    {
                        int len = 4+2*data.Length + ((polygonDraw) ? 2 : 0);
                        cpts = new Point[len];
                    }
                    return cpts;
                }
            }

            public bool PolygonDraw
            {
                get { return polygonDraw; }
                set
                {
                    polygonDraw = value;
                    cpts = null;
                }
            }

            public Color LineColor
            {
                get { return color; }
            }

            public bool LineHidden
            {
                get { return hide; }
                set
                {
                    hide = value;

                    _obj.ReCalcLegend();
                    _obj.ReCalcViewPort();
                }
            }

            public String LineLegend
            {
                get { return legend; }
                set
                {
                    legend = value;

                    _obj.ReCalcLegend();
                    _obj.ReCalcViewPort();
                }
            }


            // DEPRECATED!!!
            bool LineFill
            {
                get { return PolygonDraw; }
                set { PolygonDraw = value; }
            }

            internal GraphData(Graph obj)
            {
                _obj = obj;
            }

            Graph _obj;

            // CACHE
            public Size legendText;
            bool polygonDraw;

            // CACHE
            Pen line;
            Point[] cpts;
            Brush brushLn;
        }
    }
}
