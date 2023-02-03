using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Runtime.InteropServices;
using System.Text;

namespace corelib
{
    public abstract class DataCartogram : AbstactTupleItem, IPvkSink, ITupleItem, corelib.IDataCartogram
    {
        public abstract class IndexedDataCartogram : DataCartogram
        {
            internal DataArray _indexes;
            internal Coords[] _indexedCoords;

            protected IndexedDataCartogram(TupleMetaData info)
                : base(info)
            {
            }

            public override bool IsValidCoord(Coords c)
            {
                return GetIndexNoThrow(c) != -1;
            }


            public int GetIndexNoThrow(Coords c)
            {
                for (int i = 0; i < _indexedCoords.Length; i++)
                {
                    if (_indexedCoords[i] == c)
                        return i;
                }
                return -1;

            }

            public int GetIndex(Coords c)
            {
                for (int i = 0; i < _indexedCoords.Length; i++)
                {
                    if (_indexedCoords[i] == c)
                        return i;
                }
                throw new Exception("Can't find coords!");
            }


            internal static IndexedDataCartogram CreateIndexedTyped(TupleMetaData info, CartogramType type)
            {
                IndexedDataCartogram dc;

                switch (type)
                {
                    case CartogramType.Indexed:
                        dc = new DataCartogramIndexed(info);
                        break;
                    case CartogramType.IndexedMultiValue:
                        dc = new DataCartogramIdxMultiValue(info);
                        break;
                    default:
                        throw new Exception("Unknown convert type!");
                }
                return dc;
            }

            public override DataCartogram Clone(string name, string humanName, DateTime date)
            {
                IndexedDataCartogram dci = CreateIndexedTyped(new TupleMetaData(name, humanName, date), CartType);
                dci.InitBase(this.ElementType, this.ScaleIndex);

                dci._pvk = _pvk;
                dci._main = _main.Clone(name, humanName, date);

                dci._indexes = _indexes.Clone(_indexes.GetName(), _indexes.GetHumanName(), _indexes.GetTimeDate());
                dci._indexedCoords = dci._indexes.GetArrayCoords();
                return dci;
            }

            public override ITupleItem Rename(string newParamName)
            {
                IndexedDataCartogram dci = CreateIndexedTyped(
                    new TupleMetaData(newParamName, HumaneName, Date), CartType);
                dci.InitBase(this.ElementType, this.ScaleIndex);

                dci._pvk = _pvk;
                dci._main = _main;

                dci._indexes = _indexes;
                dci._indexedCoords = _indexedCoords;
                return dci;
            }

            public override ITupleItem Rename(TupleMetaData newInfo)
            {
                IndexedDataCartogram dci = CreateIndexedTyped(
                    newInfo, CartType);
                dci.InitBase(this.ElementType, this.ScaleIndex);

                dci._pvk = _pvk;
                dci._main = _main;

                dci._indexes = _indexes;
                dci._indexedCoords = _indexedCoords;
                return dci;
            }
        }





        #region IChronoSerializer Members

        public override void Serialize(ISerializeStream stream)
        {
            int idx;
            if (this as DataCartogramLinear != null)
                idx = DataCartogramLinear.StreamSerializerId;
            else if (this as DataCartogramLinearWide != null)
                idx = DataCartogramLinearWide.StreamSerializerId;
            else if (this as DataCartogramNative != null)
                idx = DataCartogramNative.StreamSerializerId;
            else if (this as DataCartogramIndexed != null)
                idx = DataCartogramIndexed.StreamSerializerId;
            else if (this as DataCartogramPvk != null)
                idx = DataCartogramPvk.StreamSerializerId;
            else if (this as DataCartogramIdxMultiValue != null)
                idx = DataCartogramIdxMultiValue.StreamSerializerId;
            else
                throw new Exception("Unknown instance");

            stream.Put(idx);
            _main.Serialize(stream);
            stream.PutStruct(_scale);

            IndexedDataCartogram dci = this as IndexedDataCartogram;
            if (dci != null)
            {
                dci._indexes.Serialize(stream);
            }

        }

        public ISerializeStream Serialize()
        {
            StreamSerializer sr = new StreamSerializer(Info);
            Serialize(sr);
            return sr;
        }
        #endregion

        #region IPvkSink Members

        public void SetPvkInfo(IPvkInfo pvk)
        {
            if (_pvk == null)
                _pvk = pvk;
            /* else
                 if (_pvk != pvk)
                     throw new Exception("Pvk sheme already has been set");
             */
        }

        #endregion

        public IPvkInfo PvkInfo
        {
            get { return _pvk; }
        }

