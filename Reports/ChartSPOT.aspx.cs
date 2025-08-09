using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using System.IO;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;


namespace PTT.GQMS.USL.Web.Reports
{
    //-- edit 26/06/2019 --

    public partial class ChartSPOT : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String FIDList = "", TmplateList = "", Comp = "", MM1 = "", YY1 = "", MM2 = "", YY2 = "";
    
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
                    FIDList = Validation.GetParamStr("F", IsEncoded: false);
                    if (FIDList != "") FIDList = Utility.Left(FIDList, FIDList.Length - 1);
                    Comp = Validation.GetParamStr("C", IsEncoded: false);
                    MM1 = Validation.GetParamStr("MM1", IsEncoded: false);
                    YY1 = Validation.GetParamStr("YY1", IsEncoded: false);
                    MM2 = Validation.GetParamStr("MM2", IsEncoded: false);
                    YY2 = Validation.GetParamStr("YY2", IsEncoded: false);
                

                    if ((TmplateList + FIDList != "") && Comp != "" && MM1 != "" && YY1 != "" && MM2 != "" && YY2 != "")
                    {
                        lblPERIOD.Text = Utility.EnMonth(Utility.ToInt(MM1)) + " " + YY1 + "  -  "+ Utility.EnMonth(Utility.ToInt(MM2)) + " " + YY2;
                        lblCOMP.Text = Project.SpotName(Comp);
                        if (Project.SpotUnit(Comp) != "") lblCOMP.Text += " (" + Project.SpotUnit(Comp) + ")";


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
            ArrayList alPeriod = new ArrayList();
            int cntPeriod = 0;
            try
            {
                //chartLabel = "\"Jan 2019\", \"Feb 2019\", \"Mar 2019\", \"Apr 2019\", \"May 2019\", \"Jun 2019\", \"Jul 2019\", \"Aug 2019\", \"Sep 2019\", \"Oct 2019\", \"Nov 2019\", \"Dec 2019\",\"Jan 2020\", \"Feb 2020\", \"Mar 2020\"";
                //chartData = "{ label: 'OFFSHORE 34\"', data: [94.136,94.046,93.773,93.586,93.479,93.717,92.890,91.422,90.773,90.841,91.626,92.691,92.475,91.847,91.046,], borderWidth: 2, borderColor: 'rgba(204, 0, 0, 1)', pointRadius: 2, },{ label: 'OFFSHORE 36\"', data: [94.136,94.046,93.773,93.586,93.479,93.717,93.077,91.512,90.789,90.886,91.515,92.701,92.475,92.298,91.074,], borderWidth: 2, borderColor: 'rgba(0, 153, 255, 1)', pointRadius: 2, },{ label: 'Maptaput', data: [94.106,93.999,93.749,93.548,93.463,93.736,92.694,91.305,90.748,90.787,91.725,92.720,92.222,91.821,90.997,], borderWidth: 2, borderColor: 'rgba(204, 0, 204, 1)', pointRadius: 2, },";


                string fromDate = "01/" + MM1.PadLeft(2, '0') + "/" + YY1;
                string tmpDate = "01/" + MM2.PadLeft(2, '0') + "/" + YY2;
                string toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(tmpDate)).AddMonths(1).AddDays(-1));

                //-- เนื่องจากต้องแสดงทุกเดือนตามช่วงเดือนที่กำหนด 
                int YM1 = Utility.ToInt(YY1) * 100  + Utility.ToInt(MM1);
                int YM2 = Utility.ToInt(YY2) * 100 + Utility.ToInt(MM2);
                string tmpP = "";
                for (int y = Utility.ToInt(YY1); y <= Utility.ToInt(YY2); y++)
                {
                    for (int m = 1; m <= 12; m++)
                    {
                        if ((y * 100 + m) >= YM1 && (y * 100 + m) <= YM2)
                        {
                            tmpP = Utility.EnMonthAbbr(m) + " " + y;
                            chartLabel += "'" + tmpP + "',";  // 'Jan 2018','Feb 2018',
                            alPeriod.Add(Utility.ToString(y*100 + m));         //-- จัด array ของ PERIOD->YYYYMM
                            cntPeriod++;
                        }
                    }
                    
                }

