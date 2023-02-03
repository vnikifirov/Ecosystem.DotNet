using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using corelib;


using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.Steps.MacroProcessing;
using Boo.Lang.Compiler.IO;

using Rhino.DSL;

using Assembly = System.Reflection.Assembly;


namespace WindowsApplication1
{
#if !DOTNET_V11
    using DataArrayDouble = DataArray<double>;
#endif


    public abstract class AbstractDslClass
    {
        protected IDataTupleVisualizerUI _obj;

        string _defaultStream;

        public AbstractDslClass(IDataTupleVisualizerUI obj)
        {
            _obj = obj;

            _defaultStream = _obj.GetActiveTupleItemStream();

            if (_defaultStream == null)
            {
                if (MultiData != null)
                {
                    if (MultiData.GetStreamNames().Length > 0)
                        _defaultStream = MultiData.GetStreamNames()[0];
                }
            }
        }

        public IDuckTuple Data
        {
            get
            {
                if (Item != null)
                {
                    return MultiData.GetData(Item.GetTimeDate(), _defaultStream)[0];
                }
                return null;
            }
        }


        public IEnumerable<IDataTuple> Tuples
        {
            get { return GetSingleTuples(_defaultStream); }
        }
        public IEnumerable<IMultiDataTuple> MultiTuples
        {
            get { return GetMultiTuples(_defaultStream); }
        }

        public DataArray CollectData(Coords coords, string name)
        {
            return CollectData(coords, name, _defaultStream);
        }

        public string DefaultStream
        {
            get { return _defaultStream; }
            set { _defaultStream = value; }
        }

        public DataArray CollectData(Coords coords, string name, string stream)
        {
            Timestamp[] dates;
            int i;
            return new DataArrayDouble(
                new TupleMetaData("collected_" + name, name, DateTime.Now, TupleMetaData.StreamAuto),
                DataCartogramVisualizerPlugins.CollectDataFromCoord(GetSingleTuples(stream), coords, name, null, null, out i, out dates));
        }

        public IEnumerable<IDataTuple> GetSingleTuples(string stream)
        {
            foreach (DateTime dt in MultiData.GetDates(stream))
            {
                yield return MultiData.GetData(dt, stream)[0];
            }
        }
        public IEnumerable<IMultiDataTuple> GetMultiTuples(string stream)
        {
            foreach (DateTime dt in MultiData.GetDates(stream))
            {
                yield return MultiData.GetData(dt, stream);
            }
        }
        public IMultiDataProvider MultiData
        {
            get { return _obj.GetMultiDataProvider(); }
        }
        public ITupleItem Item
        {
            get { return _obj.GetActiveTupleItem(); }
        }
        public IDataCartogram Cartogram
        {
            get { return _obj.GetActiveTupleItem() as IDataCartogram; }
        }
        public DataCartogramVisualizer CartogramVis
        {
            get { return _obj.GetItemVisualizer() as DataCartogramVisualizer; }
        }

        public IDataTupleVisualizerUI TupleVis
        {
            get { return _obj; }
        }

        public BasicEnv Enviroment
        {
            get { return _obj.GetEnviroment() as BasicEnv; }
        }

        public void display(ITupleItem item)
        {
            TupleVis.SetActiveTupleItem(item, _obj.GetActiveTupleItemStream());
        }

        public void display(IMultiTupleItem ar)
        {
            TupleVis.SetActiveTupleItems(ar, _obj.GetActiveTupleItemStream());
        }

        public void display(params ITupleItem[] ar)
        {
            display(new MultiTupleItem(ar));
        }

        public void display(Array ar)
        {
            if (((ar.Rank == 1) && (ar as Array[] != null) && (((Array[])ar)[0].Rank == 1)))
            {

                Array[] a = (Array[])ar;
                DataArray[] d = new DataArray[ar.Length];
                for (int i = 0; i < d.Length; i++)
                {
                    d[i] = DataArray.Create(
                        new TupleMetaData("temp" + i, "temp" + i, DateTime.Now, TupleMetaData.StreamAuto), a[i]);
                }

                display(d);
            }
            else if (ar.Rank < 4)
            {
                display((ITupleItem)DataArray.Create(
                    new TupleMetaData("array", "массив", DateTime.Now, TupleMetaData.StreamAuto), ar));
            }
        }

