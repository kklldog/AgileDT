﻿using AgileDT.Data.Entites;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Data
{
    public static class FreeSQL
    {
        private static IFreeSql _freesql;
        private static object _lockobj = new object();

        static FreeSQL()
        {
            _freesql = new FreeSql.FreeSqlBuilder()
                 .UseConnectionString(ProviderToFreesqlDbType(DbProvider), DbConnection)
                 .Build();
            _freesql.CodeFirst.SyncStructure<EventMessage>();
        }

        public static IFreeSql Instance
        {
            get
            {
                return _freesql;
            }
        }

        private static string DbProvider => Config.Instance["db:provider"];
        private static string DbConnection => Config.Instance["db:conn"];

        private static FreeSql.DataType ProviderToFreesqlDbType(string provider)
        {
            switch (provider)
            {
                case "sqlite":
                    return FreeSql.DataType.Sqlite;
                case "mysql":
                    return FreeSql.DataType.MySql;
                case "sqlserver":
                    return FreeSql.DataType.SqlServer;
                case "npgsql":
                    return FreeSql.DataType.PostgreSQL;
                case "oracle":
                    return FreeSql.DataType.Oracle;
                default:
                    break;
            }

            return FreeSql.DataType.Sqlite;
        }
    }
}
