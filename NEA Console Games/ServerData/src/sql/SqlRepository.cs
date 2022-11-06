using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.sql
{
    class SqlRepository
    {
        public SQLController sqlController;

        public SqlRepository(SQLController controller)
        {
            sqlController = controller;
        }

        public OdbcDataReader Select(string SQL)
        {
            OdbcCommand sqlToRun = new OdbcCommand(SQL, sqlController.connection);
            OdbcDataReader results = sqlToRun.ExecuteReader();
            return results;
        }
    }
}
