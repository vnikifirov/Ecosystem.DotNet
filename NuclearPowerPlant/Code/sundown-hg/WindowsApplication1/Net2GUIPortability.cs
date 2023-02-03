using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
	//////////////////////////////////////////////////////////////////
	
	public enum SizeType
	{
		Absolute,
		AutoSize,
		Percent
	}

	public enum TableLayoutPanelGrowStyle
	{
		AddColumns,
		AddRows,
		FixedSize 
	}
	

	public struct TableLayoutPanelCellPosition
	{
		public TableLayoutPanelCellPosition (
			int column,
			int row
			)
		{
			_column = column;
			_row = row;
		}

		public int Column 
		{
			get 
			{
				return _column;
			}
			set
			{
				_column = value;
			}
		}

		public int Row 
		{ 
			get
			{
				return _row;
			}
			set
			{
				_row = value;
			}
		}


		public override bool Equals (
			object other
			)
		{
			if (typeof(TableLayoutPanelCellPosition).IsAssignableFrom(other.GetType()))
			{
				TableLayoutPanelCellPosition i = 
					(TableLayoutPanelCellPosition)other;

				return this == i;
			}
			return false;
		}

		public static bool operator == (
			TableLayoutPanelCellPosition p1,
			TableLayoutPanelCellPosition p2
			)
		{
			return (p1._column == p2._column) && (p1._row == p2._row);
		}

		public static bool operator != (
			TableLayoutPanelCellPosition p1,
			TableLayoutPanelCellPosition p2
			)
		{
			return !(p1==p2);
		}

		public override int GetHashCode ()
		{
			return ((_column << 16) | _row );
		}


		int _column;
		int _row;
	}

	public abstract class TableLayoutStyle
	{
		public SizeType SizeType 
		{
			get
			{
				return _sztype; 
			}
			set
			{
				_sztype = value;
			}
		}

		protected SizeType _sztype = SizeType.AutoSize;
	}

	public class ColumnStyle : TableLayoutStyle
	{
		public float Width 
		{ 
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public ColumnStyle (
			SizeType sizeType,
			float width
			)
		{
			_sztype = sizeType;
			_width = width;
		}

		float _width = 0;		

	}

	public class RowStyle : TableLayoutStyle
	{
		public float Height 
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public RowStyle (
			SizeType sizeType,
			float height
			)
		{
			_sztype = sizeType;
			_height = height;
		}

		float _height;
	}

	public abstract class TableLayoutStyleCollection : IList, ICollection, IEnumerable
	{
		public int Count 
		{ 
			get
			{
				return _l.Count;
			} 
		}

		public TableLayoutStyle this [int index]
		{ 
			get
			{
				return (TableLayoutStyle)_l[index];
			}
			set
			{
				_l[index] = value;
			}
		}


		public int Add (TableLayoutStyle style)
		{
			return _l.Add(style);
		}

		public void Clear()
		{
			_l.Clear();
		}

		public void RemoveAt (int index)
		{
			_l.RemoveAt(index);
		}

        #region IList Members

        int IList.Add(object value)
        {
            return _l.Add(value);
        }

        bool IList.Contains(object value)
        {
            return _l.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return _l.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            _l.Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return _l.IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return _l.IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            _l.Remove(value);
        }

        object IList.this[int index]
        {
			get
			{
				return _l[index];
			}
			set
			{
				_l[index] = value;
			}
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            _l.CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return _l.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _l.IsSynchronized; }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _l.GetEnumerator();
        }

        #endregion

		protected ArrayList _l = new ArrayList();
    }

	public class TableLayoutColumnStyleCollection : TableLayoutStyleCollection
	{
		new public ColumnStyle this [int index] 
		{
			get
			{
				if (index >= 0 && index < base.Count)
					return (ColumnStyle)base[index];
				else
					return new ColumnStyle(SizeType.Percent, 20);
			}
			set
			{
				base[index] = value;
			}
		}


		public int Add (ColumnStyle columnStyle)
		{
			return base.Add(columnStyle);
		}

		public bool Contains (
			ColumnStyle columnStyle
			)
		{
			return _l.Contains(columnStyle);
		}

		public int IndexOf (
			ColumnStyle columnStyle
			)
		{
			return _l.IndexOf(columnStyle);
		}

		public void Insert (
			int index,
			ColumnStyle columnStyle
			)
		{
			_l.Insert(index, columnStyle);
		}

		public void Remove (
			ColumnStyle columnStyle
			)
		{
			_l.Remove(columnStyle);
		}

	}

	public class TableLayoutRowStyleCollection : TableLayoutStyleCollection
	{
		new public RowStyle this [int index] 
		{ 
			get
			{
				if (index >=0 && index < base.Count)
					return (RowStyle)base[index];
				else
					return new RowStyle(SizeType.Absolute, 30);
			}
			set
			{
				base[index] = value;
			}
		}


		public int Add (RowStyle rowStyle)
		{
			return base.Add(rowStyle);
		}

		public bool Contains (
			RowStyle rowStyle
			)
		{
			return _l.Contains(rowStyle);
		}

		public int IndexOf (
			RowStyle rowStyle
			)
		{
			return _l.IndexOf(rowStyle);
		}

		public void Insert (
			int index,
			RowStyle rowStyle
			)
		{
			_l.Insert(index, rowStyle);
		}

		public void Remove (
			RowStyle rowStyle
			)
		{
			_l.Remove(rowStyle);
		}
	}


	public class TableLayoutControlCollection : Control.ControlCollection 
	{
		public TableLayoutControlCollection(TableLayoutPanel container)
			: base(container)
		{
			_parent = container;
		}

		public TableLayoutPanel Container 
		{ 
			get
			{
				return _parent;
			}
		}

		public new void Add (
			Control control
			)
		{
			throw new NotImplementedException();
		}

		public virtual void Add (
			Control control,
			int column,
			int row
			)
		{
			_parent.AddControl(control, column, row);

			_parent._UpdateSizes();
		}


		TableLayoutPanel _parent;
	}


	public class TableLayoutPanel : Panel
	{

		public TableLayoutPanel()
		{
			_controls = new TableLayoutControlCollection(this);
		}

		public TableLayoutColumnStyleCollection ColumnStyles
		{ 
			get
			{
				return _cloumnstyle;
			}
		}

		new public TableLayoutControlCollection Controls 
		{
			get
			{
				return _controls;
			}
		}

		public TableLayoutRowStyleCollection RowStyles
		{
			get
			{
				return _rowstyle;
			}
		}


		public int ColumnCount
		{ 
			get
			{
				return _columnReal.Length;
			}
			set
			{
				GrowTo(value, RowCount);
			}
		}

		public int RowCount
		{ 
			get
			{
				return _rowReal.Length;
			}
			set
			{
				GrowTo(ColumnCount, value);
			}
		}

		public TableLayoutPanelGrowStyle GrowStyle
		{
			get
			{
				return _growstyle;
			}
			set
			{
				_growstyle = value;
			}
		}

		public Control GetControlFromPosition(int column, int row)
		{
			if ((column <0) || (row < 0))
				throw new				ArgumentException();

			CellControlInfo c = tableControls[column, row]; 
			if (c!=null)
				return c.control;

			return null;
		}


		internal void UpdateCache()
		{
			int maxCol = 0;
			int maxRow = 0;
			foreach (CellControlInfo c in cellContainer)
			{
				if (c.pos.Column > maxCol)
					maxCol = c.pos.Column;
				if (c.pos.Row > maxRow)
					maxRow = c.pos.Row;
			}

			if (maxCol + 1 > tableControls.GetLength(0) || maxRow + 1 > tableControls.GetLength(1))
				tableControls = new CellControlInfo[maxCol + 1, maxRow + 1];

			foreach (CellControlInfo c in cellContainer)
			{
				if (c != null)
				{
					tableControls[c.pos.Column, c.pos.Row] = c;
				}
			}

			_layoutChanged = false;
		}

		internal void UpdateSizes()
		{
			int margin_all = 3;

			float totalHeightPrecent = 0;
			float totalWidthPrecent = 0;

			int totalHeightAbsolute = 0;
			int totalWidthAbsolute = 0;

			
			totalWidthAbsolute += margin_all;

			for (int i = 0; i < ColumnCount; i++)
			{
				ColumnStyle s = _cloumnstyle[i];
				if (s.SizeType == SizeType.Absolute)
				{
					totalWidthAbsolute += (int)s.Width;
				}
				else if (s.SizeType == SizeType.Percent)
				{
					totalWidthPrecent += s.Width;
				}
				else
				{
					throw new NotImplementedException();
				}

				totalWidthAbsolute += margin_all;
			}
			
			totalHeightAbsolute += margin_all;
			for (int i = 0; i < RowCount; i++)
			{
				RowStyle s = _rowstyle[i];
				if (s.SizeType == SizeType.Absolute)
				{
					totalHeightAbsolute += (int)s.Height;
				}
				else if (s.SizeType == SizeType.Percent)
				{
					totalHeightPrecent += s.Height;
				}
				else
				{
					throw new NotImplementedException();
				}
				totalHeightAbsolute += margin_all;
			}
			

			int autoHeight = base.Size.Height - totalHeightAbsolute;
			int autoWidth = base.Size.Width - totalWidthAbsolute;


			for (int i = 0, tmpColPos = margin_all; i < ColumnCount; i++)
			{
				ColumnStyle s = _cloumnstyle[i];
				if (s.SizeType == SizeType.Absolute)
				{
					_columnReal[i] = (int)s.Width;
				}
				else if (s.SizeType == SizeType.Percent)
				{
					if (autoWidth > 0)
						_columnReal[i] = (int)((s.Width / totalWidthPrecent) * autoWidth);
					else
						_columnReal[i] = 0;
				}

				_columnPos[i] = tmpColPos;
				tmpColPos += _columnReal[i];
				tmpColPos += margin_all;
			}

			for (int i = 0, tmpRowPos = margin_all; i < RowCount; i++)
			{
				RowStyle s = _rowstyle[i];
				if (s.SizeType == SizeType.Absolute)
				{
					_rowReal[i] = (int)s.Height;
				}
				else if (s.SizeType == SizeType.Percent)
				{
					if (autoWidth > 0)
						_rowReal[i] = (int)((s.Height / totalHeightPrecent) * autoHeight);
					else
						_rowReal[i] = 0;
				}

				_rowPos[i] = tmpRowPos;
				tmpRowPos += _rowReal[i];
				tmpRowPos += margin_all;
			}

			//Move
			
			for (int row = 0; row < RowCount; row++)
			{
				for (int col = 0; col < ColumnCount; col++)
				{
					CellControlInfo c = tableControls[col, row];
					if (c != null)
					{
						c.cpanel.Location = new Point(_columnPos[col], _rowPos[row]);
						c.cpanel.Size = new Size(_columnReal[col], _rowReal[row]);
					}
				}
			}


		}

		internal void GrowTo(int col, int row)
		{
			tableControls = new CellControlInfo[col,row];
			_columnReal = new int[col];
			_rowReal = new int[row];

			_columnPos = new int[col];
			_rowPos = new int[row];

			_layoutChanged = true;
		}

		internal void AddControl(Control ctrl, int col, int row)
		{
			if ((col <0) || (row < 0))
			{
				throw new NotImplementedException();
			}
			CellControlInfo c;

			foreach (CellControlInfo cp in cellContainer)
			{
				if ((cp.pos.Column == col) && (cp.pos.Row == row))
				{
					//throw new Exception("Already set!");
					//cellContainer.Remove(c);
					//break;
					base.Controls.Remove(cp.cpanel);
				}
			}

			c = new CellControlInfo();
			c.pos = new TableLayoutPanelCellPosition(col, row);
			c.cpanel = new Panel();
			c.control = ctrl;
			c.cpanel.Controls.Add(ctrl);			
			c.original = ctrl.Size;

			//c.cpanel.BackColor = Color.AliceBlue;

			ctrl.Location = new Point(0, 0);

			cellContainer.Add(c);
			base.Controls.Add(c.cpanel);
			//c.cpanel.Update();
			//c.cpanel.Refresh();
			_layoutChanged = true;
		}

		public void _UpdateSizes()
		{
			if (tableControls.Length > 0)
			{
				if (_layoutChanged)
					UpdateCache();

			
				UpdateSizes();
			}

		}

		protected override void OnResize (
			EventArgs eventargs
			)
		{
			_UpdateSizes();
			base.OnResize(eventargs);
		}

		protected override void OnVisibleChanged (
			EventArgs e
			)
		{
			if (_layoutChanged)
			{
				UpdateCache();
				UpdateSizes();
			}


			base.OnVisibleChanged(e);
		}
/*
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( _controls != null )
					_controls.Dispose();
			}
			base.Dispose( disposing );
		}
		*/

		class CellControlInfo
		{
			public TableLayoutPanelCellPosition pos;
			public Control control;
			public Panel cpanel;
			public Size original;
		}

		ArrayList cellContainer = new ArrayList();

		TableLayoutPanelGrowStyle _growstyle = TableLayoutPanelGrowStyle.AddRows;		
		TableLayoutControlCollection _controls;
		TableLayoutRowStyleCollection _rowstyle = new TableLayoutRowStyleCollection();
		TableLayoutColumnStyleCollection _cloumnstyle = new TableLayoutColumnStyleCollection();

		// Cached items
		CellControlInfo[,] tableControls;		

		int[] _columnReal = new int[0];
		int[] _rowReal = new int[0];

		int[] _columnPos;
		int[] _rowPos;

		bool _layoutChanged = false;
	}



}
