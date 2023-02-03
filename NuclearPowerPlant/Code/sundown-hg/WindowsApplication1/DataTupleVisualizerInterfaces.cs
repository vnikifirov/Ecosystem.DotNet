using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.ComponentModel;
using System.Drawing;

namespace corelib
{

    public interface ITreeItemFormatter
    {
        string GetStreamName(string streamName);
        string GetSingleDataTupleName(string streamName, DateTime dateTime);
        string GetSingleTupleItemName(string streamName, DateTime dateTime,
            TupleMetaData itemInfo);

        string GetMultiDataTupleName(string streamName, DateTime dateTime, int idx);
        string GetMultiTupleItemName(string streamName, DateTime dateTime, int idx, TupleMetaData itemInfo);
    }

    public enum TreeSelectionType
    {
        None = 0,
        Stream,
        SingleDataTuple,
        SingleTupleItem,
        MultiDataTuples,
        MultiDataTuplesSingle,
        MultiTupleItems,
        MultiTupleItemsSingle
    }

    public class SelectedTupleItem
    {
        TreeSelectionType _selection;
        public string _stream;
        public TupleMetaData _tupleInfo;
        public DateTime _dt;
        public int _idx;

        public const int AllItems = -1;

        public TreeSelectionType Selection
        {
            get { return _selection; }
        }

        public string Stream
        {
            get { return _stream; }
        }

        public DateTime DateTime
        {
            get { return _dt; }
        }

        public TupleMetaData TupleInfo
        {
            get { return _tupleInfo; }
        }

        public int Index
        {
            get { return _idx; }
        }

        public bool IsMulti
        {
            get
            {
                return (_idx == AllItems) &&
              (_selection == TreeSelectionType.MultiDataTuples || _selection == TreeSelectionType.MultiTupleItems);
            }
        }

        public bool IsSetStreamInfo
        {
            get
            {
                return (_selection == TreeSelectionType.Stream ||
                    _selection == TreeSelectionType.SingleDataTuple ||
                    _selection == TreeSelectionType.SingleTupleItem ||
                    _selection == TreeSelectionType.MultiDataTuples ||
                    _selection == TreeSelectionType.MultiDataTuplesSingle ||
                    _selection == TreeSelectionType.MultiTupleItems ||
                    _selection == TreeSelectionType.MultiTupleItemsSingle);
            }
        }

        public bool IsSetDataTupleInfo
        {
            get 
            {
                return (_selection == TreeSelectionType.SingleDataTuple ||
                    _selection == TreeSelectionType.SingleTupleItem ||
                    _selection == TreeSelectionType.MultiDataTuples ||
                    _selection == TreeSelectionType.MultiDataTuplesSingle ||
                    _selection == TreeSelectionType.MultiTupleItems ||
                    _selection == TreeSelectionType.MultiTupleItemsSingle);
            }
        }

        public bool IsSetTupleInfo
        {
            get
            {
                return (
                    _selection == TreeSelectionType.SingleTupleItem ||
                    _selection == TreeSelectionType.MultiTupleItems ||
                    _selection == TreeSelectionType.MultiTupleItemsSingle);
            }
        }

        public bool IsSetIndex
        {
            get { return _idx != AllItems; }
        }

        public SelectedTupleItem(string stream)
        {
            _selection = TreeSelectionType.Stream;
            _stream = stream;
            _tupleInfo = new TupleMetaData();
            _dt = DateTime.Now;
            _idx = 0;
        }

        public SelectedTupleItem(string stream, DateTime dt)
        {
            _selection = TreeSelectionType.SingleDataTuple;
            _stream = stream;
            _tupleInfo = new TupleMetaData();
            _dt = dt;
            _idx = 0;
        }

        public SelectedTupleItem(string stream, DateTime dt, TupleMetaData info)
        {
            _selection = TreeSelectionType.SingleTupleItem;
            _stream = stream;
            _tupleInfo = info;
            _dt = dt;
            _idx = 0;
        }


        public SelectedTupleItem(string stream, DateTime dt, int idx)
        {
            if (AllItems == idx)
                _selection = TreeSelectionType.MultiDataTuples;
            else
                _selection = TreeSelectionType.MultiDataTuplesSingle;
            _stream = stream;
            _tupleInfo = new TupleMetaData();
            _dt = dt;
            _idx = idx;
        }

        public SelectedTupleItem(string stream, DateTime dt, TupleMetaData info, int idx)
        {
            if (AllItems == idx)
                _selection = TreeSelectionType.MultiTupleItems;
            else
                _selection = TreeSelectionType.MultiTupleItemsSingle;

            _stream = stream;
            _tupleInfo = info;
            _dt = dt;
            _idx = idx;
        }
    }

    public class IntermediateEventDataUI
    {
        public EventArgs _args;
        public ActionVisualizerUI _ui;

        public IntermediateEventDataUI(EventArgs args, ActionVisualizerUI ui)
        {
            _args = args;
            _ui = ui;
        }
    }

    public abstract class ActionVisualizerUI
    {
        public string _humaneName;
        public string _name;
        public string _descr;

        public EventHandler _handler;

        public ITupleItemVisualizerUI _tivui;

        public Image _image;
        public object _tag;
    }

