using System;
using System.Reflection;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.IO;
using System.Text;

namespace corelib
{
    public class SingleSearchToMultiProvider : ListMultiDataProvider
    {
#if DOTNET_V11
        static void AddSubSearch(string path, string pattern, ArrayList founded)
        {
            string[] dir = System.IO.Directory.GetDirectories(path);
            foreach (string d in dir)
                AddSubSearch(d, pattern, founded);

            string[] files = System.IO.Directory.GetFiles(path, pattern);
            foreach (string f in files)
                founded.Add(f);
        }

        static string[] FixGetFiles(string path, string pattern)
        {
            ArrayList a = new ArrayList();
            AddSubSearch(path, pattern, a);

            string[] res = new string[a.Count];
            a.CopyTo(res);
            return res;
        }
#endif
        public readonly string Errors;
        public readonly string[] Names;
        public bool IsErrors
        {
            get { return Errors != null; }
        }

        struct SortData : IComparable 
        {
            public SortData(string nm, DateTime date)
            {
                name = nm;
                dt = date;
            }
            public string name;
            public DateTime dt;

            public int CompareTo(object obj)
            {
                SortData sobj = (SortData)obj;
                return dt.CompareTo(sobj.dt);
            }
        };

        public SingleSearchToMultiProvider(IEnviromentEx enviromentObject, IDataComponent component, string searchDirectoryPath)
        {
            string searchingFile = component.Info.FileFilter.Split('|')[1];

#if !DOTNET_V11
            string[] files = System.IO.Directory.GetFiles(searchDirectoryPath, searchingFile, System.IO.SearchOption.AllDirectories);
#else
            string[] files = FixGetFiles(searchDirectoryPath, searchingFile);
#endif
            ArrayList u = new ArrayList(); // даты несортированные

            ArrayList d = new ArrayList(); // сами данные несортированные
            ArrayList md = new ArrayList();
            ArrayList sd = new ArrayList(); //массив для хранения структур данных


            StringBuilder sb = null;
            int errors = 0;

            foreach (string f in files)
            {
                try
                {
                    DataParamTable tup = new DataParamTable(new TupleMetaData("params", "params", DateTime.Now, "params"),
                        new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(enviromentObject)),
                        new DataParamTableItem(component.Info.ComponentFileNameArgument, f));
                    //tup.Add("enviromentObject", enviromentObject);
                    //tup.Add(component.Info.ComponentFileNameArgument, f);

                    using (ISingleDataProvider prov = (ISingleDataProvider)component.Create(tup))
                    {
                        IMultiDataTuple tt = prov.GetData();
                        d.Add(tt);
                        sd.Add(new SortData(f, tt.GetTimeDate()));
                        u.Add(tt.GetTimeDate());
                    }
                }
                catch (Exception e)
                {
                    if (sb == null)
                        sb = new StringBuilder();
                    sb.AppendFormat("{0} [{1}]\r\n", f, e.Message);
                    errors++;
                }
            }

            DateTime[] ut = new DateTime[u.Count];
            u.Sort();
            u.CopyTo(ut);

            string[] sdt = new string[u.Count];
            sd.Sort();
            for (int i = 0; i < u.Count; i++)
            {
                sdt[i] = ((SortData)sd[i]).name;
            }
            Names = sdt;


            DataTuple[] t = null;
            DataTuple[][] mt = null;
            //Sorting data
            int idx = 0;
            if (component.Info.MultiTupleOutput)
            {
                DataTuple[][] dtem = new DataTuple[md.Count][];
                d.CopyTo(dtem);

                mt = new DataTuple[md.Count][];
                foreach (DateTime dtt in ut)
                    for (int k = 0; k < md.Count; k++)
                        if (dtem[k][0].GetTimeDate() == dtt)
                            mt[idx++] = dtem[k];
            }
            else
            {
                DataTuple[] dtem = new DataTuple[d.Count];
                d.CopyTo(dtem);

                t = new DataTuple[d.Count];
                foreach (DateTime dtt in ut)
                    for (int k = 0; k < d.Count; k++)
                        if (dtem[k].GetTimeDate() == dtt)
                            t[idx++] = dtem[k];
            }

