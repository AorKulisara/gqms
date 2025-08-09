using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;

namespace PTT.GQMS.USL.Web.Handlers
{
    //-- edit 14/08/2018 --
    /// <summary>
    /// ดึงข้อมูลรายวันจาก GQMS_DAILY_UPDATE
    /// </summary>
    public class GetGQMS_DAILY_UPDATE : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = "";
            DataTable DT = null;
            string FindFID = "";
            string FindRDate = "";
            string curFID = "";
            String sql = ""; String criteria = "";
            try
            {

                FindFID = Utility.ToString(context.Request.QueryString["pName"]).Trim();
                FindRDate = Utility.ToString(context.Request.QueryString["pDATE"]).Trim();
                curFID = Utility.ToString(context.Request.QueryString["curFID"]).Trim();

                if ( FindFID != "" && FindRDate != "")
                {
                    //-- กรณีที่ current FID เป็นตัวเดียวกับ FindFID ก็พิจารณาว่ามีฟิลด์สำรอง L หรือไม่ ถ้ามีน่าจะใช้ตัวสำรอง L เพราะตัวจริงอาจจะแก้ไขทับไปแล้ว

                    Project.dal.AddCriteria(ref criteria, "D.FID", FindFID, DBUTIL.FieldTypes.ftText);
                    Project.dal.AddCriteriaRange(ref criteria, "D.RDATE", Utility.AppDateValue(FindRDate), Utility.AppDateValue(FindRDate), DBUTIL.FieldTypes.ftDate);

                    sql = " SELECT D.* FROM GQMS_DAILY_UPDATE D  ";
                    sql += " WHERE " + criteria; 
                    sql += " ORDER BY D.RDATE ";

                    DT = Project.dal.QueryData(sql);

                    if ( FindFID == curFID)  //-- กรณีที่ current FID เป็นตัวเดียวกับ FindFID ก็พิจารณาว่ามีฟิลด์สำรอง L หรือไม่ ถ้ามีน่าจะใช้ตัวสำรอง L เพราะตัวจริงอาจจะแก้ไขทับไปแล้ว แต่ต้องตรวจสอบว่ามีค่าสำรอง L อยู่หรือเปล่า
                    {
                        if ( Utility.ToString(DT.Rows[0]["LC1"]) != "" || Utility.ToString(DT.Rows[0]["LC2"]) != "")
                        {
                            sql = "SELECT D.LC1 C1, D.LC2 C2, D.LC3 C3, D.LIC4 IC4, D.LNC4 NC4, D.LIC5 IC5, D.LNC5 NC5, D.LC6 C6, " +
                                " D.LN2 N2, D.LCO2 CO2, D.LSG SG, D.LGHV GHV, D.LNHV NHV, D.LWC WC, D.LGHVFC GHVFC, D.LUNNORMALIZED UNNORMALIZED, " +
                                " D.LWB WB, D.LH2S H2S, D.LC7 C7, D.LC8 C8, D.LUNNORMMIN UNNORMMIN, D.LUNNORMMAX UNNORMMAX " + 
                                " FROM GQMS_DAILY_UPDATE D  ";
                            sql += " WHERE " + criteria;
                            sql += " ORDER BY D.RDATE ";
                            DT = Project.dal.QueryData(sql);
                        }

                    }


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