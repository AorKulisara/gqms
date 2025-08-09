using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;

namespace PTT.GQMS.USL.Web.Handlers
{
    //-- aor edit 02/06/2018 --
    /// <summary>
    /// List FID
    
    public class ListFID : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = "";
            DataTable DT = null;
            string SQL = "";
            string FindName = "";
            try
            {

                FindName = Utility.FormatSearchData(Validation.ValidateStr(Utility.ToString(context.Request.QueryString["pName"]))).ToUpper().Trim();

                if (FindName != "")
                {

                   SQL = " SELECT FID AS value, FID AS label FROM O_SITE_FID " +
                        " WHERE UPPER(FID) LIKE '" + FindName + "' " +
                        " ORDER BY FID ";

                    DT = Project.dal.QueryData(SQL);
                        json = JsonConvert.SerializeObject(DT);
                        context.Response.ContentType = "text/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    context.Response.Cache.SetExpires(DateTime.Now);
                    context.Response.Write(json);


                }


            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
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