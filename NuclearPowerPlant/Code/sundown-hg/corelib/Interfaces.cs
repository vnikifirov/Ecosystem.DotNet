using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endif
using System.Text;

namespace corelib
{
    public interface ITimeData
    {
        DateTime GetTimeDate();
    }

    public interface IStreamInfo
    {
        string GetStreamName();
    }

    public interface ITupleMetaData : ITimeData, IStreamInfo
    {
        string GetName();
        String GetHumanName();

        string HumaneName { get; }
        string Name { get; }
        DateTime Date { get; }

        TupleMetaData Info { get; }
    }

    public interface IMultiTupleItem : ITimeData, IStreamInfo
    {
        string GetName();
        string Name { get; }

        int Count
        {
            get;
        }

        ITupleItem this[int idx]
        {
            get;
        }

        //IMultiTupleItem ReStream(string newname);
    }

    public interface IDuckTuple : ITimeData, IStreamInfo
    {
        string GetParamHelp(string name);

        object this[string name]
        {
            get;
        }
    }


    public interface IDataTupleInfo : ITimeData, IStreamInfo
    {
        /// <summary>
        /// Возвращает количество элементов в кортеже
        /// </summary>
        int ItemsCount
        {
            get;
        }

        void CopyNamesTo(string[] array);
        void CopyNamesTo(string[] arraym, int idx);

    }

    public interface IDataTuple : IDuckTuple, IDataTupleInfo
#if DOTNET_V11
        , IEnumerable
#else
        , IEnumerable<ITupleItem>
#endif
    {
        ITupleItem this[int idx]
        {
            get;
        }

        new ITupleItem this[string name]
        {
            get;
        }

        ITupleItem GetParam(string name);
        ITupleItem GetParamSafe(string name);

        ITupleItem[] GetData();

        IDataTuple CastTo(ISingleTupleRules rules);

        IDataCartogram GetCart(string name);
        IDataArray GetArray(string name);


        IDataTuple ReStream(string newname);
    }


    public interface IMultiDataTuple : IDataTupleInfo
    {
        /// <summary>
        /// Возвращает количестыо кортежей в мультекортеже
        /// </summary>
        int Count
        {
            get;
        }

        IDataTuple this[int idx]
        {
            get;
        }
        IMultiTupleItem this[string name]
        {
            get;
        }

        IMultiTupleItem GetItem(int idx);


        IMultiDataTuple ReStream(string newname);
    }


    public interface IInfoFormatter
    {
        int ColumntCount
        {
            get;
        }

        string[] ColumnNames
        {
            get;
        }

        Type[] RelutingTypes
        {
            get;
        }

        Type RelutingType
        {
            get;
        }

        string[] GetStrings(Object o);

        string GetString(Object o);
        string GetStringQuoted(Object o);

        object[] GetValues(Object o);
        object GetValue(Object o);
    }

    public interface IDataProvider : IDisposable
    {
        string[] GetExistNames();
    }

    public interface ISingleDataProvider : IDataProvider
    {
        IMultiDataTuple GetData();

        void PushData(IMultiDataTuple data);
    }

    public interface IMultiDataProvider : IDataProvider
    {
        /***********************************************************/
        /**************************Роман****************************/
        IMultiDataTuple GetConstData(string stream);
        /***********************************************************/
        /***********************************************************/

        IMultiDataTuple GetData(DateTime date, string[] names);

        IMultiDataTuple GetData(DateTime date, string stream);

        /// <summary>
        /// Возвращает всемозможные имена элементов в потоке
        /// </summary>
        /// <param name="stream">Имя потока</param>
        /// <returns>Имена элементов</returns>
        string[] GetAllDataNames(string stream);


        string[] GetDataNames(DateTime date, string stream);

        TupleMetaData GetTupleItemInfo(DateTime date, string name);

        TupleMetaData[,] GetTupleItemInfo(DateTime[] dates, string[] names);

        int GetMultiTuplesCount(DateTime date, string name);

        void GetMultiTuplesCount(DateTime[] dates, string name, out int[] count);

        DateTime[] GetDates();

        DateTime[] GetDates(string stream);

        /// <summary>
        /// Возвращает всевозможные имена потоков в базе
        /// </summary>
        /// <returns></returns>
        string[] GetStreamNames();

        void PushData(IMultiDataTuple data);

#if !DOTNET_V11
        void PushData(IList<IMultiDataTuple> datas);
#endif

