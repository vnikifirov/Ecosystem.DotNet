#if !DOTNET_V11
#define DATAGRIDCOMMON
#endif

using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WindowsApplication1;

namespace corelib
{
    public class DataGridListVisualizer : UserControl, IDataGridVisualizerUI
    {
        IDataTupleVisualizerUI _parent;
        DataGrid _data;

        public enum DataGridView
        {
            List,
            Graph
        };

        DataGridView _view = DataGridView.Graph;
#if DATAGRIDCOMMON
        DataGridCommon _list = new DataGridCommon();
#else
        DataGridListView _list = new DataGridListView();
#endif
        DataGridGraphView _graph = new DataGridGraphView();

        DataGridViewGraphAction _vgAction;

        class DataGridViewGraphAction : ActionDataTupleVisualizerUI
        {
            DataGridListVisualizer _dav;

            public DataGridViewGraphAction(DataGridListVisualizer dav)
            {
                _name = "DataGridViewGraph";
                _humaneName = "График";
                _descr = "Отображать в виде графика или списка";                

                _image = Resource1.ImageGraph;

                _action = Actions.ToolBarButton;
                _flags = Flags.ToolBarCheckButton;
                if (dav._view == DataGridView.Graph)
                    _flags |= Flags.ToolBarCheckButtonPushed;

                _handler = new EventHandler(onClick);

                _dav = dav;
            }

            void onClick(object sender, EventArgs e)
            {
                if (_dav._view == DataGridView.Graph)
                    _dav.SetDataView(DataGridView.List);
                else
                    _dav.SetDataView(DataGridView.Graph);
            }
        }

        public class DataGridListView : ListView
        {
            public DataGridListView()
            {
                this.View = View.Details;
                this.FullRowSelect = true;
                this.Dock = DockStyle.Fill;
                this.GridLines = true;
            }

            public static void InitListView(ListView l, DataGrid d)
            {
                InitListView(l, d, 60);
            }
            public static void InitListView(ListView l, DataGrid d, int columnSize)
            {
                l.Items.Clear();
                l.Columns.Clear();                

                foreach (DataGrid.Column c in d.Columns)
                {
                    AddColumn(l, c.Header, columnSize);
                }

                ListViewItem[] items = new ListViewItem[d.RowCount];
                string[] lColumns = new string[d.ColumnCount];
                for(int i = 0; i < d.RowCount; i++)
                {
                    DataGrid.Row r = d.StringRows[i];
                    r.CopyTo(lColumns, 0);
                    items[i] = new ListViewItem(lColumns);
                }

                l.Items.AddRange(items);
#if !DOTNET_V11
                l.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
#endif
            }

            static void AddColumn(ListView l, string s, int width)
            {
                ColumnHeader header = new ColumnHeader();
                header.Text = s;
                header.Width = width;

                l.Columns.Add(header);
            }


            public void SetDataGrid(DataGrid data)
            {
                if (data == null)
                {
                    Items.Clear();
                    Columns.Clear();
                }
                else
                    InitListView(this, data);
            }
        }

#if !DOTNET_V11
        public class DataGridCommon : System.Windows.Forms.DataGridView  
        {
            public DataGridCommon()
            {                                
                this.Dock = DockStyle.Fill;
                this.ReadOnly = true;
                this.AllowUserToAddRows = false;
                this.AllowUserToDeleteRows = false;
                this.AllowUserToResizeRows = false;

                this.TopLeftHeaderCell.Value = "#";
                this.RowHeadersWidth = 100;
            }


            protected override void OnSortCompare(DataGridViewSortCompareEventArgs e)
            {
                base.OnSortCompare(e);

                int columnIdx = e.Column.Index;
                int oiginalIndex1 = Convert.ToInt32(Rows[e.RowIndex1].HeaderCell.Value) - 1;
                int oiginalIndex2 = Convert.ToInt32(Rows[e.RowIndex2].HeaderCell.Value) - 1;

                if (_data.Columns[columnIdx].ColumnType == typeof(Sensored))
                {
                    e.SortResult = Sensored.Compare((Sensored)_data.Rows[oiginalIndex1][columnIdx],
                                                    (Sensored)_data.Rows[oiginalIndex2][columnIdx]);
                    e.Handled = true;
                }
                if (_data.Columns[columnIdx].ColumnType == typeof(FiberCoords))
                {
                    e.SortResult = FiberCoords.Compare((FiberCoords)_data.Rows[oiginalIndex1][columnIdx],
                                                       (FiberCoords)_data.Rows[oiginalIndex2][columnIdx]);
                    e.Handled = true;
                }
                else if (_data.Columns[columnIdx].ColumnType == typeof(double))
                {
                    e.SortResult = Math.Sign((double)_data.Rows[oiginalIndex2][columnIdx] -
                                             (double)_data.Rows[oiginalIndex1][columnIdx]);
                    e.Handled = true;
                }
                else if (_data.Columns[columnIdx].ColumnType == typeof(float))
                {
                    e.SortResult = Math.Sign((float)_data.Rows[oiginalIndex2][columnIdx] -
                                             (float)_data.Rows[oiginalIndex1][columnIdx]);
                    e.Handled = true;
                }
                else if (_data.Columns[columnIdx].ColumnType == typeof(int))
                {
                    e.SortResult = Math.Sign((int)_data.Rows[oiginalIndex2][columnIdx] -
                                             (int)_data.Rows[oiginalIndex1][columnIdx]);
                    e.Handled = true;
                }
                else if (_data.Columns[columnIdx].ColumnType == typeof(short))
                {
                    e.SortResult = Math.Sign((short)_data.Rows[oiginalIndex2][columnIdx] -
                                             (short)_data.Rows[oiginalIndex1][columnIdx]);
                    e.Handled = true;
                }

            }

