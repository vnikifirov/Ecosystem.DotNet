using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Globalization;

namespace corelib
{
    abstract public class SingleDataConverter : IInfoFormatter
    {
        public abstract Type GetSingleType();

        public abstract object GetValue(object o);


        #region IInfoFormatter Members

        public abstract string GetString(object o);

        public abstract string GetStringQuoted(object o);

        public int ColumntCount
        {
            get { return 1; }
        }

        static readonly string[] _names =  { "Значение" };

        public string[] ColumnNames
        {
            get { return _names; }
        }

        public Type[] RelutingTypes
        {
            get { return new Type[] { GetSingleType() }; }
        }

        public string[] GetStrings(object o)
        {
            return new string[] { GetString(o) };
        }

        public object[] GetValues(object o)
        {
            return new object[] { GetValue(o) };
        }

        public Type RelutingType
        {
            get { return GetSingleType(); }
        }

        #endregion
    }

    public class DataNumericConverter : SingleDataConverter
    {
        public override string GetString(object obj)
        {
            return obj.ToString();
        }

        public override string GetStringQuoted(object obj)
        {
            return GetString(obj);
        }

        public override Type GetSingleType()
        {
            return null;
        }

        public override object GetValue(object o)
        {
            return o;
        }
    }

    public class DataFloatConverter : SingleDataConverter
    {
        static  CultureInfo _cultures = new CultureInfo("en-US");

        public static CultureInfo DefFloatCulture
        {
            get { return _cultures; }
        }

        public override string GetString(object obj)
        {
            return System.Convert.ToString(obj, _cultures);
        }

        public override string GetStringQuoted(object obj)
        {
            return GetString(obj);
        }

        public override Type GetSingleType()
        {
            return null;
        }

        public override object GetValue(object o)
        {
            return o;
        }
    }

    public class DataFloatConverter2 : DataFloatConverter
    {
        public override string GetString(object obj)
        {
            return String.Format(DefFloatCulture, "{0:0.00}", obj);
        }
    }
    public class DataFloatConverter4 : DataFloatConverter
    {
        public override string GetString(object obj)
        {
            return String.Format(DefFloatCulture, "{0:0.0000}", obj);
        }
    }


    public class DataStringConverter : SingleDataConverter
    {
        public override string GetString(object obj)
        {
            if (obj == null)
                return String.Empty;

            return obj.ToString();
        }

        public static string QuoteString(string s)
        {
            s.Replace("\"", "\\\"");
            return "\"" + s + "\"";
        }

        public override string GetStringQuoted(object obj)
        {
            return QuoteString(GetString(obj));
        }

        public override Type GetSingleType()
        {
            return typeof(string);
        }

        public override object GetValue(object o)
        {
            return GetString(o);
        }
    }

    public class DataFibersConverter : IInfoFormatter
    {
        #region IInfoFormatter Members

        public int ColumntCount
        {
            get { return 2; }
        }

        public string[] ColumnNames
        {
            get { return new string[] { "Нитка", "ПВК" }; }
        }

        public Type[] RelutingTypes
        {
            get { return new Type[] { typeof(int), typeof(int) }; }
        }

        public Type RelutingType
        {
            get { return typeof(string); }
        }

        public string[] GetStrings(object o)
        {
            FiberCoords c = (FiberCoords)o;
            if (c.IsValid)
                return new string[] { (1 + c.Fiber).ToString(), (1 + c.Pvk).ToString() };
            else
                return new string[] { "[нет]", "" };
        }

        public string GetString(object o)
        {
            FiberCoords c = (FiberCoords)o;
            if (c.IsValid)
                return String.Format("Нитка {0} ПВК {1}", c.Fiber + 1, c.Pvk + 1);
            else
                return "[нет]";
        }

        public string GetStringQuoted(object o)
        {
            return DataStringConverter.QuoteString(GetString(o));
        }

        public object[] GetValues(object o)
        {
            FiberCoords c = (FiberCoords)o;
            if (c.IsValid)
                return new object[] { c.Fiber + 1, c.Pvk + 1 };
            else
                return new object[] { -1, -1 };
        }

        public object GetValue(object o)
        {
            //FiberCoords c = (FiberCoords)o;
            return GetString(o);
        }

        #endregion
    }

    public class CartViewInfoProviderSensored : IInfoFormatter
    {
        ScaleIndex _scale;

        public CartViewInfoProviderSensored(ScaleIndex c)
        {
            _scale = c;
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

            return new string[] { String.Format(DataFloatConverter.DefFloatCulture, "{0:0.00}", val),
                                  s.Value.ToString(), s.IsOk ? "" : s.State.ToString() };
        }

        public string GetString(object o)
        {
            return String.Format(DataFloatConverter.DefFloatCulture, "{0:0.00} [{1}]{2}", GetValues(o));
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

    public class CartViewInfoProviderDouble : SingleDataConverter
    {
        ScaleIndex _scale;

        public CartViewInfoProviderDouble(ScaleIndex scale)
        {
            _scale = scale;
        }

        public override string GetString(object o)
        {
            return String.Format(DataFloatConverter.DefFloatCulture, "{0:0.00}", _scale.Scale(o));
        }

        public override string GetStringQuoted(object o)
        {
            return GetString(o);
        }

        public override Type GetSingleType()
        {
            return typeof(double);
        }

        public override object GetValue(object o)
        {
            return _scale.Scale(o);
        }
    }

    public class CartViewInfoProviderStandard : SingleDataConverter
    {
        ScaleIndex _scale;

        public CartViewInfoProviderStandard(ScaleIndex scale)
        {
            _scale = scale;
        }

        public override string GetString(object o)
        {
            return _scale.Scale(o).ToString(DataFloatConverter.DefFloatCulture);
        }

        public override string GetStringQuoted(object o)
        {
            return GetString(o);
        }

        public override Type GetSingleType()
        {
            return typeof(double);
        }

        public override object GetValue(object o)
        {
            return _scale.Scale(o);
        }
    }
    
}
