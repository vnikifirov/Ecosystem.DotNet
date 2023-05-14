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
    public abstract class DataSqlFormatV : AbstractSQLMultiProvider
    {
        protected int _abiVersion;
        protected IDbConnection _conn;

        protected DataSqlFormatV(IEnviromentEx env)
            : base(env)
        {

        }

        protected void SetParameters(IDbConnection conn, int abi)
        {
            _conn = conn;
            _abiVersion = abi;
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
            IDbCommand cmd = _conn.CreateCommand();

            if (_abiVersion < 1)
            {
                cmd.CommandText =
                    " CREATE TABLE vdata(sname varchar(32) NOT NULL, sdate timestamp  NOT NULL, name varchar(32), ndate timestamp, sbin bytea, PRIMARY KEY(name, ndate)); " +
                    " CREATE TABLE cdata(name varchar(32), sbin bytea, PRIMARY KEY(name)); " +
                    " CREATE TABLE helpstr(name varchar(32), help varchar(200), PRIMARY KEY(name)); ";
            }
            else if (_abiVersion == 1)
            {
                cmd.CommandText =
                    " CREATE TABLE vdata(sname varchar(32) NOT NULL, sdate timestamp  NOT NULL, name varchar(32), ndate timestamp, idx integer DEFAULT(0), sbin bytea, PRIMARY KEY(name, ndate, idx)); " +
                    " CREATE TABLE cdata(name varchar(32), sbin bytea, PRIMARY KEY(name)); " +
                    " CREATE TABLE helpstr(name varchar(32), help varchar(200), PRIMARY KEY(name)); ";
            }

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public override void ClearAllData()
        {
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText =
                " DELETE FROM vdata;  DELETE FROM cdata; DELETE FROM helpstr; ";

            cmd.ExecuteNonQuery();
            cmd.Dispose();
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
            try
            {
                IDataReader dr = command.ExecuteReader();
                try
                {
                    while (dr.Read())
                    {
                        d.Add(Convert.ToDateTime(dr[0]));
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
            return d.ToArray();
        }

        private string[] ReadArrayOfString(IDbCommand command)
        {
            List<string> d = new List<string>();
            try
            {
                IDataReader dr = command.ExecuteReader();
                try
                {
                    while (dr.Read())
                    {
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
            return d.ToArray();
        }

#endif
        public override string[] GetStreamNames()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT sname from vdata;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        private IDbDataParameter CreateParam(IDbCommand cmd, string id, DbType type)
        {
            IDbDataParameter par = cmd.CreateParameter();
            par.ParameterName = id;
            par.DbType = type;

            cmd.Parameters.Add(par);
            return par;
        }

        public override DateTime[] GetDates(string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT sdate from vdata where sname = :sn;";           
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
            command.CommandText = "SELECT DISTINCT sdate from vdata;";

#if DOTNET_V11
            return (DateTime[])ReadArrayOf(command, typeof(DateTime));
#else
            return ReadArrayOfDateTime(command);
#endif
        }

        protected override string[] GetNamesInAll()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vdata;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string[] GetNames(string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vdata where sname = :sv;";
            CreateParam(command, ":sv", DbType.String).Value = stream;

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        public override string[] GetDataNames(DateTime date, string stream)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from vdata where sname = :sv AND sdate = :nd;";
            CreateParam(command, ":sv", DbType.String).Value = stream;
            CreateParam(command, ":nd", DbType.DateTime).Value = date;
#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string[] GetConstNames()
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT DISTINCT name from cdata;";

#if DOTNET_V11
            return (string[])ReadArrayOf(command, typeof(string));
#else
            return ReadArrayOfString(command);
#endif
        }

        protected override string GetHelpString(string param)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT help from helpstr where name = :sv;";
            CreateParam(command, ":sv", DbType.String).Value = param;
            object obj = null;
            try
            {
                obj = command.ExecuteScalar();
            }
            catch
            {
            }
            finally
            {
                command.Dispose();
            }
            return (string)obj;                
        }

        protected override void SetHelpString(string param, string value)
        {
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM helpstr WHERE name = :hn ;";
            CreateParam(cmd, ":hn", DbType.String).Value = param;
            long cnt = 0;
            try
            {
                cnt = (long)cmd.ExecuteScalar();
            }
            catch
            {
            	
            }

            cmd.Dispose();

            if (cnt == 1)
                return;

            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "INSERT INTO helpstr(help, name)  VALUES ( :hs, :hn );";
            CreateParam(command, ":hs", DbType.String).Value = value;
            CreateParam(command, ":hn", DbType.String).Value = param;
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                //TODO FIXME !!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            finally
            {
                command.Dispose();
            }
        }

        void checkAbiZero(int idx)
        {
            if (idx != 0)
                throw new ArgumentException("В формате данных 0 нет поддержки грепповых данных");
        }

        protected override DataItemInfo[,] GetDataItem(DateTime date, string[] names, int maxIdx)
        {
            if (_abiVersion == 0)
            {
                checkAbiZero(maxIdx);
            }

            DataItemInfo[,] ret = new DataItemInfo[names.Length, maxIdx + 1];

            IDbCommand command = _conn.CreateCommand();
            if (_abiVersion == 0)
                command.CommandText = "SELECT sname, sdate, sbin from vdata where (name = :nm AND sdate = :nd );";
            else
                command.CommandText = "SELECT sname, sdate, sbin from vdata where (name = :nm AND sdate = :nd AND idx = :dx );";

            IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
            CreateParam(command, ":nd", DbType.DateTime).Value = date;
            IDbDataParameter idx = null;
            if (_abiVersion != 0)
                idx = CreateParam(command, ":dx", DbType.Int32);

            try
            {
                for (int i = 0; i < names.Length; i++)
                {
                    name.Value = names[i];

                    for (int j = 0; j < maxIdx + 1; j++)
                    {
                        if (_abiVersion != 0)
                            idx.Value = j;

                        IDataReader dr = command.ExecuteReader();
                        try
                        {
                            if (dr.Read())
                            {
                                ret[i, j].Stream = (string)dr[0];
                                ret[i, j].StreamDate = (DateTime)dr[1];
                                ret[i, j].Data = (byte[])dr[2];
                            }
                        }
                        finally
                        {
                            dr.Close();
                        }
                    }
                }
            }
            finally
            {
                command.Dispose();
            }
            return ret;

        }

        protected override bool GetInfoItem(DateTime[] dates, string name, int idx, out string stream, out DateTime[] sdate)
        {
            bool ret = false;
            stream = null;
            sdate = null;


            IDbCommand command = _conn.CreateCommand();
            if (_abiVersion == 0)
                command.CommandText = "SELECT sname, sdate from vdata where (name = :nm AND sdate = :nd);";
            else
                command.CommandText = "SELECT sname, sdate from vdata where (name = :nm AND sdate = :nd AND idx = :dx);";

            CreateParam(command, ":nm", DbType.String).Value = name;
            IDbDataParameter date = CreateParam(command, ":nd", DbType.DateTime);
            if (_abiVersion != 0)
                CreateParam(command, ":dx", DbType.Int32).Value = idx;


            sdate = new DateTime[dates.Length];

            try
            {
                for (int i = 0; i < dates.Length; i++)
                {
                    date.Value = dates[i];

                    IDataReader dr = command.ExecuteReader();
                    try
                    {
                        if (dr.Read())
                        {
                            stream = (string)dr[0];
                            sdate[i] = (DateTime)dr[1];

                            ret = true;
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
            }
            finally
            {
                command.Dispose();
            }
            return ret;
        }
        


        protected override void StoreData(DataItemInfo[,] data)
        {
            int idx = data.GetLength(1);
            int items = data.GetLength(0);

            if (_abiVersion == 0)
            {
                checkAbiZero(idx - 1);
            }
            
            IDbTransaction tr = _conn.BeginTransaction();
            IDbCommand command = _conn.CreateCommand();
            
            if (_abiVersion != 0)
                command.CommandText = "INSERT INTO vdata (sname, sdate, name, ndate, sbin, idx) VALUES  (:sn, :sd, :nm, :nd, :sb, :dx);";
            else
                command.CommandText = "INSERT INTO vdata (sname, sdate, name, ndate, sbin) VALUES  (:sn, :sd, :nm, :nd, :sb);";

            IDbDataParameter sname = CreateParam(command, ":sn", DbType.String);
            IDbDataParameter sdate = CreateParam(command, ":sd", DbType.DateTime);
            IDbDataParameter name = CreateParam(command, ":nm", DbType.String);
            IDbDataParameter date= CreateParam(command, ":nd", DbType.DateTime);
            IDbDataParameter bin = CreateParam(command, ":sb", DbType.Binary);
            IDbDataParameter db_idx = null;
            if (_abiVersion != 0)
                db_idx = CreateParam(command, ":dx", DbType.Int32);

            bool success = false;
            try
            {
                for (int n = 0; n < items; n++)
                {
                    for (int i = 0; i < idx; i++)
                    {
                        sname.Value = data[n, i].Stream;
                        sdate.Value = data[n, i].StreamDate;
                        name.Value = data[n, i].ItemName;
                        date.Value = data[n, i].ItemDate;
                        bin.Value = data[n, i].Data;

                        if (_abiVersion != 0)
                            db_idx.Value = i;


                        command.ExecuteNonQuery();
                    }
                }

                success = true;
            }
            finally
            {
                command.Dispose();
                if (success)
                    tr.Commit();
                else
                    tr.Rollback();
            }
        }

        protected override int MaxIdx(DateTime date, string name)
        {
            if (_abiVersion == 0)
                return 0;

            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT MAX(idx) from vdata where (name = :nm AND sdate = :nd);";

            CreateParam(command, ":nm", DbType.String).Value = name;
            CreateParam(command, ":nd", DbType.DateTime).Value = date;

            int res = -1;
            try
            {
                IDataReader dr = command.ExecuteReader();
                try
                {
                    if (dr.Read())
                    {
                        res = Convert.ToInt32(dr[0]);
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
            return res;
        }

        protected override void MaxIdx(DateTime[] dates, string name, out int[] maxs)
        {
            maxs = new int[dates.Length];
            if (_abiVersion == 0)
                return;

            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT MAX(idx) from vdata where (name = :nm AND sdate = :nd);";

            CreateParam(command, ":nm", DbType.String).Value = name;
            IDbDataParameter date = CreateParam(command, ":nd", DbType.DateTime);

            try
            {
                for (int i = 0; i < dates.Length; i++)
                {
                    date.Value = dates[i];

                    IDataReader dr = command.ExecuteReader();
                    try
                    {
                        if (dr.Read())
                        {
                            maxs[i] = Convert.ToInt32(dr[0]);
                        }
                        else
                        {
                            maxs[i] = -1;
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        protected override byte[] GetConstData(string name)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "SELECT sbin from cdata where (name = :nm );";
            CreateParam(command, ":nm", DbType.String).Value = name;

            byte[] tup = null;
            try
            {
                IDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    tup = (byte[])dr[0];
                }
            }
            catch
            {
            }
            finally
            {
                command.Dispose();
            }
            return tup;
        }

        protected override void StoreConstData(string name, byte[] item)
        {
            IDbCommand command = _conn.CreateCommand();
            command.CommandText = "INSERT INTO cdata (name, sbin) VALUES  (:sn, :sb);";

            CreateParam(command, ":sn", DbType.String).Value = name;
            CreateParam(command, ":sb", DbType.Binary).Value = item;
            try
            {
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Dispose();
            }
        }


    }

    
}
