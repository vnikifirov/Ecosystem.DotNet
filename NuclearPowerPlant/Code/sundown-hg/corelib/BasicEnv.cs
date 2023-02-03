using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

namespace corelib
{
    public class ActionCanceledException : Exception
    {
        public ActionCanceledException()
            : base ("Действие отменено")
        {

        }
        public ActionCanceledException(Exception e)
            : base("Невозможно разрешить параметр", e)
        {
            
        }
    }

    public class BasicEnv : EnvConverterFormatter, IEnviroment
    {
        DataParamTable _params;

        DataComponents _dc;
        AlogComponents _ac;

#if !DOTNET_V11
        Dictionary<string, IAlgoResource> _algoTable = new Dictionary<string, IAlgoResource>();

        Dictionary<string, IDataResource> _dataInstance = new Dictionary<string, IDataResource>();
        Dictionary<string, IResourceInstance> _algoInstance = new Dictionary<string, IResourceInstance>();
#else
        Hashtable _algoTable = new Hashtable();
        Hashtable _algoInstance = new Hashtable();

        Hashtable _dataInstance = new Hashtable();
#endif

        public override bool GetCartPresentation(IDataCartogram c, out CartogramPresentationConfig cnf, out string className, out DataParamTable paramConstructor)
        {
            cnf = DefCartPresentation;
            if (c != null && c.GetName() == "zagr_rockmicro" && IsXmlParam("DrawZagrCart"))
            {
                className = "Zagr";
                paramConstructor = ParamTuple;

                return true;
            }
            else
            {
                className = null;
                paramConstructor = null;

                return false;
            }
        }

        public static CartogramPresentationConfig FromConfig(AnyValue cnf)
        {
            return new CartogramPresentationConfig(cnf.ToInt32());
        }

        public BasicEnv(DataParamTable param, string dataPath, string algoPath) :
            this(param,
                 dataPath,
                 algoPath,
                 FromConfig(param["coordsCalculation"]),
                 FromConfig(param["coordsVisualization"]))
        {
        }

