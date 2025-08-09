using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- aor edit 28/06/2019 ---
    public partial class rptON : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public String showFID = "", showMonth = "", showYear = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskRptMonthly, true);

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
                    case "LOAD": break;  //--- ตอนเรียกหน้าจอครั้งแรก ยังไม่ต้องแสดงข้อมูล เนื่องจากใช้เวลานาน
                    case "SEARCH":
                        LoadData(); break;
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
            DataTable DT = new DataTable();
            try
            {
                Utility.LoadMonthCombo(ref ddlMONTH, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEAR, "2018");

                Utility.LoadMonthCombo(ref ddlMONTHTO, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEARTO, "2018");

                DateTime today = System.DateTime.Today;
                if (today.Day < 6) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        Utility.SetCtrl(ddlMONTH, "12");
                        Utility.SetCtrl(ddlYEAR, (today.Year - 1).ToString());

                        Utility.SetCtrl(ddlMONTHTO, "12");
                        Utility.SetCtrl(ddlYEARTO, (today.Year - 1).ToString());
                    }
                    else
                    {
                        Utility.SetCtrl(ddlMONTH, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEAR, today.Year.ToString());

                        Utility.SetCtrl(ddlMONTHTO, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEARTO, today.Year.ToString());
                    }
                }
                else
                {
                    Utility.SetCtrl(ddlMONTH, today.Month.ToString());
                    Utility.SetCtrl(ddlYEAR, today.Year.ToString());

                    Utility.SetCtrl(ddlMONTHTO, today.Month.ToString());
                    Utility.SetCtrl(ddlYEARTO, today.Year.ToString());
                }

                DT = Project.dal.SearchSiteFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                DT = Project.dal.SearchRptFidTemplate("1", "", "");
                Utility.LoadList(ref ddlTEMPLATE, DT, "T_NAME", "TID");


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

            String MM1 = "", YY1 = "", MM2 = "", YY2 = "";
            String TmplateList = "", SIDList = "";
            String pType = "";
            String fromDate = "", toDate = "";
            try
            {

                MM1 = Validation.GetCtrlIntStr(ddlMONTH);   YY1 = Validation.GetCtrlIntStr(ddlYEAR);
                MM2 = Validation.GetCtrlIntStr(ddlMONTHTO); YY2 = Validation.GetCtrlIntStr(ddlYEARTO);
                pType = Validation.GetCtrlStr(ddlRPT_TYPE);
 
                TmplateList = Utility.GetListBoxValue(ddlTEMPLATE);
                SIDList = Utility.GetListBoxValue(ddlFID);

                if ((TmplateList + SIDList != "") && MM1 != "" && YY1 != "" && MM2 != "" && YY2 != "")
                {
                    
                    fromDate = "01/" + MM1.PadLeft(2, '0') + "/" + YY1;
                    string tmpDate = "01/" + MM2.PadLeft(2, '0') + "/" + YY2;
                    toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(tmpDate)).AddMonths(1).AddDays(-1));

                    //pType ="10DAY","15DAY","ENDMTH"
                    //-- edit 27/05/2024 เพิ่ม "1DAY"
                    DT = Project.dal.SearchRptOnshoreSummary(pType, fromDate, toDate, TmplateList, SIDList, "", "");

       
                    //---- Bound Data Table --------------------------------------------------------
                    Utility.BindGVData(ref gvResult, DT, false);
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

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {

                    //create a new row  (UNIT)-------------------------------------------------------------------
                    //cast the sender back to a gridview
                    GridView gv = sender as GridView;
                    TableCell cell = null;
                    e.Row.TableSection = TableRowSection.TableHeader;
                    GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                    extraHeader.TableSection = TableRowSection.TableHeader;

                    cell = new TableCell(); cell.Text = "";
                    cell.Width = 90; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "";
                    cell.Width = 50; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "";
                    cell.Width = 60; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "unit";
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "mole %";
                    cell.ColumnSpan = 10; cell.Width = 700; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "ug/m3";          //-- Hg  
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "ppmv";          //-- H2S
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "Btu/scf";          //-- GHVsat
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "";          //-- SG
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "Btu/scf";          //-- WI
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "Lb/MMscf";          //-- H2O
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "mole %";          //-- C2+
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    cell = new TableCell(); cell.Text = "mole %";          //-- CO2+N2
                    cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);
 
                    //add the new row to the gridview
                    gv.Controls[0].Controls.AddAt(1, extraHeader);
                    //---------------------------------------------------------------

                }
                else
                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    if ( showFID != Utility.ToString(dr["FID"]))
                    {
                        showFID = Utility.ToString(dr["FID"]);
                        e.Row.Cells[0].Text = showFID;
                        showMonth = ""; showYear = "";
                        e.Row.CssClass = "itemRow10top";
                    }
                    else
                    {
                        e.Row.Cells[0].Text = "";
                    }

                    if (showYear != Utility.ToString(dr["YY"]))
                    {
                        showYear = Utility.ToString(dr["YY"]);
                        e.Row.Cells[1].Text = showYear;
                        showMonth = "";
                        if (e.Row.Cells[0].Text == "")   e.Row.Cells[1].CssClass = "cell-center cell-Middle cell-border11top";
                    }
                    else
                    {
                        e.Row.Cells[1].Text = "";
                    }

                    if ( showMonth != Utility.EnMonth(Utility.ToInt(dr["MM"])) )
                    {
                        showMonth = Utility.EnMonth(Utility.ToInt(dr["MM"]));
                        e.Row.Cells[2].Text = showMonth;
                        
                        if (e.Row.CssClass != "itemRow10top")
                        {
                            e.Row.Cells[2].CssClass = "cell-center cell-Middle cell-border11top";
                            e.Row.Cells[3].CssClass = "cell-center cell-Middle cell-border11top";
                            for (int i = 4; i < 22; i++)
                            {
                                e.Row.Cells[i].CssClass = "cell-right cell-Middle cell-border11top";
                            }
                        }
                        
                    }
                    else
                    {
                        e.Row.Cells[2].Text = "";
                    }


                    string showDay = "";
                    switch (Utility.ToString(dr["DDAY"]))
                    {
                        case "D1": showDay = "1-10"; break;
                        case "D2": showDay = "11-20"; break;
                        case "D3":
                            showDay = "21-"+ Utility.LastDayofMonth(Utility.ToInt(dr["YY"]), Utility.ToInt(dr["MM"])); break;
                        case "D4": showDay = "1-15"; break;
                        case "D5":
                            showDay = "16-" + Utility.LastDayofMonth(Utility.ToInt(dr["YY"]), Utility.ToInt(dr["MM"])); break;
                        case "D6":
                            showDay = "1-" + Utility.LastDayofMonth(Utility.ToInt(dr["YY"]), Utility.ToInt(dr["MM"])); break;
                        default: //-- edit 28/05/2024
                            showDay = Utility.ToString(dr["DDAY"]).Replace("D0","").Replace("D","");
                            break;
                    }
                    e.Row.Cells[3].Text = showDay;

                    //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                    ShowValue(ref e, 4, 21, dr);

                }
                else
                if (e.Row.RowType == DataControlRowType.Footer)
                {


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


        private void ShowValue(ref GridViewRowEventArgs gRow, int sCol, int eCol, DataRowView gDR)
        {
            String result = "";
            String fd = "";
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
                                    case "H2S":
                                    case "WC":
                                        result = Utility.FormatNum(gDR[fd], 2); break;
                                    case "SG":
                                        result = Utility.FormatNum(gDR[fd], 4); break;
                                    default: result = Utility.FormatNum(gDR[fd], 3); break;
                                }

                            }
                            else
                           {  //-- ข้อมูลไม่ใช่ตัวเลข แสดงว่า error
                            //    result = Utility.ToString(gDR[fd]);
                            //    if (result != "")
                            //    {
                            //        if (fd.IndexOf("DATE") < 0) gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border";

                            //    }
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
                    case 4: result = "C1"; break;
                    case 5: result = "C2"; break;
                    case 6: result = "C3"; break;
                    case 7: result = "IC4"; break;
                    case 8: result = "NC4"; break;
                    case 9: result = "IC5"; break;
                    case 10: result = "NC5"; break;
                    case 11: result = "C6"; break;
                    case 12: result = "CO2"; break;
                    case 13: result = "N2"; break;
                    case 14: result = "HG"; break;
                    case 15: result = "H2S"; break;
                    case 16: result = "GHV"; break;
                    case 17: result = "SG"; break;
                    case 18: result = "CALC_WI"; break; //WI เดิมใช้ WB //-- 25/09/2019  เปลี่ยนเป็นคำนวณ
                    case 19: result = "WC"; break; //H2O
                    case 20: result = "SUM_C2"; break; //C2+
                    case 21: result = "CO2_N2"; break; //CO2+N2
                }

                return result;

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
                return "";
            }
        }


        //////======================================================================

    }
}