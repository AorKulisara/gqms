using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Net;
using System.Data;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;

//-- EDIT 03/06/2024 --
namespace PTT.GQMS.USL.Web
{
    public class FLOW_ServiceAgent
    {

        //{"FROM_DATE" : "24-MAR-24", "FROM_HOUR" : "20", "TO_DATE" : "25-MAR-24", "TO_HOUR" : "10", "LIST" : "HOUR_FLOWRATE"}
        //1.	DAY_FLOWRATE = VW_ARCH_DAY_FLOWRATE
        //2.	HOUR_FLOWRATE = VW_ARCH_HOUR_FLOWRATE
        //3.	DAY_MOISTURE = VW_ARCH_DAY_MOISTURE
        //4.	HOUR_MOISTURE = VW_ARCH_HOUR_MOISTURE
        //-- FromDate, ToDate ส่งมาเป็น YMD เช่น 20241201 ต้องแปลงให้เป็นตามที่ api กำหนด
        public static bool FLOW_Invoke(string TypeList, string FromDate, string FromHour, string ToDate, string ToHour, ref string Response)
        {
            WebClient WC = null;
            string WS_URL = "";
            string postData = "";
            string KeyId = "";
            string fDate = "";

            try
            {
                WS_URL = @ConfigurationManager.AppSettings["FLOWURL"];
                KeyId = @ConfigurationManager.AppSettings["FLOW_KeyId"];

                if (WS_URL == "" || KeyId == "")
                {
                    return false;
                }
                else
                {

                    WC = new WebClient();
                    WC.Headers.Add("Content-Type", "application/json");
                    WC.Headers.Add("KeyId", KeyId);

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    //{"FROM_DATE" : "24-MAR-24", "FROM_HOUR" : "20", "TO_DATE" : "25-MAR-24", "TO_HOUR" : "10", "LIST" : "HOUR_FLOWRATE"}
                    postData = "{\"LIST\":\"" + TypeList + "\"";
                    if (FromDate != "")
                    {   //YYYYMMDD --> DD-MON-YYYY
                        System.Globalization.CultureInfo EN = new System.Globalization.CultureInfo("en-US");
                        DateTime LastDate = Convert.ToDateTime(DateTime.ParseExact(FromDate.ToString(), @"yyyyMMdd", EN));
                        fDate = Utility.FormatDate(LastDate, "DD-MON-YY");
                        postData += ",\"FROM_DATE\":\"" + fDate + "\" ";
                        if (ToDate != "")
                        {
                            LastDate = Convert.ToDateTime(DateTime.ParseExact(ToDate.ToString(), @"yyyyMMdd", EN));
                            fDate = Utility.FormatDate(LastDate, "DD-MON-YY");
                        }
                        else
                        {
                            fDate = Utility.FormatDate(System.DateTime.Today, "DD-MON-YY");
                        }
                        postData += ",\"TO_DATE\":\"" + fDate + "\" ";

                        if (FromHour == "") FromHour = "00";
                        postData += ",\"FROM_HOUR\":\"" + FromHour + "\" ";
                        if (ToHour == "") ToHour = "23";
                        postData += ",\"TO_HOUR\":\"" + ToHour + "\" ";
                      
                    }

                    postData += "}";


                    Response = Encoding.UTF8.GetString(WC.UploadData(WS_URL, "POST", Encoding.UTF8.GetBytes(postData)));

                    return true;
                }
            }
            catch (Exception ex)
            {

                Response = Utility.GetErrorMessage(ex);
                //--- edit 19/03/2025 -- ส่งเมล์แจ้ง error 
                string MailMSG = "Can not call : " + WS_URL + " <br/><br/> Message : " + ex.Message;
                mdlMail.SendMailError("GQMS: FLOW API", MailMSG);

                return false;
            }
            finally
            {

            }

        }



    }
}