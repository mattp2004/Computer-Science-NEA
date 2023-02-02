using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.sql
{
    public class SqlRepository
    {
        public SQLController sqlController;

        public SqlRepository(SQLController controller)
        {
            sqlController = controller;
        }
    }
}