            if (errors > 0)
                Errors = sb.ToString();
            else
                Errors = null;

            foreach (IMultiDataTuple tup in t)
                PushData(tup);

        }
    }

    //////////////////////////
    ///New//////////////My////
    //////////////////////////

    public class SingleSearchToPgSqlProvider
    {
        public readonly string Errors;
        public readonly string[] Names;
        public bool IsErrors
        {
            get { return Errors != null; }
        }
        struct SortData : IComparable
        {
            public SortData(string nm, DateTime date)
            {
                name = nm;
                dt = date;
            }
            public string name;
            public DateTime dt;
            public int CompareTo(object obj)
            {
                SortData sobj = (SortData)obj;
                return dt.CompareTo(sobj.dt);
            }
        };
        //Конструктор; последний параметр p - передаем провайдер на основе хранилища Postgresql
        public SingleSearchToPgSqlProvider(IEnviromentEx enviromentObject, IDataComponent component, string searchDirectoryPath, IMultiDataProvider p)
        {
            string searchingFile = component.Info.FileFilter.Split('|')[1];

#if !DOTNET_V11
            string[] files = System.IO.Directory.GetFiles(searchDirectoryPath, searchingFile, System.IO.SearchOption.AllDirectories);
#else
            string[] files = FixGetFiles(searchDirectoryPath, searchingFile);
#endif
            ArrayList u = new ArrayList(); // даты несортированные
            ArrayList d = new ArrayList(); // сами данные несортированные
            ArrayList md = new ArrayList();
            ArrayList sd = new ArrayList(); //массив для хранения структур данных

            StringBuilder sb = null;
            int errors = 0;

            foreach (string f in files)
            {
                try
                {
                    DataParamTable tup = new DataParamTable(new TupleMetaData("params", "params", DateTime.Now, "params"),
                        new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(enviromentObject)),
                        new DataParamTableItem(component.Info.ComponentFileNameArgument, f));
                    //tup.Add("enviromentObject", enviromentObject);
                    //tup.Add(component.Info.ComponentFileNameArgument, f);

                    using (ISingleDataProvider prov = (ISingleDataProvider)component.Create(tup))
                    {
                        IMultiDataTuple tt = prov.GetData();
                        d.Add(tt);
                        sd.Add(new SortData(f, tt.GetTimeDate()));
                        u.Add(tt.GetTimeDate());
                    }
                }
                catch (Exception e)
                {
                    if (sb == null)
                        sb = new StringBuilder();
                    sb.AppendFormat("{0} [{1}]\r\n", f, e.Message);
                    errors++;
                }
            }
            DateTime[] ut = new DateTime[u.Count];
            u.Sort();
            u.CopyTo(ut);

            string[] sdt = new string[u.Count];
            sd.Sort();
            for (int i = 0; i < u.Count; i++)
            {
                sdt[i] = ((SortData)sd[i]).name;
            }
            Names = sdt;
            DataTuple[] t = null;
            DataTuple[][] mt = null;
            //Sorting data
            int idx = 0;
            if (component.Info.MultiTupleOutput)
            {
                DataTuple[][] dtem = new DataTuple[md.Count][];
                d.CopyTo(dtem);
                mt = new DataTuple[md.Count][];
                foreach (DateTime dtt in ut)
                    for (int k = 0; k < md.Count; k++)
                        if (dtem[k][0].GetTimeDate() == dtt)
                            mt[idx++] = dtem[k];
            }
            else
            {
                DataTuple[] dtem = new DataTuple[d.Count];
                d.CopyTo(dtem);
                t = new DataTuple[d.Count];
                foreach (DateTime dtt in ut)
                    for (int k = 0; k < d.Count; k++)
                        if (dtem[k].GetTimeDate() == dtt)
                            t[idx++] = dtem[k];
            }

            if (errors > 0)
                Errors = sb.ToString();
            else
                Errors = null;
            foreach (IMultiDataTuple tup in t)
                p.PushData(tup);
        }
    }
}
