using System;
using System.Reflection;
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
    public class DataCartogramVisualizer : CartView, IDataCartogramVisualizerUI
    {
        enum CartogramView
        {
            Graphical,
            List,
            PvkList
        };

        DataTupleVisualizer _ui;

        bool _isCart = true;
        bool _showHystogramm = true;

        string _stream;
        IDataCartogram _item;
        IDataCartogram _itemDifferential;
        IDataCartogram _itemDifferentialPre;

        IDataTuple _fullTuple;
        ITupleItem[] _arranged;

        ActionViewChange _actionViewChange;
        ActionHystShowChange _actionHystShowChange;

        Hashtable _actions = new Hashtable();

        #region Actions

        class ActionViewChange : ActionDataTupleVisualizerUI
        {
            DataCartogramVisualizer _dcv;

            public ActionViewChange(DataCartogramVisualizer dcv)
            {
                _name = "ActionViewChange";
                _humaneName = "Список";
                _descr = "Отображать в виде списка или картограммы";
                _tivui = dcv;

                _action = Actions.ToolBarButton;
                _flags = Flags.ToolBarCheckButton;
                if (!dcv._isCart)
                    _flags |= Flags.ToolBarCheckButtonPushed;

                _handler = new EventHandler(onClick);

                _image = Resource1.ImageList;

                _dcv = dcv;
            }

            void onClick(object sender, EventArgs e)
            {
                _dcv.SetView(!_dcv._isCart, true);
            }
        }

        class ActionHystShowChange : ActionDataTupleVisualizerUI
        {
            DataCartogramVisualizer _dcv;

            public ActionHystShowChange(DataCartogramVisualizer dcv)
            {
                _name = "ActionHystShowChange";
                _humaneName = "Гистограмма";
                _descr = "Отображать гистограмму";
                _tivui = dcv;

                _action = Actions.ToolBarButton;
                _flags = Flags.ToolBarCheckButton;
                if (dcv._showHystogramm)
                    _flags |= Flags.ToolBarCheckButtonPushed;

                _handler = new EventHandler(onClick);

                _image = Resource1.ImageHyst;

                _dcv = dcv;
            }

            void onClick(object sender, EventArgs e)
            {
                _dcv.Navigation.ShowLegend = !_dcv._showHystogramm;

                _dcv._showHystogramm = !_dcv._showHystogramm;
            }
        }

        #endregion



        static void AddColumn(ListView l, string s, int width)
        {
            ColumnHeader header = new ColumnHeader();
            header.Text = s;
            header.Width = width;

            l.Columns.Add(header);
        }

        public DataCartogramVisualizer(DataTupleVisualizer ui)
        {
            this.Navigation.LeftClickCoord += new CoordsEventDelegate(_nav_LeftClickCoord);
            this.Navigation.MovedToCoord += new CoordsEventDelegate(_nav_MovedToCoord);
            this.Navigation.RightClickCoord += new CoordsEventDelegate(_nav_RightClickCoord);
            this.Navigation.EnableSelection = true;
            this.Dock = DockStyle.Fill;

            _ui = ui;

            // Add actions
            ui.RegisterAction(_actionViewChange = new ActionViewChange(this));
            ui.RegisterAction(_actionHystShowChange = new ActionHystShowChange(this));


            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(ActionDataCartogramVisualizerUI).IsAssignableFrom(t) &&
                    t.Name.StartsWith("DCVPlugin"))
                {
                    ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
                    if (ci != null)
                        RegisterAction((ActionDataCartogramVisualizerUI)ci.Invoke(null));
                }
            }
        }


        void AddHookForAction(ActionDataCartogramVisualizerUI ac)
        {
        }

        void SetView(bool cartView, bool update)
        {
            if ((_isCart != cartView) || update)
            {
                _isCart = cartView;

                if (_isCart)
                {
                    _ui.SetActiveTupleItem(_item, _fullTuple, _stream);

                    _ui.EnableAction(_actionHystShowChange, true);
                }
                else
                {
                    _ui.EnableAction(_actionHystShowChange, false);

                    SetCartogrammListView(false);
                }
            }
        }


        void SetCartogrammListView(bool viewByPvk)
        {
            if (_item == null)
                return;

            _ui.SetDataGrid(_item.CreateDataGrid(_ui.GetEnviroment()), _item, _ui.GetActiveDataTuple(), _stream);
        }

        void _nav_MovedToCoord(object sender, CoodrsEventArgs e)
        {
            if (_item != null)
            {
                if (e.Coords.IsOk)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(ComposeCellName(e.Coords, true));
                    sb.Append("; ");

                    RenderParavName((IDataCartogram)_item, e.Coords, sb);
                    _ui.SetStatusString(sb.ToString());
                }
                else
                    _ui.SetStatusString("");
            }
        }

        void _nav_LeftClickCoord(object sender, CoodrsEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (_itemDifferential == null)
            {
                sb.AppendFormat("{0}\r\n", _item.GetTimeDate());
                sb.Append(ComposeCellName(e.Coords, false));

                foreach (ITupleItem i in _arranged)
                {
                    if (i is IDataCartogram)
                    {
                        RenderParavName((IDataCartogram)i, e.Coords, sb);
                    }
                }
            }
            else
            {
                try
                {
                    if (_itemDifferential != null)
                    {
                        // Разобраться с названием потока!!!
                        IDataTuple t = _ui.GetMultiDataProvider().
                            GetData(_itemDifferential.GetTimeDate(), _fullTuple.GetStreamName())[0];

                        sb.AppendFormat("{0}\r\n", t.GetTimeDate());
                        sb.Append(ComposeCellName(e.Coords, false));

                        foreach (ITupleItem i in t)
                            if (i is IDataCartogram)
                                RenderParavName((IDataCartogram)i, e.Coords, sb);
                    }

                    if (_itemDifferentialPre != null)
                    {
                        // Разобраться с названием потока!!!
                        IDataTuple t = _ui.GetMultiDataProvider().
                            GetData(_itemDifferentialPre.GetTimeDate(), _fullTuple.GetStreamName())[0];

                        sb.AppendFormat("\r\n\r\n{0}\r\n", t.GetTimeDate());
                        sb.Append(ComposeCellName(e.Coords, false));

                        foreach (ITupleItem i in t)
                            if (i is IDataCartogram)
                                RenderParavName((IDataCartogram)i, e.Coords, sb);
                    }

                }
                catch (Exception)
                {
                    sb.Remove(0, sb.Length);
                    sb.AppendFormat("{0}\r\n", _item.GetTimeDate());
                    sb.Append(ComposeCellName(e.Coords, false));

                    foreach (ITupleItem i in _arranged)
                    {
                        if (i is IDataCartogram)
                        {
                            RenderParavName((IDataCartogram)i, e.Coords, sb);
                        }
                    }

                }
            }

            _ui.SetDetailString(sb.ToString());
        }

        #region String info operation

        void RenderParavName(IDataCartogram c, Coords paramInCart, StringBuilder sb)
        {
            IInfoFormatter nf = _ui.GetEnviroment().GetFormatter((IDataCartogram)c);

            if (!c.IsValidCoord(paramInCart))
                return;

            if (c.Layers == 1)
            {
                object o = c.GetObject(paramInCart, 0);
                sb.AppendFormat("{0}: ", c.GetHumanName());
                sb.Append(nf.GetString(o));
                sb.Append("\r\n");
            }
            else
            {
                sb.AppendFormat("{0}: {{", c.GetHumanName());
                for (int l = 0; l < c.Layers; l++)
                {
                    if (l > 0)
                        sb.AppendFormat("; ");

                    sb.Append(nf.GetString(c.GetObject(paramInCart, l)));
                }
                sb.AppendFormat("}}\r\n");
            }
        }

        String ComposeCellName(Coords coords, bool singleLine)
        {
            string separator = "\r\n";
            if (singleLine)
                separator = "";

            IInfoFormatter formatter = _ui.GetEnviroment().GetDefFormatter(FormatterType.Coords);
            return String.Format("Ячейка {0}{1}{1}", formatter.GetString(coords), separator);

        }

        #endregion

        #region menu -- Sould be puginable
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

        void _nav_RightClickCoord(object sender, CoodrsEventArgs e)
        {
#if DOTNET_V11
			_menuTags.Clear();

            ArrayList list = new ArrayList();
#else
            ContextMenuStrip stripMenu = new ContextMenuStrip();
#endif

            foreach (ActionDataCartogramVisualizerUI obj in _actions.Values)
            {
                if (obj._action == ActionDataCartogramVisualizerUI.Actions.CellItemContextMenu)
                {
#if DOTNET_V11
                    MenuItem mi = new MenuItem(obj._humaneName, new EventHandler(ProxyUIMenuEvent));
                    _menuTags[mi] = new IntermediateEventDataUI(new ADCVEventCoordArgs(this, e.Coords), obj);
                    list.Add(mi);
#else
                    ToolStripMenuItem mi = new ToolStripMenuItem(obj._humaneName, null, new EventHandler(ProxyUIMenuEvent));
                    mi.Tag = new IntermediateEventDataUI(new ADCVEventCoordArgs(this, e.Coords), obj);
                    mi.Image = obj._image;
                    stripMenu.Items.Add(mi);
#endif
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

        #endregion


        #region ITupleItemVisualizerUI Members

        public IMultiDataProvider GetMultiDataProvider()
        {
            return _ui.GetMultiDataProvider();
        }

        public string GetActiveTupleItemStream()
        {
            return _stream;
        }

        public ITupleItem GetActiveTupleItem()
        {
            // return _item;

            ITupleItem item = _itemDifferential; // currectTupleOrig;
            if (item == null)
                item = _item;

            return item;
        }

        public void SetActiveTupleItem(ITupleItem item, string stream)
        {
            _itemDifferentialPre = null;
            _itemDifferential = null;
            _item = (IDataCartogram)item;
            _stream = stream;

            _fullTuple = _ui.GetActiveDataTuple();
            if (_fullTuple != null)
                _arranged = _fullTuple.GetData();
            else
                _arranged = new ITupleItem[0];

            //_formatter = ((IDataCartogram)_item).GetDefForamtter(_ui.GetEnviroment());
            //_formatter = _ui.GetEnviroment().GetFormatter((IDataCartogram)_item);
            SetCartogram(_item, _ui.GetEnviroment());

            SetView(_isCart, _isCart == false);
        }

        public void SetDifferentialView(IDataCartogram orig, IDataCartogram prev)
        {
            _itemDifferential = orig;
            _itemDifferentialPre = prev;

            SetCartogram(_item, _ui.GetEnviroment(), "DiffLevel");
        }

        #endregion

        #region IDataCartogramVisualizerUI Members

        public Coords[] GetSelectedCells()
        {
            return Navigation.GetSelected();
        }

        public void SetSelectedCells(Coords[] cells)
        {
            Navigation.FlushSelection();
            if (cells != null)
                Navigation.SelectCoords(cells);
        }

        public bool SelectCell(Coords crd, bool select)
        {
            Navigation.SelectCoord(crd, select);

            return true; // TODO fix returning value
        }

        public bool RegisterAction(ActionDataCartogramVisualizerUI ac)
        {
            _actions.Add(ac._name, ac);
            AddHookForAction(ac);
            return true;
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

        #region ITupleItemVisualizerUI Members


        public IDataTupleVisualizerUI GetDataTupleVisualizer()
        {
            return _ui;
        }

        #endregion
    }
}
