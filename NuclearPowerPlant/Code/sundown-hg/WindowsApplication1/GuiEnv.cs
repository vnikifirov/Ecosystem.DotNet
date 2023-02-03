using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;

namespace corelib
{

    public class GuiEnv : BasicEnv
    {
        public GuiEnv(DataParamTable par) :
            base(par, (string)par["componentPath"], (string)par["componentPath"])
        {

        }

        public GuiEnv(DataParamTable param, string dataPath, string algoPath) :
            base (param, dataPath, algoPath)
        {

        }


        public override IDataResource CreateData(string name)
        {
            if (ParamTuple.GetParamSafe(name).IsDataParamTable)
                return base.CreateData(name);

            IDataResource res = NewDataInstance.SelectComponent(this, name);
            return res;
        }

        public override IDataResource CreateData(string name, DataParamTable param)
        {
            DataParamTable stup = (param != null && param.GetParamSafe(name).IsDataParamTable) ? param[name].ValueDataParamTable : param;

            stup = new DataParamTable(stup.Info, stup, new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)));

            IDataComponent c = Data.Find(stup.Info.Name);

            if (c == null)
                throw new ArgumentException(String.Format("Компонент доступа к данным `{0}` не найден", name));

            if ((c.Info.ComponentFileNameArgument != null && c.Info.ComponentFileNameArgument.Length > 0) &&
                (c.Info.ParametrsInfoString == null))
            {
                if (stup.GetParamSafe(c.Info.ComponentFileNameArgument).IsNull)
                {
                    OpenFileDialog fd = new OpenFileDialog();
                    fd.Title = c.Info.HumanDescribe;
                    fd.ShowReadOnly = false;
                    fd.Multiselect = false;
                    fd.Filter = c.Info.FileFilter;

                    if (fd.ShowDialog() == DialogResult.OK)
                        //stup.Add(c.Info.ComponentFileNameArgument, fd.FileName);
                        stup = new DataParamTable(stup.Info, stup, new DataParamTableItem(c.Info.ComponentFileNameArgument, AnyValue.FromBoxedValue(fd.FileName)));
                    else
                        throw new ActionCanceledException();
                }
            }
            if (c.Info.IsParametrsInfoMethodSet && c.Info.ParametrsInfoString != null)
            {
                AutoParameterInfo[] pars = c.Info.GetParametrsInfo();
                if (AutoParameterInfo.GetUnsatisfiedCount(pars, stup, out stup) > 0)
                {
                    ParametersListVisualizerForm f = new ParametersListVisualizerForm(pars, stup, true, "Задайте параметры для создания объекта данных"
);
                    f.Text = String.Format("Создание объекта: {0}", c.Info.HumanDescribe);
                    f.Width = 450;

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        stup = f.GetParameters();
                        //stup.Add("enviromentObject", this);
                        stup = new DataParamTable(stup.Info, stup,
                            new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)));
                    }
                    else
                    {
                        throw new ActionCanceledException();
                    }
                }
            }
            return base.CreateData(name, stup);
        }


        public DataTupleVisualizer ViewMultiTupleForm(IMultiDataProvider tupels)
        {
            DataTupleVisualizer dv3 = new DataTupleVisualizer(this);
            dv3.SetDataProvider(tupels);

            Form form = new Form();
            dv3.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(dv3);
            return dv3;
        }





        public IDataResource GetOpenFileComponent()
        {
#if !DOTNET_V11
            List<DataComponents.DataComponent> items = new List<DataComponents.DataComponent>();
#else
            ArrayList items = new ArrayList();
#endif

            StringBuilder sb = new StringBuilder();
            StringBuilder sbAll = new StringBuilder();
            bool first = true;
            foreach (DataComponents.DataComponent component in Data.Components)
            {
                if ((component.Info.FileFilter.Length > 0) &&
                    (component.Info.ComponentFileNameArgument != null && component.Info.ComponentFileNameArgument.Length > 0))
                {
                    string[] fileDetails = component.Info.FileFilter.Split('|');
                    if (fileDetails.Length == 2)
                    {
                        items.Add(component);
                        sb.Append(component.Info.FileFilter);
                        sb.Append("|");

                        if (first)
                            first = false;
                        else
                            sbAll.Append(';');
                        sbAll.Append(fileDetails[1]);
                    }
                }
            }

            sb.AppendFormat("Все доступные файлы ({0})|{0}", sbAll);
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = sb.ToString();
            fd.FilterIndex = items.Count + 1;

            DialogResult res = fd.ShowDialog();
            if (res == DialogResult.OK)
            {
                int idx = fd.FilterIndex - 1;
                if (idx == items.Count)
                {
                    string[] nparts = fd.FileName.Split('.');
                    string ext = nparts[nparts.Length - 1];

                    string[] allNames = sbAll.ToString().Split(';');
                    int i = 0;
                    for (; i < allNames.Length; i++)
                    {
                        string[] spext = allNames[i].Split('.');
                        string pext = spext[spext.Length - 1];

                        if (pext == ext)
                            break;
                    }
                    idx = i;
                }

                DataComponents.DataComponent component = (DataComponents.DataComponent)items[idx];

                //ParamTuple pt = new ParamTuple("config");
                //pt.Add("enviromentObject", all);
                //pt.Add(component.Info.ComponentFileNameArgument, fd.FileName);

                DataParamTable pt = new DataParamTable(new TupleMetaData("params", "params", DateTime.Now, "params"),
                    new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)),
                    new DataParamTableItem(component.Info.ComponentFileNameArgument, fd.FileName));

                if (component.Info.ParametrsInfoString != null && component.Info.IsParametrsInfoMethodSet)
                {
                    AutoParameterInfo[] parameters = component.Info.GetParametrsInfo();
                    foreach (AutoParameterInfo par in parameters)
                    {
                        if (pt.GetParamSafe(par.Name).IsNull)
                        {
                            //pt.Add(par.Name, par.DefValue);
                            pt = new DataParamTable(pt.Info, pt,
                                new DataParamTableItem(par.Name, AnyValue.FromBoxedValue(par.DefValue)));
                        }
                    }

                }

                //return component.Create(pt);
                return Data.CreateSuperResource(component.Name, component.Name, pt);
            }
            else
                return null;
        }

        /***********************************************************/
        /**************************Роман****************************/
        public IDataResource GetOpenFileComponent(string file_database)
        {
#if !DOTNET_V11
            List<DataComponents.DataComponent> items = new List<DataComponents.DataComponent>();
#else
            ArrayList items = new ArrayList();
#endif

            StringBuilder sb = new StringBuilder();
            StringBuilder sbAll = new StringBuilder();
            bool first = true;
            foreach (DataComponents.DataComponent component in Data.Components)
            {
                if ((component.Info.FileFilter.Length > 0) &&
                    (component.Info.ComponentFileNameArgument != null && component.Info.ComponentFileNameArgument.Length > 0))
                {
                    string[] fileDetails = component.Info.FileFilter.Split('|');
                    if (fileDetails.Length == 2)
                    {
                        items.Add(component);
                        sb.Append(component.Info.FileFilter);
                        sb.Append("|");

                        if (first)
                            first = false;
                        else
                            sbAll.Append(';');
                        sbAll.Append(fileDetails[1]);
                    }
                }
            }


            string[] nparts = file_database.Split('.');
            string ext = nparts[nparts.Length - 1];
            int idx = 0;
            string[] allNames = sbAll.ToString().Split(';');
            int i = 0;
            for (; i < allNames.Length; i++)
            {
                string[] spext = allNames[i].Split('.');
                string pext = spext[spext.Length - 1];

                if (pext == ext)
                    break;
            }
            idx = i;

            DataComponents.DataComponent component2 = (DataComponents.DataComponent)items[idx];

            //ParamTuple pt = new ParamTuple("config");
            //pt.Add("enviromentObject", all);
            //pt.Add(component.Info.ComponentFileNameArgument, fd.FileName);

            DataParamTable pt = new DataParamTable(new TupleMetaData("params", "params", DateTime.Now, "params"),
                new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)),
                new DataParamTableItem(component2.Info.ComponentFileNameArgument, file_database));

            if (component2.Info.ParametrsInfoString != null && component2.Info.IsParametrsInfoMethodSet)
            {
                AutoParameterInfo[] parameters = component2.Info.GetParametrsInfo();
                foreach (AutoParameterInfo par in parameters)
                {
                    if (pt.GetParamSafe(par.Name).IsNull)
                    {
                        //pt.Add(par.Name, par.DefValue);
                        pt = new DataParamTable(pt.Info, pt,
                            new DataParamTableItem(par.Name, AnyValue.FromBoxedValue(par.DefValue)));
                    }
                }

            }

            //return component.Create(pt);
            return Data.CreateSuperResource(component2.Name, component2.Name, pt);
        }
        /***********************************************************/
        /***********************************************************/

    

    }
}