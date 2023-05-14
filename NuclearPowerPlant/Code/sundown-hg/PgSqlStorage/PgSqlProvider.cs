using System;
using System.Collections;
#if !DOTNET_V11
using System.Collections.Generic;
#endif
using System.Text;
using System.Data;
using corelib;
using Npgsql;

namespace PgSqlStorage
{
    [AttributeDataComponent("Провайдер данных на основе хранилиша PostgreSQL",ParametrsInfoString =
        @"enviromentObject('', Enviroment),
        serverNameString('Адрес сервера (например, 127.0.0.1)', String),
        serverPort('Порт (например, 5342)', String),
        pgsqlUserId('Имя пользователя', String),
        pgsqlPassword('Пароль', Password),
        baseName('Имя базы данных', String)")]

    public class PgSqlProvider : DataSqlFormatQ
    {
        public PgSqlProvider(IEnviromentEx enviromentObject, string serverNameString, string serverPort, string pgsqlUserId, string pgsqlPassword, string baseName)
            : base(enviromentObject)
        {
            string connectionString = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", serverNameString, serverPort, pgsqlUserId, pgsqlPassword, baseName) ;
            _conn = new NpgsqlConnection(connectionString);
            _conn.Open();

            
            try
                {
                    using (IDbCommand cmd = _conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM hdata;";
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