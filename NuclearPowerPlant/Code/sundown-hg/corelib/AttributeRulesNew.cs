using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using dsltools;

/*
     IRules { ITupleRules(tozagr), ITupleRules(coeff[]), ITupleRules(return[]) }
 
  tozagr contains (zagr)  cast ( zagr as Cart(pvk,int,scale));
  coeff[] contains (kpd, bet1, gamma) cast (kpd as Array, bet1 as Array, gamma as ParamTable); 
  return[] contains (kpd, bet1, gamma) cast (kpd as Array, bet1 as Array, gamma as ParamTable)    
 */

/*
     ITupleRules { ISingleTupleRules  }
 
  
 */


/*
    ISingleTupleRules{ ITypeRules(pkd), ITypeRules(bet1), ITypeRules(gamma)   }
 
    coeff[]() contains (kpd, bet1, gamma) cast (kpd as Array, bet1 as Array, gamma as ParamTable)
 */

/*
    ITypeRules
 
    kpd as Array
 */


namespace corelib
{
    public class TupleDsl
    {
        static readonly ILexerRule[] rules = {
                               new LexerRuleKeyword("to", true),
                               new LexerRuleKeyword("as", true),
                               new LexerRuleKeyword("is", true),
                               new LexerRuleKeyword("contains", true),
                               new LexerRuleKeyword("cast", true),
                               new LexerRuleKeyword("["),
                               new LexerRuleKeyword("]"),
                               new LexerRuleKeyword("("),
                               new LexerRuleKeyword(")"),
                               new LexerRuleKeyword(","),
                               new LexerRuleKeyword(";"),
                               new LexerRuleKeyword("."),
                               new LexerRuleRegex("digit", @"\d+"),
                               new LexerRuleRegex("word", @"\w[\w|\d]*"),
                               new LexerRuleRegex("string", @"'[^']*'"),
                               new LexerRuleRegex("white", @"\s+", false)
                           };

        public static readonly Lexer Lex = new Lexer(rules, true);
    }

    public class AttributeTypeRules : ITypeRules
    {
        public readonly string Name;
        public readonly string HelpName;
        public readonly string TypeName;
        public readonly string Storage;

        string[] _oldNames;
        public readonly bool IsParametred;
        public readonly bool IsIndexed;

        static readonly string[] sNullNames = new string[0];

        string[] _storageeNames;

        public int StoragesCount
        {
            get { return (_storageeNames == null) ? -1 : _storageeNames.Length; }
        }
        public int Count
        {
            get { return (_oldNames == null) ? -1 : _oldNames.Length; }
        }
        public string this[int i]
        {
            get { return _oldNames[i]; }
        }

        #region ITypeRules Members

        string ITypeRules.GetHelpName()
        {
            return HelpName;
        }

        string ITypeRules.GetStorage()
        {
            return Storage;
        }

        string ITypeRules.GetTypeString()
        {
            return TypeName;
        }

        string[] ITypeRules.GetCastDetails()
        {
            return _oldNames;
        }

        string[] ITypeRules.GetStorages()
        {
            return _storageeNames;
        }

        string ITypeRules.GetName()
        {
            return Name;
        }

        #endregion

        public AttributeTypeRules(string name, string type)
            : this (name, null, type, false, false, null, null, null)
        {

        }

        protected AttributeTypeRules(string name, string helpName, string type, bool isPar, bool indexed, string[] names, string storage, string[] storagesList)
        {
            Name = name;
            HelpName = helpName != null ? helpName : name;
            TypeName = type;
            IsParametred = isPar;
            _oldNames = (names != null) ? names : sNullNames;
            _storageeNames = storagesList; //storagesList != null ? storagesList : sNullNames;
            Storage = storage;
            IsIndexed = indexed;
        }

        public AttributeTypeRules(string rule)
        {
            LexemeStream outStream;
            AttributeTypeRules obj = init(new LexemeStream(TupleDsl.Lex.Parse(rule), 0), out outStream);
            if (obj == null)
                throw new ArgumentException("Невозможно разобрать");
            if (outStream.IsMore)
                throw new ArgumentException(String.Format("Невозможно разобрать `{0}`", rule.Substring(outStream.Current.StartIdx)));

            Name = obj.Name;
            TypeName = obj.TypeName;
            IsParametred = obj.IsParametred;
            _oldNames = obj._oldNames;
            Storage = obj.Storage;
        }

