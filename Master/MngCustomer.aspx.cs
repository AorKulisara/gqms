using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 21/07/2023 ---

    public partial class MngCustomer : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public bool canEdit;
        public bool canDelete;
        public bool canAdd;
        String MsgSuccess = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDCustomer, true);
                SetCtrl();

                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();
                    ServerAction = Validation.GetParamStr("ServerAction", DefaultVal: "LOAD");
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");

                }

                switch (ServerAction)
                {
                    case "LOAD": break;
                    case "SEARCH": LoadData();
                        break;
                    case "EXPORT_XLS":
                        //-- ส่ง session ไป เนื่องจากภาษาไทยเพี้ยน
                        Session["custName"] = Utility.FormatSearchData(Validation.GetCtrlStr(txtNAME));
                        String regionList = "", subTypeList = "", qualityMainList = "", OtherCri = "";
                        regionList = Utility.GetListBoxValue(ddlREGION);
                        if (regionList != "")
                        {
                            regionList = regionList.Replace(",", "','");
                            OtherCri = " REGION IN ('" + regionList + "') ";
                        }
                        subTypeList = Utility.GetListBoxValue(ddlSUB_TYPE);
                        if (subTypeList != "")
                        {
                            subTypeList = subTypeList.Replace(",", "','");
                            if (OtherCri != "") OtherCri += " AND ";
                            OtherCri += " SUB_TYPE IN ('" + subTypeList + "') ";
                        }
                        qualityMainList = Utility.GetListBoxValue(ddlQUALITY_MAIN);
                        if (qualityMainList != "")
                        {
                            qualityMainList = qualityMainList.Replace(",", "','");
                            if (OtherCri != "") OtherCri += " AND ";
                            OtherCri += " QUALITY_MAIN IN ('" + qualityMainList + "') ";
                        }
                        Session["OtherCri"] = OtherCri;

                        PageAction = "Result('EC','ExcelCustomer');";
                        break;

                    //-- edit 29/05/2024 --
                    case "IMPORT_XLS":
                        pnlFILE.Visible = (pnlFILE.Visible) ? false : true;  // toggle
                        break;
                    case "SAVE_XLS":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            ImportData();
                            pnlFILE.Visible = false;  // toggle
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

        private void SetCtrl()
        {
            try
            {
                canAdd = Security.CanDo(Security.TaskMDCustomer, Security.actAdd);
                canEdit = canAdd;
                canDelete = canAdd;

                //-- EDIT 04/08/2023 -- ไม่ต้องเพิ่ม เพราะดึงข้อมูลจากระบบ GIS
                //pnlADD.Visible = (canAdd) ? true : false;
                pnlADD.Visible = false;

                //-- EDIT 30/05/2024 --  
                pnlIMPORT.Visible = (canAdd) ? true : false;
                 
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void InitCtrl()
        {
            DataTable DT = new DataTable();
            try
            {
                DT = Project.dal.SearchCustomerRegion();
                Utility.LoadList(ref ddlREGION, DT, "REGION", "REGION");

                DT = Project.dal.SearchCustomerSubtype();
                Utility.LoadList(ref ddlSUB_TYPE, DT, "SUB_TYPE", "SUB_TYPE");

                DT = Project.dal.SearchCustomerQualityMain();
                Utility.LoadList(ref ddlQUALITY_MAIN, DT, "QUALITY_MAIN", "QUALITY_MAIN");

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
            }
        }

        private void LoadData()
        {
            DataTable DT;
            String custName = "";
            String regionList = "", subTypeList = "", qualityMainList = "", OtherCri = "";
            try
            {
                custName = Utility.FormatSearchData(Validation.GetCtrlStr(txtNAME));
                regionList = Utility.GetListBoxValue(ddlREGION);
                if (regionList != "")
                {
                    regionList = regionList.Replace(",", "','");
                    OtherCri = " REGION IN ('" + regionList + "') ";
                }
                subTypeList = Utility.GetListBoxValue(ddlSUB_TYPE);
                if (subTypeList != "")
                {
                    subTypeList = subTypeList.Replace(",", "','");
                    if (OtherCri != "") OtherCri += " AND ";
                    OtherCri += " SUB_TYPE IN ('" + subTypeList +"') ";
                }
                qualityMainList = Utility.GetListBoxValue(ddlQUALITY_MAIN);
                if (qualityMainList != "")
                {
                    qualityMainList = qualityMainList.Replace(",", "','");
                    if (OtherCri != "") OtherCri += " AND ";
                    OtherCri += " QUALITY_MAIN IN ('" + qualityMainList + "') ";
                }

                //-- EDIT 28/05/2024 --
                String FromDate = Validation.GetCtrlStr(txtDateFrom);
                String ToDate = Validation.GetCtrlStr(txtDateTo);
                if (FromDate != "" )
                {
                    if (ToDate == "") ToDate = FromDate;
                    Project.dal.AddCriteriaRange(ref OtherCri, "CREATED_DATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);
                }
                String FromDate2 = Validation.GetCtrlStr(txtDateFrom2);
                String ToDate2 = Validation.GetCtrlStr(txtDateTo2);
                if (FromDate2 != "" )
                {
                    if (ToDate2 == "") ToDate2 = FromDate2;
                    Project.dal.AddCriteriaRange(ref OtherCri, "MODIFIED_DATE", Utility.AppDateValue(FromDate2), Utility.AppDateValue(ToDate2), DBUTIL.FieldTypes.ftDate);
                }

                //-- ค้นหา region จากต้นทาง 
                DT = Project.dal.SearchCustomer("", "", custName, "", OtherCriteria: OtherCri);

                Session["DT"] = DT;
                Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }


        protected void gvResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable DT = (DataTable)Session["DT"];

            if (DT != null)
            {
                DataView DV = new DataView(DT);
                if (Utility.ToString(this.ViewState["sortExpression"]) == "" || Utility.ToString(this.ViewState["sortExpression"]) == "DESC")
                {
                    this.ViewState["sortExpression"] = "ASC";
                }
                else
                {
                    this.ViewState["sortExpression"] = "DESC";
                }

                DV.Sort = e.SortExpression + " " + Utility.ToString(this.ViewState["sortExpression"]);
                Session["DT"] = DV.ToTable();
                gvResult.PageIndex = 0;
                Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);

            }


        }


        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    e.Row.Attributes.Add("onclick", "javascript:DoAction('SELECT','" + Validation.EncodeParam(Utility.ToString(dr["PERMANENT_CODE"])) + "');");
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

            int colPERMANENT_CODE = -1, colNAME_ABBR = -1, colNAME_FULL = -1, colQUALITY_MAIN = -1, colQUALITY_SUPPORT1 = -1, colQUALITY_SUPPORT2 = -1;
            int colOMA_MAIN = -1, colOMA_SUPPORT1 = -1, colH2S = -1, colHG = -1, colREMARK = -1;

            object pPERMANENT_CODE = null, pNAME_ABBR = null, pNAME_FULL = null, pQUALITY_MAIN = null, pQUALITY_SUPPORT1 = null, pQUALITY_SUPPORT2 = null;
            object pOMA_MAIN = null, pOMA_SUPPORT1 = null, pH2S = null, pHG = null, pREMARK = null;

            try
            {

                Project.ReadConfiguration();
                Msg = "";
                if (FileImportData.HasFile)
                {

                    FileType = (Utility.GetFileType(FileImportData.FileName) + "").ToLower();
                    FileName = Utility.GetFileName(FileImportData.FileName);

                    String gExcelFile = "|" + Project.gExcelFileType + "|";
                    if (gExcelFile.IndexOf("|" + FileType + "|") > 0)
                    {
                        UploadUserFile(ref FileImportData);

                        if (Msg == "")
                        {

                            //Upload สำเร็จ -- จัดเก็บเข้า tmp Database
                            Exc = new MngExcel();
                            FullFileName = Server.MapPath(Project.gExcelPath + "Import/" + FileName);

                            DT1 = Exc.ReadWorksheet(FullFileName);
                            if ((DT1 == null) || DT1.Rows.Count < 4)
                            {
                                Msg = " - Data not found!";

                            }
                            else
                            {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref ExMaxCol1, ref ExRow1, ref colPERMANENT_CODE, ref colNAME_ABBR, ref colNAME_FULL, ref colQUALITY_MAIN, ref colQUALITY_SUPPORT1, ref colQUALITY_SUPPORT2, ref colOMA_MAIN, ref colOMA_SUPPORT1, ref colH2S, ref colHG, ref colREMARK);
                                if (FormatExcel > 0)
                                {
                                    
                                    //-- delete tmp data
                                    Project.dal.MngTmpCustomerXLS(DBUTIL.opDELETE, "");

                                    //2) บันทึกใน tmp table
                                    foreach (DataRow DR1 in DT1.Rows)
                                    {
                                        if (ExRow1 > ExHeadRow1) //-- ข้อมูลจะเริ่มจากบรรทัด 1
                                        {

                                            pPERMANENT_CODE = null; pNAME_ABBR = null; pNAME_FULL = null; pQUALITY_MAIN = null; pQUALITY_SUPPORT1 = null; pQUALITY_SUPPORT2 = null;
                                            pOMA_MAIN = null; pOMA_SUPPORT1 = null; pH2S = null; pHG = null; pREMARK = null;
  
                                            if (colPERMANENT_CODE >= 0) pPERMANENT_CODE = DR1[colPERMANENT_CODE];
                                            if (colNAME_ABBR > 0) pNAME_ABBR = DR1[colNAME_ABBR];
                                            if (colQUALITY_MAIN > 0) pQUALITY_MAIN = DR1[colQUALITY_MAIN];
                                            if (colQUALITY_SUPPORT1 > 0) pQUALITY_SUPPORT1 = DR1[colQUALITY_SUPPORT1];
                                            if (colQUALITY_SUPPORT2 > 0) pQUALITY_SUPPORT2 = DR1[colQUALITY_SUPPORT2];
                                            if (colOMA_MAIN > 0) pOMA_MAIN = DR1[colOMA_MAIN];
                                            if (colOMA_SUPPORT1 > 0) pOMA_SUPPORT1 = DR1[colOMA_SUPPORT1];
                                            if (colH2S > 0) pH2S = DR1[colH2S];
                                            if (colHG > 0) pHG = DR1[colHG];
                                            if (colREMARK > 0) pREMARK = DR1[colREMARK];

                                            Project.dal.MngTmpCustomerXLS(DBUTIL.opINSERT, pPERMANENT_CODE, pNAME_ABBR, pNAME_FULL, QualityMain: pQUALITY_MAIN, QualitySupport1: pQUALITY_SUPPORT1, QualitySupport2: pQUALITY_SUPPORT2, omaMain: pOMA_MAIN, omaSupport1: pOMA_SUPPORT1, h2s: pH2S, hg: pHG, remark: pREMARK);
                                           
                                        }

                                        ExRow1++;
                                    }


                                    if (Msg == "")
                                    {
                                       
                                        //-- ตรวจสอบ ดึงข้อมูลต่างๆจาก OGC Main Point ดูจาก Quaility Main Point
                                        string sql = " MERGE INTO O_TMP_CUSTOMER_XLS T USING ( SELECT * FROM O_SITE_FID ) F ON(T.QUALITY_MAIN=F.FID) " +
                                        " WHEN MATCHED THEN UPDATE SET " +
                                        " T.QUALITY_SUPPORT1=CASE WHEN F.OGC_NAME1='LAST_GOOD' THEN F.OGC_NAME2 ELSE F.OGC_NAME1 END, " +
                                        " T.QUALITY_SUPPORT2=CASE WHEN F.OGC_NAME1='LAST_GOOD' THEN F.OGC_NAME3 ELSE F.OGC_NAME2 END, " +
                                        " T.OMA_MAIN=F.OMA_NAME1, T.OMA_SUPPORT1=F.OMA_NAME2, " +
                                        " T.H2S=F.H2S_NAME1, T.HG=F.HG_NAME1, " +
                                        " T.CREATED_DATE=SYSDATE, T.CREATED_BY='MAPPING' ";
                                        Project.dal.ExecuteSQL(sql);

                                        //-- บันทึกลง O_CUSTOMER 
                                        string user = System.Web.HttpContext.Current.Session["USER_NAME"] + "";
                                        sql = " MERGE INTO O_CUSTOMER  C USING ( SELECT * FROM O_TMP_CUSTOMER_XLS ) F ON(C.PERMANENT_CODE=F.PERMANENT_CODE ) " +
                                        " WHEN MATCHED THEN UPDATE SET " +
                                        " C.QUALITY_MAIN = F.QUALITY_MAIN, C.QUALITY_SUPPORT1=F.QUALITY_SUPPORT1, " +
                                        " C.QUALITY_SUPPORT2=F.QUALITY_SUPPORT2, C.OMA_MAIN=F.OMA_MAIN, " +
                                        " C.OMA_SUPPORT1=F.OMA_SUPPORT1, C.H2S=F.H2S, C.HG=F.HG, C.REMARK=F.REMARK, " +
                                        " C.MODIFIED_DATE=SYSDATE, C.MODIFIED_BY='" + user +"' ";
                                        Project.dal.ExecuteSQL(sql);

                                        MsgSuccess = "Successfully imported [" + FileName + "] </br>";
                                        //-- ต้องดึงข้อมูลที่เพิ่ง import excel
                                        string OtherCri = " PERMANENT_CODE IN  ( SELECT PERMANENT_CODE FROM O_TMP_CUSTOMER_XLS )";
                                        DataTable DT = Project.dal.SearchCustomer("", "", "", "", OtherCriteria: OtherCri);

                                        Session["DT"] = DT;
                                        Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);
                                        Utility.ClearObject(ref DT);

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
                        Msg = " - Please select the excel file! (" + Project.gExcelFileType.Replace("|", " ") + ") </br>";
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
                if (Msg != "")
                {
                    if (Msg == "UploadXLS/Unexpected error from external database driver (22). </br>") Msg = " - Worksheet name must be less than 30 characters!";

                    Msg = "Fail to import [" + FileName + "] : </br>" + Msg;
                }
                else
               if (MsgSuccess != "")
                {
                    Msg = MsgSuccess;
                    MsgSuccess = "";
                }

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



        private int CheckFormat(DataTable DT, ref int ExMaxCol, ref int ExRow, ref int colPERMANENT_CODE, ref int colNAME_ABBR, ref int colNAME_FULL, ref int colQUALITY_MAIN, ref int colQUALITY_SUPPORT1, ref int colQUALITY_SUPPORT2, ref int colOMA_MAIN, ref int colOMA_SUPPORT1, ref int colH2S, ref int colHG, ref int colREMARK)
        {
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;
            int maxCol = 17;  //มี 18 คอลัมน์ แต่เริ่มจาก 0 

            try
            {
                //-- รูปแบบ  _______________________________________________________________________________
                //บรรทัด1    ID	Short Name	Customer Name	Customer Type	Region	BV Zoning	Block Valve	Status	OGC Main Point	OGC Support Point1	OGC Support Point2	OMA Main Point	OMA Support Point1	H2S Main Point	Hg	Remark	Created Date	Updated Date
                //Column=>  A	    B       C	            D	            E	    F	        G	        H	    I	                J	                    K	                    L	            M	                N   O	 P	    Q	            R	 

                //--- การอ่านไฟล์ ถ้าคอลัมน์แรกไม่มีข้อมูล ระบบจะไม่อ่าน record นั้น ดังนั้นจึงทำให้บรรทัดเลื่อนได้

                if (maxCol > DT.Columns.Count - 1) maxCol = DT.Columns.Count - 1;

                foreach (DataRow DR in DT.Rows)
                {
                    ExMaxCol = maxCol;
                    //-- HEADER ROW1
                    for (int col = 0; col < ExMaxCol; col++)
                    {
                        data = Utility.ToString(GetCellData(DR, col)).ToUpper().Trim().Replace(" ", "");
                        switch (data)
                        {
                            case "ID": colPERMANENT_CODE = col; break;
                            case "SHORTNAME": colNAME_ABBR = col; break;
                            case "CUSTOMERNAME": colNAME_FULL = col; break;
                            case "OGCMAINPOINT": colQUALITY_MAIN = col; break;
                            case "OGCSUPPORTPOINT1": colQUALITY_SUPPORT1 = col; break;
                            case "OGCSUPPORTPOINT2": colQUALITY_SUPPORT2 = col; break;
                            case "OMAMAINPOINT": colOMA_MAIN = col; break;
                            case "OMASUPPORTPOINT1": colOMA_SUPPORT1 = col; break;
                            case "H2SMAINPOINT": colH2S = col; break;
                            case "HGMAINPOINT": colHG = col; break;
                            case "REMARK": colREMARK = col; break;

                        }

                    }

                    if (colPERMANENT_CODE > -1 && colQUALITY_MAIN > -1)
                    {
                        ChkFormat = 1;
                        ExRow = rowIndex;
                        break;
                    }
                    else
                    {
                        ChkFormat = 0;
                        ExRow = rowIndex;
                    }


                    rowIndex++;
                    if (rowIndex > 3) break;
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