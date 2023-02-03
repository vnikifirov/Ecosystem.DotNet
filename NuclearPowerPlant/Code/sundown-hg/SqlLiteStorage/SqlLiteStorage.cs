using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Data;
using corelib;

#if MONO
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif

namespace SqliteStorage
{
    [AttributeDataComponent("Провайдер данных на основе хранилиша SQLite", ParametrsInfoString=
        @"enviromentObject('', Enviroment),
          databaseFileString('Путь к файлу базы SQLite *.db3|*.db3', FilePath),
          readOnlyAccess('Только для чтения', Bool, 'True', True),
          autoInit('Автоматическая инициализация', Bool, 'True', True),
          abiVer('Формат хранения данных', List, 'v1', contains ('v1', 'v0') )",
        ComponentFileNameArgument = "databaseFileString", ComponentFileFilter = "Путь к файлу базы SQLite *.db3|*.db3"
    )]
    public class SqliteProvider : DataSqlFormatV
    {
        public SqliteProvider(IEnviromentEx enviromentObject, string databaseFileString, bool readOnlyAccess, bool autoInit, string abiVer) :
            base(enviromentObject)
        {
            string connectionString = String.Format("Data Source={0};Version=3;Read Only={1};", databaseFileString,
                readOnlyAccess ? "True" : "False");

            if (abiVer == "v0")
                _abiVersion = 0;
            else if (abiVer == "v1")
                _abiVersion = 1;
            else
                throw new ArgumentException("Неизвестный тип ABI");


            _conn = new SQLiteConnection(connectionString);
            _conn.Open();

            if (autoInit)
            {
                try
                {
                    using (IDbCommand cmd = _conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM vdata;";
                        cmd.ExecuteScalar();
                    }
                }
                catch
                {
                    CreateStructure();
                }
            }
        }
        
    }












    [AttributeDataComponent("Провайдер данных на основе хранилиша SQLite (2)", ParametrsInfoString =
            @"enviromentObject('', Enviroment),
          databaseFileString('Путь к файлу базы SQLite *.dq3|*.dq3', FilePath),
          readOnlyAccess('Только для чтения', Bool, 'True', True),
          compression('Сжатие: 0-нет,1-Deflate,2-LZMA,3-LZMA(max)', List, '3', contains ('0', '1', '2', '3') )",
            ComponentFileNameArgument = "databaseFileString", ComponentFileFilter = "Путь к файлу базы SQLite *.dq3|*.dq3"
        )]
    public class SqliteProvider2 : DataSqlFormatQ
    {
        public SqliteProvider2(IEnviromentEx enviromentObject, string databaseFileString, bool readOnlyAccess, int compression) :
            base(enviromentObject)
        {
            string connectionString = String.Format("Data Source={0};Version=3;Read Only={1};", databaseFileString,
                readOnlyAccess ? "True" : "False");

            _conn = new SQLiteConnection(connectionString);
            _conn.Open();

            if ((compression >= 0) && (compression <= 3))
                _compression = compression;

            try
            {
                using (IDbCommand cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM vidx;";
                    cmd.ExecuteScalar();
                }
            }
            catch
            {
                CreateStructure();
            }
        }

    }
}
