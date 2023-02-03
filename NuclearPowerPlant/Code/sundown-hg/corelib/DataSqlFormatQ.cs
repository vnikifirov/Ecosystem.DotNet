using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Data;
using corelib;

namespace corelib
{
    public class DataSqlFormatQ : AbstractSQLMultiProvider2
    {
        protected IDbConnection _conn;

        protected DataSqlFormatQ(IEnviromentEx env)
            : base(env)
        {

        }

        public override void Dispose()
        {
            _conn.Dispose();
        }

        public override void CreateDatabase(string databaseName)
        {

        }

        public override void CreateStructure()
        {
            using (IDbCommand cmd = _conn.CreateCommand())
            {
               cmd.CommandText =
                /*   
                "DROP TABLE vidx; " +
                "DROP TABLE cidx; " +
                "DROP TABLE data; " +
                "DROP TABLE hdata; " +*/
"    CREATE TABLE hdata(id bigint, ndx integer, help varchar(240), PRIMARY KEY(id,ndx)); " +
"    CREATE TABLE data(id bigint, ndx integer, algo integer DEFAULT(0), refid bigint, hash bigint, data bytea, PRIMARY KEY(id,ndx)); " +
//"    CREATE TABLE vidx(sname varchar(32) NOT NULL, sdate timestamp NOT NULL, name varchar(32), ndate timestamp, dataid bigint, hid integer, hndx integer, datandx bigint, FOREIGN KEY(hid,hndx) REFERENCES hdata(id,ndx), FOREIGN KEY(dataid,datandx) REFERENCES data(id,ndx),  PRIMARY KEY(name, ndate)); " +
"    CREATE TABLE vidx(sname varchar(32) NOT NULL, sdate timestamp NOT NULL, name varchar(32), ndate timestamp, dataid bigint, hid integer, PRIMARY KEY(name, ndate)); " +
//"    CREATE TABLE cidx(sname varchar(32) DEFAULT('const'), name varchar(32), dataid bigint, hid integer, hndx integer, datandx bigint, FOREIGN KEY(hid,hndx) REFERENCES hdata(id,ndx), FOREIGN KEY(dataid,datandx) REFERENCES data(id,ndx), PRIMARY KEY(name)); " +
"    CREATE TABLE cidx(sname varchar(32) DEFAULT('const'), name varchar(32), dataid bigint, hid integer, PRIMARY KEY(name)); " +
"    CREATE INDEX dindex ON data (id); " +
"    CREATE INDEX dindex2 ON data (ndx,algo,refid,hash); " +
"    CREATE INDEX vindex ON vidx (name,ndate); " +
"    CREATE INDEX vindex2 ON vidx (name, sdate, dataid); ";


               cmd.ExecuteNonQuery(); 
            }
        }

        public override void ClearAllData()
        {
            
            /*
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText =
                " DELETE FROM vdata;  DELETE FROM cdata; DELETE FROM helpstr; ";

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            */
        }
      


        private IDbDataParameter CreateParam(IDbCommand cmd, string id, DbType type)
        {
            IDbDataParameter par = cmd.CreateParameter();
            par.ParameterName = id;
            par.DbType = type;

            cmd.Parameters.Add(par);
            return par;
        }


#if DOTNET_V11
        private Array ReadArrayOf(SQLiteCommand command, Type t)
        {
            ArrayList d = new ArrayList();
            try
            {
                SQLiteDataReader dr = command.ExecuteReader();
                try
                {
                    while (dr.Read())
                    {
                        if (t == typeof(DateTime))
                            d.Add(Convert.ToDateTime(dr[0]));
                        else
                            d.Add(dr[0].ToString());
                    }
                }
                finally
                {
                    dr.Close();
                }
            }
            finally
            {
                command.Dispose();
            }
            return d.ToArray(t);
        }
#else

        private DateTime[] ReadArrayOfDateTime(IDbCommand command)
        {
            List<DateTime> d = new List<DateTime>();
            using (command)
            {
                using (IDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        d.Add(Convert.ToDateTime(dr[0]));
                    }
                }
            }
            return d.ToArray();
        }

