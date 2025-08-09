using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace PTT.GQMS.USL.Web.Forms
{
    //-- edit 22/06/2019 
    public partial class UploadHG : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String MsgSuccess = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskUpload, true);

                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();
                    ServerAction = Validation.GetParamStr("ServerAction");
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");
                    lblAlert.Text = "";
                    lblSuccess.Text = "";
                    gvResult.Visible = false;
                }

                switch (ServerAction)
                {
                    case "IMPORT_XLS":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            ImportData();

                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                        }
                        break;
                    case "SEARCH":
                        LoadData();
                        break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }

        //Prevent duplicate insert on page refresh
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["CheckRefresh"] = HttpContext.Current.Session["CheckRefresh"];
        }


        private void InitCtrl()
        {
            try
            {
                Utility.LoadYearCombo(ref ddlYEAR, "2018");
                Utility.LoadMonthCombo(ref ddlMONTH,false, "","EN","" );

                DateTime today = System.DateTime.Today;
                if (today.Day < 6) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        Utility.SetCtrl(ddlMONTH, "12");
                        Utility.SetCtrl(ddlYEAR, (today.Year - 1).ToString());
                    }
                    else
                    {
                        Utility.SetCtrl(ddlMONTH, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEAR, today.Year.ToString());
                    }


                }
                else
                {
                    Utility.SetCtrl(ddlMONTH, today.Month.ToString());
                    Utility.SetCtrl(ddlYEAR, today.Year.ToString());
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




        private void LoadData()
        {
            DataTable DT;


            try
            {

                String YYMM = Utility.GetCtrl(ddlYEAR) + Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');

                DT = Project.dal.SearchOgcHG("", OtherCriteria: "TO_CHAR(SDATE,'YYYYMM') = '" + YYMM + "'");

                Session["DT"] = DT;
                Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);
                gvResult.Visible = true;

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;


                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }





        //////======================================================================
        //////  IMPORT EXCEL
        //////======================================================================

        private void ImportData()
        {
            MngExcel Exc = null;
            String FullFileName = "";
            String FileName = "";
            String FileType = "";

            int FormatExcel = 0;

            DataTable DT1 = null;
            Int32 ExMaxCol1 = 0;
            Int32 ExHeadRow1 = 0;  //-- start header row
            Int32 ExRow1 = 0;
            //-- กำหนดคอลัมน์
            int colHG_NAME = -1, colSDATE = -1, colSTIME = -1, colSAMPLE_NO = -1, colHG = -1, colVOL = -1;
            string pHG_NAME = "", pSDATE = "";
            object pSAMPLE_NO = null, pHG = null, pVOL = null;
            int colNO = -1;
            object pNO = null;


            String ChkDate = "", Value = "";

            try
            {
                String sMM = Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');
                String sYY = Utility.GetCtrl(ddlYEAR);
                String sYYMM = Utility.GetCtrl(ddlYEAR) + Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');

                Project.ReadConfiguration();
                Msg = "";
                if (FileImportData.HasFile)
                {

                    FileType = (Utility.GetFileType(FileImportData.FileName) + "").ToLower();
                    FileName = Utility.GetFileName(FileImportData.FileName);

                    String gExcelFile = "||.xls|.xlsx||"; //-- ให้แค่ exel ไม่ให้ .csv
                    if (gExcelFile.IndexOf("|" + FileType + "|") > 0)
                    {
                        UploadUserFile(ref FileImportData);

                        if (Msg == "")
                        {

                            //Upload สำเร็จ -- จัดเก็บเข้า tmp Database
                            Exc = new MngExcel();
                            FullFileName = Server.MapPath(Project.gExcelPath + "Import/" + FileName);

                            //-- ส่งชื่อ worksheet ตามเดือนไป เนื่องจากกรณีมีหลาย worksheet แล้วตัวอ่าน excel จะเรียงตามชื่อ
                            String WSheetList = "|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC|";
                            String WSheet = WSheetList.Split('|')[Utility.ToInt(ddlMONTH.SelectedValue)];

                            DT1 = Exc.ReadWorksheet(FullFileName, WSheet);

                            if ((DT1 == null) || DT1.Rows.Count < 1)
                            { //อ่านชื่อย่อไม่พบ ให้อ่านจากชื่อเต็ม
                                WSheetList = "|JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|";
                                WSheet = WSheetList.Split('|')[Utility.ToInt(ddlMONTH.SelectedValue)];
                                DT1 = Exc.ReadWorksheet(FullFileName, WSheet);
                            }

                            if ((DT1 == null) || DT1.Rows.Count < 4)
                            {
                                Msg = " - Data not found!";

                            }
                            else
                            {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref Msg, ref ExMaxCol1, ref ExHeadRow1, ref colNO, ref colHG_NAME, ref colSDATE, ref colSTIME, ref colSAMPLE_NO, ref colHG, ref colVOL);
                                if (FormatExcel > 0)
                                {

                                    //-- delete tmp data
                                    Project.dal.MngTmpOgcHG(DBUTIL.opDELETE, "", "", OtherCri: " TO_CHAR(SDATE,'MM')='" + sMM + "' ");


                                    //2) บันทึกใน tmp table
                                    foreach (DataRow DR1 in DT1.Rows)
                                    {
                                        if (ExRow1 > ExHeadRow1) //-- ข้อมูลจะเริ่มจากบรรทัด Sampling Point ไปอีก 
                                        {

                                            pHG_NAME = null; pSDATE = null; pSAMPLE_NO = null; pHG = null; pVOL = null;


                                            //--- ตรวจสอบความถูกต้องของข้อมูล
                                            //-- column Sampling Point ถ้าไม่มีใน DB ก็ไม่นำเข้า เพราะตอนท้ายจะเป็น บรรทัด remark ------------------------- 
                                            if (colHG_NAME > -1) pHG_NAME = Utility.ToString(DR1[colHG_NAME]);
                                            if (colSDATE > -1) pSDATE = Utility.ToString(DR1[colSDATE]);

                                            if (pHG_NAME != "" && pSDATE.Length > 6)
                                            {
                                                DataTable dtN = Project.dal.SearchDimHG(pHG_NAME);
                                                if (dtN != null && dtN.Rows.Count > 0)
                                                {
                                                    pHG_NAME = Utility.ToString(dtN.Rows[0]["NAME"]);  //ใช้ชื่อตาม db 

                                                    //-- column Sampling Date ------------------------
                                                    Value = Utility.ToString(DR1[colSDATE]) + ""; //"8-ม.ค.-19" "10:00 AM"
                                                    if (Value != "")
                                                    {

                                                        //-- ตรวจสอบเรื่องวันที่ dd/mm/yyyy, mm/dd/yyyy, 8-ม.ค.-19, 8-Jan-19 
                                                        String dd = "", mm = "", yyyy = "", tt = "";

                                                        if (Value.IndexOf("/") > -1)
                                                        {
                                                            yyyy = Value.Split('/')[2];
                                                            if (Project.gXLSDateFormat == "mm/dd/yyyy")
                                                            {
                                                                dd = Value.Split('/')[1].PadLeft(2, '0');
                                                                mm = Value.Split('/')[0].PadLeft(2, '0');
                                                            }
                                                            else
                                                            {
                                                                dd = Value.Split('/')[0].PadLeft(2, '0');
                                                                mm = Value.Split('/')[1].PadLeft(2, '0');
                                                            }

                                                        }
                                                        else
                                                        {
                                                            if (Value.IndexOf("-") > -1)  // 8-ม.ค.-19, 8-Jan-19 
                                                            {
                                                                dd = Value.Split('-')[0].PadLeft(2, '0');
                                                                string x = Value.Split('-')[1];
                                                                if (x.IndexOf(".") > -1)
                                                                    mm = Utility.ThMonthAbbrVal(x);
                                                                else
                                                                    mm = Utility.EnMonthAbbrVal(x.ToUpper());
                                                                yyyy = Value.Split('-')[2];
                                                                if (yyyy.Length > 4) yyyy = Utility.Left(yyyy, 4);  //เผื่อมี time ติดมา
                                                                if (yyyy.IndexOf(" ") > -1) yyyy = yyyy.Split(' ')[0];
                                                                if (yyyy.Length == 2) yyyy = "20" + yyyy; //--19  จะได้เป็น 2019
                                                            }
                                                            else
                                                            {
                                                                Msg += " - Row: " + (ExRow1 + 1) + " - Invalid format DATE  (" + Value + ")</br>";
                                                            }
                                                        }

                                                        if (Msg == "")
                                                        {
                                                            //-- ระบบนี้เป็น ค.ศ. แต่บางครั้ง excel อ่านมาเป็น พ.ศ.
                                                            if (yyyy.Length > 4) yyyy = Utility.Left(yyyy, 4);
                                                            if (Utility.ToNum(yyyy) > 2500)
                                                            {
                                                                yyyy = Utility.ToString(Utility.ToNum(yyyy) - 543);
                                                            }

                                                            //--- time ---

                                                            if (colSTIME > -1)
                                                            {
                                                                string y = Utility.ToString(DR1[colSTIME]);  //0:00 , 0:00:00,  "10:00 AM","9:10 AM", "01:10  PM", "-"

                                                                if (y.Length > 4)
                                                                {
                                                                    if (y.IndexOf("AM") > -1 || y.IndexOf("PM") > -1)
                                                                    {
                                                                        string z = y.Split(':')[0];
                                                                        if (z.Length == 1)
                                                                        {
                                                                            y = "0" + y; //9:10 AM
                                                                            z = "0" + z; //09 
                                                                        }

                                                                        if (y.IndexOf("PM") > -1)
                                                                        {    // "12:10  PM" คือเที่ยง
                                                                            if (Utility.ToNum(z) < 12) z = Utility.ToString(Utility.ToNum(z) + 12);
                                                                        }

                                                                        tt = z + ":" + y.Substring(3, 2); //09:10
                                                                    }
                                                                    else
                                                                    {
                                                                        tt = y;
                                                                        if (tt.Split(':')[0].Length == 1) tt = "0" + tt; //0:00
                                                                    }

                                                                    tt = " " + tt;
                                                                }

                                                            }

                                                            ChkDate = dd + "/" + mm + "/" + yyyy + tt;

                                                            if (Utility.IsDate(ChkDate))
                                                            {
                                                                // ต้องตรวจสอบว่าวันที่ ตรงกับเดือนที่เลือกหรือไม่
                                                                if (yyyy == sYY && mm == sMM)
                                                                {
                                                                    pSDATE = ChkDate;
                                                                }
                                                                else
                                                                {
                                                                    Msg += " - Row: " + (ExRow1 + 1) + " - Invalid Month  (" + Value + ")</br>";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Msg += " - Row: " + (ExRow1 + 1) + " - Invalid format DATE  (" + Value + ")</br>";
                                                                break;
                                                            }

                                                        }



                                                        if (Msg == "")
                                                        {
                                                            if (colNO > -1) pNO = DR1[colNO];
                                                            if (colSAMPLE_NO > -1) pSAMPLE_NO = DR1[colSAMPLE_NO];
                                                            if (colHG > -1) pHG = DR1[colHG];
                                                            if (colVOL > -1) pVOL = DR1[colVOL];

                                                            //-- 15/08/2018 บางทีก็อาจจะเป็นการไม่ใช้ข้อมูลของวันนั้นก็ได้ เลยอนุญาตให้บันทึกข้อมูลในวันที่ไม่มีข้อมูล
                                                            //-- จะ import เฉพาะวันที่มีข้อมูลเท่านั้น Sampling point
                                                            if (Utility.ToString(pHG_NAME) != "")
                                                            {
                                                                Project.dal.MngTmpOgcHG(DBUTIL.opINSERT, pHG_NAME, pSDATE, pNO, pSAMPLE_NO, pHG, pVOL);

                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        //-- edit 04/10/2019 -- กรณีเป็นวันที่ว่าง ไม่ต้องแสดง alert
                                                       // Msg += " - Row: " + (ExRow1 + 1) + " - Invalid format DATE </br>";
                                                    }

                                         

                                                }


                                            }



                                        }

                                        ExRow1++;
                                    }


                                    if (Msg == "")
                                    {
                                        //5) บันทึกลง O_OGC_HG
                                        Project.dal.MngTmp2OgcHG(sYYMM);
                                       // MsgSuccess = "Successfully imported [" + FileName + "] </br>";

                                        //-- edit 10/10/2019 -- นับจำนวน records 
                                        string cnt = Project.dal.GetSQLValue("select COUNT(*) cnt from O_OGC_HG WHERE TO_CHAR(SDATE,'YYYYMM') = '" + sYYMM + "'");
                                        MsgSuccess = "Successfully imported [" + FileName + "],  " + Utility.FormatNum(cnt) + " records </br>";

                                    }


                                }
                                else
                                {
                                    Msg = " - The file template is invalid!";
                                }
                            }

                        }


                    }
                    else
                    {
                        Msg = " - Please select the excel file! (" + gExcelFile.Replace("|", " ") + ") </br>";
                    }


                }
                else
                {
                    Msg = "Please select the excel file!";
                }


            }
            catch (Exception ex)
            {
                if (ex.Message == "External table is not in the expected format.")
                {
                    Msg = " - Please select the excel file! </br>";
                }
                else if (string.IsNullOrEmpty(Msg))
                {
                    Msg += Utility.GetErrorMessage(ex) + " </br>";
                }

            }
            finally
            {
                lblSuccess.Text += MsgSuccess;

                if (Msg != "")
                {
                    //Fail to uploaded [BCS.xls] : 
                    //UploadXLS/System.Data.OleDb.OleDbException (0x80004005): Unexpected error from external database driver (22).\r\n at MngExcel.ReadWorksheet(String FileName, String WorkSheetName) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\App_Code\Modules\MngExcel.cs:line 122\r\n at PTT.GQMS.USL.Web.Forms.UploadXLS.ImportData(HttpPostedFile uploadFile) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\Forms\UploadXLS.aspx.cs:line 156 

                    if (Msg.IndexOf("external database driver (22)") > -1) Msg = " - Worksheet name must be less than 30 characters!</br>";

                    Msg = "Fail to uploaded [" + FileName + "] : </br>" + Msg;
                    lblAlert.Text += Msg;
                }

                Msg = "";
                MsgSuccess = "";

                Utility.ClearObject(ref DT1);



            }

        }


        private void UploadUserFile(ref FileUpload FileUpload)
        {
            String FullFileName = "", FileName = "", FileType = "";
            try
            {
                FileType = Utility.GetFileType(FileUpload.FileName).ToLower();
                FileName = Utility.GetFileName(FileUpload.FileName);

                if (FileUpload.PostedFile.ContentLength > 10485760)
                {
                    Msg = " - The file size exceeds the limit allowed (10MB) and cannot be saved! </br>";
                }
                else
                {
                    FullFileName = Server.MapPath(Project.gExcelPath + "Import/" + FileName);
                    FileUpload.SaveAs(FullFileName);
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }


        //-- edit 26/07/2019 -- เพิ่มคอลัมน์ A= No.
        private int CheckFormat(DataTable DT, ref string StrFormat, ref int ExMaxCol, ref int ExRow, ref int colNO, ref int colHG_NAME, ref int colSDATE, ref int colSTIME, ref int colSAMPLE_NO, ref int colHG, ref int colVOL)
        {
            string dataA = "";
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;

            int maxCol = 9;  //มี 10 คอลัมน์ เริ่มจาก 0  
            int startCol = 0; //เริ่มที่ column 0=A เริ่มจาก 0  
            int samCol = 1;  //column ="SAMPLINGPOINT"
            try
            {
                //-- มี 1 รูปแบบ -------------------------------------------------------------------------
                //_______________________________________________________________________________
                //บรรทัด1			    	        PTT PUBLIC COMPANY LIMITED							
                //บรรทัด2		      		        MERCURY ANALYSIS							
                //บรรทัด3		    		        using the NIC WA-5A (ASTM D 5504)							
                //บรรทัด4		    Report  No.   :  	QC 504 - 19 - 1	 						
                //บรรทัด5										
                //บรรทัด6		No. Sampling Point	Date	Sample No.	Time	Hg (ug/cu.m.)	Vol.(Lit.)	Remark	
                //บรรทัด7		1   OFFSHORE 34"		    HG20190041						
                //Column=>	A   B	            C 		D	        E       F	G	        H           I  J


                //--- การอ่านไฟล์ ถ้าคอลัมน์แรกไม่มีข้อมูล ระบบจะไม่อ่าน record นั้น ดังนั้นจึงทำให้บรรทัดเลื่อนได้
                //-- ดังนั้นจะตรวจสอบจากบรรทัด Sampling Point ในคอลัมน์ A  
                 //-- HEADER ROW1=//บรรทัด6		Sampling Point	Date	Sample No.	Time	Hg (ug/cu.m.)	Vol.(Lit.)	Remark	
                //-- จะปรับ upper และตัด ช่องว่าง -. ออก 

                if (maxCol > DT.Columns.Count - 1) maxCol = DT.Columns.Count - 1;

                foreach (DataRow DR in DT.Rows)
                {
                    dataA = Utility.ToString(GetCellData(DR, samCol)).ToUpper().Trim().Replace("-", "").Replace(".", "").Replace(" ", "");
                    if (dataA == "SAMPLINGPOINT")
                    {
                        ExMaxCol = maxCol;
                        //-- HEADER ROW1
                        for (int col = startCol; col < ExMaxCol; col++)
                        {
                            data = Utility.ToString(GetCellData(DR, col)).ToUpper().Trim().Replace("-", "").Replace(".", "").Replace(" ", "");
                            switch (data)
                            {
                                case "": break;
                                case "NO": colNO = col; break;
                                case "SAMPLINGPOINT": colHG_NAME = col; break;
                                case "DATE": colSDATE = col; break;
                                case "TIME": colSTIME = col; break;
                                case "SAMPLENO": colSAMPLE_NO = col; break;
                                case "HG(UG/CUM)": colHG = col; break; //Hg (ug/cu.m.)
                                case "VOL(LIT)": colVOL = col; break; //Vol.(Lit.)
                                case "REMARK":  break;
                                default:
                                    Msg = " - คอลัมน์ที่ " + Utility.ToString(col + 1) + " (" + Utility.ToString(GetCellData(DR, col)) + ") ชื่อคอลัมน์ไม่ถูกต้อง!";
                                    ChkFormat = 0; col = 99; break;
                            }

                        }

                        if (Msg == "")
                        {
                            if (colHG_NAME > -1 && colSDATE > -1)
                            {
                                ChkFormat = 1;
                                ExRow = rowIndex;
                            }
                            else
                            {
                                ChkFormat = 0;
                                ExRow = rowIndex;
                            }
                        }
                        break;
                    }

                    rowIndex++;
                    if (rowIndex > 15) break;  //วนหาแค่ 15 บรรทัด
                }


                return ChkFormat;

            }
            catch (Exception ex)
            {
                //Throw New BLLException(ex)
                return 0;
            }
            finally
            {
            }
        }


        private string GetCellData(DataRow DR, int ColNum)
        {
            try
            {
                string txt = DR[ColNum] + "";
                return (string.IsNullOrEmpty(txt) ? null : txt);
            }
            catch
            {
                return null;
            }
        }

        //////======================================================================



    }
}