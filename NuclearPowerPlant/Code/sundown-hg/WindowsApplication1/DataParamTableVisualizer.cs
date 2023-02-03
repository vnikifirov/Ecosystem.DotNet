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

namespace corelib
{
    class DataParamTableVisualizer : ITupleItemVisualizerUI
    {
        DataTupleVisualizer _ui;

        ITupleItem _item;
        string _stream;

        public DataParamTableVisualizer(DataTupleVisualizer ui)
        {
            _ui = ui;
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

        public void SetActiveTupleItem(ITupleItem item, string stream)
        {
            _item = item; _stream = stream;
            _ui.SetDataGrid(item.CreateDataGrid(_ui.GetEnviroment()), item, _ui.GetActiveDataTuple(), stream);
        }

        public string GetActiveTupleItemStream()
        {
            return _stream;
        }

        #endregion


        #region ITupleItemVisualizerUI Members

        public IMultiTupleItem GetActiveTupleItems()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetActiveTupleItems(IMultiTupleItem item, string stream)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public IDataTupleVisualizerUI GetDataTupleVisualizer()
        {
            return _ui;
        }
    }
}
