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
    // 30/08/2018  ถ้าเลือก site แค่ 1 เดียว ตัวสุดท้ายไม่มา
    //-- edit 28/06/2019 -- เปลี่ยนเงื่อนไขจาก month เป็น date from-to
    public partial class ChartCompare : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String SiteIDList = "", TmplateList = "", Comp = "", MM = "", YY = "";
        String DateF = "", DateT = "";

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
                    Comp = Validation.GetParamStr("C", IsEncoded: false);
                    //MM = Validation.GetParamStr("MM", IsEncoded: false);
                    //YY = Validation.GetParamStr("YY", IsEncoded: false);
                    //-- edit 13/05/2019 --
                    DateF = Validation.GetParamStr("DF", IsEncoded: false);
                    DateT = Validation.GetParamStr("DT", IsEncoded: false);

                    //if ((TmplateList + SiteIDList != "") && Comp != "" && MM != "" && YY != "")
                    if ((TmplateList + SiteIDList != "") && Comp != "" && DateF != "" && DateT != "")
                    {
                        // lblPERIOD.Text = Utility.EnMonth(Utility.ToInt(MM)) + " " + YY;
                        lblPERIOD.Text = DateF + " - " + DateT;
                        lblCOMP.Text = Project.CompositionName(Comp);
                        if (Project.CompositionUnit(Comp) != "") lblCOMP.Text += " (" + Project.CompositionUnit(Comp) + ")";


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
            DataTable DTdy = null;
            DataTable DT = null;
            string SQL = "";
            try
            {
                //chartLabel = "\"1\", \"2\", \"3\", \"4\", \"5\", \"6\", \"7\", \"8\", \"9\", \"10\", \"11\", \"12\", \"13\", \"14\", \"15\", \"16\", \"17\", \"18\", \"19\", \"20\", \"21\", \"22\", \"23\", \"24\", \"25\", \"26\", \"27\", \"28\", \"29\", \"30\"";
                //chartData = "{ label: 'BCS', data: [, 94.235, 94.016, 93.958, 94.118, 94.168, 94.104, 94.029, 93.979, 94.134, 94.151, 94.18, 93.977, 94.24, 94.133, 93.929, 94.05, 93.955, 93.992, 94.098, 93.031, 92.987, 93.628, 93.308, 92.725, 92.618, 92.81, 93.431, 93.295, 93.976, 94.199], borderWidth: 2, borderColor: 'rgba(204, 0, 0, 1)', pointRadius: 2, },";


                //string fromDate = "01/" + MM.PadLeft(2, '0') + "/" + YY;
                //string toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));
                //-- edit 13/05/2019
                string fromDate = DateF;
                string toDate = DateT;
                
                int fromDayOfYear = Convert.ToDateTime(Utility.AppDateValue(fromDate)).DayOfYear;

                SQL = " SELECT * FROM O_DIM_DATE " +  
                     " WHERE ADATE>=TO_DATE('" + fromDate + "','DD/MM/YYYY') AND ADATE<=TO_DATE('" + toDate + "','DD/MM/YYYY') " +
                     " ORDER BY ADATE ";
                
                DTdy = Project.dal.QueryData(SQL);
                chartLabel = "";
                String[] day = new String[DTdy.Rows.Count];
                int monthDAY = DTdy.Rows.Count;
                int i = 0;
                foreach (DataRow DRdy in DTdy.Rows)
                {
                    //chartLabel += "\"" + Convert.ToDateTime(DRdy["ADATE"]).Day +"\",";
                    //-- edit 13/05/2019 -- แสดงวันเดือนปี
                    chartLabel += "\"" + Utility.AppFormatDate(DRdy["ADATE"]) + "\",";
                    day[i] = Utility.AppFormatDate(DRdy["ADATE"]);
                    i++;
                }
                

                if ( TmplateList != "" )
                {
                    SQL = " SELECT D.* " +
                    " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
                    " WHERE RDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND RDATE<= TO_DATE('" + toDate + "', 'DD/MM/YYYY') " +
                    " AND SITE_ID IN (SELECT DISTINCT S.SITE_ID FROM O_RPT_FID_DETAIL T INNER JOIN O_SITE_FID S ON T.FID = S.FID WHERE TID IN ("+ TmplateList+")) " +
                    "ORDER BY D.FID, D.RDATE ";
                }
                else
                {
                    SQL = " SELECT D.* " +
                    " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID = S.FID " +
                    " WHERE RDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND RDATE<= TO_DATE('" + toDate + "', 'DD/MM/YYYY') " +
                    " AND SITE_ID IN (" + SiteIDList + ") " +
                    "ORDER BY D.FID, D.RDATE ";
                }


                int fidCnt = 0;
                String FID = "";
                String[] data = new String[monthDAY];
                chartData = "";
                DT = Project.dal.QueryData(SQL);
                foreach (DataRow DR in DT.Rows)
                {
                    if ( FID != Utility.ToString(DR["FID"]) )
                    {
                        if ( FID != "")
                        {
                            chartData += "{ label: '" + FID + "', data: [";
                            for (int cnt = 0; cnt < monthDAY; cnt++)
                            {
                                chartData += data[cnt] + ",";
                            }
                            chartData += "], borderWidth: 2, borderColor: '" + BgColor(fidCnt) +"', pointRadius: 2, },";
                            fidCnt++;
                        }

                        FID = Utility.ToString(DR["FID"]);
                        //-- clear data ---
                        for (int cnt = 0; cnt < monthDAY; cnt++)
                        {
                            data[cnt] = "";
                        }
                    }


                    //int d = Convert.ToDateTime(DR["RDATE"]).Day-1;
                    //-- edit 13/05/2019 ---
                    int d = Array.IndexOf(day, Utility.AppFormatDate(DR["RDATE"]));
                    
                    //-- edit 18/03/2021 ---
                    if (Comp == "SG")
                        data[d] = Utility.FormatCheckNumNoComma(DR[Comp], 4);
                    else
                        data[d] = Utility.FormatCheckNumNoComma(DR[Comp], 3);



                }

                //generate FID ตัวสุดท้าย
                if (FID != "")
                {
                    chartData += "{ label: '" + FID + "', data: [";
                    for (int cnt = 0; cnt < monthDAY; cnt++)
                    {
                        chartData += data[cnt] + ",";
                    }
                    chartData += "], borderWidth: 2, borderColor: '" + BgColor(fidCnt) + "', pointRadius: 2, },";
                    fidCnt++;
                }
                //-- clear data ---
                for (int cnt = 0; cnt < monthDAY; cnt++)
                {
                    data[cnt] = "";
                }


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DTdy);
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