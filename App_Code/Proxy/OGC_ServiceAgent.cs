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

//-- EDIT 02/06/2024 --
namespace PTT.GQMS.USL.Web
{
    public class OGC_ServiceAgent
    {

        //{"Location":["YETAGUN","YADANA"],"FromDate":"24-03-2024","FromHour":"","ToDate":"24-03-2024","ToHour":"","Source":"OGC"}
        //-- FromDate, ToDate ส่งมาเป็น YMD เช่น 20241201 ต้องแปลงให้เป็นตามที่ api กำหนด
        public static bool OGC_Invoke(string LocationList, string FromDate, string ToDate, ref string Response)
        {
            WebClient WC = null;
            string WS_URL = "";
            string postData = "";
            string KeyId = "";
            string fDate = "";

            try
            {
                WS_URL = @ConfigurationManager.AppSettings["OGCURL"];
                KeyId = @ConfigurationManager.AppSettings["OGC_KeyId"];

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

                    //{"Location":["YETAGUN","YADANA"],"FromDate":"24-03-2024","FromHour":"","ToDate":"24-03-2024","ToHour":"","Source":"OGC"}
                    postData = "{\"Source\":\"OGC\"";
                    if (FromDate != "")
                    {   //YYYYMMDD --> DD-MM-YYYY
                        fDate = FromDate.Substring(6, 2) + "-" + FromDate.Substring(4, 2) + "-" + FromDate.Substring(0, 4);
                        postData += ",\"FromDate\":\"" + fDate + "\" ";
                        if (ToDate != "")
                        {
                            fDate = ToDate.Substring(6, 2) + "-" + ToDate.Substring(4, 2) + "-" + ToDate.Substring(0, 4);
                        }
                        else
                        {
                            fDate = Utility.FormatDate(System.DateTime.Today, "DD-MM-YYYY");
                        }
                        postData += ",\"ToDate\":\"" + fDate + "\" ";
                    }
                    if (LocationList != "")
                    {
                        LocationList = LocationList.Replace("'", "\"");
                        postData += ",\"Location\":[" + LocationList + "] ";
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
                string MailMSG = "Can not call : " + WS_URL + " <br/><br/> Message : " + ex.Message ;
                mdlMail.SendMailError("GQMS: OGC API", MailMSG);


                return false;
            }
            finally
            {

            }

        }



    }
}