        public BasicEnv(DataParamTable param, string dataPath, string algoPath, CartogramPresentationConfig coordsCalc, CartogramPresentationConfig cartVis)
            : base (coordsCalc, cartVis)            
        {            
            _params = new DataParamTable(param.Info, param, new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)));
            _dc = new DataComponents(dataPath);
            _ac = new AlogComponents(algoPath);
            //throw new Exception("4grh4");
        }

        public string GetGlobalParam(string param)
        {
            return (string)_params[param].ToString();
        }

        public bool IsXmlParam(string param)
        {
            return _params.GetParamSafe(param).IsXml;
        }

        public System.Xml.XmlElement GetXmlParam(string param)
        {
            return _params[param].ValueXml;
        }


        public DataParamTable ParamTuple
        {
            get { return _params; }
        }

        public DataComponents Data
        {
            get { return _dc; }
        }

        public AlogComponents Algo
        {
            get { return _ac; }
        }

        public IResourceInstance FindAlgorithmInstance(string typename)
        {
            IResourceInstance tcached;
#if !DOTNET_V11
            _algoInstance.TryGetValue(typename, out tcached);
#else
            tcached = (IResourceInstance)_algoInstance[typename];
#endif
            return tcached;
        }

        public IDataResource FindDataInstance(string typename)
        {
            IDataResource tcached;
#if !DOTNET_V11
            _dataInstance.TryGetValue(typename, out tcached);
#else
            tcached = (ISuperDataResource)_dataInstance[typename];
#endif
            return tcached;
        }


        public IAlgoResource GetAlgorithm(string name)
        {
            IAlgoResource cached;
#if !DOTNET_V11
            _algoTable.TryGetValue(name, out cached);
#else
            cached = (AlogComponents.AlgoResource)_algoTable[name];
#endif

            if (cached != null)
                return cached;

            string str;
            if (Algo.GetInstanceTypeName(name, out str))
            {
                IResourceInstance tcached = FindAlgorithmInstance(str);
                if (tcached == null)
                {
                    tcached = Algo.CreateInstance(str, ParamTuple);
                    _algoInstance[str] = tcached;
                }
                cached = Algo.CreateResource(name, name, this, tcached);
                _algoTable[name] = cached;
            }
            else
            {
                cached = Algo.CreateResource(name, name, this);
                _algoTable[name] = cached;
            }
            return cached;
        }

        virtual public IDataResource CreateData(string name)
        {
            return CreateData(name, ParamTuple);
        }

        virtual public IDataResource OpenData(string name)
        {
            return _dataInstance[name];
        }

        virtual public IDataResource CreateData(string name, DataParamTable param)
        {
            DataParamTable stup = param.GetParamSafe(name).IsDataParamTable ? param[name].ValueDataParamTable :
                param;

            IDataComponent c = Data.Find(stup.Info.Name);
            if (c == null)
            {
                c = Data.Find(name);
            }
            if (c== null)
                throw new ArgumentException(String.Format("Компонент доступа к данным `{0}` не найден", name));

            /*if(name == "kgoExporterUseProvider")
                throw new Exception("error111");*/
            return CreateData(c, name, stup);
            
        }

        static string GetUniqueName(IDataComponent c)
        {
            Guid g = Guid.NewGuid();
            return String.Format("{0}_{1}", c.ComponentType.Name, g);
        }

        virtual public IDataResource CreateData(IDataComponent c, string name, DataParamTable param)
        {
            DataParamTable stup = param;
            if (stup.GetParamSafe("enviromentObject").IsNull)
                stup = new DataParamTable(stup.Info, stup, new DataParamTableItem("enviromentObject", AnyValue.FromBoxedValue(this)));

            if (name == null)
                name = GetUniqueName(c);

            IDataResource res = Data.CreateSuperResource(c.Name, name, stup);
            res.OnDisposed += new IDataResourceDisposedEvent(res_OnDisposed);

            _dataInstance.Add(name, res);
            return res;
        }

        void res_OnDisposed(IDataResource resource)
        {
            CloseData(resource.InstanceName);
        }

        public void CloseData(string name)
        {
            IDataResource d = FindDataInstance(name);
            if (d != null)
            {
                _dataInstance.Remove(name);
                d.Dispose();
            }
        }

        public void ExportAllBase(string instanceFrom, string instanceTo)
        {
            IDataResource from = FindDataInstance(instanceFrom);
            IDataResource to = FindDataInstance(instanceTo);

            bool createdFrom = (from == null) ? (from = CreateData(instanceFrom)) != null : false;
            bool createdTo = (to == null) ? (to = CreateData(instanceTo)) != null : false;

            IMultiDataProvider pfrom = from.GetMultiProvider();
            IMultiDataProvider pto = to.GetMultiProvider();

            foreach (string s in pfrom.GetStreamNames())
            {                
                foreach (DateTime t in pfrom.GetDates(s))
                {
                    pto.PushData(pfrom.GetData(t, s));
                }
            }


            if (createdFrom)
                CloseData(instanceFrom);
            if (createdTo)
                CloseData(instanceTo);
        }


        public void ExportToBase(IMultiDataProvider pfrom, string instanceTo)
        {
            IDataResource to = FindDataInstance(instanceTo);

            bool createdTo = (to == null) ? (to = CreateData(instanceTo)) != null : false;

            IMultiDataProvider pto = to.GetMultiProvider();

#if MOD
            List<IMultiDataTuple> lst = new List<IMultiDataTuple>();
            foreach (string s in pfrom.GetStreamNames())
            {
                lst.Clear();

                foreach (DateTime t in pfrom.GetDates(s))
                {
                    if (lst.Count < 32)
                        lst.Add(pfrom.GetData(t, s));
                    else
                    {
                        pto.PushData(lst);
                        lst.Clear();
                    }
                    //pto.PushData(pfrom.GetData(t, s));
                }

                pto.PushData(lst);
            }
#else
            foreach (string s in pfrom.GetStreamNames())
            {
                foreach (DateTime t in pfrom.GetDates(s))
                {
                    pto.PushData(pfrom.GetData(t, s));
                }
            }
#endif

            if (createdTo)
                CloseData(instanceTo);
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IResourceInstance instance in _algoInstance.Values)
            {
                instance.Dispose();
            }
            _algoInstance.Clear();

            foreach (IDataResource instance in _dataInstance.Values)
            {
                instance.OnDisposed -= new IDataResourceDisposedEvent(res_OnDisposed);
                instance.Dispose();
            }
            _dataInstance.Clear();
        }

        #endregion
     
    }
}
