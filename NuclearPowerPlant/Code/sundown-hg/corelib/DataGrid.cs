using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Globalization;

namespace corelib
{
    public struct OleObject
    {
        object _obj;

        const System.Reflection.BindingFlags def = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static;

        const System.Reflection.BindingFlags gp = def | System.Reflection.BindingFlags.GetProperty;
        const System.Reflection.BindingFlags sp = def | System.Reflection.BindingFlags.SetProperty;

        const System.Reflection.BindingFlags inv = def | System.Reflection.BindingFlags.InvokeMethod;

        public OleObject(Object c)
        {
            _obj = c;
        }

        public OleObject(string objectName)
        {
            Type tp = Type.GetTypeFromProgID(objectName);
            _obj = Activator.CreateInstance(tp);
        }

        public Object Object
        {
            get { return _obj; }
        }


        public OleObject GetProperty(string name)
        {
            return GetProperty(name, null);
        }

        public OleObject GetProperty(string name, params object[] parameters )
        {
            return new OleObject(_obj.GetType().InvokeMember(name, gp, null, _obj, parameters));
        }

        public OleObject SetProperty(string name, params object[] parameters)
        {
            return new OleObject(_obj.GetType().InvokeMember(name, sp, null, _obj, parameters));
        }

        public OleObject Invoke(string name)
        {
            return Invoke(name, null);
        }

        public OleObject Invoke(string name, params object[] parameters)
        {
            return new OleObject(_obj.GetType().InvokeMember(name, inv, null, _obj, parameters));
        }
    }



    public class DataGrid
    {
        public class Column
        {
            string _header;
            Type _type;
            IInfoFormatter _converter;

            public Column(string name, Type t, IInfoFormatter conv)
            {
                if (conv == null)
                    throw new ArgumentException();

                _header = name;
                _type = t;
                _converter = conv;
            }

            static public IInfoFormatter GetDefaultFormatter(Type t)
            {
                if ((t == typeof(byte)) || (t == typeof(sbyte)) ||
                    (t == typeof(ushort)) || (t == typeof(short)) ||
                    (t == typeof(uint)) || (t == typeof(int)) ||
                    (t == typeof(ulong)) || (t == typeof(long)))
                {
                    return _dc;
                }
                else if ((t == typeof(float)) || (t == typeof(double)))
                {
                    return _df;
                }
                else
                {
                    return _ds;
                }
            }

            public Column(string name, Type t)
                : this(name, t, GetDefaultFormatter(t))
            {               
            }

            public Column(string name)
                : this(name, typeof(string), _ds)
            {
               
            }

            public string Header
            {
                get { return _header; }
                set { _header = value; }
            }

            public Type ColumnType
            {
                get { return _type; }
            }

            public IInfoFormatter Converter
            {
                set { _converter = value; }
                get { return _converter; }
            }
        }

        class ColumnData
        {
            Array _data;

            public void Realloc(int newCount)
            {
                Array newData = Array.CreateInstance(ColumnType, newCount);
                int count = Math.Min(newCount, _data.Length);
                for (int i = 0; i < count; i++)
                    newData.SetValue(_data.GetValue(i),i);

                _data = newData;
            }

            public Type ColumnType
            {
                get { return _data.GetType().GetElementType(); }
            }

            public object this[int index]
            {
                get { return _data.GetValue(index); }
                set { _data.SetValue(value, index); }
            }

            public ColumnData(Type t, int dataSize)
            {
                _data = Array.CreateInstance(t, dataSize);
            }

        }

        public abstract class Row : IList
        {
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Count; i++)
                {
                    sb.Append(this[i]);
                    if (i < Count - 1)
                        sb.Append("; ");
                }

                return sb.ToString();
            }

            #region IList Members

            public int Add(object value)
            {
                throw new NotSupportedException("Row has fixed size");
            }

            public void Clear()
            {
                throw new NotSupportedException("Row has fixed size");
            }

            public bool Contains(object value)
            {
                return (IndexOf(value) != -1);
            }

            public int IndexOf(object value)
            {
                int i = 0;
                foreach (object obj in this)
                {
                    if (obj == value)
                        return i;

                    i++;
                }

                return -1;
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException("Row has fixed size");
            }

            public bool IsFixedSize
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Remove(object value)
            {
                throw new NotSupportedException("Row has fixed size");
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException("Row has fixed size");
            }

