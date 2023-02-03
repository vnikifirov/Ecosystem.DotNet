using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Data;
using System.Data.Odbc;
using corelib;

namespace AkgoSqlPlugin
{
#if !DOTNET_V11
    using DataArrayInt = DataArray<int>;
#endif

    public class AkgoSqlDb : IDisposable
    {
        IDbCommand CreateCommand()
        {
            return conn.CreateCommand();
        }

        int FillRecordData(out int[] dt, int window, Int64 prpId)
        {
            if ((window < 0) || (window > 5))
                throw new ArgumentException("Incorrect window number");


            String query = String.Format("SELECT n, count{0} FROM prp_specs,spec WHERE (prp_specs.spec_id = spec.spec_id ) AND ( prp_id = ? )", window);
            IDbCommand cmd = conn.CreateCommand();

            IDataParameter prp_id = cmd.CreateParameter();
            cmd.Parameters.Add(prp_id);
            cmd.CommandText = query;

            prp_id.Value = prpId;

            cmd.Prepare();
            IDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                dt = new int[0];
                return -1;
            }
#if !DOTNET_V11
            dt = new int[reader.RecordsAffected];
            do
            {
                //int n = (int)reader[0];
                int n = reader.GetInt32(0);
                //Int64 c = (Int64)reader[1];
                Int64 c = reader.GetInt64(1);                
                dt[n] = (int)c;
            } while (reader.Read());
#else
			int count = 0;

			// FIXME !!
			int[] dt_tmp = new int[20000];
			do
			{
				int n = (int)reader.GetInt32(0);
				Int64 c = (Int64)reader.GetInt64(1);
				dt_tmp[n] = (int)c;
				count++;
			} while (reader.Read());
			
			dt = new int[count];
			for (int t = 0; t < count; t++)
			{
				dt[t] = dt_tmp[t];
			}

#endif
            reader.Close();
            //reader.Dispose();
            cmd.Dispose();
            return 0;
        }



        int FillRecordData(Int64 prpId, out int[] dt0, out int[] dt1, out int[] dt2, out int[] dt3, out int[] dt4)
        {
            String query = String.Format("SELECT n, count0, count1, count2, count3, count4 FROM prp_specs,spec WHERE (prp_specs.spec_id = spec.spec_id ) AND ( prp_id = ? )");
            IDbCommand cmd = conn.CreateCommand();

            IDataParameter prp_id = cmd.CreateParameter();
            cmd.Parameters.Add(prp_id);
            cmd.CommandText = query;

            prp_id.Value = prpId;

            cmd.Prepare();
            IDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                dt0 = new int[0];
                dt1 = new int[0];
                dt2 = new int[0];
                dt3 = new int[0];
                dt4 = new int[0];
                return -1;
            }
#if !DOTNET_V11
            dt0 = new int[reader.RecordsAffected];
            dt1 = new int[reader.RecordsAffected];
            dt2 = new int[reader.RecordsAffected];
            dt3 = new int[reader.RecordsAffected];
            dt4 = new int[reader.RecordsAffected];
            do
            {
                //int n = (int)reader[0];
                int n = reader.GetInt32(0);
                dt0[n] = (int)reader.GetInt64(1);
                dt1[n] = (int)reader.GetInt64(2);
                dt2[n] = (int)reader.GetInt64(3);
                dt3[n] = (int)reader.GetInt64(4);
                dt4[n] = (int)reader.GetInt64(5);
            } while (reader.Read());
#else
			int count = 0;

			// FIXME !!
			int[] dt_tmp = new int[20000];
			do
			{
				int n = (int)reader.GetInt32(0);
				Int64 c = (Int64)reader.GetInt64(1);
				dt_tmp[n] = (int)c;
				count++;
			} while (reader.Read());
			