        public Coords[] SelectedCoords
        {
            get { return CartogramVis.GetSelectedCells(); }
            set { CartogramVis.SetSelectedCells(value); }
        }

        public void load(IDataResource r)
        {
            load(r.GetMultiProvider());
        }
        public void load(IMultiDataProvider p)
        {
            ((DataTupleVisualizer)_obj).SetDataProvider(p);
        }

        public bool select(Coords c)
        {
            if (CartogramVis != null)
                return CartogramVis.SelectCell(c, true);
            return false;
        }

        public bool unselect(Coords c)
        {
            if (CartogramVis != null)
                return CartogramVis.SelectCell(c, false);
            return false;
        }

        public IMultiDataTuple call(string function, params IMultiDataTuple[] data)
        {
            IAlgoResource res = Enviroment.GetAlgorithm(function);
            return res.CallIntelli(data);
        }

        public DataCartogramIndexed<double> mod_cart(DataCartogram cart, Coords[] cells, double[] vals)
        {
            CoordsConverter c = cart.AllCoords;
            if (c.Name == "convdummy")
                c = Enviroment.Linear1884;

            double[] val = cart.ScaleToIndexed(c, 0);

            for (int i = 0; i < vals.Length; i++)
            {
                val[c[cells[i]]] = vals[i];
            }
            
            return new DataCartogramIndexed<double>(cart.Info, ScaleIndex.Default, c, val);
        }
        public delegate double modifier(double val);

        public DataTuple mod_tup(IDataTuple tup, string item, modifier mod, params Coords[] cells)
        {
            DataCartogram it = tup[item] as DataCartogram;
            double[] modv = new double[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                modv[i] = mod(it[cells[i], 0]);
            }
            DataCartogram it2 = mod_cart(it, cells, modv);

            ITupleItem[] items = new ITupleItem[tup.ItemsCount + 1];
            for (int i = 0; i < tup.ItemsCount; i++)
            {
                if (tup[i].Name == item)
                    items[i] = tup[i].Rename(new TupleMetaData(item + "_old", tup[i].HumaneName + "_old",
                        tup[i].Info.Date, tup[i].Info.StreamName));
                else
                    items[i] = tup[i];
            }
            items[tup.ItemsCount] = it2;

            return new DataTuple("main"//tup.GetStreamName()
                , tup.GetTimeDate(), items);
        }

        public IDataTupleVisualizerUI window(IMultiDataProvider prov)
        {
            return window(prov, "DSL");
        }
        public IDataTupleVisualizerUI window(IMultiDataProvider prov, string caption)
        {
            DataTupleVisualizer dv3 = new DataTupleVisualizer(Enviroment);
            dv3.SetDataProvider(prov);

            Form form = new Form();
            form.Text = caption;
            dv3.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(dv3);
            form.Show();

            return dv3;
        }

        public abstract void print(params object[] str);

        public abstract void status(object s);

        public abstract void log(object s);

        public abstract void onClick();


        public CoordsConverter Linear1884
        {
            get
            {
                return Enviroment.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData());
            }
        }

        public DataCartogram DataCartogram(TupleMetaData td, ScaleIndex sc, CoordsConverter cv, object data)
        {
            if (data is double[])
            {
                if (cv == CoordsConverter.sDummyConverter)
                    return DataCartogramNative<double>.FromLinear(td, sc, (double[])data);
                else if (cv.Rank == 1)
                    return new DataCartogramIndexed<double>(td, sc, cv, (double[])data);
                else
                    return DataCartogramIndexed<double>.FromLinear(td, sc, cv, (double[])data);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }


}