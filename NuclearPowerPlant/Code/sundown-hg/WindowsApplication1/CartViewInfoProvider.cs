using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

using System.Diagnostics;
using System.Xml;

using System.IO;

namespace corelib
{
    public class CartViewInfoProviderZagr : IInfoFormatter
    {
        NodeZagrInfo[] _ni;

        struct NodeZagrInfo
        {
            public string _name;
            public string _descr;

            public void SetDefault()
            {
                _name = "[неизвестно]";
                _descr = "[неизвестно]";
            }

            public void Reset(string name, string descr)
            {
                _name = name;
                _descr = descr;
            }
        }

        public void Init(XmlNode root)
        {
            _ni = new NodeZagrInfo[256];
            for (int i = 0; i < _ni.Length; i++)
            {
                _ni[i].SetDefault();
            }

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.Name == "#comment")
                    continue;

                if (n.Name == "item")
                {
                    string id = n.Attributes["id"].Value;
                    string name = n.Attributes["name"].Value;
                    string descr = n.Attributes["descr"].Value;

                    int nId = Convert.ToInt32(id, 16);

                    if ((nId >= 0) && (nId < 256))
                    {
                        _ni[nId].Reset(name, descr);
                    }
                }
            }
        }

        public CartViewInfoProviderZagr(XmlNode root)
        {
            Init(root);
        }

        #region IInfoFormatter Members

        public int ColumntCount
        {
            get { return 3; }
        }

        static readonly string[] _names =  { "Тип", "Описание", "Код" };
        public string[] ColumnNames
        {
            get { return _names; }
        }

        public void GetInfo(int i, out string descr, out string name)
        {
            if ((_ni != null) && (i >= 0) && (i < 256))
            {
                descr = _ni[i]._descr;
                name = _ni[i]._name;
            }
            else
            {
                descr = String.Format("[код {0}]", i);
                name = descr;
            }
        }

        public string[] GetStrings(object o)
        {
            int i = Convert.ToInt32(o);
            string descr;
            string name;

            GetInfo(i, out descr, out name);

            return new string[] { name, descr, i.ToString() };
        }

        public string GetString(object o)
        {
            return String.Format("{0}: {1} [{2}]", GetStrings(o));
        }

        #endregion

        #region IInfoFormatter Members

        public Type[] RelutingTypes
        {
            get { return new Type[] { typeof(string), typeof(string), typeof(int) }; }
        }

        public string GetStringQuoted(object o)
        {
            return DataStringConverter.QuoteString(GetString(o));
        }

        public object[] GetValues(object o)
        {
            int i = Convert.ToInt32(o);
            string descr;
            string name;

            GetInfo(i, out descr, out name);

            return new object[] { name, descr, i };
        }

        public object GetValue(object o)
        {
            int i = Convert.ToInt32(o);
            string descr;
            string name;

            GetInfo(i, out descr, out name);

            return descr;
        }

        public Type RelutingType
        {
            get { return typeof(string); }
        }

        #endregion
    }
}
