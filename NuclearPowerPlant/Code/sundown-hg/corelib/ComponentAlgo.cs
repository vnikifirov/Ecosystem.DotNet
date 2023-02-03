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
    public class AttributeAlgoComponent : Attribute, IAlgoComponentInfo
    {
        #region IComponentInfo Members

        public string HumanDescribe
        {
            get { return _humanInfo; }
        }

        #endregion

        bool _static;

        public AttributeAlgoComponent(string humanInfo)
            : this(humanInfo, true)
        {

        }

        public AttributeAlgoComponent(string humanInfo, bool isStatic)
        {
            _static = isStatic;
            _humanInfo = humanInfo;
        }

        private string _humanInfo;

        #region IAlgoComponentInfo Members

        public bool IsStatic
        {
            get { return _static; }
        }

        #endregion
    }

    public class AlogComponents : IAlgoComponents
    {
        public bool GetInstanceTypeName(string resourceType, out string instanceTypeName)
        {
            instanceTypeName = null;
            IAlgoResourceComponent c = Find(resourceType);
            if (c == null)
                return false;

            if (c.IsInstancable)
            {
                instanceTypeName = c.InstanceType.Name;
                return true;
            }
            return false;
        }

        public IAlgoResourceComponent Find(string name)
        {
#if !DOTNET_V11
            AlgoComponent c;
            _components.TryGetValue(name, out c);
            return c;
#else
            return (IAlgoResourceComponent)_components[name];
#endif
        }

        #region IResourceComponents Members

        public IResourceComponent this[string index]
        {
            get { return Find(index); }
        }

        IResourceComponent IResourceComponents.Find(string name)
        {
            return Find(name);
        }

        #endregion

        #region IComponents Members

        public IEnumerable Components
        {
            get { return _components.Values; }
        }

        IComponent IComponents.Find(string name)
        {
            return (IResourceComponent)_components[name];
        }

        #endregion

        private void CheckCompatability(Type ptype, ITupleRules tupRules, string name)
        {
            if (tupRules == null)
                if (typeof(void) == ptype)
                    return;
                else
                    throw new Exception(String.Format("Can't find rule for parameter {0}", name));

            bool arr = tupRules.IsArraied;
            bool parr = ptype.IsArray;

            if (parr != arr)
                throw new Exception(String.Format("Rules for array {0} is incorrect", name));

            if (!parr && !typeof(IDataTuple).IsAssignableFrom(ptype))
                throw new Exception(String.Format("Parameter {0} cannot be cast to ITuple", name));
            else if (parr && !typeof(IDataTuple[]).IsAssignableFrom(ptype))
                throw new Exception(String.Format("Parameter {0} cannot be cast to ITuple[]", name));
        }

        void CheckReverse(AttributeRules ar, ParameterInfo[] parameters)
        {
            foreach (ITupleRules tupRules in ar.MultiInputRules)
            {
                bool found = false;
                foreach (ParameterInfo p in parameters)
                {
                    if (p.Name == tupRules.TupleName)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    throw new Exception(String.Format("Can't find parameter for rule {0}", tupRules.TupleName));
            }
        }

        void FullCheck(AttributeRules ar, ParameterInfo[] parameters, Type returnType)
        {
#if U
            // Check rules before
            foreach (ParameterInfo p in parameters)
                CheckCompatability(p.ParameterType, ar.InputRules(p.Name), p.Name);

            // Check output
            CheckCompatability(returnType, ar.OutputRules, "{return}");

            // Check reverse
            CheckReverse(ar, parameters);
#endif
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // FIXME Update checking logic
        }

        public AlogComponents(string path)
        {
            string[] files = Directory.GetFiles(path, "*.dll");
            //_components = new Hashtable();

            foreach (string file in files)
            {
                Assembly a;
                Type[] types;

                try
                {
                    a = Assembly.LoadFrom(file);
                    types = a.GetTypes();
                }
                catch
                {
                    continue;
                }

                foreach (Type t in types)
                {
                    bool containerPresent = typeof(IResourceInstance).IsAssignableFrom(t);

                    MethodInfo[] mi = t.GetMethods((containerPresent ? BindingFlags.Instance : BindingFlags.Static) | BindingFlags.Public | BindingFlags.InvokeMethod);
                    if (mi.Length == 0)
                        continue;

                    foreach (MethodInfo m in mi)
                    {
                        AttributeAlgoComponent ac = null;
                        AttributeRules ar = null;

                        foreach (Attribute attr in m.GetCustomAttributes(true))
                        {
                            if (ar == null)
                                ar = attr as AttributeRules;
                            if (ac == null)
                                ac = attr as AttributeAlgoComponent;

                            if ((ac != null) && (ar != null))
                                break;
                        }

                        if (ac == null)
                            continue;
                        if ((!containerPresent) && (!ac.IsStatic))
                            continue;

                        if (ar == null)
                            throw new Exception("Can't find rules for the resource");

                        FullCheck(ar, m.GetParameters(), m.ReturnType);

                        AlgoComponent algoc = new AlgoComponent(containerPresent, t, m, ar, ac);

                        _components.Add(algoc.Name, algoc);

                        //
                    }

                    if (containerPresent)
                    {
                        _nonStatic.Add(t.Name, t);
                    }
                }
            }
        }

        public IAlgoResource CreateResource(string resourceName, string instanceName, IEnviroment env, IResourceInstance instance)
        {
            AlgoComponent comp = (AlgoComponent)Find(resourceName);
            if (comp == null)
                throw new ArgumentException(String.Format("Компонент алгоритма '{0}' не найден", resourceName));

            return comp.Create(instanceName, null, env, instance);
        }

        public IAlgoResource CreateResource(string resourceName, string instanceName, IEnviroment env)
        {
            return CreateResource(resourceName, instanceName, env, null);
        }

        public class AlgoComponent : IAlgoResourceComponent
        {
            #region IResourceComponent Members

            public IRules Rules
            {
                get { return _rules; }
            }

            public bool IsCachable()
            {
                return _algoInfo.IsStatic;
            }

            public IAlgoResource Create(string name, DataParamTable parameters)
            {
                return new AlgoResource(name,
                    this,
                    parameters.GetParamSafe("enviromentObject").Value as IEnviroment,
                    parameters.GetParamSafe("instance").Value as IResourceInstance);
            }

            #endregion

            #region IComponent Members

            object IComponent.Create(string name, DataParamTable parameters)
            {
                return new AlgoResource(name,
                    this,
                    parameters.GetParamSafe("enviromentObject").Value as IEnviroment,
                    parameters.GetParamSafe("instance").Value as IResourceInstance);
            }

            #endregion

            public IAlgoResource Create(string name, DataParamTable parameters, IEnviroment env, IResourceInstance instance)
            {
                return new AlgoResource(name,
                    this,
                    env,
                    instance);
            }

            #region IComponent Members

            public string Name
            {
                get
                {
                    //if (IsStaticMethod)
                    return _methodInfo.Name;

                    //return _type.ToString() + "." + _methodInfo.Name;
                }
            }

            public IAlgoComponentInfo Info
            {
                get { return _algoInfo; }
            }

            public Type ComponentType
            {
                get { return _type; }
            }

            #region IAlgoResourceComponent Members

            public Type InstanceType
            {
                get { return (_contained) ? _type : null; }
            }

            public bool IsInstancable
            {
                get { return (_contained); }
            }
            #endregion

            #endregion
            #region IComponent Members

            IComponentInfo IComponent.Info
            {
                get { return _algoInfo; }
            }

            #endregion

            public bool IsStaticMethod
            {
                get { return _methodInfo.IsStatic; }
            }

            internal AlgoComponent(bool contained, Type type, MethodInfo mi, AttributeRules ar, IAlgoComponentInfo ai)
            {
                _contained = contained;
                _type = type;
                _methodInfo = mi;
                _rules = ar;
                _algoInfo = ai;

                _parameters = _methodInfo.GetParameters();
                _inParameters = _parameters.Length;

                //if (!ar.WithStorage)
                if (false)
                {
                    if (_inParameters > 1)
                    {
                        _cachedIndex = new Hashtable();
                        for (int i = 0; i < _parameters.Length; i++)
                        {
                            ParamTupleInfo pti;
                            pti.index = i;
                            pti.isArray = typeof(IDataTuple[]).IsAssignableFrom(_parameters[i].ParameterType);
                            pti.rules = _rules.InputRules(_parameters[i].Name);

                            _cachedIndex.Add(_parameters[i].Name, pti);
                        }
                    }
                    else if (_inParameters == 1)
                    {
                        _singleParamName = _parameters[0].Name;
                        _singleParamTi.index = 0;
                        _singleParamTi.isArray = typeof(IDataTuple[]).IsAssignableFrom(_parameters[0].ParameterType);
                        _singleParamTi.rules = _rules.InputRules(_parameters[0].Name);
                    }
                }
                else
                {
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // FIXME
                    // Update for storage usage

                    System.Diagnostics.Debug.WriteLine("Method unchecked: " + mi.ToString());
                    
                }

                if (typeof(IDataTuple[]).IsAssignableFrom(_methodInfo.ReturnType))
                    _arrayedOut = true;
                else
                    _arrayedOut = false;

                if (_methodInfo.ReturnType == typeof(void))
                {
                    _arrayedOut = Rules.OutputRules.IsArraied;
                }
            }

            internal struct ParamTupleInfo
            {
                public int index;
                public bool isArray;
                public ITupleRules rules;
            }

            private bool _contained;
            private Type _type;
            internal MethodInfo _methodInfo;
            internal ParameterInfo[] _parameters;

            private AttributeRules _rules;
            internal IAlgoComponentInfo _algoInfo;

            internal int _inParameters;

            internal string _singleParamName;
            internal ParamTupleInfo _singleParamTi;

            internal Hashtable _cachedIndex;

            internal bool _arrayedOut;
        }

        public IResourceInstance CreateInstance(string resourceType, DataParamTable parameters)
        {
            Type type;
#if !DOTNET_V11
            _nonStatic.TryGetValue(resourceType, out type);
#else
            type = (Type)_nonStatic[resourceType];
#endif
            if (type == null)
                throw new ArgumentException(String.Format("Интерфейс ресурс {0} не найден", resourceType));

#if !DOTNET_V11
            Dictionary<string, Type>[] e;
#else
            Hashtable[] e;
#endif
            Object obj;
            bool res = Component.Create(type, parameters, out obj, out e);
            if (!res)
            {
                throw new ArgumentException(Component.ComposeHumaneErrorInfo(e));
            }
            return (IResourceInstance)obj;
        }

#if DOTNET_V11
        private Hashtable _components = new Hashtable();
        private Hashtable _nonStatic = new Hashtable();
#else
        private Dictionary<string, AlgoComponent> _components = new Dictionary<string, AlgoComponent>();
        private Dictionary<string, Type> _nonStatic = new Dictionary<string, Type>();
#endif


        public class AlgoResource : IAlgoResource
        {
            #region IResource Members

            public IRules Rules
            {
                get { return _component.Rules; }
            }

            public string InstanceName
            {
                get { return _iname; }
            }

            public string ResourceName
            {
                get { return _component.Name; }
            }

            #endregion

            #region IBaseResource Members

            /*
            public void Send(ITuple data)
            {
                if (_component._inParameters == 1)
                {
                    if (_component._singleParamTi.isArray == true)
                    {
                        throw new Exception("Array data is supplied while algorithm needed scalar");
                    }
                    if (data.GetStreamName() != _component._singleParamName)
                    {
                        throw new Exception("Incorrect tuple supplied, if it's correct rename it before usage");
                    }

                    //CASTING
                    //FIXME
                    _inObjects[0] = data;
                }
                else if (_component._inParameters > 1)
                {
                    object o = _component._cachedIndex[data.GetStreamName()];
                    if (o == null)
                    {
                        throw new Exception("Incorrect tuple supplied, if it's correct rename it before usage");
                    }
                    AlgoComponent.ParamTupleInfo pti = (AlgoComponent.ParamTupleInfo)o;
                    if (pti.isArray == true)
                    {
                        throw new Exception("Array data is supplied while algorithm needed scalar");
                    }

                    //CASTING
                    //FIXME
                    _inObjects[pti.index] = data;
                }
                else
                {
                    throw new Exception("Incorrect usage");
                }
            }

            public void SendMulti(ITuple[] datas)
            {
                if (_component._inParameters == 1)
                {
                    if (_component._singleParamTi.isArray != true)
                    {
                        throw new Exception("Non-array data is supplied while algorithm needed multi data");
                    }
                    foreach (ITuple data in datas)
                        if (data.GetStreamName() != _component._singleParamName)
                        {
                            throw new Exception("Incorrect tuple supplied, if it's correct rename it before usage");
                        }

                    //CASTING
                    //FIXME
                    _inObjects[0] = datas;
                }
                else if (_component._inParameters > 1)
                {
                    string sname = datas[0].GetStreamName();
                    object o = _component._cachedIndex[sname];
                    if (o == null)
                    {
                        throw new Exception("Incorrect tuple supplied, if it's correct rename it before usage");
                    }
                    foreach (ITuple data in datas)
                        if (data.GetStreamName() != sname)
                        {
                            throw new Exception("Incorrect tuple in array supplied, if it's correct rename it before usage");
                        }

                    AlgoComponent.ParamTupleInfo pti = (AlgoComponent.ParamTupleInfo)o;
                    if (pti.isArray != true)
                    {
                        throw new Exception("Non-array data is supplied while algorithm needed multi data");
                    }

                    //CASTING
                    //FIXME
                    _inObjects[pti.index] = datas;
                }
                else
                {
                    throw new Exception("Incorrect usage");
                }
            }
            */

            #endregion

            private IMultiDataTuple InvokeMethod()
            {
                if (!_component.IsStaticMethod && (_obj == null))
                    throw new ArgumentException("Вызов нестатического алгоритма с нулевым объектом");

                return InvokeFillAlgorithmStorage();
            }

            public void Send(IMultiDataTuple item)
            {
                throw new NotImplementedException();
            }

            public IMultiDataTuple Receive()
            {
                return InvokeMethod();
            }

            public IMultiDataTuple CallIntelli(ITupleMaps maps, params IMultiDataTuple[] objs)
            {
                SendIntelli(maps, objs);
                return Receive();
            }
            public IMultiDataTuple CallIntelli(params IMultiDataTuple[] objs)
            {
                SendIntelli(null, objs);
                return Receive();
            }

            public IMultiDataTuple CallIntelli(IMultiDataTuple item)
            {
                return CallIntelli(null, new IMultiDataTuple[] { item });
            }


            public void SendIntelli(ITupleMaps maps, params IMultiDataTuple[] objs)
            {
                foreach (ITupleRules r in _component.Rules.MultiInputRules)
                {
                    ISingleTupleRules srule = r.Rule;
                    MultiDataTuple.CreateParamFromTuple(_env, _inObjects, _params, srule, maps, objs);
                }
            }

            internal IMultiDataTuple InvokeFillAlgorithmStorage()
            {
                object q = _component._methodInfo.Invoke(_obj, _inObjects);
                
                if (q == null)
                    return MultiDataTuple.CreateTupleFromParam(_env, Rules.OutputRules.Rule, _params, _inObjects);

                return q as IMultiDataTuple;
            }

            internal AlgoResource(string name, AlgoComponent cmp, IEnviroment env, IResourceInstance instance)
            {
                _obj = instance;
                _iname = name;
                _component = cmp;

                if (_component._parameters.Length > 0)
                    _inObjects = new object[_component._parameters.Length];

                _params = _component._methodInfo.GetParameters();
                _env = env;
            }

            private IEnviroment _env;
            private IResourceInstance _obj;
            private object[] _inObjects;

            private ParameterInfo[] _params;
            private AlgoComponent _component;
            private string _iname;
        }

    }




    interface ITask
    {
        bool DoWork();
        int GetProgress();
        void Cancel();
    }

    public class TaskLoad
    {
        ITask[] _tasks;
        int _remaining;

    }

    public class WorkingThread
    {
        int _state;
       
        ITask _baseTask;
    }

    public class WorkingThreadHost
    {
        WorkingThread[] _threads;


    }
}
