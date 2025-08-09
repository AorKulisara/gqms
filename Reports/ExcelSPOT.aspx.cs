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
    //-- edit 27/06/2019 --

    public partial class ExcelSPOT : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String FIDList = "", TmplateList = "", Comp = "", MM1 = "", YY1 = "", MM2 = "", YY2 = "";


        public String rOpenFile = "SPOT.xlsx";

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
            try
            {
                string srcFileName = "SPOT_TEMPLATE.xlsx";
                string destFileName = "SPOT_"+ Comp + "_" + Utility.EnMonthAbbr(Utility.ToInt(MM1)) + YY1 + ".xlsx";
                string srcFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + srcFileName);
                string destFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + "Export/" + destFileName);

                FileInfo srcFile = new FileInfo(srcFullFileName);
                FileInfo destFile = new FileInfo(destFullFileName);
                ExcelPackage p = new ExcelPackage(srcFile);
                ExcelWorkbook myWorkbook = p.Workbook;
                //--Should return the sheet name
                ExcelWorksheet myWorksheet = myWorkbook.Worksheets.FirstOrDefault();
                ExcelRange cell;

                cell = myWorksheet.Cells["B1"]; //-- report name
                cell.Value = Project.SpotName(Comp) + " ( " + Utility.EnMonth(Utility.ToInt(MM1)) + " " + YY1 + " - " + Utility.EnMonth(Utility.ToInt(MM2)) + " " + YY2 +" )";


                //===== FID header =====================================================
                if (TmplateList != "") //3,2,1,5
                {

                    string[] tid = TmplateList.Split(',');
                    for (int i = 0; i < tid.Length; i++)
                    {
                        if (tid[i] != "")
                        {
                            ORDER += tid[i] + "," + i + ",";  //1, 0,
                        }
                    }
                    ORDER = Utility.Left(ORDER, ORDER.Length - 1);

                    SQL = " SELECT T.TID, T.T_NAME,D.FID, D.SEQ " +
                    " FROM O_RPT_FID_TEMPLATE T INNER JOIN O_RPT_FID_DETAIL D ON T.TID = D.TID " +
                    " WHERE T.TID IN (" + TmplateList + ") " +
                    " ORDER BY DECODE(T.TID, " + ORDER + " ), D.SEQ ";
                }
                else
                {
                    string[] sid = FIDList.Split(',');
                    for (int i = 0; i < sid.Length; i++)
                    {
                        if (sid[i] != "")
                        {
                            ORDER += "'" +sid[i] + "'," + i + ",";  //4, 0,
                        }
                    }
                    ORDER = Utility.Left(ORDER, ORDER.Length - 1);

                    FIDList = "'" + FIDList.Replace(",", "','") + "'";

                    //SQL = " SELECT DISTINCT '' AS T_NAME, FID " +
                    // " FROM O_RPT_FID_DETAIL " +
                    // " WHERE replace(replace(FID,'#',''),'\"','')  IN (" + FIDList + ") " +
                    // " ORDER BY DECODE(replace(replace(FID,'#',''),'\"',''), " + ORDER + " ) ";

                    SQL = " SELECT DISTINCT '' AS T_NAME, FID " +
                       " FROM (SELECT H2S_NAME AS FID FROM O_DIM_H2S " +
                      " UNION SELECT HC_NAME AS FID FROM O_DIM_HC " +
                      " UNION SELECT HG_NAME AS FID FROM O_DIM_HG " +
                      " UNION SELECT O2_NAME AS FID FROM O_DIM_O2) A " +
                     " WHERE replace(replace(FID,'#',''),'\"','')  IN (" + FIDList + ") " +
                     " ORDER BY DECODE(replace(replace(FID,'#',''),'\"',''), " + ORDER + " ) ";

                }

                FIDList = "";
                string TName = "";
                int col = 2; //start column 
                int cntH = 0; //count header
                int cntSITE = 0; //count site id
                string bgColor = "#afd6ff"; //ฟ้า
                DTH = Project.dal.QueryData(SQL);
                foreach (DataRow DRH in DTH.Rows)
                {
                    FIDList += Utility.ToString(DRH["FID"]).Replace("#", "").Replace("\"","") + ","; //-- เก็บ site_id เพื่อไป query daily

                    if (TName != Utility.ToString(DRH["T_NAME"]))
                    {
                        TName = Utility.ToString(DRH["T_NAME"]);
                        cell = myWorksheet.Cells[2, col]; cell.Value = TName;

                        bgColor = BgColor(cntH);
                        cntH++;
                    }

                    cell = myWorksheet.Cells[3, col]; cell.Value = Utility.ToString(DRH["FID"]);

                    //set the background color of a cell or range of cells 
                    myWorksheet.Cells[2, col, 3, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    myWorksheet.Cells[2, col, 3, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(bgColor));

                    col++; cntSITE++;
                }


                //===== data =====================================================
                //วันที่สิ้นเดือนของเดือนที่เลือก
                string fromDate = "01/" + MM1.PadLeft(2, '0') + "/" + YY1;
                string tmpDate = "01/" + MM2.PadLeft(2, '0') + "/" + YY2;
                string toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(tmpDate)).AddMonths(1).AddDays(-1));

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

                int rw = 4; //strar row
                if (FIDList != "") //4,6,7,2,3,5,
                {
                    FIDList = Utility.Left(FIDList, FIDList.Length - 1);

                    string[] sid = FIDList.Split(',');
                    cntH = sid.Length;
                    for (int i = 0; i < sid.Length; i++)
                    {
                        if (sid[i] != "")
                        {
                            CRI += " MAX(DECODE(replace(replace(" + TabName + "_NAME,'#',''),'\"',''), '" + sid[i] + "'," + Comp + ")) F" + i + ",";
                        }
                    }
                    CRI = Utility.Left(CRI, CRI.Length - 1);

                    FIDList = "'" + FIDList.Replace(",", "','") + "'";

                    SQL = "SELECT TO_CHAR(SDATE,'YYYY') YY, TO_CHAR(SDATE,'MM') MM, " +
                       " " + CRI +
                       " FROM O_OGC_" + TabName + "  " +
                       " WHERE SDATE>= TO_DATE('" + fromDate + "', 'DD/MM/YYYY') AND SDATE<= TO_DATE('" + toDate + "', 'DD/MM/YYYY')   " +
                       " AND replace(replace(" + TabName + "_NAME,'#',''),'\"','') IN (" + FIDList + ")  " +
                       " GROUP BY TO_CHAR(SDATE,'YYYY'), TO_CHAR(SDATE,'MM') " +
                       " ORDER BY TO_CHAR(SDATE,'YYYY'), TO_CHAR(SDATE,'MM')  ";


                    DT = Project.dal.QueryData(SQL);
                    foreach (DataRow DR in DT.Rows)
                    {
                        col = 1;
                        cell = myWorksheet.Cells[rw, col];
                        cell.Value = Utility.EnMonthAbbr(Utility.ToInt(DR["MM"])) + " " + Utility.ToString(DR["YY"]); //-- Jan 2019

                        for (int f = 0; f < cntSITE; f++)
                        {
                            col++;
                            WriteValue(ref myWorksheet, rw, col, DR["F" + f]);
                        }

                        rw++;
                    }

                }

                //-- delete day row 
                if (rw < 65)
                {
                    myWorksheet.DeleteRow(rw, 65 - (rw), true);   // Should Delete multi rows and shift up the rows after deletion
                }
                //-- delete column
                if (cntSITE + 1 < 93)
                {
                    myWorksheet.DeleteColumn(cntSITE + 2, 93 - (cntSITE + 1));
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


        private string BgColor(int indx)
        {
            String bgColor = "";
            try
            {
                switch (indx % 10)
                {
                    case 0: bgColor = "#afd6ff"; break; //ฟ้า
                    case 1: bgColor = "#60ff7a"; break; //เขียว
                    case 2: bgColor = "#ffefaa"; break; //ส้ม
                    case 3: bgColor = "#ffa393"; break; //แดง
                    case 4: bgColor = "#ff99e3"; break; //ม่วง
                    case 5: bgColor = "#a6a3ff"; break; //น้ำเงิน
                    case 6: bgColor = "#ffc27a"; break; //ส้ม
                    case 7: bgColor = "#fffc68"; break; //เหลือง
                    case 8: bgColor = "#9cfcb9"; break; //เขียว
                    case 9: bgColor = "#ffa8ef"; break; //ปานเย็น
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