            public abstract object this[int index]
            {
                get;
                set;
            }

            #endregion

            #region ICollection Members

            public abstract void CopyTo(Array array, int index);

            public abstract int Count
            {
                get;
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public abstract object SyncRoot
            {
                get;
            }

            #endregion

            #region IEnumerable Members

            public abstract IEnumerator GetEnumerator();

            #endregion
        }

        class InternalRow : Row
        {
            int _index;
            DataGrid _parent;

            public InternalRow(DataGrid parent, int index)
            {
                _parent = parent;
                _index = index;
            }

            public int Index
            {
                get { return _index; }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Count; i++)
                {
                    sb.Append(GetString(i));
                    if (i < Count - 1)
                        sb.Append("; ");
                }

                return sb.ToString();
            }

            public override object this[int index]
            {
                get
                {
                    return ((ColumnData)_parent._data[index])[_index];
                }
                set
                {
                    ((ColumnData)_parent._data[index])[_index] = value;
                }
            }

            public override void CopyTo(Array array, int index)
            {
                foreach (object i in this)
                {
                    array.SetValue(i, index++);
                }
            }

            public override int Count
            {
                get { return _parent._data.Count; }
            }

#if DOTNET_V11
            class DataRowEnumarator : IEnumerator
            {
                IEnumerator _base;
                InternalRow _row;

                public DataRowEnumarator(InternalRow r)
                {
                    _row = r;
                    _base = _row._parent._data.GetEnumerator();
                }

                #region IEnumerator Members

                public object Current
                {
                    get { return ((ColumnData)_base.Current)[_row._index]; }
                }

                public bool MoveNext()
                {
                    return _base.MoveNext();
                }

                public void Reset()
                {
                    _base.Reset();
                }

                #endregion
            }

            public override IEnumerator GetEnumerator()
            {
                return new DataRowEnumarator(this);
            }
#else
            public override IEnumerator GetEnumerator()
            {
                foreach (ColumnData d in _parent._data)
                {
                    yield return d[_index];
                }
            }
#endif
            public override object SyncRoot
            {
                get { return _parent; }
            }


            public string GetString(int index)
            {
                return _parent._columns[index].Converter.GetString(this[index]);
            }

