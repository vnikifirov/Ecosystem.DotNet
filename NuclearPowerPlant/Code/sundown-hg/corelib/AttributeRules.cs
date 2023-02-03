using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

#if u
namespace corelib_deprecated
{

    public class AttributeTypeRules : ITypeRules
    {
        #region ITypeRules Members

        public string GetTypeString()
        {
            return _stringType;
        }

        public string[] GetCastDetails()
        {
            return _castDetails;
        }

        public string GetName()
        {
            return _param;
        }

        #endregion

        static Regex _def = new Regex(@"^\s*(?<a>\w+[\w\d]*)\s+as\s+(?<t>\w+[\w\d]*)\s*([\(|\[]\s*(?<c>[\w\d]*\s*(,\s*[\w\d]*\s*)*)[\)|\]])?\s*");

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetName());
            sb.Append(" as ");
            sb.Append(GetTypeString());
            sb.Append("(");
            for (int i = 0; i < _castDetails.Length; i++)
            {
                sb.Append(_castDetails[i]);
                if (i < _castDetails.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(")");
            return sb.ToString();
        }

        private void CreateFromPatch(string rule, out string residue)
        {
            //Match m = Regex.Match(rule, @"^\s*(?<a>\w+[\w\d]*)\s+as\s+(?<t>\w+[\w\d]*)\s*(\(\s*(?<c>[\w\d]+\s*(,\s*[\w\d]+\s*)*)\))?\s*");
            Match m = _def.Match(rule);

            _param = m.Groups["a"].Value;
            _stringType = m.Groups["t"].Value;
            string aattr = m.Groups["c"].Value;

            aattr = aattr.Trim();
            if (aattr.Length == 0)
                _castDetails = new string[0];
            else
                _castDetails = Regex.Split(aattr, @"\s*,\s*");

            if (m.Success)
            {
                residue = rule.Substring(m.Length).Trim();
            }
            else
            {
                throw new Exception("Can't parse!");
            }
        }

        public AttributeTypeRules(string rule)
        {
            string outStr;
            CreateFromPatch(rule, out outStr);
            if (outStr.Length != 0)
            {
                throw new Exception(String.Format("Can't parse residue {0}", outStr));
            }
        }

        public AttributeTypeRules(string rule, out string residue)
        {
            CreateFromPatch(rule, out residue);
        }

        private string _param;
        private string _stringType;
        //private Type _type;
        private string[] _castDetails;
    }


    public class AttributeSingleTupleRules : ISingleTupleRules
    {
        #region ITupleRules Members

        public string[] GetDesiredNames()
        {
            return _attributes;
        }

        public bool IsTypeInfoFor(string param)
        {
            return _typeRules.ContainsKey(param);
        }

        public ITypeRules GetTypeInfo(string param)
        {
            int idx = (int)_typeRules[param];
            return _rules[idx];
        }

        public ITypeRules GetTypeInfo(int idx)
        {
            return _rules[idx];
        }

        public string GetTupleName()
        {
            return _tupleName;
        }

        public bool IsContainName(string name)
        {
            foreach (string s in _attributes)
                if (s == name)
                    return true;
            return false;            
        }

        public bool IsTupleArray()
        {
            return _arrayOfTuple != ArrayTupleType.NoArray;
        }

        public string[] GetStorageNames()
        {
            return _storages;
        }

        #endregion



        public enum ArrayTupleType 
        {
            NoArray = 0,
            Numeric,
            Stream,
            WholeArray
        }

        public AttributeSingleTupleRules(string rule)
        {
            string outStr;
            CreateFromPatch(rule, out outStr);
            if (outStr.Length != 0)
            {
                throw new Exception(String.Format("Can't parse residue {0}", outStr));
            }
        }

        public AttributeSingleTupleRules(string rule, out string residue)
        {
            CreateFromPatch(rule, out residue);
        }
        static Regex _splitter = new Regex(@"\s*,\s*");
        static Regex _def = new Regex(@"^\s*(?<par>\w+)\s*(?<r>\[\s*((?<rd>\d*)|(?<rw>\w?[\w\d]*))\s*\])?\s+contains\s*\(\s*(?<attr>\w+[\w\d]+\s*(,\s*\w+[\w\d]+\s*)*)\)(\s*cast\s*\((?<cast>.+)\))?(\s*to\s*(?<store>.+)\s*)?");

