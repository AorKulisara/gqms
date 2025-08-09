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
    //-- edit 20/08/2018 --

    public partial class ExcelMonthly : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String SiteID = "", FID = "", RptType = "", MM = "", YY = "";
        String fromDate = "", toDate = "";
        DateTime DateFROM, DateTO;

        String gH2S_FLAG = "";  // site นี้มี H2S
        String gISO_FLAG = "";  // site นี้รองรับ ISO

        string rName = "", rNo = "", rPeriod = "";
        string rRemark = "", rISO = "", rRemarkAdd = "";
        string rReportByName = "", rReportBySign = "", rApproveByName = "", rApproveBySign = "";
        //-- EDIT 22/07/2019
        Decimal gorderYMD = 0, gexpireYMD = 0;
        string rSigned = "", rISOMINMAX = "";
        Double gC1 = -999, gC2 = -999, gC3 = -999, gIC4 = -999, gNC4 = -999;
        Double gIC5 = -999, gNC5 = -999, gC6 = -999, gN2 = -999, gCO2 = -999, gH2S = -999, gHG = -999;
        Double gC1_MIN = -999, gC2_MIN = -999, gC3_MIN = -999, gIC4_MIN = -999, gNC4_MIN = -999;
        Double gIC5_MIN = -999, gNC5_MIN = -999, gC6_MIN = -999, gN2_MIN = -999, gCO2_MIN = -999, gH2S_MIN = -999, gHG_MIN = -999;
        Double gC1_MAX = -999, gC2_MAX = -999, gC3_MAX = -999, gIC4_MAX = -999, gNC4_MAX = -999;
        Double gIC5_MAX = -999, gNC5_MAX = -999, gC6_MAX = -999, gN2_MAX = -999, gCO2_MAX = -999, gH2S_MAX = -999, gHG_MAX = -999;
        string gALERT1_COLUMN = "", gALERT2_COLUMN = ""; //,B,C,D,
        String gISO_ACCREDIT = "";  //ISO Alert
        String gISO_MINMAX = "";  //ISO MIN/MAX Alert
        String gChromatDate = "";
        String gOgcName = ""; //-- edit 23/04/2021 --

        public String rOpenFile = "BLANK.xlsx";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!this.IsPostBack)
                {
                    SiteID = Validation.GetParamStr("K", IsEncoded: false);
                    FID = Validation.GetParamStr("F", IsEncoded: false);
                    RptType = Validation.GetParamStr("T", IsEncoded: false);
                    MM = Validation.GetParamStr("MM", IsEncoded: false);
                    YY = Validation.GetParamStr("YY", IsEncoded: false);

                    if ( SiteID != "" && FID != "" && RptType != "" && MM != "" && YY != "")
                    {
                        LoadData();
                    }

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }


        private void LoadData()
        {

            
            try
            {
               
                string srcFileName = "MONTHLY_TEMPLATE.xlsx";
                //-- edit 22/07/2019 excel file มีหลาย worksheet ตามชื่อเดือน
                //string destFileName = FID + Utility.FormatDate(DateTO,"_MON_YYYY") +"("+ RptType+")" + ".xlsx";
                string destFileName = FID + "_"+ YY + "(" + RptType + ")" + ".xlsx";
                destFileName = destFileName.Replace("#", "");


                string srcFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + srcFileName);
                string destFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + "Export/" + destFileName);

                FileInfo srcFile = new FileInfo(srcFullFileName);
                FileInfo destFile = new FileInfo(destFullFileName);
                ExcelPackage p = new ExcelPackage(srcFile);
                ExcelWorkbook myWorkbook = p.Workbook;

                for (int m=1; m<=Utility.ToInt(MM); m++) //-- write data ตั้งแต่เดือน 1 ถึงเดือนที่เลือก
                {
                    WriteXSL(ref myWorkbook, m);
                }

                //-- กำหนด tab default 
                myWorkbook.Worksheets[Utility.EnMonthAbbr(Utility.ToInt(MM))].Select();


                p.SaveAs(destFile);

                rOpenFile = destFileName;

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
     
            }

        }


        private void WriteXSL(ref ExcelWorkbook myWorkbook, int m)
        {
            DataTable DTH = null;
            DataRow DRH = null;
            DataTable DTF = null;
            DataRow DRF = null;
            String anlmetID = ""; //-- EDIT 28/06/2023 -- เพิ่ม Analysis Method 
            try
            {
                gALERT1_COLUMN = ""; gALERT2_COLUMN = "";
                gChromatDate = ""; 

                rRemark = ""; rRemarkAdd = ""; rISO = ""; rReportByName = ""; rReportBySign = "";
                rApproveByName = ""; rApproveBySign = ""; rISOMINMAX = ""; rSigned = "";


                //--Should return the sheet name
                //ExcelWorksheet myWorksheet = myWorkbook.Worksheets.FirstOrDefault();
                string sheetName = Utility.EnMonthAbbr(m);    //-- กำหนดชื่อ worksheet เป็นเดือน

                ExcelWorksheet myWorksheet = myWorkbook.Worksheets[sheetName];
                ExcelRange cell;
                
                string mm = Utility.ToString(m).PadLeft(2, '0');

                //-- พิจารณา report type เพื่อแสดงวันที่เริ่ม-สิ้นสุด
                switch (RptType)
                {
                    case "20DAY":
                        // ตั้งแต่ 21 ของเดือนที่แล้ว ถึง 20 ของเดือนที่เลือก
                        toDate = "20/" + mm + "/" + YY;
                        DateTO = Convert.ToDateTime(Utility.AppDateValue(toDate));
                        fromDate = Utility.AppFormatDate(DateTO.AddMonths(-1).AddDays(1));   // 21 ของเดือนที่แล้ว  
                        DateFROM = Convert.ToDateTime(Utility.AppDateValue(fromDate));
                        //December 21,2017-January 20,2018
                        //January 21-Febuary 20,2018
                        if (DateTO.Month == 1)
                            rPeriod = Utility.FormatDate(DateFROM, "MONTH DD,YYYY") + "-" + Utility.FormatDate(DateTO, "MONTH DD,YYYY");
                        else
                            rPeriod = Utility.FormatDate(DateFROM, "MONTH DD") + "-" + Utility.FormatDate(DateTO, "MONTH DD,YYYY");
                        break;
                    case "27DAY":
                        // ตั้งแต่ 28 ของเดือนที่แล้ว ถึง 27 ของเดือนที่เลือก
                        toDate = "27/" + mm + "/" + YY;
                        DateTO = Convert.ToDateTime(Utility.AppDateValue(toDate));
                        fromDate = Utility.AppFormatDate(DateTO.AddMonths(-1).AddDays(1));   // 28 ของเดือนที่แล้ว 
                        DateFROM = Convert.ToDateTime(Utility.AppDateValue(fromDate));
                        //December 28,2017-January 27,2018
                        //January 28-Febuary 27,2018
                        if (DateTO.Month == 1)
                            rPeriod = Utility.FormatDate(DateFROM, "MONTH DD,YYYY") + "-" + Utility.FormatDate(DateTO, "MONTH DD,YYYY");
                        else
                            rPeriod = Utility.FormatDate(DateFROM, "MONTH DD") + "-" + Utility.FormatDate(DateTO, "MONTH DD,YYYY");
                        break;
                    case "ENDMTH":
                        //วันที่สิ้นเดือนของเดือนที่เลือก
                        fromDate = "01/" + mm + "/" + YY;
                        toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));
                        DateTO = Convert.ToDateTime(Utility.AppDateValue(toDate));
                        DateFROM = Convert.ToDateTime(Utility.AppDateValue(fromDate));
                        //January 1-31,2018
                        rPeriod = Utility.FormatDate(DateTO, "MONTH 1-DD,YYYY");
                        break;
                }

                if (RptType == "20DAY")
                    DTH = Project.dal.SearchSiteReport("", SiteID, "", "ENDMTH");
                else
                    DTH = Project.dal.SearchSiteReport("", SiteID, "", RptType);
                if (DTH == null || DTH.Rows.Count == 0) //-- กรณีที่หาไม่เจอให้เอาข้อมูลของ period อื่นมาใส่แทน
                {
                    DTH = Project.dal.SearchSiteReport("", SiteID);
                }
                if (DTH != null && DTH.Rows.Count > 0)
                {
                    DRH = Utility.GetDR(ref DTH);

                    rName = Utility.ToString(DRH["GC_RPT_NAME"]);

                    if (RptType == "20DAY")
                        rNo = ""; //-- กรณี 20 วัน ไม่ต้องใส่ Report Number
                    else
                        rNo = Utility.ToString(DRH["GC_RPT_NO"]);

                    FID = Utility.ToString(DRH["FID"]); //-- กำหนด FID อีกครั้ง เนื่องจากบางที FID ที่ส่งมาอาจจะมี special char ที่หายไป
                }

                //--- EDIT 26/06/2023 ----------------
                //เพิ่ม Analyzer Area:  สัมพันธ์กับ Region 
                DataTable DTH2 = null;
                DataRow DRH2 = null;
                DTH2 = Project.dal.SearchSiteFID(SiteID);
                if (DTH2 != null && DTH2.Rows.Count > 0)
                {
                    DRH2 = Utility.GetDR(ref DTH2);
                    cell = myWorksheet.Cells["D4"]; //--  Analyzer Area: 
                    cell.Value = Utility.ToString(DRH2["REGION_FULL"]) + "  " + Utility.ToString(DRH2["REGION_ADDR"]);

                    anlmetID = Utility.ToString(DRH2["ANLMET_ID"]); //-- EDIT 28/06/2023 -- เพิ่ม Analysis Method 
                }
                Utility.ClearObject(ref DTH2);

                //-- iso ----------------------
                gC1 = -999; gC2 = -999; gC3 = -999; gIC4 = -999; gNC4 = -999;
                gIC5 = -999; gNC5 = -999; gC6 = -999; gN2 = -999; gCO2 = -999; gH2S = -999; gHG = -999;
                gC1_MIN = -999; gC2_MIN = -999; gC3_MIN = -999; gIC4_MIN = -999; gNC4_MIN = -999;
                gIC5_MIN = -999; gNC5_MIN = -999; gC6_MIN = -999; gN2_MIN = -999; gCO2_MIN = -999; gH2S_MIN = -999; gHG_MIN = -999;
                gC1_MAX = -999; gC2_MAX = -999; gC3_MAX = -999; gIC4_MAX = -999; gNC4_MAX = -999;
                gIC5_MAX = -999; gNC5_MAX = -999; gC6_MAX = -999; gN2_MAX = -999; gCO2_MAX = -999; gH2S_MAX = -999; gHG_MAX = -999;
                GetISO(SiteID, mm, YY);

                //-- report name 
                cell = myWorksheet.Cells["A6"]; //myWorksheet.Cells["A5"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                cell.Value = rName;
                //-- report number
                cell = myWorksheet.Cells["D7"]; //myWorksheet.Cells["D6"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                cell.Value = rNo;
                //-- report period
                cell = myWorksheet.Cells["Q7"]; //myWorksheet.Cells["Q6"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                cell.Value = rPeriod;
               


                //===== daily =====================================================
                //ดูว่าวันไหนไม่ต้องแสดง
                string NoShow = "";
                DataTable DTn = Project.dal.SearchRptMonthlyDate(FID, fromDate, toDate, OtherCriteria: "SHOW_FLAG='N'");
                foreach (DataRow DRn in DTn.Rows)
                {
                    NoShow += "," + Utility.AppFormatDate(DRn["RDATE"]) + ",";
                }

                //DataTable DT = (DataTable)Session[SiteID + "_DAILY"];
                DataTable DT = Project.dal.SearchGqmsDailyUpdateReport(SiteID, FID, fromDate, toDate);
                int rw = 11; //10; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                int cntRow = 0;
                string rDate = "";


                if (DT != null && DT.Rows.Count > 0)
                {
                    gH2S_FLAG = Utility.ToString(DT.Rows[0]["H2S_FLAG"]);
                    gISO_FLAG = Utility.ToString(DT.Rows[0]["ISO_FLAG"]);
                }
                //===== image =====================================================
                //-- ISO 30/08/2018 กรณีที่เป็น Site ISO ให้แสดงรูปภาพด้วย  ไม่ต้องพิจารณาว่าตก ISO หรือไม่
                if (gISO_FLAG == "Y")
                {
                    try
                    {
                        gISO_ACCREDIT = "Y"; gISO_MINMAX = "Y";

                        string imageFile = HttpContext.Current.Server.MapPath("../dat/Signature/ISO17025.jpg");  //-- EDIT 14/06/2023 --ISO.jpg
                        OfficeOpenXml.Drawing.ExcelPicture PicISO = myWorksheet.Drawings.AddPicture("ISO", System.Drawing.Image.FromFile(imageFile));
                        if (gH2S_FLAG == "Y")
                            PicISO.SetPosition(1, 2, 18, 2);
                        else
                            PicISO.SetPosition(1, 2, 17, 2);

                        //PicISO.SetSize(100); // ต้องกำหนด setsize หลัง setposition
                        PicISO.SetSize(12); //-- EDIT 14/06/2023
 
                    }
                    catch (Exception ex)
                    {
                    }
                }


                foreach (DataRow DR in DT.Rows)
                {
                    gH2S_FLAG = Utility.ToString(DT.Rows[0]["H2S_FLAG"]);

                    cell = myWorksheet.Cells["A" + rw]; cell.Value = Utility.ToInt(Utility.FormatDate(Convert.ToDateTime(DR["ADATE"]), "DD")); //--Day
                    //-- ตรวจสอบว่าถ้าไม่ show ให้แสดงสีเทา
                    rDate = Utility.AppFormatDate(DR["ADATE"]);
                    if (NoShow.IndexOf("," + rDate + ",") > -1)
                    {
                        //set the background color of a cell or range of cells 
                        myWorksheet.Cells["A" + rw + ":S" + rw].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        myWorksheet.Cells["A" + rw + ":S" + rw].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#CCCCCC"));
                    }
                    else
                    {
                        //-- edit 23/04/2021 --
                        gOgcName = Utility.ToString(DR["OGC_NAME"]);

                        WriteValue(ref myWorksheet, "B", rw, 2, cntRow, DR["C1"]);   //--CH4	
                        WriteValue(ref myWorksheet, "C", rw, 3, cntRow, DR["C2"]);   //--C2H6
                        WriteValue(ref myWorksheet, "D", rw, 4, cntRow, DR["C3"]);   //--C3H8
                        WriteValue(ref myWorksheet, "E", rw, 5, cntRow, DR["IC4"]);  //--i-C4H10
                        WriteValue(ref myWorksheet, "F", rw, 6, cntRow, DR["NC4"]);  //--n-C4H10
                        WriteValue(ref myWorksheet, "G", rw, 7, cntRow, DR["IC5"]);  //--i-C5H12
                        WriteValue(ref myWorksheet, "H", rw, 8, cntRow, DR["NC5"]);  //--n-C5H12
                        WriteValue(ref myWorksheet, "I", rw, 9, cntRow, DR["C6"]);   //--n-C5H12
                        WriteValue(ref myWorksheet, "J", rw, 10, cntRow, DR["CO2"]);  //--CO2
                        WriteValue(ref myWorksheet, "K", rw, 11, cntRow, DR["N2"]);  //--N2
                        WriteValue(ref myWorksheet, "L", rw, 12, cntRow, DR["H2S"]);  //--H2S
                        WriteValue(ref myWorksheet, "M", rw, 13, cntRow, DR["NHV"]);  //--netHVdry
                        WriteValue(ref myWorksheet, "N", rw, 14, cntRow, DR["GHV"]);  //--Hvsat
                        WriteValue(ref myWorksheet, "O", rw, 15, cntRow, DR["SG"]);   //--SG
                        WriteValue(ref myWorksheet, "P", rw, 16, cntRow, DR["WC"], Utility.ToString(DR["WC_ALERT"]));   //--H2O
                        WriteValue(ref myWorksheet, "Q", rw, 17, cntRow, DR["UNNORMMIN"]);  //--unnormmin
                        WriteValue(ref myWorksheet, "R", rw, 18, cntRow, DR["UNNORMMAX"]);  //--unnormmax
                        //--30/08/2018  เก็บค่า TOLERANCE_RUN เป็นตัวเลข เช่น 30 หมายถึง ยอมรับได้ในช่วง +-30
                        Double minRUN = Utility.ToDouble(DR["TOTAL_RUN"]) - Utility.ToDouble(DR["TOLERANCE_RUN"]);
                        WriteValue(ref myWorksheet, "S", rw, 19, cntRow, DR["RUN"], "", Utility.ToString(minRUN), Utility.FormatDate(Convert.ToDateTime(DR["ADATE"]), "D MONTH YYYY"));  //--chromat  Utility.FormatDate(Convert.ToDateTime(gDR["ADATE"]), "D MONTH YYYY"); //gDR["ADATE"]
                    }

                    rw++;
                    cntRow++;
                }

                //--- กำหนด Header -----------------------------------
                //-- ตรวจ alert จาก gALERT1_COLUMN เก็บแยกแต่ละคอลัมน์ เช่น ,B,C,D,
                string alphabet = "BCDEFGHIJKLMNOPQRS";
                foreach (char c in alphabet)
                {
                    //if (gALERT1_COLUMN.IndexOf("," + c + ",") > -1)
                    //{
                    //    cell = myWorksheet.Cells[c + "8"]; //-- table header
                    //    cell.Value = cell.Value + "[1]";
                    //}
                    //if (gALERT2_COLUMN.IndexOf("," + c + ",") > -1)
                    //{
                    //    cell = myWorksheet.Cells[c + "8"]; //-- table header
                    //    cell.Value = cell.Value + "[2]";
                    //}
                    //-- edit 08/07/2020 ---
                    //- ปรับการตรวจสอบข้อมูล สมอ.และ standard gas ให้เป็นหัวข้อเดียวกัน				
                    //- Auto Remarks ให้แสดง[1] "out of TISI Accreditation Scope for our Laboratory "
                    if (gALERT1_COLUMN.IndexOf("," + c + ",") > -1  || gALERT2_COLUMN.IndexOf("," + c + ",") > -1)
                    {   //-- table header
                        cell = myWorksheet.Cells[c + "9"]; //myWorksheet.Cells[c + "8"];  //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                        cell.Value = cell.Value + "[1]";
                    }
                    //-- EDIT 26/06/2023 --
                    //เพิ่มวงเล็บ [1] ต่อท้าย Unnorm Min // Unnorm Max // Chromat run  //AVG
                    //-- EDIT 22/09/2023 -- ต้องเป็น FID ที่รองรับ ISO ด้วยเท่านั้น
                    if ("QRS".Contains(c) && gISO_FLAG =="Y")
                    {
                        cell = myWorksheet.Cells[c + "9"]; 
                        cell.Value = cell.Value + "[1]";
                    }
                }
                //-- EDIT 26/06/2023 --
                //--- กำหนด AVG -----------------------------------
                //เพิ่มวงเล็บ [1] ต่อท้าย Unnorm Min // Unnorm Max // Chromat run  //AVG
                //-- EDIT 22/09/2023 -- ต้องเป็น FID ที่รองรับ ISO ด้วยเท่านั้น
                if (gISO_FLAG == "Y")
                {
                    cell = myWorksheet.Cells["A42"];
                    cell.Value = cell.Value + "[1]";
                }


                //------ ส่วนใต้ตาราง  ก่อนส่วน Remark -------------------------------------------------------------
                //-- EDIT 28/06/2023 -- เพิ่ม Analysis Method 
                if (anlmetID != "")
                {
                    
                    DataTable DTA = Project.dal.SearchDimAnalysisItem(anlmetID);
                    if (DTA != null && DTA.Rows.Count > 0)
                    {
                        String std = ""; int itm = 0;
                        String anlCol = "A"; int anlRow = 43; //-- analysis row
                        decimal  cntItemHalf = Math.Round(Utility.ToNum( DTA.Rows.Count) / 2);
                        foreach (DataRow DRA in DTA.Rows )
                        {
                            std = Utility.ToString(DRA["STD_HEAD"]) + " " + Utility.ToString(DRA["STD_REF"]);
                            cell = myWorksheet.Cells[anlCol + anlRow];  //เริ่มที่ Cells["A43"]
                            cell.Value = std;
                            anlRow++;
                            itm++;
                            if (itm>cntItemHalf)
                            {
                                anlCol = "M"; anlRow = 43;  //-- เขียนอีกฝั่ง
                                itm = 0;
                            }
                        }

                    }
                    Utility.ClearObject(ref DTA);
                }



                //------ ส่วน Remark -------------------------------------------------------------
                DTF = Project.dal.SearchRptMonthly("", FID, Utility.ToString(m), YY, RptType);
                if (DTF != null && DTF.Rows.Count > 0)
                {
                    DRF = Utility.GetDR(ref DTF);
                    rRemark = Utility.ToString(DRF["REMARK1"]);
                    rRemarkAdd = Utility.ToString(DRF["REMARK2"]);
                    rISO = Utility.ToString(DRF["ISO_ACCREDIT"]);
                    rReportByName = Utility.ToString(DRF["REPORT_BY_NAME"]);
                    rReportBySign = Utility.ToString(DRF["REPORT_BY_SIGN"]);
                    rApproveByName = Utility.ToString(DRF["APPROVE_BY_NAME"]);
                    rApproveBySign = Utility.ToString(DRF["APPROVE_BY_SIGN"]);

                    rISOMINMAX = Utility.ToString(DRF["ISO_MINMAX"]);
                    rSigned = Utility.ToString(DRF["SIGNED_FLAG"]);

                }
                else
                {

                    //-- ใส่หมายเหตุ ---------------------------------------------------------------------------------------------------
                    //--1. เพิ่มการแสดงหมายเหตุ กรณี Out of scope ดังนี้
                    //(1) Out of scope สมอ. (TISI)(ตรวจสอบจาก Min - Max ISO)
                    //(2) Out of scope Standard Gas
                    //-- 2. เพิ่มหมายเหตุ โดยแสดงวันที่ที่มี chromat run ต่ำกว่า 360 เดือนนั้นต้องมีวันเดียว ถ้ามีหลายวันก็ไม่ต้องแสดง   ระบุค่าของ H2S, Hg ต่อท้ายด้วย  ตัวอย่างเช่น
                    //1. OGC was calibrated on 23 July  2018  (use  time 120  mins.),  H2S = 3.41 ppmv,  Hg  =  0.033  ug/m3.
                    //1. OGC was calibrated on 19 January 2018(use  time 80 mins. ),  H2S[1] = 3.39  ppmv,  Hg[1] = 0.29    ug / m3.

                    //-- edit 08/07/2020 ---
                    //- ปรับการตรวจสอบข้อมูล สมอ.และ standard gas ให้เป็นหัวข้อเดียวกัน				
                    //- Auto Remarks ให้แสดง[1] "out of TISI Accreditation Scope for our Laboratory "
                    string remark1 = "", remark2 = "";
                    if (gISO_FLAG == "Y")
                    {
                        //if (gISO_MINMAX == "N") remark1 = " [1] \"Out of scope สมอ. (TISI)\" ";
                        //if (gISO_ACCREDIT == "N")
                        //{
                        //    if (remark1 != "") remark1 += " / ";
                        //    remark1 += " [2] \"Out of scope Standard Gas\" ";
                        //}
                        if (gISO_MINMAX == "N" || gISO_ACCREDIT == "N")
                            remark1 = " [1] \"Out of TISI Accreditation Scope for our Laboratory\" ";
                      
                        if (remark1 != "") remark1 = "Test Marked " + remark1 + "\r\n";
                    }

                    String h2s = "", hg = "";
                    String h2sFlag = "", hgFlag = "";
                    
                    String sDate = "01/" + mm + "/" + YY; //-- กำหนดเป็นวันที่ 1 ของเดือน
                    DataTable DTg = Project.dal.SearchSpotFID( FID, sDate);

                    if (DTg != null && DTg.Rows.Count > 0)
                    {
                        if (Utility.ToString(DTg.Rows[0]["H2S"]) != "")
                        {
                            h2s = Utility.FormatNum(DTg.Rows[0]["H2S"], 2);
                            ////Double dVal = Utility.ToDouble(h2s);
                            //////-- กรณีไม่ตรงมาตรฐานให้ขึ้นหมายเลข [1]
                            ////if ((gH2S_MIN != -999 && dVal < gH2S_MIN) || (gH2S_MAX != -999 && dVal > gH2S_MAX))  //เพิ่มตรวจสอบ Min/Max
                            ////{
                            ////    h2sFlag = "[1]";
                            ////}
                            ////-- edit 08/07/2020 -- ถ้า site นั้นมี ISO ให้กำหนด H2S เป็น [1] เสมอ 
                            //if (gISO_FLAG == "Y")
                            //{
                            //    h2sFlag = "[1]";
                            //}
                            //--edit 14/09/2020 -- ที่ remark H2S คือค่าที่ได้จากเครื่องวัดของ Lab ซึ่งได้มาตรฐานอยู่แล้ว จึงไม่เกี่ยวกับค่าในตาราง ไม่ต้องแสดง [1]ที่ remark
                            h2sFlag = "";
                        }
                        if (Utility.ToString(DTg.Rows[0]["HG"]) != "")
                        {
                            hg = Utility.FormatNum(DTg.Rows[0]["HG"], 3);
                            Double dVal = Utility.ToDouble(hg);
                            //-- กรณีไม่ตรงมาตรฐานให้ขึ้นหมายเลข [1]
                            if ((gHG_MIN != -999 && dVal < gHG_MIN) || (gHG_MAX != -999 && dVal > gHG_MAX))  //เพิ่มตรวจสอบ Min/Max
                            {
                                hgFlag = "[1]";
                            }
                        }

                        remark2 += "1. H2S" + h2sFlag + " = " + h2s + " ppmv,  Hg" + hgFlag + " = " + hg + " ug/m3." + "\r\n";

                    }

                    //-- edit 31/08/2020 -- ส่วน remark ให้แสดง OGC was calibrated on (date) ดึงวันที่ As Found จากระบบ OGC Data
                    string sql = "SELECT C.* , S.OSITE_ID " +
                        " FROM C_CALIBRATE C INNER JOIN C_SITE_FID S ON C.CSITE_ID = S.CSITE_ID" +
                        " WHERE C.WORK_TYPE IN('ML2','ML3') AND C.MM =" + m + " AND C.YY =" + YY + " AND S.OSITE_ID =" + SiteID +
                        " ORDER BY C.FOUND_DATE DESC ";
                    DataTable DTc = Project.dal.QueryData(sql);
                    if (DTc != null && DTc.Rows.Count > 0)
                    {
                        gChromatDate = Utility.FormatDate(Convert.ToDateTime(DTc.Rows[0]["FOUND_DATE"]), "D MONTH YYYY");
                    }
                    Utility.ClearObject(ref DTc);


                    if (gChromatDate != "" && gChromatDate != "MULTI-DATE")
                    {
                        if (remark2 == "")
                            remark2 += "1.";
                        else
                            remark2 += "2.";

                        remark2 += " OGC was calibrated on " + gChromatDate  + "\r\n";  //" ( use time 0 mins. ) "+  //edit--14/10/2020
                    }
                    //-- ใส่หมายเหตุ ---------------------------------------------------------------------------------------------------
                    rRemark = remark1 + remark2;
                }


                //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                //-- remarks ----------------------
                if (rRemark != "")
                {
                    string r = rRemark;
                    string t = "";
                    //-- remark line1
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["C48"]; //myWorksheet.Cells["C42"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area + analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line2
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["C49"]; //myWorksheet.Cells["C43"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line3
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["C50"];// myWorksheet.Cells["C44"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line4
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["C51"]; //myWorksheet.Cells["C45"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line5
                    cell = myWorksheet.Cells["C52"]; //myWorksheet.Cells["C46"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = r;

                }

                //-- remarks Addition 30/07/2019 ----------------------
                if (rRemarkAdd != "")
                {
                    string r = rRemarkAdd;
                    string t = "";
                    //-- remark line1
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["M48"]; //myWorksheet.Cells["M42"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t; //-- EDIT 18/09/2019 เปลี่ยนจาก L42 เป็น M42 เพราะตอนท้ายมีลบคอลัมน์ H2S = L
                    //-- remark line2
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["M49"]; //myWorksheet.Cells["M43"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line3
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["M50"]; //myWorksheet.Cells["M44"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line4
                    if (r.IndexOf("\r\n") > 0)
                    {
                        t = r.Substring(0, r.IndexOf("\r\n") + 2);
                        r = r.Substring(t.Length);
                    }
                    else
                    {
                        t = r;
                        r = "";
                    }
                    cell = myWorksheet.Cells["M51"]; //myWorksheet.Cells["M45"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = t;
                    //-- remark line5
                    cell = myWorksheet.Cells["M52"]; //myWorksheet.Cells["M46"]; //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                    cell.Value = r;

                }

                //-- reported by ----------------------
                //-- reported by
                cell = myWorksheet.Cells["D54"]; //myWorksheet.Cells["D48"];  //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                cell.Value = "( " + rReportByName + " )";
                //-- approved by -------------------------
                //-- approved by
                cell = myWorksheet.Cells["K54"]; //myWorksheet.Cells["K48"];  //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                cell.Value = "( " + rApproveByName + " ) ";

                //-- EDIT 28/06/2023 -- Analysis method มี 5 บรรทัด ----
                //-- ลบบรรทัดที่ไม่มีข้อความ  เริ่มที่ Cells["A43"] - Cells["A47"]
                int delRowCnt2 = 0;
                cell = myWorksheet.Cells["A47"];
                if (Utility.ToString(cell.Text) == "")
                {
                    myWorksheet.DeleteRow(47, 1, true); delRowCnt2++;
                }
                cell = myWorksheet.Cells["A46"];
                if (Utility.ToString(cell.Text) == "")
                {
                    myWorksheet.DeleteRow(46, 1, true); delRowCnt2++;
                }
                cell = myWorksheet.Cells["A45"];
                if (Utility.ToString(cell.Text) == "")
                {
                    myWorksheet.DeleteRow(45, 1, true); delRowCnt2++;
                }
                cell = myWorksheet.Cells["A44"];
                if (Utility.ToString(cell.Text) == "")
                {
                    myWorksheet.DeleteRow(44, 1, true); delRowCnt2++;
                }
                cell = myWorksheet.Cells["A43"];
                if (Utility.ToString(cell.Text) == "")
                {
                    myWorksheet.DeleteRow(43, 1, true); delRowCnt2++;
                }


                //-- delete day row กำหนด ทุกเดือนของ template เป็น 31 วัน เพราะบางทีเลือก report type=28-27 จะมี 31 วัน
                int delRowCnt = 0;
                if (rw < 42) //(rw < 41) //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                {
                    delRowCnt = (42 - rw); //(41 - rw); //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area
                    myWorksheet.DeleteRow(rw, delRowCnt, true);   // Should Delete multi rows and shift up the rows after deletion
                }

                //-- ซ่อนคอลัมน์ H2S --
                if (gH2S_FLAG != "Y") myWorksheet.DeleteColumn(12);

               

                //===== image =====================================================
                //-- Reported by sign
                if (rReportBySign != "" && rSigned == "Y")  //-- edit 22/07/2019 && rSigned == "Y"
                {
                    try
                    {
                        //-- EDIT 28/06/2023 -- Analysis method มี 5 บรรทัด ที่อาจลบ ----
                        delRowCnt = delRowCnt + delRowCnt2;

                        string imageFile1 = HttpContext.Current.Server.MapPath("../dat/Signature/" + rReportBySign);
                        OfficeOpenXml.Drawing.ExcelPicture PicSign1 = myWorksheet.Drawings.AddPicture("Signature 1", System.Drawing.Image.FromFile(imageFile1));
                        PicSign1.SetPosition(52 - delRowCnt, 2, 2, 2); //PicSign1.SetPosition(46 - delRowCnt, 2, 2, 2); //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                        PicSign1.SetSize(100);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //-- Approved by sign
                if (rApproveBySign != "" && rSigned == "Y")  //-- edit 22/07/2019 && rSigned == "Y"
                {
                    try
                    {
                        string imageFile2 = HttpContext.Current.Server.MapPath("../dat/Signature/" + rApproveBySign);
                        OfficeOpenXml.Drawing.ExcelPicture PicSign2 = myWorksheet.Drawings.AddPicture("Signature 2", System.Drawing.Image.FromFile(imageFile2));
                        PicSign2.SetPosition(52 - delRowCnt, 2, 9, 2);  //PicSign2.SetPosition(46 - delRowCnt, 2, 9, 2); //-- edit 26/06/2023 เพิ่มบรรทัด header analyzer area+ analysis method 5 บรรทัด
                        PicSign2.SetSize(100);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //===== image =====================================================


              


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
             finally
            {
                Utility.ClearObject(ref DTH);
                Utility.ClearObject(ref DTF);
            }
        }

        //-- write composition cell
        //private void WriteValue(ref ExcelWorksheet WS, string sCol, int sRow, int sessionCol, int sessionRow, Object gDR, int dec=3) //ไม่ต้องกำหนดทศนิยม เพราะกำหนดใน excel อยู่แล้ว

        //-- edit 22/07/2019 -- , String wcAlert = "", String minRun = "")
        //-- edit 30/07/2019 -- aDate 
        private void WriteValue(ref ExcelWorksheet WS, string sCol, int sRow, int sessionCol, int sessionRow, Object gDR, String wcAlert = "", String minRun = "", String aDate = "")
        {
            ExcelRange cell;
            String AL;
            try
            {
                
                cell = WS.Cells[sCol + sRow];
                if (Utility.IsNumeric(gDR))
                {
                    cell.Value = Utility.ToNum(gDR);
                    //-- ตรวจ alert
                    ShowAlertISO(ref cell, sCol, Utility.ToString(gDR),wcAlert, minRun, aDate);
                }
                else
                {
                    cell.Value = Utility.ToString(gDR);
                }


                //-- edit 23/04/2021 -- ในวันที่มีการแทนค่าสำรอง ต้องมีข้อความเป็นสีน้ำเงิน (ยกเว้นค่าน้ำ H2O) และตัดข้อมูล Unnorm ออกไป
                //-- edit 23/08/2022 -- แสดงข้อมูล Unnorm กลับมา
                if (gOgcName != "")
                {
                    int c = Utility.ToInt(sessionCol);
                    if ( c <= 15 || (c >= 17 && c <= 19))
                    {
                        cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                    }
                    //-- edit 23/08/2022 -- แสดงข้อมูล Unnorm กลับมา
                    //else
                    //{
                    //    if (c >= 17 && c <= 19) //unnorm, run ให้ใส่ -
                    //    {
                    //        cell.Value = "-";
                    //        cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                    //    }
                    //}
                }


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }

        }

        //-- edit 30/07/2019 -- aDate
        private void ShowAlertISO(ref ExcelRange cell, string sCol, String sValue, String wcAlert = "", String minRun = "", String aDate = "")
        {
            try
            {
                //-- ตรวจสอบว่าเป็น ISO Site หรือไม่ ถ้าใช่ต้องตรวจสอบค่า ว่าตรงกับเงื่อนไข ISO หรือไม่
                //  ค่า ISO ต้องอยู่ในช่วง x/ 2 – 2x     
                //  ต้องตรวจสอบค่า gas composition ต้องไม่ต่ำกว่า x / 2  และมากกว่า 2x ทศนิยม 6 ตำแหน่ง
                //  ยกเว้น H2S ไม่ต้องตรวจสอบ

                Double dVal = Utility.ToDouble(sValue);

                switch (sCol)
                {
                    //-- edit 23/04/2021 -- excel ไม่ต้องแสดงสี ISO
                    //-- edit 24/05/2021 --- แต่ต้องแสดง [1] ที่ Header ---
                    case "B": // "C1"
                        if (gC1 != -999 && (dVal < gC1 / 2 || dVal > 2 * gC1))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC1_MIN != -999 && dVal < gC1_MIN) || (gC1_MAX != -999 && dVal > gC1_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "C": // "C2":
                        if (gC2 != -999 && (dVal < gC2 / 2 || dVal > 2 * gC2))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC2_MIN != -999 && dVal < gC2_MIN) || (gC2_MAX != -999 && dVal > gC2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "D": // "C3":
                        if (gC3 != -999 && (dVal < gC3 / 2 || dVal > 2 * gC3))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC3_MIN != -999 && dVal < gC3_MIN) || (gC3_MAX != -999 && dVal > gC3_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "E": // "IC4":
                        if (gIC4 != -999 && (dVal < gIC4 / 2 || dVal > 2 * gIC4))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gIC4_MIN != -999 && dVal < gIC4_MIN) || (gIC4_MAX != -999 && dVal > gIC4_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "F": // "NC4":
                        if (gNC4 != -999 && (dVal < gNC4 / 2 || dVal > 2 * gNC4))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gNC4_MIN != -999 && dVal < gNC4_MIN) || (gNC4_MAX != -999 && dVal > gNC4_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "G": // "IC5":
                        if (gIC5 != -999 && (dVal < gIC5 / 2 || dVal > 2 * gIC5))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gIC5_MIN != -999 && dVal < gIC5_MIN) || (gIC5_MAX != -999 && dVal > gIC5_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "H": // "NC5":
                        if (gNC5 != -999 && (dVal < gNC5 / 2 || dVal > 2 * gNC5))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gNC5_MIN != -999 && dVal < gNC5_MIN) || (gNC5_MAX != -999 && dVal > gNC5_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "I": // "C6":
                        if (gC6 != -999 && (dVal < gC6 / 2 || dVal > 2 * gC6))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC6_MIN != -999 && dVal < gC6_MIN) || (gC6_MAX != -999 && dVal > gC6_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "J": // "CO2":
                        if (gCO2 != -999 && (dVal < gCO2 / 2 || dVal > 2 * gCO2))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gCO2_MIN != -999 && dVal < gCO2_MIN) || (gCO2_MAX != -999 && dVal > gCO2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;

                    case "K": // "N2":
                        if (gN2 != -999 && (dVal < gN2 / 2 || dVal > 2 * gN2))
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gN2_MIN != -999 && dVal < gN2_MIN) || (gN2_MAX != -999 && dVal > gN2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;

                    case "L": // "H2S":
                              //-- edit 08/07/2020 -- ถ้า site นั้นมี ISO ให้กำหนด H2S เป็น [1] เสมอ 
                        if (gISO_FLAG == "Y")
                        {

                            //cell.Style.Font.Color.SetColor(Color.Blue); // set the font color
                            if (gALERT2_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT2_COLUMN += "," + sCol + ",";
                            gISO_ACCREDIT = "N";
                        }

                        break;

                    case "M": //"NHV"
                    case "N": //"GHV"
                    case "O": //"SG"
                        break;

                    case "P": //H2O, WC //ค่า WC ต้องไม่เกิน 7 lb และไม่น้อยกว่าหรือเท่ากับ 0 และ กรณีไม่มีค่าซ้ำกันเกิน 3 ชั่วโมง
                        if (wcAlert == "Y")
                        {
                            cell.Style.Font.Color.SetColor(Color.Red); // set the font color
                        }

                        //-- คอลัมน์ H2O ต้องใส่ [1] เสมอ //-- edit 22/07/2019 ---
                        if (gALERT1_COLUMN.IndexOf("," + sCol + ",") < 0) gALERT1_COLUMN += "," + sCol + ",";

                        break;
                    case "Q": // "UNNORMMIN":   //ค่า UnnormMin ต้องไม่ต่ำกว่า 98
                        if (dVal < 98)
                        {
                            cell.Style.Font.Color.SetColor(Color.Red); // set the font color
                        }
                        break;
                    case "R": // "UNNORMMAX":   //ค่า UnnormMax ต้องไม่เกิน 102
                        if (dVal > 102)
                        {
                            cell.Style.Font.Color.SetColor(Color.Red); // set the font color
                        }
                        break;

                    case "S": // "RUN"://ค่า run ในตาราง จะต้องไม่ต่ำกว่า Minimum run ที่กำหนด
                        //Double minRUN = Utility.ToDouble(gDR["TOTAL_RUN"]) * ((100 - Utility.ToDouble(gDR["TOLERANCE_RUN"])) / 100);
                        //--เก็บค่า TOLERANCE_RUN เป็นตัวเลข เช่น 30 หมายถึง ยอมรับได้ในช่วง +-30
                        if (dVal < Utility.ToDouble(minRun))
                        {
                            cell.Style.Font.Color.SetColor(Color.Red); // set the font color
                        }
                        //-- edit 31/08/2020 -- ส่วน remark ให้แสดง OGC was calibrated on (date) ดึงวันที่ As Found จากระบบ OGC Data
                        ////-- edit 22/07/2019 --
                        //// เพิ่มหมายเหตุ โดยแสดงวันที่ที่มี chromat run ต่ำกว่า 360 เดือนนั้นต้องมีวันเดียว ถ้ามีหลายวันก็ไม่ต้องแสดง   ระบุค่าของ H2S, Hg ต่อท้ายด้วย  
                        //if (dVal > 0 && dVal < 360)
                        //{
                        //    if (gChromatDate == "")
                        //        gChromatDate = aDate; // Utility.FormatDate(Convert.ToDateTime(gDR["ADATE"]), "D MONTH YYYY"); //gDR["ADATE"]
                        //    else
                        //        gChromatDate = "MULTI-DATE";
                        //}



                        break;
                }


            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }



        private void GetISO(String iSiteID, String iMM, String iYY)
        {
            try
            {
                //-- iso ---------------------------
                String YYMM = Utility.ToString(Utility.ToNum(iYY) * 100 + Utility.ToNum(iMM));
                String OtherCri = "( TO_CHAR(ORDER_DATE,'YYYYMM') < '" + YYMM + "' OR ORDER_DATE IS NULL) " +
                                  " AND (TO_CHAR(EXPIRE_DATE, 'YYYYMM') >= '" + YYMM + "' OR EXPIRE_DATE IS NULL)  " +
                                  " AND S.SITE_ID IN (SELECT SITE_ID FROM O_SITE_FID WHERE SITE_ID="+ iSiteID+" AND ISO_FLAG='Y') "; //-- ต้องเป็น site ที่กำหนด iso ด้วย
                DataTable gasDT = Project.dal.SearchSiteSgc(iSiteID, OtherCriteria: OtherCri);
                if (gasDT != null && gasDT.Rows.Count > 0)
                {
                    DataRow gasDR = Utility.GetDR(ref gasDT);
                    if (Utility.ToString(gasDR["ORDER_DATE"]) != "")
                        gorderYMD = Utility.ToNum(Utility.FormatDate(Convert.ToDateTime(gasDR["ORDER_DATE"]), "YYYYMMDD"));
                    else
                        gorderYMD = 99999999;
                    if (Utility.ToString(gasDR["EXPIRE_DATE"]) != "")
                        gexpireYMD = Utility.ToNum(Utility.FormatDate(Convert.ToDateTime(gasDR["EXPIRE_DATE"]), "YYYYMMDD"));
                    else
                        gexpireYMD = 99999999;

                    if (Utility.IsNumeric(gasDR["C1"])) gC1 = Utility.ToDouble(gasDR["C1"]);
                    if (Utility.IsNumeric(gasDR["C2"])) gC2 = Utility.ToDouble(gasDR["C2"]);
                    if (Utility.IsNumeric(gasDR["C3"])) gC3 = Utility.ToDouble(gasDR["C3"]);
                    if (Utility.IsNumeric(gasDR["IC4"])) gIC4 = Utility.ToDouble(gasDR["IC4"]);
                    if (Utility.IsNumeric(gasDR["NC4"])) gNC4 = Utility.ToDouble(gasDR["NC4"]);
                    if (Utility.IsNumeric(gasDR["IC5"])) gIC5 = Utility.ToDouble(gasDR["IC5"]);
                    if (Utility.IsNumeric(gasDR["NC5"])) gNC5 = Utility.ToDouble(gasDR["NC5"]);
                    if (Utility.IsNumeric(gasDR["C6"])) gC6 = Utility.ToDouble(gasDR["C6"]);
                    if (Utility.IsNumeric(gasDR["N2"])) gN2 = Utility.ToDouble(gasDR["N2"]);
                    if (Utility.IsNumeric(gasDR["CO2"])) gCO2 = Utility.ToDouble(gasDR["CO2"]);
                    if (Utility.IsNumeric(gasDR["H2S"])) gH2S = Utility.ToDouble(gasDR["H2S"]);
                    if (Utility.IsNumeric(gasDR["HG"])) gHG = Utility.ToDouble(gasDR["HG"]);
                    if (Utility.IsNumeric(gasDR["C1_MIN"])) gC1_MIN = Utility.ToDouble(gasDR["C1_MIN"]);
                    if (Utility.IsNumeric(gasDR["C2_MIN"])) gC2_MIN = Utility.ToDouble(gasDR["C2_MIN"]);
                    if (Utility.IsNumeric(gasDR["C3_MIN"])) gC3_MIN = Utility.ToDouble(gasDR["C3_MIN"]);
                    if (Utility.IsNumeric(gasDR["IC4_MIN"])) gIC4_MIN = Utility.ToDouble(gasDR["IC4_MIN"]);
                    if (Utility.IsNumeric(gasDR["NC4_MIN"])) gNC4_MIN = Utility.ToDouble(gasDR["NC4_MIN"]);
                    if (Utility.IsNumeric(gasDR["IC5_MIN"])) gIC5_MIN = Utility.ToDouble(gasDR["IC5_MIN"]);
                    if (Utility.IsNumeric(gasDR["NC5_MIN"])) gNC5_MIN = Utility.ToDouble(gasDR["NC5_MIN"]);
                    if (Utility.IsNumeric(gasDR["C6_MIN"])) gC6_MIN = Utility.ToDouble(gasDR["C6_MIN"]);
                    if (Utility.IsNumeric(gasDR["N2_MIN"])) gN2_MIN = Utility.ToDouble(gasDR["N2_MIN"]);
                    if (Utility.IsNumeric(gasDR["CO2_MIN"])) gCO2_MIN = Utility.ToDouble(gasDR["CO2_MIN"]);
                    if (Utility.IsNumeric(gasDR["H2S_MIN"])) gH2S_MIN = Utility.ToDouble(gasDR["H2S_MIN"]);
                    if (Utility.IsNumeric(gasDR["HG_MIN"])) gHG_MIN = Utility.ToDouble(gasDR["HG_MIN"]);
                    if (Utility.IsNumeric(gasDR["C1_MAX"])) gC1_MAX = Utility.ToDouble(gasDR["C1_MAX"]);
                    if (Utility.IsNumeric(gasDR["C2_MAX"])) gC2_MAX = Utility.ToDouble(gasDR["C2_MAX"]);
                    if (Utility.IsNumeric(gasDR["C3_MAX"])) gC3_MAX = Utility.ToDouble(gasDR["C3_MAX"]);
                    if (Utility.IsNumeric(gasDR["IC4_MAX"])) gIC4_MAX = Utility.ToDouble(gasDR["IC4_MAX"]);
                    if (Utility.IsNumeric(gasDR["NC4_MAX"])) gNC4_MAX = Utility.ToDouble(gasDR["NC4_MAX"]);
                    if (Utility.IsNumeric(gasDR["IC5_MAX"])) gIC5_MAX = Utility.ToDouble(gasDR["IC5_MAX"]);
                    if (Utility.IsNumeric(gasDR["NC5_MAX"])) gNC5_MAX = Utility.ToDouble(gasDR["NC5_MAX"]);
                    if (Utility.IsNumeric(gasDR["C6_MAX"])) gC6_MAX = Utility.ToDouble(gasDR["C6_MAX"]);
                    if (Utility.IsNumeric(gasDR["N2_MAX"])) gN2_MAX = Utility.ToDouble(gasDR["N2_MAX"]);
                    if (Utility.IsNumeric(gasDR["CO2_MAX"])) gCO2_MAX = Utility.ToDouble(gasDR["CO2_MAX"]);
                    if (Utility.IsNumeric(gasDR["H2S_MAX"])) gH2S_MAX = Utility.ToDouble(gasDR["H2S_MAX"]);
                    if (Utility.IsNumeric(gasDR["H2S_MAX"])) gHG_MAX = Utility.ToDouble(gasDR["HG_MAX"]);
                }
 

                //-----------------------------
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }




    }
}