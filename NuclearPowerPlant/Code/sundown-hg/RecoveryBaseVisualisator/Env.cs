using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Windows.Forms;
using corelib;
using RecoveryBaseVisualisator;

namespace corelib
{

#if !DOTNET_V11
    using DataArrayCoords = DataArray<Coords>;
#endif

    ////////

    class DTVSunEnvRefresh : ActionDataTupleVisualizerUI
    {
        public DTVSunEnvRefresh()
        {
            
            _name = "DTRefresh";

            _humaneName = "Обновить" ;
            _descr = "Обновить текущую базу";
            _action = Actions.ToolBarButton;
            _handler = new EventHandler(onClick);
            _image = DefImages.ImageRun;
        }

        public void onClick(object sender, EventArgs e)
        {
            ADTVEventArgs args = (ADTVEventArgs)e;
            IDataTupleVisualizerUI ui = args._ui;
            RBVEnv env = (RBVEnv)ui.GetEnviroment();

            try
            {
                env.UpdateDates();
                ((DataTupleVisualizer)ui).SetDataProvider(env.DefProvider);
                string NN = DateTime.Now.ToString();
                MessageBox.Show("База обновлена в " + NN);
                Form.ActiveForm.Text = "База данных архивной информации с восстановлением расходов, последнее обновление " + NN;
            }
            catch (ActionCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Произошла ошибка");
            }
            
        }
    }

    public class RBVEnv : GuiEnv
    {
        IInfoFormatter _zagrFormatter;

        static readonly string[] names = { "power", "flow", "energovir", "rbmk_params", "pvk_maxes_cart" };
        string[] allNames;

        bool initialized = false;

        DateTime[] mainDates = new DateTime[0];

        DataCartogram pvkLengthCartNew;

        string mainStream;
        string azotStream;
        IDataResource dataResource;

        IAlgoResource zagrConvToInt;

        public RBVEnv(DataParamTable par) :
            base(par, (string)par["componentPath"], (string)par["componentPath"])
        {

        }


        public void LoadPVK()
        {
            // Cхема ПВК
            using (IDataResource dataPvk = CreateData("SunEnv_PvkSchemeProvider"))
            {
                IDataTuple consts = dataPvk.GetConstData()[0];
                DataArrayCoords c = (DataArrayCoords)consts["pvk_scheme"];

                CoordsConverter pvk = new CoordsConverter(
                    c.Info,
                    CoordsConverter.SpecialFlag.PVK,
                    c);

                SetPVK(pvk);
            }

            // Длины ПВК
            using (IDataResource lengthPvk = CreateData("SunEnv_PvkLengthProvider"))
            {
                IDataTuple constsLen = lengthPvk.GetConstData()[0];
                pvkLengthCartNew = (DataCartogram)constsLen["pvk_length"];
            }
        }

        public void Init()
        {
            //ParamTuple.Add("enviromentObject", this);

            // Для хранения СКАЛА
            mainStream = GetGlobalParam("SunEnv_DataStorageStream");
            azotStream = "azot";

            //dataResource = CreateData("SunEnv_DataStorageProvider");
            dataResource = CreateData("SunEnv_PgDataStorageProvider2");
           
            LoadPVK();

            _zagrFormatter = new CartViewInfoProviderZagr(GetXmlParam("DrawZagrCart"));

            zagrConvToInt = GetAlgorithm(GetGlobalParam("SunEnv_AlgoZagrConvert"));
            initialized = true;
        }

        public override IInfoFormatter GetDefFormatter(FormatterType type, ITupleItem ti)
        {
            if ((ti.Name == "energovir") ||
                (ti.Name == "flow_az1_cart") ||
                (ti.Name == "flow_dp_cart"))
                return GetDefFormatter(FormatterType.RealHumane);

            if ((ti.Name == "zagr_rockmicro") && (_zagrFormatter != null))
                return _zagrFormatter;

            else if (ti.Name == "kpd_cart" || ti.Name == "bet1_cart")
                return GetDefFormatter(FormatterType.RealHumane4);


            return base.GetDefFormatter(type, ti);
        }

        public override IInfoFormatter GetFormatter(ITupleItem item)
        {
            if ((item.GetName() == "zagr_rockmicro") && (_zagrFormatter != null))
                return _zagrFormatter;
            else if (item.GetName() == "kpd_cart" || item.GetName() == "bet1_cart")
                return GetDefFormatter(FormatterType.Real);

            return base.GetFormatter(item);
        }

        public IDataResource GetDatabaseMainResource()
        {
            return dataResource;
        }

        public bool Initialized
        {
            get { return initialized; }
        }