        private string[] ReadArrayOfString(IDbCommand command)
        {
            List<string> d = new List<string>();
            using (command)
            {
                using (IDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        d.Add(dr[0].ToString());
                    }
                }
            }
            return d.ToArray();
        }

#endif
        public override string[] GetStreamNames()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT sname from vidx;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string[] GetNamesInAll()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vidx;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string[] GetNames(string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vidx where sname = :sv;";
            CreateParam(command, ":sv", DbType.String).Value = stream;

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string[] GetConstNames()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from cidx;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        public override string[] GetDataNames(DateTime date, string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vidx where sname = :sv AND sdate = :nd;";
            CreateParam(command, ":sv", DbType.String).Value = stream;
            CreateParam(command, ":nd", DbType.DateTime).Value = date;
#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        public override DateTime[] GetDates(string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            //command.CommandText = "SELECT DISTINCT sdate from vidx where sname = :sn;";
            command.CommandText = "SELECT DISTINCT sdate from vidx where sname = :sn ORDER BY sdate;";
            CreateParam(command, ":sn", DbType.String).Value = stream;

#if DOTNET_V11
            return (DateTime[])ReadArrayOf(command, typeof(DateTime));
#else
            return ReadArrayOfDateTime(command);
#endif
        }

        public override DateTime[] GetDates()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT sdate from vidx;";

#if DOTNET_V11
            return (DateTime[])ReadArrayOf(command, typeof(DateTime));
#else
            return ReadArrayOfDateTime(command);
#endif
        }




        protected override DataItemInfo[,] GetDataItemInfo(DateTime[] dates, string[] names)
        {
            DataItemInfo[,] ret = new DataItemInfo[dates.Length, names.Length];

            using (IDbCommand command = _conn.CreateCommand())
            using (IDbCommand command2 = _conn.CreateCommand())
            {/*
                //было
                //command.CommandText = "SELECT sname, ndate, dataid, hid, COUNT(ndx) from vidx, data where (name = :nm AND sdate = :nd AND dataid = id ) GROUP BY id;";

                //command.CommandText = "SELECT sname, ndate, dataid, hid, COUNT(datandx) from vidx where (name = :nm AND sdate = :nd AND dataid = id AND datandx = ndx) GROUP BY dataid;";
                command.CommandText = "SELECT COUNT(data.ndx), dataid from vidx, data where (name = :nm AND sdate = :nd AND dataid = id ) GROUP BY dataid  ORDER BY dataid;";
                

                IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
                IDbDataParameter date = CreateParam(command, ":nd", DbType.DateTime);

                for (int i = 0; i < dates.Length; i++)
                {
                    date.Value = dates[i];

                    for (int j = 0; j < names.Length; j++)
                    {
                        name.Value = names[j];

                        using (IDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                ret[i, j].ItemDate = (DateTime)dr[1]; //dates[i];
                                ret[i, j].ItemName = names[j];
                                ret[i, j].Stream = (string)dr[0];
                                ret[i, j].StreamDate = dates[i]; //(DateTime)dr[1];
                                ret[i, j].DataId = (Int64)dr[2];
                                ret[i, j].HelpId = (Int64)dr[3];
                                ret[i, j].Count = Convert.ToInt32(dr[4]);
                            }
                        }
                    }
                }//*/
                
                command.CommandText = "SELECT COUNT(data.ndx), dataid from vidx, data where (name = :nm AND sdate = :nd AND dataid = id ) GROUP BY dataid  ORDER BY dataid;";
                command2.CommandText = "select sname, ndate, dataid, hid from vidx where (name = :nm AND sdate = :nd ) ORDER BY dataid;";
                IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
                IDbDataParameter date = CreateParam(command, ":nd", DbType.DateTime);
                IDbDataParameter name2 = CreateParam(command2, ":nm", DbType.String);
                IDbDataParameter date2 = CreateParam(command2, ":nd", DbType.DateTime);

                for (int i = 0; i < dates.Length; i++)
                {
                    date.Value = dates[i];
                    date2.Value = dates[i];

                    for (int j = 0; j < names.Length; j++)
                    {
                        name.Value = names[j];
                        name2.Value = names[j];

                        using (IDataReader dr = command.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                              /*  ret[i, j].ItemDate = (DateTime)dr[1]; //dates[i];
                                ret[i, j].ItemName = names[j];
                                ret[i, j].Stream = (string)dr[0];
                                ret[i, j].StreamDate = dates[i]; //(DateTime)dr[1];
                                ret[i, j].DataId = (Int64)dr[2];
                                ret[i, j].HelpId = (Int64)dr[3];*/
                                ret[i, j].Count = Convert.ToInt32(dr[0]);
                            }
                        }

                        using (IDataReader dr = command2.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                ret[i, j].ItemDate = (DateTime)dr[1]; //dates[i];
                                ret[i, j].ItemName = names[j];
                                ret[i, j].Stream = (string)dr[0];
                                ret[i, j].StreamDate = dates[i]; //(DateTime)dr[1];
                                ret[i, j].DataId = (Int64)dr[2];
                                ret[i, j].HelpId = (Int32)dr[3];
                               // ret[i, j].Count = Convert.ToInt32(dr[4]);
                            }
                        }

                    }
                }


                //////

            }
            return ret;
        }

        protected override DataItemInfo[,] GetConstDataItemInfo(string[] names)
        {
            DataItemInfo[,] ret = new DataItemInfo[1, names.Length];

            using (IDbCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT sname, dataid, hid from cidx where (name = :nm );";

                IDbDataParameter name = CreateParam(command, ":nm", DbType.String);

                for (int j = 0; j < names.Length; j++)
                {
                    name.Value = names[j];

                    using (IDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ret[0, j].ItemDate = new DateTime();
                            ret[0, j].ItemName = names[j];
                            ret[0, j].Stream = (string)dr[0];
                            ret[0, j].StreamDate = new DateTime();
                            ret[0, j].DataId = (Int64)dr[1];
                            ret[0, j].HelpId = (Int32)dr[2];
                            ret[0, j].Count = 1;
                        }
                    }
                }
            }
            return ret;
        }

        protected override void FillHelpInfo(DataItemInfo[,] data)
        {
            using (IDbCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT ndx, help from hdata where (id = :id);";

                IDbDataParameter id = CreateParam(command, ":id", DbType.Int64);

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].help != null)
                            continue;

                        data[i, j].help = new HelpItem[data[i,j].Count];
                        id.Value = data[i, j].HelpId;

                        using (IDataReader dr = command.ExecuteReader())
                        {                            
                            for (int k = 0; k < data[i,j].Count; k++)
                            {
                                bool b = dr.Read();
                                if (!b)
                                    throw new Exception("Нарушение целостности данных");

                                int m = Convert.ToInt32(dr[0]);

                                data[i, j].help[m].HelpId = data[i, j].HelpId;
                                data[i, j].help[m].idx = m;
                                data[i, j].help[m].Info = (string)(dr[1]);
                            }
                        }
                    }
                }
            }
        }

        protected override void FillDataInfo(DataItemInfo[,] data)
        {
            using (IDbCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT ndx, refid, algo, data, hash from data where (id = :id );";

                IDbDataParameter id = CreateParam(command, ":id", DbType.Int64);

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].data != null)
                            continue;

                        data[i, j].data = new DataItem[data[i, j].Count];
                        id.Value = data[i, j].DataId;

                        using (IDataReader dr = command.ExecuteReader())
                        {
                            for (int k = 0; k < data[i, j].Count; k++)
                            {
                                bool b = dr.Read();
                                if (!b)
                                    throw new Exception("Нарушение целостности данных");


                                int m = Convert.ToInt32(dr[0]);

                                data[i, j].data[m].DataId = data[i, j].DataId;
                                data[i, j].data[m].idx = m;
                                data[i, j].data[m].RefDataId = (Int64)dr[1];
                                data[i, j].data[m].algo = Convert.ToInt32(dr[2]);
                                data[i, j].data[m].data = (byte[])(dr[3]);
                                data[i, j].data[m].hash = Convert.ToUInt32(dr[4]);
                            }
                        }
                    }
                }
            }
        }


        protected override void FindHelp(DataItemInfo[,] data)
        {
            using (IDbCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT id FROM hdata where (ndx = :dx AND help = :hp );";
                IDbDataParameter ndx = CreateParam(command, ":dx", DbType.Int32);
                IDbDataParameter help = CreateParam(command, ":hp", DbType.String);

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].HelpId == 0)
                        {
                            Int64 val = 0;
                            for (int k = 0; k < data[i, j].Count; k++)
                            {
                                ndx.Value = data[i, j].help[k].idx;
                                help.Value = data[i, j].help[k].Info;

                                object o = command.ExecuteScalar();
                                if ((o != null) && (!(o is DBNull)))
                                {
                                    if (val == 0)
                                    {
                                        val = Convert.ToInt64(o);
                                    }
                                    else if (val != (Int64)o)
                                    {
                                        val = 0;
                                        break;
                                    }
                                }
                                else
                                {
                                    val = 0;
                                    break;
                                }
                            }

                            data[i, j].HelpId = val;
                        }
                    }
                }
            }
        }

        private static bool IsEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            
            for (int i = 0; i<a.Length;i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        protected override void FindData(DataItemInfo[,] data)
        {
            using (IDbCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT id, data FROM data where (ndx = :dx AND algo = :ag AND refid = :ri AND hash = :hh );";
                IDbDataParameter ndx = CreateParam(command, ":dx", DbType.Int32);
                IDbDataParameter algo = CreateParam(command, ":ag", DbType.Int32);
                IDbDataParameter refid = CreateParam(command, ":ri", DbType.Int64);
                IDbDataParameter hash = CreateParam(command, ":hh", DbType.Int64);
                

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].DataId == 0)
                        {
                            Int64 val = 0;
                            for (int k = 0; k < data[i, j].Count; k++)
                            {
                                ndx.Value = (object)data[i, j].data[k].idx;
                                algo.Value = (object)data[i, j].data[k].algo;
                                refid.Value = (object)data[i, j].data[k].RefDataId;
                                hash.Value = (object)data[i, j].data[k].hash;

                                Int64 cid = 0;

                                using (IDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        Int64 rid = (Int64)reader[0];//Convert.ToInt64(reader[0]);
                                        byte[] rdata = (byte[])reader[1];

                                        if (IsEqual(rdata, data[i, j].data[k].data))
                                        {
                                            cid = rid;
                                            break;
                                        }
                                    }
                                }
                                
                                if ((cid != 0) && (val == 0))
                                {
                                    val = cid;
                                }
                                else if ((cid == 0) && (val == 0))
                                {
                                    break;
                                }
                                else if (cid != val)
                                {
                                    break;
                                }
                            }

                            data[i, j].DataId = val;
                        }
                    }
                }
            }
        }


        private void StoreHelp(DataItemInfo[,] data)
        {
            Int64 idx = 0;
            IDbCommand command = null;
            IDbDataParameter id = null;
            IDbDataParameter ndx = null;
            IDbDataParameter help = null;

            try
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].HelpId == 0)
                        {
                            if (idx == 0)
                            {
                                using (IDbCommand cmd = _conn.CreateCommand())
                                {
                                    cmd.CommandText = "SELECT MAX(id) FROM hdata";
                                    object d = cmd.ExecuteScalar();
                                    idx = (d is DBNull) ? 1 : Convert.ToInt32(d) + 1;
                                }
                            }

                            if (command == null)
                            {
                                command = _conn.CreateCommand();
                                command.CommandText = "INSERT INTO hdata(id, ndx, help) VALUES (:id, :nd, :hp);";
                                id = CreateParam(command, ":id", DbType.Int64);
                                ndx = CreateParam(command, ":nd", DbType.Int32);
                                help = CreateParam(command, ":hp", DbType.String);
                            }

                            data[i, j].HelpId = idx;
                            id.Value = idx++;

                            for (int k = 0; k < data[i, j].Count; k++)
                            {
                                ndx.Value = data[i, j].help[k].idx;
                                help.Value = data[i, j].help[k].Info;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }



        private void StoreData(DataItemInfo[,] data)
        {
            Int64 idx = 0;
            IDbCommand command = null;
            IDbDataParameter id = null;
            IDbDataParameter ndx = null;
            IDbDataParameter algo = null;
            IDbDataParameter refid = null;
            IDbDataParameter da = null;
            IDbDataParameter hash = null;
            try
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j].DataId == 0)
                        {
                            if (idx == 0)
                            {
                                using (IDbCommand cmd = _conn.CreateCommand())
                                {
                                    cmd.CommandText = "SELECT MAX(id) FROM data";
                                    object d = cmd.ExecuteScalar();
                                    idx = (d is DBNull) ? 1 : Convert.ToInt32(d) + 1;
                                }
                            }

                            if (command == null)
                            {
                                command = _conn.CreateCommand();
                                command.CommandText = "INSERT INTO data(id, ndx, algo, refid, data, hash) VALUES (:id, :nd, :ag, :rf, :da, :hh);";
                                id = CreateParam(command, ":id", DbType.Int64);
                                ndx = CreateParam(command, ":nd", DbType.Int32);
                                algo = CreateParam(command, ":ag", DbType.Int32);
                                refid = CreateParam(command, ":rf", DbType.Int64);
                                da = CreateParam(command, ":da", DbType.Binary);
                                hash = CreateParam(command, ":hh", DbType.Int64);
                            }

                            data[i, j].DataId = idx;
                            id.Value = idx++;

                            for (int k = 0; k < data[i, j].Count; k++)
                            {
                                ndx.Value = data[i, j].data[k].idx;
                                algo.Value = data[i, j].data[k].algo;
                                refid.Value = data[i, j].data[k].RefDataId;
                                da.Value = data[i, j].data[k].data;
                                hash.Value = data[i, j].data[k].hash;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (command != null)
                    command.Dispose();
            }
        }


        protected override void Store(DataItemInfo[,] data)
        {
            using (IDbTransaction tr = _conn.BeginTransaction())
            {
                bool success = false;
                try
                {
                    StoreHelp(data);
                    StoreData(data);

                    using (IDbCommand command = _conn.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO vidx(sname, sdate, name, ndate, dataid, hid) VALUES (:sn, :sd, :nm, :nd, :id, :hd);";
                        IDbDataParameter sname = CreateParam(command, ":sn", DbType.String);
                        IDbDataParameter sdate = CreateParam(command, ":sd", DbType.DateTime);
                        IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
                        IDbDataParameter date = CreateParam(command, ":nd", DbType.DateTime);
                        IDbDataParameter dataid = CreateParam(command, ":id", DbType.Int64);
                        IDbDataParameter hid = CreateParam(command, ":hd", DbType.Int64);

                        for (int i = 0; i < data.GetLength(0); i++)
                        {
                            for (int j = 0; j < data.GetLength(1); j++)
                            {
                                sname.Value = data[i, j].Stream;
                                sdate.Value = data[i, j].StreamDate;
                                name.Value = data[i, j].ItemName;
                                date.Value = data[i, j].ItemDate;
                                dataid.Value = data[i, j].DataId;
                                hid.Value = data[i, j].HelpId;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    success = true;
                }
                finally
                {
                    if (success)
                        tr.Commit();
                    else
                        tr.Rollback();
                }
            }
        }



        protected override void StoreConst(DataItemInfo[,] data)
        {
            using (IDbTransaction tr = _conn.BeginTransaction())
            {
                bool success = false;
                try
                {
                    StoreHelp(data);
                    StoreData(data);

                    using (IDbCommand command = _conn.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO cidx(sname, name, dataid, hid) VALUES (:sn, :nm, :id, :hd);";
                        IDbDataParameter sname = CreateParam(command, ":sn", DbType.String);
                        IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
                        IDbDataParameter dataid = CreateParam(command, ":id", DbType.Int64);
                        IDbDataParameter hid = CreateParam(command, ":hd", DbType.Int64);

                        for (int i = 0; i < data.GetLength(0); i++)
                        {
                            for (int j = 0; j < data.GetLength(1); j++)
                            {
                                sname.Value = data[i, j].Stream;
                                name.Value = data[i, j].ItemName;
                                dataid.Value = data[i, j].DataId;
                                hid.Value = data[i, j].HelpId;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    success = true;
                }
                finally
                {
                    if (success)
                        tr.Commit();
                    else
                        tr.Rollback();
                }
            }
        }
    }
}