        public String DumpCSV()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Coords c in AllCoords)
            {
                if (!c.IsOk)
                    continue;

                if (this as DataCartogramIdxMultiValue != null)
                {
                    sb.AppendFormat("{0}", c);
                    Array a = (Array)GetItem(c);
                    for (int i = 0; i < a.Length; i++)
                    {
                        sb.AppendFormat(";{0}", _scale.Scale(a.GetValue(i)));
                    }
                }
                else
                    sb.AppendFormat("{0};{1}", c, this[c]);

                if (_pvk != null)
                {
                    FiberCoords fc = _pvk.GetByCoords(c);
                    if (fc.IsValid)
                        sb.AppendFormat(";{0};{1}", fc.Fiber, fc.Pvk);
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        public static DataCartogram Create(IDeserializeStream ds)
        {
            int idx;
            DataCartogram dc;

            ds.Get(out idx);            

            if (idx == DataCartogramLinear.StreamSerializerId)
                dc = new DataCartogramLinear(ds.Info);
            else if (idx == DataCartogramLinearWide.StreamSerializerId)
                dc = new DataCartogramLinearWide(ds.Info);
            else if (idx == DataCartogramNative.StreamSerializerId)
                dc = new DataCartogramNative(ds.Info);
            else if (idx == DataCartogramIndexed.StreamSerializerId)
                dc = new DataCartogramIndexed(ds.Info);
            else if (idx == DataCartogramPvk.StreamSerializerId)
                dc = new DataCartogramPvk(ds.Info);
            else if (idx == DataCartogramIdxMultiValue.StreamSerializerId)
                dc = new DataCartogramIdxMultiValue(ds.Info);
            else
                throw new Exception("Unknown id instance");

            dc._main = new DataArray(ds);
            ScaleIndex scale = (ScaleIndex)ds.GetStruct(typeof(ScaleIndex));

            dc.InitBase(dc._main.ElementType, scale);

            if ((idx == DataCartogramIndexed.StreamSerializerId) ||
                (idx == DataCartogramIdxMultiValue.StreamSerializerId))
            {
                IndexedDataCartogram dci = (IndexedDataCartogram)dc;
                dci._indexes = new DataArray(ds);
                dci._indexedCoords = dci._indexes.GetArrayCoords();
            }

            return dc;
        }

        /// <summary>
        /// Создает разницу result - source
        /// </summary>
        /// <param name="result"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataCartogram AbsoluteDiff(DataCartogram result, DataCartogram source)
        {
            //DataCartogram c = CreateNativeDouble("absolute_diff", String.Format("Абосютная разница {0}", result.GetHumanName()), DateTime.Now);
            DataCartogram c = CreateDoubleResultLogicCart(result, source,
                new TupleMetaData("absolute_diff", String.Format("Абосютная разница {0}", result.GetHumanName()), DateTime.Now));
            foreach (Coords crd in c.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = result.GetItemSafe(crd);
                    object o2 = source.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < result.Layers; l++)
                        {
                            double diff = result[crd, l] - source[crd, l];
                            c[crd, l] = diff;
                        }
                    }
                }
            }
            return c;
        }

        public IDataCartogram RelativeDiff(IDataCartogram source)
        {
            return DataCartogram.RelativeDiff(this, (DataCartogram)source);
        }

        public IDataCartogram AbsoluteDiff(IDataCartogram source)
        {
            return DataCartogram.AbsoluteDiff(this, (DataCartogram)source);
        }

        static DataCartogram CreateDoubleResultLogicCart(DataCartogram curr, DataCartogram prev, TupleMetaData info)
        {
            DataCartogram dc;

            if (((curr.CartType == CartogramType.Indexed) || (curr.CartType == CartogramType.PVK)) &&
                ((prev.CartType == CartogramType.Linear) || (prev.CartType == CartogramType.LinearWide) || (prev.CartType == CartogramType.Native)))
            {
                if (curr.CartType == CartogramType.Indexed)
                {
                    DataCartogramIndexed dci = new DataCartogramIndexed(info);
                    dci.InitIndex(typeof(double), ScaleIndex.Default,
                        new DataArray("coords", "coords", DateTime.Now, ((IndexedDataCartogram)curr)._indexedCoords));
                    dc = dci;
                }
                else
                {
                    dc = DataCartogram.CreateTyped(info, CartogramType.PVK);
                    dc.Init(typeof(double), ScaleIndex.Default, null);
                }
            }
            else if (((prev.CartType == CartogramType.Indexed) || (prev.CartType == CartogramType.PVK)) &&
                ((curr.CartType == CartogramType.Linear) || (curr.CartType == CartogramType.LinearWide) || (curr.CartType == CartogramType.Native)))
            {
                return CreateDoubleResultLogicCart(prev, curr, info);
            }
            else if (((curr.CartType == CartogramType.Indexed) && (prev.CartType == CartogramType.Indexed)) ||
           ((curr.CartType == CartogramType.IndexedMultiValue) && (prev.CartType == CartogramType.IndexedMultiValue)))
            {
                IndexedDataCartogram dci_curr = (IndexedDataCartogram)curr;
                IndexedDataCartogram dci_prev = (IndexedDataCartogram)prev;

                bool check_similarity = true;
                if (dci_curr._indexedCoords != dci_prev._indexedCoords)
                {
                    for (int i = 0; i < dci_curr.AllCoordsCount; i++)
                    {
                        if ((dci_curr._indexedCoords[i] != dci_prev._indexedCoords[i]) &&
                            (dci_curr._indexedCoords[i].IsOk && dci_prev._indexedCoords[i].IsOk))
                        {
                            check_similarity = false;
                            break;
                        }
                    }
                }

                if (check_similarity)
                {
                    //IndexedDataCartogram dci = IndexedDataCartogram.CreateIndexedTyped(curr.CartType);
                    //dci.InitIndex(name, help, new DateTime(), typeof(double), new ScaleIndex(0, 1),
                    //    new DataArray("coords", "coords", DateTime.Now, dci_curr._indexedCoords));

                    dc = CreateDouble(info.Name, info.HumaneName, info.Date, curr);
                }
                else
                    dc = CreateNativeDouble(info.Name, info.HumaneName, info.Date);
            }
            else
            {
                dc = CreateNativeDouble(info.Name, info.HumaneName, info.Date);
            }
            dc._pvk = (curr._pvk != null) ? curr._pvk : prev._pvk;
            return dc;
        }

        public static DataCartogram RelativeDiff(DataCartogram result, DataCartogram source)
        {
            DataCartogram c = CreateDoubleResultLogicCart(result, source, 
                new TupleMetaData("releative_diff", String.Format("Относительная разница {0}", result.GetHumanName()), DateTime.Now));

            foreach (Coords crd in c.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = result.GetItemSafe(crd); //GetItemLayerSafe
                    object o2 = source.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < result.Layers; l++)
                        {
                            double diff = 0;
                            if (source[crd, l] != 0)
                                diff = (result[crd, l] - source[crd, l]) / source[crd, l];
                            c[crd, l] = diff;
                        }
                    }
                }
            }

            return c;
        }

        public double GetScaledMaxMin(out double min, out double max)
        {
            max = min = 0;
            bool first = true;
            foreach (Coords crd in AllCoords)
            {
                if (crd.IsOk)
                {
                    for (int l = 0; l < Layers; l++)
                    {
                        double diff = this[crd, l];
                        if (first)
                        {
                            min = max = diff;
                            first = false;
                        }
                        else
                        {
                            min = Math.Min(min, diff);
                            max = Math.Max(max, diff);
                        }
                    }
                }
            }
            return Math.Max(Math.Abs(min), Math.Abs(max));
        }

        public static DataCartogram operator +(DataCartogram c, double val)
        {
            DataCartogram ret = CreateDouble(c);
            foreach (Coords crd in c.AllCoords)
            {
                if (crd.IsOk)
                {
                    for (int l = 0; l < c.Layers; l++)
                    {
                        double res = c[crd, l] + val;

                        ret[crd, l] = res;
                    }
                }
            }
            return ret;
        }
        public static DataCartogram operator -(DataCartogram c, double val)
        {
            return c + (-val);
        }

        public static DataCartogram operator *(DataCartogram c, double val)
        {
            DataCartogram ret = CreateDouble(c);
            foreach (Coords crd in c.AllCoords)
            {
                if (crd.IsOk)
                {
                    for (int l = 0; l < c.Layers; l++)
                    {
                        double res = c[crd, l] * val;

                        ret[crd, l] = res;
                    }
                }
            }
            return ret;
        }
        public static DataCartogram operator /(DataCartogram c, double val)
        {
            return c * (1 / val);
        }

        public static DataCartogram operator +(DataCartogram c, DataCartogram d)
        {
            DataCartogram ret = CreateDoubleResultLogicCart(c, d, new TupleMetaData("temp", "temp", DateTime.Now));

            foreach (Coords crd in ret.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = c.GetItemSafe(crd); //GetItemLayerSafe
                    object o2 = d.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < ret.Layers; l++)
                        {
                            ret[crd, l] = c[crd, l] + d[crd, l];
                        }
                    }
                }
            }

            return ret;
        }

        public static DataCartogram operator -(DataCartogram c, DataCartogram d)
        {
            DataCartogram ret = CreateDoubleResultLogicCart(c, d, new TupleMetaData("temp", "temp", DateTime.Now));

            foreach (Coords crd in ret.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = c.GetItemSafe(crd); //GetItemLayerSafe
                    object o2 = d.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < ret.Layers; l++)
                        {
                            ret[crd, l] = c[crd, l] - d[crd, l];
                        }
                    }
                }
            }

            return ret;
        }

        public static DataCartogram operator *(DataCartogram c, DataCartogram d)
        {
            DataCartogram ret = CreateDoubleResultLogicCart(c, d, new TupleMetaData("temp", "temp", DateTime.Now));

            foreach (Coords crd in ret.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = c.GetItemSafe(crd); //GetItemLayerSafe
                    object o2 = d.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < ret.Layers; l++)
                        {
                            ret[crd, l] = c[crd, l] * d[crd, l];
                        }
                    }
                }
            }

            return ret;
        }

        public static DataCartogram operator /(DataCartogram c, DataCartogram d)
        {
            DataCartogram ret = CreateDoubleResultLogicCart(c, d, new TupleMetaData("temp", "temp", DateTime.Now));

            foreach (Coords crd in ret.AllCoords)
            {
                if (crd.IsOk)
                {
                    object o1 = c.GetItemSafe(crd); //GetItemLayerSafe
                    object o2 = d.GetItemSafe(crd);

                    if ((o1 != null) && (o2 != null))
                    {
                        for (int l = 0; l < ret.Layers; l++)
                        {
                            ret[crd, l] = c[crd, l] / d[crd, l];
                        }
                    }
                }
            }

            return ret;
        }

        public static DataCartogram CreateNativeDouble(string name, string humaneName, DateTime dt)
        {
            return Create(new DataArray(name, humaneName, dt, typeof(double), 48, 48, 0));
        }

        public static DataCartogram Create(string name, string humaneName, DateTime dt, Array data)
        {
            return Create(new DataArray(name, humaneName, dt, data));
        }

        public static DataCartogram Create(string name, DateTime dt, Array data)
        {
            return Create(new DataArray(name, name, dt, data));
        }


        public static DataCartogram Create(DataCartogram from)
        {
            return Create("temp", from);
        }

        public static DataCartogram Create(string name, DataCartogram from)
        {
            return Create(name, from.GetTimeDate(), from);
        }

        public static DataCartogram Create(string name, DateTime dt, DataCartogram from)
        {
            return Create(name, name, dt, from);
        }

        public static DataCartogram Create(string name, string humaneName, DateTime dt, DataCartogram from)
        {
            DataCartogram dc;
            IndexedDataCartogram dci;
            IndexedDataCartogram fromi = from as IndexedDataCartogram;
            if (fromi != null)
            {
                dci = IndexedDataCartogram.CreateIndexedTyped(new TupleMetaData(name, humaneName, dt), 
                    from.CartType);
                dci._indexes = fromi._indexes;
                dci._indexedCoords = fromi._indexedCoords;
                dc = dci;
            }
            else
            {
                dc = CreateTyped(new TupleMetaData(name, humaneName, dt), 
                    from.CartType);
            }

            dc._main = DataArray.Create(name, humaneName, dt, from.Array);
            dc.InitBase(from.ElementType, from.ScaleIndex);

            dc._pvk = from._pvk;
            return dc;
        }

        public static DataCartogram CreateDouble(DataCartogram from)
        {
            return CreateDouble("temp", from);
        }
        public static DataCartogram CreateDouble(string name, DataCartogram from)
        {
            return CreateDouble(name, from.GetTimeDate(), from);
        }
        public static DataCartogram CreateDouble(string name, DateTime dt, DataCartogram from)
        {
            return CreateDouble(name, name, dt, from);
        }
        public static DataCartogram CreateDouble(string name, string humaneName, DateTime dt, DataCartogram from)
        {
            DataCartogram dc;
            IndexedDataCartogram dci;
            IndexedDataCartogram fromi = from as IndexedDataCartogram;
            if (fromi != null)
            {
                dci = IndexedDataCartogram.CreateIndexedTyped(
                    new TupleMetaData(name, humaneName, dt), from.CartType);
                dci._indexes = fromi._indexes;
                dci._indexedCoords = fromi._indexedCoords;
                dc = dci;
            }
            else
            {
                dc = CreateTyped(
                    new TupleMetaData(name, humaneName, dt), from.CartType);
            }

            dc._main = DataArray.CreateDouble(name, humaneName, dt, from.Array);
            dc.InitBase(dc.ElementType, ScaleIndex.Default);

            dc._pvk = from._pvk;
            return dc;
        }

        public static DataCartogram Create(DataArray ar)
        {
            return Create(ar, ScaleIndex.Default);
        }

        public static DataCartogram Create(DataArray ar, ScaleIndex scale)
        {
            DataCartogram dc;
            CartogramType t;

            if ((ar.Rank == 1) && (ar.DimX == 1884))
                t = CartogramType.Linear;
            else if ((ar.Rank == 1) && (ar.DimX == 2448))
                t = CartogramType.LinearWide;
            else if ((ar.Rank == 2) && (ar.DimX == 48) && (ar.DimY == 48))
                t = CartogramType.Native;
            else if ((ar.Rank == 2) && (ar.DimX == 16) && (ar.DimY == 115))
                t = CartogramType.PVK;
            else
                throw new Exception("Unknown type");

            dc = CreateTyped(ar.Info, t);
            dc._main = ar;
            dc.InitBase(ar.ElementType, scale);

            return dc;
        }

        public static DataCartogram Create(DataArray ar, ScaleIndex scale, Coords[] coords)
        {
            if ((ar.DimY == 0) && (ar.DimZ == 0) && (ar.Length == coords.Length))
            {
                DataCartogramIndexed dci = new DataCartogramIndexed(ar.Info);
                DataArray nindex = new DataArray(String.Format("coords_{0}", ar.GetName()),
                                    String.Format("Координаты для {0}", ar.GetHumanName()), ar.GetTimeDate(), coords);

                dci.InitIndex(ar.ElementType, scale, nindex, ar);
                //dci._main = ar;

                return dci;
            }
            else if ((ar.DimZ == 0) && (ar.DimX == coords.Length))
            {
                DataCartogramIdxMultiValue dci = new DataCartogramIdxMultiValue(ar.Info);
                DataArray nindex = new DataArray(String.Format("coords_{0}", ar.GetName()),
                                    String.Format("Координаты для {0}", ar.GetHumanName()), ar.GetTimeDate(), coords);

                dci.InitIndex(ar.GetName(), ar.GetHumanName(), ar.GetTimeDate(), ar.ElementType, scale, nindex, ar.DimY, ar);
                //dci._main = ar;

                return dci;
            }
            else
            {
                throw new Exception("Incorrect argument");
            }
        }





        #region ITupleItem Members

        ITupleItem ITupleItem.CastTo(ITypeRules rules)
        {
            if (rules.GetTypeString().ToLower() != "cart")
                throw new Exception("Don't know how to cast");

            return CastTo(rules);
        }

        ITupleItem ITupleItem.Clone(string name, string humanName, DateTime date)
        {
            return Clone(name, humanName, date);
        }

        #endregion



        public DataCartogram CastTo(ITypeRules rules)
        {
            ParsedCartogramRules cr = new ParsedCartogramRules(this, rules);
            CartogramType nFormat = cr.CartType;
            Type nType = cr.DataType;
            ScaleIndex nSc = cr.Scale;

            if ((nFormat == CartType) &&
                (nType == ElementType) &&
                (nSc == _scale))
            {
                return this;
            }

            DataCartogram c = Convert(nFormat);
            return c.ToType(nType, nSc);
        }

        public virtual DataCartogram Clone(string name, string humanName, DateTime date)
        {
            DataCartogram t = CreateTyped(new TupleMetaData(name, humanName, date), CartType);
            t.InitBase(ElementType, ScaleIndex);

            t._pvk = _pvk;
            t._main = _main.Clone(name, humanName, date);
            return t;
        }

        virtual public int Layers
        {
            get
            {
                return 1;
            }
        }

        public double this[Coords crd, int layer]
        {
            get
            {
                return _scale.Scale(GetItemLayer(crd, layer));
            }
            set
            {
                SetItemLayer(crd, _scale.Rescale(value, _main.ElementType), layer);
            }
        }


        // TODO Optimize with IsDouble
        public double this[Coords crd]
        {
            get
            {
                return _scale.Scale(GetItem(crd));
            }
            set
            {
                SetItem(crd, _scale.Rescale(value, _main.ElementType));
            }
        }

        public virtual double this[FiberCoords crd]
        {
            get
            {
                return _scale.Scale(GetItem(crd));
            }
        }

        public virtual object GetItemLayer(Coords crd, int layer)
        {
            return GetItem(crd);
        }

        public virtual object GetItemLayerSafe(Coords crd, int layer)
        {
            return GetItemSafe(crd);
        }

        public virtual object GetItemLayer(FiberCoords crd, int layer)
        {
            return GetItem(crd);
        }

        public virtual object GetItemLayerSafe(FiberCoords crd, int layer)
        {
            return GetItemSafe(crd);
        }

        public object GetObject(Coords c, int layer)
        {
            return GetItemLayer(c, layer);
        }

        public abstract object GetItem(Coords crd);
        public abstract object GetItemSafe(Coords crd);

        public abstract int GetItemInt(Coords crd);
        public abstract short GetItemShort(Coords crd);
        public abstract byte GetItemByte(Coords crd);
        public abstract float GetItemFloat(Coords crd);
        public abstract double GetItemDouble(Coords crd);
        public abstract Sensored GetItemAdcVal(Coords crd);
        public abstract FiberCoords GetItemFiberCoords(Coords crd);

        internal abstract void SetItem(Coords crd, object val);

        public virtual bool IsValidCoord(Coords c)
        {
            return c.IsOk;
        }

        public virtual bool IsNative
        {
            get
            {
                return false;
            }
        }

        internal virtual void SetItemLayer(Coords crd, object val, int layer)
        {
            SetItem(crd, val);
        }

        #region Get_ for FiberCoords
        public virtual object GetItem(FiberCoords crd)
        {
            return GetItem(_pvk.GetByPvk(crd));
        }
        public virtual object GetItemSafe(FiberCoords crd)
        {
            if (crd.IsValid && _pvk.GetByPvk(crd).IsOk)
                return GetItemSafe(_pvk.GetByPvk(crd));

            return null;
        }

        public virtual int GetItemInt(FiberCoords crd)
        {
            return GetItemInt(_pvk.GetByPvk(crd));
        }
        public virtual short GetItemShort(FiberCoords crd)
        {
            return GetItemShort(_pvk.GetByPvk(crd));
        }
        public virtual byte GetItemByte(FiberCoords crd)
        {
            return GetItemByte(_pvk.GetByPvk(crd));
        }
        public virtual float GetItemFloat(FiberCoords crd)
        {
            return GetItemFloat(_pvk.GetByPvk(crd));
        }
        public virtual double GetItemDouble(FiberCoords crd)
        {
            return GetItemDouble(_pvk.GetByPvk(crd));
        }
        public virtual Sensored GetItemAdcVal(FiberCoords crd)
        {
            return GetItemAdcVal(_pvk.GetByPvk(crd));
        }
        public virtual FiberCoords GetItemFiberCoords(FiberCoords crd)
        {
            return crd;
        }

        public virtual void SetItem(FiberCoords crd, object val)
        {
            SetItem(_pvk.GetByPvk(crd), val);
        }

        #endregion

        public abstract CartogramType CartType
        {
            get;
        }


        public Type ElementType
        {
            get { return _main.ElementType; }
        }

        internal DataArray Array
        {
            get { return _main; }
        }

        #region IsType checking
        public bool IsByte
        {
            get { return _main.IsByte; }
        }
        public bool IsShort
        {
            get { return _main.IsShort; }
        }
        public bool IsInt
        {
            get { return _main.IsInt; }
        }
        public bool IsDouble
        {
            get { return _main.IsDouble; }
        }
        public bool IsFloat
        {
            get { return _main.IsFloat; }
        }
        public bool IsSensored
        {
            get { return _main.IsSensored; }
        }
        public bool IsCoords
        {
            get { return _main.IsCoords; }
        }
        public bool IsFiberCoords
        {
            get { return _main.IsFiberCoords; }
        }

        #endregion

        #region RawArray Properties

        public int[,] RawArrayInt
        {
            get { return _main.GetArrayInt2(); }
        }

        public short[,] RawArrayShort
        {
            get { return _main.GetArrayShort2(); }
        }

        public byte[,] RawArrayByte
        {
            get { return _main.GetArrayByte2(); }
        }

        public float[,] RawArrayFloat
        {
            get { return _main.GetArrayFloat2(); }
        }

        public double[,] RawArrayDouble
        {
            get { return _main.GetArrayDouble2(); }
        }

        public int[] RawLinerInt
        {
            get { return _main.GetArrayInt(); }
        }

        public short[] RawLinerShort
        {
            get { return _main.GetArrayShort(); }
        }

        public byte[] RawLinerByte
        {
            get { return _main.GetArrayByte(); }
        }

        public float[] RawLinerFloat
        {
            get { return _main.GetArrayFloat(); }
        }

        public double[] RawLinerDouble
        {
            get { return _main.GetArrayDouble(); }
        }


        public static explicit operator int[,](DataCartogram s)
        {
            return (int[,])s._main;
        }
        public static explicit operator byte[,](DataCartogram s)
        {
            return (byte[,])s._main;
        }

        public static implicit operator double[](DataCartogram s)
        {
            return (double[])s._main;
        }
        public static implicit operator float[](DataCartogram s)
        {
            return (float[])s._main;
        }
        public static implicit operator double[,](DataCartogram s)
        {
            return (double[,])s._main;
        }
        public static implicit operator float[,](DataCartogram s)
        {
            return (float[,])s._main;
        }

        #endregion

        internal abstract void Init(Type elementType, ScaleIndex scale, Object exParam);

        protected DataCartogram(TupleMetaData info)
            : base(info)
        {
        }
        

        static readonly IInfoFormatter sDefDouble = new CartViewInfoProviderDouble(ScaleIndex.Default);
        static readonly IInfoFormatter sDefSensored = new CartViewInfoProviderSensored(ScaleIndex.Default);

        static readonly IInfoFormatter sDefStandard = new CartViewInfoProviderStandard(ScaleIndex.Default);

        protected void InitBase(Type elementType, ScaleIndex scale)
        {
            _scale = scale;

            if (elementType == typeof(Sensored))
            {
                if (_scale.IsDefault)
                    _formatter = sDefSensored;
                else
                    _formatter = new CartViewInfoProviderSensored(_scale);
            }
            else if (elementType == typeof(double))
            {
                if (_scale.IsDefault)
                    _formatter = sDefDouble;
                else
                    _formatter = new CartViewInfoProviderDouble(_scale);

            }
            else
            {
                if (_scale.IsDefault)
                    _formatter = sDefStandard;
                else
                    _formatter = new CartViewInfoProviderStandard(_scale);
            }
        }

        public double ScaleToDouble(object item)
        {
            return _scale.Scale(item);
        }