        public static AttributeTypeRules init(LexemeStream lexs, out LexemeStream end)
        {
            end = lexs;
            string helpName = null;

            if (lexs.IsSatisfied("word", "(", "string", ")", "as", "word"))
                helpName = end[2].Data.Substring(1, end[2].Data.Length - 2);
            else if (!lexs.IsSatisfied("word", "as", "word"))
                return null;

            string name = end[0].Data;
            string type = lexs[-1].Data;

            bool indexing;
            string[] list = initList(lexs, out lexs, out indexing);
            string storage = null;
            string[] listStorages = null;

            if (lexs.IsSatisfied("to", "word"))
            {
                storage = lexs[-1].Data;
            }
            else if (lexs.IsSatisfied("to"))
            {
                listStorages = initList(lexs, out lexs, out indexing);
                if (listStorages == null)
                    throw new ArgumentException("Неверная последовательность хранилища");
            }

            end = lexs;
            return new AttributeTypeRules(name, helpName, type, list != null, indexing, list, storage, listStorages);
        }

        public static string[] initList(LexemeStream lexs, out LexemeStream end, out bool indexing)
        {
            indexing = false;
            end = lexs;
            if (!lexs.IsSatisfied("("))
                if (!lexs.IsSatisfied("["))
                    return null;
                else
                    indexing = true;
#if !DOTNET_V11
            List<string> s = new List<string>();
#else
            ArrayList s = new ArrayList();
#endif
            while (lexs.IsMore)
            {
                if (lexs.IsOr("word", "digit"))
                    s.Add(lexs[-1].Data);
                else
                    s.Add(null);

                if ((!indexing && lexs.IsSatisfied(")")) ||
                    (indexing && lexs.IsSatisfied("]")))
                {
                    end = lexs;
#if !DOTNET_V11
                    return s.ToArray();
#else
                    return (string[])s.ToArray(typeof(string));
#endif
                }
                else if (lexs.IsSatisfied(","))
                    continue;

                break;
            }
            return null;
        }
    }

    public class AttributeSingleTupleRules : ISingleTupleRules, ITupleRules
    {
        public readonly string TupleName;
        public readonly string TupleHelpName;
        public readonly bool IsArray;
        public readonly bool WithStorage;
        AttributeTypeRules[] _rules;

        public int Count
        {
            get { return _rules.Length; }
        }

        public AttributeTypeRules this[int i]
        {
            get { return _rules[i]; }
        }


        protected AttributeSingleTupleRules(string tupleName, string tupleHelpName, bool isArray, AttributeTypeRules[] rules)
        {
            TupleName = tupleName;
            TupleHelpName = tupleHelpName != null ? tupleHelpName : tupleName;
            IsArray = isArray;
            _rules = rules;

            WithStorage = rules[0].Storage != null || rules[0].StoragesCount > 0 ;
            for (int i = 1; i < rules.Length; i++)
                if ((rules[i].Storage != null || rules[i].StoragesCount > 0) != WithStorage)
                    throw new ArgumentException("Ошибка в правиле: хранилища должны быть указаня для всех или вообще не указаны");
        }

        public AttributeSingleTupleRules(string rule)
        {
            LexemeStream outStream;
            AttributeSingleTupleRules obj = init(new LexemeStream(TupleDsl.Lex.Parse(rule), 0), out outStream);
            if (obj == null)                
                throw new ArgumentException("Невозможно разобрать");

            if (outStream.IsMore)
                throw new ArgumentException(String.Format("Невозможно разобрать `{0}`", rule.Substring(outStream.Current.StartIdx)));

            TupleName = obj.TupleName;
            TupleHelpName = obj.TupleHelpName;
            IsArray = obj.IsArray;
            _rules = obj._rules;
        }


        public static AttributeSingleTupleRules init(LexemeStream lexs, out LexemeStream end)
        {
            AttributeSingleTupleRules w = initNew(lexs, out end);
            if (w == null)
                w = initOld(lexs, out end);
            return w;
        }