			dt = new int[count];
			for (int t = 0; t < count; t++)
			{
				dt[t] = dt_tmp[t];
			}

#endif
            reader.Close();
            //reader.Dispose();
            cmd.Dispose();
            return 0;
        }

        ChannelType[] GetFiberChannelTypes(Int64 prpId)
        {
            ChannelType[] pvkLoad = new ChannelType[115];

            IDbCommand cmd = conn.CreateCommand();
            IDataParameter prp_id = cmd.CreateParameter();
            cmd.Parameters.Add(prp_id);
            cmd.CommandText = "SELECT n_pvk, kind FROM prp_pvk WHERE prp_pvk_id = ?";
            prp_id.Value = prpId;

            cmd.Prepare();
            IDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                cmd.Dispose();
                return null;
            }
            do
            {
                ChannelType ct = ChannelType.EMPTY;
                String k = reader.GetString(1);
                Int32 pvk = reader.GetInt32(0);

                if (k.CompareTo("tvs") == 0)
                {
                    ct = ChannelType.TVS;
                }
                else if (k.CompareTo("dp") == 0)
                {
                    ct = ChannelType.DP;
                }
                else if (k.CompareTo("sv") == 0)
                {
                    ct = ChannelType.WATER;
                }
                pvkLoad[pvk - 1] = ct;
            } while (reader.Read());

            reader.Close();
            //reader.Dispose();
            cmd.Dispose();

            return pvkLoad;
        }
        
        FiberRecord GetRecordByPrpId(RecordInfo rec, int window)
        {
            FiberRecord fr = new FiberRecord(rec);

            if (fr.recordInfo.FibNum >= 0)
            {
                if (FillRecordData(out fr.data, window, rec.PrpId) != 0)
                    //return null;
                    return fr;
            }
            else
            {
                // Вероятно ошибочная запись в БД
                fr.data = new int[0];
            }

            return fr;
        }

        FiberRecord GetRecordByPrpId(RecordInfo rec)
        {
            FiberRecord fr = new FiberRecord(rec);
            fr.all = new int[5][];

            if (fr.recordInfo.FibNum >= 0)
            {
                if (FillRecordData(rec.PrpId, out fr.all[0], out fr.all[1], out fr.all[2], out fr.all[3], out fr.all[4]) != 0)
                    return fr;
            }
            else
            {
                // Вероятно ошибочная запись в БД
                for (int i = 0; i < 5; i++)
                    fr.all[i] = new int[0];
            }
            return fr;
        }

        public AkgoSqlDb(IDbConnection connection, bool showOnlyFullFibers)
        {
            conn = connection;
            IDbCommand cmd = conn.CreateCommand();

            cmd.CommandText =
               @"SELECT NPP.system_id, NPP.name, BLK.system_id AS block_id, BLK.name AS block_name
                    FROM system as BLK, system as NPP  WHERE NPP.type='npp' AND NPP.owner_id IS NULL AND BLK.owner_id=NPP.system_id";

            IDataReader reader = cmd.ExecuteReader();
            //Rearrange structure
            while (reader.Read())
            {
                AkgoSystemInfo asi = new AkgoSystemInfo(this,
                    (Int64)reader[0],
                    (Int64)reader[2],
                    (String)reader[1],
                    Convert.ToInt64((String)reader[3]),
                    showOnlyFullFibers);

                systems.Add(asi);
            }
            reader.Close();
            cmd.Dispose();

            for (int i = 0; i < systems.Count; i++)
            {
                ((AkgoSystemInfo)systems[i]).Init();
            }
        }

        public int SystemCount
        {
            get { return systems.Count; }
        }

        public AkgoSystemInfo this[int index]
        {
            get { return (AkgoSystemInfo)systems[index]; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            conn.Dispose();
        }

        #endregion

        public void Close()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
        }

        private IDbConnection conn;
#if DOTNET_V11
        private ArrayList systems = new ArrayList();
#else
        private List<AkgoSystemInfo> systems = new List<AkgoSystemInfo>();