        public DataParamTable RootParameters
        {
            get { return ParamTuple; }
        }

#if null
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
#endif
        public void ViewMultiTuple(IMultiDataTuple[] tupels)
        {
            ListMultiDataProvider p = new ListMultiDataProvider(tupels);
            ViewMultiTuple(p);

        }

        public void ViewMultiTuple(IMultiDataProvider tupels)
        {
            ViewMultiTupleForm(tupels).ShowDialog();
            
        }

        public Form ViewMultiTupleForm(IMultiDataProvider tupels)
        {
            DataTupleVisualizer dv3 = new DataTupleVisualizer(this);
            dv3.SetDataProvider(tupels);
            dv3.RegisterAction(new DTVSunEnvRefresh());
            Form form = new Form();
            
            dv3.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(dv3);
            return form;
        }


  /*      public IMultiDataTuple GetSkalaData(DateTime closeTo, bool allowExternal)
        {
            IDataResource rcSkala = CreateData("SunEnv_ImportSkalaProvider");
            IMultiDataTuple tup = GetSingleData(rcSkala, GetGlobalParam("SunEnv_ImportSkalaStream"));
            CloseData("SunEnv_ImportSkalaProvider");

            return tup;
        }*/

        DateTime GetSingleDate(IDataResource resource, string stream)
        {
            DateTime[] rcIds = resource.GetMultiProvider().GetDates();

            DateTime dt;

            if (rcIds.Length == 1)
                dt = rcIds[0];
            else
                throw new NotImplementedException();

            return dt;
        }

        IMultiDataTuple GetSingleData(IDataResource resource, DateTime dt, string stream)
        {
            return GetMultiData(resource, dt, stream);
        }

        IMultiDataTuple GetMultiData(IDataResource resource, DateTime dt, string stream)
        {
            return resource.GetMultiProvider().GetData(dt, stream);
        }

        IMultiDataTuple GetSingleData(IDataResource resource, string stream)
        {
            return GetSingleData(resource, GetSingleDate(resource, stream), stream);
        }
        IMultiDataTuple GetMultiData(IDataResource resource, string stream)
        {
            return GetMultiData(resource, GetSingleDate(resource, stream), stream);
        }

        /// <summary>
        /// Экспортирование все базы в оригинальном виде
        /// </summary>
        /*public void ExportToBase()
        {
            ExportAllBase("SunEnv_DataStorageProvider", "SunEnv_ExportStorageProvider");
        }*/

        
        public IMultiDataTuple ParseKgo(IMultiDataTuple dataSkala, IMultiDataTuple dataAzot)
        {
            return GetAlgorithm("kgoDetectMaxPos").CallIntelli(zagrConvToInt.CallIntelli(dataSkala), dataAzot); 
        }


        public DataCartogram CompactIntoPvk(TupleMetaData info, string item, IMultiDataTuple dataAzot)
        {
            IMultiTupleItem kpd = dataAzot[item];
            DataCartogram coeffDP = DataCartogram.CreateFromParts(info,
                GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, info), kpd);
            return coeffDP;
        }

        /// <summary>
        /// Построение списка дат информационных срезов
        /// </summary>
        public void UpdateDates()
        {
            
                allNames = dataResource.GetMultiProvider().GetExistNames();
                mainDates = dataResource.GetMultiProvider().GetDates();
               // System.Threading.Thread.Sleep(100000);
            
        }
        public DateTime[] GetDates()
        {
            return mainDates;
        }

        public IMultiDataTuple GetAllData(IMultiDataProvider p, DateTime date)
        {
            return p.GetData(date, allNames);
        }

        public IMultiDataTuple[] GetAllData(IMultiDataProvider p, DateTime[] dates)
        {
            if (dates.Length == 0)
                return new DataTuple[0];

            IMultiDataTuple[] res = new IMultiDataTuple[dates.Length];

            for (int i = 0; i < mainDates.Length; i++)
            {
                res[i] = p.GetData(dates[i], allNames); 
            }
            return res;
        }

        public IMultiDataProvider DefProvider
        {
            get
            {
                return dataResource.GetMultiProvider();
            }
        }

        public IMultiDataTuple[] GetAllData(IMultiDataProvider p)
        {
            return GetAllData(p, mainDates);
        }