                string TabName = "H2S";
                switch (Comp)
                {
                    case "SULFUR": //"H2S - Total Sulfur"  
                    case "H2S": //"H2S - H2S"  
                    case "COS": //"H2S - COS"  
                    case "CH3SH": //"H2S - CH3SH"  
                    case "C2H5SH": //"H2S - C2H5SH" 
                    case "DMS": //"H2S - DMS"  
                    case "LSH": //"H2S - T-bulylSH" 
                    case "C3H7SH": //"H2S - C3H7SH" 
                        TabName = "H2S";
                        break;
                    case "HG": //"HG"  
                    case "VOL": //"HG - Vol" 
                        TabName = "HG";
                        break;
                    case "O2": //"O2" 
                        TabName = "O2";
                        break;
                    case "HC": //"HC - Temp" 
                        TabName = "HC";
                        break;
                }

                if (TmplateList != "")
                {
                    SQL = "SELECT "+ TabName + "_NAME AS FID, O_OGC_" + TabName + ".* FROM O_OGC_" + TabName + " " +
                          " WHERE SDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND SDATE<= TO_DATE('" + toDate + "', 'DD/MM/YYYY')  " +
                          " AND " + TabName + "_NAME IN (SELECT FID FROM O_RPT_FID_DETAIL WHERE TID IN(" + TmplateList + ")) " +
                          "ORDER BY " + TabName + "_NAME, SDATE ";

                 

                }
                else
                {
                    FIDList = "'" + FIDList.Replace(",","','") +"'";

                    SQL = "SELECT " + TabName + "_NAME AS FID, O_OGC_" + TabName + ".* FROM O_OGC_" + TabName + " " +
                           " WHERE SDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND SDATE<= TO_DATE('" + toDate + "', 'DD/MM/YYYY')  " +
                           " AND replace(replace(" + TabName + "_NAME,'#',''),'\"','')  IN (" + FIDList + ") " +
                           "ORDER BY " + TabName + "_NAME, SDATE ";

             


                }


                int fidCnt = 0;
                String FID = "";
                String[] data = new String[cntPeriod];
                chartData = "";
                DT = Project.dal.QueryData(SQL);
                foreach (DataRow DR in DT.Rows)
                {
                    if (FID != Utility.ToString(DR["FID"]))
                    {
                        if (FID != "")
                        {
                            chartData += "{ label: '" + FID + "', data: [";
                            for (int cnt = 0; cnt < cntPeriod; cnt++)
                            {
                                chartData += data[cnt] + ",";
                            }
                            chartData += "], borderWidth: 2, borderColor: '" + BgColor(fidCnt) + "', pointRadius: 2, },";
                            fidCnt++;
                        }

                        FID = Utility.ToString(DR["FID"]);
                        //-- clear data ---
                        for (int cnt = 0; cnt < cntPeriod; cnt++)
                        {
                            data[cnt] = "";
                        }
                    }


 
                    //-- ใส่ข้อมูลใน array
                    int indexItem = alPeriod.IndexOf(Utility.FormatDate(Convert.ToDateTime(DR["SDATE"]),"YYYYMM"));
                    
                    if ( indexItem > -1 ) data[indexItem] = Utility.FormatCheckNumNoComma(DR[Comp], 3); //-- แสดงทศนิยม  



                }

                //generate FID ตัวสุดท้าย
                if (FID != "")
                {
                    chartData += "{ label: '" + FID + "', data: [";
                    for (int cnt = 0; cnt < cntPeriod; cnt++)
                    {
                        chartData += data[cnt] + ",";
                    }
                    chartData += "], borderWidth: 2, borderColor: '" + BgColor(fidCnt) + "', pointRadius: 2, },";
                    fidCnt++;
                }
                //-- clear data ---
                for (int cnt = 0; cnt < cntPeriod; cnt++)
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