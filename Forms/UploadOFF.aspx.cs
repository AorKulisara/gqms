using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace PTT.GQMS.USL.Web.Forms
{
    //-- edit 26/06/2019 
    public partial class UploadOFF : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        String MsgSuccess = "", MsgAlert = "";

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
                Utility.LoadMonthCombo(ref ddlMONTH, false, "", "EN", "");

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
            DataTable DTfid = null;
            DataTable DT1 = null;
            Int32 ExMaxCol1 = 0;
            Int32 ExHeadRow1 = 0;  //-- start header row
            Int32 ExRow1 = 0;
            //-- กำหนดคอลัมน์
            int colRDATE = -1, colC1 = -1, colC2 = -1, colC3 = -1, colIC4 = -1, colNC4 = -1, colIC5 = -1, colNC5 = -1, colC6 = -1, colC7 = -1;
            int colCO2 = -1, colN2 = -1, colGHV = -1, colSG = -1, colH2O = -1, colHG = -1, colH2S = -1;
            string pRDATE = "";
            object pC1 = null, pC2 = null, pC3 = null, pIC4 = null, pNC4 = null, pIC5 = null, pNC5 = null, pC6 = null, pC7 = null;
            object pCO2 = null, pN2 = null, pGHV = null, pSG = null, pH2O = null, pHG = null, pH2S = null;

            String ChkDay = "";
            String ChkDate = "", Value = "";
            String fid = "";
            int CntSuccess = 0, CntFail = 0;
            
            try
            {
                String sMM = Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');
                String sYY = Utility.GetCtrl(ddlYEAR);
                String sYYMM = Utility.GetCtrl(ddlYEAR) + Utility.GetCtrl(ddlMONTH).PadLeft(2, '0');
                String sMMYY = "/" + Utility.GetCtrl(ddlMONTH).PadLeft(2, '0') + "/" + Utility.GetCtrl(ddlYEAR);

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

                            //-- ส่งชื่อ worksheet ตาม Offshore FID เนื่องจากกรณีมีหลาย worksheet แล้วตัวอ่าน excel จะเรียงตามชื่อ
                            String WSheet = "";
                            DTfid = Project.dal.SearchOffshoreFID(orderSQL: "FID");
                            foreach (DataRow DRfid in DTfid.Rows)
                            {
                                WSheet = Utility.ToString(DRfid["FID"]);
                                fid = Utility.ToString(DRfid["FID"]);
                                 
                                //------------------------------------------------------------------------------------------------------------
                                //-- อ่าน worksheet ตาม fid
                                DT1 = Exc.ReadWorksheet(FullFileName, WSheet);

                                if ((DT1 == null) || DT1.Rows.Count < 4)
                                {
                                    // Msg = " - Data not found!"; //กรณี worksheet ตามชื่อ fid ไม่มี ไม่ต้องแสดง error ให้ทำ worksheet อื่นต่อเลย

                                }
                                else
                                {
                                    Msg = "";
                                    ExMaxCol1 = 0; ExHeadRow1 = 0; ExRow1 = 0;
                                    //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                    FormatExcel = CheckFormat(DT1, ref Msg, ref ExMaxCol1, ref ExHeadRow1, ref colRDATE, ref colC1, ref colC2, ref colC3, ref colIC4, ref colNC4, ref colIC5, ref colNC5, ref colC6, ref colC7
                                                            , ref colCO2, ref colN2, ref colGHV, ref colSG, ref colH2O, ref colHG, ref colH2S);
                                    if (FormatExcel > 0)
                                    {

                                        //-- delete tmp data
                                        Project.dal.MngTmpOffshoreDaily(DBUTIL.opDELETE,"","",fid);

       
                                        //2) บันทึกใน tmp table
                                        foreach (DataRow DR1 in DT1.Rows)
                                        {
                                            if (ExRow1 > ExHeadRow1+1 && ExRow1 < ExHeadRow1 + 33 && Msg == "") //-- ข้อมูลจะเริ่มจากบรรทัด Date ไปอีก 2 บรรทัด  และต้องไม่เกิน 31 วัน เพราะท้ายตารางมีตารางอีก 
                                            {

                                                pRDATE = "";
                                                pC1 = null; pC2 = null; pC3 = null; pIC4 = null; pNC4 = null; pIC5 = null; pNC5 = null; pC6 = null; pC7 = null;
                                                pCO2 = null; pN2 = null; pGHV = null; pSG = null; pH2O = null; pHG = null; pH2S = null;

                                                //--- ตรวจสอบความถูกต้องของข้อมูล
                                                //-- column Date ------------------------
                                               if (colRDATE > -1) pRDATE = Utility.ToString(DR1[colRDATE]);  //คอลัมน์ Date 

                                               if (pRDATE != "")
                                                {
                                                    if (pRDATE.Length > 6)
                                                    {
                                                        //-- Date ------------------------
                                                        Value = Utility.ToString(DR1[colRDATE]) + ""; //"1/6/2018"  เป็น dd/mm/yyyy
                                                        if (Value != "")
                                                        {

                                                            //-- ตรวจสอบเรื่องวันที่ dd/mm/yyyy, mm/dd/yyyy, 8-ม.ค.-19, 8-Jan-19 
                                                            String dd = "", mm = "", yyyy = "", tt = "";

                                                            if (Value.IndexOf("/") > -1)
                                                            {
                                                                yyyy = Value.Split('/')[2];

                                                                //-- edit 26/11/2019 -- เนื่องจาก import excel format date ของ offshore ไม่เหมือนกับ
                                                                //โดยวันที่แรกจะเป็นวันที่ 1 เสมอ  
                                                                if (Project.gXLSDateFormatOff == "mm/dd/yyyy")
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
                                                                    Msg += " - Row: " + ExRow1 + " - Invalid format DATE  (" + Value + ")</br>";
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

                                                                ChkDate = dd + "/" + mm + "/" + yyyy + tt;

                                                                if (Utility.IsDate(ChkDate))
                                                                {
                                                                    // ต้องตรวจสอบว่าวันที่ ตรงกับเดือนที่เลือกหรือไม่
                                                                    if (yyyy == sYY && mm == sMM)
                                                                    {
                                                                        pRDATE = ChkDate;
                                                                    }
                                                                    else
                                                                    {
                                                                        Msg += " - Row: " + ExRow1 + " - Invalid Month/Year  (" + Value + ")</br>";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Msg += " - Row: " + ExRow1 + " - Invalid format DATE  (" + Value + ")</br>";
                                                                    break;
                                                                }

                                                            }

                                                        }
                                                        else
                                                        {
                                                            Msg += " - Row: " + ExRow1 + " - Invalid format DATE </br>";
                                                        }

                                                        if (Msg == "")
                                                        {
                                                            if (colC1 > 0) pC1 = DR1[colC1]; if (colC2 > 0) pC2 = DR1[colC2];
                                                            if (colC3 > 0) pC3 = DR1[colC3]; if (colIC4 > 0) pIC4 = DR1[colIC4];
                                                            if (colNC4 > 0) pNC4 = DR1[colNC4]; if (colIC5 > 0) pIC5 = DR1[colIC5];
                                                            if (colNC5 > 0) pNC5 = DR1[colNC5]; if (colC6 > 0) pC6 = DR1[colC6];
                                                            if (colC7 > 0) pC7 = DR1[colC7]; if (colCO2 > 0) pCO2 = DR1[colCO2];
                                                            if (colN2 > 0) pN2 = DR1[colN2]; if (colGHV > 0) pGHV = DR1[colGHV];
                                                            if (colSG > 0) pSG = DR1[colSG]; if (colH2O > 0) pH2O = DR1[colH2O];
                                                            if (colHG > 0) pHG = DR1[colHG]; if (colH2S > 0) pH2S = DR1[colH2S];

                                                            Project.dal.MngTmpOffshoreDaily(DBUTIL.opINSERT, "", ChkDate, fid, pC1, pC2, pC3, pIC4, pNC4, pIC5, pNC5, pC6, pC7, pCO2, pN2, pGHV, pSG, pH2O, pHG, pH2S);

                                                        }

                                                    }
                                                    else
                                                    { //-- edit 19/09/2019 -- กรณีวันที่ผิด
                                                        Msg += " - Row: " + ExRow1 + " - Invalid format DATE  (" + pRDATE + ")</br>";

                                                    }
                                                }
                                       

                                            }

                                            ExRow1++;
                                        }


                                        if (Msg == "")
                                        {
                                            //5) บันทึกลง  OFFSHORE_DAILY_UPDATE
                                            Project.dal.MngTmp2OffshoreDailyUpdate(fid, sYYMM);
                                            //MsgSuccess += "Successfully imported [" + WSheet + "] </br>";

                                            //-- edit 10/10/2019 -- นับจำนวน records 
                                            string cnt = Project.dal.GetSQLValue("select COUNT(*) cnt  FROM OFFSHORE_DAILY_UPDATE WHERE FID='" + fid + "' AND TO_CHAR(RDATE,'YYYYMM')= '" + sYYMM + "' ");
                                            MsgSuccess += "Successfully imported [" + WSheet + "],  " + Utility.FormatNum(cnt) + " records </br>";


                                            CntSuccess++;
                                        }
                                        else
                                        {
                                            MsgAlert += "Fail to uploaded [" + WSheet + "] : </br>" + Msg;
                                            CntFail++;
                                        }


                                    }
                                    else
                                    {
                                       // Msg = " - The file template is invalid!";  //กรณี worksheet ตามชื่อ fid ไม่ตรง format template ไม่ต้องแสดง ให้ทำ worksheet อื่นต่อเลย
                                    }
                                }
                                //------------------------------------------------------------------------------------------------------------
                                //-- อ่าน worksheet ตาม fid
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
                if (CntSuccess > 0) lblSuccess.Text += "<b>Total successed " + CntSuccess + " sheets.</b></br>";

                if (MsgAlert != "")
                {
                    //Fail to uploaded [BCS.xls] : 
                    //UploadXLS/System.Data.OleDb.OleDbException (0x80004005): Unexpected error from external database driver (22).\r\n at MngExcel.ReadWorksheet(String FileName, String WorkSheetName) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\App_Code\Modules\MngExcel.cs:line 122\r\n at PTT.GQMS.USL.Web.Forms.UploadXLS.ImportData(HttpPostedFile uploadFile) in D:\WORK\PTT_OGC_Data_Verfication_61\Source\PTT_OGC\Forms\UploadXLS.aspx.cs:line 156 

                    if (Msg.IndexOf("external database driver (22)") > -1) MsgAlert = " - Worksheet name must be less than 30 characters!</br>";

                    lblAlert.Text += MsgAlert;

                    if (CntFail > 0) lblAlert.Text += "<b>Total failed " + CntFail + " sheets.</b></br>";
                }

                Msg = "";
                MsgSuccess = "";

                Utility.ClearObject(ref DT1);
                Utility.ClearObject(ref DTfid);


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



        private int CheckFormat(DataTable DT, ref string StrFormat, ref int ExMaxCol, ref int ExRow, ref int colRDATE, ref int colC1, ref int colC2, ref int colC3, ref int colIC4, ref int colNC4, ref int colIC5, ref int colNC5, ref int colC6, ref int colC7
                                , ref int colCO2, ref int colN2, ref int colGHV, ref int colSG, ref int colH2O, ref int colHG, ref int colH2S)
        {
            string dataA = "";
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;

            int maxCol = 19;  //อ่านแค่ 19 คอลัมน์ เริ่มจาก 0  (มีคอลัมน์เยอะ)
            int startCol = 1; //เริ่มที่ column 1=B เริ่มจาก 0  
            try
            {
                //-- มี 1 รูปแบบ -------------------------------------------------------------------------
                //_______________________________________________________________________________
                //บรรทัด1 	Home		            C1	    C2	    C3	    iC4	    nC4	    iC5	    nC5	    C6	    C7+	    CO2	    N2	    Hg	    H2S	    GHVsat	    SG	    WI	        H2O		
                //บรรทัด2 	Calculate		        % Mole											                                        ug/m3	ppmv	Btu/scf	    -	    Btu/scf	    lb/MMscf		
                //บรรทัด3 		        1 - 15	    58.363 	8.077 	5.781 	1.286 	1.172 	0.277 	0.168 	0.087 	0.042 	22.751 	1.995 	0.334 	28.658 	971.132 	0.9279 	1,026.020 	0.0299 		
                //บรรทัด4 		        16 - 30	    54.224 	7.757 	5.748 	1.306 	1.133 	0.240 	0.142 	0.065 	0.029 	21.243 	1.864 	0.270 	28.470 	918.520 	0.8737 	1,000.067 	0.0256 		
                //บรรทัด5 		        AVG (1-31)	56.227 	7.912 	5.764 	1.296 	1.152 	0.258 	0.154 	0.076 	0.035 	21.973 	1.927 	0.292 	27.643 	943.978 	0.8999 	1,012.706 	0.0277 		
                //บรรทัด6 		        MIN	        0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	0.000 	    0.0000 	#DIV/0!	    0.0000 		
                //บรรทัด7 		        MAX	        58.562 	8.523 	6.416 	1.486 	1.308 	0.398 	0.260 	0.186 	0.117 	22.825 	2.342 	0.344 	31.700 	990.354 	0.9367 	1,041.417 	0.0323 		
                //บรรทัด8 																				
                //บรรทัด9 		                    C1	    C2	    C3	    iC4	    nC4	    iC5	    nC5	    C6+	    C7	    CO2	    N2	    H2S	    GHV(sat)SG	        Water	Hg	        H2S	        Sum	
                //บรรทัด10 		        Date	    CH4	    C2H6	C3H8	i-C4H10	n-C4H10	i-C5H12	n-C5H12	C6H14	C7H16	CO2	    N2	    H2S	    GHvsat	SG	        H2O	    ug/m3	    H2S		
                //บรรทัด11 		        unit	    mole %											                                        ppm	    Btu/scf		        lb/MMscf		    ppm		
                //บรรทัด12 		        6/1/2018    58.562	7.879	5.798	1.319	1.155	0.268	0.158	0.077	0.031	22.730	2.022		    968.769	0.9263	    0.0304	0.3312	    28.4342	    100.000 	
                //Column=>	A		    B	        C       D	    E       F       G	    H       I       J       K       L       M       N       O       P           Q       R           S           T



                //--- การอ่านไฟล์ ถ้าคอลัมน์แรกไม่มีข้อมูล ระบบจะไม่อ่าน record นั้น ดังนั้นจึงทำให้บรรทัดเลื่อนได้
                //-- ดังนั้นจะตรวจสอบจากบรรทัด Date ในคอลัมน์ B  
                //-- HEADER ROW1=//บรรทัด10 		        Date	    CH4	    C2H6	C3H8	i-C4H10	n-C4H10	i-C5H12	n-C5H12	C6H14	C7H16	CO2	    N2	    H2S	    GHvsat	SG	        H2O	    ug/m3	    H2S		
                //-- จะปรับ upper และตัด ช่องว่าง -. ออก 

                if (maxCol > DT.Columns.Count - 1) maxCol = DT.Columns.Count - 1;

                foreach (DataRow DR in DT.Rows)
                {
                    dataA = Utility.ToString(GetCellData(DR, startCol)).ToUpper().Trim().Replace("-", "").Replace(".", "").Replace(" ", "");
                    if (dataA == "DATE")
                    {
                        ExMaxCol = maxCol;
                        //-- HEADER ROW1
                        for (int col = startCol; col < ExMaxCol; col++)
                        {
                            data = Utility.ToString(GetCellData(DR, col)).ToUpper().Trim().Replace("-", "").Replace(".", "").Replace(" ", "");
                            switch (data)
                            {
                                case "": break;
                                case "DATE": colRDATE = col; break;
                                case "CH4": colC1 = col; break;
                                case "C2H6": colC2 = col; break;
                                case "C3H8": colC3 = col; break;
                                case "IC4H10": colIC4 = col; break;
                                case "NC4H10": colNC4 = col; break;
                                case "IC5H12": colIC5 = col; break;
                                case "NC5H12": colNC5 = col; break;
                                case "C6H14": colC6 = col; break;
                                case "C7H16": colC7 = col; break;
                                case "CO2": colCO2 = col; break;
                                case "N2": colN2 = col; break; 
                                case "GHVSAT": colGHV = col; break;
                                case "SG": colSG = col; break;
                                case "H2O": colH2O = col; break;
                                case "HG":
                                case "UG/M3": colHG = col; break;
                                case "H2S": colH2S = col; break; //H2S มี 2 คอลัมน์ ให้ใช้อันหลัง
                                default:
                                    Msg = " - คอลัมน์ที่ " + Utility.ToString(col + 1) + " (" + Utility.ToString(GetCellData(DR, col)) + ") ชื่อคอลัมน์ไม่ถูกต้อง!";
                                    ChkFormat = 0; col = 99; break;
                            }

                        }

                        if (Msg == "")
                        {
                            if (colRDATE > -1)
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