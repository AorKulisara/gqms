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

//-- EDIT 11/08/2023 --
namespace PTT.GQMS.USL.Web
{
    
    public class GIS_Service_Agent
    {

        public static bool GIS_Invoke(string permanetCodeList, ref string Response)
        {
            WebClient WC = null;
            string WS_URL = "";
            string postData = "";
            string KeyId = "";

            try
            {
                WS_URL = @ConfigurationManager.AppSettings["GISURL_StationLocation"];
                KeyId = @ConfigurationManager.AppSettings["GIS_KeyId"];

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
                    permanetCodeList = permanetCodeList.Replace("'","\"");
                    postData = "{\"PERMANENT_CODE\":[" + permanetCodeList +"]}";
                    Response = Encoding.UTF8.GetString(WC.UploadData(WS_URL, "POST", Encoding.UTF8.GetBytes(postData)));
          
                    return true;
                }
            }
            catch (Exception ex)
            {

                Response = Utility.GetErrorMessage(ex);

                //--- edit 19/03/2025 -- ส่งเมล์แจ้ง error 
                string MailMSG = "Can not call : " + WS_URL + " <br/><br/> Message : " + ex.Message;
                mdlMail.SendMailError("GQMS: GIS API", MailMSG);

                return false;
            }
            finally
            {
               
            }

        }



    }

}
