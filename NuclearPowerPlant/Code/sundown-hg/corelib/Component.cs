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
    public enum AutoParameterInfoType
    {
        String,
        Bool,
        FilePath,
        DirectoryPath,
        Password,
        Enviroment,
        List
    }

    public struct AutoParameterInfo
    {
        public readonly string Name;
        public readonly string Help;
        public readonly AutoParameterInfoType InfoType;
        public readonly object DefValue;
        public readonly bool HideSatisfied;
        public readonly Type TestType;
        public readonly object[] VaidValues;

        public AutoParameterInfo(string name, string help, AutoParameterInfoType type, Type testType, object defValue, bool hide, object[] valid)
        {
            Name = name;
            Help = help;
            InfoType = type;
            DefValue = defValue;
            HideSatisfied = hide;
            TestType = testType;
            VaidValues = valid;
        }

        public AutoParameterInfo(string name, string help, AutoParameterInfoType type)
            : this(name, help, type, null, null, true, null)
        {

        }


        static AutoParameterInfo ParseLexItem(dsltools.LexemeStream inl, out dsltools.LexemeStream outl, System.Reflection.ParameterInfo paramsInfo)
        {
            outl = inl;
            if (!inl.IsSatisfied("word", "(", "string", ",", "word"))
                throw new ArgumentException("Невозможно разобрать");

            string name = outl[0].Data;
            string help = outl[2].Data.Substring(1, outl[2].Data.Length - 2);
            AutoParameterInfoType it;
            switch (outl[4].Data)
            {
                case "String": it = AutoParameterInfoType.String; break;
                case "Bool": it = AutoParameterInfoType.Bool; break;
                case "FilePath": it = AutoParameterInfoType.FilePath; break;
                case "DirectoryPath": it = AutoParameterInfoType.DirectoryPath; break;
                case "Password": it = AutoParameterInfoType.Password; break;
                case "Enviroment": it = AutoParameterInfoType.Enviroment; break;
                case "List": it = AutoParameterInfoType.List; break;
                default:
                    throw new ArgumentException("Невозможно заробрать отображаемый тип параметра");
            }

            if (paramsInfo.Name != name)
                throw new ArgumentException("Несоответствие аргумента и описания для него");

            object defValue = null;
            bool hide = true;
            object[] validValues = null;

            if (inl.IsSatisfied(",", "string"))
            {
                defValue = Component.CastObject(inl[-1].Data.Substring(1, inl[-1].Data.Length - 2), paramsInfo.ParameterType);

                if (inl.IsSatisfied(",", "word"))
                {
                    hide = Convert.ToBoolean(inl[-1].Data);
                }
            }
            if (inl.IsSatisfied(",", "contains"))
            {
                validValues = initList(paramsInfo.ParameterType, inl, out inl);
            }
            else
            {
                if (it == AutoParameterInfoType.List)
                {
                    throw new ArgumentException("Обязательно должн быть задан список эелементов для List");
                }
            }
            if (!inl.IsSatisfied(")"))
                throw new ArgumentException("Невозможно разобрать");

            outl = inl;
            return new AutoParameterInfo(name, help, it, paramsInfo.ParameterType, defValue, hide, validValues);
        }


        public static object[] initList(Type parType, dsltools.LexemeStream lexs, out dsltools.LexemeStream end)
        {
            end = lexs;
            if (!lexs.IsSatisfied("("))
                throw new ArgumentException("Требуется перечисление аргументов");

#if !DOTNET_V11
            List<object> s = new List<object>();
#else
            ArrayList s = new ArrayList();
#endif

            while (lexs.IsMore)
            {
                if (lexs.IsSatisfied("string"))
                    s.Add(Component.CastObject(lexs[-1].Data.Substring(1, lexs[-1].Data.Length - 2), parType));
                else
                    throw new ArgumentException();

                if (lexs.IsSatisfied(")"))
                {
                    end = lexs;
#if !DOTNET_V11
                    return s.ToArray();
#else
                    return (object[])s.ToArray(typeof(object));
#endif
                }
                else if (lexs.IsSatisfied(","))
                    continue;

                break;
            }
            return null;
        }

        public static AutoParameterInfo[] ParseString(string dsl, System.Reflection.ParameterInfo[] paramsInfo)
        {
            AutoParameterInfo[] ret = new AutoParameterInfo[paramsInfo.Length];
            dsltools.LexemeStream w = new dsltools.LexemeStream(TupleDsl.Lex.Parse(dsl), 0);

            for (int i = 0; i < ret.Length; i++)
            {
                dsltools.LexemeStream ostr;
                ret[i] = ParseLexItem(w, out ostr, paramsInfo[i]);
                w = ostr;

                if (i < ret.Length - 1)
                    if (!w.IsSatisfied(","))
                        throw new ArgumentException("Невозможно разобрать разделитель параметров");
            }

            return ret;
        }

        public static int GetUnsatisfiedCount(AutoParameterInfo[] parameters, DataParamTable preInfo, out DataParamTable withDef)
        {            
            List<DataParamTableItem> items = new List<DataParamTableItem>();
            int count = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (preInfo.GetParamSafe(parameters[i].Name).IsNull)
                {
                    if (parameters[i].DefValue == null)
                        count++;
                    else
                        items.Add(new DataParamTableItem(parameters[i].Name, AnyValue.FromBoxedValue(parameters[i].DefValue)));
                }
                else
                {
                    items.Add(new DataParamTableItem(parameters[i].Name, preInfo[parameters[i].Name]));
                }
            }

            withDef = new DataParamTable(preInfo.Info, items);
            return count;
        }
    }

    public class ConstructorParameterNotFound : Exception
    {

        public Type Type
        {
            get { return _iType; }
        }

        public string Name
        {
            get { return _pname; }
        }

        public Component Component
        {
            get { return _c; }
        }

        public ConstructorParameterNotFound(Component c, string n, Type t)
        {
            _pname = n;
            _iType = t;
            _c = c;
        }

        public override string ToString()
        {
           return string.Format(
               "Невозможно найти параметр {0} типа {1}\r\nКомпонент '{2}'\r\n{3}",
               Name, Type.ToString(), Component.Info.HumanDescribe, base.ToString());
        }

        Type _iType;
        string _pname;
        Component _c;
    }

    public delegate bool CheckTypeCompatibility(Type t);



    public class Plugins
    {
        bool _glogal;
        bool _invalid = true;
        protected string _startWith;
        protected Type _baseType;
        protected Type[] _plugins;

		protected CheckTypeCompatibility _dCheck = new CheckTypeCompatibility(Check);

        public struct Enumarator
        {
            int _idx;
            Type[] _pugins;
            public Enumarator(Plugins p)
            {
                _idx = -1;
                _pugins = p._plugins;
            }
            public Type Current
            {
                get { return _pugins[_idx]; }
            }

            public bool MoveNext()
            {
                if (++_idx < _pugins.Length)
                    return true;
                return false;
            }
        }

        public Enumarator GetEnumerator()
        {
            Update();

            return new Enumarator(this);
        }

        void Update()
        {
            if (_invalid)
            {
                _invalid = false;
                _plugins = Component.GetAllTypes(_baseType, _startWith, _dCheck);
            }
        }

        public object Create(string classname)
        {
            return Create(classname, null);
        }

        public object Create(string classname, DataParamTable table)
        {
            Update();

            string sn = classname.StartsWith(_startWith) ? classname : _startWith + classname;
            Type t = null;
            foreach (Type i in _plugins)
            {
                if (i.Name == sn)
                {
                    t = i;
                    break;
                }
            }

            if (t != null)
            {
                bool defaultConstr;
                ConstructorInfo indfo = FindConstructor(t, out defaultConstr);
                if (indfo == null)
                    throw new ArgumentException();
                
                if (defaultConstr)
                    return indfo.Invoke(null);
                else
                    return indfo.Invoke(new object[] { table });
            }
            else
                throw new ArgumentException();
        }

        static ConstructorInfo FindConstructor(Type t, out bool def)
        {
            ConstructorInfo i = t.GetConstructor(new Type[] { typeof(DataParamTable) });
            if (i == null)
            {
                def = true;
                i = t.GetConstructor(Type.EmptyTypes);
            }
            else
            {
                def = false;
            }
            return i;
        }

        static bool Check(Type t)
        {
            bool i;
            return FindConstructor(t, out i) != null;
        }

        public Plugins(Type basetype, string startWith)
            : this (basetype, startWith, false)
        {

        }
        public Plugins(Type basetype, string startWith, bool globalSearch)
        {
            _baseType = basetype;
            _startWith = startWith;
            _glogal = globalSearch;            

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);         
        }

        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            _invalid = true;
        }
    }

    public abstract class Component : IComponent
    {

		static protected CheckTypeCompatibility _dDefCheckTypeCompatibility = new CheckTypeCompatibility(DefCheckTypeCompatibility);

        static bool DefCheckTypeCompatibility(Type t)
        {
            return (t.GetConstructors().Length > 0);
        }

        public static Type[] GetAllTypes(Type basetype, string startsWith)
        {
            return GetAllTypes(basetype, startsWith, _dDefCheckTypeCompatibility);
        }

        public static Type[] GetAllTypes(Type basetype, string startsWith, CheckTypeCompatibility check)
        {
#if !DOTNET_V11
            List<Type> res = new List<Type>();
#else
            ArrayList res = new ArrayList();
#endif

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.FullName.StartsWith("mscorlib"))
                    continue;
                if (a.FullName.StartsWith("System"))
                    continue;
                try
                {
                    foreach (Type t in a.GetExportedTypes())
                    {
                        if (basetype.IsAssignableFrom(t))
                        {
                            if (startsWith != null)
                            {
                                if (!t.Name.StartsWith(startsWith))
                                    continue;
                            }

                            if (check != null)
                            {
                                if (!check(t))
                                    continue;
                            }

                            res.Add(t);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            Type[] result = new Type[res.Count];
            res.CopyTo(result);
            return result;
        }


        #region IComponent Members

        public Type ComponentType
        {
            get { return _type; }
        }

        #endregion

        public static Object CastObject(Object ob, Type tp)
        {
            if (ob is IConvertible)
                return Convert.ChangeType(ob, tp);
            else if (tp.IsAssignableFrom(ob.GetType()))
                return ob;
            else
                throw new InvalidCastException();
        }


        public virtual IComponentInfo Info
        {
            get
            {
                return _info;
            }
        }

        #region IComponent Members

        public abstract object Create(string iname, DataParamTable parameters);

        #endregion
#if !DOTNET_V11
        internal static bool Create(Type t, DataParamTable parameters, out Object obj, out Dictionary<string, Type>[] notSatysfied) 
#else
        internal static bool Create(Type t, DataParamTable parameters, out Object obj, out Hashtable[] notSatysfied) 
#endif
        {
            ConstructorInfo[] cis = t.GetConstructors();
            notSatysfied = null;
            obj = null;
            bool successed = false;


            for (int j = 0; j < cis.Length; j++)
            {
                ConstructorInfo ci = cis[j];

                ParameterInfo[] pis = ci.GetParameters();
                Object[] param = new Object[pis.Length];
                Boolean complited = true;

                for (int i = 0; i < pis.Length; i++)
                {
                    Object ob = (parameters!= null) ? parameters.GetParamSafe(pis[i].Name).Value : null;
                    if (ob == null)
                    {
                      /* if (cis.Length == 1)
                         {
                               throw new ConstructorParameterNotFound(this, pis[i].Name, pis[i].ParameterType);
                         }*/

                        if (notSatysfied == null)
                        {
#if !DOTNET_V11
                            notSatysfied = new Dictionary<string, Type>[cis.Length];
#else
                            notSatysfied = new Hashtable[cis.Length];
#endif
                        }

                        if (notSatysfied[j] == null)
                        {
#if !DOTNET_V11
                            notSatysfied[j] = new Dictionary<string, Type>();
#else
                            notSatysfied[j] = new Hashtable();
#endif
                        }

                        notSatysfied[j].Add(pis[i].Name, pis[i].ParameterType);

                        complited = false;
                        //break;
                    }

                    if (complited)
                        param[i] = CastObject(ob, pis[i].ParameterType);
                        
                }
                /*if (t.Name == "AkgoSqlProvider")
                    throw new Exception("error");*/

                if (complited)
                {
                    /*******************/
                    /*if (t.Name == "AkgoSqlProvider")
                    {
                        obj = ci.Invoke(param);
                        successed = true;throw new Exception("error");
                        
                        break;
                    }*/
                    /*******************/
                    //Perform call
                    try
                    {
                        obj = ci.Invoke(param);
                        successed = true;
                        break;
                    }
                    catch (TargetInvocationException e)
                    {

                        // TODO Add more carefully 
                        throw e.InnerException;
                    }
                }
            }
            return successed;
        }

#if !DOTNET_V11
        internal static string ComposeHumaneErrorInfo(Dictionary<string, Type>[] e)
#else
        internal static string ComposeHumaneErrorInfo(Hashtable[] e)
#endif
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Невозможно создать объект:\r\n");
            if (e != null)
            {
                for (int i = 0; i < e.Length; i++)
                {
                    if (e[i] != null)
                    {
                        sb.AppendFormat("Конструктор {0}\r\n", i);

                        foreach (string param in e[i].Keys)
                        {
                            sb.AppendFormat("    Параметр {0} типа {1}\r\n", param, e[i][param]);
                        }

                        sb.Append("\r\n");
                    }
                }
            }
            else
            {
                sb.Append("не найден ни один конструктор");
            }
            return sb.ToString();
        }

        public virtual Object Create(DataParamTable parameters)
        {
#if !DOTNET_V11
            Dictionary<string, Type>[] e;
#else
            Hashtable[] e;
#endif
            Object obj;
            bool res = Create(_type, parameters, out obj, out e);
            if (!res)
            {
                int j = -1;
                for (int i = 0; i < e.Length; i++)
                {
                    if ((e[i] != null) && e[i].Count == 1)
                    {
                        j = i;
                        break;
                    }
                }

                if (j >= 0)
                {
#if !DOTNET_V11
                    IEnumerator<string>  dirk = e[j].Keys.GetEnumerator();
                    IEnumerator<Type>  dirt = e[j].Values.GetEnumerator();
#else
                    IEnumerator dirk = e[j].Keys.GetEnumerator();
                    IEnumerator dirt = e[j].Values.GetEnumerator();
#endif
                    dirk.MoveNext();
                    dirt.MoveNext();
                    
                    throw new ConstructorParameterNotFound(this, (string)dirk.Current,
                        (Type)dirt.Current);
                }

                throw new ArgumentException(ComposeHumaneErrorInfo(e));
            }
            return obj;          
        }

        public virtual String Name
        {
            get
            {
                return _type.Name;
            }
        }

        public delegate Component CreateDelegate();

        public static IEnumerable Load(String path, Type attributeType, CreateDelegate cd)
        {
            
            if (path.IndexOf("\\corelib.dll") != -1)
                path = typeof(IComponentInfo).Module.FullyQualifiedName;
            /*
            StreamWriter sw = new StreamWriter(File.Open("C:\\tmp\\dll.log",
                              System.IO.FileMode.Append));
            int i = 0;
            */
            Assembly a;
            Type[] types;

            try
            {
                a = Assembly.LoadFrom(path);
                types = a.GetTypes();
            }
            catch
            {
                return new object[0];
            }
            /**/
            //sw.WriteLine(path);
            //sw.WriteLine();
            /**/
            /*if(path == ".\\AkgoSqlDb.dll")
                throw new Exception("error");*/

            ArrayList stack = new ArrayList();

            foreach (Type t in types)
            {
                /*мои строки*/
                //i++;
                //sw.WriteLine(i.ToString() + ") " +t.Name + ",   " + t.FullName + ";");
                int ppp;
                IDataComponentInfo[] minf4;
                if (t.Name == "DataTupleProvider")
                {
                    ppp = 1;
                    //IComponent ic = null;
                    //IComponent[] ic_attrs = (IComponent[])t.GetCustomAttributes(typeof(IComponent), true);
                    //throw new Exception("error");
                    minf4 = (IDataComponentInfo[])t.GetCustomAttributes(typeof(IDataComponentInfo), true);
                }
                Object[] minf0 = (Object[])typeof(IComponentInfo).GetCustomAttributes(true);
                Object[] minf1 = t.GetCustomAttributes(true);
                Object[] minf2 = t.GetCustomAttributes(typeof(IComponentInfo),true);
                Object[] minf3 = t.GetCustomAttributes(typeof(AttributeDataComponent), true);
                Object[] minf5 = t.GetCustomAttributes(false);
                string sss = Directory.GetCurrentDirectory();
                
                /*мои строки*/
                IComponentInfo info = null;
                IComponentInfo[] attrs = (IComponentInfo[])t.GetCustomAttributes(typeof(IComponentInfo), true);
                //IComponentInfo[] attrs = (IComponentInfo[])t.GetCustomAttributes(true);
                if (attrs.Length > 0)
                {
                    info = attrs[0];
                    //sw.WriteLine("/***************/");
                    //sw.WriteLine("Строка сработала");
                    //sw.WriteLine("/***************/");
                }
                /*if (t.Name == "DataTupleProvider")
                    throw new Exception("error");*/
                /*if (t.Name == "SqliteProvider")
                {
                    int a123 = 1;
                    throw new Exception("error");
                }*/
                /*if (t.Name == "AkgoSqlProvider")
                    throw new Exception("error");*/
                if (info == null)
                    continue;

                Component dc = cd();
                dc._asm = a;
                dc._info = info;
                dc._type = t;
                dc._filename = path;

                // FIXME Add exception check!!!
                dc.Init();

                stack.Add(dc);

                
            }
            //sw.WriteLine();
            //sw.WriteLine();
            //sw.Close();
            return stack;

        }

        internal abstract void Init();

        internal Component()
        {
        }

        protected Component(Assembly asm, IComponentInfo info, Type t, String filename)
        {
            _asm = asm;
            _info = info;
            _type = t;
            _filename = filename;
        }
        protected Component(IComponentInfo info, Type t)
        {
            _asm = t.Assembly;
            _info = info;
            _type = t;
            _filename = _asm.Location;
        }

        private Assembly _asm;
        private IComponentInfo _info;
        private Type _type;
        private String _filename;

    }


    public class DataComponents : IDataComponents
    {
        #region IDataComponents Members

        public IDataComponent Find(string name)
        {
#if DOTNET_V11
            return (IDataComponent)_components[name];
#else
            DataComponent c;
            _components.TryGetValue(name, out c);
            return c;
#endif
        }

        public IDataComponent this[string index]
        {
            get { return Find(index); }
        }

        #endregion

        #region IComponents Members

        public IEnumerable Components
        {
            get { return _components.Values; }
        }

        IComponent IComponents.Find(string name)
        {
            return (IComponent)_components[name];
        }

        #endregion


        public DataComponents(string path)
        {
            string put = Directory.GetCurrentDirectory();
            //File.Copy("C:\\Windows\\SysWOW64\\corelib.dll", "C:\\sundown-hg\\Service_P1\\bin\\Debug\\corelib.dll", true);
            //File.Copy("C:\\Windows\\SysWOW64\\corelib.dll", path + "\\corelib.dll", true);
            string[] files = Directory.GetFiles(path, "*.dll");
            //throw new Exception("wr5hg");
            foreach (string file in files)
            {
				Component.CreateDelegate cd = new Component.CreateDelegate(DataComponent.CreateComponent);
                foreach (DataComponent c in Component.Load(file, typeof(AttributeDataComponent), cd))
                {
                    ConstructorInfo[] cns = c.ComponentType.GetConstructors();
                    if (cns.Length == 1)
                        ((AttributeDataComponent)c.Info).ParametrsInfoMethod = cns[0].GetParameters();

                    _components.Add(c.Name, c);
                    /*if (file == ".\\corelib.dll") 
                        throw new Exception("провал!");*/
                }
            }
        }

        public IDataResource CreateSuperResource(string resourceName, string instanceName, DataParamTable parameters)
        {
            IDataComponent comp = Find(resourceName);
            if (comp == null)
                throw new Exception("Can't find resource");

            return (IDataResource)comp.Create(instanceName, parameters);
        }

        public class DataComponent : Component, IDataComponent
        {
            #region IDataComponent Members

            public new IDataComponentInfo Info
            {
                get { return (IDataComponentInfo)base.Info; }
            }

            public new IDataProvider Create(DataParamTable parameters)
            {
                return (IDataProvider)base.Create(parameters);
            }

            #endregion

            public override object Create(string iname, DataParamTable parameters)
            {
                return new DataResource(iname, this, parameters);
            }
            

            internal static Component CreateComponent()
            {
                return new DataComponent();
            }

            internal DataComponent()
            {
            }

            public DataComponent(Type t)
                : base((IComponentInfo)(t.GetCustomAttributes(typeof(IDataComponentInfo), true)[0]), t)
            {
                Init();
            }

            internal override void Init()
            {
                if (typeof(IMultiDataProvider).IsAssignableFrom(ComponentType))
                {
                    _isMultiProvider = true;
                }
                else if (typeof(ISingleDataProvider).IsAssignableFrom(ComponentType))
                {
                    _isMultiProvider = false;
                }
                else
                {
                    throw new Exception("Unknown data component!");
                }
            }

            public bool IsMultiProvider
            {
                get { return _isMultiProvider; }
            }

            public override string ToString()
            {
                return ComponentType.Name;
            }

            bool _isMultiProvider;
        }

#if DOTNET_V11
        private Hashtable _components = new Hashtable();
#else
        private Dictionary<string, DataComponent> _components = new Dictionary<string, DataComponent>();
#endif

        public class DataResource : IDataResource
        {
            private bool disposed = false;
            private string _iname;

            protected DataComponent _component;
            protected IMultiDataProvider _dprovider;

            // For single only
            protected MultiDataProviderOverSingle _doprovider;

            public event IDataResourceDisposedEvent OnDisposed;

            public DataResource(string name, DataComponent component, DataParamTable parameters)
            {
                _iname = name;
                _component = component;

                if (!_component.IsMultiProvider)
                    _dprovider = _doprovider = new MultiDataProviderOverSingle((ISingleDataProvider)_component.Create(parameters));
                else
                    _dprovider = (IMultiDataProvider)_component.Create(parameters);
            }


            public IMultiDataProvider GetMultiProvider()
            {
                if (!disposed)
                    return _dprovider;
                else
                    throw new ObjectDisposedException("_dprovider");
            }

            public ISingleDataProvider GetSingleProvider()
            {
                if (disposed == false)
                    return _doprovider.ISingleDataProvider;
                else
                    throw new ObjectDisposedException("_doprovider");
            }

            public IMultiDataTuple GetConstData()
            {
                if (_doprovider != null)
                {
                    return GetSingleProvider().GetData();
                }
                else
                {
                    return GetMultiProvider().GetData(new DateTime(), TupleMetaData.StreamConst);
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                if (!this.disposed)
                {
                    disposed = true;

                    try
                    {
                        if (OnDisposed != null)
                            OnDisposed(this);
                    }
                    finally
                    {
                        _dprovider.Dispose();
                        _dprovider = null;
                        _doprovider = null;
                    }
                }
            }

            #endregion

            #region IResource Members

            public virtual IRules Rules
            {
                get { throw new NotSupportedException(); }
            }

            public string InstanceName
            {
                get { return _iname; }
            }

            #endregion

            #region IBaseResource Members

            public virtual void Send(IMultiDataTuple data)
            {
                throw new NotSupportedException();
            }

            public virtual IMultiDataTuple Receive()
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IResource Members

            public string ResourceName
            {
                get { return _component.Name; }
            }

            #endregion

        }

    }

    public class AttributeDataComponent : Attribute, IDataComponentInfo
    {
        #region IComponentInfo Members

        public string HumanDescribe
        {
            get { return _humanInfo; }
        }

        #endregion

        #region IDataComponentInfo Members

        public string[] AvailableSources
        {
            get
            {
                if ((_origNames != null) && (_availableNames == null))
                {
                    _availableNames = _origNames.Split(',');
                }
                return _availableNames;
            }
        }

        public bool MultiTupleOutput
        {
            get { return _multiTuple; }
        }

        #endregion

        public string SourceNames
        {
            get { return _origNames; }
            set { _origNames = value; }
        }

        public bool MultiTuple
        {
            get { return _multiTuple; }
            set { _multiTuple = value; }
        }

        public string ComponentFileFilter
        {
            get { return _fileFilter; }
            set { _fileFilter = value; }
        }

        public string ComponentFileNameArgument
        {
            get { return _componentFileNameArgument; }
            set { _componentFileNameArgument = value; }
        }

        public string ParametrsInfoString
        {
            get { return _parametrInfoString;}
            set { _parametrInfoString = value;}
        }

        public ParameterInfo[] ParametrsInfoMethod
        {
            set { _parametersMethodInfo = value; }
        }

        public bool IsParametrsInfoMethodSet
        {
            get { return _parametersMethodInfo != null; }        
        }

        public AutoParameterInfo[] GetParametrsInfo()
        {
            if (ParametrsInfoString == null)
                return null;

            return AutoParameterInfo.ParseString(ParametrsInfoString, _parametersMethodInfo);
        }

        public AttributeDataComponent(string humanInfo)
        {
            _humanInfo = humanInfo;
            /*if (humanInfo == "Чтение среза данных")
                throw new Exception("error");*/
        }

        private ParameterInfo[] _parametersMethodInfo;
        private string _parametrInfoString;
        private string _componentFileNameArgument;
        private string _humanInfo;
        private string[] _availableNames;
        private string _origNames;
        private bool _multiTuple = false;
        private string _fileFilter = "";

        #region IDataComponentInfo Members

        public string FileFilter
        {
            get { return _fileFilter; }
        }

        #endregion
    }

    
}