        public static AttributeSingleTupleRules initNew(LexemeStream lexs, out LexemeStream end)
        {
            end = lexs;

            bool array;
            string helpName = null;

            if (lexs.IsSatisfied("word", "[", "]"))
                array = true;
            else if (lexs.IsSatisfied("word"))
                array = false;
            else
                return null;

            if (lexs.IsSatisfied("(", "string", ")"))
                helpName = lexs[-2].Data.Substring(1, lexs[-2].Data.Length - 2);

            if (!lexs.IsSatisfied("is", "("))
                return null;

            string name = end[0].Data;
#if !DOTNET_V11
            List<AttributeTypeRules> s = new List<AttributeTypeRules>();
#else
            ArrayList s = new ArrayList();
#endif
            while (lexs.IsMore)
            {
                AttributeTypeRules tup = AttributeTypeRules.init(lexs, out lexs);
                if (tup == null)
                    return null;

                s.Add(tup);

                if (lexs.IsSatisfied(")"))
                    break;
                else if (lexs.IsSatisfied(","))
                    continue;

                return null;
            }

            end = lexs;
            return new AttributeSingleTupleRules(name, helpName, array,
#if !DOTNET_V11
                s.ToArray());
#else
                (AttributeTypeRules[])s.ToArray(typeof(AttributeTypeRules)));
#endif
        }

        public static AttributeSingleTupleRules initOld(LexemeStream lexs, out LexemeStream end)
        {
            end = lexs;

            bool array;
            if (!lexs.IsSatisfied("word", "contains"))
            {
                if (!lexs.IsSatisfied("word", "[", "]", "contains"))
                    return null;
                else
                {
                    array = true;
                }
            }
            else
                array = false;

            bool indexing;
            string[] list = AttributeTypeRules.initList(lexs, out lexs, out indexing);
            if (list == null)
                return null;

            string name = end[0].Data;
#if !DOTNET_V11
            List<AttributeTypeRules> s = new List<AttributeTypeRules>();
#else
            ArrayList s = new ArrayList();
#endif

            if (lexs.IsSatisfied("cast", "("))
            {
                while (lexs.IsMore)
                {
                    AttributeTypeRules tup = AttributeTypeRules.init(lexs, out lexs);
                    if (tup == null)
                        return null;

                    s.Add(tup);

                    if (lexs.IsSatisfied(")"))
                        break;
                    else if (lexs.IsSatisfied(","))
                        continue;

                    return null;
                }
            }

            end = lexs;

            AttributeTypeRules[] res = new AttributeTypeRules[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                foreach (AttributeTypeRules attr in s)
                {
                    if (list[i] == attr.Name)
                    {
                        res[i] = attr;
                        goto next;
                    }
                }
                res[i] = new AttributeTypeRules(list[i], null);
            next: ;
            }

            return new AttributeSingleTupleRules(name, null, array, res);
        }


        #region ISingleTupleRules Members

        string ISingleTupleRules.GetHelpName()
        {
            return TupleHelpName;
        }

        int ISingleTupleRules.GetStorageSpace()
        {
            return Count;
        }

        bool ISingleTupleRules.IsStorageSet()
        {
            return WithStorage;
        }

        string[] ISingleTupleRules.GetDesiredNames()
        {
            string[] ret = new string[_rules.Length];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = _rules[i].Name;

            return ret;
        }

        bool ISingleTupleRules.IsTypeInfoFor(string param)
        {
            for (int i = 0; i < _rules.Length; i++)
                if (_rules[i].Name == param)
                    return _rules[i].TypeName != null;

            return false;
        }

        ITypeRules ISingleTupleRules.GetTypeInfo(string param)
        {
            for (int i = 0; i < _rules.Length; i++)
                if (_rules[i].Name == param)
                    return _rules[i];

            throw new ArgumentException();
        }

        string ISingleTupleRules.GetTupleName()
        {
            return TupleName;
        }

        bool ISingleTupleRules.IsContainName(string name)
        {
            for (int i = 0; i < _rules.Length; i++)
                if (_rules[i].Name == name)
                    return true;

            return false;
        }

        bool ISingleTupleRules.IsTupleArray()
        {
            return IsArray;
        }

        string[] ISingleTupleRules.GetStorageNames()
        {
            string[] ret = new string[_rules.Length];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = _rules[i].Storage;

            return ret;
        }

