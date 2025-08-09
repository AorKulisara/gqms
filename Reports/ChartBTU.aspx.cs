using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using System.IO;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PTT.GQMS.USL.Web.Reports
{ 
    //-- edit 22/08/2018 --
    public partial class ChartBTU : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String SiteIDList = "", TmplateList = "", MM = "", YY = "";

        public String chartLabel = "";
        public String chartData = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!this.IsPostBack)
                {
                    TmplateList = Validation.GetParamStr("T", IsEncoded: false); //3,4,5,6,
                    if (TmplateList != "") TmplateList = Utility.Left(TmplateList, TmplateList.Length - 1);
                    SiteIDList = Validation.GetParamStr("F", IsEncoded: false);
                    if (SiteIDList != "") SiteIDList = Utility.Left(SiteIDList, SiteIDList.Length - 1);
                    MM = Validation.GetParamStr("MM", IsEncoded: false);
                    YY = Validation.GetParamStr("YY", IsEncoded: false);

                    if ((TmplateList + SiteIDList != "") &&  MM != "" && YY != "")
                    {
                        lblPERIOD.Text = Utility.EnMonth(Utility.ToInt(MM)) + " " + YY;

                        GenerateChart();
                    }

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }


        private void GenerateChart()
        {
            DataTable DT = null;
            string SQL = "";
            try
            {
                //chartLabel = "\"0\",\"1\", \"2\", \"3\", \"4\", \"5\", \"6\", \"7\"";
                //chartData = "{ label: 'BCS', data: [, 94.235, , , , , , ], borderWidth: 6, borderColor: 'rgba(204, 0, 0, 1)', pointRadius: 4, },;


                string fromDate = "01/" + MM.PadLeft(2, '0') + "/" + YY;
                string toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));

                //--30/08/2018 เปลี่ยนมาใช้ BTU-->GHV ตรวจสอบวันที่ด้วย
                if (TmplateList != "")
                {
                    SQL = "SELECT S.SITE_ID, D.FID, ROUND(AVG(to_numbernull3(GHV)),3) AS BTU  " +
                    "  FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID   " +
                    "  WHERE D.RDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND D.RDATE< TO_DATE('" + toDate + "', 'DD/MM/YYYY')   " +
                    "  AND S.SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_SITE_FID S ON T.FID = S.FID  WHERE TID IN (" + TmplateList + "))  " +
                    "  GROUP BY D.FID ,S.SITE_ID ORDER BY D.FID ";

                }
                else
                {
                    SQL = "SELECT S.SITE_ID, D.FID, ROUND(AVG(to_numbernull3(GHV)),3) AS BTU  " +
                    "  FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID   " +
                    "  WHERE D.RDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND D.RDATE< TO_DATE('" + toDate + "', 'DD/MM/YYYY')   " +
                    "  AND S.SITE_ID IN (" + SiteIDList + ") " +
                    "  GROUP BY D.FID ,S.SITE_ID ORDER BY D.FID ";
                }

                int rwCnt = 0, rw = 0;
 
                chartLabel = "\"0\","; //เริ่มจาก 0 เพื่อไม่ให้จุดติดแกน y
                chartData = "";
                DT = Project.dal.QueryData(SQL);
                rwCnt = DT.Rows.Count;
                foreach (DataRow DR in DT.Rows)
                {
                    rw++;
                    chartLabel += "\"" + rw +"\",";

                    chartData += "{ label: '" + Utility.ToString(DR["FID"]) + "', data: [";
                    string data = ",";
                    for (int i=1; i<=rwCnt; i++)
                    {
                        if (i == rw) data += Utility.FormatCheckNumNoComma(DR["BTU"], 3); ;

                        data += ",";

                    }
                    chartData += data + "], borderWidth: 6, borderColor: '" + BgColor(rw) + "', pointRadius: 4, },";
                }

                chartLabel += "\"" + (rw+1) + "\","; //เพิ่มจุดสุดท้าย เพื่อไม่ให้จุดติดกราฟ

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
            }


        }


        private string BgColor(int indx)
        {
            String bgColor = "";
            try
            {
                switch (indx % 15)
                {
                    case 0: bgColor = "rgba(204, 0, 0, 1)"; break;
                    case 1: bgColor = "rgba(0, 153, 255, 1)"; break;
                    case 2: bgColor = "rgba(204, 0, 204, 1)"; break;
                    case 3: bgColor = "rgba(51, 204, 51, 1)"; break;
                    case 4: bgColor = "rgba(255, 153, 0, 1)"; break;
                    case 5: bgColor = "rgba(0, 0, 204, 1)"; break;
                    case 6: bgColor = "rgba(102, 255, 127,1)"; break;
                    case 7: bgColor = "rgba(255, 202, 31, 1)"; break;
                    case 8: bgColor = "rgba(96, 20, 2,1)"; break;
                    case 9: bgColor = "rgba(164, 167, 168,1)"; break;
                    case 10: bgColor = "rgba(255, 64, 44, 1)"; break;
                    case 11: bgColor = "rgba(179, 255, 2, 1)"; break;
                    case 12: bgColor = "rgba(1, 255, 246, 1)"; break;
                    case 13: bgColor = "rgba(0, 64, 255, 1)"; break;
                    case 14: bgColor = "rgba(255, 0, 255, 1)"; break;
                }

                return bgColor;
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
                return "";
            }



        }

    }
}