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
    //-- edit 02/07/2019 --

    public partial class ExcelON : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String SIDList = "", TmplateList = "", MM1 = "", YY1 = "", MM2 = "", YY2 = "";
        String pType = "";
        public String showFID = "", showMonth = "", showYear = "";

        public String rOpenFile = "ONSHORE_SUMMARY.xlsx";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!this.IsPostBack)
                {
                    TmplateList = Validation.GetParamStr("T", IsEncoded: false); //3,4,5,6,
                    if (TmplateList != "") TmplateList = Utility.Left(TmplateList, TmplateList.Length - 1);
                    SIDList = Validation.GetParamStr("F", IsEncoded: false);
                    if (SIDList != "") SIDList = Utility.Left(SIDList, SIDList.Length - 1);
                    pType = Validation.GetParamStr("C", IsEncoded: false);
                    MM1 = Validation.GetParamStr("MM1", IsEncoded: false);
                    YY1 = Validation.GetParamStr("YY1", IsEncoded: false);
                    MM2 = Validation.GetParamStr("MM2", IsEncoded: false);
                    YY2 = Validation.GetParamStr("YY2", IsEncoded: false);

                    if ((TmplateList + SIDList != "") && pType != "" && MM1 != "" && YY1 != "" && MM2 != "" && YY2 != "")
                    {
                        WriteXSL();
                    }

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }


        private void WriteXSL()
        {
            DataTable DTH = null;
            DataTable DT = null;
            string SQL = "", CRI = "", ORDER = "";
            String fromDate = "", toDate = "";
            try
            {
                string srcFileName = "ONSHORE_SUMMARY_TEMPLATE.xlsx";
                string destFileName = "ONSHORE_SUMMARY_" + pType +"_" + Utility.EnMonthAbbr(Utility.ToInt(MM1)) + YY1 + ".xlsx";
                string srcFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + srcFileName);
                string destFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + "Export/" + destFileName);

                FileInfo srcFile = new FileInfo(srcFullFileName);
                FileInfo destFile = new FileInfo(destFullFileName);
                ExcelPackage p = new ExcelPackage(srcFile);
                ExcelWorkbook myWorkbook = p.Workbook;
                //--Should return the sheet name
                ExcelWorksheet myWorksheet = myWorkbook.Worksheets.FirstOrDefault();
                ExcelRange cell;

                //===== Report header =====================================================
                cell = myWorksheet.Cells["B1"]; //-- report name
                cell.Value = "Onshore summary " + "( " + Utility.EnMonth(Utility.ToInt(MM1)) + " " + YY1 + " - " + Utility.EnMonth(Utility.ToInt(MM2)) + " " + YY2 + " )";

                if (TmplateList != "")
                {
                    cell = myWorksheet.Cells["B2"]; //-- template name
                    
                    DTH = Project.dal.SearchRptFidTemplate("1", "", "", orderSQL: " TID", OtherCriteria: " TID in (" + TmplateList +") ");
                    string HName = "";
                    foreach (DataRow DRH in DTH.Rows)
                    {
                        if (HName != "") HName += ", ";
                        HName += Utility.ToString(DRH["T_NAME"]);
                    }
                    cell.Value = HName;
                }



                //===== data =====================================================

                fromDate = "01/" + MM1.PadLeft(2, '0') + "/" + YY1;
                string tmpDate = "01/" + MM2.PadLeft(2, '0') + "/" + YY2;
                toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(tmpDate)).AddMonths(1).AddDays(-1));

                //pType ="10DAY","15DAY","ENDMTH"
                //-- edit 27/05/2024 เพิ่ม "1DAY"
                DT = Project.dal.SearchRptOnshoreSummary(pType, fromDate, toDate, TmplateList, SIDList, "", "");
                int rw = 5;  //-- start row 5
 

                foreach (DataRow DR in DT.Rows)
                {
                    cell = myWorksheet.Cells[rw, 1];

                    if (showFID != Utility.ToString(DR["FID"]))
                    {
                        showFID = Utility.ToString(DR["FID"]);
                        showMonth = ""; showYear = "";
                        cell.Value = showFID;
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        cell.Value = "";
                    }

                    cell = myWorksheet.Cells[rw, 2];
                    if (showYear != Utility.ToString(DR["YY"]))
                    {
                        showYear = Utility.ToString(DR["YY"]);
                        cell.Value = showYear;
                        showMonth = "";
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        cell.Value = "";
                    }

                    cell = myWorksheet.Cells[rw, 3];
                    if (showMonth != Utility.EnMonth(Utility.ToInt(DR["MM"])))
                    {
                        showMonth = Utility.EnMonth(Utility.ToInt(DR["MM"]));
                        cell.Value = showMonth;
                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        for (int i = 4; i < 23; i++)
                        {
                            myWorksheet.Cells[rw, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        }
                    }
                    else
                    {
                        cell.Value = "";
                    }


                    cell = myWorksheet.Cells[rw, 4];
                    string showDay = "";
                    switch (Utility.ToString(DR["DDAY"]))
                    {
                        case "D1": showDay = "1-10"; break;
                        case "D2": showDay = "11-20"; break;
                        case "D3":
                            showDay = "21-" + Utility.LastDayofMonth(Utility.ToInt(DR["YY"]), Utility.ToInt(DR["MM"])); break;
                        case "D4": showDay = "1-15"; break;
                        case "D5":
                            showDay = "16-" + Utility.LastDayofMonth(Utility.ToInt(DR["YY"]), Utility.ToInt(DR["MM"])); break;
                        case "D6":
                            showDay = "1-" + Utility.LastDayofMonth(Utility.ToInt(DR["YY"]), Utility.ToInt(DR["MM"])); break;
                    }
                    cell.Value = showDay;

                    WriteValue(ref myWorksheet, rw, 5, DR["C1"]);
                    WriteValue(ref myWorksheet, rw, 6, DR["C2"]);
                    WriteValue(ref myWorksheet, rw, 7, DR["C3"]);
                    WriteValue(ref myWorksheet, rw, 8, DR["IC4"]);
                    WriteValue(ref myWorksheet, rw, 9, DR["NC4"]);
                    WriteValue(ref myWorksheet, rw, 10, DR["IC5"]);
                    WriteValue(ref myWorksheet, rw, 11, DR["NC5"]);
                    WriteValue(ref myWorksheet, rw, 12, DR["C6"]);
                    WriteValue(ref myWorksheet, rw, 13, DR["CO2"]);
                    WriteValue(ref myWorksheet, rw, 14, DR["N2"]);
                    WriteValue(ref myWorksheet, rw, 15, DR["HG"]);
                    WriteValue(ref myWorksheet, rw, 16, DR["H2S"]);
                    WriteValue(ref myWorksheet, rw, 17, DR["GHV"]);
                    WriteValue(ref myWorksheet, rw, 18, DR["SG"]);
                    WriteValue(ref myWorksheet, rw, 19, DR["CALC_WI"]);  //WI เดิมใช้ WB //-- 25/09/2019  เปลี่ยนเป็นคำนวณ
                    WriteValue(ref myWorksheet, rw, 20, DR["WC"]);  //H2O
                    WriteValue(ref myWorksheet, rw, 21, DR["SUM_C2"]);  //C2+
                    WriteValue(ref myWorksheet, rw, 22, DR["CO2_N2"]);  //CO2+N2

                    rw++;
                }

                //-- ขึดเส้นท้ายตาราง
                for (int i = 1; i < 23; i++)
                {
                    myWorksheet.Cells[rw, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                }



                myWorksheet.Name = Utility.EnMonthAbbr(Utility.ToInt(MM1)); //-- กำหนดชื่อ worksheet เป็นเดือน
                p.SaveAs(destFile);

                rOpenFile = destFileName;

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DTH);
                Utility.ClearObject(ref DT);
            }

        }

        //-- write composition cell
        private void WriteValue(ref ExcelWorksheet WS, int sRow, int sCol, Object gDR)
        {
            ExcelRange cell;
            try
            {

                cell = WS.Cells[sRow, sCol];
                if (Utility.IsNumeric(gDR))
                    cell.Value = Utility.ToNum(gDR);
                else
                    cell.Value = Utility.ToString(gDR);

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }

        }



    }
}