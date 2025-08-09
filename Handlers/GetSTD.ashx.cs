using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;

namespace PTT.GQMS.USL.Web.Handlers
{
    // search standard gas history

    public class GetSTD : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = "";
            DataTable DT = null;
            try
            {

                string siteid =  context.Request.Form["siteid"] + "";
                string delstdid = context.Request.Form["delstdid"] + "";
               
                string sql = "";
                if (siteid != "" && delstdid != "")
                {

                    sql = "DELETE FROM O_SITE_SGC WHERE SITE_ID=" + siteid + " AND STD_ID=" + delstdid + " ";
                    Project.dal.ExecuteSQL(sql);
                    sql = "DELETE FROM O_SITE_TISI WHERE SITE_ID=" + siteid + " AND STD_ID=" + delstdid + " ";
                    Project.dal.ExecuteSQL(sql);
                }


                if (siteid != "")
                {
                     sql = "SELECT  SITE_ID, CYLINDER_NO, ORDER_DATE, EXPIRE_DATE, STD_ID " +
                        " ,TO_CHAR(ORDER_DATE, 'DD/MM/YYYY') ORDER_SHOW ,TO_CHAR(EXPIRE_DATE, 'DD/MM/YYYY') EXPIRE_SHOW  " + 
                        " FROM O_SITE_SGC " +
                        " WHERE SITE_ID = " + siteid +
                        " ORDER BY ORDER_DATE ";
                    DT = Project.dal.QueryData(sql);
                }


                DT.TableName = "data";
                DataSet ds = new DataSet();
                ds.Tables.Add(DT);
                json = JsonConvert.SerializeObject(ds);
                Utility.ClearObject(ref ds);
                Utility.ClearObject(ref DT);

                context.Response.ContentType = "text/json";
                context.Response.Write(json);

            }
            catch (Exception ex)
            {
                throw (ex);
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}