#endif

        public class FiberRecord
        {
            public RecordInfo recordInfo;
            public int[] data;
            public ChannelType[] pvkLoad;

            public int[][] all;

            public FiberRecord(RecordInfo ri)
            {
                recordInfo = ri;
                data = null;
                pvkLoad = null;
            }
        }

        public enum RecordType
        {
            PC,
            PRP,
            SRV,
            RELOAD,
            FAST_SRV,
            BRELOAD,
            ARELOAD,

            UNKNOWN
        }



        public class RecordInfo
        {
            internal readonly Int64 PrpId;
            internal readonly Int64 GrdId;
            internal readonly Int64 WindowId;

            public readonly RecordType Type;
            public readonly int FibNum;
            public readonly DateTime Dt;
            public readonly int Pvk;  // For single pvk only
			public readonly float SpecClock;

            public static RecordType FromString(string type)
            {
                switch (type.ToLower())
                {
                    case "prp":
                        return RecordType.PRP;
                    case "pc":
                        return RecordType.PC;
                    case "srv":
                        return RecordType.SRV;
                    case "reload":
                        return RecordType.RELOAD;
                    case "fast srv":
                        return RecordType.FAST_SRV;
                    case "breload":
                        return RecordType.BRELOAD;
                    case "areload":
                        return RecordType.ARELOAD;

                    default:
                        return RecordType.UNKNOWN;
                }
            }

            public RecordInfo(RecordType type, int fibNum, DateTime dt, float specTime, Int64 prpId, Int64 grdId, Int64 windowId)
                : this(type, fibNum, dt, specTime, prpId, grdId, windowId, -1)
            {                
            }

            public RecordInfo(RecordType type, int fibNum, DateTime dt, float specTime, Int64 prpId, Int64 grdId, Int64 windowId, int pvk)
            {
                Type = type;
                FibNum = fibNum;
                Dt = dt;
                PrpId = prpId;
				SpecClock = specTime;
                Pvk = pvk;
                GrdId = grdId;
                WindowId = windowId;
            }
        }

        public class AkgoSystemInfo
        {
            struct FiberIndex : IComparable, ICloneable
#if !DOTNET_V11
                , IComparable<FiberIndex>
#endif
            {
                public DateTime Date;
                public int FiberNum;

                public FiberIndex(DateTime date, int fiberNum)
                {
                    Date = date;
                    FiberNum = fiberNum;
                }

                public override int GetHashCode()
                {
                    return Date.GetHashCode() ^ FiberNum.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    return ((FiberIndex)obj) == this;
                }

                public static bool operator ==(FiberIndex r1, FiberIndex r2)
                {
                    return (r1.Date == r2.Date) && (r1.FiberNum == r2.FiberNum);
                }

                public static bool operator !=(FiberIndex r1, FiberIndex r2)
                {
                    return !(r1 == r2);
                }

                #region IComparable Members

                public int CompareTo(object obj)
                {
                    return CompareTo((FiberIndex)obj);
                }

                #endregion

                public int CompareTo(FiberIndex fi)
                {
                    int diff = this.Date.CompareTo(fi.Date);
                    if (diff == 0)
                        diff = this.FiberNum.CompareTo(fi.FiberNum);

                    return diff;
                }

                public override string  ToString()
                {
                    return String.Format("{0}@{1}", FiberNum, Date);
                }

                #region ICloneable Members

                public object Clone()
                {
                    return new FiberIndex(Date, FiberNum);
                }

                #endregion
            }


            public String NppName
            {
                get { return _nppName; }
            }

            public int NppBlock
            {
                get { return (int)_blockNum; }
            }

            public int UpdateCache()
            {
                _cachedIndividual.Clear();

                IDbCommand cmd = _conn.CreateCommand();
                IDbDataParameter nblock_id = cmd.CreateParameter();
                cmd.Parameters.Add(nblock_id);

				
                cmd.CommandText =
#if OLD_QUERY
                   @"select p.dt, p.prp_id, S.name, p.kind, p.time, pc.n_pvk, p.grd_id, p.windows_id from pc, prp as p,grd, system as S, system as K, system as B  
                                    where pc.pc_id = p.prp_id AND p.grd_id=grd.grd_id AND
                     S.system_id=grd.system_id AND S.owner_id=K.system_id AND K.owner_id=B.system_id AND B.owner_id=?
                              order by p.dt DESC ";
#else
 @"select p.dt, p.prp_id, S.name, p.kind, p.time, pc.n_pvk, p.grd_id, p.windows_id from pc right join prp as p on p.prp_id = pc.pc_id, grd, system as S, system as K, system as B
                                    where p.grd_id=grd.grd_id AND
                     S.system_id=grd.system_id AND S.owner_id=K.system_id AND K.owner_id=B.system_id AND B.owner_id=?
                              order by p.dt DESC";
#endif
				// Данные о номере ПВК хранятся в следущем виде, 1 ПВК соотвествует код 1, 2 - 2 и тд
                // 0 - скорее всего особый случай, когда положение ПВК меняется
                // Поэтому в RecordInfo они хранятся точно также !!! 
                // Необходимое смещение на 1 должно производиться в пользовательском коде

                nblock_id.Value = _blockId;
                cmd.Prepare();
                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RecordInfo ri = null;
                    DateTime dt = reader.GetDateTime(0);
                    Int64 prpId = reader.GetInt64(1);
                    int fiber = (int)Convert.ToInt64(reader[2]) - 1;
                    RecordType type = RecordInfo.FromString(reader[3].ToString());
                    float specTime = reader.GetFloat(4);
                    Int64 grdId = reader.GetInt64(6);
                    Int64 windowId = reader.GetInt64(7);
                    switch (type)
                    {
                        case RecordType.PC:
                            ri = new RecordInfo(type, fiber, dt, specTime, prpId, grdId, windowId, Convert.ToInt32(reader[5]));
                            break;
                        //case RecordType.SRV:
                        //case RecordType.FAST_SRV:
                        //case RecordType.RELOAD:
                        //case RecordType.ARELOAD:
                        //case RecordType.BRELOAD:
                        case RecordType.PRP:
                            ri = new RecordInfo(type, fiber, dt, specTime, prpId, grdId, windowId);
                            break;
                        default:
                            continue;
                    }
                    
#if !DOTNET_V11
                    RecordInfo tryDt;
                    if (_cachedIndividual.TryGetValue(new FiberIndex(dt, fiber), out tryDt))
#else
                    if (_cachedIndividual[new FiberIndex(dt, fiber)] != null)
#endif
                    {
                        // Duplicating record... hmm

                        System.Diagnostics.Debug.WriteLine(
                            String.Format("Duplicate at {1}@{0}", dt, fiber));
                    }
                    else
                    {
                        _cachedIndividual.Add(new FiberIndex(dt, fiber), ri);
                    }

                }

                reader.Close();
                cmd.Dispose();

                ComposePrpCache();
                ComposePcCache();

                return _cachedIndividual.Count;
            }
            

            public int ComposePrpCache()
            {
#if !DOTNET_V11
                List<FiberIndex> t = new List<FiberIndex>();
#else
                ArrayList t = new ArrayList();
#endif
                foreach (RecordInfo r in _cachedIndividual.Values)
                {
                    if (r.Type == RecordType.PRP)
                        t.Add(new FiberIndex(r.Dt, r.FibNum));
                }
                t.Sort();
                t.Reverse();
                FiberIndex[] dtn = new FiberIndex[t.Count];
                t.CopyTo(dtn, 0);

                _cachedSortedPrp = dtn;


                _cachedDtPrpAllThread.Clear();

                DateTime[] prp_ids = null;
                DateTime dt0 = new DateTime();
                DateTime dt;
                int i = 0;
                //bool newDate = false;
                bool newDate = true;
                bool first = true;

                bool[] newFlags = new bool[16];

                foreach (FiberIndex fidex in _cachedSortedPrp)
                {
                    dt = fidex.Date;

                    DateTime oldDt = new DateTime();
                    int found = 0;
                    DateTime[] oldPrpId = null;

                    if (first)
                    {
                        oldDt = dt;
                        dt0 = dt;
                    }

                    TimeSpan tm = dt0 - dt;
                    double delta = Math.Abs(tm.TotalSeconds);
                    if ((delta > DateThresouldPeriod) || (first) || newFlags[fidex.FiberNum])
                    {
                        found = i;
                        oldDt = dt0;
                        oldPrpId = prp_ids;
                        if (first)
                            first = false;
                        else
                            newDate = true;

                        i = 1;
                        dt0 = dt;
                        prp_ids = new DateTime[16];
                    }
                    else
                    {
                        newDate = false;
                        i++;
                    }

                    prp_ids[fidex.FiberNum] = fidex.Date;

                    if (newDate)
                    {
                        if ((found == 16) || (!_showOnly16Fibers))
                        {
                            _cachedDtPrpAllThread.Add(oldDt, oldPrpId);
                        }


                        for (int l = 0; l < 16; l++)
                            newFlags[l] = false;
                    }

                    newFlags[fidex.FiberNum] = true;
                }

                //if (newDate)
                //{
                    if ((i == 16) || (!_showOnly16Fibers))
                    {
                        _cachedDtPrpAllThread.Add(dt0, prp_ids);
                    }
                //}

                int retcount = _cachedDtPrpAllThread.Count;
                _cachedSortedPrpAllThreads = new DateTime[retcount];
                _cachedDtPrpAllThread.Keys.CopyTo(_cachedSortedPrpAllThreads, 0);

                Array.Sort(_cachedSortedPrpAllThreads);
                Array.Reverse(_cachedSortedPrpAllThreads);
                return _cachedDtPrpAllThread.Count;
            }

            public int ComposePcCache()
            {
                if (_cachedIndividual.Count == 0)
                    UpdateCache();

                ArrayList t = new ArrayList();
                foreach (RecordInfo r in _cachedIndividual.Values)
                {
                    if (r.Type == RecordType.PC)
                        t.Add(r.Dt);
                }
                t.Sort();
                t.Reverse();
                DateTime[] dt = new DateTime[t.Count];
                t.CopyTo(dt);
                _cachedSortedPcThread = dt;
                return dt.Length;
            }

            public DateTime[] GetPcDates()
            {
                //UpdateCache();

                return _cachedSortedPcThread;
            }

            public DateTime[] GetPrpDates()
            {
                //UpdateCache();

                return _cachedSortedPrpAllThreads;
            }

            public RecordInfo GetSingleRecordInfo(DateTime dt)
            {
                RecordInfo ri = null;

                for (int i = 0; i < 16; i++)
                {
#if !DOTNET_V11
                    if (_cachedIndividual.TryGetValue(new FiberIndex(dt, i), out ri))
                        return ri;
#else
                    ri = (RecordInfo)_cachedIndividual[new FiberIndex(dt, i)];
                    if (ri!=null)
                        return ri;
#endif

                }
                return ri;
            }

            public FiberRecord GetSingleAzotRecord(DateTime dt, bool addSubInfo)
            {
                return GetSingleRecord(0, dt, addSubInfo);
            }

            public FiberRecord GetSingleRecord(int window, DateTime dt, bool addSubInfo)
            {
                FiberRecord fr = null;
                RecordInfo ri = GetSingleRecordInfo(dt);
                switch (ri.Type)
                {
                    case RecordType.PRP:
                        fr = _conn.GetRecordByPrpId(ri, window);
                        fr.recordInfo = ri;
                        if (addSubInfo)
                            fr.pvkLoad = _conn.GetFiberChannelTypes(ri.PrpId);
                        return fr;
                    case RecordType.PC:
                        fr = _conn.GetRecordByPrpId(ri, window);
                        fr.recordInfo = ri;
                        return fr;
                }
                return fr;
            }

            public FiberRecord GetSingleRecord(DateTime dt, bool addSubInfo)
            {
                FiberRecord fr = null;
                RecordInfo ri = GetSingleRecordInfo(dt);
                switch (ri.Type)
                {
                    case RecordType.PRP:
                        fr = _conn.GetRecordByPrpId(ri);
                        fr.recordInfo = ri;
                        if (addSubInfo)
                            fr.pvkLoad = _conn.GetFiberChannelTypes(ri.PrpId);
                        return fr;
                    case RecordType.PC:
                        fr = _conn.GetRecordByPrpId(ri);
                        fr.recordInfo = ri;
                        return fr;
                }
                return fr;
            }

            static readonly Int64[] noneId = new Int64[0];


            private int GetFiberPrpCount(DateTime dt, out DateTime[] prp_ids)
            {
#if !DOTNET_V11
                if (!_cachedDtPrpAllThread.TryGetValue(dt, out prp_ids))
                    return 0;
#else
                prp_ids = (DateTime[])_cachedDtPrpAllThread[dt];
                if (prp_ids == null)
                    return 0;
#endif

                int cnt = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (prp_ids[i].Year > 1900)
                        cnt++;
                }

                return cnt;
            }

            private FiberRecord[] GetAllFiberRecordsInfo(DateTime dt, ref Int64[] iprp)
            {
                DateTime[] prp_ids;
                int cnt = GetFiberPrpCount(dt, out prp_ids);

                if (iprp != noneId)
                {
                    iprp = new Int64[cnt];
                }
                FiberRecord[] recs = new FiberRecord[cnt];
                for (int i = 0, j = 0; j < cnt; i++)
                {
                    if (prp_ids[i].Year <= 1900)
                        continue;

                    Int64 id = ((RecordInfo)_cachedIndividual[new FiberIndex(prp_ids[i], i)]).PrpId;
                    if (iprp != noneId)
                    {
                        iprp[j] = id;
                    }

                    recs[j++] = new FiberRecord(
                        GetFiberRecordInfo(new FiberIndex(prp_ids[i], i)));
                }

                return recs;
            }

            private RecordInfo GetFiberRecordInfo(FiberIndex idx)
            {
                RecordInfo ri = null;
#if !DOTNET_V11
                if (!_cachedIndividual.TryGetValue(idx, out ri))
                    return null;
#else
                ri = (RecordInfo)_cachedIndividual[idx];    
                if (ri == null)
                    return null;
#endif
                return ri;
            }


            public FiberRecord[] GetAllFiberRecordsInfo(DateTime dt)
            {
                Int64[] x = noneId;
                return GetAllFiberRecordsInfo(dt, ref x);
            }

            public int GetFiberPrpCount(DateTime dt)
            {
                DateTime[] prp_ids;
                return GetFiberPrpCount(dt, out prp_ids);
            }

            public FiberRecord[] GetAllFiberRecords(int window, DateTime dt)
            {
                Int64[] prp_ids = null;
                FiberRecord[] recs = GetAllFiberRecordsInfo(dt, ref prp_ids);

                for (int i = 0; i < prp_ids.Length; i++)
                {
                    recs[i] = _conn.GetRecordByPrpId(recs[i].recordInfo, window);
                    recs[i].pvkLoad = _conn.GetFiberChannelTypes(prp_ids[i]);
                }
                return recs;
            }

            public FiberRecord[] GetAllFiberAzotRecords(DateTime dt)
            {
                return GetAllFiberRecords(0, dt);
            }



            public FiberRecord[] GetAllFiberRecords(DateTime dt)
            {
                Int64[] prp_ids = null;
                FiberRecord[] recs = GetAllFiberRecordsInfo(dt, ref prp_ids);

                for (int i = 0; i < prp_ids.Length; i++)
                {
                    recs[i] = _conn.GetRecordByPrpId(recs[i].recordInfo);
                    recs[i].pvkLoad = _conn.GetFiberChannelTypes(prp_ids[i]);
                }
                return recs;
            }

            internal void Init()
            {
                using (IDbCommand cmd = _conn.CreateCommand())
                {
                    IDataParameter id = cmd.CreateParameter();
                    cmd.Parameters.Add(id);
                    IDataReader reader;

                    cmd.CommandText =
                       @"SELECT THREAD.system_id, THREAD.name, K.system_id, K.name, BS.system_id, BS.name FROM
                                system AS THREAD, system AS K, system AS BS WHERE
                                    BS.type = 'bs' AND K.type='cart' AND THREAD.type='thread' AND THREAD.owner_id=K.system_id AND K.owner_id=BS.system_id AND BS.owner_id = ?";

                    id.Value = _blockId;
                    cmd.Prepare();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int nit = (int)Convert.ToInt64(reader[1]) - 1;
                        if (nit >= 0 && nit < 16)
                        {
                            _threads[nit] = reader.GetInt64(0);
                            _karts[nit] = reader.GetInt64(2);
                            _bs[nit] = reader.GetInt64(4);
                            _kartsVals[nit] = Convert.ToInt64(reader[3]);
                            _bsNames[nit] = reader.GetString(5);
                        }
                        else
                        {
                            reader.Close();
                            throw new ArgumentException("Incorrect AKGO database!");
                        }
                    }
                    reader.Close();

                    Coords[,] coords = new Coords[16, 115];
                    for (int j = 0; j < 16; j++)
                    {
                        Int64 numThreadId = _threads[j];
                        cmd.CommandText = "SELECT n_pvk, x, y, kind FROM kart WHERE thread_id = ?";

                        id.Value = numThreadId;
                        cmd.Prepare();
                        reader = cmd.ExecuteReader();

                        //Processing cartogram
                        //int pvk = 0;
                        while (reader.Read())
                        {
                            Int32 npvk, scx, scy;
                            String kind;

                            npvk = reader.GetInt32(0);
                            scx = reader.GetInt32(1);
                            scy = reader.GetInt32(2);
                            kind = reader.GetString(3);

                            bool unrouted = (kind.CompareTo("pus") == 0);
                            Coords crd = Coords.FromHumane(scy, scx);

                            if ((crd.IsOk) && (!unrouted))
                            {
                                coords[j, npvk - 1] = crd;
                            }
                            else
                            {
                                coords[j, npvk - 1] = Coords.incorrect;
                            }
                        }
                        reader.Close();
                    }
                    _pvkConv = new CoordsConverter(
                        new TupleMetaData("pvk_scheme", "Разводка ПВК", new DateTime(), TupleMetaData.StreamConst),
                        CoordsConverter.SpecialFlag.PVK, coords);
                }
            }

            public CoordsConverter PvkScheme
            {
                get { return _pvkConv; }
            }

            public DateTime[] CachedDates
            {
                get { return _cachedSortedPrpAllThreads; }
            }


            private const int DateThresouldPeriod = 120;

            private String _nppName;
            private Int64 _blockNum;

            private Int64 _blockId;
            private Int64 _nppId;

            private Int64[] _bs = new Int64[16];
            private String[] _bsNames = new String[16];
            private Int64[] _karts = new Int64[16];
            private Int64[] _kartsVals = new Int64[16];
            private Int64[] _threads = new Int64[16];

            private CoordsConverter _pvkConv;

            private AkgoSqlDb _conn;

            private bool _showOnly16Fibers;
            // CACHES