            DataGrid _data;

            public void SetDataGrid(DataGrid d)
            {
                Columns.Clear();
                Rows.Clear();
                _data = d;

                if (d != null)
                {
                    foreach (DataGrid.Column c in d.Columns)
                    {
                        int i =Columns.Add(c.Header, c.Header);
                        Columns[i].ValueType = c.ColumnType;
                    }

                    string[] lColumns = new string[d.ColumnCount];
                    for (int i = 0; i < d.RowCount; i++)
                    {
                        DataGrid.Row r = d.StringRows[i];
                        r.CopyTo(lColumns, 0);

                        int q = Rows.Add(lColumns);
                        Rows[q].HeaderCell.Value = (q + 1).ToString();
                    }
                    
                    AutoResizeColumnHeadersHeight();
                    //AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader);
                }
            }
        }

#endif

        public class DataGridGraphView : Graph
        {
            public DataGridGraphView()
            {
                this.Dock = DockStyle.Fill;
            }
            public void SetDataGrid(DataGrid data)
            {
                this.ClearData();

                if (data != null)
                {
                    int dataCols = data.ColumnCount - data.HeadColumns;

                    for (int i = data.HeadColumns; i < data.ColumnCount; i++)
                    {
                        double[] ndata = new double[data.RowCount];
                        for (int j = 0; j < data.RowCount; j++)
                            ndata[j] = Convert.ToDouble(data.Columns[i].Converter.GetValue(data.Rows[j][i]));

                        Graph.GraphData gd = this.AddLinearData(ndata);
                        if (gd!=null)
                            gd.LineLegend = data.Columns[i].Header;
                    }

                    this.LegendEnabled = dataCols > 1;
                }
            }
        }

        public DataGridListVisualizer(IDataTupleVisualizerUI parent)
        {
            _parent = parent;

            _parent.RegisterAction(_vgAction = new DataGridViewGraphAction(this));
        }

        #region IDataGridManipulator Members

        public DataGrid GetActiveDataGrid()
        {
            return _data;
        }

        public void SetDataGrid(DataGrid data)
        {
            _data = data;
            
            _parent.VisibleAction(_vgAction, data != null);            
            //if (data == null)
            _parent.EnableAction(_vgAction, true);


            SetDataView(_view);
        }

        
        public void SetDataView(DataGridView view)
        {
            this.Controls.Clear();

            bool grpahCapable = true;
            if (_data != null)
            {
                for (int i = _data.HeadColumns; i < _data.ColumnCount; i++)
                {
                    DataGrid.Column c = _data.Columns[i];
                    if (!(c.ColumnType == typeof(double) || c.ColumnType == typeof(float) ||
                        c.ColumnType == typeof(int) || c.ColumnType == typeof(short) ||
                        c.ColumnType == typeof(long) || c.ColumnType == typeof(byte)))
                    {
                        grpahCapable = false;
                        break;
                    }
                }
            }

            _parent.EnableAction(_vgAction, grpahCapable);
            if (!grpahCapable && view != DataGridView.List)
            {
                _list.SetDataGrid(_data);
                this.Controls.Add(_list);
                return;
            }

            if (view == DataGridView.List)
            {
                _list.SetDataGrid(_data);
                this.Controls.Add(_list);
                _view = view;
            }
            else
            {
                try
                {
                    _graph.SetDataGrid(_data);
                    this.Controls.Add(_graph);
                    _view = view;
                }
                catch (System.Exception)
                {
                    SetDataView(DataGridView.List);

                    _parent.EnableAction(_vgAction, false);
                }
            }
        }

        #endregion

        #region IDataTupleVisualizerGetter Members

        public IDataTupleVisualizerUI GetDataTupleVisualizer()
        {
            return _parent;
        }

        #endregion
    }
}
