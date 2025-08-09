using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- edit 14/08/2018 ---
    //30/08/2018 site ไหนที่ไม่ได้เลือก H2S ให้ซ่อนคอลัมน์ H2S เลย รวมทั้งใน Excel ด้วย
    public partial class rptMonthly : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public Int32 chkCount = 0;

        String fromDate = "", toDate = "";

        String gISO_FLAG = "";  // site นี้มี ISO
        Decimal gorderYMD = 0, gexpireYMD = 0;
        Double gC1 = -999, gC2 = -999, gC3 = -999, gIC4 = -999, gNC4 = -999;
        Double gIC5 = -999, gNC5 = -999, gC6 = -999, gN2 = -999, gCO2 = -999, gH2S = -999, gHG = -999;
        String gISO_ACCREDIT = "";  //ISO Alert
        String gH2S_FLAG = "";  // site นี้มี H2S

        //-- EDIT 19/07/2019 --
        Double gC1_MIN = -999, gC2_MIN = -999, gC3_MIN = -999, gIC4_MIN = -999, gNC4_MIN = -999;
        Double gIC5_MIN = -999, gNC5_MIN = -999, gC6_MIN = -999, gN2_MIN = -999, gCO2_MIN = -999, gH2S_MIN = -999, gHG_MIN = -999;
        Double gC1_MAX = -999, gC2_MAX = -999, gC3_MAX = -999, gIC4_MAX = -999, gNC4_MAX = -999;
        Double gIC5_MAX = -999, gNC5_MAX = -999, gC6_MAX = -999, gN2_MAX = -999, gCO2_MAX = -999, gH2S_MAX = -999, gHG_MAX = -999;
        String gISO_MINMAX = "";  //ISO MIN/MAX Alert
        String gChromatDate = "";
        String gOgcName = ""; //-- edit 23/04/2021 --

        int cntChecked = 0; //-- ตัวแปรนับจำนวน checked checkbox


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

                    if (ServerAction != "SEARCH")
                    {
                        Utility.SetCtrl(hidSITE_ID, ddlFID.SelectedValue);
                        Utility.SetCtrl(hidFID, ddlFID.SelectedItem.Text);
                        Utility.SetCtrl(hidMM, ddlMONTH.SelectedValue);
                        Utility.SetCtrl(hidYY, ddlYEAR.SelectedValue);
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
                        LoadData(); break;

                    case "SAVE":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            SaveData();
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
            DataTable DT = new DataTable();
            try
            {
                DT = Project.dal.SearchSiteFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                Utility.LoadMonthCombo(ref ddlMONTH, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEAR, "2015");

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

               
                DT = Project.dal.SearchUserList("", OtherCriteria: "");
                Utility.LoadList(ref ddlREPORT_BY, DT, "USER_DESC", "USER_NAME", true, "");
                Utility.LoadList(ref ddlAPPROVE_BY, DT, "USER_DESC", "USER_NAME", false, "");

                // 30/08/2018 -- ให้ default เป็น user 2 คน
                Utility.SetCtrl(ddlREPORT_BY, Project.dal.GetSysConfigValue("REPORT_BY").ToString());
                Utility.SetCtrl(ddlAPPROVE_BY, Project.dal.GetSysConfigValue("APPROVE_BY").ToString());

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

        private void InitSession()
        {
            try
            {
                Session[hidSITE_ID.Value + "_DAILY"] = null;

                for (int c = 2; c < 29; c++)
                {
                    Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(c)] = "";

                    Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(c)] = "";
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void LoadData()
        {
            DataTable DT = null;
            DataTable rmDT = null;
            try
            {

                if (Validation.GetCtrlIntStr(ddlFID) != "" && Validation.GetCtrlIntStr(ddlMONTH) != "" && Validation.GetCtrlIntStr(ddlYEAR) != "")
                {
                    InitSession();

                    //-- พิจารณา report type เพื่อแสดงวันที่เริ่ม-สิ้นสุด
                    switch (Utility.GetCtrl(ddlRPT_TYPE))
                    {
                        case "20DAY": 
                            // ตั้งแต่ 21 ของเดือนที่แล้ว ถึง 20 ของเดือนที่เลือก
                            toDate = "20/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                            DateTime Date20 = Convert.ToDateTime(Utility.AppDateValue(toDate));
                            fromDate = Utility.AppFormatDate(Date20.AddMonths(-1).AddDays(1));   // 21 ของเดือนที่แล้ว  
                            break;
                        case "27DAY":
                            // ตั้งแต่ 28 ของเดือนที่แล้ว ถึง 27 ของเดือนที่เลือก
                            toDate = "27/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                            DateTime Date27 = Convert.ToDateTime(Utility.AppDateValue(toDate));
                            fromDate = Utility.AppFormatDate(Date27.AddMonths(-1).AddDays(1));   // 28 ของเดือนที่แล้ว 
                            break;
                        case "ENDMTH":
                            //วันที่สิ้นเดือนของเดือนที่เลือก
                            fromDate = "01/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                            toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));

                            break;
                    }
                    Utility.SetCtrl(hidfromDate, fromDate);
                    Utility.SetCtrl(hidtoDate, toDate);


                    //---- ค้นหา ข้อมูลในตาราง -----------------------------------------------------------------
                    DT = Project.dal.SearchGqmsDailyUpdateReport(Validation.GetCtrlIntStr(ddlFID), ddlFID.SelectedItem.Text, fromDate, toDate);
                  
                    //-- ตรวจสอบว่าเป็น ISO Site หรือไม่ ถ้าใช่ต้องตรวจสอบค่า ว่าตรงกับเงื่อนไข ISO หรือไม่
                    //  ค่า ISO ต้องอยู่ในช่วง x/ 2 – 2x     
                    //  ต้องตรวจสอบค่า gas composition ต้องไม่ต่ำกว่า x / 2  และมากกว่า 2x ทศนิยม 6 ตำแหน่ง
                    //  ยกเว้น H2S ไม่ต้องตรวจสอบ
                    //  ในกรณีที่มีค่า ไม่ตามเกณฑ์ ISO ในรายงานให้เอา logo iso ออก
                    //-- ค้นหา ISO-> Standard Gas Composition 
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        gH2S_FLAG = Utility.ToString(DT.Rows[0]["H2S_FLAG"]);
                        Utility.SetCtrl(hidH2S_FLAG, gH2S_FLAG);

                        if (Utility.ToString(DT.Rows[0]["ISO_FLAG"]) == "Y")
                        {
                            String YYMM = Utility.ToString(Utility.ToNum(ddlYEAR.Text) * 100 + Utility.ToNum(ddlMONTH.SelectedValue));
                            String OtherCri = "( TO_CHAR(ORDER_DATE,'YYYYMM') < '" + YYMM + "' OR ORDER_DATE IS NULL) " +
                                              " AND (TO_CHAR(EXPIRE_DATE, 'YYYYMM') >= '" + YYMM + "' OR EXPIRE_DATE IS NULL)  ";
                            DataTable gasDT = Project.dal.SearchSiteSgc(Validation.GetCtrlIntStr(ddlFID), OtherCriteria: OtherCri);
                            if (gasDT != null && gasDT.Rows.Count > 0)
                            {
                                DataRow gasDR = Utility.GetDR(ref gasDT);

                                gISO_FLAG = "Y";
                                gISO_ACCREDIT = "Y";
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

                                Utility.SetCtrl(hidgISO_FLAG, gISO_FLAG);
                                Utility.SetCtrl(hidgorderYMD, Utility.ToString(gorderYMD));
                                Utility.SetCtrl(hidgexpireYMD, Utility.ToString(gexpireYMD));
                                Utility.SetCtrl(hidgC1, Utility.ToString(gC1));
                                Utility.SetCtrl(hidgC2, Utility.ToString(gC2));
                                Utility.SetCtrl(hidgC3, Utility.ToString(gC3));
                                Utility.SetCtrl(hidgIC4, Utility.ToString(gIC4));
                                Utility.SetCtrl(hidgNC4, Utility.ToString(gNC4));
                                Utility.SetCtrl(hidgIC5, Utility.ToString(gIC5));
                                Utility.SetCtrl(hidgNC5, Utility.ToString(gNC5));
                                Utility.SetCtrl(hidgC6, Utility.ToString(gC6));
                                Utility.SetCtrl(hidgN2, Utility.ToString(gN2));
                                Utility.SetCtrl(hidgCO2, Utility.ToString(gCO2));
                                Utility.SetCtrl(hidgH2S, Utility.ToString(gH2S));
                                Utility.SetCtrl(hidgHG, Utility.ToString(gHG));


                                //-- EDIT 22/07/2019 --
                                gISO_MINMAX = "Y";
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

                                Utility.SetCtrl(hidgC1_MIN, Utility.ToString(gC1_MIN));
                                Utility.SetCtrl(hidgC2_MIN, Utility.ToString(gC2_MIN));
                                Utility.SetCtrl(hidgC3_MIN, Utility.ToString(gC3_MIN));
                                Utility.SetCtrl(hidgIC4_MIN, Utility.ToString(gIC4_MIN));
                                Utility.SetCtrl(hidgNC4_MIN, Utility.ToString(gNC4_MIN));
                                Utility.SetCtrl(hidgIC5_MIN, Utility.ToString(gIC5_MIN));
                                Utility.SetCtrl(hidgNC5_MIN, Utility.ToString(gNC5_MIN));
                                Utility.SetCtrl(hidgC6_MIN, Utility.ToString(gC6_MIN));
                                Utility.SetCtrl(hidgN2_MIN, Utility.ToString(gN2_MIN));
                                Utility.SetCtrl(hidgCO2_MIN, Utility.ToString(gCO2_MIN));
                                Utility.SetCtrl(hidgH2S_MIN, Utility.ToString(gH2S_MIN));
                                Utility.SetCtrl(hidgHG_MIN, Utility.ToString(gHG_MIN));
                                Utility.SetCtrl(hidgC1_MAX, Utility.ToString(gC1_MAX));
                                Utility.SetCtrl(hidgC2_MAX, Utility.ToString(gC2_MAX));
                                Utility.SetCtrl(hidgC3_MAX, Utility.ToString(gC3_MAX));
                                Utility.SetCtrl(hidgIC4_MAX, Utility.ToString(gIC4_MAX));
                                Utility.SetCtrl(hidgNC4_MAX, Utility.ToString(gNC4_MAX));
                                Utility.SetCtrl(hidgIC5_MAX, Utility.ToString(gIC5_MAX));
                                Utility.SetCtrl(hidgNC5_MAX, Utility.ToString(gNC5_MAX));
                                Utility.SetCtrl(hidgC6_MAX, Utility.ToString(gC6_MAX));
                                Utility.SetCtrl(hidgN2_MAX, Utility.ToString(gN2_MAX));
                                Utility.SetCtrl(hidgCO2_MAX, Utility.ToString(gCO2_MAX));
                                Utility.SetCtrl(hidgH2S_MAX, Utility.ToString(gH2S_MAX));
                                Utility.SetCtrl(hidgHG_MAX, Utility.ToString(gHG_MAX));

                            }
                            Utility.ClearObject(ref gasDT);
                        }
                        else
                        {
                                gISO_FLAG = "N";
                                gISO_ACCREDIT = "N";
                                Utility.SetCtrl(hidISO_ACCREDIT, gISO_ACCREDIT);
                                Utility.SetCtrl(hidgorderYMD, "");
                                Utility.SetCtrl(hidgexpireYMD, "");
                                Utility.SetCtrl(hidgC1, "");
                                Utility.SetCtrl(hidgC2, "");
                                Utility.SetCtrl(hidgC3, "");
                                Utility.SetCtrl(hidgIC4, "");
                                Utility.SetCtrl(hidgNC4, "");
                                Utility.SetCtrl(hidgIC5, "");
                                Utility.SetCtrl(hidgNC5, "");
                                Utility.SetCtrl(hidgC6, "");
                                Utility.SetCtrl(hidgN2, "");
                                Utility.SetCtrl(hidgCO2, "");
                                Utility.SetCtrl(hidgH2S, "");
                            Utility.SetCtrl(hidgHG, "");

                            //-- EDIT 19/07/2019 --
                            gISO_MINMAX = "N";
                            Utility.SetCtrl(hidISO_MINMAX, gISO_MINMAX);
                            Utility.SetCtrl(hidgC1_MIN, "");
                            Utility.SetCtrl(hidgC2_MIN, "");
                            Utility.SetCtrl(hidgC3_MIN, "");
                            Utility.SetCtrl(hidgIC4_MIN, "");
                            Utility.SetCtrl(hidgNC4_MIN, "");
                            Utility.SetCtrl(hidgIC5_MIN, "");
                            Utility.SetCtrl(hidgNC5_MIN, "");
                            Utility.SetCtrl(hidgC6_MIN, "");
                            Utility.SetCtrl(hidgN2_MIN, "");
                            Utility.SetCtrl(hidgCO2_MIN, "");
                            Utility.SetCtrl(hidgH2S_MIN, "");
                            Utility.SetCtrl(hidgHG_MIN, "");
                            Utility.SetCtrl(hidgC1_MAX, "");
                            Utility.SetCtrl(hidgC2_MAX, "");
                            Utility.SetCtrl(hidgC3_MAX, "");
                            Utility.SetCtrl(hidgIC4_MAX, "");
                            Utility.SetCtrl(hidgNC4_MAX, "");
                            Utility.SetCtrl(hidgIC5_MAX, "");
                            Utility.SetCtrl(hidgNC5_MAX, "");
                            Utility.SetCtrl(hidgC6_MAX, "");
                            Utility.SetCtrl(hidgN2_MAX, "");
                            Utility.SetCtrl(hidgCO2_MAX, "");
                            Utility.SetCtrl(hidgH2S_MAX, "");
                            Utility.SetCtrl(hidgHG_MAX, "");

                        }

                        chkCount = DT.Rows.Count;
                        pnlREPORT.Visible = true;
                    }
                    else
                    {
                        chkCount = 0;
                        pnlREPORT.Visible = false;
                    }

                    //---- Bound Data Table --------------------------------------------------------
                    Session[hidSITE_ID.Value +"_DAILY"] = DT;
                    Utility.BindGVData(ref gvResult, DT, false);
                    //---- Bound Data Table --------------------------------------------------------

                    //-- ถ้า check box ทุกตัวถูก checked ให้กำหนด chkAll เป็น checked
                    if (cntChecked == gvResult.Rows.Count)
                    {
                        gvResult.HeaderRow.Cells[0].Text = "<input type='checkbox' name='chkAll' id='chkAll' onclick='javascript:SelectAll();' checked=\"checked\" />";
                    }

                    //---- ค้นหา REMARK เพื่่อจะได้ทราบว่าเคยบันทึก show date แล้วหรือยัง -----------------------------------------------------------------
                    rmDT = Project.dal.SearchRptMonthly("", ddlFID.SelectedItem.Text, Validation.GetCtrlIntStr(ddlMONTH), Validation.GetCtrlIntStr(ddlYEAR), Validation.GetCtrlStr(ddlRPT_TYPE));
                    if (rmDT != null && rmDT.Rows.Count > 0)
                    {
                        DataRow rmDR = Utility.GetDR(ref rmDT);
                        Utility.SetCtrl(hidREPORT_ID, Utility.ToString(rmDR["ID"]));
                        Utility.SetCtrl(txtREMARK, Utility.ToString(rmDR["REMARK1"])); //-- EDIT 30/07/2019 --
                        Utility.SetCtrl(txtREMARK_ADD, Utility.ToString(rmDR["REMARK2"])); //-- EDIT 30/07/2019 --
                        Utility.SetCtrl(ddlREPORT_BY, Utility.ToString(rmDR["REPORT_BY"]));
                        Utility.SetCtrl(ddlAPPROVE_BY, Utility.ToString(rmDR["APPROVE_BY"]));

                        //-- EDIT 22/07/2019 ---
                        if ( Utility.ToString(rmDR["SIGNED_FLAG"]) == "N")
                        {
                            rblSIGN_N.Checked = true; rblSIGN_Y.Checked = false;
                        }
                        else
                        {
                            rblSIGN_N.Checked = false; rblSIGN_Y.Checked = true;
                        }

                    }
                    else
                    {
                        Utility.SetCtrl(hidREPORT_ID, "");
                        //Utility.SetCtrl(txtREMARK, "");

                        // 30/08/2018 -- ให้ default เป็น user 2 คน
                        Utility.SetCtrl(ddlREPORT_BY, Project.dal.GetSysConfigValue("REPORT_BY").ToString());
                        Utility.SetCtrl(ddlAPPROVE_BY, Project.dal.GetSysConfigValue("APPROVE_BY").ToString());

                        //-- EDIT 22/07/2019 ---
                        rblSIGN_N.Checked = true; rblSIGN_Y.Checked = false;
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
                        //-- EDIT 26/06/2023 --
                        //เพิ่มวงเล็บ [1] ต่อท้าย Unnorm Min // Unnorm Max // Chromat run  //AVG 
                        //-- EDIT 22/09/2023 -- ต้องเป็น FID ที่รองรับ ISO ด้วยเท่านั้น
                        if (gISO_FLAG == "Y" ) //if (gISO_FLAG == "Y" || true)--EDIT 22/09/2023 //if (gISO_FLAG == "Y" ) //-- EDIT 26/06/2023 --
                        {
                            //if (gISO_MINMAX == "N") remark1 = " [1] \"Out of scope สมอ. (TISI)\" " ;
                            //if (gISO_ACCREDIT == "N")
                            //{
                            //    if (remark1 != "") remark1 += " / ";
                            //    remark1 += " [2] \"Out of scope Standard Gas\" ";
                            //}
                            if (gISO_MINMAX == "N" || gISO_ACCREDIT == "N" || true) //if (gISO_MINMAX == "N" || gISO_ACCREDIT == "N" ) //-- EDIT 26/06/2023 --
                                remark1 = " [1] \"Out of TISI Accreditation Scope for our Laboratory\" ";

                            if (remark1 != "") remark1 = "Test Marked " + remark1 + "\r\n";
                        }

                        String h2s = "", hg = "";
                        String h2sFlag = "", hgFlag = "";
                        String rDate = "01/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR); //-- กำหนดเป็นวันที่ 1 ของเดือน
                        DataTable DTg = Project.dal.SearchSpotFID(ddlFID.SelectedItem.Text, rDate);
                            
                        if ( DTg != null && DTg.Rows.Count >0)
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

                            remark2 += "1. H2S" + h2sFlag + " = " + h2s + " ppmv,  Hg" + hgFlag + " = " + hg + " ug/m3."+ "\r\n";

                        } 


                        //-- edit 31/08/2020 -- ส่วน remark ให้แสดง OGC was calibrated on (date) ดึงวันที่ As Found จากระบบ OGC Data
                        string sql = "SELECT C.* , S.OSITE_ID " +
                            " FROM C_CALIBRATE C INNER JOIN C_SITE_FID S ON C.CSITE_ID = S.CSITE_ID" +
                            " WHERE C.WORK_TYPE IN('ML2','ML3') AND C.MM =" + Utility.GetCtrl(ddlMONTH) + " AND C.YY =" + Utility.GetCtrl(ddlYEAR) + " AND S.OSITE_ID =" + Utility.GetCtrl(ddlFID) +
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

                            remark2 += " OGC was calibrated on " + gChromatDate + "\r\n";  //" ( use time 0 mins. ) "+  //edit--14/10/2020
                        }
                        //-- ใส่หมายเหตุ ---------------------------------------------------------------------------------------------------

                        Utility.SetCtrl(txtREMARK, remark1+remark2);

                        //-- edit 30/07/2019  ไม่ต้องบันทึก
                        ////-- บันทึก default remark
                        //String ID = "";
                        //String SignedFlag = "N";

                        //Project.dal.MngRptMonthly(DBUTIL.opINSERT, ref ID, ddlFID.SelectedItem.Text, Validation.GetCtrlIntStr(ddlMONTH), Validation.GetCtrlIntStr(ddlYEAR), Validation.GetCtrlStr(ddlRPT_TYPE), remark, Validation.GetCtrlStr(hidISO_ACCREDIT), Validation.GetCtrlStr(ddlREPORT_BY), Validation.GetCtrlStr(ddlAPPROVE_BY), SignedFlag, Validation.GetCtrlStr(hidISO_MINMAX));
                        //Utility.SetCtrl(hidREPORT_ID, ID);


                       

                    }


                    //---------------------------------------------------------------------

                    //-- ต้องผ่านการ data bound ก่อน เพื่อจะได้ทราบว่า มี iso alert หรือเปล่า
                    Utility.SetCtrl(hidISO_ACCREDIT, gISO_ACCREDIT);
                    Utility.SetCtrl(hidISO_MINMAX, gISO_MINMAX);

                    //-- ซ่อมคอลัมน์ H2S --
                    if (gH2S_FLAG != "Y")
                        gvResult.Columns[12].Visible = false;
                    else
                        gvResult.Columns[12].Visible = true;


                    //-- EDIT 22/07/2019 --- 
                    //-- กำหนด header column ต้องผ่านการ data bound ก่อน เพื่อจะได้ทราบว่า มี iso alert หรือเปล่า
                    if (gISO_FLAG == "Y")
                    {
                        string headerAlert = "";
                        for (int c = 2; c < 13; c++)
                        {
                            headerAlert = "";
                            //if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(c)]) != "")
                            //{
                            //    headerAlert += "<br/>[1]";
                            //}
                            //if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(c)]) != "")
                            //{
                            //    if (headerAlert == "")
                            //        headerAlert = "<br/>[2]";
                            //    else
                            //        headerAlert += "[2]";
                            //}
                            //-- edit 08/07/2020 ---
                            //- ปรับการตรวจสอบข้อมูล สมอ.และ standard gas ให้เป็นหัวข้อเดียวกัน				
                            //- Auto Remarks ให้แสดง[1] "out of TISI Accreditation Scope for our Laboratory "
                            if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(c)]) != ""
                                || Utility.ToString(Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(c)]) != "")
                            {
                                headerAlert += "<br/>[1]";
                            }
                            
                            gvResult.HeaderRow.Cells[c].Text = ConfigColHeader(c) + headerAlert;
                        }

                        //-- H2O  column = 16
                        headerAlert = "";
                        //if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERTMINMAX_COL16"]) != "")
                        //{
                        //    headerAlert += "<br/>[1]";
                        //}
                        //if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERT_COL16"]) != "")
                        //{
                        //    if (headerAlert == "")
                        //        headerAlert = "<br/>[2]";
                        //    else
                        //        headerAlert += "[2]";
                        //}
                        //-- edit 08/07/2020 ---
                        //- ปรับการตรวจสอบข้อมูล สมอ.และ standard gas ให้เป็นหัวข้อเดียวกัน				
                        //- Auto Remarks ให้แสดง[1] "out of TISI Accreditation Scope for our Laboratory "
                        if (Utility.ToString(Session[hidSITE_ID.Value + "_ALERTMINMAX_COL16"]) != ""
                            || Utility.ToString(Session[hidSITE_ID.Value + "_ALERT_COL16"]) != "")
                        {
                            headerAlert += "<br/>[1]";
                        }

                        gvResult.HeaderRow.Cells[16].Text = ConfigColHeader(16) + headerAlert;
                    }

                    //-- EDIT 26/06/2023 --
                    //เพิ่มวงเล็บ [1] ต่อท้าย Unnorm Min // Unnorm Max // Chromat run  //AVG
                    //-- EDIT 22/09/2023 -- ต้องเป็น FID ที่รองรับ ISO ด้วยเท่านั้น
                    if (gISO_FLAG == "Y")
                    {
                        gvResult.HeaderRow.Cells[17].Text = ConfigColHeader(17) + "<br/>[1]";
                        gvResult.HeaderRow.Cells[18].Text = ConfigColHeader(18) + "<br/>[1]";
                        gvResult.HeaderRow.Cells[19].Text = ConfigColHeader(19) + "<br/>[1]";
                    }
                    else
                    {
                        gvResult.HeaderRow.Cells[17].Text = ConfigColHeader(17);
                        gvResult.HeaderRow.Cells[18].Text = ConfigColHeader(18);
                        gvResult.HeaderRow.Cells[19].Text = ConfigColHeader(19);
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
                Utility.ClearObject(ref rmDT);
            }
        }


        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable aDT = null;
            DataRow aDR = null;

            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[0].Text = "<input type='checkbox' name='chkAll' id='chkAll' onclick='javascript:SelectAll();' />";
                }
                else
                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    //-- Check box เมื่อกำหนดวันที่ให้ออก report  (ต้องจำไว้แต่ละ report ด้วย)
                    //-- checkbox column 0      checked="checked"
                    String chked = " checked=\"checked\" "; //-- ให้ default เป็น เลือกไว้ก่อน

                    if (Utility.ToString(dr["SHOW_FLAG"]) == "N" )
                    {
                        chked = "";
                        e.Row.CssClass = "itemDisable";
                    }
                    else
                    {
                        cntChecked++;
                    }
  
                    e.Row.Cells[0].Text = "<input type=\"checkbox\" id=\"chkSelect" + e.Row.DataItemIndex + "\" name=\"chkSelect" + e.Row.DataItemIndex + "\" " + chked + " /> ";

                    //-- edit 23/04/2021 --
                    gOgcName = Utility.ToString(dr["OGC_NAME"]);

                    //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                    //-- OGC column 2-18 
                    ShowValue(ref e, 2, 18, dr);

                    //-- OGC BTU column 19 RUN
                    ShowValue(ref e, 19, 19, dr);

                    //-- FLOW column 20 FLOW
                    ShowValue(ref e, 20, 20, dr);


                }
                else
                if (e.Row.RowType == DataControlRowType.Footer)
                {

                    //row=> AVERAGE ==============================================================================================================
                    //e.Row.Cells[1].Text = "AVERAGE";
                    //-- EDIT 26/06/2023 --
                    //เพิ่มวงเล็บ [1] ต่อท้าย Unnorm Min // Unnorm Max // Chromat run  //AVG
                    //-- EDIT 22/09/2023 -- ต้องเป็น FID ที่รองรับ ISO ด้วยเท่านั้น
                    if (gISO_FLAG == "Y")
                        e.Row.Cells[1].Text = "AVG" + " [1]";
                    else
                        e.Row.Cells[1].Text = "AVG";

                    string OtherCri = "";
                    //-- คำนวณ average โดยพิจารณาวันที่ต้องการ show ด้วย  ถ้าวันไหนไม่ show ก็ไม่ต้องเอามาคำนวณ
                    OtherCri = "NOT D.RDATE IN (SELECT RDATE FROM O_RPT_MONTHLY_DATE " +
                                "      WHERE FID = '" + hidFID.Value +"' AND SHOW_FLAG = 'N' AND RDATE>= TO_DATE('" + fromDate +"', 'DD/MM/YYYY') AND RDATE<= TO_DATE('" + toDate +"', 'DD/MM/YYYY') )";

                    aDT = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate, OtherCriteria:OtherCri);
                    if (aDT != null && aDT.Rows.Count > 0)
                    {
                        aDR = Utility.GetDR(ref aDT);
                        //-- OGC column 2-16 C1-H2O 
                        ShowFooterValue(ref e, 2, 16, aDR);
                    }


                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref aDT);
            }
        }


        //-- edit 23/04/2021 -- ในวันที่มีการแทนค่าสำรอง ต้องมีข้อความเป็นสีน้ำเงิน (ยกเว้นค่าน้ำ H2O) และตัดข้อมูล Unnorm ออกไป
        //-- edit 23/08/2022 -- แสดงข้อมูล Unnorm กลับมา
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
                                    case "SG": result = Utility.FormatNum(gDR[fd], 4); break;
                                    case "WC":
                                    case "WC1":
                                    case "WC2": result = Utility.FormatNum(gDR[fd], 2); break;
                                    case "RUN": result = Utility.FormatNum(gDR[fd], 0); break;
                                    default: result = Utility.FormatNum(gDR[fd], 3); break;
                                }
 
                                ShowAlert(ref gRow, c, fd, result, gDR);
                                if (gISO_FLAG == "Y")
                                {
                                    Decimal rYMD = Utility.ToNum(Utility.FormatDate(Convert.ToDateTime(gDR["RDATE"]), "YYYYMMDD"));
                                    if (rYMD >= gorderYMD && rYMD <= gexpireYMD)
                                    {
                                        ShowAlertISO(ref gRow, c, fd, result);
                                    }
                                }

                            }
                            else
                            {  //-- ข้อมูลไม่ใช่ตัวเลข แสดงว่า error
                                result = Utility.ToString(gDR[fd]);
                                if (result != "")
                                {
                                    if (fd.IndexOf("NAME") < 0 && fd.IndexOf("DATE") < 0) gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning";

                                    if (fd.IndexOf("DATE") > 0) result = Utility.AppFormatDate(gDR[fd]);

                                }
                            }
                        }
                    }

                    //-- edit 23/04/2021 -- ในวันที่มีการแทนค่าสำรอง ต้องมีข้อความเป็นสีน้ำเงิน (ยกเว้นค่าน้ำ H2O) และตัดข้อมูล Unnorm ออกไป
                    //-- edit 23/08/2022 -- แสดงข้อมูล Unnorm กลับมา
                    if (gOgcName != "")
                    {
                        if (c <= 15 || (c >= 17 && c <= 19) ) //-- edit 23/08/2022 
                        {
                            string css = gRow.Row.Cells[c].CssClass;
                            if (css == "")
                            {
                                gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning4";
                            }
                            else
                            {
                                if (css.IndexOf("txt-warning4") < 1) gRow.Row.Cells[c].CssClass = css + " txt-warning4";
                            }
                        }
                        //-- edit 23/08/2022 -- แสดงข้อมูล Unnorm กลับมา
                        //else
                        //{
                        //    if (c >= 17 && c <= 19) //unnorm, run ให้ใส่ -
                        //    {
                        //        result = "-";
                        //        gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning4";
                        //    }
                        //}
                    }


                    gRow.Row.Cells[c].Text = result;

                }

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }

        //-- edit 22/07/2019 --
        private String ConfigColHeader(int gCol)
        {
            String result = "";
            try
            {
                switch (gCol)
                {
                    //-- OGC column 2-18
                    case 2: result = "CH4"; break;
                    case 3: result = "C2H6"; break;
                    case 4: result = "C3H8"; break;
                    case 5: result = "IC4H10"; break;
                    case 6: result = "NC4H10"; break;
                    case 7: result = "IC5H12"; break;
                    case 8: result = "NC5H12"; break;
                    case 9: result = "C6H14"; break;
                    case 10: result = "CO2"; break;
                    case 11: result = "N2"; break;
                    case 12: result = "H2S"; break;
                    case 13: result = "NETHVDRY"; break;
                    case 14: result = "HVSAT"; break;
                    case 15: result = "SG"; break;
                    case 16: result = "H2O"; break;
                    case 17: result = "UNNORMMIN"; break;
                    case 18: result = "UNNORMMAX"; break;

                    //-- OGC BTU column 19 RUN
                    case 19: result = "RUN"; break;

                    //-- FLOW column 20 FLOW
                    case 20: result = "FLOW"; break;
                }

                return result;

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
                return "";
            }
        }



        private String ConfigCol(int gCol)
        {
            String result = "";
            try
            {
                switch (gCol)
                {
                    //-- OGC column 2-18
                    case 2: result = "C1"; break;
                    case 3: result = "C2"; break;
                    case 4: result = "C3"; break;
                    case 5: result = "IC4"; break;
                    case 6: result = "NC4"; break;
                    case 7: result = "IC5"; break;
                    case 8: result = "NC5"; break;
                    case 9: result = "C6"; break;
                    case 10: result = "CO2"; break;
                    case 11: result = "N2"; break;
                    case 12: result = "H2S"; break;
                    case 13: result = "NHV"; break;
                    case 14: result = "GHV"; break;
                    case 15: result = "SG"; break;
                    case 16: result = "WC"; break;
                    case 17: result = "UNNORMMIN"; break;
                    case 18: result = "UNNORMMAX"; break;

                    //-- OGC BTU column 19 RUN
                    case 19: result = "RUN"; break;

                    //-- FLOW column 20 FLOW
                    case 20: result = "FLOW"; break;
                }

                return result;

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
                return "";
            }
        }



        private void ShowAlert(ref GridViewRowEventArgs gRow, int gCol, String dField, String sValue, DataRowView gDR)
        {
            try
            {
                Double dVal = Utility.ToDouble(sValue);

                switch (dField)
                {
                    case "WC":   //ค่า WC ต้องไม่เกิน 7 lb และไม่น้อยกว่าหรือเท่ากับ 0 และ กรณีไม่มีค่าซ้ำกันเกิน 3 ชั่วโมง
                        if (Utility.ToString(gDR["WC_ALERT"]) == "Y")
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        //-- คอลัมน์ H2O ต้องใส่ [1] เสมอ //-- edit 22/07/2019 ---
                        Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        break;
                    case "UNNORMMIN":   //ค่า UnnormMin ต้องไม่ต่ำกว่า 98
                        if (dVal < 98)
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "UNNORMMAX":   //ค่า UnnormMax ต้องไม่เกิน 102
                        if (dVal > 102)
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;

                    case "GHV":// เอา BTU ไป compare กับ HVSAT (GHV) ด้วยวันที่เหลือมกัน (BTU DATE จะมากกว่า RDATE)
                        if (Utility.ToString(gDR["BTU"]) != "")
                        {
                            if (dVal != Utility.ToDouble(Utility.FormatNum(gDR["BTU"], 3)))
                            {
                                gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            }
                        }
                        break;

                    case "RUN":      //ค่า run ในตาราง จะต้องไม่ต่ำกว่า Minimum run ที่กำหนด
                        //Double minRUN = Utility.ToDouble(gDR["TOTAL_RUN"]) * ((100 - Utility.ToDouble(gDR["TOLERANCE_RUN"])) / 100);
                        //--30/08/2018  เก็บค่า TOLERANCE_RUN เป็นตัวเลข เช่น 30 หมายถึง ยอมรับได้ในช่วง +-30
                        Double minRUN = Utility.ToDouble(gDR["TOTAL_RUN"]) - Utility.ToDouble(gDR["TOLERANCE_RUN"]);

                        if (dVal < minRUN)
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }

                        //-- edit 31/08/2020 --
                        ////-- edit 22/07/2019 --
                        //// เพิ่มหมายเหตุ โดยแสดงวันที่ที่มี chromat run ต่ำกว่า 360 เดือนนั้นต้องมีวันเดียว ถ้ามีหลายวันก็ไม่ต้องแสดง   ระบุค่าของ H2S, Hg ต่อท้ายด้วย  
                        //if ( dVal>0 && dVal <360 )
                        //{
                        //    if (gChromatDate == "")
                        //        gChromatDate = Utility.FormatDate(Convert.ToDateTime(gDR["ADATE"]), "D MONTH YYYY"); //gDR["ADATE"]
                        //    else
                        //        gChromatDate = "MULTI-DATE";
                        //}
                        //-- edit 28/08/2020 -- ส่วน remark ให้แสดง OGC was calibrated on (date) ดึงวันที่ As Found จากระบบ OGC Data



                        break;
                    case "FLOW": //กรณีมีค่าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                        if (Utility.ToString(gDR["FLOW_ALERT"]) == "Y")
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }


        //-- edit 22/07/2019 --- เพิ่มตรวจสอบ Min/Max
        private void ShowAlertISO(ref GridViewRowEventArgs gRow, int gCol, String dField, String sValue)
        {
            try
            {
                //-- ตรวจสอบว่าเป็น ISO Site หรือไม่ ถ้าใช่ต้องตรวจสอบค่า ว่าตรงกับเงื่อนไข ISO หรือไม่
                //  ค่า ISO ต้องอยู่ในช่วง x/ 2 – 2x     
                //  ต้องตรวจสอบค่า gas composition ต้องไม่ต่ำกว่า x / 2  และมากกว่า 2x ทศนิยม 6 ตำแหน่ง
                //  ยกเว้น H2S ไม่ต้องตรวจสอบ

                Double dVal = Utility.ToDouble(sValue);

                switch (dField)
                {
                    case "C1":
                        if (gC1 != -999 && (dVal < gC1 / 2 || dVal > 2 * gC1))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC1_MIN != -999 && dVal < gC1_MIN) || (gC1_MAX != -999 && dVal > gC1_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "C2":
                        if (gC2 != -999 && (dVal < gC2 / 2 || dVal > 2 * gC2))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC2_MIN != -999 && dVal < gC2_MIN) || (gC2_MAX != -999 && dVal > gC2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "C3":
                        if (gC3 != -999 && (dVal < gC3 / 2 || dVal > 2 * gC3))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC3_MIN != -999 && dVal < gC3_MIN) || (gC3_MAX != -999 && dVal > gC3_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "IC4":
                        if (gIC4 != -999 && (dVal < gIC4 / 2 || dVal > 2 * gIC4))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gIC4_MIN != -999 && dVal < gIC4_MIN) || (gIC4_MAX != -999 && dVal > gIC4_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "NC4":
                        if (gNC4 != -999 && (dVal < gNC4 / 2 || dVal > 2 * gNC4))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gNC4_MIN != -999 && dVal < gNC4_MIN) || (gNC4_MAX != -999 && dVal > gNC4_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "IC5":
                        if (gIC5 != -999 && (dVal < gIC5 / 2 || dVal > 2 * gIC5))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gIC5_MIN != -999 && dVal < gIC5_MIN) || (gIC5_MAX != -999 && dVal > gIC5_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "NC5":
                        if (gNC5 != -999 && (dVal < gNC5 / 2 || dVal > 2 * gNC5))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gNC5_MIN != -999 && dVal < gNC5_MIN) || (gNC5_MAX != -999 && dVal > gNC5_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "C6":
                        if (gC6 != -999 && (dVal < gC6 / 2 || dVal > 2 * gC6))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gC6_MIN != -999 && dVal < gC6_MIN) || (gC6_MAX != -999 && dVal > gC6_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "N2":
                        if (gN2 != -999 && (dVal < gN2 / 2 || dVal > 2 * gN2))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gN2_MIN != -999 && dVal < gN2_MIN) || (gN2_MAX != -999 && dVal > gN2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "CO2":
                        if (gCO2 != -999 && (dVal < gCO2 / 2 || dVal > 2 * gCO2))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        }
                        if ((gCO2_MIN != -999 && dVal < gCO2_MIN) || (gCO2_MAX != -999 && dVal > gCO2_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        }
                        break;
                    case "H2S":
                        //if (gH2S != -999 && (dVal < gH2S / 2 || dVal > 2 * gH2S))
                        //{
                        //    gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                        //    Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        //    if (gISO_ACCREDIT == "Y") gISO_ACCREDIT = "N";
                        //}
                        //if ((gH2S_MIN != -999 && dVal < gH2S_MIN) || (gH2S_MAX != -999 && dVal > gH2S_MAX))  //เพิ่มตรวจสอบ Min/Max
                        //{
                        //    gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                        //    Session[hidSITE_ID.Value + "_ALERTMINMAX_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                        //    if (gISO_MINMAX == "Y") gISO_MINMAX = "N";
                        //}
                        //-- edit 08/07/2020 -- ถ้า site นั้นมี ISO ให้กำหนด H2S เป็น [1] เสมอ 
                        if (gISO_FLAG == "Y")
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session[hidSITE_ID.Value + "_ALERT_COL" + Utility.ToString(gCol)] += "," + gRow.Row.RowIndex.ToString() + ",";
                            gISO_ACCREDIT = "N";
                        }

                        break;

                    
              
                }


            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

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
                                    result = Utility.FormatNum(gDR[fd], 4);
                                    break;
                            case "WC":
                                    result = Utility.FormatNum(gDR[fd], 2);
                                    break;
                            default:
                                    result = Utility.FormatNum(gDR[fd], 3);
                                    break;
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
        private void SaveData()  
        {
            int op = -99;
            String rDate = "";
 
            try
            {

                //--วนหาว่า checked box วันใดบ้าง  
                foreach (GridViewRow rowData in gvResult.Rows)
                {
                    if (rowData.RowType == DataControlRowType.DataRow)
                    {
                        //-- ถ้าเป็นการ checked จากการ select all จะอ่านค่าได้เป็น กรณีเลือก = on,on  ไม่เลือก = on
                        //-- ถ้าเป็นการ checked เอง  จะอ่านค่าได้เป็น กรณีเลือก = on  ไม่เลือก = ว่าง
                        String chked = "N";
                        //if (Utility.GetCtrl(hidSELECTALL) == "Y")
                        //{
                        //    if (Validation.GetParamStr("chkSelect" + rowData.RowIndex) == "on,on") chked = "Y";
                        //}
                        //else
                        //{
                            if (Validation.GetParamStr("chkSelect" + rowData.RowIndex) == "on") chked = "Y";
                        //}

                        rDate = gvResult.Rows[rowData.RowIndex].Cells[1].Text; //-- วันที่
                        //-- ลบก่อน แล้วค่อย insert
                        Project.dal.MngRptMonthlyDate(DBUTIL.opDELETE, Validation.GetCtrlStr(hidFID), rDate);
                        Project.dal.MngRptMonthlyDate(DBUTIL.opINSERT, Validation.GetCtrlStr(hidFID), rDate, chked);
 
                    }

                }
               
                String ID = "";

                //-- บันทึก remark
                if (hidREPORT_ID.Value == "")
                    op = DBUTIL.opINSERT;
                else
                {
                    op = DBUTIL.opUPDATE;
                    ID = hidREPORT_ID.Value;
                }

                //-- EDIT 22/07/2019 ---
                String SignedFlag = "N";
                if (rblSIGN_Y.Checked) SignedFlag = "Y";

                Project.dal.MngRptMonthly(op, ref ID, Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidMM), Validation.GetCtrlStr(hidYY), Validation.GetCtrlStr(ddlRPT_TYPE), Validation.GetCtrlStr(txtREMARK), Validation.GetCtrlStr(hidISO_ACCREDIT), Validation.GetCtrlStr(ddlREPORT_BY), Validation.GetCtrlStr(ddlAPPROVE_BY), SignedFlag , Validation.GetCtrlStr(hidISO_MINMAX), Validation.GetCtrlStr(txtREMARK_ADD));
                Utility.SetCtrl(hidREPORT_ID, ID);

                LoadData();


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

    }
}