        ITypeRules ISingleTupleRules.GetTypeInfo(int num)
        {
            return _rules[num];
        }

        #endregion

        #region ITupleRules Members

        string ITupleRules.TupleName
        {
            get { return TupleName; }
        }

        bool ITupleRules.IsArraied
        {
            get { return IsArray; }
        }

        IEnumerable ITupleRules.Rules
        {
            get { return null; }
        }

        ISingleTupleRules ITupleRules.Rule
        {
            get { return this; }
        }

        #endregion
    }


    public class AttributeRules : Attribute, IRules
    {
        AttributeSingleTupleRules[] _rules;

        public readonly AttributeSingleTupleRules ReturnRules;
        public readonly bool WithStorage;

        public int InputCount
        {
            get { return _rules.Length; }
        }

        public AttributeSingleTupleRules this[int i]
        {
            get { return _rules[i]; }
        }


        public AttributeRules(AttributeSingleTupleRules ret, AttributeSingleTupleRules[] rules)
        {
            ReturnRules = ret;
            _rules = rules;

            WithStorage = (ret != null) ? ret.WithStorage : rules[0].WithStorage;
          //  for (int i = 1; i < rules.Length; i++)
          //      if ((rules[i].WithStorage) != WithStorage)
          //          throw new ArgumentException("Ошибка в правиле: хранилища должны быть указаня для всех или вообще не указаны");
        }

        public AttributeRules(string rule)
        {
            AttributeRules obj = init(new LexemeStream(TupleDsl.Lex.Parse(rule), 0));
            if (obj == null)
                throw new ArgumentException("Невозможно разобрать");

            ReturnRules = obj.ReturnRules;
            _rules = obj._rules;
            WithStorage = obj.WithStorage;
        }

        public static AttributeRules init(LexemeStream lexs)
        {
#if !DOTNET_V11
            List<AttributeSingleTupleRules> s = new List<AttributeSingleTupleRules>();
#else
            ArrayList s = new ArrayList();
#endif
            while (lexs.IsMore)
            {
                AttributeSingleTupleRules tup = AttributeSingleTupleRules.init(lexs, out lexs);
                if (tup == null)
                    goto raise_error;

                s.Add(tup);

                if (lexs.IsSatisfied(";"))
                    continue;
                else if (!lexs.IsMore)
                    break;

                goto raise_error;
            }

            bool retPre = false;
            foreach (AttributeSingleTupleRules i in s)
            {
                if (i.TupleName == "return")
                {
                    if (retPre)
                        throw new ArgumentException();

                    retPre = true;
                }
            }

            AttributeSingleTupleRules returnType = null;
            AttributeSingleTupleRules[] ret = new AttributeSingleTupleRules[retPre ? s.Count - 1 : s.Count];
            int im = 0;
            foreach (AttributeSingleTupleRules i in s)
            {
                if (i.TupleName != "return")
                    ret[im++] = i;
                else
                    returnType = i;
            }

            return new AttributeRules(returnType, ret);

        raise_error:
            throw new ArgumentException(String.Format("Невозможно разобрать c `{0}`",
                lexs.Current.Data));
        }

        #region IRules Members

        public bool IsStorageSet
        {
            get { return WithStorage; }
        }

        public ITupleRules OutputRules
        {
            get { return ReturnRules; }
        }

        ITupleRules IRules.SingleInputRules
        {
            get { return (InputCount == 1) ? _rules[0] : null; }
        }

        public IEnumerable MultiInputRules
        {
            get { return _rules; }
        }

        public ITupleRules InputRules(string pname)
        {
            for (int i = 0; i < _rules.Length; i++)
                if (_rules[i].TupleName == pname)
                    return _rules[i];

            throw new ArgumentException();
        }

        #endregion
    }


    public class TupleMapsRule
    {
        public readonly string TargetStream;
        public readonly string OrigStream;
        public readonly string TargetName;
        public readonly string OrigName;

        public readonly int OrigIndex;

        protected TupleMapsRule(string origStream, string origName, int origIndex, string targetString, string targetName)
        {
            OrigStream = origStream;
            OrigName = origName;
            OrigIndex = origIndex;
            TargetStream = targetString;
            TargetName = targetName;

            if ((OrigName == null) && (origIndex < 0))
                throw new ArgumentException();
        }

        public static TupleMapsRule init(LexemeStream lexs, out LexemeStream end)
        {
            end = lexs;
            TupleMapsRule obj;

            if (lexs.IsSatisfied("word", ".", "word", "as", "word", ".", "word"))
                obj = new TupleMapsRule(end[0].Data, end[2].Data, -1, end[4].Data, end[6].Data);
            else if (lexs.IsSatisfied("word", ".", "word", "as", "word"))
                obj = new TupleMapsRule(end[0].Data, end[2].Data, -1, null, end[4].Data);
            else if (lexs.IsSatisfied("word", "as", "word", ".", "word"))
                obj = new TupleMapsRule(null, end[0].Data, -1, end[2].Data, end[4].Data);
            else if (lexs.IsSatisfied("word", "as", "word"))
                obj = new TupleMapsRule(null, end[0].Data, -1, null, end[2].Data);
            else if (lexs.IsSatisfied("[", "digit", "]", "as", "word", ".", "word"))
                obj = new TupleMapsRule(null, null, Convert.ToInt32(end[1].Data), end[4].Data, end[6].Data);
            else if (lexs.IsSatisfied("[", "digit", "]", "as", "word"))
                obj = new TupleMapsRule(null, null, Convert.ToInt32(end[1].Data), null, end[4].Data);
            else
                return null;

            end = lexs;
            return obj;
        }
    }

    public class TupleMaps : ITupleMaps
    {
        TupleMapsRule[] _maps;

        protected TupleMaps(TupleMapsRule[] maps)
        {
            _maps = maps;
        }

        public TupleMaps(string rule)
        {
            TupleMaps obj = init(new LexemeStream(TupleDsl.Lex.Parse(rule), 0));
            if (obj == null)
                throw new ArgumentException("Невозможно разобрать");

            _maps = obj._maps;
        }

        public static TupleMaps init(LexemeStream lexs)
        {
#if !DOTNET_V11
            List<TupleMapsRule> s = new List<TupleMapsRule>();
#else
            ArrayList s = new ArrayList();
#endif
            while (lexs.IsMore)
            {
                TupleMapsRule tup = TupleMapsRule.init(lexs, out lexs);
                if (tup == null)
                    goto raise_error;

                s.Add(tup);

                if (lexs.IsSatisfied(","))
                    continue;
                else if (!lexs.IsMore)
                    break;

                goto raise_error;
            }
#if !DOTNET_V11
            return new TupleMaps(s.ToArray());
#else
            return new TupleMaps((TupleMapsRule[])s.ToArray(typeof(TupleMapsRule)));
#endif

        raise_error:
            throw new ArgumentException(String.Format("Невозможно разобрать c `{0}`",
                lexs.Current.Data));
        }

        #region ITupleMaps Members

        public bool IsMapped(int num, out string ostream, out string oname)
        {
            for (int i = 0; i < _maps.Length; i++)
            {
                if (_maps[i].OrigIndex == num)
                {
                    ostream = _maps[i].TargetStream;
                    oname = _maps[i].TargetName;
                    return true;
                }
            }
            ostream = null;
            oname = null;
            return false;
        }

        public bool IsMapped(string istream, string iname, out string ostream, out string oname)
        {
            for (int i = 0; i < _maps.Length; i++)
            {
                if (_maps[i].OrigName == iname)
                    if ((_maps[i].OrigStream == null) || (istream != null && _maps[i].OrigStream == istream))
                    {
                        ostream = _maps[i].TargetStream;
                        oname = _maps[i].TargetName;
                        return true;
                    }
            }
            ostream = null;
            oname = null;
            return false;
        }

        public TupleMapMatch IsMappedTo(string istream, string iname, string ostream, string oname)
        {
            string nStream;
            string nName;

            bool res = IsMapped(istream, iname, out nStream, out nName);
            if (res)
            {
                if ((nName == oname) && (ostream == null || nStream == ostream))
                    return TupleMapMatch.Matched;
                else
                    return TupleMapMatch.Unmatched;
            }
            return TupleMapMatch.NoRule;
        }

        #endregion
    }
}
