using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;

namespace corelib
{

    public interface IGetCoordsConverter
    {
        CoordsConverter GetSpecialConverter(CoordsConverter.SpecialFlag flag, TupleMetaData reqestingData);
        CoordsConverter GetSpecialConverter(string name, TupleMetaData reqestingData);
    }

    public enum FormatterType
    {
        String,
        Real,
        Int,
        FiberCoords,
        Coords,
        RealHumane,
        RealHumane4
    };

    public interface IGetDataFormatter
    {
        IInfoFormatter GetDefFormatter(FormatterType type, ITupleItem it);
        IInfoFormatter GetDefFormatter(FormatterType type);
        IInfoFormatter GetFormatter(ITupleItem item);
    }
    
    public interface IGetCartPresentation
    {
        CartogramPresentationConfig DefCartPresentation { get; }
        bool GetCartPresentation(IDataCartogram c, out CartogramPresentationConfig cnf, out string className, out DataParamTable paramConstructor);
    }

    public struct CartogramPresentationConfig
    {
        int _value;

        public CartogramPresentationConfig(bool revertX, bool revertY, bool switchXY)
        {
            _value = (revertX ? 1 : 0) | (revertY ? 2 : 0) | (switchXY ? 4 : 0);
        }

        /// <summary>
        /// Преобразует первый бит <paramref name="val"/> в revertX, второй в revertY, и последний в switchXY.
        /// <seealso cref="CartogramPresentationConfig(bool,bool,bool)"/>
        /// </summary>
        /// <param name="val">Закодированное представление интерпретации</param>
        public CartogramPresentationConfig(int val)
        {
            _value = val;
        }

        public bool RevertX
        {
            get { return (_value & 1) != 0; }
        }

        public bool RevertY
        {
            get { return (_value & 2) != 0; }
        }

        public bool SwitchXY
        {
            get { return (_value & 4) != 0; }
        }

        public int Value
        {
            get { return _value; }
        }
    }

    public interface IEnviromentEx : IGetCoordsConverter, IGetDataFormatter, IGetCartPresentation
    {
        
    }

    public class EnvConverter : IGetCoordsConverter
    {
        protected CoordsConverter linear;
        protected CoordsConverter wlinear;
        protected CoordsConverter pvk;

        public EnvConverter(CartogramPresentationConfig cnf)
            : this (cnf.RevertX, cnf.RevertY, cnf.SwitchXY)
        {
        }

        public EnvConverter(bool revertX, bool revertY, bool switchXY)
        {
            linear = CoordsConverter.LoadLinear1884(revertX, revertY, switchXY);
            wlinear = CoordsConverter.LoadWideLinear2448(revertX, revertY, switchXY);
            pvk = null;
        }

        protected EnvConverter(CoordsConverter nlinear, CoordsConverter nwlinear, CoordsConverter npvk)
        {
            linear = nlinear;
            wlinear = nwlinear;
            pvk = npvk;
        }

        protected virtual void SetPVK(CoordsConverter pvk)
        {
            this.pvk = pvk;
        }

        public CoordsConverter Linear1884
        {
            get { return linear; }
        }

        public CoordsConverter WideLinear2448
        {
            get { return wlinear; }
        }

        public CoordsConverter PVK
        {
            get { return pvk; }
        }

        public virtual CoordsConverter GetSpecialConverter(CoordsConverter.SpecialFlag flag, TupleMetaData reqestingData)
        {
            if (flag == CoordsConverter.SpecialFlag.Linear1884)
                return linear;
            else if (flag == CoordsConverter.SpecialFlag.WideLinear2448)
                return wlinear;
            else if (flag == CoordsConverter.SpecialFlag.PVK)
                return pvk;
            else if (flag == CoordsConverter.SpecialFlag.Named)
                throw new ArgumentException("You should call GetSpecialConverter() with a name for named flag");

            return null;
        }

        public virtual CoordsConverter GetSpecialConverter(string name, TupleMetaData reqestingData)
        {
            throw new NotImplementedException();
        }
    }

    public class EnvConverterFormatter : EnvConverter, IEnviromentEx
    {
        static readonly IInfoFormatter defNum = new DataNumericConverter();
        static readonly IInfoFormatter defFloat = new DataNumericConverter();
        static readonly IInfoFormatter defStr = new DataNumericConverter();
        static readonly IInfoFormatter defFloat2 = new DataFloatConverter2();
        static readonly IInfoFormatter defFloat4 = new DataFloatConverter4();

        static readonly DataFibersConverter sDefFibers = new DataFibersConverter();

        protected DataCartogramCoordsFormatter defCoords;
        protected CartogramPresentationConfig defVisualPresentation;

        /// <summary>
        /// Задание парапетров интерпретации координат АЗ
        /// </summary>
        /// <param name="linearConf">Интерпретация последовательного перебора (где оси X и Y считать при последовательном обсчете ячеек)</param>
        /// <param name="visualConf">Интепретация отображения картограммы (где оси X и Y отображать на экране)</param>
        public EnvConverterFormatter(CartogramPresentationConfig linearConf, CartogramPresentationConfig visualConf)
            : base(linearConf)
        {
            defVisualPresentation = visualConf;            
        }

        #region IGetDataFormatter Members

        public virtual IInfoFormatter GetDefFormatter(FormatterType type, ITupleItem ti)
        {
            return GetDefFormatter(type);
        }

        public virtual IInfoFormatter GetDefFormatter(FormatterType type)
        {
            switch (type)
            {
                case FormatterType.Coords: 
                    return (defCoords == null) ? defCoords = new DataCartogramCoordsFormatter(this) : defCoords;
                case FormatterType.FiberCoords: return sDefFibers;
                case FormatterType.Int: return defNum;
                case FormatterType.Real: return defFloat;
                case FormatterType.RealHumane: return defFloat2;
                case FormatterType.RealHumane4: return defFloat4;

                default: return defStr;
            }
        }

        public virtual IInfoFormatter GetFormatter(ITupleItem item)
        {
            return item.GetDefForamtter(this);
        }

        #endregion

        #region IGetCartPresentation Members

        public virtual CartogramPresentationConfig DefCartPresentation
        {
            get { return defVisualPresentation; }
        }

        public virtual bool GetCartPresentation(IDataCartogram c, out CartogramPresentationConfig cnf, out string className, out DataParamTable paramConstructor)
        {
            cnf = defVisualPresentation;
            className = null;
            paramConstructor = null;
            return false;
        }

        #endregion
    }   
}