        public MultiDataTuple CartToFibers(string name, IMultiDataTuple azotCart)
        {
            return CartToFibers(name, azotCart, true);
        }
        public MultiDataTuple CartToFibers(string name, IMultiDataTuple azotCart, bool param)
        {
            DataCartogram c = (DataCartogram)azotCart[0][name + "_cart"];
            DataArray[] ar = c.ConvertToPartArray(
                new TupleMetaData(name, name, c.Date, TupleMetaData.StreamAuto), GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, c.Info));
            if (param)
            {
                DataParamTable[] pt = new DataParamTable[16];
                for (int i = 0; i < pt.Length; i++)
                    pt[i] = new DataParamTable(new TupleMetaData("kgoprp_info", "kgoprp_info", c.Date, TupleMetaData.StreamAuto),
                            new DataParamTableItem("fiberNum", i));

                return new MultiDataTuple(name, azotCart.GetTimeDate(), new MultiTupleItem(ar), new MultiTupleItem(pt));
            }
            return new MultiDataTuple(name, azotCart.GetTimeDate(), new MultiTupleItem(ar));
        }
        

        public IMultiDataTuple ReadAzotData(IMultiDataProvider p,  DateTime dt)
        {
            return CartToFibers("pvk_maxes", p.GetData(dt, names));
        }
        public IMultiDataTuple ConvAzotData(IMultiDataTuple azotCart)
        {
            return CartToFibers("pvk_maxes", azotCart);
        }

        public IMultiDataTuple ReadRockData(IMultiDataProvider p, DateTime dt)
        {
            //dataResource.GetMultiProvider()
            string stream = mainStream;
            string[] streams = p.GetStreamNames();
            if (streams.Length == 1)
                stream = streams[0];

            return p.GetData(dt, stream);
        }

     /*   public IMultiDataTuple ComputeAdaptationCoefficient(IMultiDataProvider p, DateTime dt)
        {
            IMultiDataTuple azot = ReadAzotData(p, dt);
            IMultiDataTuple zagr = ReadRockData(p, dt);
            IMultiDataTuple tmp_wockr = GetAlgorithm("calculateMiddlePower").CallIntelli(zagr);
            // //Evaluate adaptation coefficients
            IMultiDataTuple koeffs = GetAlgorithm("evaluteAzotDpCoeff").CallIntelli( 
                zagr,
                tmp_wockr,
                pvkLengthCartNew,
                azot);

            return koeffs;
        }
        */
        public IMultiDataTuple RecoveryData(IMultiDataProvider p, DateTime dt, IMultiDataTuple coeff)
        {
            IMultiDataTuple azot = ReadAzotData(p, dt);
            IMultiDataTuple zagr = ReadRockData(p, dt);
            IMultiDataTuple tmp_wockr = GetAlgorithm("calculateMiddlePower").CallIntelli(zagr);

            IMultiDataTuple recovered = GetAlgorithm("evaluteAzotDpFlow").CallIntelli(
                zagr,
                tmp_wockr,
                pvkLengthCartNew,
                azot,
                coeff);

            return recovered;
        }

        public IMultiDataTuple RecoveryDataFib(IMultiDataTuple zagr, IMultiDataTuple coeff)
        {
            IMultiDataTuple tmp_wockr = GetAlgorithm("calculateMiddlePower").CallIntelli(zagr);

            IMultiDataTuple recovered = GetAlgorithm("evaluteAzotDpFlow").CallIntelli(
                zagr,
                tmp_wockr,
                pvkLengthCartNew,
                coeff);

            return recovered;
        }


        /// <summary>
        /// Возвращает массив координат только топливных ячеек
        /// </summary>
        /// <param name="zagr">Картограмма загрузки АЗ</param>
        /// <returns></returns>
        Coords[] GetFuelCoords(IMultiDataTuple zagr)
        {
            IDataTuple zone = zagrConvToInt.CallIntelli(zagr)[0];

            DataCartogram c = (DataCartogram)zone["zagr"];
            ArrayList lts = new ArrayList();
            foreach (Coords d in GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()))
                if (c.GetAnyValue(d, 0).ToInt32(null) == (int)ChannelType.TVS)
                    lts.Add(d);


            Coords[] allCoords = new Coords[lts.Count];
            lts.CopyTo(allCoords);
            return allCoords;
        }

        Coords[] GetValidCoords(IMultiDataTuple zagr, IMultiDataTuple zagrPrev)
        {
            IDataTuple zone = zagrConvToInt.CallIntelli(zagr)[0];
            IDataTuple zone2 = zagrConvToInt.CallIntelli(zagrPrev)[0];

            DataCartogram c = (DataCartogram)zone["zagr"];
            DataCartogram c2 = (DataCartogram)zone2["zagr"];
            DataCartogram ca = (DataCartogram)zagr[0]["energovir"];
            DataCartogram cb = (DataCartogram)zagrPrev[0]["energovir"];
            ArrayList lts = new ArrayList();
            foreach (Coords d in GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()))
                if ((c.GetAnyValue(d, 0).ToInt32(null) == (int)ChannelType.TVS) &&
                    (c2.GetAnyValue(d, 0).ToInt32(null) == (int)ChannelType.TVS) &&
                    (cb[d,0] < ca[d,0]))
                    lts.Add(d);


            Coords[] allCoords = new Coords[lts.Count];
            lts.CopyTo(allCoords);
            return allCoords;
        }

        Coords[] GetValidCoordsZrk(IMultiDataTuple zagr, IMultiDataTuple zagrPrev)
        {
            IDataTuple zone = zagrConvToInt.CallIntelli(zagr)[0];
            IDataTuple zone2 = zagrConvToInt.CallIntelli(zagrPrev)[0];

            DataCartogram c = (DataCartogram)zone["zagr"];
            DataCartogram c2 = (DataCartogram)zone2["zagr"];
            DataCartogram ca = (DataCartogram)zagr[0]["energovir"];
            DataCartogram cb = (DataCartogram)zagrPrev[0]["energovir"];
            DataCartogram za = (DataCartogram)zagr[0]["zrk"];
            DataCartogram zb = (DataCartogram)zagrPrev[0]["zrk"];

            ArrayList lts = new ArrayList();
            foreach (Coords d in GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData()))
                if ((c.GetAnyValue(d, 0).ToInt32(null) == (int)ChannelType.TVS) &&
                    (c2.GetAnyValue(d, 0).ToInt32(null) == (int)ChannelType.TVS) &&
                    (cb[d, 0] < ca[d, 0]) &&
                    za[d,0] == zb[d,0])
                    lts.Add(d);


            Coords[] allCoords = new Coords[lts.Count];
            lts.CopyTo(allCoords);
            return allCoords;
        }


        public IMultiDataTuple ComputeAdaptationCoefficient(IMultiDataTuple data)
        {
            IMultiDataTuple tmp_wockr = GetAlgorithm("calculateMiddlePower").CallIntelli(data);
            //Evaluate adaptation coefficients
            IMultiDataTuple koeffs = GetAlgorithm("evaluteAzotDpCoeff").CallIntelli(
                data,
                tmp_wockr,
                pvkLengthCartNew, ConvAzotData(data));

            return koeffs;
        }

        public DataCartogram compact(IMultiDataTuple tup, string part, CoordsConverter conv)
        {
            IMultiTupleItem kpd = tup[part];
            DataCartogram coeffDP = DataCartogram.CreateFromParts(kpd[0].Info,
                GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, kpd[0].Info), kpd);
            DataCartogram fdp = coeffDP.ConvertToIndexedType(
                new TupleMetaData(part+"_cart", coeffDP.HumaneName, coeffDP.Date, TupleMetaData.StreamAuto), conv, coeffDP.ElementType);
            return fdp;
        }

        public DataTuple CombineCoeffsIntoCartogram(IMultiDataTuple datas, IMultiDataTuple zagr)
        {
            Coords[] allCoords = GetFuelCoords(zagr);
            CoordsConverter conv = new CoordsConverter(
                new TupleMetaData("recover", "recover", DateTime.Now, TupleMetaData.StreamAuto), CoordsConverter.SpecialFlag.Named, allCoords);

            DataCartogram fdp = compact(datas, "kpd", conv);
            DataCartogram faz = compact(datas, "bet1", conv);
            return new DataTuple("auto", faz.Date, fdp, faz);
        }

        public DataTuple CombineRecoveredIntoCartogram(IMultiDataTuple datas/*, IMultiDataTuple fiberNumeration*/, 
            IMultiDataTuple zagr, IMultiDataTuple zagrPrev)
        {

#if FILTER_PER            
            Coords[] allCoords = GetValidCoords(zagr, zagrPrev);
#else
            Coords[]  allCoords = GetFuelCoords(zagr);
#endif
            CoordsConverter conv = new CoordsConverter(
                new TupleMetaData("recover", "recover", DateTime.Now, TupleMetaData.StreamAuto), CoordsConverter.SpecialFlag.Named, allCoords);

            DataCartogram fdp = compact(datas, "flow_dp", conv);
            DataCartogram faz = compact(datas, "flow_az1", conv);
            return new DataTuple("auto", faz.Date, fdp, faz);
        }


        public DataTuple ComputeAdaptationCoefficientCart(IMultiDataTuple data)
        {
            IMultiDataTuple coefs = ComputeAdaptationCoefficient(data);
            return CombineCoeffsIntoCartogram(coefs, coefs);
        }

        public DataTuple RecoveryData(IMultiDataTuple zagr, IMultiDataTuple zagr_prev, IMultiDataTuple coeff)
        {
            IMultiDataTuple ret = RecoveryDataFib(zagr, MultiDataTuple.Combine(null, CartToFibers("pvk_maxes", zagr),
                CartToFibers("kpd", coeff, true), CartToFibers("bet1", coeff, true)));
            return CombineRecoveredIntoCartogram(ret, zagr, zagr_prev);
        }



    }
   
}