        private void CreateFromPatch(string rules, out string residue)
        {
            //Match m = Regex.Match(rules, @"^\s*(?<par>\w+)\s*(?<r>\[\s*((?<rd>\d*)|(?<rw>\w?[\w\d]*))\s*\])?\s+contains\s*\(\s*(?<attr>\w+[\w\d]+\s*(,\s*\w+[\w\d]+\s*)*)\)(\s*cast\s*\((?<cast>.+)\))?\s*");
            Match m = _def.Match(rules);

            _tupleName = m.Groups["par"].Value;
            string attr = m.Groups["attr"].Value;
            string cast = m.Groups["cast"].Value;
            _attributes = _splitter.Split(attr);
            string array = m.Groups["r"].Value;
            string st = m.Groups["store"].Value;

            if (!m.Success)
                throw new Exception(String.Format("Couldn't parse: {0}", rules));

            residue = rules.Substring(m.Length).Trim();

            if (array.Length > 0)
            {
                string ard = m.Groups["rd"].Value;
                string arw = m.Groups["rw"].Value;

                if (ard.Length > 0)
                {
                    _indexInArray = Convert.ToInt32(ard);
                    _arrayOfTuple = ArrayTupleType.Numeric;
                }
                else if (arw.Length > 0)
                {
                    _streamInArray = arw;
                    _arrayOfTuple = ArrayTupleType.Stream;
                }
                else
                    _arrayOfTuple = ArrayTupleType.WholeArray;
            }
            else
            {
                _arrayOfTuple = ArrayTupleType.NoArray;
            }

            if (cast.Length > 0)
            {
                String proc = cast;
                //_rules = new AttributeTypeRules[cast.Length];

                ArrayList a = new ArrayList();
                while (proc.Length > 0)
                {
                    string oproc;
                    AttributeTypeRules r = new AttributeTypeRules(proc, out oproc);
                    proc = oproc;

                    if (!IsContainName(r.GetName()))
                        throw new Exception(String.Format("Couldn't find '{0}' in parameter's block", r.GetName()));

                    //_typeRules.Add(r.GetName(), r);
                    a.Add(r);

                    if (proc.Length > 0)
                    {
                        m = Regex.Match(proc, @"^\s*,");
                        if (!m.Success)
                            throw new Exception(String.Format("Couldn't parse cast part: {0}", proc));
                        proc = proc.Substring(m.Length);
                    }
                }

                _rules = (AttributeTypeRules[])a.ToArray(typeof(AttributeTypeRules));
                for (int i = 0; i< _rules.Length;i++)
                {
                    int idx = -1;
#if DOTNET_V11
                    obj s = _typeRules[_rules[i].GetName()];
                    if (s != null)
                        idx = (int)obj;
#else
                    if (!_typeRules.TryGetValue(_rules[i].GetName(), out idx))
                        idx = -1;
#endif

                    if (idx == -1)
                        _typeRules.Add(_rules[i].GetName(), i);
                    else
                    {
                        _typeRules[_rules[i].GetName()] = -1;
                    }

                }
            }

            if (st.Length > 0)
            {
                _storages = _splitter.Split(st);
                if (_storages.Length != _rules.Length)
                    throw new ArgumentException("Incorrect rule");
            }
        }

        private string _tupleName;
        private string[] _attributes;
        private string[] _storages;

        private AttributeTypeRules[] _rules;

#if DOTNET_V11
        private Hashtable _typeRules = new Hashtable();
#else        
        private Dictionary<string, int> _typeRules = new Dictionary<string, int>();
#endif
        private ArrayTupleType _arrayOfTuple;
        private int _indexInArray;
        private string _streamInArray;
    }



    public class AttributeRules : Attribute, IRules
    {
        #region IRules Members

        public ITupleRules OutputRules
        {
            get { return _outRule; }
        }

        public ITupleRules SingleInputRules
        {
            get
            {
                if (_inRules.Count == 1)
                    foreach (AttributeTupleRules i in _inRules.Values)
                        return i;

                throw new Exception("There are many inputs");
            }
        }

        public ITupleRules InputRules(string pname)
        {
            return ((ITupleRules)_inRules[pname]);
        }


        public IEnumerable MultiInputRules
        {
            get { return _inRules.Values; }
        }

        #endregion

        public AttributeRules(string rule)
        {
            string[] procs = Regex.Split(rule, @"\s*;\s*");

            foreach (string proc in procs)
            {
                if (proc.Length == 0)
                    continue;

                AttributeSingleTupleRules r = new AttributeSingleTupleRules(proc);
                AttributeTupleRules rt = null;

                string tn = r.GetTupleName();
                if (tn == "return")
                {
                    if (_outRule == null)
                        _outRule = new AttributeTupleRules(r);
                    else
                        _outRule.AddTupleRule(r);
                }
                else
                {
                    //if (_inRules == null)
                    //    _inRules = new Hashtable();
                    //else
                    //    rt = (AttributeTupleRules)_inRules[tn];

                    //if (rt == null)
                    if (!_inRules.ContainsKey(tn))
                    {
                        rt = new AttributeTupleRules(r);
                        _inRules.Add(tn, rt);
                    }
                    else
                    {
                        rt = (AttributeTupleRules)_inRules[tn];
                        rt.AddTupleRule(r);
                    }
                }
            }
        }