        event MultiDataProviderGetDataEventHandler OnGetData;
        event MultiDataProviderGetDataEventHandler OnPushingData;
        event MultiDataProviderDataEventHandler OnPushedData;
    }

    public interface IBaseResource
    {
        void Send(IMultiDataTuple data);

        IMultiDataTuple Receive();
    }

    public interface IResource : IBaseResource
    {
        IRules Rules
        {
            get;
        }

        string InstanceName
        {
            get;
        }

        string ResourceName
        {
            get;
        }        
    }

    public delegate void IDataResourceDisposedEvent(IDataResource resource);
    public delegate void IResourceDisposedEvent(IResourceInstance resource);

    public interface IResourceInstance : IDisposable
    {
        //event IResourceDisposedEvent OnDisposed;
    }

    public interface IDataResource : IResource, IDisposable
    {
        IMultiDataProvider GetMultiProvider();

        IMultiDataTuple GetConstData();

        event IDataResourceDisposedEvent OnDisposed;
    }

    public interface IAlgoResource : IResource
    {
        IMultiDataTuple CallIntelli(ITupleMaps maps, params IMultiDataTuple[] objs);
        IMultiDataTuple CallIntelli(params IMultiDataTuple[] objs);
        IMultiDataTuple CallIntelli(IMultiDataTuple item);
    }

    public interface ISingleTupleRules
    {
        string GetHelpName();

        int GetStorageSpace();

        bool IsStorageSet();

        string[] GetDesiredNames();

        bool IsTypeInfoFor(string param);

        ITypeRules GetTypeInfo(string param);

        string GetTupleName();

        bool IsContainName(string name);

        bool IsTupleArray();

        string[] GetStorageNames();

        ITypeRules GetTypeInfo(int num);
    }

    public interface ITypeRules
    {
        string GetHelpName();

        string GetStorage();

        string[] GetStorages();

        string GetTypeString();

        string[] GetCastDetails();

        string GetName();
    }

    public interface ITupleRules
    {

        string TupleName
        {
            get;        
        }

        bool IsArraied
        {
            get;
        }

        IEnumerable Rules
        {
            get;
        }
        ISingleTupleRules Rule
        {
            get;
        }
    }

    public interface IRules
    {
        bool IsStorageSet
        {
            get;
        }

        ITupleRules OutputRules
        {
            get;
        }

        ITupleRules SingleInputRules
        {
            get;
        }


        IEnumerable MultiInputRules
        {
            get;
        }
     
        ITupleRules InputRules(string pname);

    }

    public interface ITupleParamMap
    {
        string Original
        {
            get;
        }

        string Target
        {
            get;
        }

        string OriginalStream
        {
            get;
        }

        string TargetStream
        {
            get;
        }
    }


    public interface IComponentInfo
    {
        String HumanDescribe
        {
            get;
        }
    }

    public interface IComponent
    {
        Object Create(string iname, DataParamTable parameters);
        String Name
        {
            get;
        }

        IComponentInfo Info
        {
            get;
        }

        Type ComponentType
        {
            get;
        }
    }

    public interface IDataComponent : IComponent
    {
        IDataProvider Create(DataParamTable parameters);

        new IDataComponentInfo Info
        {
            get;
        }
    }

    public interface IResourceComponent : IComponent
    {
        IRules Rules
        {
            get;
        }
    
        bool IsCachable();
    }

    public interface IAlgoResourceComponent : IResourceComponent
    {
        bool IsInstancable { get; }
        Type InstanceType { get; }
    }

    public interface IComponents
    {
        IEnumerable Components
        {
            get;
        }

        IComponent Find(string name);
    }

    public interface IDataComponents : IComponents
    {
        IDataComponent this[string index]
        {
            get;
        }

        new IDataComponent Find(string name);
    }

    public interface IResourceComponents : IComponents
    {
        IResourceComponent this[string index]
        {
            get;
        }

        new IResourceComponent Find(string name);
    }

    public interface IAlgoComponents : IResourceComponents
    {
        IResourceInstance CreateInstance(string resourceType, DataParamTable parameters);

        bool GetInstanceTypeName(string resourceType, out string instanceTypeName);

        new IAlgoResourceComponent Find(string name);
    }

    public interface IEnviromentSettings : IDisposable
    {
        string GetGlobalParam(string param);

        bool IsXmlParam(string param);

        System.Xml.XmlElement GetXmlParam(string param);

        DataParamTable ParamTuple { get; }

        DataComponents Data { get;}

        AlogComponents Algo { get; }
    }

