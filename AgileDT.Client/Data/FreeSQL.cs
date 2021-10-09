using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Client.Data
{
    internal static class FREESQL
    {
        private static IFreeSql _freesql;

        static FREESQL()
        {
            _freesql = new FreeSql.FreeSqlBuilder()
                 .UseConnectionString(ProviderToFreesqlDbType(DbProvider), DbConnection)
                 .Build();
#if DEBUG
            _freesql.CodeFirst.SyncStructure<EventMessage>();
#endif
        }

        public static IFreeSql Instance
        {
            get
            {
                return _freesql;
            }
        }

        private static string DbProvider => Config.Instance["agiledt:db:provider"];
        private static string DbConnection => Config.Instance["agiledt:db:conn"];

        private static FreeSql.DataType ProviderToFreesqlDbType(string provider)
        {
            switch (provider)
            {
                case "mysql":
                    return FreeSql.DataType.MySql;
                case "sqlserver":
                    return FreeSql.DataType.SqlServer;
                default:
                    break;
            }

            return FreeSql.DataType.Sqlite;
        }
    }
}
