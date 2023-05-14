using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;


namespace corelib
{
    public class DataCartogramFormatterSensored : IInfoFormatter
    {
        ScaleIndex _scale;
        IInfoFormatter _parent;

        public DataCartogramFormatterSensored(ScaleIndex c, IInfoFormatter parent)
        {
            _scale = c;
            _parent = parent;
        }

        #region IInfoFormatter Members

        public int ColumntCount
        {
            get { return 3; }
        }

        static readonly string[] _names = { "Значение", "СМкро", "Состояние" };
        public string[] ColumnNames
        {
            get { return _names; }
        }

        public string[] GetStrings(object o)
        {
            Sensored s = (Sensored)o;
            double val = _scale.Scale(s);

            return new string[] { _parent.GetString(val),
                                  s.Value.ToString(), s.IsOk ? "" : s.State.ToString() };
        }

        public string GetString(object o)
        {
            object[] vals = GetValues(o);
            return String.Format("{0} [{1}]{2}", _parent.GetString(vals[0]), vals[1], vals[2]);
        }

        #endregion

        #region IInfoFormatter Members


        public Type[] RelutingTypes
        {
            get { return new Type[] { typeof(double), typeof(int), typeof(string) }; }
        }

        public string GetStringQuoted(object o)
        {
            return DataStringConverter.QuoteString(GetString(o));
        }

        readonly static string[] sdiagnostics = {
            "", //Ok
            "Код1", // 1
            "Код2", // 2
            "Код3", // 3
            "Неточность", //SensorProhibition  4
            "Остутствует", //SensorAbsence     5
            "Код6", // 6
            "Код7", // 7
            "Код8", // 8
            "Код9", // 9
            "Код10", // 10
            "Код11", // 11
            "Код12", // 12
            "Код13", // 13
            "Код14", // 14
            "Код15" // 15
        };

        public object[] GetValues(object o)
        {
            Sensored s = (Sensored)o;
            double val = _scale.Scale(s);

            return new object[] { val, s.Value, s.IsOk ? "" : " " + sdiagnostics[(int)s.State] };
        }

        public object GetValue(object o)
        {
            Sensored s = (Sensored)o;
            return _scale.Scale(s);
        }

        public Type RelutingType
        {
            get { return typeof(double); }
        }

        #endregion
    }

    public class DataCartogramFormatter : SingleDataConverter
    {
        IInfoFormatter _parent;
        ScaleIndex _scale;

        public DataCartogramFormatter(ScaleIndex scale, IInfoFormatter parent)
        {
            _scale = scale;
            _parent = parent;
        }

        public override Type GetSingleType()
        {
            return typeof(double);
        }

        public override object GetValue(object o)
        {
            return _scale.Scale(o);
        }

        public override string GetString(object o)
        {
            return _parent.GetString(GetValue(o));
        }

        public override string GetStringQuoted(object o)
        {
            return _parent.GetStringQuoted(GetValue(o));
        }
    }

    public class DataCartogramCoordsFormatter : IInfoFormatter
    {
        readonly bool _showLinear1884;
        readonly bool _showLinear2448;
        readonly bool _showPvk;

        CoordsConverter _linear1884;
        CoordsConverter _wide2448;
        CoordsConverter _pvk;

        IInfoFormatter _fiber;

        public DataCartogramCoordsFormatter(IEnviromentEx env)
            : this(env, true, true, true)
        {

        }

        public DataCartogramCoordsFormatter(IEnviromentEx env, bool showLin1884, bool show2448, bool showPvk)
        {
            _linear1884 = env.GetSpecialConverter(CoordsConverter.SpecialFlag.Linear1884, new TupleMetaData());
            _wide2448 = env.GetSpecialConverter(CoordsConverter.SpecialFlag.WideLinear2448, new TupleMetaData());
            _pvk = env.GetSpecialConverter(CoordsConverter.SpecialFlag.PVK, new TupleMetaData());

            _showLinear1884 = showLin1884 && (_linear1884 != null);
            _showLinear2448 = show2448 && (_wide2448 != null);
            _showPvk = showPvk && (_pvk != null);

            _fiber = env.GetDefFormatter(FormatterType.FiberCoords);
        }

