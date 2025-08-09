using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace PTT.GQMS.USL.Web.Forms
{
    //-- edit 17/07/2018 
    public partial class UploadXLS : System.Web.UI.Page
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
                }

                switch (ServerAction)
                {
                    case "IMPORT_XLS":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            ImportAll();

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


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }
        }

        private void ImportAll()
        {
            try
            {
                Project.ReadConfiguration();
                Msg = "";
                if (FileImportData.HasFile)
                {

                    foreach (HttpPostedFile uploadedFile in FileImportData.PostedFiles)
                    {
                        ImportData(uploadedFile);

                    }

                }
                else {
                    Msg = "Please select the excel file!";
                }


            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(Msg))
                {
                    Msg += Utility.GetErrorMessage(ex) + " </br>";
                }

            }
        }


        private void ImportData(HttpPostedFile uploadFile)
        {
            MngExcel Exc = null;
            String FullFileName = "";
            String FileName = "";
            String FileType = "";

            DataTable DT1 = null;
            Int32 ExRow1 = 0;

            String Value = "";
            String ChkDate = "";

            int FormatExcel = 0;
            String FID = "";
            int colBTUDATE = -1, colBTU=-1, colRUN=-1;
            String pBTUDATE = "", pBTU = "", pRUN = "";
            try
            {
                FileType = (Utility.GetFileType(uploadFile.FileName) + "").ToLower();
                FileName = Utility.GetFileName(uploadFile.FileName);

                String gExcelFile = "|" + Project.gExcelFileType + "|";
                if (gExcelFile.IndexOf("|" + FileType + "|") > 0)
                {
                    FileName = UploadUserFile(ref uploadFile);

                    if ( Msg == "" )
                    {
                        //-- กำหนดให้ชื่อไฟล์คือ Site FID OGC Main Point
                        FID = Utility.Left(FileName, FileName.Length - FileType.Length);
                        //-- อนุโลมให้ใช้ชื่อ ดังนี้ได้ BTU sat BV10.xls
                        FID = FID.Replace("BTU sat ", "").Trim();

                        //-- 19/12/2018 -- บางทีชื่อไฟล์เป็นตัวพิมพ์เล็กพิมพ์ใหญ่ ไม่ตรงกับ FID เลยต้องดึงจาก DB
                        DataTable DTFID = Project.dal.SearchSiteFID(FID: FID);
                        FID = Utility.ToString(DTFID.Rows[0]["FID"]);


                        //Upload สำเร็จ -- จัดเก็บเข้า tmp Database
                        Exc = new MngExcel();
                        FullFileName = Server.MapPath(Project.gExcelPath + "Upload/" + FileName);

                        if ( FileType == ".csv")
                            DT1 = Exc.GetDataTableFromCSVFile(FullFileName, startRow:7, headerRow: -1);
                        else
                            DT1 = Exc.ReadWorksheet(FullFileName);


                        if ((DT1 == null) || DT1.Rows.Count < 8)
                            {
                                Msg = " - Data not found!";

                            }
                            else {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref Msg);
                                if (FormatExcel > 0)
                                {
                                    //-- delete tmp data
                                    Project.dal.MngTmpDailyBtu(DBUTIL.opDELETE, FID, "", "", "");

                                //-- มี 3 รูปแบบ -------------------------------------------------------------------------
                                //แบบที่ 1 _______________________________________________________________________________
                                //Column=>	A		            B		    C		    D		    E		F
                                //บรรทัด1		Current value:				
                                //บรรทัด2		2/4/2561 0:00:00	992.8182	985.21		999.5786	137
                                //บรรทัด3		Archive values:				
                                //บรรทัด4		1/4/2561 0:00:00	998.0324	993.0931	1008.51		360

                                //แบบที่ 2 _______________________________________________________________________________
                                //Column=>	A		B		            C		    D		    E		    F
                                //บรรทัด1				Current value:				
                                //บรรทัด2				2/4/2561 0:00:00	992.8182	985.21      999.5786	137
                                //บรรทัด3				Archive values:				
                                //บรรทัด4		1		1/4/2561 0:00:00	998.0324	993.0931	1008.51		360
                                //*** กรณีแบบที่ 2 ตัวอย่าง excel จะไม่อ่านบรรทัดที่ 1-3 จะเริ่มที่บรรทัด 4 เป็น record แรก เลย จึงต้องตรวจสอบให้ดี

                                //แบบที่ 3 เป็นไฟล์ .csv _______________________________________________________________________________
                                //Column=>	A		        B		    C		    D		    E		    
                                //บรรทัด1    ------------------------------------------------------------------------------------------				
                                //บรรทัด2    	Averages - 30/8/2561 15:18:38 				
                                //บรรทัด3					
                                //บรรทัด4	    Date/Time	Average	Min	Max	Samples
                                //บรรทัด5					
                                //บรรทัด6    	24 Hour Average 29 - Heating Value Gross BTU Sat. <Sample gas>				
                                //บรรทัด7					
                                //บรรทัด8	    30/8/2561 0:00	996.8864	996.7814	997.0457	360


                                if (FormatExcel == 2)
                                { colBTUDATE = 1; colBTU = 2; colRUN = 5; }
                                else
                                { colBTUDATE = 0; colBTU = 1; colRUN = 4; } //FormatExcel ==1,3


                                Boolean canRead = false;
                                //2) บันทึกใน tmp table
                                foreach (DataRow DR1 in DT1.Rows)
                                {
                                    pBTUDATE = ""; pBTU = ""; pRUN = "";

                                    //บรรทัด1		Current value:	  ให้ผ่านเลย
                                    //บรรทัด2		2/4/2561 0:00:00	992.8182	985.21		999.5786	137 	 เป็นค่าระหว่างวัน ให้ผ่านเลย
                                    //บรรทัด3		Archive values:	  ให้ผ่านเลย
                                    //*** กรณีแบบที่ 2 ตัวอย่าง excel จะไม่อ่านบรรทัดที่ 1-3 จะเริ่มที่บรรทัด 4 เป็น record แรก เลย จึงต้องตรวจสอบให้ดี
                                    canRead = false;
                                    ////if ( canRead == false )
                                    ////{
                                        switch ( FormatExcel )
                                        {
                                            case 1:
                                            if (ExRow1 > 2 && Utility.ToString(DR1[colBTUDATE]) != "")
                                            {
                                                Value = Utility.ToString(DR1[colBTUDATE]) + "";
                                                if (Value.IndexOf("/") > -1)
                                                    canRead = true;
                                                else
                                                    canRead = false;

                                            }
                                                break;
                                            case 2:
                                            //-- EDIT 28/10/2019 -- บางทีก็อ่านบรรทัดที่ 1-3 เลยต้องตรวจสอบ คอลัมน์แรกว่าเป็นตัวเลขหรือเปล่า
                                           if (Utility.ToString(DR1[0]) != "" && Utility.ToString(DR1[colBTUDATE]) != "")
                                            {
                                                Value = Utility.ToString(DR1[colBTUDATE]) + "";
                                                if (Value.IndexOf("/") > -1)
                                                    canRead = true;
                                                else
                                                    canRead = false;

                                            }
                                            break;
                                            case 3:
                                                if (Utility.ToString(DR1[colBTUDATE]) != "")
                                            {
                                                Value = Utility.ToString(DR1[colBTUDATE]) + "";
                                                if (Value.IndexOf("/") > -1)
                                                    canRead = true;
                                                else
                                                    canRead = false;

                                            }
                                            break;
                                        }
                                        
                                    ////}


                                    if (canRead)
                                    {

                                            //3) ตรวจสอบความถูกต้องของข้อมูล
                                            //-- column BTU Date ------------------------
                                            Value = Utility.ToString(DR1[colBTUDATE]) + "";

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
                                            //--- time --- //-- EDIT 18/09/2019 --->? Value = 10-มี.ค.-19 มีความยาว 11
                                            if (Value.Length > 11)
                                                {
                                                    tt = Value.Split(' ')[1];  //0:00 , 0:00:00
                                                    if ( tt.Split(':')[0].Length == 1 ) tt = "0" + tt; //0:00
                                                    tt = " " + tt;
                                                }

                                                if (Project.gXLSDateFormat == "mm/dd/yyyy" && FormatExcel != 3)
                                                {
                                                    ChkDate = Value.Split('/')[1].PadLeft(2, '0') + "/" + Value.Split('/')[0].PadLeft(2, '0') + "/" + yyyy + tt;
                                                }
                                                else {
                                                    ChkDate = Value.Split('/')[0].PadLeft(2, '0') + "/" + Value.Split('/')[1].PadLeft(2, '0') + "/" + yyyy + tt;
                                                }

                                                if (Utility.IsDate(ChkDate))
                                                {
                                                    //if (ChkDate.Length > 0) ChkDate = Utility.Left(ChkDate, 10).Trim(); //-- บันทึกเวลาด้วย
                                                    pBTUDATE = ChkDate;
                                                }
                                                else {

                                                    Msg += " - Row: " + ExRow1 + " - Invalid format DATE  (" + Value + ")</br>";
                                                    break;
                                                }


                                            }
                                            //-- column BTU ------------------------
                                            Value = Utility.ToString(DR1[colBTU]) + "";
                                            if (Utility.IsNumeric(Value))
                                            {
                                                pBTU = Value;
                                            }
                                            else {
                                                Msg += " - Row: " + ExRow1 + " - Invalid BTU  (" + Value + ")</br>";
                                                break;
                                            }

                                            //-- column RUN ------------------------
                                            Value = Utility.ToString(DR1[colRUN]) + "";
                                            if (Utility.IsNumeric(Value))
                                            {
                                                pRUN = Value;
                                            }
                                            else {
                                                Msg += " - Row: " + ExRow1 + " - Invalid RUN  (" + Value + ")</br>";
                                                break;
                                            }

                                            //4) บันทึกลง tmp table 
                                            if (pBTUDATE != "" && pBTU != "" & pRUN != "")
                                            {
                                                //Value = Utility.ToString(DR1[colBTUDATE]) + "";
                                                //-- มีบางวันที่ส่งมาซ้ำ 6/7/2560 9:38:30
                                                //                6/7/2560 0:00:00
                                                // ให้เลือกเฉพาะวันที่เวลาเป็น 0:00:00
                                                //Value = Utility.ToString(DR1[colBTUDATE]) + "";
                                                //if ( Utility.Right(Value,7) == "0:00:00")
                                                Project.dal.MngTmpDailyBtu(DBUTIL.opINSERT, FID, pBTUDATE, pBTU, pRUN);
                                            }
                                        }

                                        ExRow1++;

                                    }

                                    if (Msg == "")
                                    {
                                        //5) บันทึกลง O_OGC_DAILY_BTU
                                        Project.dal.MngTmp2OgcDailyBtu(FID);
                                        MsgSuccess = "Successfully upload [" + FileName + "] </br>";
                                    }


                                }
                                else
                                {
                                    Msg = " - The file template is invalid!";
                                }
                            }

                        }


                    }
                    else {
                        Msg = " - Please select the excel file! (" + Project.gExcelFileType.Replace("|", " ") + ") </br>";
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



        private string UploadUserFile(ref HttpPostedFile FileUpload)
        {
            String FullFileName = "", FileName = "", FileType = "";
            String FID = "";
            try
            {
                FileType = Utility.GetFileType(FileUpload.FileName).ToLower();
                FileName = Utility.GetFileName(FileUpload.FileName);
                //-- ตรวจสอบว่าชื่อไฟล์ เป็นชื่อ FID หรือไม่
                FID = Utility.Left(FileName, FileName.Length - FileType.Length);
                //-- อนุโลมให้ใช้ชื่อ ดังนี้ได้ BTU sat BV10.xls
                FID = FID.Replace("BTU sat ", "").Trim();

                DataTable DT = Project.dal.SearchSiteFID(FID:FID);
                if ( DT != null && DT.Rows.Count > 0 )
                {
                    if (FileUpload.ContentLength > 10485760)
                    {
                        Msg = " - The file size exceeds the limit allowed (10MB) and cannot be saved! </br>";
                    }
                    else
                    {
                        FullFileName = Server.MapPath(Project.gExcelPath + "Upload/" + FileName);

                        FileUpload.SaveAs(FullFileName);
                    }
                }
                else
                {
                    Msg = " - The file name have to be the same as OGC Main Point! </br>";
                }


                 
                return FileName;
            }
            catch (Exception ex)
            {

                Msg = Utility.GetErrorMessage(ex);
                return "";
            }
        }


        //-- จะส่งรูปแบบของ excel ไปว่าเป็นแบบ 1 หรือ 2 หรือ 3 ถ้าไม่ใช่ส่งเป็น 0
        private int CheckFormat(DataTable DT, ref string StrFormat)
        {
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;
            try
            {
                //-- มี 3 รูปแบบ -------------------------------------------------------------------------
                //แบบที่ 1 _______________________________________________________________________________
                //Column=>	A		            B		    C		    D		    E		F
                //บรรทัด1		Current Value				
                //บรรทัด2		2/4/2561 0:00:00	992.8182	985.21		999.5786	137
                //บรรทัด3		Archive Values				
                //บรรทัด4		1/4/2561 0:00:00	998.0324	993.0931	1008.51		360

                //แบบที่ 2 _______________________________________________________________________________
                //Column=>	A		B		            C		    D		    E		    F
                //บรรทัด1				Current value:				
                //บรรทัด2				2/4/2561 0:00:00	992.8182	985.21      999.5786	137
                //บรรทัด3				Archive values:				
                //บรรทัด4		1		1/4/2561 0:00:00	998.0324	993.0931	1008.51		360
                //*** กรณีแบบที่ 2 ตัวอย่าง excel จะไม่อ่านบรรทัดที่ 1-3 จะเริ่มที่บรรทัด 4 เป็น record แรก เลย จึงต้องตรวจสอบให้ดี

                //แบบที่ 3 เป็นไฟล์ .csv _______________________________________________________________________________
                //Column=>	A		        B		    C		    D		    E		    
                //บรรทัด1    ------------------------------------------------------------------------------------------				
                //บรรทัด2    	Averages - 30/8/2561 15:18:38 				
                //บรรทัด3					
                //บรรทัด4	    Date/Time	Average	Min	Max	Samples
                //บรรทัด5					
                //บรรทัด6    	24 Hour Average 29 - Heating Value Gross BTU Sat. <Sample gas>				
                //บรรทัด7					
                //บรรทัด8	    30/8/2561 0:00	996.8864	996.7814	997.0457	360
                //** กรณีแบบที่ 3 อ่าน .csv ให้เพิ่มจากข้อมูลเลย บรรทัดที่ 8 ป็น record แรก เลย

                if (DT.Rows.Count > 8)
                {
                    foreach (DataRow DR in DT.Rows)
                    {
                        switch (rowIndex)
                        {
                            case 0://-- HEADER ROW=0  ตรวจบรรทัด 1 Current value:  ถ้าไม่ใช่ให้ถือว่าเป็น ChkFormat = 3 ก่อน แล้วค่อยตรวจสอบ rowIndex=8 
                                data = Utility.ToString(GetCellData(DR, 0)).ToLower().Trim();
                                if (data == "current value:" || data == "current value")
                                {
                                    ChkFormat = 1;
                                }
                                else
                                {
                                    if (data == "1")
                                    {
                                        ChkFormat = 2;
                                    }
                                    else
                                    {
                                        data = Utility.ToString(GetCellData(DR, 1)).ToLower().Trim();
                                        if (data == "current value:" || data == "current value")
                                        {
                                            ChkFormat = 2;
                                        }
                                        else
                                        {
                                            //-- HEADER ROW=7  ตรวจบรรทัด 8  ถ้าเป็นแบบที่ 1 คอลัมน์แรกต้องเป็นวันที่ 
                                            data = Utility.ToString(GetCellData(DR, 0)).Trim();
                                            if (Utility.IsExcelDate(data))
                                                ChkFormat = 3;
                                            else
                                                ChkFormat = 0;
                                        }
                                    }
                                }
                                break;
                            case 1://-- HEADER ROW=1  ตรวจบรรทัด 2  ถ้าเป็นแบบที่ 1 คอลัมน์แรกต้องเป็นวันที่  ถ้าเป็นแบบที่ 2 คอลัมน์ที่ 2 ต้องเป็นวันที่
                                if (ChkFormat == 1)
                                {
                                    data = Utility.ToString(GetCellData(DR, 0)).Trim();
                                    if (!Utility.IsExcelDate(data))
                                    {
                                        ChkFormat = 0;
                                    }
                                }
                                else
                                {
                                    if (ChkFormat == 2)
                                    {
                                        data = Utility.ToString(GetCellData(DR, 1)).Trim();
                                        if (!Utility.IsExcelDate(data))
                                        {
                                            ChkFormat = 0;
                                        }
                                    }
                                }
                                break;
                            case 2://-- HEADER ROW=2  ตรวจบรรทัด 3  Archive values: ถ้าเป็นแบบที่ 1 คอลัมน์แรก   ถ้าเป็นแบบที่ 2 คอลัมน์ที่ 2  
                                if (ChkFormat == 1)
                                {
                                    data = Utility.ToString(GetCellData(DR, 0)).ToLower().Trim();
                                    if (data != "archive values:" && data != "archive values")
                                    {
                                        ChkFormat = 0;
                                    }
                                }
                                else
                                {
                                    if (ChkFormat == 2)
                                    {
                                        data = Utility.ToString(GetCellData(DR, 1)).ToLower().Trim();
                                        if (data != "archive values:" && data != "archive values" && !Utility.IsExcelDate(data))
                                        {
                                            ChkFormat = 0;
                                        }
                                    }
                                }
                                break;
                            case 3://-- HEADER ROW=3  ตรวจบรรทัด 4  ถ้าเป็นแบบที่ 1 คอลัมน์แรกต้องเป็นวันที่  ถ้าเป็นแบบที่ 2 คอลัมน์ที่ 2 ต้องเป็นวันที่
                                if (ChkFormat == 1)
                                {
                                    data = Utility.ToString(GetCellData(DR, 0)).Trim();
                                    if (!Utility.IsExcelDate(data))
                                    {
                                        ChkFormat = 0;
                                    }
                                }
                                else
                                {
                                    if (ChkFormat == 2)
                                    {
                                        data = Utility.ToString(GetCellData(DR, 1)).Trim();
                                        if (!Utility.IsExcelDate(data))
                                        {
                                            ChkFormat = 0;
                                        }
                                    }
                               }
                                break;
                            //case 4:
                            //    break;
                            //case 5:
                            //    break;
                            //case 6:
                            //    break;
                            //case 7: //-- HEADER ROW=7  ตรวจบรรทัด 8  ถ้าเป็นแบบที่ 1 คอลัมน์แรกต้องเป็นวันที่ 
                            //    if (ChkFormat == 3)
                            //    {
                            //        data = Utility.ToString(GetCellData(DR, 0)).Trim();
                            //        if (!Utility.IsExcelDate(data))
                            //        {
                            //            ChkFormat = 0;
                            //        }
                            //    }
                            //    break;

                        }



                        rowIndex++;
                        if (rowIndex > 4 || ChkFormat == 0) break;
                    }
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

        


    }
}