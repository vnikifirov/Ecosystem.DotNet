using System;
using System.Reflection;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using corelib;

namespace corelib
{
    public class DataTupleTreeStandardFormatter : ITreeItemFormatter
    {

        #region ITreeItemFormatter Members

        public string GetStreamName(string streamName)
        {
            return streamName;
        }

        public string GetSingleDataTupleName(string streamName, DateTime dateTime)
        {
            return dateTime.ToString();
        }

        public string GetSingleTupleItemName(string streamName, DateTime dateTime, TupleMetaData itemInfo)
        {
            return itemInfo.HumaneName;
        }

        public string GetMultiDataTupleName(string streamName, DateTime dateTime, int idx)
        {
            if (idx == -1)
                return dateTime.ToString();
            else
                return idx.ToString();
        }

        public string GetMultiTupleItemName(string streamName, DateTime dateTime, int idx, TupleMetaData itemInfo)
        {
            if (idx == -1)
                return "[" + itemInfo.HumaneName + "]";
            else
                return itemInfo.HumaneName;
        }

        #endregion
    }

    public class DataTupleVisualizer : UserControl, IDataTupleVisualizerUI
    {
        #region Automatic
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DataTupleVisualizer
            // 
#if !DOTNET_V11
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
#endif
            this.Name = "DataTupleVisualizer";
            this.Size = new System.Drawing.Size(438, 301);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        private TreeView treeTuples;
        private ImageList imgList;
        private ToolBar toolBar1;

#if !DOTNET_V11
        private SplitContainer tableLayoutPanel1;
        private SplitContainer tableLayoutPanel2;
#else
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
#endif
        private TextBox textHelp;
        private TableLayoutPanel tableLayoutPanel3;
        private Label subInfo;