        #region IInfoFormatter Members

        public int ColumntCount
        {
            get
            {
                int count = 1;
                if (_showLinear1884) ++count;
                if (_showLinear2448) ++count;
                if (_showPvk) count += _fiber.ColumntCount;

                return count;
            }
        }

        public string[] ColumnNames
        {
            get
            {
                string[] res = new string[ColumntCount];
                int i = 0;

                res[i++] = "X-Y";
                if (_showLinear1884) res[i++] = "Индекс 1884";
                if (_showLinear2448) res[i++] = "Индекс 2448";
                if (_showPvk)
                {
                    string[] f = _fiber.ColumnNames;
                    for (int j = 0; j < _fiber.ColumntCount; j++)
                        res[i++] = f[j];
                }

                return res;
            }
        }

        public Type[] RelutingTypes
        {
            get
            { //return new Type[] { typeof(int), typeof(Coords) }; }
                Type[] res = new Type[ColumntCount];

                int i = 0;
                res[i++] = typeof(Coords);
                if (_showLinear1884) res[i++] = typeof(int);
                if (_showLinear2448) res[i++] = typeof(int);
                if (_showPvk)
                {
                    Type[] f = _fiber.RelutingTypes;
                    for (int j = 0; j < _fiber.ColumntCount; j++)
                        res[i++] = f[j];
                }

                return res;
            }
        }

        public Type RelutingType
        {
            get { return typeof(string); }
        }

        public string[] GetStrings(object o)
        {
            Coords c = (Coords)o;

            string[] res = new string[ColumntCount];

            int i = 0;

            if (c.IsOk)
            {
                res[i++] = c.ToString();
                if (_showLinear1884) res[i++] = _linear1884[c].ToString();
                if (_showLinear2448) res[i++] = _wide2448[c].ToString();
                if (_showPvk)
                {
                    string[] f = _fiber.GetStrings(_pvk.GetFiberCoords(c));
                    for (int j = 0; j < _fiber.ColumntCount; j++)
                        res[i++] = f[j];
                }
            }
            else
            {
                res[i++] = "[нет]";
                if (_showLinear1884) res[i++] = "";
                if (_showLinear2448) res[i++] = "";
                if (_showPvk)
                {
                    for (int j = 0; j < _fiber.ColumntCount; j++)
                        res[i++] = "";
                }
            }

            return res;
        }

        public string GetString(object o)
        {
            Coords c = (Coords)o;
            if (c.IsOk)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(c.ToString());
                if (_showLinear1884) sb.AppendFormat(" [{0}]", _linear1884[c]);
                if (_showLinear2448) sb.AppendFormat(" ||{0}||", _wide2448[c]);
                if (_showPvk)
                {
                    string f = _fiber.GetString(_pvk.GetFiberCoords(c));
                    sb.AppendFormat(" ({0})", f);
                }

                return sb.ToString();
            }
            else
                return "[нет]";
        }

        public string GetStringQuoted(object o)
        {
            return DataStringConverter.QuoteString(GetString(o));
        }

        public object[] GetValues(object o)
        {
            Coords c = (Coords)o;

            object[] res = new object[ColumntCount];

            int i = 0;

            res[i++] = c;
            if (_showLinear1884) res[i++] = _linear1884[c];
            if (_showLinear2448) res[i++] = _wide2448[c];
            if (_showPvk)
            {
                object[] f = _fiber.GetValues(_pvk.GetFiberCoords(c));
                for (int j = 0; j < _fiber.ColumntCount; j++)
                    res[i++] = f[j];
            }

            return res;
        }

        public object GetValue(object o)
        {
            Coords c = (Coords)o;
            return c.ToString();
        }

        #endregion
    }

}
