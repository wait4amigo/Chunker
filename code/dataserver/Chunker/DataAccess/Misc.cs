using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Chunker.DataAccess
{
    public class Misc : DBBase
    {
        ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Misc()
        {

        }

        public string Get(string fieldName, ref string errorInfo)
        {

            _log.Debug("Get misc info " + fieldName);

            string sql = string.Format("select field_value from misc where field_name='{0}'", fieldName);
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query " + fieldName);

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "Not exist";
                return null;
            }

            return ds.Tables[0].Rows[0][0].ToString();
        }
    }
}