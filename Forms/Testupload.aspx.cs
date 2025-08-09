using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.OGC.USL.Web
{
    public partial class Testupload : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String MsgSuccess = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {



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
                Utility.LoadMonthCombo(ref ddlMONTH);

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
            int colH2S_NAME = -1, colSDATE = -1, colSTIME = -1, colSAMPLE_NO = -1, colSULFUR = -1, colH2S = -1, colCOS = -1, colCH3SH = -1, colC2H5SH = -1, colDMS = -1, colLSH = -1, colC3H7SH = -1;
            string pH2S_NAME = "", pSDATE = "";
            object pSAMPLE_NO = null, pSULFUR = null, pH2S = null, pCOS = null, pCH3SH = null, pC2H5SH = null, pDMS = null, pLSH = null, pC3H7SH = null;



            String ChkDate = "", Value = "";

            try
            {
                String sMM = Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');
                String sYY = Utility.GetCtrl(ddlYEAR);
                String sYYMM = Utility.GetCtrl(ddlYEAR)+Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');

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

                            if ((DT1 == null) || DT1.Rows.Count < 4)
                            {
                                Msg = " - Data not found!";

                            }
                            else
                            {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref Msg, ref ExMaxCol1, ref ExHeadRow1, ref colH2S_NAME, ref colSDATE, ref colSTIME, ref colSAMPLE_NO, ref colSULFUR, ref colH2S, ref colCOS, ref colCH3SH, ref colC2H5SH, ref colDMS, ref colLSH, ref colC3H7SH);
                                if (FormatExcel > 0)
                                {
                                    

                                    //-- delete tmp data
                                    Project.dal.MngTmpOgcH2S(DBUTIL.opDELETE,""," TO_CHAR(SDATE,'MM')='" + sMM +"' ");

                                    

                                    //2) บันทึกใน tmp table
                                    foreach (DataRow DR1 in DT1.Rows)
                                    {
                                        if (ExRow1 > ExHeadRow1) //-- ข้อมูลจะเริ่มจากบรรทัด Sampling Point ไปอีก 
                                        {
   
                                            pH2S_NAME = null; pSDATE = null; pSAMPLE_NO = null; pSULFUR = null; pH2S = null; pCOS = null; pCH3SH = null; pC2H5SH = null; pDMS = null; pLSH = null; pC3H7SH = null;

                                            //--- ตรวจสอบความถูกต้องของข้อมูล
                                            //-- column Sampling Date ------------------------

                                            Value = Utility.ToString(DR1[colSDATE]) + "";
                                            if (Value != "")
                                            {

                                                //-- ตรวจสอบเรื่องวันที่ dd/mm/yyyy, mm/dd/yyyy
                                                //-- ระบบนี้เป็น ค.ศ. แต่บางครั้ง excel อ่านมาเป็น พ.ศ.
                                                String yyyy = Value.Split('/')[2];
                                                if (yyyy.Length > 4) yyyy = Utility.Left(yyyy, 4);
                                                if (Utility.ToNum(yyyy) > 2500)
                                                {
                                                    yyyy = Utility.ToString(Utility.ToNum(yyyy) - 543);
                                                }

                                                //--- time ---
                                                String tt = "";
                                                if (colSTIME > 0)
                                                {
                                                    tt = Utility.ToString(DR1[colSTIME]);  //0:00 , 0:00:00
                                                    if (tt.Split(':')[0].Length == 1) tt = "0" + tt; //0:00
                                                    tt = " " + tt;
                                                }


                                                if (Project.gXLSDateFormat == "mm/dd/yyyy" )
                                                {
                                                    ChkDate = Value.Split('/')[1].PadLeft(2, '0') + "/" + Value.Split('/')[0].PadLeft(2, '0') + "/" + yyyy + tt;
                                                }
                                                else
                                                {
                                                    ChkDate = Value.Split('/')[0].PadLeft(2, '0') + "/" + Value.Split('/')[1].PadLeft(2, '0') + "/" + yyyy + tt;
                                                }

                                                if (Utility.IsDate(ChkDate))
                                                {
                                                    // ต้องตรวจสอบว่าวันที่ ตรงกับเดือนที่เลือกหรือไม่
                                                    string mm = ChkDate.Split('/')[1].PadLeft(2);
                                                    if ( yyyy == sYY && mm == sMM)
                                                    {
                                                        pSDATE = ChkDate;
                                                    }
                                                    else
                                                    {
                                                        Msg += " - Row: " + ExRow1 + " - Invalid DATE  (" + Value + ")</br>";
                                                    }
                                                }
                                                else
                                                {
                                                    Msg += " - Row: " + ExRow1 + " - Invalid format DATE  (" + Value + ")</br>";
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                Msg += " - Row: " + ExRow1 + " - Invalid format DATE </br>";
                                            }

                                            if ( Msg == "")
                                            {
                                                if (colH2S_NAME > 0) pH2S_NAME = Utility.ToString(DR1[colH2S_NAME]);
                                                if (colSAMPLE_NO > 0) pSAMPLE_NO = DR1[colSAMPLE_NO];
                                                if (colSULFUR > 0) pSULFUR = DR1[colSULFUR];
                                                if (colH2S > 0) pH2S = DR1[colH2S];
                                                if (colCOS > 0) pCOS = DR1[colCOS];
                                                if (colCH3SH > 0) pCH3SH = DR1[colCH3SH];
                                                if (colC2H5SH > 0) pC2H5SH = DR1[colC2H5SH];
                                                if (colDMS > 0) pDMS = DR1[colDMS];
                                                if (colLSH > 0) pLSH = DR1[colLSH];
                                                if (colC3H7SH > 0) pC3H7SH = DR1[colC3H7SH];


                                                //-- 15/08/2018 บางทีก็อาจจะเป็นการไม่ใช้ข้อมูลของวันนั้นก็ได้ เลยอนุญาตให้บันทึกข้อมูลในวันที่ไม่มีข้อมูล
                                                //-- จะ import เฉพาะวันที่มีข้อมูลเท่านั้น Sampling point
                                                if ( Utility.ToString(pH2S_NAME) != "" )
                                                {

                                                    Project.dal.MngTmpOgcH2S(DBUTIL.opINSERT, pH2S_NAME, pSDATE, pSAMPLE_NO, pSULFUR, pH2S, pCOS, pCH3SH, pC2H5SH, pDMS, pLSH, pCH3SH);
                                                }
                                            }

                                        }

                                        ExRow1++;
                                    }


                                    if (Msg == "")
                                    {
                                        //5) บันทึกลง O_OGC_H2S
                                        Project.dal.MngTmp2OgcH2S(sYYMM);
                                        MsgSuccess = "Successfully imported [" + FileName + "] </br>";

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
                    //UploadXLS/System.Data.OleDb.OleDbException (0x80004005): Unexpected error from external database driver (22).\r\n at MngExcel.ReadWorksheet(String FileName, String WorkSheetName) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\App_Code\Modules\MngExcel.cs:line 122\r\n at PTT.OGC.USL.Web.Forms.UploadXLS.ImportData(HttpPostedFile uploadFile) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\Forms\UploadXLS.aspx.cs:line 156 

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



        private int CheckFormat(DataTable DT, ref string StrFormat, ref int ExMaxCol, ref int ExRow, ref int colH2S_NAME, ref int colSDATE, ref int colSTIME, ref int colSAMPLE_NO, ref int colSULFUR, ref int colH2S, ref int colCOS, ref int colCH3SH, ref int colC2H5SH, ref int colDMS, ref int colLSH, ref int colC3H7SH)
        {
            string dataA = "";
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;

            int maxCol = 13;  //มี 14 คอลัมน์ เริ่มจาก 0  
            int startCol = 1; //เริ่มที่ column 1=B เริ่มจาก 0  
            try
            {
                //-- มี 1 รูปแบบ -------------------------------------------------------------------------
                //_______________________________________________________________________________
                //บรรทัด1														
                //บรรทัด2										ต้นฉบับ		สำเนา		
                //บรรทัด3		PETROLEUM  AUTHORITY  OF  THAILAND												
                //บรรทัด4		Operation Centre Laboratory												
                //บรรทัด5		59 Moo 8 , Bypass Road , Thumbol Napa , Amphure Muang , Cholburi , 20000 												
                //บรรทัด6		Tel No. (038)274-390-8, (02)537-2000 ext.35106-7   												
                //บรรทัด7														
                //บรรทัด8		MONTHLY TOTAL SULFUR CONTENT REPORT												
                //บรรทัด9														
                //บรรทัด10	Report Number   :  	C802 - 18 - 05									Month  :  	MAY,2018	
                //บรรทัด11													
                //บรรทัด12	Sampling Point	Sampling Date	Time	Sample No.	Total Sulfur 	ปริมาณ Sulfur Contents (ppm.)							
                //บรรทัด13									                                    H2S	    COS	    CH3SH	C2H5SH	DMS	    T-ButylSH   C3H7SH	
                //บรรทัด14	OFFSHORE 34"	26-Mar-19	09:15 AM	S2018128	8.65	        9.19	0.00	0.00	0.00	0.00	0.00	    0.00	
                //Column=>A	B		        C		    D	        E		    F	            G	    H	    I	    J	    K	    L	        M



                //--- การอ่านไฟล์ ถ้าคอลัมน์แรกไม่มีข้อมูล ระบบจะไม่อ่าน record นั้น ดังนั้นจึงทำให้บรรทัดเลื่อนได้
                //-- ดังนั้นจะตรวจสอบจากบรรทัด Sampling Point ในคอลัมน์ B เนื่องจากมี merge cell ต้องตรวจสอบ 2 บรรทัด
                //-- HEADER ROW1=//บรรทัด12	Sampling Point	Sampling Date	Time	Sample No.	Total Sulfur 	ปริมาณ Sulfur Contents (ppm.)							
                //-- HEADER ROW2=//บรรทัด13									                            H2S	    COS	    CH3SH	C2H5SH	DMS	    T-ButylSH C3H7SH	
                //-- จะปรับ upper และตัด ช่องว่าง -. ออก 

                if (maxCol > DT.Columns.Count - 1) maxCol = DT.Columns.Count - 1;

                foreach (DataRow DR in DT.Rows)
                {
                    dataA = Utility.ToString(GetCellData(DR, startCol)).ToUpper().Trim().Replace("-", "").Replace(".", "").Replace(" ","");
                    if (dataA == "SAMPLINGPOINT")
                    {
                        ExMaxCol = maxCol;
                        //-- HEADER ROW1
                        for (int col = startCol; col < ExMaxCol; col++)
                        {
                            data = Utility.ToString(GetCellData(DR, col)).ToUpper().Trim().Replace("-", "").Replace(".","").Replace(" ", "");
                            switch (data)
                            {
                                case "": break;
                                case "SAMPLINGPOINT": colH2S_NAME = col; break;
                                case "SAMPLINGDATE": colSDATE = col; break;
                                case "TIME": colSTIME = col; break;
                                case "SAMPLENO": colSAMPLE_NO = col; break;
                                case "TOTALSULFUR": colSULFUR = col; break;
                                case "H2S": colH2S = col; break;
                                case "COS": colCOS = col; break;
                                case "CH3SH": colCH3SH = col; break;
                                case "C2H5SH": colC2H5SH = col; break;
                                case "DMS": colDMS = col; break;
                                case "TBUTYLSH": colLSH = col; break;
                                case "C3H7SH": colC3H7SH = col; break;
                            }

                        }


                        //-- HEADER ROW2 
                        rowIndex = rowIndex + 1;
                        DataRow DR2 = DT.Rows[rowIndex];
                        for (int col = startCol; col < ExMaxCol; col++)
                        {
                            data = Utility.ToString(GetCellData(DR2, col)).ToUpper().Trim().Replace("-", "").Replace(".", "");
                            switch (data)
                            {
                                case "": break;
                                case "SAMPLINGPOINT": colH2S_NAME = col; break;
                                case "SAMPLINGDATE": colSDATE = col; break;
                                case "TIME": colSTIME = col; break;
                                case "SAMPLENO": colSAMPLE_NO = col; break;
                                case "TOTALSULFUR": colSULFUR = col; break;
                                case "H2S": colH2S = col; break;
                                case "COS": colCOS = col; break;
                                case "CH3SH": colCH3SH = col; break;
                                case "C2H5SH": colC2H5SH = col; break;
                                case "DMS": colDMS = col; break;
                                case "TBUTYLSH": colLSH = col; break;
                                case "C3H7SH": colC3H7SH = col; break;
                                default:
                                    Msg = " - คอลัมน์ที่ " + Utility.ToString(col + 1) + " (" + Utility.ToString(GetCellData(DR2, col)) + ") ชื่อคอลัมน์ไม่ถูกต้อง!";
                                    ChkFormat = 0; col = 99; break;
                            }

                        }

                        if (Msg == "")
                        {
                            ChkFormat = 1;
                            ExRow = rowIndex;
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
        //////======================================================================


    }
}