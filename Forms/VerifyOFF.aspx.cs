using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;

namespace PTT.GQMS.USL.Web.Forms
{
    //--  edit 26/06/2019 ---

    public partial class VerifyOFF : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public Int32 chkCount = 0;
        String MsgSuccess = "";

        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;

        String fromDate = "", toDate = "";
        int colSUM = 12; //-- ตัวแปรคอลัมน์ summary เริ่มนับจาก 0

        Boolean LoadSessionFlag = false; //ตัวแปรกำหนดให้ load data จาก session
        String gSessionID = ""; // ตัวแปรเก็บ sessionid เพื่อให้สามารถเปิดได้หลายหน้า formt: siteid_yyyy_mm

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskVerify, true);
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

                    if ( ServerAction != "SEARCH")
                    {
                        Utility.SetCtrl(hidSITE_ID, ddlFID.SelectedValue);
                        Utility.SetCtrl(hidFID, ddlFID.SelectedItem.Text);
                        Utility.SetCtrl(hidMM, ddlMONTH.SelectedValue);
                        Utility.SetCtrl(hidYY, ddlYEAR.SelectedValue);

                        gSessionID = "_"+ddlFID.SelectedValue+"_"+ ddlYEAR.SelectedValue+ "_"+ ddlMONTH.SelectedValue; // ตัวแปรเก็บ sessionid เพื่อให้สามารถเปิดได้หลายหน้า formt: siteid_yyyy_mm
                    }

                }

                switch (ServerAction)
                {
                    case "LOAD": break;  //--- ตอนเรียกหน้าจอครั้งแรก ยังไม่ต้องแสดงข้อมูล เนื่องจากใช้เวลานาน
                    case "SEARCH":

                        Utility.SetCtrl(hidSITE_ID, ddlFID.SelectedValue);
                        Utility.SetCtrl(hidFID, ddlFID.SelectedItem.Text);
                        Utility.SetCtrl(hidMM, ddlMONTH.SelectedValue);
                        Utility.SetCtrl(hidYY, ddlYEAR.SelectedValue);
                        gSessionID = "_"+ddlFID.SelectedValue + "_" + ddlYEAR.SelectedValue + "_" + ddlMONTH.SelectedValue; // ตัวแปรเก็บ sessionid เพื่อให้สามารถเปิดได้หลายหน้า formt: siteid_yyyy_mm

                        //-- EDIT 28/08/2020 -- ดึงข้อมูล As Found+As Left+Final Cal จากระบบ OGC Data
                        LoadCalibrateData();

                        LoadData();
                        break;



                    case "IMPORT_XLS":
                        LoadDataSession();  //ถ้าไม่สั่ง load session ใหม่ ส่วน footer จะหาย

                    
                        pnlFILE.Visible = (pnlFILE.Visible) ? false : true;  // toggle
                        break;
                    case "SAVE_XLS":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            ImportData();
                            

                            if (Msg == "")
                            {
                                if (MsgSuccess != "") Msg = MsgSuccess;
                                pnlFILE.Visible = false;  // toggle
                                LoadData();
                            }
                           
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
                //-- กำหนดให้มี 2 คอลัมน์คือ Read และ Add/Edit/Delete 
                canEdit = Security.CanDo(Security.TaskVerify, Security.actAdd);
                canDelete = canEdit;
                canAdd = canEdit;

                pnlIMPORT.Visible = (canEdit) ? true : false;
 

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
                DT = Project.dal.SearchSiteOffshoreFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                Utility.LoadMonthCombo(ref ddlMONTH);
                Utility.LoadYearCombo(ref ddlYEAR, "2018");

                DateTime today = System.DateTime.Today;
                if ( today.Day < 6 ) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        Utility.SetCtrl(ddlMONTH, "12");
                        Utility.SetCtrl(ddlYEAR, (today.Year - 1).ToString());
                    }
                    else
                    {
                        Utility.SetCtrl(ddlMONTH, (today.Month-1).ToString());
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
                Utility.ClearObject(ref DT);
            }
        }


        private void LoadData()
        {
            DataTable DT = null;

            try
            {
 
                if (Validation.GetCtrlIntStr(ddlFID) != "" && Validation.GetCtrlIntStr(ddlMONTH) != "" && Validation.GetCtrlIntStr(ddlYEAR) != "")
                {
                    

                    fromDate = "01/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                    toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));

                    DT = Project.dal.SearchOffshoreDailyUpdate(Validation.GetCtrlIntStr(hidSITE_ID), "", fromDate, toDate);

                    if (DT != null && DT.Rows.Count > 0)
                    {
                        Session["OFFSHORE_DAILY_ALL"+ gSessionID] = DT;
                        chkCount = DT.Rows.Count;
                    }
                    else
                    {
                        chkCount = 0;
                    }

                    //---- Bound Data Table --------------------------------------------------------
                    Utility.BindGVData(ref gvResult, DT, false);
                    gvResult.FooterRow.Visible = true;
                    //---- Bound Data Table --------------------------------------------------------


                }


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


        private void LoadDataSession()
        {
            try
            {

                LoadSessionFlag = true; //ตัวแปรกำหนดให้ load data จาก session

                Utility.BindGVData(ref gvResult, (DataTable)Session["OFFSHORE_DAILY_ALL"+ gSessionID], false);
                gvResult.FooterRow.Visible = true;


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }
        }



        //-- EDIT 26/10/2020 -- ดึงข้อมูล As Found+As Left+Final Cal จากระบบ OGC Data
        private void LoadCalibrateData()
        {
            DataTable DT = null;

            try
            {
                divASFOUND.Visible = false;

                if (Validation.GetCtrlIntStr(ddlFID) != "" && Validation.GetCtrlIntStr(ddlMONTH) != "" && Validation.GetCtrlIntStr(ddlYEAR) != "")
                {
                    string sql = "SELECT S.OSITE_ID, A.*  " +
                    " FROM C_OFFSHORE_FID S LEFT OUTER JOIN " +
                    " (SELECT C.*FROM C_OFF_CALIBRATE C INNER JOIN C_OFFSHORE_FID S ON C.CSITE_ID = S.CSITE_ID " +
                    " WHERE C.MM =" + Validation.GetCtrlIntStr(ddlMONTH) + " AND C.YY =" + Validation.GetCtrlIntStr(ddlYEAR) + " AND S.OSITE_ID =" + Validation.GetCtrlIntStr(ddlFID) + ") A ON A.CSITE_ID = S.CSITE_ID " +
                    " WHERE S.OSITE_ID = " + Validation.GetCtrlIntStr(ddlFID) + " " +
                    " ORDER BY A.FOUND_DATE DESC ";

                    DT = Project.dal.QueryData(sql);
                    if (DT != null && DT.Rows.Count > 0) //ถ้ามี record หมายถึง มีการ mapping site เอาไว้
                    {
                        DataRow DR = Utility.GetDR(ref DT);
                        divASFOUND.Visible = true;
                        Utility.SetCtrl(lblFOUND_DATE, Utility.AppFormatDate(DR["FOUND_DATE"]));
                        Utility.SetCtrl(lblFOUND_STATUS, Utility.ToString(DR["FOUND_STATUS"]));
                        switch (Utility.ToString(DR["FOUND_STATUS"]))
                        {
                            case "FAIL":
                                lblFOUND_STATUS.CssClass = "cell-center cell-bg-nopass";
                                break;
                            case "PASS":
                                lblFOUND_STATUS.CssClass = "cell-center cell-bg-pass";
                                break;
                            default:
                                lblFOUND_STATUS.CssClass = "cell-center cell-bg-sum";
                                break;
                        }

                        Utility.SetCtrl(lblLEFT_DATE, Utility.AppFormatDate(DR["LEFT_DATE"]));
                        Utility.SetCtrl(lblLEFT_STATUS, Utility.ToString(DR["LEFT_STATUS"]));
                        switch (Utility.ToString(DR["LEFT_STATUS"]))
                        {
                            case "FAIL":
                                lblLEFT_STATUS.CssClass = "cell-center cell-bg-nopass";
                                break;
                            case "PASS":
                                lblLEFT_STATUS.CssClass = "cell-center cell-bg-pass";
                                break;
                            default:
                                lblLEFT_STATUS.CssClass = "cell-center cell-bg-sum";
                                break;
                        }

                        Utility.SetCtrl(lblCAL_DATE, Utility.AppFormatDate(DR["CAL_DATE"]));
                        Utility.SetCtrl(lblCAL_STATUS, Utility.ToString(DR["CAL_STATUS"]));
                        switch (Utility.ToString(DR["CAL_STATUS"]))
                        {
                            case "FAIL":
                                lblCAL_STATUS.CssClass = "cell-center cell-bg-nopass";
                                break;
                            case "PASS":
                                lblCAL_STATUS.CssClass = "cell-center cell-bg-pass";
                                break;
                            default:
                                lblCAL_STATUS.CssClass = "cell-center cell-bg-sum";
                                break;
                        }



                    }


                }


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


        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable aDTavg31 = null, aDTmin = null, aDTmax = null;
            DataRow aDR = null;
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {

                }
                else
                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                    //-- OGC column 1-17, SUM column 12
                    ShowValue(ref e, 1, 17, dr);

                }
                else
                if (e.Row.RowType == DataControlRowType.Footer)
                {

                    //Add row=> AVERAGE(end month) ==============================================================================================================
                    e.Row.CssClass = "ItemFooter_green2";
                    e.Row.Cells[0].Text = "AVERAGE";



                    if (LoadSessionFlag)
                        aDTavg31 = (DataTable)Session["OFFSHORE_AVG31" + gSessionID];
                    else
                    {
                        aDTavg31 = Project.dal.SearchOffshoreDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);
                        Session["OFFSHORE_AVG31" + gSessionID] = aDTavg31;
                    }

                    if (aDTavg31 != null && aDTavg31.Rows.Count > 0)
                    {
                        aDR = Utility.GetDR(ref aDTavg31);
                        //-- OGC column 1-17, SUM column 12
                        ShowFooterValue(ref e, 1, 17, aDR);
                    }
                    


                    //Add row=> MIN ==============================================================================================================
                    GridViewRow extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                    extraFooter.CssClass = "ItemFooter_blue";

                    TableCell cell = new TableCell();
                    cell.CssClass = "cell-center cell-Middle cell-border text-black";
                    cell.Text = "MIN";
                    extraFooter.Cells.Add(cell);

                    if (LoadSessionFlag)
                        aDTmin = (DataTable)Session["OFFSHORE_MIN" + gSessionID];
                    else
                    {
                        aDTmin = Project.dal.SearchOffshoreDailyUpdateMIN(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);
                        Session["OFFSHORE_MIN" + gSessionID] = aDTmin;
                    }

                    if (aDTmin != null && aDTmin.Rows.Count > 0)
                    {
                        aDR = Utility.GetDR(ref aDTmin);
                        //-- OGC column 1-17, SUM column 12
                        String fd = "", fdValue = "";
                        for (int c = 1; c <= 17; c++)
                        {
                            fd = ConfigCol(c);
                            if (fd != "")
                            {
                                switch (fd)
                                {
                                    case "SG":
                                    case "H2O":
                                    case "HG":
                                    case "H2S":
                                        fdValue = Utility.FormatNum(aDR[fd], 4);
                                        break;
                                    default:
                                        fdValue = Utility.FormatNum(aDR[fd], 3);
                                        break;
                                }
                            }
                            else
                            {
                                fdValue = "";
                            }

                            cell = new TableCell();
                            if (c == colSUM)
                            {
                                cell.CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                            }
                            else
                            {
                                cell.CssClass = "cell-right cell-Middle cell-border";
                            }
                            cell.Text = fdValue;
                            extraFooter.Cells.Add(cell);

                        }
                    }
                    else
                    {
                        //-- OGC column 1-17, SUM column 12
                        for (int c = 1; c <= 17; c++)
                        {
                            cell = new TableCell();
                            if (c == colSUM)
                            {
                                cell.CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                            }
                            else
                            {
                                cell.CssClass = "cell-right cell-Middle cell-border";
                            }
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }
                    }

                    gvResult.Controls[0].Controls.Add(extraFooter);


                    //Add row=> MAX ==============================================================================================================
                    extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                    extraFooter.CssClass = "ItemFooter_blue2";
                    cell = new TableCell();
                    cell.CssClass = "cell-center cell-Middle cell-border text-black";
                    cell.Text = "MAX";
                    extraFooter.Cells.Add(cell);

                    if (LoadSessionFlag)
                        aDTmax = (DataTable)Session["OFFSHORE_MAX" + gSessionID];
                    else
                    {
                        aDTmax = Project.dal.SearchOffshoreDailyUpdateMAX(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);
                        Session["OFFSHORE_MAX" + gSessionID] = aDTmax;
                    }

                    if (aDTmax != null && aDTmax.Rows.Count > 0)
                    {
                         
                        aDR = Utility.GetDR(ref aDTmax);
                        //-- OGC column 1-17, SUM column 12
                        String fd = "", fdValue = "";
                        for (int c = 1; c <= 17; c++)
                        {
                            fd = ConfigCol(c);
                            if (fd != "")
                            {
                                switch (fd)
                                {
                                    case "SG":
                                    case "H2O":
                                    case "HG":
                                    case "H2S":
                                        fdValue = Utility.FormatNum(aDR[fd], 4);
                                        break;
                                    default:
                                        fdValue = Utility.FormatNum(aDR[fd], 3);
                                        break;
                                }
                            }
                            else
                            {
                                fdValue = "";
                            }

                            cell = new TableCell();
                            if (c == colSUM)
                            {
                                cell.CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                            }
                            else
                            {
                                cell.CssClass = "cell-right cell-Middle cell-border";
                            }
                            cell.Text = fdValue;
                            extraFooter.Cells.Add(cell);

                        }
                    }
                    else
                    {
                        //-- OGC column 1-17, SUM column 12
                        for (int c = 1; c <= 17; c++)
                        {
                            cell = new TableCell();
                            if (c == colSUM)
                            {
                                cell.CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                            }
                            else
                            {
                                cell.CssClass = "cell-right cell-Middle cell-border";
                            }
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }
                    }

                    gvResult.Controls[0].Controls.Add(extraFooter);

              
                }   //e.Row.RowType == DataControlRowType.Footer


  


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref aDTavg31);
                Utility.ClearObject(ref aDTmin); Utility.ClearObject(ref aDTmax);
            }
        }

        protected void gvResult_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //cast the sender back to a gridview
            GridView gv = sender as GridView;

            //check if the row is the header row
            if (e.Row.RowType == DataControlRowType.Header)
            {

                //create a new row -------------------------------------------------------------------

                GridView HeaderGrid = (GridView)sender;

                GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
          

                TableCell cell = new TableCell();
                cell.Text = "DATE";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "CH4";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "C2H6";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "C3H8";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "IC4H10";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "NC4H10";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "IC5H12";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "NC5H12";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "C6H14";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "C7H16";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "CO2";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "N2";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);
                cell = new TableCell();

                cell.Text = "SUM";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "GHVSAT";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "SG";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-gray cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "H2O";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-primary cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "HG";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-success cell-center";
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Text = "H2S";
                cell.ColumnSpan = 1;
                cell.CssClass = "Table-head-orange cell-center";
                row.Cells.Add(cell);

                ///////////////////////////////////////////////////////////////////////////
 
                GridViewRow row2 = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                TableCell cell2 = new TableCell();
                cell2.Text = "";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-gray cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "mole%";
                cell2.ColumnSpan = 11;
                cell2.CssClass = "Table-head-gray cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-gray cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "Btu/scf";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-gray cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-gray cell-center";
                row2.Cells.Add(cell2);


                cell2 = new TableCell();
                cell2.Text = "lb/MMscf";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-primary cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "ug/m3";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-success cell-center";
                row2.Cells.Add(cell2);

                cell2 = new TableCell();
                cell2.Text = "ppm";
                cell2.ColumnSpan = 1;
                cell2.CssClass = "Table-head-orange cell-center";
                row2.Cells.Add(cell2);

                gvResult.Controls[0].Controls.AddAt(0, row);
                //-- edit 02/10/2019 -- ย้ายหน่วยไปอยู่บรรทัดที่ 2 ให้เหมือนกับ on shore ไม่อย่างนั้นจะเกิด error ตอนอ่าน datarow 
                gvResult.Controls[0].Controls.AddAt(0, row2);
               
            }
        }


        private void ShowValue(ref GridViewRowEventArgs gRow, int sCol, int eCol, DataRowView gDR)
        {
            String result = "";
            String fd = "";
            String AL = "";

            try
            {
                for (int c = sCol; c <= eCol; c++)
                {
                    result = "";
                    fd = ConfigCol(c);
                    if (fd != "")
                    {
                        if (Utility.ToString(gDR[fd]) != "")
                        {
                            //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                            if (Utility.IsNumeric(gDR[fd]))
                            {

                                switch (fd)
                                {
                                    case "SG":
                                    case "H2O":
                                    case "HG":
                                    case "H2S":
                                        result = Utility.FormatNum(gDR[fd], 4); break;
                                    case "SUM_COMPO":   //ค่า SUM คือ บวก CH4->H2S ต้องได้ 100(+-0.003)
                                        result = Utility.FormatNum(gDR[fd], 3);
                                          Double dVal = Utility.ToDouble(gDR[fd]);
                                        if (dVal < 99.997 || dVal > 100.003)
                                        {
                                            gRow.Row.Cells[c].CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                                         }
                                        break;
                                    default: result = Utility.FormatNum(gDR[fd], 3); break;
                                }

                            }
                            else
                            {  //-- ข้อมูลไม่ใช่ตัวเลข แสดงว่า error
                                result = Utility.ToString(gDR[fd]);
                                if (result != "")
                                {
                                    if (fd.IndexOf("DATE") < 0) gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning";

                                    if (fd.IndexOf("DATE") > 0) result = Utility.AppFormatDate(gDR[fd]);

                                }
                            }
                        }
                    }

                    gRow.Row.Cells[c].Text = result;

                }

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }


        private String ConfigCol(int gCol)
        {
            String result = "";
            try
            {
                switch (gCol)
                {
                    //-- OGC column 1-17, sum column 12
                    case 1: result = "C1"; break;
                    case 2: result = "C2"; break;
                    case 3: result = "C3"; break;
                    case 4: result = "IC4"; break;
                    case 5: result = "NC4"; break;
                    case 6: result = "IC5"; break;
                    case 7: result = "NC5"; break;
                    case 8: result = "C6"; break;
                    case 9: result = "C7"; break;
                    case 10: result = "CO2"; break;
                    case 11: result = "N2"; break;
                    case 12: result = "SUM_COMPO"; break;
                    case 13: result = "GHV"; break;
                    case 14: result = "SG"; break;
                    case 15: result = "H2O"; break;
                    case 16: result = "HG"; break;
                    case 17: result = "H2S"; break;
 
                }

                return result;

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
                return "";
            }
        }


        private void ShowFooterValue(ref GridViewRowEventArgs gRow, int sCol, int eCol, DataRow gDR)
        {
            String result = "";
            String fd = "";
            try
            {
                for (int c = sCol; c <= eCol; c++)
                {
                    fd = ConfigCol(c);
                    if (fd != "")
                    {
                        switch (fd)
                        {
                            case "SG":
                            case "H2O":
                            case "HG":
                            case "H2S":
                                result = Utility.FormatNum(gDR[fd], 4); break;
                            case "SUM_COMPO":   //ค่า SUM คือ บวก CH4->H2S ต้องได้ 100(+-0.003)
                                result = Utility.FormatNum(gDR[fd], 3);
                                Double dVal = Utility.ToDouble(gDR[fd]);
                                if (dVal < 99.997 || dVal > 100.003)
                                {
                                    gRow.Row.Cells[c].CssClass = "cell-bg-sum cell-right cell-Middle cell-border";
                                }
                                break;
                            default: result = Utility.FormatNum(gDR[fd], 3); break;
                        }

                    }
                    else
                    {
                        result = "";
                    }

                    gRow.Row.Cells[c].Text = result;
                }

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

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
            int colRDATE = -1, colC1 = -1, colC2 = -1, colC3 = -1, colIC4 = -1, colNC4 = -1, colIC5 = -1, colNC5 = -1, colC6 = -1, colC7 = -1;
            int colCO2 = -1, colN2 = -1, colGHV = -1, colSG = -1, colH2O = -1, colHG = -1, colH2S = -1;
            string pRDATE = "";
            object pC1 = null, pC2 = null, pC3 = null, pIC4 = null, pNC4 = null, pIC5 = null, pNC5 = null, pC6 = null, pC7 = null;
            object pCO2 = null, pN2 = null, pGHV = null, pSG = null, pH2O = null, pHG = null, pH2S = null;

            String ChkDay = "";
            String ChkDate = "", Value = "";
            String fid = "";

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
                            String WSheet = Utility.GetCtrl(hidFID) ;
                            fid = Utility.GetCtrl(hidFID);
                            //-- อ่าน worksheet ตาม fid
                            DT1 = Exc.ReadWorksheet(FullFileName, WSheet);

                            if ((DT1 == null) || DT1.Rows.Count < 4)
                            {
                                Msg = " - Data not found!";

                            }
                            else {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref Msg, ref ExMaxCol1, ref ExHeadRow1, ref colRDATE, ref colC1, ref colC2, ref colC3, ref colIC4, ref colNC4, ref colIC5, ref colNC5, ref colC6, ref colC7
                                                        , ref colCO2, ref colN2, ref colGHV, ref colSG, ref colH2O, ref colHG, ref colH2S);
                                if (FormatExcel > 0)
                                {

                                    //-- delete tmp data
                                    Project.dal.MngTmpOffshoreDaily(DBUTIL.opDELETE, "", "", fid);


                                    //2) บันทึกใน tmp table
                                    foreach (DataRow DR1 in DT1.Rows)
                                    {
                                        if (ExRow1 > ExHeadRow1 + 1 && ExRow1 < ExHeadRow1 + 33 && Msg == "") //-- ข้อมูลจะเริ่มจากบรรทัด Date ไปอีก 2 บรรทัด  และต้องไม่เกิน 31 วัน เพราะท้ายตารางมีตารางอีก 
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

                                            }
                                        }

                                        ExRow1++;
                                    }


                                    if (Msg == "")
                                    {
                                        //5) บันทึกลง  OFFSHORE_DAILY_UPDATE
                                        Project.dal.MngTmp2OffshoreDailyUpdate(fid, sYYMM);
                                        MsgSuccess += "Successfully imported [" + WSheet + "] </br>";

                                    }
                                    else
                                    {
                                        Msg += "Fail to uploaded [" + WSheet + "] : </br>" + Msg;
                                    }


                                }
                                else
                                {
                                    Msg = " - The file template is invalid!";  //กรณี worksheet ตามชื่อ fid ไม่ตรง format template ไม่ต้องแสดง ให้ทำ worksheet อื่นต่อเลย
                                }
                            }

                        }


                    }
                    else {
                        Msg = " - Please select the excel file! (" + Project.gExcelFileType.Replace("|", " ") + ") </br>";
                    }


                }
                else {
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
        //////======================================================================



    }
}