    public interface IEnviroment : IEnviromentSettings, IEnviromentEx
    {

    }

    public interface ISerializeStream : ITupleMetaData
    {
        byte[] GetData();

        void PutStruct(ValueType obj);

        void Put(double val);

        void Put(float val);

        void Put(int val);

        void Put(Array array);

        void Put(short val);

        void Put(byte val);

        void Put(string str);

        void Put(Sensored val);

        void Put(Coords val);

        void Put(FiberCoords val);

        void Put(MultiIntFloat val);

#if !DOTNET_V11
        void Put<T>(T[] ar) where T : struct;
        void Put<T>(T[,] ar) where T : struct;
        void Put<T>(T[,,] ar) where T : struct;
#endif

    }



    public interface IDeserializeStream : ITupleMetaData
    {
        void Get(out double val);

        void Get(out float val);

        void Get(out int val);

        Array GetArray(Type arrayType, int x, int y, int z);

        object GetStruct(Type objType);

        void Get(out short val);

        void Get(out byte val);

        void Get(out Sensored val);

        void Get(out Coords val);

        void Get(out FiberCoords val);

        void Get(out MultiIntFloat val);

        void Get(out string str);

        void RestoryOriginal();

        void Rewind(int count);

        int GetPos();

#if !DOTNET_V11
        void Get<T>(T[] ar) where T : struct;
        void Get<T>(T[,] ar) where T : struct;
        void Get<T>(T[, ,] ar) where T : struct;
#endif
    }


    public interface ITupleItem : ITupleMetaData
    {
        void Serialize(ISerializeStream stream);

        ISerializeStream Serialize();

        ITupleItem CastTo(ITypeRules rules);

        ITupleItem Clone(string name, string humanName, DateTime date);

        ITupleItem Rename(string newParamName);

        String DumpCSV();

        IInfoFormatter GetDefForamtter(IGetDataFormatter env);

        DataGrid CreateDataGrid(IEnviromentEx env);

        object CastTo(IGetCoordsConverter en, ITypeRules rules);

        ITupleItem Rename(TupleMetaData newInfo);

        void Serialize(int abiver, ISerializeStream stream);
    }

    public interface IDataComponentInfo : IComponentInfo
    {
        AutoParameterInfo[] GetParametrsInfo();

        string ParametrsInfoString
        {
            get;
        }

        bool IsParametrsInfoMethodSet
        {
            get;
        }

        string FileFilter
        {
            get;
        }

        string[] AvailableSources
        {
            get;
        }

        bool MultiTupleOutput
        {
            get;
        }

        string ComponentFileNameArgument
        {
            get;
        }
    }

    public interface IAlgoComponentInfo : IComponentInfo
    {
        bool IsStatic { get; }

    }


    public enum TupleMapMatch
    {
        Matched,
        Unmatched,
        NoRule
    };

    public interface ITupleMaps
    {
        bool IsMapped(int num, out string ostream, out string oname);
        bool IsMapped(string istream, string iname, out string ostream, out string oname);
        TupleMapMatch IsMappedTo(string istream, string iname, string ostream, string oname);
    }

    public interface IDataArray : ITupleItem
    {
        int Rank { get; }

        int Length { get; }

        int DimX { get; }
        int DimY { get; }
        int DimZ { get; }


        bool IsByte { get; }
        bool IsShort { get; }
        bool IsInt { get; }
        bool IsDouble { get; }
        bool IsFloat { get; }
        bool IsSensored { get; }
        bool IsCoords { get; }
        bool IsFiberCoords { get; }
        bool IsMultiIntFloat { get; }

        object this[int index]
        {
            get;
        }
        object this[int x, int y]
        {
            get;
        }
        object this[int x, int y, int z]
        {
            get;
        }

        Type ElementType
        {
            get;
        }

        double Sum();
        double Mean();        
    }

    public interface IDataCartogram : ITupleItem
    {
        int Layers
        {
            get;
        }

        double this[Coords c, int layer]
        {
            get;
        }

        object GetObject(Coords c, int layer);

        CoordsConverter AllCoords
        {
            get;
        }

        bool IsNative
        {
            get;
        }

        bool IsValidCoord(Coords c);

        double GetScaledMaxMin(out double min, out double max);

        Type ElementType
        {
            get;
        }

        IDataCartogram RelativeDiff(IDataCartogram source);
        IDataCartogram AbsoluteDiff(IDataCartogram source);
    }


    public interface IDataParamTable : ITupleItem
    {
        int Count
        {
            get;
        }
            
    }
}
