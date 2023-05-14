using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using corelib;

using WindowsApplication1;

namespace corelib
{
    public class DataArrayVisualizer : ITupleItemVisualizerUI
    {
        DataTupleVisualizer _ui;

        IDataArray _item;
        IMultiTupleItem _items;
        string _stream;


        public DataArrayVisualizer(DataTupleVisualizer ui)
        {
            _ui = ui;            
        }


        public static DataGrid CombineDataArrays(IDataArray[] arr)
        {
            DataGrid d = new DataGrid();
            int rank = arr[0].Rank;

            string[] names = { "x", "y", "z" };

            for (int i = 0; i < rank; i++)
            {
                d.Columns.Add(names[i], typeof(int));
            }
            d.HeadColumns = rank;

            for (int w = 0; w < arr.Length; w++)
            {
                //d.Columns.Add(String.Format("Значение {0}", w), arr[w].ElementType);
                
                //d.Columns.Add(arr[w].HumaneName, arr[w].ElementType);
                d.Columns.Add(arr[w].GetHumanName(), arr[w].ElementType);
            }

            if (rank == 1)
            {

                for (int x = 0; ; x++)
                {
                    DataGrid.Row r = d.Rows.Add();
                    r[0] = x;

                    bool empty = true;
                    for (int w = 0; w < arr.Length; w++)
                    {
                        if (arr[w].DimX > x)
                        {
                            empty = false;
                            r[1 + w] = arr[w][x];
                        }
                    }
                    if (empty)
                    {
                        d.Rows.Remove(r);
                        break;
                    }
                }
            }
            else if (rank == 2)
            {
                for (int x = 0; ; x++)
                {
                    bool emptyX = true;
                    for (int y = 0; ; y++)
                    {
                        DataGrid.Row r = d.Rows.Add();
                        r[0] = x;
                        r[1] = y;

                        bool emptyY = true;
                        for (int w = 0; w < arr.Length; w++)
                        {
                            if (arr[w].DimX > x)
                            {
                                emptyX = false;
                                if (arr[w].DimY > y)
                                {
                                    emptyY = false;
                                    r[2 + w] = arr[w][x, y];
                                }
                            }
                        }
                        if (emptyY)
                        {
                            d.Rows.Remove(r);
                            break;
                        }

                    }
                    if (emptyX)
                        break;
                }
            }
            else if (rank == 3)
            {
                for (int x = 0; ; x++)
                {
                    bool emptyX = true;
                    for (int y = 0; ; y++)
                    {
                        bool emptyY = true;
                        for (int z = 0; ; z++)
                        {
                            bool emptyZ = true;

                            DataGrid.Row r = d.Rows.Add();
                            r[0] = x;
                            r[1] = y;
                            r[2] = z;


                            for (int w = 0; w < arr.Length; w++)
                            {
                                if (arr[w].DimX > x)
                                {
                                    emptyX = false;
                                    if (arr[w].DimY > y)
                                    {
                                        emptyY = false;
                                        if (arr[w].DimZ > z)
                                        {
                                            emptyZ = false;
                                            r[3 + w] = arr[w][x, y, z];
                                        }
                                    }
                                }
                            }
                            if (emptyZ)
                            {
                                d.Rows.Remove(r);
                                break;
                            }
                        }
                        if (emptyY)
                            break;
                    }
                    if (emptyX)
                        break;
                }
            }
            else
            {
                throw new NotSupportedException("Bad Rank");
            }

            return d;
        }

        void SetDataArrayListView(IDataArray arr)
        {
            _ui.SetDataGrid(arr.CreateDataGrid(_ui.GetEnviroment()), arr, _ui.GetActiveDataTuple(), _stream);
        }

        void SetDataArraysListView(IDataArray[] arr)
        {
            _ui.SetDataGrid(CombineDataArrays(arr), _items, _ui.GetActiveDataTuples(), _stream);
        }

        #region ITupleItemVisualizerUI Members

        public IMultiDataProvider GetMultiDataProvider()
        {
            return _ui.GetMultiDataProvider();
        }

        public ITupleItem GetActiveTupleItem()
        {
            return _item;
        }

        public string GetActiveTupleItemStream()
        {
            return _stream;
        }

        public void SetActiveTupleItem(ITupleItem item, string stream)
        {
            _item = (IDataArray)item;
            _items = null;
            _stream = stream;

            SetDataArrayListView(_item);
        }

        public IMultiTupleItem GetActiveTupleItems()
        {
            return _items;
        }

        public void SetActiveTupleItems(IMultiTupleItem items, string stream)
        {
            _item = null;
            _items = items;

            IDataArray[] out_items = new IDataArray[items.Count];
            for (int i = 0; i < items.Count; i++)
                out_items[i] = (IDataArray)items[i];


            SetDataArraysListView(out_items);
        }

        #endregion

        public IDataTupleVisualizerUI GetDataTupleVisualizer()
        {
            return _ui;
        }
    }
}