        protected void InitializeView()
        {
#if !DOTNET_V11
            tableLayoutPanel1 = new SplitContainer();
            tableLayoutPanel2 = new SplitContainer();
#else
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
#endif
            textHelp = new TextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            subInfo = new Label();

            tableLayoutPanel1.SuspendLayout();

            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
#if !DOTNET_V11
            tableLayoutPanel1.Panel1.Controls.Add(this.tableLayoutPanel2);
            tableLayoutPanel1.Panel2.Controls.Add(this.tableLayoutPanel3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Orientation = Orientation.Vertical;
#else
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.TabIndex = 0;
#endif
            // 
            // tableLayoutPanel2
            // 
#if !DOTNET_V11
            tableLayoutPanel2.Panel1.Padding = new Padding(3);
            tableLayoutPanel2.Panel2.Padding = new Padding(3);
            tableLayoutPanel2.Panel1.Controls.Add(this.treeTuples);
            tableLayoutPanel2.Panel2.Controls.Add(this.textHelp);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Orientation = Orientation.Horizontal;
            tableLayoutPanel2.SplitterDistance = 200;
#else
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(this.textHelp, 0, 1);
            tableLayoutPanel2.Controls.Add(this.treeTuples, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
            tableLayoutPanel2.TabIndex = 0;
#endif
            // 
            // textHelp
            // 
            this.textHelp.AcceptsReturn = true;
            this.textHelp.Dock = DockStyle.Fill;
            this.textHelp.Multiline = true;
            this.textHelp.ScrollBars = ScrollBars.Vertical;
            this.textHelp.Name = "textHelp";
            this.textHelp.ReadOnly = true;
            this.textHelp.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(subInfo, 0, 1);
            this.tableLayoutPanel3.Dock = DockStyle.Fill;
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // subInfo
            // 
            this.subInfo.BorderStyle = BorderStyle.Fixed3D;
            this.subInfo.Dock = DockStyle.Fill;
            this.subInfo.Name = "subInfo";
            this.subInfo.TabIndex = 0;
            this.subInfo.TextAlign = ContentAlignment.MiddleLeft;

            // restore view
            this.Controls.Add(this.tableLayoutPanel1);

            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        public DataTupleVisualizer(IEnviroment env)
        {
            InitializeComponent();

            //
            imgList = new ImageList();
            imgList.ImageSize = new Size(24, 24);
            imgList.TransparentColor = Color.Magenta;

            //
            toolBar1 = new ToolBar();
            toolBar1.ImageList = imgList;

            //
            treeTuples = new TreeView();
            treeTuples.Dock = System.Windows.Forms.DockStyle.Fill;
            treeTuples.Name = "listTuples";
            treeTuples.HideSelection = false;

            treeTuples.TabIndex = 0;
#if DOTNET_V11
            treeTuples.AfterSelect += new TreeViewEventHandler(treeTuples_AfterSelect);			
			treeTuples.MouseUp += new MouseEventHandler(treeTuples_MouseClick);
#else
            treeTuples.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeTuples_NodeMouseClick);
            treeTuples.MouseClick += new MouseEventHandler(treeTuples_MouseClick);
#endif
            treeTuples.KeyDown += new KeyEventHandler(treeTuples_KeyDown);
            treeTuples.KeyUp += new KeyEventHandler(treeTuples_KeyUp);

            
            _env = env;

            InitializeView();

            _dcv = new DataCartogramVisualizer(this);
            _dptv = new DataParamTableVisualizer(this);
            _dav = new DataArrayVisualizer(this);
            _dgv = new DataGridListVisualizer(this);

            HideUnsuitableActions(null);

            foreach (Type t in Component.GetAllTypes(typeof(ActionDataTupleVisualizerUI),
                "DTVPlugin"))
            {
                ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                if (ci != null)
                    RegisterAction((ActionDataTupleVisualizerUI)ci.Invoke(null));
                else
                {
                    ci = t.GetConstructor(new Type[] { typeof(IEnviroment) });
                    if (ci != null)
                        RegisterAction((ActionDataTupleVisualizerUI)ci.Invoke(new object[] { _env } ));
                }

            }

            // Add the event-handler delegate.
            toolBar1.ButtonClick += new ToolBarButtonClickEventHandler(toolBar1_ButtonClick);

            // Add the ToolBar to the Form.
            Controls.Add(toolBar1);


            _dgv.SetDataGrid(null);
        }

        public IEnviroment GetEnviroment()
        {
            return _env;
        }

        static ITreeItemFormatter _defaultFormatter = new DataTupleTreeStandardFormatter();
        ITreeItemFormatter _treeFormatter = _defaultFormatter;
        private IEnviroment _env;

        private DataCartogramVisualizer _dcv;
        private DataParamTableVisualizer _dptv;
        private DataArrayVisualizer _dav;

        private DataGridListVisualizer _dgv;

        void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            ToolBarButton m = e.Button;

            ActionDataTupleVisualizerUI aui = m.Tag as ActionDataTupleVisualizerUI;
            if (aui != null)
            {
                aui._handler(null, new ADTVEventArgs(this));
            }
        }

#if DOTNET_V11
		Hashtable _buttonNames = new Hashtable();
#endif

        void AddHookForAction(ActionDataTupleVisualizerUI ac)
        {
            if (ac._action == ActionDataTupleVisualizerUI.Actions.ToolBarButton)
            {
                ToolBarButton tb = new ToolBarButton(ac._humaneName);
                tb.ToolTipText = ac._descr;
                tb.Tag = ac;

#if !DOTNET_V11
                tb.Name = ac._name;
                if (ac._image != null)
                {
                    imgList.Images.Add(ac._name, ac._image);
                    tb.ImageKey = ac._name;
                }
#else
                if (ac._image != null)
                {
                    tb.ImageIndex = imgList.Images.Count;
					imgList.Images.Add(ac._image);
                }
#endif
                if (((int)ac._flags & (int)ActionDataTupleVisualizerUI.Flags.ToolBarCheckButton) ==
                    (int)ActionDataTupleVisualizerUI.Flags.ToolBarCheckButton)
                {
                    tb.Style = ToolBarButtonStyle.ToggleButton;

                    tb.Pushed = ((int)ac._flags & (int)ActionDataTupleVisualizerUI.Flags.ToolBarCheckButtonPushed) ==
                        (int)ActionDataTupleVisualizerUI.Flags.ToolBarCheckButtonPushed;
                }

                int i = toolBar1.Buttons.Add(tb);
#if DOTNET_V11
				_buttonNames.Add(ac._name, i);
#endif
            }
        }

        #region treeTuples Mouse and keyboard

        bool _mouseRight = false;
        MouseEventArgs _oldArgs;

        void treeTuples_MouseClick(object sender, MouseEventArgs e)
        {
            _mouseRight = (e.Button == MouseButtons.Right);
            if (_mouseRight == true)
            {
                _mouseRight = false;
                TreeNode nd = treeTuples.SelectedNode;
                treeTuples.SelectedNode = null;
                treeTuples.SelectedNode = nd;
                _mouseRight = true;
                MouseClickToNode(sender, e, nd);
                _mouseRight = false;
            }
            _oldArgs = e;
        }

#if DOTNET_V11
		Hashtable _menuTags = new Hashtable();
#endif

        void ProxyUIMenuEvent(object sender, EventArgs e)
        {
#if DOTNET_V11
            MenuItem mi = (MenuItem)sender;
			IntermediateEventDataUI iedui = (IntermediateEventDataUI)_menuTags[mi];
#else
            ToolStripMenuItem mi = (ToolStripMenuItem)sender;
            IntermediateEventDataUI iedui = (IntermediateEventDataUI)mi.Tag;
#endif

            iedui._ui._handler(null, iedui._args);
        }

        void MouseClickToNode(object sender, MouseEventArgs e, TreeNode node)
        {
            if (_mouseRight)
            {
#if DOTNET_V11
				_menuTags.Clear();

                ArrayList list = new ArrayList();
#else
                ContextMenuStrip stripMenu = new ContextMenuStrip();
#endif

                if (node.Tag.GetType() == typeof(SelectedTupleItem))
                {
                    SelectedTupleItem ta = (SelectedTupleItem)node.Tag;

                    if ((ta.Selection == TreeSelectionType.SingleTupleItem) ||
                            (ta.Selection == TreeSelectionType.MultiDataTuplesSingle))
                    {
                        ITupleItem curItem;
                        curItem = _dataSource.GetData(ta.DateTime, ta.Stream)[ta.Index][ta.TupleInfo.Name];

                        foreach (ActionDataTupleVisualizerUI obj in _actions.Values)
                        {
                            if (obj._action == ActionDataTupleVisualizerUI.Actions.TupleItemContextMenu)
                            {
#if DOTNET_V11
                                MenuItem mi = new MenuItem(obj._humaneName, new EventHandler(ProxyUIMenuEvent));
                                _menuTags[mi] = new IntermediateEventDataUI(new ADTVEventItemArgs(this, curItem), obj);
                                list.Add(mi);
#else
                                ToolStripMenuItem mi = new ToolStripMenuItem(obj._humaneName, null, new EventHandler(ProxyUIMenuEvent));
                                mi.Tag = new IntermediateEventDataUI(new ADTVEventItemArgs(this, curItem), obj);
                                mi.Image = obj._image;
                                stripMenu.Items.Add(mi);
#endif
                            }
                        }
                    }
                    else if ((ta.Selection == TreeSelectionType.SingleDataTuple) ||
                        (ta.Selection == TreeSelectionType.MultiDataTuplesSingle))
                    {
                        IDataTuple tuple = _dataSource.GetData(ta.DateTime, ta.Stream)[ta.Index];

                        foreach (ActionDataTupleVisualizerUI obj in _actions.Values)
                        {
                            if (obj._action == ActionDataTupleVisualizerUI.Actions.DataTupleContextMenu)
                            {
#if DOTNET_V11
                                MenuItem mi = new MenuItem(obj._humaneName, new EventHandler(ProxyUIMenuEvent));
                                _menuTags[mi] = new IntermediateEventDataUI(new ADTVEventTupleArgs(this, tuple), obj);
                                list.Add(mi);
#else
                                ToolStripMenuItem mi = new ToolStripMenuItem(obj._humaneName, null, new EventHandler(ProxyUIMenuEvent));
                                mi.Tag = new IntermediateEventDataUI(new ADTVEventTupleArgs(this, tuple), obj);
                                mi.Image = obj._image;
                                stripMenu.Items.Add(mi);
#endif
                            }
                        }
                    }
                }

#if DOTNET_V11
                if (list.Count > 0)
                {
                    MenuItem[] menus = (MenuItem[])list.ToArray(typeof(MenuItem));                    

                    ContextMenu buttonMenu = new ContextMenu(menus);                    
                    buttonMenu.Show((Control)sender, new Point(e.X, e.Y));
                }
#else
                if (stripMenu.Items.Count > 0)
                    stripMenu.Show((Control)sender, new Point(e.X, e.Y));
#endif
            }
            else
                if ((node != null) && (_keyControl == false) && (_keyShift == false)) // && (e == MouseButtons.Left))
                {
                    //currectTupleOrig = null;
                    treeTuplesNavigate(node);
                }
                else
                {
                    try
                    {
                        bool ok = false;
                        ITupleItem prev = _currentTupleItem;
                        ITupleItem next = null;

                        if (node.Tag.GetType() == typeof(SelectedTupleItem))
                        {
                            SelectedTupleItem ta = (SelectedTupleItem)node.Tag;

                            if ((!ta.IsMulti) && (ta.IsSetTupleInfo))
                            {
                                IDataTuple dataTuple = _dataSource.GetData(ta.DateTime, ta.Stream)[ta.Index];
                                next = dataTuple[ta.TupleInfo.Name];
                            }
                        }

                        if (prev != null && next != null)
                        {
                            IDataCartogram old = prev as IDataCartogram;
                            IDataCartogram n = next as IDataCartogram;

                            if (old != null && n != null && old.Layers == n.Layers)
                            {
                                IDataCartogram diff = null;
                                if (_keyControl)
                                    diff = n.AbsoluteDiff(old);
                                else if (_keyShift)
                                    diff = n.RelativeDiff(old);

                                if (diff != null)
                                    diff = (IDataCartogram)diff.Clone(diff.GetName(), diff.GetHumanName(), n.GetTimeDate());

                                SetCartogrammDiff(diff, n, old, _currentTupleItemStream);
                                ok = true;

                                //currectTupleOrig = _currentTupleItem;
                                //_currentTupleItem = diff;
                                

                                _currentTupleItem = next;
                            }
                        }

                        if (!ok)
                        {
                            treeTuplesNavigate(node);
                            
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
        }

        void treeTuples_AfterSelect(object sender, TreeViewEventArgs e)
        {
            MouseClickToNode(sender, _oldArgs, e.Node);
        }
#if !DOTNET_V11
        void treeTuples_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            MouseClickToNode(sender, e, e.Node);
        }
#endif


        bool _keyControl = false;
        bool _keyShift = false;
        bool _keyAlt = false;

        void treeTuples_KeyUp(object sender, KeyEventArgs e)
        {
            _keyControl = e.Control;
            _keyShift = e.Shift;
            _keyAlt = e.Alt;

            if ((_keyControl) || (_keyShift))
                Cursor = Cursors.Cross;
            else
                Cursor = Cursors.Arrow;
        }
        
        void treeTuples_KeyDown(object sender, KeyEventArgs e)
        {
            _keyControl = e.Control;
            _keyShift = e.Shift;
            _keyAlt = e.Alt;

            if ((_keyControl) || (_keyShift))
                Cursor = Cursors.Cross;
            else
                Cursor = Cursors.Arrow;
        }

        #endregion


        #region TupleItem Navigation


        void treeTuplesNavigate(TreeNode node)
        {
            _currentDataGrid = null;
            _currentTupleItem = null;
            _currentTupleItems = null;
            _currentDataTuple = null;
            _currentDataTuples = null;

            switch (_selectedType)
            {
                case TreeSelectionType.Stream:
                    if (OnStream != null)
                        OnStream(this, null);
                    break;
                case TreeSelectionType.MultiDataTuplesSingle:
                case TreeSelectionType.SingleDataTuple:
                    if (OnDataTuple != null)
                        OnDataTuple(this, null);
                    break;
                case TreeSelectionType.MultiTupleItemsSingle:
                case TreeSelectionType.SingleTupleItem:
                    if (OnTupleItem != null)
                        OnTupleItem(this, null, null);
                    break;
                case TreeSelectionType.MultiDataTuples:
                    if (OnDataTupleMulti != null)
                        OnDataTupleMulti(this, null);
                    break;
                case TreeSelectionType.MultiTupleItems:
                    if (OnTupleItemMulti != null)
                        OnTupleItemMulti(this, null, null);
                    break;
            }
            _selectedType = TreeSelectionType.None;

            if (node.Tag.GetType() == typeof(SelectedTupleItem))
            {
                SelectedTupleItem ta = (SelectedTupleItem)node.Tag;
                _selectedType = ta.Selection;

                if (ta.IsMulti)
                {
                    if (ta.IsSetTupleInfo)
                    {
                        IMultiDataTuple tuples = _dataSource.GetData(ta.DateTime, ta.Stream);
                        IMultiTupleItem items = tuples[ta.TupleInfo.Name];

                        SetActiveTupleItems(items, tuples, ta.Stream);

                        if (OnTupleItemMulti != null)
                            OnTupleItemMulti(this, tuples, items);
                    }
                    else if (ta.IsSetDataTupleInfo)
                    {
                        IMultiDataTuple tuples = _dataSource.GetData(ta.DateTime, ta.Stream);

                        SetActiveUI(null);

                        if (OnDataTupleMulti != null)
                            OnDataTupleMulti(this, tuples);
                    }
                    else if (ta.IsSetStreamInfo)
                    {
                        SetActiveUI(null);
                        if (OnStream != null)
                            OnStream(this, ta.Stream);
                    }
                    else
                    {
                        SetActiveUI(null);
                    }
                }
                else
                {
                    if (ta.IsSetTupleInfo)
                    {
                        IDataTuple dataTuple = _dataSource.GetData(ta.DateTime, ta.Stream)[ta.Index];
                        ITupleItem curItem = dataTuple[ta.TupleInfo.Name];

                        SetActiveTupleItem(curItem, dataTuple, ta.Stream);

                        if (OnTupleItem != null)
                            OnTupleItem(this, dataTuple, curItem);
                    }
                    else if (ta.IsSetDataTupleInfo)
                    {
                        IDataTuple dataTuple = _dataSource.GetData(ta.DateTime, ta.Stream)[ta.Index];
 
                        SetActiveUI(null);

                        if (OnDataTuple != null)
                            OnDataTuple(this, dataTuple);
                    }
                    else if (ta.IsSetStreamInfo)
                    {
                        SetActiveUI(null);
                        if (OnStream != null)
                            OnStream(this, ta.Stream);
                    }
                    else
                    {
                        SetActiveUI(null);
                    }
                }
            }
            else
            {
                SetActiveUI(null);
            }
        }

        #endregion

        TreeSelectionType _selectedType = TreeSelectionType.None;

        DataGrid _currentDataGrid;
        IDataTuple _currentDataTuple;
        IMultiDataTuple _currentDataTuples;
        ITupleItem _currentTupleItem;
        IMultiTupleItem _currentTupleItems;
        string _currentTupleItemStream;

        IMultiDataProvider _dataSource;

        static readonly SelectedTupleItem[] _noneItems = { };


        Hashtable _actions = new Hashtable();

        bool _multiStream;

        #region Data Set and Tree updating

        public void UpdateTupleList()
        {
            treeTuples.Nodes.Clear();

            //int sidx = 0;
            bool addStream = _dataSource.GetStreamNames().Length > 1;

            _multiStream = addStream;

            foreach (string stream in _dataSource.GetStreamNames())
            {
                bool processStream = true;
                TreeNode streamParent = null;
                if (addStream)
                {
                    string streamName = _treeFormatter.GetStreamName(stream);
                    if (streamName != null)
                    {
                        streamParent = treeTuples.Nodes.Add(streamName);
                        streamParent.Tag = new SelectedTupleItem(stream);
                    }
                    else
                    {
                        processStream = false;
                    }
                }

                if (processStream)
                {
                    TreeNodeCollection sp;
                    if (addStream)
                        sp = streamParent.Nodes;
                    else
                        sp = treeTuples.Nodes;

                    string[] snames = _dataSource.GetAllDataNames(stream);
                    DateTime[] dates = _dataSource.GetDates(stream);
                    /////*********Роман*********/////
                    /*
                    //сортировка dates
                    bool t = true;
                    while (t == true)
                    {
                        t = false;
                        for (int i = 0; i < dates.Length - 1; i++)
                        {
                            if (dates[i] > dates[i + 1])
                            {
                                DateTime temp_date = dates[i];
                                dates[i] = dates[i + 1];
                                dates[i + 1] = temp_date;
                                t = true;
                            }
                        }
                    }
                    */
                    /////*********Роман*********/////
                    TupleMetaData[,] dts = null;
                    if (snames != null)
                        dts = _dataSource.GetTupleItemInfo(dates ,snames);

                    int[] counts = null;
                    if (dts != null)
                        _dataSource.GetMultiTuplesCount(dates, snames[0], out counts);

                    //int didx = 0;
                    //foreach (DateTime dt in dates)
                    for (int k = 0; k < dates.Length;k++ )
                    {
                        DateTime dt = dates[k];
                        TreeNode dtParent;

                        //if (!_dataSource.IsStreamMulti(stream))
                        string[] itemNames = null;
                        if (dts == null)
                            itemNames = _dataSource.GetDataNames(dt, stream);
                        else
                            itemNames = snames;

                        int count;
                        count = (counts != null) ? counts[k] : -1;

                        if (count == -1)
                            count = _dataSource.GetMultiTuplesCount(dt, itemNames[0]);

                        if (count <= 1)
                        {
                            string dataTupleName = _treeFormatter.GetSingleDataTupleName(stream, dt);
                            if (dataTupleName != null)
                            {
                                dtParent = sp.Add(dataTupleName);

                                //string[] itemNames = _dataSource.GetDataNames(dt, stream);
                                dtParent.Tag = new SelectedTupleItem(stream, dt);

                                for (int l = 0; l < itemNames.Length; l++)
                                {
                                    TupleMetaData itemInfo;
                                    if (dts != null)
                                        itemInfo = dts[k, l];
                                    else
                                        itemInfo = _dataSource.GetTupleItemInfo(dt, itemNames[l]);

                                    if ((itemInfo.Name == null) || (itemInfo.Name.Length == 0))
                                        continue;

                                    string tupleItemName = _treeFormatter.GetSingleTupleItemName(stream, dt, itemInfo);
                                    if (tupleItemName != null)
                                    {
                                        TreeNode item = dtParent.Nodes.Add(tupleItemName);
                                        item.Tag = new SelectedTupleItem(stream, dt, itemInfo);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string multiDataTupleName = _treeFormatter.GetMultiDataTupleName(stream, dt, -1);
                            if (multiDataTupleName != null)
                            {
                                dtParent = sp.Add(multiDataTupleName);

                                //string[] itemNames = _dataSource.GetDataNames(dt, stream);
                                dtParent.Tag = new SelectedTupleItem(stream, dt, SelectedTupleItem.AllItems);

                                for (int l = 0; l < itemNames.Length; l++)
                                {
                                    TupleMetaData itemInfo;
                                    if (dts != null)
                                        itemInfo = dts[k, l];
                                    else
                                        itemInfo = _dataSource.GetTupleItemInfo(dt, itemNames[l]);

                                    if ((itemInfo.Name == null) || (itemInfo.Name.Length == 0))
                                        continue;

                                    string multiTupleItemName = _treeFormatter.GetMultiTupleItemName(stream, dt, -1, itemInfo);
                                    if (multiTupleItemName != null)
                                    {
                                        TreeNode item = dtParent.Nodes.Add(multiTupleItemName);
                                        item.Tag = new SelectedTupleItem(stream, dt, itemInfo, SelectedTupleItem.AllItems);
                                    }
                                }

                                //int count = _dataSource.GetMultiTuplesCount(dt, itemNames[0]);
                                for (int idx = 0; idx < count; idx++)
                                {
                                    string dataTupleName = _treeFormatter.GetMultiDataTupleName(stream, dt, idx);
                                    if (dataTupleName != null)
                                    {
                                        TreeNode item = dtParent.Nodes.Add(dataTupleName);
                                        item.Tag = new SelectedTupleItem(stream, dt, idx);

                                        for (int l = 0; l < itemNames.Length; l++)
                                        {
                                            //TupleMetaData itemInfo = _dataSource.GetTupleItemInfo(dt, name);
                                            TupleMetaData itemInfo;
                                            if (dts != null)
                                                itemInfo = dts[k, l];
                                            else
                                                itemInfo = _dataSource.GetTupleItemInfo(dt, itemNames[l]);

                                            if ((itemInfo.Name == null) || (itemInfo.Name.Length == 0))
                                                continue;

                                            string tupleItemName = _treeFormatter.GetMultiTupleItemName(stream, dt, idx, itemInfo);
                                            if (tupleItemName != null)
                                            {
                                                TreeNode itemIt = item.Nodes.Add(tupleItemName);
                                                itemIt.Tag = new SelectedTupleItem(stream, dt, itemInfo, idx);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //didx++;
                    }
                }
                //sidx++;
            }
            

        }
        //***********************************//
        //*****************Роман******************//
        public void UpdateRenewTuple(string srez)
        {
            int select_item = -1;
            try
            {
                int[] sel = new int[treeTuples.Nodes[0].Nodes.Count];
                for (int k = 0; k < treeTuples.Nodes[0].Nodes.Count; k++)
                {
                    if (treeTuples.Nodes[0].Nodes[k].IsSelected == true)
                    {
                        sel[k] = 1;
                        select_item = k;
                    }
                }
            }
            catch
            {
            }
            treeTuples.Nodes.Clear();

            //int sidx = 0;
            bool addStream = _dataSource.GetStreamNames().Length > 1;

            _multiStream = addStream;

            foreach (string stream in _dataSource.GetStreamNames())
            {
                bool processStream = true;
                TreeNode streamParent = null;
                if (addStream)
                {
                    string streamName = _treeFormatter.GetStreamName(stream);
                    if (streamName != null)
                    {
                        streamParent = treeTuples.Nodes.Add(streamName);
                        streamParent.Tag = new SelectedTupleItem(stream);
                    }
                    else
                    {
                        processStream = false;
                    }
                }

                if (processStream)
                {
                    TreeNodeCollection sp;
                    if (addStream)
                        sp = streamParent.Nodes;
                    else
                        sp = treeTuples.Nodes;

                    string[] snames = _dataSource.GetAllDataNames(stream);
                    DateTime[] dates = _dataSource.GetDates(stream);
                    /////*********Роман*********/////
                    /*
                    //сортировка dates
                    bool t = true;
                    while (t == true)
                    {
                        t = false;
                        for (int i = 0; i < dates.Length - 1; i++)
                        {
                            if (dates[i] > dates[i + 1])
                            {
                                DateTime temp_date = dates[i];
                                dates[i] = dates[i + 1];
                                dates[i + 1] = temp_date;
                                t = true;
                            }
                        }
                    }
                    */
                    /////*********Роман*********/////
                    TupleMetaData[,] dts = null;
                    if (snames != null)
                        dts = _dataSource.GetTupleItemInfo(dates, snames);

                    int[] counts = null;
                    if (dts != null)
                        _dataSource.GetMultiTuplesCount(dates, snames[0], out counts);

                    int k = 0;
                    if (srez == "prev")
                    {
                        k = dates.Length - 2;
                        if (k < 0)
                            k = dates.Length - 1;
                    }
                    else
                    {
                        k = dates.Length - 1;
                    }
                    DateTime dt = dates[k];
                    TreeNode dtParent;

                    //if (!_dataSource.IsStreamMulti(stream))
                    string[] itemNames = null;
                    if (dts == null)
                        itemNames = _dataSource.GetDataNames(dt, stream);
                    else
                        itemNames = snames;

                    int count;
                    count = (counts != null) ? counts[k] : -1;

                    if (count == -1)
                        count = _dataSource.GetMultiTuplesCount(dt, itemNames[0]);

                    if (count <= 1)
                    {
                        string dataTupleName = _treeFormatter.GetSingleDataTupleName(stream, dt);
                        if (dataTupleName != null)
                        {
                            dtParent = sp.Add(dataTupleName);

                            //string[] itemNames = _dataSource.GetDataNames(dt, stream);
                            dtParent.Tag = new SelectedTupleItem(stream, dt);

                            for (int l = 0; l < itemNames.Length; l++)
                            {
                                TupleMetaData itemInfo;
                                if (dts != null)
                                    itemInfo = dts[k, l];
                                else
                                    itemInfo = _dataSource.GetTupleItemInfo(dt, itemNames[l]);

                                if ((itemInfo.Name == null) || (itemInfo.Name.Length == 0))
                                    continue;

                                string tupleItemName = _treeFormatter.GetSingleTupleItemName(stream, dt, itemInfo);
                                if (tupleItemName != null)
                                {
                                    TreeNode item = dtParent.Nodes.Add(tupleItemName);
                                    item.Tag = new SelectedTupleItem(stream, dt, itemInfo);
                                }
                            }
                        }
                    }

                    //didx++;
                }
                //sidx++;
            }
            //***********************************//
            /*
            TreeView tv = new TreeView();
            tv.Nodes.Clear();
            
            tv.Nodes.Add(treeTuples.Nodes[treeTuples.Nodes.Count - 1]);
                //treeTuples.Nodes[treeTuples.Nodes.Count - 1];
            treeTuples.Nodes.Clear();
            treeTuples.Nodes.Add(tv.Nodes[0]);
            */
            //***********************************//
            if (select_item != -1)
            {
                treeTuplesNavigate(treeTuples.Nodes[0].Nodes[select_item]);
                treeTuples.SelectedNode = treeTuples.Nodes[0].Nodes[select_item];
            }
            else
            {
                treeTuples.Nodes[0].Nodes[0].EnsureVisible();
            }

        }

        public void SetRenewTupleProvider(IMultiDataProvider dataProvider, string srez)
        {
            _currentDataGrid = null;
            _currentTupleItem = null;
            _currentTupleItems = null;
            _currentDataTuple = null;
            _currentDataTuples = null;

            _currentTupleItemStream = null;

            if (_dataSource != null)
                _dataSource.OnPushedData -= new MultiDataProviderDataEventHandler(_dataSource_OnPushedData);

            _dataSource = dataProvider;

            _dataSource.OnPushedData += new MultiDataProviderDataEventHandler(_dataSource_OnPushedData);

            UpdateRenewTuple(srez);
        }
        //***********************************//
        //***********************************//

        public void SetDataProvider(IMultiDataProvider dataProvider)
        {
            _currentDataGrid = null;
            _currentTupleItem = null;
            _currentTupleItems = null;
            _currentDataTuple = null;
            _currentDataTuples = null;

            _currentTupleItemStream = null;

            if (_dataSource != null)
                _dataSource.OnPushedData -= new MultiDataProviderDataEventHandler(_dataSource_OnPushedData);

            _dataSource = dataProvider;

            _dataSource.OnPushedData += new MultiDataProviderDataEventHandler(_dataSource_OnPushedData);

            UpdateTupleList();
        }

        void _dataSource_OnPushedData(IMultiDataProvider sender, MultiDataProviderDataTupleEventArgs args)
        {
            UpdateTupleList();
        }

        public void SetMultiTuple(IMultiDataTuple[] tuples)
        {
            //SetDataProvider(new ArrayMultiDataProvider(tuples));

            SetDataProvider(new ListMultiDataProvider(tuples));
        }

        public void SetTuple(IMultiDataTuple tuple)
        {
            SetDataProvider(new ListMultiDataProvider(new IMultiDataTuple[] { tuple }));
        }

        #endregion

        #region View details (general DataCartogram / DataArray widgets io)
        private void SetActiveUI(Control c)
        {
            if (c == null)
            {
                Control ct = tableLayoutPanel3.GetControlFromPosition(0, 0);
                if (ct != null)
                    tableLayoutPanel3.Controls.Remove(ct);
            }
            else
            {
                Control ct = tableLayoutPanel3.GetControlFromPosition(0, 0);
                if (ct != null)
                    tableLayoutPanel3.Controls.Remove(ct);

                c.Dock = DockStyle.Fill;
                tableLayoutPanel3.Controls.Add(c, 0, 0);
            }
        }

        private Control GetActiveUI()
        {
            return tableLayoutPanel3.GetControlFromPosition(0, 0);
        }

        void SetCartogrammDiff(IDataCartogram c, IDataCartogram orig, IDataCartogram pre, string stream)
        {
            _dcv.SetActiveTupleItem(c, stream);
            SetActiveUI(_dcv);

            _dcv.SetDifferentialView(orig, pre);
        }

        void HideUnsuitableActions(ITupleItemVisualizerUI forItem)
        {
            foreach (ActionDataTupleVisualizerUI obj in _actions.Values)
            {
                if (obj._action == ActionDataTupleVisualizerUI.Actions.ToolBarButton)
                {
                    bool touch = (obj._tivui != null);
                    bool visible = (obj._tivui == forItem);

                    if (touch)
                        VisibleAction(obj, visible);
                }
            }
        }

        internal void SetActiveTupleItems(IMultiTupleItem items, IMultiDataTuple tuples, string stream)
        {
            _currentDataGrid = null;
            _currentTupleItem = null;
            _currentTupleItems = null;
            _currentDataTuple = null;
            _currentDataTuples = null;
            _currentTupleItemStream = null;


            bool isDataArray = true;

            //foreach (ITupleItem i in items)
            for (int j = 0; j < items.Count; j++ )
            {
                ITupleItem i = items[j];
                if (i as IDataArray == null)
                    isDataArray = false;
            }

            if (isDataArray)
            {
                _currentDataTuples = tuples;
                _currentTupleItems = items;
                _currentTupleItemStream = stream;

                _dav.SetActiveTupleItems(items, stream);

                HideUnsuitableActions(_dav);
            }

        }

        ITupleItemVisualizerUI GetVisualizer(ITupleItem item)
        {
            ITupleItemVisualizerUI itemviz = null;

            IDataCartogram c = item as IDataCartogram;
            IDataParamTable ptable = item as IDataParamTable;
            IDataArray arr = item as IDataArray;
            if (c != null)
            {
                itemviz = _dcv;
            }
            else if (ptable != null)
            {
                itemviz = _dptv;
            }
            else if (arr != null)
            {
                itemviz = _dav;
            }

            return itemviz;
        }

        internal void SetActiveTupleItem(ITupleItem item, IDataTuple tuple, string stream)
        {
            _currentDataGrid = null;
            _currentTupleItem = null;
            _currentTupleItems = null;
            _currentDataTuple = null;
            _currentDataTuples = null;
            _currentTupleItemStream = null;

            ITupleItemVisualizerUI itemviz = GetVisualizer(item);

            _dgv.SetDataGrid(null);


            if (itemviz as Control != null)
                SetActiveUI((Control)itemviz);

            if (itemviz != null)
            {
                _currentDataTuple = tuple;
                _currentTupleItemStream = stream;
                _currentTupleItem = item;

                itemviz.SetActiveTupleItem(item, stream);
            }
            else
                SetActiveUI(null);

            HideUnsuitableActions(itemviz);
        }

        #endregion


        #region IDataTupleVisualizerUI Members

        public IMultiDataProvider GetMultiDataProvider()
        {
            return _dataSource;
        }

        public SelectedTupleItem[] GetSelectedItems()
        {
            return _noneItems;
        }

        public ITupleItem GetActiveTupleItem()
        {
            return _currentTupleItem;
        }

        public IMultiTupleItem GetActiveTupleItems()
        {
            return _currentTupleItems;
        }

        public void SetActiveTupleItems(IMultiTupleItem items, string stream)
        {
            SetActiveTupleItems(items, null, stream);
        }

        public void SetActiveTupleItem(ITupleItem item, string stream)
        {
            SetActiveTupleItem(item, null, stream);
        }

        public void SetActiveTupleItem(ITupleItem item)
        {
            SetActiveTupleItem(item, null);
        }

        public ITupleItemVisualizerUI GetItemVisualizer()
        {
            return GetActiveUI() as ITupleItemVisualizerUI;
        }

        public bool RegisterAction(ActionDataTupleVisualizerUI ac)
        {
            _actions.Add(ac._name, ac);
            AddHookForAction(ac);
            ac.OnRegistered(this);
            return true;
        }

        public void SetStatusString(string newString)
        {
            subInfo.Text = newString;
            Application.DoEvents();
        }

        public void SetDetailString(string newString)
        {
            textHelp.Text = newString;
        }

        public bool EnableAction(ActionDataTupleVisualizerUI ac, bool enable)
        {
            switch (ac._action)
            {
                case ActionDataTupleVisualizerUI.Actions.ToolBarButton:
#if !DOTNET_V11
                    toolBar1.Buttons[ac._name].Enabled = enable;
#else
					toolBar1.Buttons[(int)_buttonNames[ac._name]].Enabled = enable;
#endif
                    return true;
            }

            return false;
        }

        public bool VisibleAction(ActionDataTupleVisualizerUI ac, bool visible)
        {
            switch (ac._action)
            {
                case ActionDataTupleVisualizerUI.Actions.ToolBarButton:
#if !DOTNET_V11
                    toolBar1.Buttons[ac._name].Visible = visible;
#else
					toolBar1.Buttons[(int)_buttonNames[ac._name]].Visible = visible;
#endif
                    return true;
            }


            return false;
        }

        public string GetActiveTupleItemStream()
        {
            return _currentTupleItemStream;
        }

        #endregion

        #region IDataGridManipulator Members

        public DataGrid GetActiveDataGrid()
        {
            return _currentDataGrid;
        }

        public void SetDataGrid(DataGrid data)
        {
            SetDataGrid(data, (ITupleItem)null, null, null);
        }

        #endregion


        public void SetDataGrid(DataGrid data, ITupleItem item, IDataTuple tuple, string stream)
        {
            _currentDataGrid = data;
            _currentTupleItem = item;
            _currentTupleItems = null;
            _currentDataTuple = tuple;
            _currentDataTuples = null;
            _currentTupleItemStream = stream;

            _dgv.SetDataGrid(data);
            HideUnsuitableActions(GetVisualizer(item));

            SetActiveUI((Control)_dgv);
        }

        public void SetDataGrid(DataGrid data, IMultiTupleItem items, IMultiDataTuple tuples, string stream)
        {
            _currentDataGrid = data;
            _currentTupleItem = null;
            _currentTupleItems = items;
            _currentDataTuple = null;
            _currentDataTuples = tuples;
            _currentTupleItemStream = stream;

            _dgv.SetDataGrid(data);
            HideUnsuitableActions(GetVisualizer(items[0]));

            SetActiveUI((Control)_dgv);
        }


        #region IDataTupleVisualizerUI Members

        public IDataTuple GetActiveDataTuple()
        {
            return _currentDataTuple;
        }

        public IMultiDataTuple GetActiveDataTuples()
        {
            return _currentDataTuples;
        }

        public void SetActiveDataTuple(IDataTuple tuple)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetActiveDataTuples(IMultiDataTuple tuples)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDataTupleVisualizerUI Members


        public event OnStreamHandler OnStream;

        public event OnDataTupleHandler OnDataTuple;

        public event OnTupleItemHandler OnTupleItem;

        public event OnDataTupleMultiHandler OnDataTupleMulti;

        public event OnTupleItemMultiHandler OnTupleItemMulti;

        #endregion

        #region IDataTupleVisualizerUI Members

        public void SetTreeFormatter(ITreeItemFormatter formatter)
        {
            _treeFormatter = formatter;
            UpdateTupleList();
        }

        #endregion
    }








}