        public class AttributeTupleRules : ITupleRules
        { 
            #region ITupleRules Members

            public bool IsArraied
            {
                get
                {
                    if (_rules != null)
                        return true;
                    return _singleRule.IsTupleArray();
                }
            }

            /*public bool CheckCompatability(ITuple[] tuple)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool CheckCompatability(ITuple tuple)
            {
                throw new Exception("The method or operation is not implemented.");
            }
            */

            public IEnumerable Rules
            {
                get { return _rules; }
            }

            public ISingleTupleRules Rule
            {
                get { return _singleRule; }
            }

            public string TupleName
            {
                get {
                    if (_singleRule != null)
                        return _singleRule.GetTupleName();
                    else
                        return ((AttributeSingleTupleRules)_rules[0]).GetTupleName();
                }
            }

            #endregion

            public AttributeTupleRules(AttributeSingleTupleRules d)
            {
                _singleRule = d;
            }

            internal void AddTupleRule(AttributeSingleTupleRules r)
            {
                if (_singleRule != null)
                {
#if DOTNET_V11
                    _rules = new ArrayList();
#else
                    _rules = new List<AttributeSingleTupleRules>();
#endif
                    _rules.Add(_singleRule);
                    _singleRule = null;
                }
                _rules.Add(r);
            }

            
            private AttributeSingleTupleRules _singleRule;

#if DOTNET_V11
            private ArrayList _rules;
#else
            private List<AttributeSingleTupleRules> _rules;
#endif

            /*
            #region ITupleRules Members


            public ITuple CastTuple(ITuple tuple)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public ITuple[] CastTuple(ITuple[] tuple)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
            */
        }

#if DOTNET_V11
        private Hashtable _inRules = new Hashtable();
#else
        private Dictionary<string, ITupleRules> _inRules = new Dictionary<string, ITupleRules>();
#endif
        private AttributeTupleRules _outRule;

    }

}


namespace corelib
{
    public class TupleMaps : ITupleMaps
    {
        #region ITupleMaps Members

        public bool IsMapped(string istream, string iname, out string ostream, out string oname)
        {
            if (_inMaps.ContainsKey(istream + "." + iname))
            {
                TupleParamMap m = (TupleParamMap)_inMaps[istream + "." + iname];
                ostream = m.TargetStream;
                oname = m.Target;

                return true;
            }
            else if (_inMaps.ContainsKey(iname))
            {
                TupleParamMap m = (TupleParamMap)_inMaps[iname];
                ostream = m.TargetStream;
                oname = m.Target;

                return true;
            }

            ostream = istream;
            oname = iname;
            return false;
        }

        #endregion


        public TupleMaps(string rule)
        {
            String proc = rule;

            while (proc.Length > 0)
            {
                string oproc;
                TupleParamMap r = new TupleParamMap(proc, out oproc);
                proc = oproc;

                _inMaps.Add(r.FullName, r);

                if (proc.Length > 0)
                {
                    Match m = Regex.Match(proc, @"^\s*,");
                    if (!m.Success)
                        throw new Exception(String.Format("Couldn't parse cast part: {0}", proc));
                    proc = proc.Substring(m.Length);
                }
            }
        }

        public class TupleParamMap : ITupleParamMap
        {
            #region ITupleParamMap Members

            public string Original
            {
                get { return _orig; }
            }

            public string Target
            {
                get { return _target; }
            }

            public string OriginalStream
            {
                get { return _origStream; }
            }

            public string TargetStream
            {
                get { return _targetStream; }
            }

            #endregion

            public string FullName
            {
                get { return _oFname; }
            }

            private void CreateFromPatch(string rule, out string residue)
            {
                Match m = Regex.Match(rule, @"^\s*(?<f>((?<os>\w+[\w\d]*)\.)?(?<o>\w+[\w\d]*))\s+as\s+((?<ns>\w+[\w\d]*)\.)?(?<n>\w+[\w\d]*)\s*");
                _orig = m.Groups["o"].Value;
                _origStream = m.Groups["os"].Value;

                _target = m.Groups["n"].Value;
                _targetStream = m.Groups["ns"].Value;

                _oFname = m.Groups["f"].Value;
                residue = rule.Substring(m.Length).Trim();
            }

            internal TupleParamMap(string n, out string o)
            {
                CreateFromPatch(n, out o);
            }

            public TupleParamMap(string n)
            {
                string s;
                CreateFromPatch(n, out s);
                if (s.Length != 0)
                {
                    throw new Exception(String.Format("Can't parse residue {0}", s));
                }
            }

            private string _oFname;

            private string _orig;
            private string _target;

            private string _origStream;
            private string _targetStream;
        }

#if DOTNET_V11
        private Hashtable _inMaps = new Hashtable();
#else
        private Dictionary<string, TupleParamMap> _inMaps = new Dictionary<string, TupleParamMap>();
#endif
    }
}


#endif