#if DOTNET_V11
        public IEnumerable AllCoords
        {
            get
            {
                if (CartType == CartogramType.Indexed)
                    return ((DataCartogramIndexed)this)._indexedCoords;
                else if (CartType == CartogramType.IndexedMultiValue)
                    return ((DataCartogramIdxMultiValue)this)._indexedCoords;
                else if (CartType == CartogramType.PVK)
                {
                    if (_pvk != null)
                        return _pvk.ValuableCoords;
                    else
                        return null;
                }
                else
                    return Coords.coreCoords;
            }
        }
#else
        public IEnumerable<Coords> AllCoords
        {
            get
            {
                if (CartType == CartogramType.Indexed)
                    return ((IndexedDataCartogram)this)._indexedCoords;
                else if (CartType == CartogramType.IndexedMultiValue)
                    return ((IndexedDataCartogram)this)._indexedCoords;

                else if (CartType == CartogramType.PVK)
                {
                    if (_pvk != null)
                        return _pvk.ValuableCoords;
                    else
                        return null;
                }
                /* else if (CartType == CartogramType.LinearWide)
                    return Coords.coreCoordsWide; */
                else
                    return Coords.coreCoords;

            }
        }
#endif

        public int AllCoordsCount
        {
            get
            {
                if (CartType == CartogramType.Indexed)
                    return ((IndexedDataCartogram)this)._indexedCoords.Length;
                else if (CartType == CartogramType.IndexedMultiValue)
                    return ((IndexedDataCartogram)this)._indexedCoords.Length;
                else if (CartType == CartogramType.PVK)
                    return _pvk.ValuableCoords.Length;
                else
                    return Coords.coreCoords.Length;
            }
        }

        //Should be optimized
        public DataCartogram ToDouble()
        {
            return ToType(typeof(double), ScaleIndex.Default);
        }
        public DataCartogram ToFloat()
        {
            return ToType(typeof(float), ScaleIndex.Default);
        }

        public DataCartogram ToType(Type ntype, ScaleIndex si)
        {
            if (ElementType == ntype)
                return this;

            DataCartogram dc;
            if (CartType == CartogramType.Indexed)
            {
                DataCartogramIndexed idc = new DataCartogramIndexed(Info);
                idc.InitIndex(ntype, si,
                    ((IndexedDataCartogram)this)._indexes);
                dc = idc;
            }
            else if (CartType == CartogramType.IndexedMultiValue)
            {
                throw new NotImplementedException();
            }
            else
            {
                dc = CreateTyped(Info, CartType);
                dc.Init(ntype, si, null);
            }

            dc._pvk = _pvk;

            foreach (Coords c in AllCoords)
            {
                dc[c] = this[c];
            }
            return dc;
        }

        public DataCartogram Convert(CartogramType type)
        {
            return Convert(type, null);
        }

        internal static DataCartogram CreateTyped(TupleMetaData info, CartogramType type)
        {
            DataCartogram dc;

            switch (type)
            {
                case CartogramType.Native:
                    dc = new DataCartogramNative(info);
                    break;
                case CartogramType.Linear:
                    dc = new DataCartogramLinear(info);
                    break;
                case CartogramType.LinearWide:
                    dc = new DataCartogramLinearWide(info);
                    break;
                case CartogramType.PVK:
                    dc = new DataCartogramPvk(info);
                    break;
                default:
                    throw new Exception("Unknown convert type!");
            }
            return dc;
        }

        // TODO Rewrite it with AllCoords
        public DataCartogram Convert(CartogramType type, object externalData)
        {
            if (CartType == type)
                return this;

            DataCartogram dc;

            if (type == CartogramType.Indexed)
            {
                Coords[] coords = externalData as Coords[];
                if (coords != null)
                {
                    DataCartogramIndexed dci = new DataCartogramIndexed(Info);
                    dci.InitIndex(ElementType, _scale,
                        new DataArray("auto_coords", "координаты", Date, coords));
                    dc = dci;
                }
                else
                    throw new Exception("It's needed PVK Indexed array to complete call");
            }
            else
            {
                dc = CreateTyped(Info, type);
                dc.Init(ElementType, _scale, null);
            }
            dc._pvk = _pvk;


            if ((type == CartogramType.Native) ||
                (type == CartogramType.Linear) ||
                (type == CartogramType.LinearWide))
            {
                if (CartType == CartogramType.Indexed)
                {
                    DataCartogramIndexed dci = this as DataCartogramIndexed;
                    //foreach (Coords c in dci._indexedCoords)
                    //    dc.SetItem(c, GetItem(c));
                    for (int i = 0; i < ((IndexedDataCartogram)dci)._indexedCoords.Length; i++)
                        dc.SetItem(((IndexedDataCartogram)dci)._indexedCoords[i], _main[i]);
                }
                else if (CartType == CartogramType.PVK)
                {
                    foreach (Coords c in _pvk.ValuableCoords)
                        dc.SetItem(c, GetItem(c));
                }
                else
                    foreach (Coords c in Coords.coreCoords)
                        dc.SetItem(c, GetItem(c));
            }
            else if (type == CartogramType.PVK)
            {
                if (dc._pvk == null)
                {
                    dc._pvk = externalData as IPvkInfo;
                }
                if (dc._pvk == null)
                    throw new PvkInfoNeededException(this);

                if (CartType == CartogramType.Indexed)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    foreach (Coords c in dc._pvk.ValuableCoords)
                        dc.SetItem(c, GetItem(c));
                }
            }
            else if (type == CartogramType.Indexed)
            {
                foreach (Coords c in dc.AllCoords)
                    dc.SetItem(c, GetItem(c));
            }

            return dc;
        }


        internal struct ParsedCartogramRules
        {
            internal readonly bool nFormatOk;
            internal readonly bool nTypeOk;
            internal readonly bool nScOk;
            internal readonly CartogramType nFormat;
            internal readonly Type nType;
            internal readonly ScaleIndex nSc;

            string _format;
            string _type;
            string _scale;

            public CartogramType CartType
            {
                get
                {
                    if (!nFormatOk)
                        throw new Exception("Unknown format");
                    return nFormat;
                }
            }
            public Type DataType
            {
                get
                {
                    if (!nTypeOk)
                        throw new Exception("Unknown format");
                    return nType;
                }
            }
            public ScaleIndex Scale
            {
                get
                {
                    if (!nScOk)
                        throw new Exception("Unknown format");
                    return nSc;
                }
            }

            public bool IsOk
            {
                get { return nScOk && nTypeOk && nFormatOk; }
            }

            static bool ToCartogramType(DataCartogram c, string format, out CartogramType nFormat)
            {
                switch (format)
                {
                    case "pvk":
                        nFormat = CartogramType.PVK;
                        break;
                    case "native":
                        nFormat = CartogramType.Native;
                        break;
                    case "linear":
                        nFormat = CartogramType.Linear;
                        break;
                    case "linearwide":
                        nFormat = CartogramType.LinearWide;
                        break;
                    case "original":
                        nFormat = c.CartType;
                        break;
                    default:
                        nFormat = c.CartType;
                        return false;
                }
                return true;
            }

            static bool ToElementType(DataCartogram c, string type, out Type nType)
            {
                switch (type)
                {
                    case "int":
                        nType = typeof(int);
                        break;
                    case "short":
                        nType = typeof(short);
                        break;
                    case "byte":
                        nType = typeof(byte);
                        break;
                    case "float":
                        nType = typeof(float);
                        break;
                    case "double":
                        nType = typeof(double);
                        break;

                    case "original":
                        nType = c.ElementType;
                        break;
                    default:
                        nType = null;
                        return false;
                }
                return true;
            }

            static bool ToScale(DataCartogram c, string scale, out ScaleIndex nSc)
            {
                switch (scale)
                {
                    case "scale":
                        nSc = new ScaleIndex(0, 1);
                        break;
                    case "original":
                        nSc = c._scale;
                        break;
                    default:
                        nSc = new ScaleIndex(0, 0);
                        return false;
                }
                return true;
            }

            public ParsedCartogramRules(DataCartogram c, ITypeRules rules)
            {
                // Format for details
                // .. Kart(PVK, float, scale)
                // .. Kart(original, float)
                // .. Kart(original, original) == no changes

                string[] details = rules.GetCastDetails();
                if (details == null)
                {
                    nFormatOk = nTypeOk = nScOk = true;
                    nFormat = c.CartType;
                    nType = c.ElementType;
                    nSc = c._scale;

                    _format = null;
                    _type = null;
                    _scale = null;
                }
                else
                {
                    _format = (details.Length > 0) ? details[0].ToLower() : "original";
                    _type = (details.Length > 1) ? details[1].ToLower() : "original";
                    _scale = (details.Length > 2) ? details[2].ToLower() : "original";

                    nFormatOk = ToCartogramType(c, _format, out nFormat);
                    nTypeOk = ToElementType(c, _type, out nType);
                    nScOk = ToScale(c, _scale, out nSc);
                }
            }
        }




        public enum CartogramType
        {
            Native,
            Linear,
            LinearWide,
            Indexed,
            PVK,
            IndexedMultiValue
        }

        protected IPvkInfo _pvk;

        protected DataArray _main;
        protected ScaleIndex _scale;

        public ScaleIndex ScaleIndex
        {
            get { return _scale; }
        }

        static public bool IsMyStreamSerializerId(int id)
        {
            if ((id == DataCartogramLinear.StreamSerializerId) ||
                (id == DataCartogramLinearWide.StreamSerializerId) ||
                (id == DataCartogramNative.StreamSerializerId) ||
                (id == DataCartogramPvk.StreamSerializerId) ||
                (id == DataCartogramIndexed.StreamSerializerId) ||
                (id == DataCartogramIdxMultiValue.StreamSerializerId))
            {
                return true;
            }
            return false;
        }

        class DataCartogramLinear : DataCartogram
        {
            public static readonly int StreamSerializerId = 31;

            #region Simple DataCartogram methods
            public override object GetItem(Coords crd)
            {
                return _main[crd.IdxLinear1884];
            }

            public override int GetItemInt(Coords crd)
            {
                return _main.GetArrayInt()[crd.IdxLinear1884];
            }

            public override short GetItemShort(Coords crd)
            {
                return _main.GetArrayShort()[crd.IdxLinear1884];
            }

            public override byte GetItemByte(Coords crd)
            {
                return _main.GetArrayByte()[crd.IdxLinear1884];
            }

            public override float GetItemFloat(Coords crd)
            {
                return _main.GetArrayFloat()[crd.IdxLinear1884];
            }

            public override double GetItemDouble(Coords crd)
            {
                return _main.GetArrayDouble()[crd.IdxLinear1884];
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                return _main.GetArraySensored()[crd.IdxLinear1884];
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                return _main.GetArrayFiberCoords()[crd.IdxLinear1884];
            }

            internal override void SetItem(Coords crd, object val)
            {
                _main[crd.IdxLinear1884] = val;
            }
            #endregion


            public override bool IsValidCoord(Coords c)
            {
                return c.IsOk && (c.IdxLinear1884 >= 0);
            }

            public override CartogramType CartType
            {
                get { return CartogramType.Linear; }
            }

            internal DataCartogramLinear(TupleMetaData info)
                : base(info)
            {
            }


            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                _main = new DataArray(Name, HumaneName, Date, elementType, 1884, 0, 0);
                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                int idx = crd.IdxLinear1884;
                if (idx < 0)
                    return null;
                else
                    return _main[idx];
            }

        }

        class DataCartogramLinearWide : DataCartogram
        {
            public static readonly int StreamSerializerId = 32;

            #region Simple DataCartogram methods
            public override object GetItem(Coords crd)
            {
                return _main[crd.IdxLinear2448];
            }

            public override int GetItemInt(Coords crd)
            {
                return _main.GetArrayInt()[crd.IdxLinear2448];
            }

            public override short GetItemShort(Coords crd)
            {
                return _main.GetArrayShort()[crd.IdxLinear2448];
            }

            public override byte GetItemByte(Coords crd)
            {
                return _main.GetArrayByte()[crd.IdxLinear2448];
            }

            public override float GetItemFloat(Coords crd)
            {
                return _main.GetArrayFloat()[crd.IdxLinear2448];
            }

            public override double GetItemDouble(Coords crd)
            {
                return _main.GetArrayDouble()[crd.IdxLinear2448];
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                return _main.GetArraySensored()[crd.IdxLinear2448];
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                return _main.GetArrayFiberCoords()[crd.IdxLinear2448];
            }

            internal override void SetItem(Coords crd, object val)
            {
                _main[crd.IdxLinear2448] = val;
            }
            #endregion


            public override bool IsValidCoord(Coords c)
            {
                return c.IdxLinear2448 >= 0;
            }

            public override CartogramType CartType
            {
                get { return CartogramType.LinearWide; }
            }

            internal DataCartogramLinearWide(TupleMetaData info)
                : base(info)
            {
            }

            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                _main = new DataArray(Name, HumaneName, Date, elementType, 2448, 0, 0);
                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                int idx = crd.IdxLinear2448;
                if (idx < 0)
                    return null;
                else
                    return _main[idx];
            }
        }

        class DataCartogramNative : DataCartogram
        {
            public static readonly int StreamSerializerId = 33;

            #region Simple DataCartogram methods
            public override object GetItem(Coords crd)
            {
                return _main[crd.Y, crd.X];
            }

            public override int GetItemInt(Coords crd)
            {
                return _main.GetArrayInt2()[crd.Y, crd.X];
            }

            public override short GetItemShort(Coords crd)
            {
                return _main.GetArrayShort2()[crd.Y, crd.X];
            }

            public override byte GetItemByte(Coords crd)
            {
                return _main.GetArrayByte2()[crd.Y, crd.X];
            }

            public override float GetItemFloat(Coords crd)
            {
                return _main.GetArrayFloat2()[crd.Y, crd.X];
            }

            public override double GetItemDouble(Coords crd)
            {
                return _main.GetArrayDouble2()[crd.Y, crd.X];
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                return _main.GetArraySensored2()[crd.Y, crd.X];
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                return _main.GetArrayFiberCoords2()[crd.Y, crd.X];
            }

            internal override void SetItem(Coords crd, object val)
            {
                _main[crd.Y, crd.X] = val;
            }
            #endregion


            public override CartogramType CartType
            {
                get { return CartogramType.Native; }
            }

            internal DataCartogramNative(TupleMetaData info)
                : base(info)
            {
            }

            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                _main = new DataArray(Name, HumaneName, Date, elementType, 48, 48, 0);
                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                if (crd.IsOk)
                    return _main[crd.Y, crd.X];
                else
                    return null;
            }

            public override bool IsNative
            {
                get
                {
                    return true;
                }
            }
        }

        protected class DataCartogramIndexed : IndexedDataCartogram
        {
            public static readonly int StreamSerializerId = 34;

            #region Simple DataCartogram methods
            public override object GetItem(Coords crd)
            {
                return _main[base.GetIndex(crd)];
            }

            public override int GetItemInt(Coords crd)
            {
                return _main.GetArrayInt()[base.GetIndex(crd)];
            }

            public override short GetItemShort(Coords crd)
            {
                return _main.GetArrayShort()[base.GetIndex(crd)];
            }

            public override byte GetItemByte(Coords crd)
            {
                return _main.GetArrayByte()[base.GetIndex(crd)];
            }

            public override float GetItemFloat(Coords crd)
            {
                return _main.GetArrayFloat()[base.GetIndex(crd)];
            }

            public override double GetItemDouble(Coords crd)
            {
                return _main.GetArrayDouble()[base.GetIndex(crd)];
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                return _main.GetArraySensored()[base.GetIndex(crd)];
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                return _main.GetArrayFiberCoords()[base.GetIndex(crd)];
            }

            internal override void SetItem(Coords crd, object val)
            {
                _main[base.GetIndex(crd)] = val;
            }
            #endregion


            public override CartogramType CartType
            {
                get { return CartogramType.Indexed; }
            }

            internal DataCartogramIndexed(TupleMetaData info)
                : base(info)
            {
            }

            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                if (exParam.GetType() == typeof(DataArray))
                    InitIndex(elementType, scale, (DataArray)exParam);

                throw new Exception("Incorrect call");
            }

            internal void InitIndex(Type elementType, ScaleIndex scale, DataArray indexOfCoords)
            {
                base._indexes = indexOfCoords;
                base._indexedCoords = indexOfCoords.GetArrayCoords();

                _main = new DataArray(Name, HumaneName, Date, elementType, indexOfCoords.Length, 0, 0);
                InitBase(elementType, scale);
            }

            internal void InitIndex(Type elementType, ScaleIndex scale, DataArray indexOfCoords, DataArray data)
            {
                base._indexes = indexOfCoords;
                base._indexedCoords = indexOfCoords.GetArrayCoords();

                _main = data;
                if ((_main.Rank != 1) || (_main.DimX != indexOfCoords.Length))
                    throw new ArgumentException("data");

                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                int idx = base.GetIndexNoThrow(crd);
                if (idx < 0)
                    return null;
                else
                    return _main[idx];
            }

        }

        class DataCartogramPvk : DataCartogram
        {
            public static readonly int StreamSerializerId = 35;


            public override bool IsValidCoord(Coords c)
            {
                return c.IsOk && _pvk.GetByCoords(c).IsValid;
            }


            #region Simple DataCartogram methods
            public override object GetItem(Coords crd)
            {
                return _main[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override int GetItemInt(Coords crd)
            {
                return _main.GetArrayInt2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override short GetItemShort(Coords crd)
            {
                return _main.GetArrayShort2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override byte GetItemByte(Coords crd)
            {
                return _main.GetArrayByte2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override float GetItemFloat(Coords crd)
            {
                return _main.GetArrayFloat2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override double GetItemDouble(Coords crd)
            {
                return _main.GetArrayDouble2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                return _main.GetArraySensored2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                return _main.GetArrayFiberCoords2()[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk];
            }

            internal override void SetItem(Coords crd, object val)
            {
                _main[_pvk.GetByCoords(crd).Fiber, _pvk.GetByCoords(crd).Pvk] = val;
            }
            #endregion

            public override CartogramType CartType
            {
                get { return CartogramType.PVK; }
            }

            #region Override of FiberCoords ops
            public override object GetItem(FiberCoords crd)
            {
                return _main[crd.Fiber, crd.Pvk];
            }
            public override int GetItemInt(FiberCoords crd)
            {
                return _main.GetArrayInt2()[crd.Fiber, crd.Pvk];
            }
            public override short GetItemShort(FiberCoords crd)
            {
                return _main.GetArrayShort2()[crd.Fiber, crd.Pvk];
            }
            public override byte GetItemByte(FiberCoords crd)
            {
                return _main.GetArrayByte2()[crd.Fiber, crd.Pvk];
            }
            public override float GetItemFloat(FiberCoords crd)
            {
                return _main.GetArrayFloat2()[crd.Fiber, crd.Pvk];
            }
            public override double GetItemDouble(FiberCoords crd)
            {
                return _main.GetArrayDouble2()[crd.Fiber, crd.Pvk];
            }
            public override Sensored GetItemAdcVal(FiberCoords crd)
            {
                return _main.GetArraySensored2()[crd.Fiber, crd.Pvk];
            }
            public override FiberCoords GetItemFiberCoords(FiberCoords crd)
            {
                return _main.GetArrayFiberCoords2()[crd.Fiber, crd.Pvk];
            }
            #endregion

            internal DataCartogramPvk(TupleMetaData info)
                : base(info)
            {
            }

            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                _main = new DataArray(Name, HumaneName, Date, elementType, 16, 115, 0);
                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                if (crd.IsOk)
                {
                    FiberCoords c = _pvk.GetByCoords(crd);
                    if (c.IsValid)
                        return _main[c.Fiber, c.Pvk];
                }
                return null;
            }

            public override object GetItemSafe(FiberCoords crd)
            {
                if (crd.IsValid)
                    return _main[crd.Fiber, crd.Pvk];

                return null;
            }
        }

        // TODO FIXME
        protected class DataCartogramIdxMultiValue : IndexedDataCartogram
        {
            public static readonly int StreamSerializerId = 36;

            public override int Layers
            {
                get
                {
                    return _main.DimY;
                }
            }

            public override object GetItem(Coords crd)
            {
                Array ar = System.Array.CreateInstance(_main.ElementType, _main.DimY);
                int idx = base.GetIndex(crd);
                for (int i = 0; i < _main.DimY; i++)
                {
                    ar.SetValue(_main[idx, i], i);
                }
                return ar;
            }

            internal override void SetItem(Coords crd, object val)
            {
                Array ar = (Array)val;
                int idx = base.GetIndex(crd);
                for (int i = 0; i < _main.DimY; i++)
                {
                    _main[idx, i] = ar.GetValue(i);
                }
            }

            internal override void SetItemLayer(Coords crd, object obj, int layer)
            {
                _main[base.GetIndex(crd), layer] = obj;
            }

            public override object GetItemLayer(Coords crd, int layer)
            {
                return _main[base.GetIndex(crd), layer];
            }

            public override object GetItemLayerSafe(Coords crd, int layer)
            {
                int idx = base.GetIndexNoThrow(crd);
                if (idx < 0)
                    return null;
                else
                    return _main[idx, layer];
            }


            public override object GetItemLayer(FiberCoords crd, int layer)
            {
                return base.GetItemLayer(_pvk.GetByPvk(crd), layer);
            }

            public override object GetItemLayerSafe(FiberCoords crd, int layer)
            {
                return base.GetItemLayerSafe(_pvk.GetByPvk(crd), layer);
            }

            #region Simple Unsupported DataCartogram methods

            public override int GetItemInt(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override short GetItemShort(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override byte GetItemByte(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override float GetItemFloat(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override double GetItemDouble(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override Sensored GetItemAdcVal(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override FiberCoords GetItemFiberCoords(Coords crd)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
            public override CartogramType CartType
            {
                get { return CartogramType.IndexedMultiValue; }
            }

            internal DataCartogramIdxMultiValue(TupleMetaData info)
                : base(info)
            {
            }

            internal override void Init(Type elementType, ScaleIndex scale, Object exParam)
            {
                throw new Exception("Incorrect call");
            }

            internal void InitIndex(string name, string humanName, DateTime dateTime, Type elementType, ScaleIndex scale, DataArray indexOfCoords, int dim3)
            {
                base._indexes = indexOfCoords;
                base._indexedCoords = indexOfCoords.GetArrayCoords();

                _main = new DataArray(Name, HumaneName, Date, elementType, indexOfCoords.Length, dim3, 0);
                InitBase(elementType, scale);
            }

            internal void InitIndex(string name, string humanName, DateTime dateTime, Type elementType, ScaleIndex scale, DataArray indexOfCoords, int dim3, DataArray data)
            {
                base._indexes = indexOfCoords;
                base._indexedCoords = indexOfCoords.GetArrayCoords();

                _main = data;
                if ((_main.Rank != 2) || (_main.DimX != indexOfCoords.Length) || (_main.DimY != dim3))
                    throw new ArgumentException("data");

                InitBase(elementType, scale);
            }

            public override object GetItemSafe(Coords crd)
            {
                int idx = base.GetIndexNoThrow(crd);
                if (idx < 0)
                    return null;
                else
                {
                    Array ar = System.Array.CreateInstance(_main.ElementType, _main.DimY);
                    for (int i = 0; i < _main.DimY; i++)
                    {
                        ar.SetValue(_main[idx, i], i);
                    }
                    return ar;
                }
            }
        }



        #region ITupleItem Members

        public object CastTo(corelibnew.IGetCoordsConverter en, ITypeRules rules)
        {
            return CastTo(rules);
        }

        public virtual ITupleItem Rename(TupleMetaData newInfo)
        {
            DataCartogram t = CreateTyped(
                newInfo, CartType);
            t.InitBase(this.ElementType, this.ScaleIndex);

            t._pvk = _pvk;
            t._main = _main;

            return t;
        }

        public virtual ITupleItem Rename(string newParamName)
        {
            DataCartogram t = CreateTyped(
                new TupleMetaData(newParamName, HumaneName, Date), CartType);
            t.InitBase(this.ElementType, this.ScaleIndex);

            t._pvk = _pvk;
            t._main = _main;

            return t;
        }

        #endregion

        static readonly DataCoordsConverter sDefCoords = new DataCoordsConverter();
        static readonly DataFibersConverter sDefFibers = new DataFibersConverter();

        static public IInfoFormatter DefaultFiberFormatter
        {
            get { return sDefFibers; }
        }

        static public IInfoFormatter DefaultCoordsFormatter
        {
            get { return sDefCoords; }
        }

        public DataGrid ExportDataGridCoords(bool showFiberIfAvail)
        {
            DataGrid d = new DataGrid();

            d.Columns.Add(new DataGrid.Column("Координата", typeof(Coords), sDefCoords));
            d.HeadColumns = 1;
            if ((_pvk != null) && showFiberIfAvail)
            {
                d.Columns.Add(new DataGrid.Column("Разводка", typeof(FiberCoords), sDefFibers));

                d.HeadColumns = d.HeadColumns + 1;
            }

            if (Layers == 1)
            {
                d.Columns.Add(new DataGrid.Column(HumaneName, ElementType, _formatter));
            }
            else
            {
                for (int i = 0; i < Layers; i++)
                    d.Columns.Add(new DataGrid.Column(HumaneName + " " + i.ToString(), ElementType, _formatter));
            }

            foreach (Coords c in AllCoords)
            {
                DataGrid.Row r = d.Rows.Add();
                int i = 0;

                r[i++] = c;
                if (c.IsOk)
                {
                    if ((_pvk != null) && showFiberIfAvail)
                    {
                        r[i++] = _pvk.GetByCoords(c);
                    }

                    for (int j = 0; j < Layers; j++)
                    {
                        r[i++] = GetItemLayer(c, j);
                    }
                }
            }
            return d;
        }

        public DataGrid ExportDataGridFibers(bool showCoordsIfAvail)
        {
            DataGrid d = new DataGrid();

            d.Columns.Add(new DataGrid.Column("Разводка", typeof(FiberCoords), sDefFibers));

            d.HeadColumns = 1;

            if ((_pvk != null) && showCoordsIfAvail)
            {
                d.Columns.Add(new DataGrid.Column("Координата", typeof(Coords), sDefCoords));
                d.HeadColumns = d.HeadColumns + 1;
            }

            if (Layers == 1)
            {
                d.Columns.Add(new DataGrid.Column(HumaneName, ElementType, _formatter));
            }
            else
            {
                for (int i = 0; i < Layers; i++)
                    d.Columns.Add(new DataGrid.Column(HumaneName + " " + i.ToString(), ElementType, _formatter));
            }

            for (int fib = 0; fib < 16; fib++)
            {
                for (int pvk = 0; pvk < 115; pvk++)
                {
                    DataGrid.Row r = d.Rows.Add();
                    int i = 0;

                    FiberCoords fc = new FiberCoords(fib, pvk);

                    r[i++] = fc;

                    if ((_pvk != null) && showCoordsIfAvail)
                    {
                        r[i++] = _pvk.GetByPvk(fc);
                    }

                    for (int j = 0; j < Layers; j++)
                    {
                        r[i++] = GetItemLayerSafe(fc, j);
                    }

                }
            }
            return d;
        }

        public DataGrid ExportDataGridFibersTable()
        {
            DataGrid d = new DataGrid();

            d.Columns.Add("Нитка", typeof(int));

            for (int fib = 0; fib < 16; fib++)
            {
                if (Layers == 1)
                {
                    d.Columns.Add(new DataGrid.Column(String.Format("Нитка {0} {1}", fib + 1, HumaneName), ElementType, _formatter));
                }
                else
                {
                    for (int i = 0; i < Layers; i++)
                        d.Columns.Add(new DataGrid.Column(String.Format("Нитка {0} {1} {2}", fib + 1, HumaneName, i), ElementType, _formatter));
                }
            }

            for (int pvk = 0; pvk < 115; pvk++)
            {
                DataGrid.Row r = d.Rows.Add();
                int i = 0;
                r[i++] = pvk + 1;

                for (int fib = 0; fib < 16; fib++)
                {
                    for (int j = 0; j < Layers; j++)
                    {
                        r[i++] = GetItemLayerSafe(new FiberCoords(fib, pvk), j);
                    }
                }
            }

            return d;
        }

        public override DataGrid DataTable
        {
            get
            {
                if ((CartType == CartogramType.PVK) && (_pvk == null))
                {
                    //return ExportDataGridFibers(true);
                    return ExportDataGridFibersTable();
                }
                return ExportDataGridCoords(true);
            }
        }
    }

    public class PvkInfoNeededException : Exception
    {
        IPvkSink _obj;

        public PvkInfoNeededException(IPvkSink obj)
        {
            _obj = obj;
        }

        public IPvkSink Object
        {
            get { return _obj; }
        }
    }

#if OLD
    public class ArrayPvkInfo : IPvkInfo
    {
        #region IPvkInfo Members

        public Coords[] ValuableCoords
        {
            get { return _vcoords; }
        }

        public Coords GetByPvk(FiberCoords crd)
        {
            return _pvk[crd.Fiber, crd.Pvk];
        }

        public FiberCoords GetByCoords(Coords crd)
        {
            return _coords[crd.Y, crd.X];
        }

        public DataArray PvkArray
        {
            get { return _arrayOrig; }
        }

        #endregion

        public ArrayPvkInfo(DataArray ar)
        {
            if ((ar.DimX == 16) && (ar.DimY == 115) && (ar.ElementType == typeof(Coords)))
            {
                _pvk = ar.GetArrayCoords2();
                _coords = new FiberCoords[48, 48];

                int vcoords = 0;

                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 115; j++)
                    {
                        Coords c = _pvk[i, j];
                        if (c.IsOk)
                        {
                            vcoords++;
                            _coords[c.Y, c.X] = new FiberCoords(i, j);
                        }
                    }

                _vcoords = new Coords[vcoords];
                int k = 0;
                for (int i = 0; i < 16; i++)
                    for (int j = 0; j < 115; j++)
                    {
                        Coords c = _pvk[i, j];
                        if (c.IsOk)
                        {
                            _vcoords[k++] = c;
                        }
                    }

                /*
                for (int i = 0; i < 48; i++)
                    for (int j = 0; j < 48; j++)
                    {
                        Coords d = new Coords(0, 0);
                        if (_coords[i, j] == d)
                            _coords[i, j] = Coords.incorrect;
                    }
                */

                _arrayOrig = ar;
            }
            else
                throw new Exception("Don't know how to trasform to pvk-sheme");
        }

        private DataArray _arrayOrig;

        private Coords[,] _pvk;
        private FiberCoords[,] _coords;
        private Coords[] _vcoords;




    }
#endif
}