    public class ADTVEventArgs : EventArgs
    {
        public ADTVEventArgs(IDataTupleVisualizerUI ui)
        {
            _ui = ui;
        }
        public IDataTupleVisualizerUI _ui;
        public ActionVisualizerUI _action;
    }

    public class ADTVEventItemArgs : ADTVEventArgs
    {
        public ADTVEventItemArgs(IDataTupleVisualizerUI ui, ITupleItem item)
            : base(ui)
        {
            _item = item;
        }
        public ITupleItem _item;
    }

    public class ADTVEventTupleArgs : ADTVEventArgs
    {
        public ADTVEventTupleArgs(IDataTupleVisualizerUI ui, IDataTuple tuple)
            : base(ui)
        {
            _tuple = tuple;
        }
        public IDataTuple _tuple;
    }


    public class ADCVEventArgs : EventArgs
    {
        public ADCVEventArgs(IDataCartogramVisualizerUI ui)
        {
            _ui = ui;
        }
        public IDataCartogramVisualizerUI _ui;
        public ActionVisualizerUI _action;
    }

    public class ADCVEventCoordArgs : ADCVEventArgs
    {
        public ADCVEventCoordArgs(IDataCartogramVisualizerUI ui, Coords c)
            : base(ui)
        {
            Coords = c;
        }
        public readonly Coords Coords;
    }

    public abstract class ActionDataCartogramVisualizerUI : ActionVisualizerUI
    {
        public enum Actions
        {
            CellItemContextMenu,
            LegendContextMenu
        }
        public Actions _action;
    }

    public abstract class ActionDataTupleVisualizerUI : ActionVisualizerUI
    {
        public enum Actions
        {
            TupleItemContextMenu,
            DataTupleContextMenu,
            StreamContextMenu,
            TupleItemCahnged,

            ToolBarButton,
            MainMenuButton
        }

        public enum Flags : int
        {
            Defaults = 0,
            ToolBarCheckButton = 1,
            ToolBarCheckButtonPushed = 2,
        }

        public Actions _action;

        public Flags _flags;

        public virtual void OnRegistered(IDataTupleVisualizerUI ui)
        {
        }
    }

    public interface ITupleItemManipulator
    {        
        ITupleItem GetActiveTupleItem();
        IMultiTupleItem GetActiveTupleItems();
        string GetActiveTupleItemStream();
        void SetActiveTupleItem(ITupleItem item, string stream);
        void SetActiveTupleItems(IMultiTupleItem item, string stream);
    }

    public interface IDataGridManipulator
    {
        DataGrid GetActiveDataGrid();
        void SetDataGrid(DataGrid data);
    }

    public interface IDataTupleVisualizerGetter
    {
        IDataTupleVisualizerUI GetDataTupleVisualizer();
    }

    public interface ITupleItemVisualizerUI : ITupleItemManipulator, IDataTupleVisualizerGetter
    {        
    }

    public interface IDataGridVisualizerUI : IDataGridManipulator, IDataTupleVisualizerGetter
    {
    }


    public interface IDataCartogramVisualizerUI : ITupleItemVisualizerUI
    {
        Coords[] GetSelectedCells();
        void SetSelectedCells(Coords[] cells);
        bool SelectCell(Coords crd, bool select);

        bool RegisterAction(ActionDataCartogramVisualizerUI ac);
    }

    public delegate void OnStreamHandler(IDataTupleVisualizerUI sender, string stream);
    public delegate void OnDataTupleHandler(IDataTupleVisualizerUI sender, IDataTuple dataTuple);
    public delegate void OnTupleItemHandler(IDataTupleVisualizerUI sender, IDataTuple dataTuple, ITupleItem item);
    public delegate void OnDataTupleMultiHandler(IDataTupleVisualizerUI sender, IMultiDataTuple dataTuple);
    public delegate void OnTupleItemMultiHandler(IDataTupleVisualizerUI sender, IMultiDataTuple dataTuple, IMultiTupleItem item);

    public interface IDataTupleVisualizerUI : ITupleItemManipulator, IDataGridManipulator
    {
        void SetTreeFormatter(ITreeItemFormatter formatter);

        IDataTuple GetActiveDataTuple();
        IMultiDataTuple GetActiveDataTuples();
        void SetActiveDataTuple(IDataTuple tuple);
        void SetActiveDataTuples(IMultiDataTuple tuples);

        void SetDataGrid(DataGrid data, ITupleItem item, IDataTuple tuple, string stream);
        void SetDataGrid(DataGrid data, IMultiTupleItem items, IMultiDataTuple tuples, string stream);

        IEnviroment GetEnviroment();
        IMultiDataProvider GetMultiDataProvider();

        SelectedTupleItem[] GetSelectedItems();

        ITupleItemVisualizerUI GetItemVisualizer();

        bool RegisterAction(ActionDataTupleVisualizerUI ac);
        bool EnableAction(ActionDataTupleVisualizerUI ac, bool enable);
        bool VisibleAction(ActionDataTupleVisualizerUI ac, bool visible);

        void SetStatusString(string newString);
        void SetDetailString(string newString);

        event OnStreamHandler OnStream;
        event OnDataTupleHandler OnDataTuple;
        event OnTupleItemHandler OnTupleItem;
        event OnDataTupleMultiHandler OnDataTupleMulti;
        event OnTupleItemMultiHandler OnTupleItemMulti;
    }
}
