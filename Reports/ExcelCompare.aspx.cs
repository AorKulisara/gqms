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
    //-- edit 21/08/2018 --
    //-- edit 28/06/2019 -- เปลี่ยนเงื่อนไขจาก month เป็น date from-to
    public partial class ExcelCompare : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String SiteIDList = "", TmplateList="", Comp = "", MM = "", YY = "";
        String DateF = "", DateT = "";

        public String rOpenFile = "BLANK.xlsx";

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
                    //-- edit 28/06/2019 --
                    DateF = Validation.GetParamStr("DF", IsEncoded: false);
                    DateT = Validation.GetParamStr("DT", IsEncoded: false);

                    // if ( (TmplateList + SiteIDList != "") && Comp != "" && MM != "" && YY != "")
                    if ((TmplateList + SiteIDList != "") && Comp != "" && DateF != "" && DateT != "")
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
                string srcFileName = "COMPARE_TEMPLATE.xlsx";
                string destFileName = Comp + "_" + Utility.EnMonthAbbr(Utility.ToInt(MM)) + YY + ".xlsx";
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
                //cell.Value = Project.CompositionName(Comp) + " Summary ( " + Utility.EnMonth(Utility.ToInt(MM)) + " " + YY + " )";
                cell.Value = Project.CompositionName(Comp) + " Summary ( " + DateF +" - " + DateT +" )";


                //===== FID header =====================================================
                if ( TmplateList != "" ) //3,2,1,5
                {
                    //SELECT T.TID, T.T_NAME,D.FID, D.SEQ
                    //FROM O_RPT_FID_TEMPLATE T INNER JOIN O_RPT_FID_DETAIL D ON T.TID = D.TID
                    //WHERE T.TID IN (1, 3, 2, 5)
                    //ORDER BY DECODE(T.TID, 1, 0, 3, 1, 2, 2, 5, 3), D.SEQ

                    string[] tid = TmplateList.Split(',');
                    for (int i = 0; i < tid.Length; i++)
                    {
                        if (tid[i] != "")
                        {
                            ORDER += tid[i] + "," + i + ",";  //1, 0,
                        }
                    }
                    ORDER = Utility.Left(ORDER, ORDER.Length - 1);

                    SQL = " SELECT T.TID, T.T_NAME,D.FID, D.SEQ, S.SITE_ID " +
                    " FROM O_RPT_FID_TEMPLATE T INNER JOIN O_RPT_FID_DETAIL D ON T.TID = D.TID " +
                    "      INNER JOIN O_SITE_FID S ON D.FID=S.FID " +
                    " WHERE T.TID IN (" + TmplateList + ") " +
                    " ORDER BY DECODE(T.TID, " + ORDER + " ), D.SEQ ";
                }
                else
                {
                    //SELECT '' T_NAME, FID
                    //FROM O_SITE_FID
                    //WHERE SITE_ID IN (4, 6, 7, 2)
                    //ORDER BY DECODE(SITE_ID, 4, 0, 6, 1, 7, 2, 2, 3)

                    string[] sid = SiteIDList.Split(',');
                    for (int i = 0; i < sid.Length; i++)
                    {
                        if (sid[i] != "")
                        {
                            ORDER += sid[i] + "," + i + ",";  //4, 0,
                        }
                    }
                    ORDER = Utility.Left(ORDER, ORDER.Length - 1);

                    SQL = " SELECT '' T_NAME, FID, SITE_ID FROM O_SITE_FID " +
                    " WHERE SITE_ID IN (" + SiteIDList +") " +
                    " ORDER BY DECODE(SITE_ID, " + ORDER +" ) ";

                }

                SiteIDList = "";
                string TName = "";
                int col = 2; //start column 
                int cntH = 0; //count header
                int cntSITE = 0; //count site id
                string bgColor = "#afd6ff"; //ฟ้า
                DTH = Project.dal.QueryData(SQL);
                foreach (DataRow DRH in DTH.Rows)
                {
                    SiteIDList += Utility.ToString(DRH["SITE_ID"])+","; //-- เก็บ site_id เพื่อไป query daily

                    if ( TName != Utility.ToString(DRH["T_NAME"]) )
                    {
                        TName = Utility.ToString(DRH["T_NAME"]);
                        cell = myWorksheet.Cells[2, col]; cell.Value = TName;

                        bgColor = BgColor(cntH);
                        cntH++;
                    }

                    cell = myWorksheet.Cells[3, col]; cell.Value = Utility.ToString(DRH["FID"]);
 
                    //set the background color of a cell or range of cells 
                    myWorksheet.Cells[2,col,3,col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    myWorksheet.Cells[2, col, 3, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml(bgColor));

                    col++; cntSITE++;
                }


                //===== daily =====================================================
                //วันที่สิ้นเดือนของเดือนที่เลือก
                //string fromDate = "01/" + MM.PadLeft(2, '0') + "/" + YY;
                //string toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));
                //int monthDAY = Utility.DateDiff(Convert.ToDateTime(Utility.AppDateValue(fromDate)), Convert.ToDateTime(Utility.AppDateValue(toDate)))+1;

                string fromDate = DateF;
                string toDate = DateT;

                int rw = 4; //strar row
                if (SiteIDList != "") //4,6,7,2,3,5,
                {
                    SiteIDList = Utility.Left(SiteIDList, SiteIDList.Length - 1);
                    //  SELECT ADATE, A.*
                    // FROM O_DIM_DATE DY LEFT OUTER JOIN
                    // ( SELECT D.RDATE, 
                    // MAX(DECODE(SITE_ID,12,C2)) F0,MAX(DECODE(SITE_ID,4,C2)) F1,
                    // MAX(DECODE(SITE_ID,6,C2)) F2,MAX(DECODE(SITE_ID,7,C2)) F3
                    // FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID=S.FID
                    // WHERE RDATE>=TO_DATE('01/08/2018','DD/MM/YYYY') AND RDATE<=TO_DATE('31/08/2018','DD/MM/YYYY')
                    // AND SITE_ID IN (12, 4, 6, 7)
                    // GROUP BY D.RDATE ) A ON DY.ADATE = A.RDATE
                    // WHERE DY.ADATE>=TO_DATE('01/08/2018','DD/MM/YYYY') AND DY.ADATE<=TO_DATE('31/08/2018','DD/MM/YYYY')
                    // ORDER BY DY.ADATE

                    string[] sid = SiteIDList.Split(',');
                    cntH = sid.Length;
                    for (int i = 0; i < sid.Length; i++)
                    {
                        if (sid[i] != "")
                        {
                            CRI += " MAX(DECODE(SITE_ID, " + sid[i] + ","+ Comp +")) F" + i + ",";
                        }
                    }
                    CRI = Utility.Left(CRI, CRI.Length - 1);


                    SQL = "  SELECT ADATE, A.* " +
                    " FROM O_DIM_DATE DY LEFT OUTER JOIN " +
                    " ( SELECT D.RDATE,  " + CRI +
                    " FROM GQMS_DAILY_UPDATE D INNER JOIN O_SITE_FID S ON D.FID=S.FID " +
                    " WHERE RDATE>=TO_DATE('" + fromDate + "','DD/MM/YYYY') AND RDATE<=TO_DATE('" + toDate + "','DD/MM/YYYY') " +
                    " AND SITE_ID IN (" + SiteIDList + ") " +
                    " GROUP BY D.RDATE ) A ON DY.ADATE = A.RDATE " +
                    " WHERE DY.ADATE>=TO_DATE('" + fromDate + "','DD/MM/YYYY') AND DY.ADATE<=TO_DATE('" + toDate + "','DD/MM/YYYY') " +
                    " ORDER BY DY.ADATE ";


                    DT = Project.dal.QueryData(SQL);
                    foreach (DataRow DR in DT.Rows )
                    {
                        col = 1; 
                        cell = myWorksheet.Cells[rw,col];
                        //cell.Value = Convert.ToDateTime(DR["ADATE"]).Day ; //--Day
                        cell.Value = Utility.AppFormatDate(DR["ADATE"]);

                        for (int f = 0; f < cntSITE; f++)
                        {
                            col++;
                            WriteValue(ref myWorksheet, rw, col, DR["F" + f]);
                        }

                        rw++;
                    }

                }

                //-- delete day row 
                //if (monthDAY+3 < 34)

                if (rw < 103) //นับตาม row ใน excel
                {
                    //myWorksheet.DeleteRow(monthDAY + 4, 34-(monthDAY + 3), true);   // Should Delete multi rows and shift up the rows after deletion
                    myWorksheet.DeleteRow(rw, 104 - (rw), true);
                }
                //-- delete column
                if (cntSITE + 1 < 93)
                {
                    myWorksheet.DeleteColumn(cntSITE + 2, 93 - (cntSITE + 1));
                }


                MM = fromDate.Split('/')[1].PadLeft(2, '0');
                myWorksheet.Name = Utility.EnMonthAbbr(Utility.ToInt(MM)); //-- กำหนดชื่อ worksheet เป็นเดือน
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
        private void WriteValue(ref ExcelWorksheet WS,  int sRow, int sCol, Object gDR)
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