            public string GetQuotedString(int index)
            {
                return _parent._columns[index].Converter.GetStringQuoted(this[index]);
            }
        }

        class InternalStringRow : Row
        {
            int _index;
            DataGrid _parent;            

            public InternalStringRow(DataGrid parent, int index)
            {
                _parent = parent;
                _index = index;
            }

            public override object this[int index]
            {
                get
                {
                    return _parent._columns[index].Converter.GetString(((ColumnData)_parent._data[index])[_index]);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override void CopyTo(Array array, int index)
            {
                for (int i = 0; i < Count; i++)
                {
                    array.SetValue(this[i], index + i);
                }
            }

            public override int Count
            {
                get { return _parent._data.Count; }
            }

            public override object SyncRoot
            {
                get { return _parent._data; }
            }

#if DOTNET_V11
			class DataStringRowEnumarator : IEnumerator
			{
				IEnumerator _base;
				InternalStringRow _row;

				public DataStringRowEnumarator(InternalStringRow r)
				{
					_row = r;
					_base = _row._parent._data.GetEnumerator();
				}

				#region IEnumerator Members

				public object Current
				{
					get { return ((ColumnData)_base.Current)[_row._index]; }
				}

				public bool MoveNext()
				{
					return _base.MoveNext();
				}

				public void Reset()
				{
					_base.Reset();
				}

				#endregion
			}

			public override IEnumerator GetEnumerator()
			{
				return new DataStringRowEnumarator(this);
			}
#else
            public override IEnumerator GetEnumerator()
            {
                foreach (ColumnData d in _parent._data)
                {
                    yield return d[_index];
                }
            }
#endif
        }

        public class DataRow : Row
        {
            object[] _data;

            public DataRow(object[] data)
            {
                _data = data;
            }

            public DataRow(int count)
            {
                _data = new object[count];
            }

            public override object this[int index]
            {
                get
                {
                    return _data[index];
                }
                set
                {
                    _data[index] = value;
                }
            }

            public override void CopyTo(Array array, int index)
            {
                _data.CopyTo(array, index);
            }

            public override int Count
            {
                get { return _data.Length; }
            }

            public override object SyncRoot
            {
                get { return _data.SyncRoot; }
            }

            public override IEnumerator GetEnumerator()
            {
                return _data.GetEnumerator();
            }
        }

        public abstract class ColumnCollection
#if !DOTNET_V11
            : IList<Column>
        {
            protected List<Column> _columns = new List<Column>();


            #region IEnumerable<Column> Members

            public IEnumerator<Column> GetEnumerator()
            {
                return _columns.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
#else
            : IList
        {
            protected ArrayList _columns = new ArrayList();


            #region IList Members

            public int IndexOf(object item)
            {
                return IndexOf((Column)item);
            }

            public bool Contains(object item)
            {
                return Contains((Column)item);
            }

            public void CopyTo(Array array, int arrayIndex)
            {
                _columns.CopyTo(array, arrayIndex);
            }

            #endregion

            public IEnumerator GetEnumerator()
            {
                return _columns.GetEnumerator();
            }

            public int Add(object o)
            {
                Add((Column)o);
                return Count - 1;
            }

            public void Insert(int index, object item)
            {
                Insert(index, (Column)item);
            }

            public void Remove(object o)
            {
                Remove((Column)o);
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    this[index] = (Column)value;
                }
            }

            #region IList Members

            public bool IsFixedSize
            {
                get { return false; }
            }

            #endregion

            #region ICollection Members

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { return _columns; }
            }

            #endregion
#endif

            #region IList<Column> Members

            public int IndexOf(Column item)
            {
                return _columns.IndexOf(item);
            }

            public bool Contains(Column item)
            {
                return _columns.Contains(item);
            }

            public void CopyTo(Column[] array, int arrayIndex)
            {
                _columns.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return _columns.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            #endregion

            #region IList<Column> Members

            public abstract void Insert(int index, Column item);

            public abstract void RemoveAt(int index);

            public abstract Column this[int index]
            {
                get;
                set;
            }

            #endregion

            #region ICollection<Column> Members

            public abstract void Add(Column item);
            public abstract void Clear();
            public abstract bool Remove(Column item);

            #endregion

            public void Add(string header, Type t)
            {
                Add(new Column(header, t));
            }

            public void Add(string header, Type t, IInfoFormatter i)
            {
                Add(new Column(header, t, i));
            }
        }

        class InternalColumns : ColumnCollection
        {
            DataGrid _parent;

            public InternalColumns(DataGrid parent)
            {
                _parent = parent;
            }

            #region IList<Column> Members

            public override void Insert(int index, Column item)
            {
                _columns.Insert(index, item);
                _parent._data.Insert(index, new ColumnData(item.ColumnType, _parent._reserveRows));
            }

            public override void RemoveAt(int index)
            {
                _columns.RemoveAt(index);
                _parent._data.RemoveAt(index);
            }

            public override Column this[int index]
            {
                get
                {
                    return (Column)_columns[index];
                }
                set
                {
                    _columns[index] = value;
                    _parent._data[index] = new ColumnData(value.ColumnType, _parent._reserveRows);
                }
            }

            #endregion

            #region ICollection<Column> Members

            public override void Add(Column item)
            {
                _columns.Add(item);
                _parent._data.Add(new ColumnData(item.ColumnType, _parent._reserveRows));
            }

            public override void Clear()
            {
                _columns.Clear();
                _parent._data.Clear();
            }

            public override bool Remove(Column item)
            {
                int i = _columns.IndexOf(item);
                if (i >= 0)
                {
                    _columns.RemoveAt(i);
                    _parent._data.RemoveAt(i);

                    return true;
                }
                return false;
            }

            #endregion
        }

        public abstract class RowCollection 
#if !DOTNET_V11            
            : IList<Row>
        {
            #region IEnumerable<Row> Members

            public IEnumerator<Row> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
#else
            : IList
        {

            #region IList Members

            public int Add(object value)
            {
                Add((Row)value);
                return Count - 1;
            }

            public bool Contains(object value)
            {
                return Contains((Row)value);
            }

            public int IndexOf(object value)
            {
                return IndexOf((Row)value);
            }

            public void Insert(int index, object value)
            {
                Insert(index, (Row)value);
            }

            public bool IsFixedSize
            {
                get { return false; }
            }

            public void Remove(object value)
            {
                Remove((Row)value);
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    this[index] = (Row)value;
                }
            }

            #endregion

            #region ICollection Members

            public void CopyTo(Array array, int index)
            {
                CopyTo((Row[])array, index);
            }

            public bool IsSynchronized
            {
                get { return false; }
            }

            public object SyncRoot
            {
                get { return this; }
            }

            #endregion



#if DOTNET_V11
			class ObjectRowEnumarator : IEnumerator
			{
				int _idx;
				RowCollection _row;

				public ObjectRowEnumarator(RowCollection r)
				{
					_row = r;
					_idx = -1;
				}

				#region IEnumerator Members

				public object Current
				{
					get { return _row[_idx]; }
				}

				public bool MoveNext()
				{
					return (++_idx < _row.Count);
				}

				public void Reset()
				{
					_idx = -1;
				}

				#endregion
			}

			public IEnumerator GetEnumerator()
			{
				return new ObjectRowEnumarator(this);
			}
#else

            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }
#endif

#endif

            public void Add(params object[] items)
            {
                Add(new DataRow(items));
            }

            #region IList<Row> Members

            public abstract int IndexOf(Row item);

            public abstract void Insert(int index, Row item);

            public abstract void RemoveAt(int index);

            public abstract Row this[int index]
            {
                get;
                set;
            }

            #endregion

            #region ICollection<Row> Members

            public abstract void Add(Row item);

            public abstract Row Add();

            public abstract void Clear();

            public bool Contains(Row item)
            {
                return (IndexOf(item) != -1);
            }

            public void CopyTo(Row[] array, int arrayIndex)
            {
                foreach (Row r in this)
                {
                    array[arrayIndex++] = r;
                }
            }

            public abstract int Count
            {
                get;
            }

            public abstract bool IsReadOnly
            {
                get;
            }

            public bool Remove(Row item)
            {
                int i = IndexOf(item);
                if (i != -1)
                {
                    RemoveAt(i);
                    return true;
                }
                return false;
            }

            #endregion
        }


        class InternalRows : RowCollection
#if !DOTNET_V11
            , IEnumerable<InternalRow>
#endif
        {
            DataGrid _parent;

            public InternalRows(DataGrid parent)
            {
                _parent = parent;
            }

            public override int IndexOf(Row item)
            {
                InternalRow r = (InternalRow)item;
                return r.Index;
            }

            public override void Insert(int index, Row item)
            {
                throw new NotSupportedException("Not supported yet");
            }

            public override void RemoveAt(int index)
            {
                if (index + 1 == _parent.RowCount)
                {
                    _parent._numRows = _parent.RowCount - 1;
                }
                else
                {
                    throw new NotSupportedException("Not supported yet");
                }
            }

            public override Row this[int index]
            {
                get
                {
                    if ((index < 0) || (index >= Count))
                        throw new ArgumentException("Неверный индекс");

                    return new InternalRow(_parent, index);
                }
                set
                {
                    if ((index < 0) || (index >= Count))
                        throw new ArgumentException("Неверный индекс");

                    for (int i = 0; i < _parent._data.Count; i++ )
                    {
                        ((ColumnData)_parent._data[i])[index] = value[i];
                    }

                    // TODO Check inferiority addition
                }
            }

            public override void Clear()
            {
                _parent._numRows = 0;

                // TODO Add memory shrinking
            }

            public override int Count
            {
                get { return _parent._numRows; }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

#if !DOTNET_V11
            public new IEnumerator<InternalRow> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return new InternalRow(_parent, i);
                }
            }
#endif

            public override void Add(Row item)
            {
                if (Count == _parent._reserveRows - 1)
                {
                    _parent.Preserve(_parent._reserveRows +  sDefaultPreserve);
                }

                int old = _parent._numRows++;
                this[old] = item;
            }

            public override Row Add()
            {
                if (Count == _parent._reserveRows - 1)
                {
                    _parent.Preserve(_parent._reserveRows + sDefaultPreserve);
                }
                int old = _parent._numRows++;
                return this[old];
            }
        }


        class InternalStringRows : RowCollection
#if !DOTNET_V11
, IEnumerable<InternalStringRow>
#endif
        {
            DataGrid _parent;

            public InternalStringRows(DataGrid parent)
            {
                _parent = parent;
            }

            public override int IndexOf(Row item)
            {
                throw new NotSupportedException();
            }

            public override void Insert(int index, Row item)
            {
                throw new NotSupportedException();
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public override Row this[int index]
            {
                get
                {
                    return new InternalStringRow(_parent, index);
                }
                set
                {
                    throw new NotSupportedException();
                }
            }

            public override void Add(Row item)
            {
                throw new NotSupportedException();
            }

            public override Row Add()
            {
                throw new NotSupportedException();
            }

            public override void Clear()
            {
                throw new NotSupportedException();
            }

            public override int Count
            {
                get { return _parent._numRows; }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

#if !DOTNET_V11
            public new IEnumerator<InternalStringRow> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return new InternalStringRow(_parent, i);
                }
            }
#endif
        }


        static DataNumericConverter _dc = new DataNumericConverter();
        static DataFloatConverter _df = new DataFloatConverter();
        static DataStringConverter _ds = new DataStringConverter();

        InternalColumns _columns;
        InternalRows _rows;
        InternalStringRows _stringRows;
#if !DOTNET_V11
        List<ColumnData> _data = new List<ColumnData>();
#else
        ArrayList _data = new ArrayList();
#endif

        int _numRows;
        int _reserveRows;

        int _headColumns;

        public static int sDefaultPreserve = 100;

        public DataGrid()
        {
            _headColumns = 0;
            _numRows = 0;
            _reserveRows = sDefaultPreserve;
            _columns = new InternalColumns(this);
            _rows = new InternalRows(this);
            _stringRows = new InternalStringRows(this);
        }

        public void Preserve(int count)
        {
            if (_reserveRows < count)
            {
                _reserveRows = count;

                foreach (ColumnData d in _data)
                {
                    d.Realloc(count);
                }
            }
        }

        public int HeadColumns
        {
            get { return _headColumns; }
            set
            {
                if ((value < 0) || (value > ColumnCount))
                    throw new ArgumentException();

                _headColumns = value;
            }
        }

        public ColumnCollection Columns
        {
            get { return _columns; }
        }

        public RowCollection Rows
        {
            get { return _rows; }
        }

        public RowCollection StringRows
        {
            get { return _stringRows; }
        }

        public int ColumnCount
        {
            get { return _columns.Count; }
        }

        public int RowCount
        {
            get { return _numRows; }
        }

        public string DumpCSV()
        {
            return DumpCSV(',');
        }

        public string DumpCSV(char separationChar)
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i < Columns.Count; i++)
            {
                b.Append(_ds.GetStringQuoted(Columns[i].Header));

                if (i < Columns.Count - 1)
                {
                    b.Append(separationChar);
                    b.Append(" ");
                }
            }
            b.Append("\r\n");
            foreach (InternalRow r in _rows)
            {
                for (int i = 0; i < r.Count; i++ )
                {
                    b.Append(r.GetQuotedString(i));

                    if (i < r.Count - 1)
                    {
                        b.Append(separationChar);
                        b.Append(" ");
                    }
                }
                b.Append("\r\n");
            }

            return b.ToString();
        }

        public OleObject ExportExcel()
        {
            OleObject excel = new OleObject("Excel.Application");
            excel.GetProperty("Workbooks").Invoke("Add");

            ExportExcelToWorkSheet(excel.GetProperty("WorkSheets", 1), 1, 1, true);

            excel.SetProperty("Visible", true);

            return excel;
        }

        static bool IsTypeInts(Type t)
        {
            return ((t == typeof(byte)) || (t == typeof(sbyte)) ||
                          (t == typeof(ushort)) || (t == typeof(short)) ||
                          (t == typeof(uint)) || (t == typeof(int)) ||
                          (t == typeof(ulong)) || (t == typeof(long)));
        }

        static bool IsTypeReals(Type t)
        {
            return ((t == typeof(float)) || (t == typeof(double)) || (t == typeof(Decimal))); 
        }

        static bool IsTypeExtra(Type t)
        {
            return (t == typeof(DateTime));
        }

        public DataGrid ExpandAll()
        {
            DataGrid d = new DataGrid();
            d.Preserve(this._reserveRows);

            foreach (Column c in Columns)
            {
                int multiColumns = c.Converter.ColumntCount;
                for (int i = 0; i < multiColumns; i++)
                {
                    if (multiColumns == 1)
                    {
                        if (c.Converter.RelutingType != null)
                        {
                            d.Columns.Add(c.Header, c.Converter.RelutingType);
                        }
                        else
                        {
                            d.Columns.Add(c);
                        }
                    }
                    else
                    {
                        d.Columns.Add(String.Format("{0}.{1}", c.Header, c.Converter.ColumnNames[i]),
                            c.Converter.RelutingTypes[i]);
                    }
                }
            }

            foreach (InternalRow r in Rows)
            {
                Row n = d.Rows.Add();

                int idx = 0;
                for (int i = 0; i < r.Count; i++)
                {
                    foreach (object ro in Columns[i].Converter.GetValues(r[i]))
                    {
                        n[idx++] = ro;
                    }
                }
            }

            return d;
        }

        public void ExportExcelToWorkSheet(OleObject worksheet, int startRow, int startCol, bool exportHeader)
        {
            ExportExcelToWorkSheet(worksheet, startRow, startCol, exportHeader, int.MaxValue);
        }
        public void ExportExcelToWorkSheet(OleObject worksheet, int startRow, int startCol, bool exportHeader, int maxRow)
        {
            int col = startCol;
            if (exportHeader)
            {
                foreach (Column c in Columns)
                {
                    worksheet.GetProperty("Cells", startRow, col).GetProperty("Font").SetProperty("Bold", true);
                    worksheet.SetProperty("Cells", startRow, col++, c.Header);
                }
                startRow++;
            }

            int curRow = 0;
            foreach (InternalRow r in Rows)
            {
                if (curRow >= maxRow)
                    break;

                col = startCol;
                foreach (object o in r)
                {
                    object res;

                    Type t = Columns[col - startCol].ColumnType;
                    Type rt = Columns[col - startCol].Converter.RelutingType;
                    if (rt != null)
                    {
                        if (rt == typeof(string))
                        {
                            res = "'" + (string)Columns[col - startCol].Converter.GetValue(o);
                        }
                        else if (!(IsTypeInts(rt) || IsTypeReals(rt) || IsTypeExtra(rt)))
                        {
                            res = "'" + Columns[col - startCol].Converter.GetString(o);
                        }
                        else
                        {
                            res = Columns[col - startCol].Converter.GetValue(o);
                        }
                    }
                    else if (!(IsTypeInts(t) || IsTypeReals(t) || IsTypeExtra(t)))
                    {
                        res = "'" + Columns[col - startCol].Converter.GetString(o);
                    }
                    else
                    {
                        res = o;
                    }

                    worksheet.SetProperty("Cells", startRow, col++, res);

                }

                startRow++;
                curRow++;
            }
        }

        static public DataGrid combineHorizon(string header, string[] preRow, DataGrid[] all)
        {
            DataGrid.Column[] cs = { new DataGrid.Column(header) };
            DataGrid.DataRow[] rows = new DataGrid.DataRow[preRow.Length];

            for (int i = 0; i < preRow.Length; i++)
            {
                rows[i] = new DataGrid.DataRow(1);
                rows[i][0] = preRow[i];
            }

            return combineHorizon(cs, rows, all);
        }

        static public DataGrid combineHorizon(DataGrid.Column[] ciPre, DataGrid.Row[] rPre, DataGrid[] all)
        {
            DataGrid d = new DataGrid();
            int extraColumn = 0;
            if (ciPre != null)
            {
                foreach (DataGrid.Column c in ciPre)
                {
                    d.Columns.Add(c); extraColumn++;
                }
            }

            int columns = 0;
            int idx = 0;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].ColumnCount > columns)
                {
                    columns = all[i].ColumnCount;
                    idx = i;
                }
            }

            foreach (DataGrid.Column cc in all[idx].Columns)
                d.Columns.Add(cc);

            for (int i = 0; i < all.Length; i++)
            {
                for (int l = 0; l < all[i].RowCount; l++)
                {
                    DataGrid.DataRow r = new DataGrid.DataRow(extraColumn + all[i].ColumnCount);
                    for (int j = 0; j < extraColumn; j++)
                        r[j] = rPre[i][j];

                    for (int j = 0; j < all[i].ColumnCount; j++)
                        r[j + extraColumn] = all[i].Rows[l][j];

                    d.Rows.Add(r);
                }
            }

            return d;
        }
    }
}