#if !DOTNET_V11
            private Dictionary<DateTime, DateTime[]> _cachedDtPrpAllThread = new Dictionary<DateTime, DateTime[]>();
#else
            private Hashtable _cachedDtPrpAllThread = new Hashtable();
#endif
            private FiberIndex[] _cachedSortedPrp;
            private DateTime[] _cachedSortedPrpAllThreads;
            private DateTime[] _cachedSortedPcThread;
            
            // Modern caches
#if !DOTNET_V11
            private Dictionary<FiberIndex, RecordInfo> _cachedIndividual = new Dictionary<FiberIndex, RecordInfo>();
#else
            internal Hashtable _cachedIndividual = new Hashtable();
#endif

            internal AkgoSystemInfo(AkgoSqlDb conn, Int64 nppId, Int64 blockId, String nppName, Int64 blockNum, bool showOnly16Fibers)
            {
                _conn = conn;
                _nppId = nppId;
                _blockId = blockId;
                _nppName = nppName;
                _blockNum = blockNum;
                _showOnly16Fibers = showOnly16Fibers;
            }
        }


    }

    [AttributeDataComponent("Чтение прописок базы данных АКГО", MultiTuple = true, SourceNames = AkgoSqlProvider.sourceNames)]
    class AkgoSqlProvider : _MultiDataProviderNotifier, IMultiDataProvider, IDisposable
    {
        

        #region IMultiDataProvider Members

        /***********************************************************/
        /**************************Роман****************************/
        protected override IMultiDataTuple UncheckedGetData(string stream)
        {
            if (stream == "pvk_scheme")
            {
                return GetConstData("const");
            }
            else
            {
                throw new NotImplementedException();
            }
           
        }
        /***********************************************************/
        /***********************************************************/
        protected override IMultiDataTuple UncheckedGetData(DateTime date, string stream)
        {
            if (stream == "const")
            {
                return new DataTuple("const", new DateTime(), new ITupleItem[] { _active.PvkScheme });
            }
            else if (stream == "pc")
            {
                return GetPcData(date, pcStream);
            }
            else if (stream == "prp")
            {
                return GetPrpData(date, prpStream);
            }

            throw new ArgumentException("unknown single stream");
        }

        protected override IMultiDataTuple UncheckedGetData(DateTime date, string[] names)
        {
            if ((names.Length == 1) && (names[0] == "pvk_scheme"))
            {
                return GetData(date, "const");
            }
            else
            {
                if (!IsNameMulti(names[0]))
                    return GetPcData(date, names);
                else
                    return GetPrpData(date, names);

                //throw new NotImplementedException();
               // return GetPcData(date, names);
            }
        }


        public override DateTime[] GetDates()
        {
            return _active.GetPrpDates();
        }

        public override DateTime[] GetDates(string stream)
        {
            if (stream == "pc")
                return _active.GetPcDates();
            else if (stream == "prp")
            {
                return _active.GetPrpDates();
            }
            else if (stream == "const")
            {
                return new DateTime[] { DateTime.Now };
            }
            else
                return null;
        }

        public override string[] GetStreamNames()
        {
            return _streams;
        }

        protected override void UncheckedPushData(IMultiDataTuple data)
        {
            throw new NotSupportedException();
        }

        public bool IsStreamMulti(string stream)
        {
            if ((stream == "const") || (stream == "pc"))
                return false;
            else
                return true;
        }

        public bool IsNameMulti(string name)
        {
            switch (name)
            {
                case "kgoprp_info": return true;
                case "kgoprp_azot": return true;
                case "kgoprp_w1": return true;
                case "kgoprp_w2": return true;
                case "kgoprp_w3": return true;
                case "kgoprp_w4": return true;
                case "pvk_scheme": return false;
                case "kgopc_info": return false;
                case "kgopc_azot": return false;
                case "kgopc_w1": return false;
                case "kgopc_w2": return false;
                case "kgopc_w3": return false;
                case "kgopc_w4": return false;
            }

            throw new ArgumentException("Unknown name");
        }

        public override string[] GetDataNames(DateTime date, string stream)
        {
            if (stream == "pc")
                return pcStream;
            else if (stream == "prp")
                return prpStream;
            else if (stream == "const")
                return constStream;

            return null;
        }

        #endregion

        #region IDataProvider Members

        public override string[] GetExistNames()
        {
            return splitedSourceNames;
        }

        #endregion

        static string GetHelpFor(string name)
        {
            if (name == "kgoprp_azot")
                return "Азотное";
            else if (name == "kgoprp_w1")
                return "Окно 1";
            else if (name == "kgoprp_w2")
                return "Окно 2";
            else if (name == "kgoprp_w3")
                return "Окно 3";
            else if (name == "kgoprp_w4")
                return "Окно 4";
            else if (name == "kgoprp_info")
                return "Информация прописки АКГО";
            else if (name == "kgopc_azot")
                return "Азотное (ПК)";
            else if (name == "kgopc_w1")
                return "Окно 1 (ПК)";
            else if (name == "kgopc_w2")
                return "Окно 2 (ПК)";
            else if (name == "kgopc_w3")
                return "Окно 3 (ПК)";
            else if (name == "kgopc_w4")
                return "Окно 4 (ПК)";
            else if (name == "kgopc_info")
                return "Информация постоянного контроля АКГО";

            return name;
        }

        DataParamTable GetPcDtInfo(AkgoSqlDb.RecordInfo ri, string stream)
        {
            return new DataParamTable(
                new TupleMetaData("kgopc_info", GetHelpFor("kgopc_info"), ri.Dt, stream),
                new DataParamTableItem[] {
									  new DataParamTableItem("specClock", ri.SpecClock),
                                      new DataParamTableItem("fiberNum", ri.FibNum),
                                      new DataParamTableItem("prpDate", ri.Dt.ToOADate()),
                                      new DataParamTableItem("pcPvk", ri.Pvk)
                                                    }
                                      );
        }

        DataTuple GetPcData(DateTime dt, string[] names)
        {
            ITupleItem[] items = new ITupleItem[names.Length];
            AkgoSqlDb.FiberRecord fr = null;

            for (int i = 0; i< names.Length; i++)
            {
                if (names[i] == "kgopc_info")
                    items[i] = GetPcDtInfo(_active.GetSingleRecordInfo(dt), "pc");
                else
                {
                    TupleMetaData info = new TupleMetaData(names[i], GetHelpFor(names[i]), dt, "pc");

                    if (fr == null)
                        fr = _active.GetSingleRecord(dt, false);

                    if (names[i] == "kgopc_azot")
                        items[i] = new DataArrayInt(info, fr.all[0]);
                    else if (names[i] == "kgopc_w1")
                        items[i] = new DataArrayInt(info, fr.all[1]);
                    else if (names[i] == "kgopc_w2")
                        items[i] = new DataArrayInt(info, fr.all[2]);
                    else if (names[i] == "kgopc_w3")
                        items[i] = new DataArrayInt(info, fr.all[3]);
                    else if (names[i] == "kgopc_w4")
                        items[i] = new DataArrayInt(info, fr.all[4]);
                    else
                        throw new ArgumentException(String.Format("Unknown name: '{0}'", names[i]));
                }
            }

            return new DataTuple("pc", dt, items);
        }

        IMultiDataTuple GetPrpData(DateTime dt, string[] names)
        {
            AkgoSqlDb.FiberRecord[] frAzot = _active.GetAllFiberRecordsInfo(dt);           

            DataTuple[] tuples = new DataTuple[frAzot.Length];
            
            ITupleItem[][] tis = new ITupleItem[frAzot.Length][];
            for (int i = 0; i < frAzot.Length; i++)
                tis[i] = new ITupleItem[names.Length];

            AkgoSqlDb.FiberRecord[] fr = null;

            for (int n = 0; n < names.Length; n++)
            {
                if (names[n] == "kgoprp_info")
                {
                    for (int i = 0; i < frAzot.Length; i++)
                    {
                        AkgoSqlDb.RecordInfo ri = frAzot[i].recordInfo;
                        
                        tis[i][n] = new DataParamTable(
                            new TupleMetaData("kgoprp_info", String.Format("{0} Н.{1}", GetHelpFor("kgoprp_info"), ri.FibNum + 1), dt, "prp"),
                            new DataParamTableItem[] {
                                                    new DataParamTableItem("specClock", ri.SpecClock),
                                                    new DataParamTableItem("fiberNum", ri.FibNum),
                                                    new DataParamTableItem("prpDate", ri.Dt.ToOADate()) }
                                                    );
                    }
                }
                else
                {
                    if (fr == null)
                        fr = _active.GetAllFiberRecords(dt);

                    int idx = 0;
                    switch (names[n])
                    {
                        case "kgoprp_azot": idx = 0; break;
                        case "kgoprp_w1": idx = 1; break;
                        case "kgoprp_w2": idx = 2; break;
                        case "kgoprp_w3": idx = 3; break;
                        case "kgoprp_w4": idx = 4; break;
                        default: throw new ArgumentException(String.Format("Unknown name: '{0}'", names[n]));
                    }

                    for (int i = 0; i < frAzot.Length; i++)
                    {
                        tis[i][n] = new DataArrayInt(
                            new TupleMetaData(names[n], String.Format("{0} Н.{1}", GetHelpFor(names[n]), frAzot[i].recordInfo.FibNum + 1), dt, "prp"), fr[i].all[idx]);

                    }
                }
            }

            for (int i = 0; i < frAzot.Length; i++)
            {
                tuples[i] = new DataTuple("prp", dt, tis[i]);
            }
            return new MultiDataTuple(tuples);
        }

        public override TupleMetaData GetTupleItemInfo(DateTime date, string name)
        {
            string stream = name.StartsWith("kgoprp") ? "prp" :
                (name.StartsWith("kgopc") ? "pc" : TupleMetaData.StreamConst);
            return new TupleMetaData(name, GetHelpFor(name), date, stream);
        }

        public override int GetMultiTuplesCount(DateTime date, string name)
        {
            if (IsNameMulti(name))
                return _active.GetFiberPrpCount(date);

            return -1;
        }

        // "kgozagr"
        private static readonly string[] prpStream = { "kgoprp_info", "kgoprp_azot", "kgoprp_w1", "kgoprp_w2", "kgoprp_w3", "kgoprp_w4" };
        private static readonly string[] pcStream = { "kgopc_info", "kgopc_azot", "kgopc_w1", "kgopc_w2", "kgopc_w3", "kgopc_w4" };
        private static readonly string[] constStream = { "pvk_scheme" };

        public const string sourceNames = "kgoprp_info,kgoprp_azot,kgoprp_w1,kgoprp_w2,kgoprp_w3,kgoprp_w4,pvk_scheme,kgopc_info,kgopc_azot,kgopc_w1,kgopc_w2,kgopc_w3,kgopc_w4";
        public static readonly string[] splitedSourceNames = sourceNames.Split(',');

        private static readonly string[] _streams = { "pc", "prp", "const" };
        private AkgoSqlDb.AkgoSystemInfo _active;

        private AkgoSqlDb _kgo;

        public static implicit operator AkgoSqlDb (AkgoSqlProvider w)
        {
            return w._kgo;
        }

        public AkgoSqlProvider(string akgoConnectionString, string akgoDbSystem)
        {
            OdbcConnection conn = new OdbcConnection(akgoConnectionString);
            conn.Open();

            _kgo = new AkgoSqlDb(conn, !false);

            if ((_kgo.SystemCount == 1) && (akgoDbSystem.ToLower() == "default"))
            {
                _active = _kgo[0];
            }
            else
            {
                int i = Convert.ToInt32(akgoConnectionString);
                if ((i >= 0) && (i < _kgo.SystemCount))
                    _active = _kgo[i];
                else
                    throw new Exception("AKGO Bad configuration");
            }

            _active.UpdateCache();
        }

        #region IDisposable Members

        public override void Dispose()
        {
            _kgo.Dispose();
        }

        #endregion

        public override string[] GetAllDataNames(string stream)
        {
            switch (stream)
            {
                case "prp": return prpStream;
                case "pc": return pcStream;
                case "const": return constStream;
                default: return null;
            }
        }
    }
}
