using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;

namespace PTT.GQMS.USL.Web.Handlers
{
    //-- edit 19/07/2019 --
    /// <summary>
    /// ดึงข้อมูล SPOT แต่ละ table 
    /// </summary>
    public class GetSPOT_Data : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = "";
            DataTable DT = null;
            string FindType = "";
            string FindSpotName = "";
            string FindMM = "";
            string FindYY = "";
            String sql = ""; String criteria = "";
            try
            {
                FindType = Utility.ToString(context.Request.QueryString["pType"]).Trim();
 
               // unescape(userinput)
                FindSpotName = Utility.ToString(context.Request.QueryString["pName"]).Trim();
                FindMM = Utility.ToString(context.Request.QueryString["pMM"]).Trim();
                FindYY = Utility.ToString(context.Request.QueryString["pYY"]).Trim();

                if (FindType != "" && FindSpotName != "" && FindMM != "" && FindYY != "")
                {
                    if (FindMM.Length == 1) FindMM = "0"+FindMM;
                    Project.dal.AddCriteria(ref criteria, "TO_CHAR(SDATE, 'MMYYYY')", FindMM + FindYY, DBUTIL.FieldTypes.ftText);

                    FindSpotName = FindSpotName + "%";
                    switch (FindType)
                    {
                        case "H2S":
                            Project.dal.AddCriteria(ref criteria, "H2S_NAME", FindSpotName, DBUTIL.FieldTypes.ftText);
                            sql = "SELECT * FROM O_OGC_H2S ";
                            break;
                        case "HG":
                            Project.dal.AddCriteria(ref criteria, "HG_NAME", FindSpotName, DBUTIL.FieldTypes.ftText);
                            sql = "SELECT * FROM O_OGC_HG ";
                            break;
                        case "O2":
                            Project.dal.AddCriteria(ref criteria, "O2_NAME", FindSpotName, DBUTIL.FieldTypes.ftText);
                            sql = "SELECT * FROM O_OGC_O2 ";
                            break;
                        case "HC":
                            Project.dal.AddCriteria(ref criteria, "HC_NAME", FindSpotName, DBUTIL.FieldTypes.ftText);
                            sql = "SELECT * FROM O_OGC_HC ";
                            break;
                    }

                    sql += " WHERE " + criteria;
                    sql += " ORDER BY SDATE ";

                    DT = Project.dal.QueryData(sql);

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