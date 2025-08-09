using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Forms
{
    //--  edit 12/07/2018 ---
    //30/08/2018 อยากทำหลายหน้าได้
    // กรณีที่ confirm daily แล้ว มีการแก้ไข ตัว checkbox จะหายไป แต่เมื่อส่ง confirm daily แล้วต้อง check box ให้ด้วย 
    // หรือจะเปลี่ยนเป็น กรณีที่ comfirm ไปแล้วมีการแก้ไข ให้สีพื้นเขียวหายไป 
    // กรณีแก้ไข แต่มีการส่ง confirm27, endmonth ไปแล้ว ต้องมี alert ให้เห็นใน row footer 27,endmonth ตัวหนังสือสีแดงพื้นสีขาว

    public partial class Verify : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public String EditItemIndex = "", hEditItemIndex = "";
        public Int32 chkCount = 0;
        String MsgSuccess = "";

        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;

        String fromDate = "", toDate = "";
        String fromDate27 = "", toDate27 = "";
        String fromDate20 = "", toDate20 = "";
        DateTime Date27, Date31, Date20;

        String gISO_FLAG = "";
        Decimal gorderYMD = 0, gexpireYMD = 0;
        Double gC1 = -999, gC2 = -999, gC3 = -999, gIC4 = -999, gNC4 = -999;
        Double gIC5 = -999, gNC5 = -999, gC6 = -999, gN2 = -999, gCO2 = -999, gH2S = -999;
        //-- EDIT 19/07/2019 --
        Double gC1_MIN = -999, gC2_MIN = -999, gC3_MIN = -999, gIC4_MIN = -999, gNC4_MIN = -999;
        Double gIC5_MIN = -999, gNC5_MIN = -999, gC6_MIN = -999, gN2_MIN = -999, gCO2_MIN = -999, gH2S_MIN = -999;
        Double gC1_MAX = -999, gC2_MAX = -999, gC3_MAX = -999, gIC4_MAX = -999, gNC4_MAX = -999;
        Double gIC5_MAX = -999, gNC5_MAX = -999, gC6_MAX = -999, gN2_MAX = -999, gCO2_MAX = -999, gH2S_MAX = -999;



        int colSUM = 22; //-- ตัวแปรคอลัมน์ summary

        String gNgRptNo27 = "", gNgRptNoEND = "", gNgRptNoDaily = "", gNgRptNoDaily27 = ""; //NGBILL report no
        String gNgRptNo20 = "", gNgRptNoDaily20 = ""; //NGBILL report no
        String gOmaName = "", gOmaName1 = "", gOmaName2 = "",  gFlowName1 = "", gFlowName2 = ""; //OMA name, FLOW name
        String gOgcName = "", gOgcName1 = "", gOgcName2 = "", gOgcName3 = "";  // OGC name สำรอง

        String SelectAllFlag = "";
        int cntChecked = 0; //-- ตัวแปรนับจำนวน checked checkbox

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
                    //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน hidden = EditItemIndex  เก็บเป็น ,2,3,5
                    hEditItemIndex  = Validation.GetParamStr("hidEditItemIndex");
                    if (hEditItemIndex != "") EditItemIndex = hEditItemIndex+ ",";

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
                        hEditItemIndex = ""; //-- clear รายการ edit 

                        //-- ก่อน loaddata ให้ตรวจสอบค่า H2O (WC) ถ้ายังไม่มีค่าให้ใส่ค่าของ OMA_NAME1 ไปด้วย
                        if (hidSITE_ID.Value != ddlFID.SelectedValue || hidMM.Value != ddlMONTH.SelectedValue || hidYY.Value != ddlYEAR.SelectedValue)
                        {
                            //-- EDIT 23/07/2019 -- กรณีไม่มี records ให้ใส่ null records เพื่อจะได้แสดง oma ได้
                            Project.dal.InsertNULLGqmsDailyUpdate(ddlFID.SelectedItem.Text, Validation.GetCtrlIntStr(ddlMONTH), Validation.GetCtrlIntStr(ddlYEAR));

                            Project.dal.UpdateWCGqmsDailyUpdate(Validation.GetCtrlIntStr(ddlFID), ddlFID.SelectedItem.Text, Validation.GetCtrlIntStr(ddlMONTH), Validation.GetCtrlIntStr(ddlYEAR));

                            //-- EDIT 28/08/2020 -- ดึงข้อมูล As Found+As Left+Final Cal จากระบบ OGC Data
                            LoadCalibrateData();



                        }
                        Utility.SetCtrl(hidSITE_ID, ddlFID.SelectedValue);
                        Utility.SetCtrl(hidFID, ddlFID.SelectedItem.Text);
                        Utility.SetCtrl(hidMM, ddlMONTH.SelectedValue);
                        Utility.SetCtrl(hidYY, ddlYEAR.SelectedValue);
                        gSessionID = "_"+ddlFID.SelectedValue + "_" + ddlYEAR.SelectedValue + "_" + ddlMONTH.SelectedValue; // ตัวแปรเก็บ sessionid เพื่อให้สามารถเปิดได้หลายหน้า formt: siteid_yyyy_mm
                        LoadData();
                        break;

                    case "CONFIRM_1":  //-- daily
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            SaveConfirmData1();
                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                        }
                        break;

                    case "CONFIRM_2":   //-- 27 days
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            //////-- edit 28/08/2020 -- เมื่อกดปุ่ม confirm ตรวจสอบว่า ถ้ายังไม่มีข้อมูล As Found ให้แจ้งเตือน
                            ////if(divASFOUND.Visible )
                            ////{
                            ////    if (Utility.GetCtrl(lblFOUND_DATE) == "") Msg = "ยังไม่มีข้อมูล Calibration!!!";
                            ////}

                            SaveConfirmData2();
                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                        }
                        break;

                    case "CONFIRM_3":   //-- end month
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            //////-- edit 28/08/2020 -- เมื่อกดปุ่ม confirm ตรวจสอบว่า ถ้ายังไม่มีข้อมูล As Found ให้แจ้งเตือน
                            ////if (divASFOUND.Visible)
                            ////{
                            ////    if (Utility.GetCtrl(lblFOUND_DATE) == "") Msg = "ยังไม่มีข้อมูล Calibration!!!";
                            ////}

                            SaveConfirmData3();
                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                        }
                        break;

                    case "CONFIRM_4":   //-- 20 days
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            SaveConfirmData4();
                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                        }
                        break;

                    case "SELECTALL":
                        //if (string.IsNullOrEmpty(Validation.GetParamStr("chkAll"))) //-- ถ้า checked จะเป็นค่า on
                        //--- เนื่องจากตรวจสอบแล้ว chkAll ส่งค่าเป็น on เสมอ เลยต้องเก็บค่าไว้ในคัวแปร
                        if ( Utility.GetCtrl(hidSELECTALL) == "Y"  ) //-- สลับค่า 
                        {
                            Utility.SetCtrl(hidSELECTALL, "N");
                            SelectAllFlag = "N";
                        }
                        else
                        {
                            Utility.SetCtrl(hidSELECTALL, "Y");
                            SelectAllFlag = "Y";
                        }

                        //LoadData();
                        LoadDataSession();
                        break;

                    case "IMPORT_XLS":
                        //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน hidden = EditItemIndex  เก็บเป็น ,2,3,5
                        //if ( hEditItemIndex != "") //--- กำลังกดปุ่ม Edit อยู่ มีการแสดง text box ในตาราง ต้อง clear text box ออก
                        //{
                        //-- เพิ่ม multi-header ทำให้ต้อง load ใหม่เสมอ
                            LoadDataSession();
                        //}

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


                    case "EDIT":
                        //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน EditItemIndex  เก็บเป็น ,2,3,5
                        LoadDataSession();
                        gvResult.FooterRow.Visible = false;

                        break;
                    case "SAVE":
                        //กดปุ่ม save ให้บันทึกทุก row edit 
                        SaveData();
                        break;

                    //-- 02/07/2019 ---------------------------------------
                    case "EDIT_SPOT":
                        LoadDataSession();
                        break;
                    case "SAVE_SPOT":
                        SaveSPOTData();
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
                pnlCONFIRM.Visible = (canEdit) ? true : false;

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
                DT = Project.dal.SearchSiteFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                Utility.LoadMonthCombo(ref ddlMONTH);
                Utility.LoadYearCombo(ref ddlYEAR, "2015");

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

        private void InitSession()
        {
            try
            {
                for (int c = 3; c < 30; c++)
                {
                    Session["ALERT_COL" + Utility.ToString(c) + gSessionID] = "";

                }
            }
            catch (Exception ex)
            {
                
            }

        }


        private void LoadData()
        {
            DataTable DT = null;

            try
            {
                divSTANDARD.Visible = false; //-- edit 28/08/2020
 
                if (Validation.GetCtrlIntStr(ddlFID) != "" && Validation.GetCtrlIntStr(ddlMONTH) != "" && Validation.GetCtrlIntStr(ddlYEAR) != "")
                {
                    InitSession();



                    //-- ค้นหา NGBILL_RPT_NO เพื่อส่งไปยัง NGBILL_DAILY_UPDATE
                    gNgRptNo27 = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "27DAY");
                    gNgRptNoEND = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "ENDMTH");
                    gNgRptNoDaily = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "DAILY");
                    gNgRptNoDaily27 = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "DAILY27");
                    gNgRptNo20 = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "20DAY");
                    gNgRptNoDaily20 = Project.dal.GetNgbillRptNo(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", "DAILY20");

                    fromDate = "01/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                    toDate = Utility.AppFormatDate(Convert.ToDateTime(Utility.AppDateValue(fromDate)).AddMonths(1).AddDays(-1));

                    toDate27 = "27/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                    Date27 = Convert.ToDateTime(Utility.AppDateValue(toDate27));
                    fromDate27 = Utility.AppFormatDate(Date27.AddMonths(-1).AddDays(1));   //AVERAGE(27) คือคำนวณตั้งแต่ 28 ของเดือนที่แล้ว ถึง 27 ของเดือนที่เลือก
                    Date31 = Convert.ToDateTime(Utility.AppDateValue(toDate));  //วันที่สิ้นเดือนของเดือนที่เลือก

                    toDate20 = "20/" + Validation.GetCtrlIntStr(ddlMONTH).PadLeft(2, '0') + "/" + Validation.GetCtrlIntStr(ddlYEAR);
                    Date20 = Convert.ToDateTime(Utility.AppDateValue(toDate20));
                    fromDate20 = Utility.AppFormatDate(Date20.AddMonths(-1).AddDays(1));   //AVERAGE(20) คือคำนวณตั้งแต่ 21 ของเดือนที่แล้ว ถึง 20 ของเดือนที่เลือก

                    Utility.SetCtrl(hidgNgRptNo27, gNgRptNo27);
                    Utility.SetCtrl(hidgNgRptNoEND, gNgRptNoEND);
                    Utility.SetCtrl(hidgNgRptNoDaily, gNgRptNoDaily);
                    Utility.SetCtrl(hidgNgRptNoDaily27, gNgRptNoDaily27);
                    Utility.SetCtrl(hidgNgRptNo20, gNgRptNo20);
                    Utility.SetCtrl(hidgNgRptNoDaily20, gNgRptNoDaily20);
                    Utility.SetCtrl(hidfromDate, fromDate);
                    Utility.SetCtrl(hidtoDate, toDate);
                    Utility.SetCtrl(hidtoDate27, toDate27);
                    Utility.SetCtrl(hidfromDate27, fromDate27);
                    Utility.SetCtrl(hidtoDate20, toDate20);
                    Utility.SetCtrl(hidfromDate20, fromDate20);


                    DT = Project.dal.SearchGqmsDailyUpdateALL(Validation.GetCtrlIntStr(ddlFID), "", fromDate, toDate, NgBillRptNo: gNgRptNoDaily);

                    //-- ตรวจสอบว่าเป็น ISO Site หรือไม่ ถ้าใช่ต้องตรวจสอบค่า ว่าตรงกับเงื่อนไข ISO หรือไม่
                    //  ค่า ISO ต้องอยู่ในช่วง x/ 2 – 2x     
                    //  ต้องตรวจสอบค่า gas composition ต้องไม่ต่ำกว่า x / 2  และมากกว่า 2x ทศนิยม 6 ตำแหน่ง
                    //  ยกเว้น H2S ไม่ต้องตรวจสอบ
                    //  ในกรณีที่มีค่า ไม่ตามเกณฑ์ ISO ในรายงานให้เอา logo iso ออก
                    //-- ค้นหา ISO-> Standard Gas Composition 
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        if (Utility.ToString(DT.Rows[0]["ISO_FLAG"]) == "Y")
                        {
                            String YYMM = Utility.ToString(Utility.ToNum(ddlYEAR.Text) * 100 + Utility.ToNum(ddlMONTH.SelectedValue));
                            String OtherCri = "( TO_CHAR(ORDER_DATE,'YYYYMM') < " + YYMM + " OR ORDER_DATE IS NULL) " +
                                              " AND (TO_CHAR(EXPIRE_DATE, 'YYYYMM') >= " + YYMM + " OR EXPIRE_DATE IS NULL)  ";
                            DataTable gasDT = Project.dal.SearchSiteSgc(Validation.GetCtrlIntStr(ddlFID), OtherCriteria: OtherCri);
                            if (gasDT != null && gasDT.Rows.Count > 0)
                            {
                                DataRow gasDR = Utility.GetDR(ref gasDT);

                                gISO_FLAG = "Y";
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

                                //-- EDIT 19/07/2019 --
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


                                //-- edit 28/08/2020 -- แจ้งเตือน Standard ใกล้หมดอายุ
                                if (Utility.ToNum(Project.gStandardExpireAlert) != 0)
                                {
                                    DateTime curr = System.DateTime.Today.AddDays(Utility.ToDouble(Project.gStandardExpireAlert));
                                    decimal currYMD = Utility.ToNum(Utility.FormatDate(curr, "YYYYMMDD"));

                                    if (gexpireYMD < currYMD)
                                    {
                                        divSTANDARD.Visible = true;
                                        Utility.SetCtrl(lblCYLINDER_NO, Utility.ToString(gasDR["CYLINDER_NO"]));
                                        Utility.SetCtrl(lblORDER_DATE, Utility.AppFormatDate(gasDR["ORDER_DATE"]));
                                        Utility.SetCtrl(lblEXPIRE_DATE, Utility.AppFormatDate(gasDR["EXPIRE_DATE"]));
                                    }

                                }




                            }
                            Utility.ClearObject(ref gasDT);
                        }
                        else
                        {
                            gISO_FLAG = "N";
                            Utility.SetCtrl(hidgISO_FLAG, gISO_FLAG);
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
                            //-- EDIT 19/07/2019 --
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
                        }

                        Session["GQMS_DAILY_ALL"+ gSessionID] = DT;
                        chkCount = DT.Rows.Count;
                        pnlCONFIRM.Visible = true;
                          

                    }
                    else
                    {
                        chkCount = 0;
                        pnlCONFIRM.Visible = false;
                    }

                    //--- Comfirm button -----------------------
                    if ( pnlCONFIRM.Visible == true )
                    {
                        //----  Last Comfirm -----------------------------------------------
                        if (gNgRptNoDaily != "")
                        {
                            //-- edit 29/11/2019 ---
                            if (System.DateTime.Today > Convert.ToDateTime(Utility.AppDateValue(fromDate)) )
                            {
                                pnlComfirm1.Visible = true;
                                pnlComfirm12.Visible = false;
                            }
                            else
                            {
                                pnlComfirm1.Visible = false;
                                pnlComfirm12.Visible = true;
                            }
                            
                            ShowConfirm(Project.catConfDAILY, ref lblComfirm1);
                        }
                        else
                        {
                            pnlComfirm1.Visible = false;
                            lblComfirm1.Visible = false;
                        }
                        //-----------------------------------
                        if (gNgRptNo27 != "")
                        {
                            //-- edit 29/11/2019 ---
                            if ( System.DateTime.Today > Date27)
                            {
                                //AVERAGE(27) คือคำนวณตั้งแต่ 28 ของเดือนที่แล้ว ถึง 27 ของเดือนที่เลือก
                                pnlComfirm2.Visible = true;
                                pnlComfirm22.Visible = false;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 27 และสิ้นเดือน
                            }
                            else
                            {
                                pnlComfirm2.Visible = false;
                                pnlComfirm22.Visible = true;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 27 และสิ้นเดือน
                            }
                            ShowConfirm(Project.catConf27DAY, ref lblComfirm2);
                        }
                        else
                        {
                            pnlComfirm2.Visible = false;
                            lblComfirm2.Visible = false;
                        }
                        //-----------------------------------
                        if (gNgRptNoEND != "")
                        {
                            //-- edit 29/11/2019 ---
                            if (System.DateTime.Today > Date31)
                            {
                                //AVERAGE(31) คือคำนวณตั้งแต่ 1 ถึง 31 ของเดือนที่เลือก
                                pnlComfirm3.Visible = true;
                                pnlComfirm32.Visible = false;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 27 และสิ้นเดือน
                            }
                            else
                            {
                                pnlComfirm3.Visible = false;
                                pnlComfirm32.Visible = true;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 27 และสิ้นเดือน
                            }

                            ShowConfirm(Project.catConfENDMTH, ref lblComfirm3);

                        }
                        else
                        {
                            pnlComfirm3.Visible = false;
                            lblComfirm3.Visible = false;
                        }
                        //----------------------------------- edit 29/11/2019 
                        if (gNgRptNo20 != "")
                        {
                            //-- edit 29/11/2019 ---
                            if (System.DateTime.Today > Date20)
                            {
                                //AVERAGE(20) คือคำนวณตั้งแต่ 21 ของเดือนที่แล้ว ถึง 20 ของเดือนที่เลือก
                                pnlComfirm4.Visible = true;
                                pnlComfirm42.Visible = false;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 20 และสิ้นเดือน
                            }
                            else
                            {
                                pnlComfirm4.Visible = false;
                                pnlComfirm42.Visible = true;  //-- กำหนดให้ปุ่ม disable ถ้ายังไม่ถึงวันที่ที่กำหนด เช่น 20 และสิ้นเดือน
                            }

                            ShowConfirm(Project.catConf20DAY, ref lblComfirm4);
                        }
                        else
                        {
                            pnlComfirm4.Visible = false;
                            lblComfirm4.Visible = false;
                        }
                    }

                    if (pnlComfirm1.Visible || pnlComfirm2.Visible || pnlComfirm3.Visible || pnlComfirm4.Visible
                        || pnlComfirm12.Visible || pnlComfirm22.Visible || pnlComfirm32.Visible || pnlComfirm42.Visible) //-- ตรวจสอบการแสดงปุ่มอีกที
                    {
                        pnlCONFIRM.Visible = true;
                    }
                    else
                    {
                        pnlCONFIRM.Visible = false;
                    }


                    //---- Bound Data Table --------------------------------------------------------
                    Utility.BindGVData(ref gvResult, DT, false);
                    gvResult.FooterRow.Visible = true;
                    //---- Bound Data Table --------------------------------------------------------

                    //--- กำหนดชื่อ site OMA
                    gvResult.HeaderRow.Cells[26].Text = gOmaName1;
                    gvResult.HeaderRow.Cells[27].Text = gOmaName2;
                    gvResult.HeaderRow.Cells[29].Text = gFlowName1;
                    gvResult.HeaderRow.Cells[30].Text = gFlowName2;

                    //-- ถ้า check box ทุกตัวถูก checked ให้กำหนด chkAll เป็น checked
                    if ( cntChecked == gvResult.Rows.Count ) 
                    {
                        gvResult.HeaderRow.Cells[0].Text = "<input type='checkbox' name='chkAll' id='chkAll' class='check_All' checked='checked' />";
                        
                        Utility.SetCtrl(hidSELECTALL, "Y");
                    }
                    else
                    {
                        Utility.SetCtrl(hidSELECTALL, "N");
                    }



                    //*** EDIT 02/07/2019 ********************************************************************
                    //-- ตรวจสอบว่า fid นี้ใช้ spot อะไรไว้ด้วยว่า fid+month นี้ใช้ spot ตัวไหน เพราะอาจจะเลือกสำรอง
                    DataTable DTsp = Project.dal.SearchSpotFID(Utility.GetCtrl(hidFID), fromDate);
                    if ( DTsp == null || DTsp.Rows.Count == 0)
                    {   //กรณีไม่มีข้อมูล ให้ดูว่ากำหนด spot ไว้หรือไม่  ถ้ากำหนดไว้ให้ update data 
                        DataTable DTf = Project.dal.SearchSiteFID(FID: Utility.GetCtrl(hidFID));
                        if (DTf != null && DTf.Rows.Count > 0)
                        {
                            DataRow DRf = Utility.GetDR(ref DTf);
                            if (Utility.ToString(DRf["H2S_NAME1"])+Utility.ToString(DRf["HG_NAME1"])+Utility.ToString(DRf["O2_NAME1"])+Utility.ToString(DRf["HC_NAME1"]) != "")
                            {
                                Project.dal.MngOSpotUpdate(DBUTIL.opINSERT, Utility.GetCtrl(hidFID), fromDate, Utility.ToString(DRf["H2S_NAME1"]), Utility.ToString(DRf["HG_NAME1"]), Utility.ToString(DRf["O2_NAME1"]), Utility.ToString(DRf["HC_NAME1"]));

                                DTsp = Project.dal.SearchSpotFID(Utility.GetCtrl(hidFID), fromDate); //reload 
                            }
                           
                        }

                    }

                    DataTable DT1 = null;
                    if (DTsp != null && DTsp.Rows.Count > 0)
                    {
                        DataRow DRsp = Utility.GetDR(ref DTsp);

                        Utility.SetCtrl(hidsH2S_NAME, Utility.ToString(DRsp["H2S_NAME"]));
                        Utility.SetCtrl(hidsHG_NAME, Utility.ToString(DRsp["HG_NAME"]));
                        Utility.SetCtrl(hidsO2_NAME, Utility.ToString(DRsp["O2_NAME"]));
                        Utility.SetCtrl(hidsHC_NAME, Utility.ToString(DRsp["HC_NAME"]));

                        if (Utility.ToString(DRsp["H2S_NAME"]) + Utility.ToString(DRsp["HG_NAME"])+ Utility.ToString(DRsp["O2_NAME"])+ Utility.ToString(DRsp["HC_NAME"]) != "")
                        {
                            pnlSPOT.Visible = true;

                            Session["SPOT_" + gSessionID] = DTsp;

                            Utility.BindGVData(ref gvSpot, DTsp, false);
                            if (Utility.GetCtrl(hidsH2S_NAME) == "")
                            {
                                gvSpot.Columns[1].Visible = false;
                                gvSpot.Columns[2].Visible = false;
                                gvSpot.Columns[3].Visible = false;
                            }
                            else
                            {
                                gvSpot.Columns[1].Visible = true;
                                gvSpot.Columns[2].Visible = true;
                                gvSpot.Columns[3].Visible = true;
                            }
                            if (Utility.GetCtrl(hidsHG_NAME) == "")
                            {
                                gvSpot.Columns[4].Visible = false;
                                gvSpot.Columns[5].Visible = false;
                                gvSpot.Columns[6].Visible = false;
                            }
                            else
                            {
                                gvSpot.Columns[4].Visible = true;
                                gvSpot.Columns[5].Visible = true;
                                gvSpot.Columns[6].Visible = true;
                            }
                            if (Utility.GetCtrl(hidsO2_NAME) == "")
                            {
                                gvSpot.Columns[7].Visible = false;
                                gvSpot.Columns[8].Visible = false;
                                gvSpot.Columns[9].Visible = false;
                            }
                            else
                            {
                                gvSpot.Columns[7].Visible = true;
                                gvSpot.Columns[8].Visible = true;
                                gvSpot.Columns[9].Visible = true;
                            }
                            if (Utility.GetCtrl(hidsHC_NAME) == "")
                            {
                                gvSpot.Columns[10].Visible = false;
                                gvSpot.Columns[11].Visible = false;
                                gvSpot.Columns[12].Visible = false;
                            }
                            else
                            {
                                gvSpot.Columns[10].Visible = true;
                                gvSpot.Columns[11].Visible = true;
                                gvSpot.Columns[12].Visible = true;
                            }

                        }
                        else
                        {
                            pnlSPOT.Visible = false;
                        }

                    }
                    Utility.ClearObject(ref DTsp);
                    Utility.ClearObject(ref DT1);




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

                gNgRptNo27 = Utility.GetCtrl(hidgNgRptNo27);
                gNgRptNoEND = Utility.GetCtrl(hidgNgRptNoEND);
                gNgRptNoDaily = Utility.GetCtrl(hidgNgRptNoDaily);
                gNgRptNoDaily27 = Utility.GetCtrl(hidgNgRptNoDaily27);
                gNgRptNo20 = Utility.GetCtrl(hidgNgRptNo20);
                gNgRptNoDaily20 = Utility.GetCtrl(hidgNgRptNoDaily20);

                fromDate = Utility.GetCtrl(hidfromDate);
                toDate = Utility.GetCtrl(hidtoDate);
                fromDate27 = Utility.GetCtrl(hidfromDate27);
                toDate27 = Utility.GetCtrl(hidtoDate27);
                Date31 = Convert.ToDateTime(Utility.AppDateValue(toDate));  //วันที่สิ้นเดือนของเดือนที่เลือก
                fromDate20 = Utility.GetCtrl(hidfromDate20);
                toDate20 = Utility.GetCtrl(hidtoDate20);

                gISO_FLAG = Utility.GetCtrl(hidgISO_FLAG);
                gorderYMD = Utility.ToNum(Utility.GetCtrl(hidgorderYMD));
                gexpireYMD = Utility.ToNum(Utility.GetCtrl(hidgexpireYMD));
                gC1 = Utility.ToDouble(Utility.GetCtrl(hidgC1));
                gC2 = Utility.ToDouble(Utility.GetCtrl(hidgC2));
                gC3 = Utility.ToDouble(Utility.GetCtrl(hidgC3));
                gIC4 = Utility.ToDouble(Utility.GetCtrl(hidgIC4));
                gNC4 = Utility.ToDouble(Utility.GetCtrl(hidgNC4));
                gIC5 = Utility.ToDouble(Utility.GetCtrl(hidgIC5));
                gNC5 = Utility.ToDouble(Utility.GetCtrl(hidgNC5));
                gC6 = Utility.ToDouble(Utility.GetCtrl(hidgC6));
                gN2 = Utility.ToDouble(Utility.GetCtrl(hidgN2));
                gCO2 = Utility.ToDouble(Utility.GetCtrl(hidgCO2));
                gH2S = Utility.ToDouble(Utility.GetCtrl(hidgH2S));
                //-- EDIT 19/07/2019 --
                gC1_MIN = Utility.ToDouble(Utility.GetCtrl(hidgC1_MIN));
                gC2_MIN = Utility.ToDouble(Utility.GetCtrl(hidgC2_MIN));
                gC3_MIN = Utility.ToDouble(Utility.GetCtrl(hidgC3_MIN));
                gIC4_MIN = Utility.ToDouble(Utility.GetCtrl(hidgIC4_MIN));
                gNC4_MIN = Utility.ToDouble(Utility.GetCtrl(hidgNC4_MIN));
                gIC5_MIN = Utility.ToDouble(Utility.GetCtrl(hidgIC5_MIN));
                gNC5_MIN = Utility.ToDouble(Utility.GetCtrl(hidgNC5_MIN));
                gC6_MIN = Utility.ToDouble(Utility.GetCtrl(hidgC6_MIN));
                gN2_MIN = Utility.ToDouble(Utility.GetCtrl(hidgN2_MIN));
                gCO2_MIN = Utility.ToDouble(Utility.GetCtrl(hidgCO2_MIN));
                gH2S_MIN = Utility.ToDouble(Utility.GetCtrl(hidgH2S_MIN));
                gC1_MAX = Utility.ToDouble(Utility.GetCtrl(hidgC1_MAX));
                gC2_MAX = Utility.ToDouble(Utility.GetCtrl(hidgC2_MAX));
                gC3_MAX = Utility.ToDouble(Utility.GetCtrl(hidgC3_MAX));
                gIC4_MAX = Utility.ToDouble(Utility.GetCtrl(hidgIC4_MAX));
                gNC4_MAX = Utility.ToDouble(Utility.GetCtrl(hidgNC4_MAX));
                gIC5_MAX = Utility.ToDouble(Utility.GetCtrl(hidgIC5_MAX));
                gNC5_MAX = Utility.ToDouble(Utility.GetCtrl(hidgNC5_MAX));
                gC6_MAX = Utility.ToDouble(Utility.GetCtrl(hidgC6_MAX));
                gN2_MAX = Utility.ToDouble(Utility.GetCtrl(hidgN2_MAX));
                gCO2_MAX = Utility.ToDouble(Utility.GetCtrl(hidgCO2_MAX));
                gH2S_MAX = Utility.ToDouble(Utility.GetCtrl(hidgH2S_MAX));

                Utility.BindGVData(ref gvResult, (DataTable)Session["GQMS_DAILY_ALL"+ gSessionID], false);
                gvResult.FooterRow.Visible = true;

                //--- กำหนดชื่อ site OMA
                gvResult.HeaderRow.Cells[26].Text = gOmaName1;
                gvResult.HeaderRow.Cells[27].Text = gOmaName2;
                gvResult.HeaderRow.Cells[29].Text = gFlowName1;
                gvResult.HeaderRow.Cells[30].Text = gFlowName2;


                //-- EDIT 19/07/2019 --
                if (pnlSPOT.Visible )
                    Utility.BindGVData(ref gvSpot, (DataTable)Session["SPOT_" + gSessionID], false);

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }
        }


        //-- EDIT 28/08/2020 -- ดึงข้อมูล As Found+As Left+Final Cal จากระบบ OGC Data
        private void LoadCalibrateData()
        {
            DataTable DT = null;

            try
            {
                divASFOUND.Visible = false;

                if (Validation.GetCtrlIntStr(ddlFID) != "" && Validation.GetCtrlIntStr(ddlMONTH) != "" && Validation.GetCtrlIntStr(ddlYEAR) != "")
                {
                    string sql = "SELECT S.OSITE_ID, A.*  " +
                    " FROM C_SITE_FID S LEFT OUTER JOIN " +
                    " (SELECT C.*FROM C_CALIBRATE C INNER JOIN C_SITE_FID S ON C.CSITE_ID = S.CSITE_ID " +
                    " WHERE C.WORK_TYPE IN('ML2','ML3') AND C.MM =" + Validation.GetCtrlIntStr(ddlMONTH) + " AND C.YY =" + Validation.GetCtrlIntStr(ddlYEAR) + " AND S.OSITE_ID =" + Validation.GetCtrlIntStr(ddlFID) + ") A ON A.CSITE_ID = S.CSITE_ID " +
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

        private void ShowConfirm(string Cat, ref Label lblConfirm)
        {
            DataTable DTL = null;
            string dConfirm = "", uConfirm = "", show = "";
            try
            {
                 DTL = Project.dal.SearchLastEventLog("", "", null, Cat, "EVENT_DETAIL LIKE '%FID=" + ddlFID.SelectedItem.Text + ";Month/Year=" + ddlMONTH.SelectedValue + "/" + ddlYEAR.SelectedValue + ";%'");
                if (DTL.Rows.Count > 0)
                {
                    dConfirm = Utility.AppFormatDateTime(DTL.Rows[0]["TRANS_DATE"]);
                    uConfirm = Utility.ToString(DTL.Rows[0]["USER_NAME"]);
                    show = "Comfirmed: " + Utility.AppFormatDateTime(dConfirm) + " By: " + uConfirm;
                    lblConfirm.Text = show;
                }
                else
                {
                    lblConfirm.Text = "";
                }
               
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DTL);
            }

        }


        private void SaveData()
        {
            int ItemIndex = 0;
            String ChkDate = "";
            object pC1 = null, pC2 = null, pC3 = null, pIC4 = null, pNC4 = null, pIC5 = null, pNC5 = null, pC6 = null, pN2 = null;
            object pCO2 = null, pSG = null, pGHV = null, pNHV = null, pWC = null, pUnnorm = null, pUnnormMin = null, pUnnormMax = null;
            object pWB = null, pH2S = null;

            object pOMA_NAME = null, pOGC_NAME = null;
            try
            {
                //--- จะบันทึกลงที่ O_TMP_DAILY_GQMS แล้ว update ทีเดียว เหมือนกับ import exel
                if ( EditItemIndex != "")
                {
                    //-- delete tmp data
                    Project.dal.MngTmpDailyGqms(DBUTIL.opDELETE, "", "", Utility.GetCtrl(hidFID));

                    //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน EditItemIndex  เก็บเป็น ,2,3,5,
                    String[] rw = EditItemIndex.Split(',');

                    for (int r = 0; r < rw.Length; r++)
                    {
                        if (rw[r] != "")
                        {
                            ItemIndex = Utility.ToInt(rw[r]);
                            ChkDate = gvResult.DataKeys[ItemIndex].Value.ToString();

                            pC1 = Validation.ValidateStr(Request.Form["txtC1_" + ItemIndex]);
                            pC2 = Validation.ValidateStr(Request.Form["txtC2_" + ItemIndex]);
                            pC3 = Validation.ValidateStr(Request.Form["txtC3_" + ItemIndex]);
                            pIC4 = Validation.ValidateStr(Request.Form["txtIC4_" + ItemIndex]);
                            pNC4 = Validation.ValidateStr(Request.Form["txtNC4_" + ItemIndex]);
                            pIC5 = Validation.ValidateStr(Request.Form["txtIC5_" + ItemIndex]);
                            pNC5 = Validation.ValidateStr(Request.Form["txtNC5_" + ItemIndex]);
                            pC6 = Validation.ValidateStr(Request.Form["txtC6_" + ItemIndex]);
                            pCO2 = Validation.ValidateStr(Request.Form["txtCO2_" + ItemIndex]);
                            pN2 = Validation.ValidateStr(Request.Form["txtN2_" + ItemIndex]);
                            pH2S = Validation.ValidateStr(Request.Form["txtH2S_" + ItemIndex]);
                            pNHV = Validation.ValidateStr(Request.Form["txtNHV_" + ItemIndex]);
                            pGHV = Validation.ValidateStr(Request.Form["txtGHV_" + ItemIndex]);
                            pSG = Validation.ValidateStr(Request.Form["txtSG_" + ItemIndex]);
                            pWC = Validation.ValidateStr(Request.Form["txtWC_" + ItemIndex]);
                            pUnnormMin = Validation.ValidateStr(Request.Form["txtUNNORMMIN_" + ItemIndex]);
                            pUnnormMax = Validation.ValidateStr(Request.Form["txtUNNORMMAX_" + ItemIndex]);
                            pUnnorm = Validation.ValidateStr(Request.Form["txtUNNORMALIZED_" + ItemIndex]);
                            pWB = Validation.ValidateStr(Request.Form["txtWB_" + ItemIndex]);

                            if (pC1.ToString() == "") pC1 = null;       if (pC2.ToString() == "") pC2 = null;
                            if (pC3.ToString() == "") pC3 = null;       if (pIC4.ToString() == "") pIC4 = null;
                            if (pNC4.ToString() == "") pNC4 = null;     if (pIC5.ToString() == "") pIC5 = null;
                            if (pNC5.ToString() == "") pNC5 = null;     if (pC6.ToString() == "") pC6 = null;
                            if (pCO2.ToString() == "") pCO2 = null;     if (pN2.ToString() == "") pN2 = null;
                            if (pH2S.ToString() == "") pH2S = null;     if (pNHV.ToString() == "") pNHV = null;
                            if (pGHV.ToString() == "") pGHV = null;     if (pSG.ToString() == "") pSG = null;
                            if (pWC.ToString() == "") pWC = null;       if (pUnnormMin.ToString() == "") pUnnormMin = null;
                            if (pUnnormMax.ToString() == "") pUnnormMax = null;     if (pUnnorm.ToString() == "") pUnnorm = null;
                            if (pWB.ToString() == "") pWB = null;

                            pOMA_NAME = Validation.ValidateStr(Request.Form["ddlOMA_" + ItemIndex]);
                            pOGC_NAME = Validation.ValidateStr(Request.Form["ddlOGC_" + ItemIndex]);
                            if (pOMA_NAME.ToString() == "") pOMA_NAME = null;
                            if (pOGC_NAME.ToString() == "") pOGC_NAME = null;

                            Project.dal.MngTmpDailyGqms(DBUTIL.opINSERT, "", ChkDate, Utility.GetCtrl(hidFID), pC1, pC2, pC3, pIC4, pNC4, pIC5, pNC5, pC6, pN2, pCO2, pSG, pGHV, pNHV, pWC, pUnnorm, pUnnormMin, pUnnormMax, pWB, pH2S, OMA_NAME:pOMA_NAME, OGC_NAME: pOGC_NAME);
                        }
                   }

                    Project.dal.MngTmp2GqmsDailyUpdate(Utility.GetCtrl(hidFID));

                    //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน hidden = EditItemIndex  เก็บเป็น ,2,3,5
                    hEditItemIndex = "";

                }              

                LoadData();

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
 
        }



        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        //--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataTable aDTavg27 = null, aDTavg31 = null, aDTmin = null, aDTmax = null, aDTng27 = null, aDTng31 = null;
            DataTable aDTavg20 = null, aDTng20 = null;
            DataRow aDR = null;
            
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    String chked = "";
                    if (SelectAllFlag == "Y") chked = " checked='checked' ";
                    //e.Row.Cells[0].Text = "<input type='checkbox' name='chkAll' id='chkAll' onclick='javascript:SelectAll();' " + chked + " />";

                    e.Row.Cells[0].Text = "<input type='checkbox' name='chkAll' id='chkAll' class='check_All' " + chked + " />";


                    //-- edit 02/10/2019 -- ย้ายหน่วยไปอยู่บรรทัดที่ 2 ไม่อย่างนั้นจะเกิด error ตอนอ่าน datarow
                    ////create a new row  (UNIT)-------------------------------------------------------------------
                    ////cast the sender back to a gridview
                    //GridView gv = sender as GridView;
                    //TableCell cell = null;
                    //e.Row.TableSection = TableRowSection.TableHeader;
                    //GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                    //extraHeader.TableSection = TableRowSection.TableHeader;

                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 30; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 40; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "unit";
                    //cell.Width = 90; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "mole %";
                    //cell.ColumnSpan = 10; cell.Width = 700; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "ppm";          //-- H2S column 13
                    //cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "Btu/scf";          //-- NETHVDRY,HVSAT column 14,15
                    //cell.ColumnSpan = 2; cell.Width = 140; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "";          //-- SG
                    //cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "Lb/MMscf";          //-- H2O
                    //cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "mole %";          //-- UNNORM column 18,19
                    //cell.ColumnSpan = 3; cell.Width = 250; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "Btu/scf";          //-- WI
                    //cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    //cell = new TableCell(); cell.Text = "";          //-- SUM
                    //cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                    ////-- OGC column 23-25
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);
                    //cell = new TableCell(); cell.Text = "Btu/scf";
                    //cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);

                    ////-- OMA(H2O) column 26-28
                    //cell = new TableCell(); cell.Text = "Lb/MMscf";
                    //cell.Width = 70; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);
                    //cell = new TableCell(); cell.Text = "Lb/MMscf";
                    //cell.Width = 70; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 80; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);

                    ////-- Flow column 29-30
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 70; cell.CssClass = "Table-head-success cell-center"; extraHeader.Cells.Add(cell);
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 70; cell.CssClass = "Table-head-success cell-center"; extraHeader.Cells.Add(cell);

                    ////-- OGC site สำหรอง column 31
                    //cell = new TableCell(); cell.Text = "";
                    //cell.Width = 80; cell.CssClass = "Table-head-danger cell-center"; extraHeader.Cells.Add(cell);

                    ////add the new row to the gridview
                    //gv.Controls[0].Controls.AddAt(2, extraHeader);
                    ////---------------------------------------------------------------

                }
                else
                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    //-- Check box เมื่อส่งไป NGBILL แล้วและ NGBILL.MODIFIED_DATE >=GQMS.MODIFIED_DATE ให้ checked
                    //   แต่ถ้าไม่เคยส่ง หรือ มีการแก้ไขที่ GQMS ให้ unchecked
                    //-- confirm checkbox column 0      checked="checked"
                    String chked = "";

                    if (Utility.ToString(dr["NGMODIFIED_DATE"]) != "" && Utility.ToString(dr["MODIFIED_DATE"]) != "" && DateTime.Compare(Convert.ToDateTime(dr["NGMODIFIED_DATE"]), Convert.ToDateTime(dr["MODIFIED_DATE"])) >= 0)
                    {
                        chked = " checked=\"checked\" ";
                        cntChecked++;
                        //-- กำหนดสีพื้นคอลัมน์ TIME เพื่อระบุว่าได้ส่งข้อมูลรายวันไปยัง NGBILL_DAILY_UPDATE แล้ว  (QCTxxx)
                        e.Row.Cells[2].CssClass = "cell-center cell-Middle cell-border cell-bg-confirmed";
                    }
                    e.Row.Cells[0].Text = "<input type=\"checkbox\" id=\"chkSelect" + e.Row.DataItemIndex + "\" name=\"chkSelect" + e.Row.DataItemIndex + "\" " + chked + " /> ";


                    //String gOmaName = "", gOmaName1 = "", gOmaName2 = "", gFlowName1 = "", gFlowName2 = ""; //OMA name, FLOW name
                    gOmaName = Utility.ToString(dr["OMA_NAME"]);
                    if (gOmaName1 == "") gOmaName1 = Utility.ToString(dr["OMA_NAME1"]);
                    if (gOmaName2 == "") gOmaName2 = Utility.ToString(dr["OMA_NAME2"]);
                    
                    if (gFlowName1 == "") gFlowName1 = Utility.ToString(dr["FLOW_NAME1"]);
                    if (gFlowName2 == "") gFlowName2 = Utility.ToString(dr["FLOW_NAME2"]);
                    gOgcName = Utility.ToString(dr["OGC_NAME"]);
                    if (gOgcName1 == "") gOgcName1 = Utility.ToString(dr["OGC_NAME1"]);
                    if (gOgcName2 == "") gOgcName2 = Utility.ToString(dr["OGC_NAME2"]);
                    if (gOgcName3 == "") gOgcName3 = Utility.ToString(dr["OGC_NAME3"]);

                    if (ServerAction == "EDIT" && EditItemIndex.IndexOf(',' + e.Row.DataItemIndex.ToString() + ',') > -1)
                    {
                        e.Row.CssClass = "itemSelect";
                        //-- edit column 1
                        e.Row.Cells[1].Text = "<li class=\"fa fa-save fa-lg\" style=\"color: #009933;\" onclick=\"DoAction('SAVE'," + e.Row.DataItemIndex + ")\"></li> ";
                    }

                    //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                    //-- OGC column 3-21, SUM column 22
                    ShowValue(ref e, 3, 22, dr); 

                    //-- OGC BTU column 23-25
                    ShowValue(ref e, 23, 25, dr);

                    //-- OMA column 26-27, เลือก Site 28
                    ShowValue(ref e, 26, 28, dr);

                    //-- FLOW column 29-30, เลือก Site 31
                    ShowValue(ref e, 29, 31, dr);


                    //-- click popup OMA HOUR 
                    if (e.Row.Cells[26].Text != "")
                        e.Row.Cells[26].Attributes.Add("onclick", "javascript:DoAction('OMA','" + Validation.EncodeParam(gOmaName1) + "','" + Validation.EncodeParam(Utility.AppFormatDate(dr["RDATE"])) + "');");
                    if (e.Row.Cells[27].Text != "")
                        e.Row.Cells[27].Attributes.Add("onclick", "javascript:DoAction('OMA','" + Validation.EncodeParam(gOmaName2) + "','" + Validation.EncodeParam(Utility.AppFormatDate(dr["RDATE"])) + "');");


                    //-- click popup FLOW HOUR 
                    if (e.Row.Cells[29].Text != "")
                        e.Row.Cells[29].Attributes.Add("onclick", "javascript:DoAction('FLOW','" + Validation.EncodeParam(gFlowName1) + "','" + Validation.EncodeParam(Utility.AppFormatDate(dr["RDATE"])) + "');");
                    if (e.Row.Cells[30].Text != "")
                        e.Row.Cells[30].Attributes.Add("onclick", "javascript:DoAction('FLOW','" + Validation.EncodeParam(gFlowName2) + "','" + Validation.EncodeParam(Utility.AppFormatDate(dr["RDATE"])) + "');");




                }
                else
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    if (ServerAction != "EDIT")
                    {
                        //-- edit 29/11/2019 --- เพิ่ม confirm 20 days
                        //row=> AVERAGE(20) ==============================================================================================================
                        //*****  จะคำนวณ เมื่อมีข้อมูลของวันที่ 20 มาแล้วหรือวันที่ปัจจุบันมากกว่า 20  เท่านั้น *****
                        e.Row.Cells[2].Text = "AVERAGE (20)";

                        //AVERAGE(20) คือคำนวณตั้งแต่ 21 ของเดือนที่แล้ว ถึง 20 ของเดือนที่เลือก
                        if (LoadSessionFlag)
                                aDTavg20 = (DataTable)Session["GQMS_AVG20" + gSessionID];
                        else
                                aDTavg20 = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate20, toDate20);
                        if (aDTavg20 != null && aDTavg20.Rows.Count > 0)
                        {
                            Session["GQMS_AVG20" + gSessionID] = aDTavg20;
                            aDR = Utility.GetDR(ref aDTavg20);
                            //-- OGC column 3-21, SUM column 22
                            ShowFooterValue(ref e, 3, 22, aDR);
                        }


                        //-- edit 29/11/2019 ---
                        //row=> AVERAGE(27) ==============================================================================================================
                        //*****  จะคำนวณ เมื่อมีข้อมูลของวันที่ 27 มาแล้วหรือวันที่ปัจจุบันมากกว่า 27  เท่านั้น *****
                        GridViewRow extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                        extraFooter.CssClass = "ItemFooter_green";
                        TableCell cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "AVERAGE (27)";
                        extraFooter.Cells.Add(cell);


                        //AVERAGE(27) คือคำนวณตั้งแต่ 28 ของเดือนที่แล้ว ถึง 27 ของเดือนที่เลือก
                        if (LoadSessionFlag)
                            aDTavg27 = (DataTable)Session["GQMS_AVG27" + gSessionID];
                        else
                            aDTavg27 = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate27, toDate27);
                        if (aDTavg27 != null && aDTavg27.Rows.Count > 0)
                        {
                            Session["GQMS_AVG27" + gSessionID] = aDTavg27;
                            aDR = Utility.GetDR(ref aDTavg27);
                            //-- OGC column 3-21, SUM column 22
                            String fd = "", fdValue = "";
                            for (int c = 3; c <= 22; c++)
                            {
                                fd = ConfigCol(c);
                                if (fd != "")
                                {
                                    switch (fd)
                                    {
                                        case "SG":
                                            fdValue = Utility.FormatNum(aDR[fd], 4);
                                            break;
                                        case "WC": //H2O
                                            fdValue = Utility.FormatNum(aDR[fd], 2);
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
                            //-- OGC column 3-21, SUM column 22
                            for (int c = 3; c <= 22; c++)
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

                        //--  column 23-31
                        for (int c = 23; c <= 31; c++)
                        {
                            cell = new TableCell();
                            cell.CssClass = "cell-right cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }

                        gvResult.Controls[0].Controls.Add(extraFooter);



                        //Add row=> AVERAGE(end month) ==============================================================================================================
                        //*****  จะคำนวณ เมื่อมีข้อมูลของวันที่สิ้นเดือน มาแล้ว หรือวันที่ปัจจุบันมากกว่า สิ้นเดือน เท่านั้น *****
                        extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                        extraFooter.CssClass = "ItemFooter_green2";
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "AVERAGE (" + Utility.Left(toDate, 2) + ")";  //AVERAGE (31) 
                        extraFooter.Cells.Add(cell);


                        //AVERAGE(31) คือคำนวณตั้งแต่ 1 ถึง 31 ของเดือนที่เลือก
                        if (LoadSessionFlag)
                            aDTavg31 = (DataTable)Session["GQMS_AVG31" + gSessionID];
                        else
                            aDTavg31 = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);
  

                        if (aDTavg31 != null && aDTavg31.Rows.Count > 0)
                        {
                            Session["GQMS_AVG31" + gSessionID] = aDTavg31;

                            aDR = Utility.GetDR(ref aDTavg31);
                            //-- OGC column 3-21, SUM column 22
                            String fd = "", fdValue = "";
                            for (int c = 3; c <= 22; c++)
                            {
                                fd = ConfigCol(c);
                                if (fd != "")
                                {
                                    switch (fd)
                                    {
                                        case "SG":
                                            fdValue = Utility.FormatNum(aDR[fd], 4);
                                            break;
                                        case "WC": //H2O
                                            fdValue = Utility.FormatNum(aDR[fd], 2);
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
                            //-- OGC column 3-21, SUM column 22
                            for (int c = 3; c <= 22; c++)
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

                        //--  column 23-31
                        for (int c = 23; c <= 31; c++)
                        {
                            cell = new TableCell();
                            cell.CssClass = "cell-right cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }

                        gvResult.Controls[0].Controls.Add(extraFooter);


                        //Add row=> MIN ==============================================================================================================
                        extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);

                        extraFooter.CssClass = "ItemFooter_blue";
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "MIN";
                        extraFooter.Cells.Add(cell);

                        if (LoadSessionFlag)
                            aDTmin = (DataTable)Session["GQMS_MIN" + gSessionID];
                        else
                            aDTmin = Project.dal.SearchGqmsDailyUpdateMIN(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);

                        if (aDTmin != null && aDTmin.Rows.Count > 0)
                        {
                            Session["GQMS_MIN" + gSessionID] = aDTmin;

                            aDR = Utility.GetDR(ref aDTmin);
                            //-- OGC column 3-21, SUM column 22
                            String fd = "", fdValue = "";
                            for (int c = 3; c <= 22; c++)
                            {
                                fd = ConfigCol(c);
                                if (fd != "")
                                {
                                    switch (fd)
                                    {
                                        case "SG":
                                            fdValue = Utility.FormatNum(aDR[fd], 4);
                                            break;
                                        case "WC": //H2O
                                            fdValue = Utility.FormatNum(aDR[fd], 2);
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
                            //-- OGC column 3-21, SUM column 22
                            for (int c = 3; c <= 22; c++)
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
                        //--  column 23-31
                        for (int c = 23; c <= 31; c++)
                        {
                            cell = new TableCell();
                            cell.CssClass = "cell-right cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }
                        gvResult.Controls[0].Controls.Add(extraFooter);


                        //Add row=> MAX ==============================================================================================================
                        extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                        extraFooter.CssClass = "ItemFooter_blue2";
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "";
                        extraFooter.Cells.Add(cell);
                        cell = new TableCell();
                        cell.CssClass = "cell-center cell-Middle cell-border";
                        cell.Text = "MAX";
                        extraFooter.Cells.Add(cell);


                        if (LoadSessionFlag)
                            aDTmax = (DataTable)Session["GQMS_MAX" + gSessionID];
                        else
                            aDTmax = Project.dal.SearchGqmsDailyUpdateMAX(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", fromDate, toDate);

                        if (aDTmax != null && aDTmax.Rows.Count > 0)
                        {
                            Session["GQMS_MAX" + gSessionID] = aDTmax;

                            aDR = Utility.GetDR(ref aDTmax);
                            //-- OGC column 3-21, SUM column 22
                            String fd = "", fdValue = "";
                            for (int c = 3; c <= 22; c++)
                            {
                                fd = ConfigCol(c);
                                if (fd != "")
                                {
                                    switch (fd)
                                    {
                                        case "SG":
                                            fdValue = Utility.FormatNum(aDR[fd], 4);
                                            break;
                                        case "WC": //H2O
                                            fdValue = Utility.FormatNum(aDR[fd], 2);
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
                            //-- OGC column 3-21, SUM column 22
                            for (int c = 3; c <= 22; c++)
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

                        //--  column 23-31
                        for (int c = 23; c <= 31; c++)
                        {
                            cell = new TableCell();
                            cell.CssClass = "cell-right cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                        }
                        gvResult.Controls[0].Controls.Add(extraFooter);


                        //-- edit 29/11/2019 -- เพิ่ม confirm 20 days
                        //Add row=> QCxxx ==============================================================================================================
                        //-- ถ้าไม่กำหนดรายงานของ 20 day สำหรับ NGBILL ก็ไม่ต้องแสดงบรรทัดนี้
                        if (gNgRptNo20 != "")
                        {
                            extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                            extraFooter.CssClass = "ItemFooter_orange";
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = gNgRptNo20; // "QCxxx";
                            extraFooter.Cells.Add(cell);

                            //--- ดึงข้อมูลจาก ngbill ใช้ session ไม่ได้ เนื่องจากมีการกดปุ่ม confirm 
                            aDTng20 = Project.dal.SearchNgbillDailyUpdate(gNgRptNo20, fromDate, fromDate);   //-- ข้อมูลจะเก็บไว้วันที 1 ของแต่ละเดือน

                            if (aDTng20 != null && aDTng20.Rows.Count > 0)
                            {
                                aDR = Utility.GetDR(ref aDTng20);
                                //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                DataRow aDR20 = null;
                                if (aDTavg20 != null && aDTavg20.Rows.Count > 0)
                                {
                                    aDR20 = Utility.GetDR(ref aDTavg20);
                                }


                                //-- OGC column 3-21, SUM column 22 
                                String fd = "", fdValue = "";
                                for (int c = 3; c <= 22; c++)
                                {
                                    fd = ConfigCol(c);
                                    if (fd != "")
                                    {
                                        switch (fd)
                                        {
                                            case "SG":
                                                fdValue = Utility.FormatNum(aDR[fd], 4);
                                                break;
                                            case "WC": //H2O
                                                fdValue = Utility.FormatNum(aDR[fd], 2);
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
                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR20 != null)
                                        {
                                            // if (fdValue != Utility.FormatNum(aDR20[fd], 3)) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                            //-- edit 13/05/2019 -- แสดงสีพื้นด้วย
                                            if (fdValue != Utility.FormatNum(aDR20[fd], 3)) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-sum";
                                        }
                                    }
                                    else
                                    {
                                        cell.CssClass = "cell-right cell-Middle cell-border";

                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR20 != null)
                                        {
                                            string fdValue20 = "";
                                            switch (fd)
                                            {
                                                case "SG":
                                                    fdValue20 = Utility.FormatNum(aDR20[fd], 4);
                                                    break;
                                                case "WC": //H2O
                                                    fdValue20 = Utility.FormatNum(aDR20[fd], 2);
                                                    break;
                                                default:
                                                    fdValue20 = Utility.FormatNum(aDR20[fd], 3);
                                                    break;
                                            }
                                            if (fdValue != fdValue20) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                        }
                                    }
                                    cell.Text = fdValue;
                                    extraFooter.Cells.Add(cell);
                                }
                            }
                            else
                            {
                                //-- OGC column 3-21, SUM column 22
                                for (int c = 3; c <= 22; c++)
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

                            //--  column 23-31
                            for (int c = 23; c <= 31; c++)
                            {
                                cell = new TableCell();
                                cell.CssClass = "cell-right cell-Middle cell-border";
                                cell.Text = "";
                                extraFooter.Cells.Add(cell);
                            }

                            gvResult.Controls[0].Controls.Add(extraFooter);

                        }


                        //Add row=> QCxxx ==============================================================================================================
                        //-- ถ้าไม่กำหนดรายงานของ 27 day สำหรับ NGBILL ก็ไม่ต้องแสดงบรรทัดนี้
                        if (gNgRptNo27 != "")
                        {
                            extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                            extraFooter.CssClass = "ItemFooter_orange";
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = gNgRptNo27; // "QCxxx";
                            extraFooter.Cells.Add(cell);

                            //--- ดึงข้อมูลจาก ngbill ใช้ session ไม่ได้ เนื่องจากมีการกดปุ่ม confirm 
                            aDTng27 = Project.dal.SearchNgbillDailyUpdate(gNgRptNo27, fromDate, fromDate);   //-- ข้อมูลจะเก็บไว้วันที 1 ของแต่ละเดือน

                            if (aDTng27 != null && aDTng27.Rows.Count > 0)
                            {
                                aDR = Utility.GetDR(ref aDTng27);
                                //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                DataRow aDR27 = null;
                                if (aDTavg27 != null && aDTavg27.Rows.Count > 0)
                                {
                                    aDR27 = Utility.GetDR(ref aDTavg27);
                                }


                                //-- OGC column 3-21, SUM column 22 
                                String fd = "", fdValue = "";
                                for (int c = 3; c <= 22; c++)
                                {
                                    fd = ConfigCol(c);
                                    if (fd != "")
                                    {
                                        switch (fd)
                                        {
                                            case "SG":
                                                fdValue = Utility.FormatNum(aDR[fd], 4);
                                                break;
                                            case "WC": //H2O
                                                fdValue = Utility.FormatNum(aDR[fd], 2);
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
                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR27 != null)
                                        {
                                           // if (fdValue != Utility.FormatNum(aDR27[fd], 3)) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                           //-- edit 13/05/2019 -- แสดงสีพื้นด้วย
                                            if (fdValue != Utility.FormatNum(aDR27[fd], 3)) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-sum";
                                        }
                                    }
                                    else
                                    {
                                        cell.CssClass = "cell-right cell-Middle cell-border";

                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR27 != null)
                                        {
                                            string fdValue27 = "";
                                            switch (fd)
                                            {
                                                case "SG":
                                                    fdValue27 = Utility.FormatNum(aDR27[fd], 4);
                                                    break;
                                                case "WC": //H2O
                                                    fdValue27 = Utility.FormatNum(aDR27[fd], 2);
                                                    break;
                                                default:
                                                    fdValue27 = Utility.FormatNum(aDR27[fd], 3);
                                                    break;
                                            }
                                            if (fdValue != fdValue27) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                        }
                                    }
                                    cell.Text = fdValue;
                                    extraFooter.Cells.Add(cell);
                                    }
                                }
                            else
                            {
                                    //-- OGC column 3-21, SUM column 22
                                    for (int c = 3; c <= 22; c++)
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

                                //--  column 23-31
                                for (int c = 23; c <= 31; c++)
                                {
                                    cell = new TableCell();
                                    cell.CssClass = "cell-right cell-Middle cell-border";
                                    cell.Text = "";
                                    extraFooter.Cells.Add(cell);
                                }

                                gvResult.Controls[0].Controls.Add(extraFooter);

                            }


                        //Add row=> QCExxx ==============================================================================================================
                        extraFooter = new GridViewRow(0, 0, DataControlRowType.Footer, DataControlRowState.Normal);
                        //-- ถ้าไม่กำหนดรายงานของ end month สำหรับ NGBILL ก็ไม่ต้องแสดงบรรทัดนี้

                        if (gNgRptNoEND != "")
                        {
                            extraFooter.CssClass = "ItemFooter_orange2";
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = "";
                            extraFooter.Cells.Add(cell);
                            cell = new TableCell();
                            cell.CssClass = "cell-center cell-Middle cell-border";
                            cell.Text = gNgRptNoEND; // "QCExxx";
                            extraFooter.Cells.Add(cell);


                            //--- ดึงข้อมูลจาก ngbill ใช้ session ไม่ได้ เนื่องจากมีการกดปุ่ม confirm 
                            aDTng31 = Project.dal.SearchNgbillDailyUpdate(gNgRptNoEND, fromDate, fromDate);  //-- ข้อมูลจะเก็บไว้วันที 1 ของแต่ละเดือน

                            if (aDTng31 != null && aDTng31.Rows.Count > 0)
                            {
                                aDR = Utility.GetDR(ref aDTng31);

                                //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                DataRow aDR31 = null;
                                if (aDTavg31 != null && aDTavg31.Rows.Count > 0)
                                {
                                    aDR31 = Utility.GetDR(ref aDTavg31);
                                }

                                //-- OGC column 3-21, SUM column 22  
                                String fd = "", fdValue = "";
                                for (int c = 3; c <= 22; c++)
                                {
                                      
                                    fd = ConfigCol(c);
                                    if (fd != "")
                                    {
                                        switch (fd)
                                        {
                                            case "SG":
                                                fdValue = Utility.FormatNum(aDR[fd], 4);
                                                break;
                                            case "WC": //H2O
                                                fdValue = Utility.FormatNum(aDR[fd], 2);
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
                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR31 != null)
                                        {
                                            if (fdValue != Utility.FormatNum(aDR31[fd], 3)) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                        }
                                    }
                                    else
                                    {
                                        cell.CssClass = "cell-right cell-Middle cell-border";

                                        //-- 31/08/2018 -- ตรวจสอบว่าค่าที่ส่งไป ngbill แต่ภายหลังมีการแก้ไข ค่าตรงกันหรือไม่ ถ้าไม่ต้องให้ alert สีแดง
                                        if (fdValue != "" && aDR31 != null)
                                        {
                                            string fdValue31 = "";
                                            switch (fd)
                                            {
                                                case "SG":
                                                    fdValue31 = Utility.FormatNum(aDR31[fd], 4);
                                                    break;
                                                case "WC": //H2O
                                                    fdValue31 = Utility.FormatNum(aDR31[fd], 2);
                                                    break;
                                                default:
                                                    fdValue31 = Utility.FormatNum(aDR31[fd], 3);
                                                    break;
                                            }

                                            if (fdValue != fdValue31) cell.CssClass = "cell-right cell-Middle cell-border txt-warning cell-bg-white";
                                        }

                                    }
                                    cell.Text = fdValue;
                                    extraFooter.Cells.Add(cell);

                                }
                            }
                            else
                            {
                                //-- OGC column 3-21, SUM column 22
                                for (int c = 3; c <= 22; c++)
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

                            //--  column 23-31
                            for (int c = 23; c <= 31; c++)
                            {
                                cell = new TableCell();
                                cell.CssClass = "cell-right cell-Middle cell-border";
                                cell.Text = "";
                                extraFooter.Cells.Add(cell);
                            }

                            gvResult.Controls[0].Controls.Add(extraFooter);

                        }




                    }   //ServerAction != "EDIT"
                }   //e.Row.RowType == DataControlRowType.Footer


  


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref aDTavg27); Utility.ClearObject(ref aDTavg31);
                Utility.ClearObject(ref aDTmin); Utility.ClearObject(ref aDTmax);
                Utility.ClearObject(ref aDTng27); Utility.ClearObject(ref aDTng31);
            }
        }

        protected void gvResult_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //cast the sender back to a gridview
            GridView gv = sender as GridView;
            TableCell cell = null;

            //check if the row is the header row
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.TableSection = TableRowSection.TableHeader;

                //create a new row -------------------------------------------------------------------
                GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                extraHeader.TableSection = TableRowSection.TableHeader;

                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 30;    extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 40;    extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 90;    extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = ""; //UNNORM
                cell.CssClass = "Table-head-white"; cell.Width = 90; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 90; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.CssClass = "Table-head-white"; cell.Width = 70; extraHeader.Cells.Add(cell);

                //-- OGC column 23-25
                cell = new TableCell();
                cell.Text = "OGC";
                cell.ColumnSpan = 3;
                cell.Width = 210;
                cell.CssClass = "Table-head-orange cell-center";
                extraHeader.Cells.Add(cell);

                //-- OMA column 26-28
                cell = new TableCell();
                cell.Text = "OMA (H2O)";
                cell.ColumnSpan = 3;
                cell.Width = 220;
                cell.CssClass = "Table-head-primary cell-center";
                extraHeader.Cells.Add(cell);

                //-- Flow column 29-30
                cell = new TableCell();
                cell.Text = "Flow";
                cell.ColumnSpan = 2;
                cell.Width = 140;
                cell.CssClass = "Table-head-success cell-center";
                extraHeader.Cells.Add(cell);

                //-- OGC site สำหรอง column 31
                cell = new TableCell();
                cell.Text = "เลือก Site";
                cell.Width = 80;
                cell.CssClass = "Table-head-danger cell-center";
                extraHeader.Cells.Add(cell);

                //add the new row to the gridview
                gv.Controls[0].Controls.AddAt(0, extraHeader);
                //---------------------------------------------------------------

                //-- edit 02/10/2019 -- ย้ายหน่วยไปอยู่บรรทัดที่ 2 ไม่อย่างนั้นจะเกิด error ตอนอ่าน datarow
                //create a new row  (UNIT)-------------------------------------------------------------------
                //cast the sender back to a gridview
                //GridView gv = sender as GridView;
                cell = null;
                e.Row.TableSection = TableRowSection.TableHeader;
                extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                extraHeader.TableSection = TableRowSection.TableHeader;

                cell = new TableCell(); cell.Text = "";
                cell.Width = 30; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";
                cell.Width = 40; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "unit";
                cell.Width = 90; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "mole %";
                cell.ColumnSpan = 10; cell.Width = 700; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "ppm";          //-- H2S column 13
                cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "Btu/scf";          //-- NETHVDRY,HVSAT column 14,15
                cell.ColumnSpan = 2; cell.Width = 140; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";          //-- SG
                cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "Lb/MMscf";          //-- H2O
                cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "mole %";          //-- UNNORM column 18,19
                cell.ColumnSpan = 3; cell.Width = 250; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "Btu/scf";          //-- WI
                cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                cell = new TableCell(); cell.Text = "";          //-- SUM
                cell.Width = 70; cell.CssClass = "Table-head-gray"; extraHeader.Cells.Add(cell);

                //-- OGC column 23-25
                cell = new TableCell(); cell.Text = "";
                cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "Btu/scf";
                cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.Width = 70; cell.CssClass = "Table-head-orange cell-center"; extraHeader.Cells.Add(cell);

                //-- OMA(H2O) column 26-28
                cell = new TableCell(); cell.Text = "Lb/MMscf";
                cell.Width = 70; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "Lb/MMscf";
                cell.Width = 70; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.Width = 80; cell.CssClass = "Table-head-primary cell-center"; extraHeader.Cells.Add(cell);

                //-- Flow column 29-30
                cell = new TableCell(); cell.Text = "";
                cell.Width = 70; cell.CssClass = "Table-head-success cell-center"; extraHeader.Cells.Add(cell);
                cell = new TableCell(); cell.Text = "";
                cell.Width = 70; cell.CssClass = "Table-head-success cell-center"; extraHeader.Cells.Add(cell);

                //-- OGC site สำหรอง column 31
                cell = new TableCell(); cell.Text = "";
                cell.Width = 80; cell.CssClass = "Table-head-danger cell-center"; extraHeader.Cells.Add(cell);

                //add the new row to the gridview
                gv.Controls[0].Controls.AddAt(1, extraHeader);
                //---------------------------------------------------------------
            }
        }


        //-- edit 23/04/2021 -- ในวันที่มีการแทนค่าสำรอง ต้องมีข้อความเป็นสีน้ำเงิน 
        private void ShowValue(ref GridViewRowEventArgs gRow, int sCol, int eCol, DataRowView gDR)
        {
            String result = "";
            String fd = "";
            String AL = "";
            String ddl = "";
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
                            //-- EDIT 30/03/2022 พบว่า col=28 OMA สำรอง บางทีมีค่า 34, 36 ซึ่งทำให้ระบบตรวจสอบว่าเป็น number 
                            //if (Utility.IsNumeric(gDR[fd]))
                            if (Utility.IsNumeric(gDR[fd]) && fd != "OMA_NAME")
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

                                if (LoadSessionFlag)
                                {
                                    //-- ตรวจ alert จาก session เก็บแยกแต่ละคอลัมน์  เก็บ rowindex เช่น ,2,3,6,
                                    AL = Utility.ToString(Session["ALERT_COL" + Utility.ToString(c) + gSessionID]);
                                    if ( AL.IndexOf(","+gRow.Row.RowIndex.ToString()+",") > -1 )
                                    {
                                        if ( c > 12 )
                                            gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                        else
                                            gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                                    }
                                }
                                else
                                {
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

                                //-- edit 23/04/2021 -- ในวันที่มีการแทนค่าสำรอง ต้องมีข้อความเป็นสีน้ำเงิน OGC_NAME
                                if (gOgcName != "" && c<=21)
                                {
                                    string css = gRow.Row.Cells[c].CssClass;

                                    if (css =="")
                                    {
                                        css = "cell-right cell-Middle cell-border txt-warning4";
                                    }
                                    else
                                    {
                                        if (css.IndexOf("txt-warning4") < 1) css = css + " txt-warning4";
                                    }

                                    //-- EDIT 23/08/2022 --- กรณี Auto replace ให้แสดงคำว่า (Auto) แล้วแถบสีเหลือง
                                    if (Utility.ToString(gDR["OGC_NAME"]) == Utility.ToString(gDR["AUTO_OGC_NAME"])) css += " cell-bg-yellow ";

                                    gRow.Row.Cells[c].CssClass = css;

                                }


                            }
                            else
                            {  //-- ข้อมูลไม่ใช่ตัวเลข แสดงว่า error
                                result = Utility.ToString(gDR[fd]);
                                //-- edit 29/11/2019 --
                                if (fd == "OMA_NAME")   ShowAlert(ref gRow, c, fd, result, gDR);

                                if (result != "")
                                {
                                    if (fd.IndexOf("NAME") < 0 && fd.IndexOf("DATE") < 0) gRow.Row.Cells[c].CssClass = "cell-right cell-Middle cell-border txt-warning";

                                    if (fd.IndexOf("DATE") > 0) result = Utility.AppFormatDate(gDR[fd]);

                                    //-- EDIT 23/08/2022 --- กรณี Auto replace ให้แสดงคำว่า (Auto) แล้วแถบสีเหลือง
                                    if (fd == "OGC_NAME" && Utility.ToString(gDR["OGC_NAME"]) == Utility.ToString(gDR["AUTO_OGC_NAME"])) result += " (Auto)";

                                }
                            }
                        }
                    }



                    // ต้องตรวจสอบว่าเป็น row ที่ edit หรือเปล่า ถ้าใช่ก็กำหนดเป็น textbox 
                    //เก็บ ItemIndex ที่ต้องการแก้ไขไว้ใน hidden = EditItemIndex  เก็บเป็น ,2,3,5
                    //ServerAction = "EDIT"
                    //-- OGC column 3-21, 22 Sum 
                    if (ServerAction == "EDIT" && EditItemIndex.IndexOf(',' + gRow.Row.DataItemIndex.ToString() + ',') > -1)
                    {
                        if (c >= 3 && c <= 21)
                        {
                            gRow.Row.Cells[c].Text = "<input type=\"text\" class=\"input-right\" id=\"txt" + fd + "_" + gRow.Row.DataItemIndex + "\" name=\"txt" + fd + "_" + gRow.Row.DataItemIndex + "\" style=\"width:65px\" MaxLength=\"20\" value=\"" + result + "\"  />";
                        }
                        else
                        {

                            if (c == 28)   //-- column 28 dropdown OMA
                            {
                                if (gOmaName1 == "" && gOmaName2 == "")
                                {
                                    //-- ไม่มีการกำหนด OMA site ก็ไม่ต้องแสดง dropdown 
                                    gRow.Row.Cells[c].Text = "";
                                }
                                else
                                {
                                    ddl = "";
                                    if (gOmaName1 != "")
                                    {
                                        if (gOmaName1 == gOmaName)
                                            ddl += "<option value=\"" + gOmaName1 + "\" selected>" + gOmaName1 + "</option>";
                                        else
                                            ddl += "<option value=\"" + gOmaName1 + "\">" + gOmaName1 + "</option>";
                                    }
                                    if (gOmaName2 != "")
                                    {
                                        if (gOmaName2 == gOmaName)
                                            ddl += "<option value=\"" + gOmaName2 + "\" selected>" + gOmaName2 + "</option>";
                                        else
                                            ddl += "<option value=\"" + gOmaName2 + "\">" + gOmaName2 + "</option>";
                                    }
                                    if (gOmaName == "NO")
                                        ddl += "<option value=\"NO\" selected>NO</option>";
                                    else
                                        ddl += "<option value=\"NO\">NO</option>";

                                    gRow.Row.Cells[c].Text = "";
                                    ddl = "<select id=\"ddlOMA_" + gRow.Row.DataItemIndex + "\" name=\"ddlOMA_" + gRow.Row.DataItemIndex + "\"  onchange=\"SelectOMA(" + gRow.Row.DataItemIndex + ",'" + Utility.FormatNum(gDR["WC1"], 2) + "','" + Utility.FormatNum(gDR["WC2"], 2) + "')\" > " + ddl + "</select>";

                                    gRow.Row.Cells[c].Text = ddl;
                                }

                            }
                            else
                            {
                                if (c == 31)    //-- column 31 dropdown OGC site สำรอง OGC_NAME
                                {
                                   if ( gOgcName1 == "" && gOgcName2 == "" && gOgcName3 == "")
                                    {
                                        //-- ไม่มีการกำหนด OGC site สำรอง ก็ไม่ต้องแสดง dropdown 
                                        gRow.Row.Cells[c].Text = "";
                                    }
                                   else
                                    {
                                        ddl = "";
                                        string f = Validation.GetCtrlStr(hidFID); //ค่า site เดิม
                                        if (f == gOgcName)
                                            ddl += "<option value=\"" + f + "\" selected>" + f + "</option>";
                                        else
                                            ddl += "<option value=\"" + f + "\">" + f + "</option>";

                                        if (gOgcName1 != "")
                                        {
                                            if (gOgcName1 == gOgcName)
                                                ddl += "<option value=\"" + gOgcName1 + "\" selected>" + gOgcName1 + "</option>";
                                            else
                                                ddl += "<option value=\"" + gOgcName1 + "\">" + gOgcName1 + "</option>";
                                        }
                                        if (gOgcName2 != "")
                                        {
                                            if (gOgcName2 == gOgcName)
                                                ddl += "<option value=\"" + gOgcName2 + "\" selected>" + gOgcName2 + "</option>";
                                            else
                                                ddl += "<option value=\"" + gOgcName2 + "\">" + gOgcName2 + "</option>";
                                        }
                                        if (gOgcName3 != "")
                                        {
                                            if (gOgcName3 == gOgcName)
                                                ddl += "<option value=\"" + gOgcName3 + "\" selected>" + gOgcName3 + "</option>";
                                            else
                                                ddl += "<option value=\"" + gOgcName3 + "\">" + gOgcName3 + "</option>";
                                        }



                                        if (gOgcName == "NO")
                                            ddl += "<option value=\"NO\" selected>NO</option>";
                                        else
                                            ddl += "<option value=\"NO\">NO</option>";

                                        gRow.Row.Cells[c].Text = "";
                                        ddl = "<select id=\"ddlOGC_" + gRow.Row.DataItemIndex + "\" name=\"ddlOGC_" + gRow.Row.DataItemIndex + "\"  onchange=\"SelectOgc(" + gRow.Row.DataItemIndex + ",'" + Utility.AppFormatDate(gDR["RDATE"]) + "')\" > " + ddl + "</select>";

                                        gRow.Row.Cells[c].Text = ddl;
                                    }

                                }
                                else
                                {
                                    gRow.Row.Cells[c].Text = result;
                                }
                            }
                        }
                    }
                    else
                    {
                        gRow.Row.Cells[c].Text = result;
                    }
                   


                      
                  
                   

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
                    //-- OGC column 3-21, sum column 22
                    case 3: result = "C1"; break;
                    case 4: result = "C2"; break;
                    case 5: result = "C3"; break;
                    case 6: result = "IC4"; break;
                    case 7: result = "NC4"; break;
                    case 8: result = "IC5"; break;
                    case 9: result = "NC5"; break;
                    case 10: result = "C6"; break;
                    case 11: result = "CO2"; break;
                    case 12: result = "N2"; break;
                    case 13: result = "H2S"; break;
                    case 14: result = "NHV"; break;
                    case 15: result = "GHV"; break;
                    case 16: result = "SG"; break;
                    case 17: result = "WC"; break;
                    case 18: result = "UNNORMMIN"; break;
                    case 19: result = "UNNORMMAX"; break;
                    case 20: result = "UNNORMALIZED"; break;
                    case 21: result = "WB"; break;
                    case 22: result = "SUM_COMPO"; break;

                    //-- OGC BTU column 23-25
                    case 23: result = "BTUDATE"; break;
                    case 24: result = "BTU"; break;
                    case 25: result = "RUN"; break;

                    //-- OMA column 26-27, เลือก Site 28
                    case 26: result = "WC1"; break;
                    case 27: result = "WC2"; break;
                    case 28: result = "OMA_NAME"; break;

                    //-- FLOW column 29-30 
                    case 29: result = "FLOW1"; break;
                    case 30: result = "FLOW2"; break;
                    //-- เลือก Site สำรอง column 31
                    case 31: result = "OGC_NAME"; break;
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
                Double  dVal = Utility.ToDouble(sValue);

                switch ( dField )
                {
                    case "WC":   //ค่า WC ต้องไม่เกิน 7 lb และไม่น้อยกว่าหรือเท่ากับ 0 และ กรณีไม่มีค่าซ้ำกันเกิน 3 ชั่วโมง
                        if ( Utility.ToString(gDR["WC_ALERT"]) == "Y" )
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += ","+  gRow.Row.RowIndex.ToString() +",";
                        }

                        break;
                    case "WC1":   //ค่า WC ต้องไม่เกิน 7 lb และไม่น้อยกว่าหรือเท่ากับ 0 และ กรณีไม่มีค่าซ้ำกันเกิน 3 ชั่วโมง
                        if (Utility.ToString(gDR["WC1_ALERT"]) == "Y")
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }

                        break;
                    case "WC2":   //ค่า WC ต้องไม่เกิน 7 lb และไม่น้อยกว่าหรือเท่ากับ 0 และ กรณีไม่มีค่าซ้ำกันเกิน 3 ชั่วโมง
                        if (Utility.ToString(gDR["WC2_ALERT"]) == "Y")
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }

                        break;
                    case "FLOW1": //กรณีมีค่าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                        if (gFlowName1 != "")
                        {
                            if (Utility.ToString(gDR["FLOW1_ALERT"]) == "Y")
                            {
                                gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                            }

                        }
                        break;
                    case "FLOW2": //กรณีมีค่าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                        if (gFlowName2 != "")
                        {
                            if (Utility.ToString(gDR["FLOW2_ALERT"]) == "Y")
                            {
                                gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                            }

                        }
                        break;

                    case "UNNORMMIN":   //ค่า UnnormMin ต้องไม่ต่ำกว่า 98
                        if (dVal < 98 )
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "UNNORMMAX":   //ค่า UnnormMax ต้องไม่เกิน 102
                        if (dVal > 102)
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "SUM_COMPO":   //ค่า SUM คือ บวก CH4->H2S ต้องได้ 100(+-0.003)
                        if ( dVal < 99.997 || dVal > 100.003 )
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;

                    case "GHV":// เอา BTU ไป compare กับ HVSAT (GHV) ด้วยวันที่เหลือมกัน (BTU DATE จะมากกว่า RDATE)
                        if ( Utility.ToString(gDR["BTU"]) != "" )
                        {
                            if (dVal != Utility.ToDouble(Utility.FormatNum(gDR["BTU"], 3)))
                            {
                                gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                            }
                        }                       
                        break;

                    case "RUN":      //ค่า run ในตาราง จะต้องไม่ต่ำกว่า Minimum run ที่กำหนด
                        //Double minRUN = Utility.ToDouble(gDR["TOTAL_RUN"]) * ((100 - Utility.ToDouble(gDR["TOLERANCE_RUN"])) / 100);
                        //--30/08/2018  เก็บค่า TOLERANCE_RUN เป็นตัวเลข เช่น 30 หมายถึง ยอมรับได้ในช่วง +-30
                        Double minRUN = Utility.ToDouble(gDR["TOTAL_RUN"]) - Utility.ToDouble(gDR["TOLERANCE_RUN"]);

                        if (dVal < minRUN )
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;


                    //--- edit 29/11/2019 --- เกิดกรณีที่ค่าน้ำ H2O (WC) ไม่ตรงกับค่าน้ำ OMA ที่เลือกไว้ อาจจะเกิดจากข้อมูลภายหลังมีการ update 
                    //-- จึงต้อง alert ให้เป็น โดยต้องตรวจที่คอลัมน์ 28 OMA_NAME ด้วยว่าเป็นข้อมูลแหล่งไหน
                    case "OMA_NAME":
                        string aWC = gRow.Row.Cells[17].Text;
                        string aWC1 = gRow.Row.Cells[26].Text;
                        string aWC2 = gRow.Row.Cells[27].Text;
                        string aWC_NAME = sValue; //ยังไม่ได้ใส่ใน ceel gRow.Row.Cells[28].Text;

                        if ( Utility.IsNumeric(aWC) )
                        {
                            if (aWC_NAME == gOmaName1 && aWC != aWC1)
                            {
                                gRow.Row.Cells[17].CssClass = "cell-right cell-Middle cell-border txt-warning2";
                                Session["ALERT_COL" + Utility.ToString(17) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                            }
                            else
                            {
                                if (aWC_NAME == gOmaName2 && aWC != aWC2)
                                {
                                    gRow.Row.Cells[17].CssClass = "cell-right cell-Middle cell-border txt-warning2";
                                    Session["ALERT_COL" + Utility.ToString(17) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                                }
                            }
                        }



                        break;


                }





            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }

        //-- edit 19/07/2019 --- เพิ่มตรวจสอบ Min/Max
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
                        //if ( gC1 != -999 && (dVal < gC1/2  || dVal > 2*gC1)) 
                        if ((gC1 != -999 && (dVal < gC1/2  || dVal > 2*gC1)) || (gC1_MIN != -999 && dVal < gC1_MIN) || (gC1_MAX != -999 && dVal > gC1_MAX))  //เพิ่มตรวจสอบ Min/Max
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "C2":
                        //if (gC2 != -999 && (dVal < gC2 / 2 || dVal > 2 * gC2))
                        if ((gC2 != -999 && (dVal < gC2 / 2 || dVal > 2 * gC2)) || (gC2_MIN != -999 && dVal < gC2_MIN) || (gC2_MAX != -999 && dVal > gC2_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "C3":
                        //if (gC3 != -999 && (dVal < gC3 / 2 || dVal > 2 * gC3))
                        if ((gC3 != -999 && (dVal < gC3 / 2 || dVal > 2 * gC3)) || (gC3_MIN != -999 && dVal < gC3_MIN) || (gC3_MAX != -999 && dVal > gC3_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "IC4":
                        //if (gIC4 != -999 && (dVal < gIC4 / 2 || dVal > 2 * gIC4))
                        if ((gIC4 != -999 && (dVal < gIC4 / 2 || dVal > 2 * gIC4)) || (gIC4_MIN != -999 && dVal < gIC4_MIN) || (gIC4_MAX != -999 && dVal > gIC4_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "NC4":
                        //if (gNC4 != -999 && (dVal < gNC4 / 2 || dVal > 2 * gNC4))
                        if ((gNC4 != -999 && (dVal < gNC4 / 2 || dVal > 2 * gNC4)) || (gNC4_MIN != -999 && dVal < gNC4_MIN) || (gNC4_MAX != -999 && dVal > gNC4_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "IC5":
                        //if (gIC5 != -999 && (dVal < gIC5 / 2 || dVal > 2 * gIC5))
                        if ((gIC5 != -999 && (dVal < gIC5 / 2 || dVal > 2 * gIC5)) || (gIC5_MIN != -999 && dVal < gIC5_MIN) || (gIC5_MAX != -999 && dVal > gIC5_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "NC5":
                        //if (gNC5 != -999 && (dVal < gNC5 / 2 || dVal > 2 * gNC5))
                        if ((gNC5 != -999 && (dVal < gNC5 / 2 || dVal > 2 * gNC5)) || (gNC5_MIN != -999 && dVal < gNC5_MIN) || (gNC5_MAX != -999 && dVal > gNC5_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "C6":
                        //if (gC6 != -999 && (dVal < gC6 / 2 || dVal > 2 * gC6))
                        if ((gC6 != -999 && (dVal < gC6 / 2 || dVal > 2 * gC6)) || (gC6_MIN != -999 && dVal < gC6_MIN) || (gC6_MAX != -999 && dVal > gC6_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "N2":
                        //if (gN2 != -999 && (dVal < gN2 / 2 || dVal > 2 * gN2))
                        if ((gN2 != -999 && (dVal < gN2 / 2 || dVal > 2 * gN2)) || (gN2_MIN != -999 && dVal < gN2_MIN) || (gN2_MAX != -999 && dVal > gN2_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "CO2":
                        //if (gCO2 != -999 && (dVal < gCO2 / 2 || dVal > 2 * gCO2))
                        if ((gCO2 != -999 && (dVal < gCO2 / 2 || dVal > 2 * gCO2)) || (gCO2_MIN != -999 && dVal < gCO2_MIN) || (gCO2_MAX != -999 && dVal > gCO2_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
                        }
                        break;
                    case "H2S":
                        //if (gH2S != -999 && (dVal < gH2S / 2 || dVal > 2 * gH2S))
                        if ((gH2S != -999 && (dVal < gH2S / 2 || dVal > 2 * gH2S)) || (gH2S_MIN != -999 && dVal < gH2S_MIN) || (gH2S_MAX != -999 && dVal > gH2S_MAX))
                        {
                            gRow.Row.Cells[gCol].CssClass = "cell-right cell-Middle cell-border txt-warning3";
                            Session["ALERT_COL" + Utility.ToString(gCol) + gSessionID] += "," + gRow.Row.RowIndex.ToString() + ",";
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
                            case "WC": //H2O
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
        //////  CONFIRM TO NGBILL
        //////======================================================================

        private void SaveConfirmData1() //-- daily
        {
            DataTable DT = null;
            DataRow DR = null;
            int op = -99;
            String ngID = "";
            String startDate = "";
            Boolean SaveFlag = false;
            try
            {

                //--1) วนหาว่า checked box วันใดบ้าง ที่ต้องการส่ง
                foreach (GridViewRow rowData in gvResult.Rows)
                {
                    if (rowData.RowType == DataControlRowType.DataRow)
                    {

                        String chked = "";
                        //-- ถ้าเป็นการ checked จากการ select all จะอ่านค่าได้เป็น กรณีเลือก = on,on  ไม่เลือก = on
                        //-- ถ้าเป็นการ checked เอง  จะอ่านค่าได้เป็น กรณีเลือก = on  ไม่เลือก = ว่าง
                        string onSelect = Validation.GetParamStr("chkSelect" + rowData.RowIndex);
                        if (Utility.GetCtrl(hidSELECTALL) == "Y")
                        {
                            if (onSelect == "on,on") chked = "Y";
                        }
                        else
                        {
                            if (onSelect == "on") chked = "Y";  
                        }


                        if (chked == "Y") //-- ตรวจสอบว่า checked  หรือไม่ 
                        { 
                            //--2) ค้นหา GQMS_DAILY_UPDATE ที่ต้องการส่ง โดยต้องดูว่าเคยส่งไปให้ NGBILL_DAILY_UPDATE หรือยัง
                            //--   หรือว่าเคยส่งแล้ว แต่ต้นทางมีแก้ไข  (ดูจาก MODIFIED_DATE)
                            //-- edit 2/10/2019 -- เนื่องจากเพิ่ม header row ทำให้ข้อมูลขยับ
                            startDate = gvResult.Rows[rowData.RowIndex].Cells[2].Text; //-- วันที่
                            //startDate = gvResult.Rows[rowData.RowIndex+1].Cells[2].Text; //-- วันที่

                            DT = Project.dal.SearchGqmsNgbillDailyUpdate(Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidgNgRptNoDaily), startDate, startDate);

                            if (DT != null && DT.Rows.Count > 0)
                            {
                            DR = Utility.GetDR(ref DT);

                            //--3) บันทึกข้อมูลใน NGBILL_DAILY_UPDATE โดยถ้ามีข้อมูลอยู่แล้วให้ update ถ้าไม่มีให้ insert
                            if ( Utility.ToString(DR["NGMODIFIED_DATE"])== "")
                            {
                                op = DBUTIL.opINSERT;
                                ngID = "";
                                SaveFlag = true;
                            }
                            else
                            {
                                if (Utility.ToString(DR["MODIFIED_DATE"]) != "" && DateTime.Compare(Convert.ToDateTime(DR["NGMODIFIED_DATE"]),Convert.ToDateTime(DR["MODIFIED_DATE"])) < 0 ) 
                                {   //-- ถ้ามีการแก้ไขที่ GQMS_DAILY_UPDATE หลังจากที่เคยส่งไป NGBILL แล้ว ให้บันทึกใหม่
                                    op = DBUTIL.opUPDATE;
                                    ngID = Utility.ToString(DR["NGID"]);
                                    SaveFlag = true;
                                }
                                else
                                {   ///-- ถ้าไม่มีการแก้ไขอะไร ก็ไม่ต้องบันทึก
                                    SaveFlag = false;
                                }
                            }

                            if (SaveFlag)
                            {
                                //-- บันทึกค่า DAILY
                                Project.dal.MngNgbillDailyUpdate(op, ngID, Utility.AppFormatDate(DR["RDATE"]) , Utility.GetCtrl(hidgNgRptNoDaily), DR["C1"], DR["C2"], DR["C3"], DR["IC4"], DR["NC4"], DR["IC5"], DR["NC5"], DR["C6"], DR["N2"], DR["CO2"], DR["SG"], DR["GHV"], DR["NHV"], DR["WC"], DR["UNNORMALIZED"], DR["UNNORMMIN"], DR["UNNORMMAX"], DR["WB"], DR["H2S"], DR["SUM_COMPO"]);
                            }

                        }

                            //--12/02/2020 -- บันทึก DAILY27 รายวันส่งไป NGBill ด้วย ------------------------------
                            DataTable DTd = Project.dal.SearchGqmsNgbillDailyUpdate(Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidgNgRptNoDaily27), startDate, startDate);
                            if (DTd != null && DTd.Rows.Count > 0)
                            {
                                String SQL = "DELETE FROM NGBILL_DAILY_UPDATE WHERE FID='" + Utility.GetCtrl(hidgNgRptNoDaily27) + "' AND RDATE>= TO_DATE('" + startDate + "','DD/MM/YYYY') AND RDATE <= TO_DATE('" + startDate + "','DD/MM/YYYY')";
                                Project.dal.ExecuteSQL(SQL);
                                foreach (DataRow DRd in DTd.Rows)
                                {
                                    Project.dal.MngNgbillDailyUpdate(DBUTIL.opINSERT, "", Utility.AppFormatDate(DRd["RDATE"]), Utility.GetCtrl(hidgNgRptNoDaily27), DRd["C1"], DRd["C2"], DRd["C3"], DRd["IC4"], DRd["NC4"], DRd["IC5"], DRd["NC5"], DRd["C6"], DRd["N2"], DRd["CO2"], DRd["SG"], DRd["GHV"], DRd["NHV"], DRd["WC"], DRd["UNNORMALIZED"], DRd["UNNORMMIN"], DRd["UNNORMMAX"], DRd["WB"], DRd["H2S"], DRd["SUM_COMPO"]);
                                }
                            }
                            //--12/02/2020 -- บันทึก DAILY20 รายวันส่งไป NGBill ด้วย ------------------------------
                            DTd = Project.dal.SearchGqmsNgbillDailyUpdate(Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidgNgRptNoDaily20), startDate, startDate);
                            if (DTd != null && DTd.Rows.Count > 0)
                            {
                                String SQL = "DELETE FROM NGBILL_DAILY_UPDATE WHERE FID='" + Utility.GetCtrl(hidgNgRptNoDaily20) + "' AND RDATE>= TO_DATE('" + startDate + "','DD/MM/YYYY') AND RDATE <= TO_DATE('" + startDate + "','DD/MM/YYYY')";
                                Project.dal.ExecuteSQL(SQL);
                                foreach (DataRow DRd in DTd.Rows)
                                {
                                    Project.dal.MngNgbillDailyUpdate(DBUTIL.opINSERT, "", Utility.AppFormatDate(DRd["RDATE"]), Utility.GetCtrl(hidgNgRptNoDaily20), DRd["C1"], DRd["C2"], DRd["C3"], DRd["IC4"], DRd["NC4"], DRd["IC5"], DRd["NC5"], DRd["C6"], DRd["N2"], DRd["CO2"], DRd["SG"], DRd["GHV"], DRd["NHV"], DRd["WC"], DRd["UNNORMALIZED"], DRd["UNNORMMIN"], DRd["UNNORMMAX"], DRd["WB"], DRd["H2S"], DRd["SUM_COMPO"]);
                                }
                            }
                            //---------------------------------------------------------------------------------------------

                        }
                }

                }

                //--- เมื่อกดปุ่ม Confirm ให้เก็บ Log ไว้ด้วย
                BLL.InsertAudit(Project.catConfDAILY, "Confirm (daily): FID=" + ddlFID.SelectedItem.Text + ";Month/Year=" + ddlMONTH.SelectedValue + "/" + ddlYEAR.SelectedValue + ";", "");

                LoadData();
                //LoadDataSession();  //-- สำหรับ daily ต้องให้ loaddata ใหม่ เพราะต้องทำ check box ใหม่ 
                ShowConfirm(Project.catConfDAILY, ref lblComfirm1);

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


        private void SaveConfirmData2() //-- 27 days
        {
            DataTable DT = null;
            DataRow DR = null;
            int op;
            String ngID = "";

            try
            {

                DT = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", Utility.GetCtrl(hidfromDate27), Utility.GetCtrl(hidtoDate27));
                if ( DT != null && DT.Rows.Count > 0)
                {
                    DR = Utility.GetDR(ref DT);

                    //-- ตรวจสอบว่ามีข้อมูลเก่าอยู่หรือเปล่า  ถ้ามีให้ update
                    DataTable DTo = Project.dal.SearchNgbillDailyUpdate(Utility.GetCtrl(hidgNgRptNo27), Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidfromDate)); //บันทึกใน ngbill เป็นวันที่ 1 ของเดือน
                    if (DTo != null && DTo.Rows.Count > 0)
                    {
                        op = DBUTIL.opUPDATE;
                        ngID = Utility.ToString(DTo.Rows[0]["ID"]);
                    }
                    else
                    {
                        op = DBUTIL.opINSERT;
                    }

                    //-- บันทึกค่า AVERAGE (27)
                    Project.dal.MngNgbillDailyUpdate(op, ngID, Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidgNgRptNo27), DR["C1"], DR["C2"], DR["C3"], DR["IC4"], DR["NC4"], DR["IC5"], DR["NC5"], DR["C6"], DR["N2"], DR["CO2"], DR["SG"], DR["GHV"], DR["NHV"], DR["WC"], DR["UNNORMALIZED"], DR["UNNORMMIN"], DR["UNNORMMAX"], DR["WB"], DR["H2S"], DR["SUM_COMPO"]);

                    //--02/09/2018 -- บันทึก DAILY27 รายวันส่งไป NGBill ด้วย
                    DataTable  DTd = Project.dal.SearchGqmsNgbillDailyUpdate(Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidgNgRptNoDaily27), Utility.GetCtrl(hidfromDate27), Utility.GetCtrl(hidtoDate27));
                    if ( DTd !=null && DTd.Rows.Count > 0)
                    {
                        String SQL = "DELETE FROM NGBILL_DAILY_UPDATE WHERE FID='"+ Utility.GetCtrl(hidgNgRptNoDaily27)+"' AND RDATE>= TO_DATE('"+ Utility.GetCtrl(hidfromDate27)+"','DD/MM/YYYY') AND RDATE <= TO_DATE('"+ Utility.GetCtrl(hidtoDate27)+"','DD/MM/YYYY')";
                        Project.dal.ExecuteSQL(SQL);
                        foreach (DataRow DRd in DTd.Rows)
                        {
                            Project.dal.MngNgbillDailyUpdate(DBUTIL.opINSERT, "", Utility.AppFormatDate(DRd["RDATE"]), Utility.GetCtrl(hidgNgRptNoDaily27), DRd["C1"], DRd["C2"], DRd["C3"], DRd["IC4"], DRd["NC4"], DRd["IC5"], DRd["NC5"], DRd["C6"], DRd["N2"], DRd["CO2"], DRd["SG"], DRd["GHV"], DRd["NHV"], DRd["WC"], DRd["UNNORMALIZED"], DRd["UNNORMMIN"], DRd["UNNORMMAX"], DRd["WB"], DRd["H2S"], DRd["SUM_COMPO"]);
                        }

                    }


                    //--- เมื่อกดปุ่ม Confirm ให้เก็บ Log ไว้ด้วย
                    BLL.InsertAudit(Project.catConf27DAY, "Confirm (27): FID=" + ddlFID.SelectedItem.Text + ";Month/Year=" + ddlMONTH.SelectedValue + "/" + ddlYEAR.SelectedValue + ";", "");

                    //LoadData();
                    LoadDataSession();

                    ShowConfirm(Project.catConf27DAY, ref lblComfirm2);
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


        private void SaveConfirmData3() //-- end month
        {
            DataTable DT = null;
            DataRow DR = null;
            int op;
            String ngID = "";

            try
            {

                DT = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidtoDate));
                if (DT != null && DT.Rows.Count > 0)
                {
                    DR = Utility.GetDR(ref DT);

                    //-- ตรวจสอบว่ามีข้อมูลเก่าอยู่หรือเปล่า  ถ้ามีให้ update
                    DataTable DTo = Project.dal.SearchNgbillDailyUpdate(Utility.GetCtrl(hidgNgRptNoEND), Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidfromDate));
                    if (DTo != null && DTo.Rows.Count > 0)
                    {
                        op = DBUTIL.opUPDATE;
                        ngID = Utility.ToString(DTo.Rows[0]["ID"]);
                    }
                    else
                    {
                        op = DBUTIL.opINSERT;
                    }

                    //-- บันทึกค่า AVERAGE (END MONTH)
                    Project.dal.MngNgbillDailyUpdate(op, ngID , Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidgNgRptNoEND), DR["C1"], DR["C2"], DR["C3"], DR["IC4"], DR["NC4"], DR["IC5"], DR["NC5"], DR["C6"], DR["N2"], DR["CO2"], DR["SG"], DR["GHV"], DR["NHV"], DR["WC"], DR["UNNORMALIZED"], DR["UNNORMMIN"], DR["UNNORMMAX"], DR["WB"], DR["H2S"], DR["SUM_COMPO"]);

                    //--- เมื่อกดปุ่ม Confirm ให้เก็บ Log ไว้ด้วย
                    BLL.InsertAudit(Project.catConfENDMTH, "Confirm (end month): FID=" + ddlFID.SelectedItem.Text + ";Month/Year=" + ddlMONTH.SelectedValue + "/" + ddlYEAR.SelectedValue + ";", "");

                    //LoadData();
                    LoadDataSession();
                    ShowConfirm(Project.catConfENDMTH, ref lblComfirm3);

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


        //-- edit 29/11/2019 -- เพิ่ม confirm 20 Days
        private void SaveConfirmData4() //-- 20 days
        {
            DataTable DT = null;
            DataRow DR = null;
            int op;
            String ngID = "";

            try
            {

                DT = Project.dal.SearchGqmsDailyUpdateAVG(Validation.GetCtrlIntStr(hidSITE_ID, "0"), "", Utility.GetCtrl(hidfromDate20), Utility.GetCtrl(hidtoDate20));
                if (DT != null && DT.Rows.Count > 0)
                {
                    DR = Utility.GetDR(ref DT);

                    //-- ตรวจสอบว่ามีข้อมูลเก่าอยู่หรือเปล่า  ถ้ามีให้ update
                    DataTable DTo = Project.dal.SearchNgbillDailyUpdate(Utility.GetCtrl(hidgNgRptNo20), Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidfromDate)); //บันทึกใน ngbill เป็นวันที่ 1 ของเดือน
                    if (DTo != null && DTo.Rows.Count > 0)
                    {
                        op = DBUTIL.opUPDATE;
                        ngID = Utility.ToString(DTo.Rows[0]["ID"]);
                    }
                    else
                    {
                        op = DBUTIL.opINSERT;
                    }

                    //-- บันทึกค่า AVERAGE (20)
                    Project.dal.MngNgbillDailyUpdate(op, ngID, Utility.GetCtrl(hidfromDate), Utility.GetCtrl(hidgNgRptNo20), DR["C1"], DR["C2"], DR["C3"], DR["IC4"], DR["NC4"], DR["IC5"], DR["NC5"], DR["C6"], DR["N2"], DR["CO2"], DR["SG"], DR["GHV"], DR["NHV"], DR["WC"], DR["UNNORMALIZED"], DR["UNNORMMIN"], DR["UNNORMMAX"], DR["WB"], DR["H2S"], DR["SUM_COMPO"]);

                    //--02/09/2018 -- บันทึก DAILY20 รายวันส่งไป NGBill ด้วย

                    DataTable DTd = Project.dal.SearchGqmsNgbillDailyUpdate(Validation.GetCtrlStr(hidFID), Validation.GetCtrlStr(hidgNgRptNoDaily20), Utility.GetCtrl(hidfromDate20), Utility.GetCtrl(hidtoDate20));
                    if (DTd != null && DTd.Rows.Count > 0)
                    {
                        String SQL = "DELETE FROM NGBILL_DAILY_UPDATE WHERE FID='" + Utility.GetCtrl(hidgNgRptNoDaily20) + "' AND RDATE>= TO_DATE('" + Utility.GetCtrl(hidfromDate20) + "','DD/MM/YYYY') AND RDATE <= TO_DATE('" + Utility.GetCtrl(hidtoDate20) + "','DD/MM/YYYY')";
                        Project.dal.ExecuteSQL(SQL);
                        foreach (DataRow DRd in DTd.Rows)
                        {
                            Project.dal.MngNgbillDailyUpdate(DBUTIL.opINSERT, "", Utility.AppFormatDate(DRd["RDATE"]), Utility.GetCtrl(hidgNgRptNoDaily20), DRd["C1"], DRd["C2"], DRd["C3"], DRd["IC4"], DRd["NC4"], DRd["IC5"], DRd["NC5"], DRd["C6"], DRd["N2"], DRd["CO2"], DRd["SG"], DRd["GHV"], DRd["NHV"], DRd["WC"], DRd["UNNORMALIZED"], DRd["UNNORMMIN"], DRd["UNNORMMAX"], DRd["WB"], DRd["H2S"], DRd["SUM_COMPO"]);
                        }

                    }


                    //--- เมื่อกดปุ่ม Confirm ให้เก็บ Log ไว้ด้วย
                    BLL.InsertAudit(Project.catConf20DAY, "Confirm (20): FID=" + ddlFID.SelectedItem.Text + ";Month/Year=" + ddlMONTH.SelectedValue + "/" + ddlYEAR.SelectedValue + ";", "");

                    //LoadData();
                    LoadDataSession();

                    ShowConfirm(Project.catConf20DAY, ref lblComfirm4);
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
            int colC1 = -1, colC2 = -1, colC3 = -1, colIC4 = -1, colNC4 = -1, colIC5 = -1, colNC5 = -1, colC6 = -1, colN2 = -1;
            int colCO2 = -1, colSG = -1, colGHV = -1, colNHV = -1, colWC = -1, colUnnorm = -1, colUnnormMin = -1, colUnnormMax = -1;
            int colWB = -1, colH2S = -1;

            object pC1 = null, pC2 = null, pC3 = null, pIC4 = null, pNC4 = null, pIC5 = null, pNC5 = null, pC6 = null, pN2 = null;
            object pCO2 = null, pSG = null, pGHV = null, pNHV = null, pWC = null, pUnnorm = null, pUnnormMin = null, pUnnormMax = null;
            object pWB = null, pH2S = null;

            String ChkDay = "";
            String ChkDate = "";

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

                            //-- ส่งชื่อ worksheet ตามเดือนไป เนื่องจากกรณีมีหลาย worksheet แล้วตัวอ่าน excel จะเรียงตามชื่อ
                            String WSheetList = "|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC|";
                            String WSheet = WSheetList.Split('|')[ Utility.ToInt(hidMM.Value)];

                            DT1 = Exc.ReadWorksheet(FullFileName, WSheet);

                            if ((DT1 == null) || DT1.Rows.Count < 4)
                            {
                                Msg = " - Data not found!";

                            }
                            else {

                                //1) ตรวจสอบ format ของ excel ก่อนว่าถูกต้องหรือเปล่า
                                FormatExcel = CheckFormat(DT1, ref Msg, ref ExMaxCol1, ref ExHeadRow1,ref colC1, ref colC2, ref colC3, ref colIC4, ref colNC4, ref colIC5, ref colNC5, ref colC6, ref colN2, ref colCO2, ref colSG, ref colGHV, ref colNHV, ref colWC, ref colUnnorm, ref colUnnormMin, ref colUnnormMax, ref colWB, ref colH2S);
                                if (FormatExcel > 0)
                                {
                                    //-- delete tmp data
                                    Project.dal.MngTmpDailyGqms(DBUTIL.opDELETE, "", "", Utility.GetCtrl(hidFID));

                                    String MMYY = "/" + Utility.GetCtrl(ddlMONTH).PadLeft(2, '0') + "/" + Utility.GetCtrl(ddlYEAR);
                                    //2) บันทึกใน tmp table
                                    foreach (DataRow DR1 in DT1.Rows)
                                    {
                                        if ( ExRow1 > ExHeadRow1  ) //-- ข้อมูลจะเริ่มจากบรรทัด Date ไปอีก 
                                        {
                                            ChkDay = Utility.ToString(DR1[0]) + "";  //เช่น 1
                                            if ( Utility.IsNumeric(ChkDay) )
                                            {
                                                ChkDate = ChkDay.PadLeft(2, '0') + MMYY;

                                                pC1 = null; pC2 = null; pC3 = null; pIC4 = null; pNC4 = null; pIC5 = null; pNC5 = null; pC6 = null; pN2 = null;
                                                pCO2 = null; pSG = null; pGHV = null; pNHV = null; pWC = null; pUnnorm = null; pUnnormMin = null; pUnnormMax = null;
                                                pWB = null; pH2S = null;

                                                if (colC1 > 0) pC1 = DR1[colC1];        if (colC2 > 0) pC2 = DR1[colC2];
                                                if (colC3 > 0) pC3 = DR1[colC3];        if (colIC4 > 0) pIC4 = DR1[colIC4];
                                                if (colNC4 > 0) pNC4 = DR1[colNC4];     if (colIC5 > 0) pIC5 = DR1[colIC5];
                                                if (colNC5 > 0) pNC5 = DR1[colNC5];     if (colC6 > 0) pC6 = DR1[colC6];
                                                if (colN2 > 0) pN2 = DR1[colN2];        if (colCO2 > 0) pCO2 = DR1[colCO2];
                                                if (colSG > 0) pSG = DR1[colSG];        if (colGHV > 0) pGHV = DR1[colGHV];
                                                if (colNHV > 0) pNHV = DR1[colNHV];     if (colWC > 0) pWC = DR1[colWC];
                                                if (colUnnorm > 0) pUnnorm = DR1[colUnnorm];
                                                if (colUnnormMin > 0) pUnnormMin = DR1[colUnnormMin];
                                                if (colUnnormMax > 0) pUnnormMax = DR1[colUnnormMax];
                                                if (colWB > 0) pWB = DR1[colWB];        if (colH2S > 0) pH2S = DR1[colH2S];

                                                //-- 15/08/2018 บางทีก็อาจจะเป็นการไม่ใช้ข้อมูลของวันนั้นก็ได้ เลยอนุญาตให้บันทึกข้อมูลในวันที่ไม่มีข้อมูล
                                                //-- 14/08/2018 จะ import เฉพาะวันที่มีข้อมูลเท่านั้น
                                                //if ( Utility.ToString(pC1) != "" || Utility.ToString(pC2) != "" || Utility.ToString(pC3) != "")
                                                //{
                                                    Project.dal.MngTmpDailyGqms(DBUTIL.opINSERT, "", ChkDate, Utility.GetCtrl(hidFID), pC1, pC2, pC3, pIC4, pNC4, pIC5, pNC5, pC6, pN2, pCO2, pSG, pGHV, pNHV, pWC, pUnnorm, pUnnormMin, pUnnormMax, pWB, pH2S);
                                                //}

                                                

                                            }
                                        }

                                        ExRow1++;
                                    }


                                    if (Msg == "")
                                    {
                                        //5) บันทึกลง GQMS_DAILY_UPDATE
                                        Project.dal.MngTmp2GqmsDailyUpdate(Utility.GetCtrl(hidFID), WCFlag: true); //-- EDIT 03/02/2020 -- เพิ่ม flag ให้ update ค่าน้ำ WC 
                                        MsgSuccess = "Successfully imported [" + FileName + "] </br>";
                                        LoadData();
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


      
        private int CheckFormat(DataTable DT, ref string StrFormat, ref int ExMaxCol, ref int ExRow, ref int colC1, ref int colC2, ref int colC3, ref int colIC4, ref int colNC4, ref int colIC5, ref int colNC5, ref int colC6, ref int colN2, ref int colCO2, ref int colSG, ref int colGHV, ref int colNHV, ref int colWC, ref int colUnnorm, ref int colUnnormMin, ref int colUnnormMax, ref int colWB, ref int colH2S )
        {
            string dataA = "";
            string data = "";
            int ChkFormat = 0;
            int rowIndex = 0;

            int maxCol = 18;  //มี 19 คอลัมน์ แต่เริ่มจาก 0  ถ้ามี H2S 
            bool H2SFlag = false;
            try
            {
                //-- มี 2 รูปแบบ -------------------------------------------------------------------------
                //   แบบที่ 1 มีคอลัมน์ H2S ,  แบบที่ 2 ไม่มีคอลัมน์ H2S
                //มีแบบ เหมือนรายงาน monthly  _______________________________________________________________________________
                //บรรทัด1			    												  ต้นฉบับ			สำเนา	
                //บรรทัด2	    PTT  PUBLIC  COMPANY  LIMITED										59 Moo 8, Bypass Road , T. Napa ,A. Muang , Chonburi , 20000		
                //บรรทัด3	    Operation  Center  Laboratory										Tel : (02)537-2000  ext.  35106-7					
                //บรรทัด4 เส้นขีด
                //บรรทัด5        							Summary   BCS   Composition (Data  from  Online  Gas  Chromatograph)									
                //บรรทัด6	    Report  Number  :	 OGC796											OGC  Period   :	January  1-31, 2018						
                //บรรทัด7 เส้นขีด
                //บรรทัด8	    Date	CH4	    C2H6	C3H8	i-C4H10	n-C4H10	i-C5H12	n-C5H12	C6H14	CO2	    N2	    H2S	netHVdry	Hvsat	SG	    H2O	    unnormmin	unnormmax	chromat
                //บรรทัด9    	unit				                mole %								                    Btu/scf		        -	    Lb/MMscf		mole %			run
                //บรรทัด10   1	    89.906	1.965	0.697	0.177	0.150	0.070	0.038	0.031	5.030	1.936	    884.773	    964.210	0.6355	1.41	99.467	    100.318	    360
                //Column=>  A	    B       C	    D	    E	    F	    G	    H	    I	    J	    K	    L	M	        N	    O	    P	    Q	        R	        S

                //--- การอ่านไฟล์ ถ้าคอลัมน์แรกไม่มีข้อมูล ระบบจะไม่อ่าน record นั้น ดังนั้นจึงทำให้บรรทัดเลื่อนได้
                //-- ดังนั้นจะตรวจสอบจากบรรทัด Date ในคอลัมน์ A
                //-- HEADER ROW=//บรรทัด8	    Date	CH4	    C2H6	C3H8	i-C4H10	n-C4H10	i-C5H12	n-C5H12	C6H14	CO2	    N2	    H2S	netHVdry	Hvsat	SG	    H2O	    unnormmin	unnormmax	chromat
                //-- จะปรับ upper และตัด - ออก 
                //"|DATE|CH4|C2H6|C3H8|IC4H10|NC4H10|IC5H12|NC5H12|C6H14|CO2|N2|H2S|NETHVDRY|HVSAT|SG|H2O|UNNORMMIN|UNNORMMAX|CHROMAT|";

                if (maxCol > DT.Columns.Count-1 ) maxCol = DT.Columns.Count-1;

                foreach (DataRow DR in DT.Rows)
                {
                    dataA = Utility.ToString(GetCellData(DR, 0)).ToUpper().Trim();
                    if ( dataA == "DATE" )
                    {
                        ExMaxCol = maxCol;

                        for (int col = 0; col < ExMaxCol; col++)
                        {
                            data = Utility.ToString(GetCellData(DR, col)).ToUpper().Trim().Replace("-","");
                            //-- edit 22/07/2019 --- บางครั้งมี Mark: out of scope ตรง header ด้วย เช่น i-C5H12[1][2]
                            data = data.Replace("[1]", "").Replace("[2]", "");

                            switch (data)
                            {
                                case "DATE": break;
                                case "CH4": colC1 = col; break;
                                case "C2H6": colC2 = col; break;
                                case "C3H8": colC3 = col; break;
                                case "IC4H10": colIC4 = col; break;
                                case "NC4H10": colNC4 = col; break;
                                case "IC5H12": colIC5 = col; break;
                                case "NC5H12": colNC5 = col; break;
                                case "C6H14": colC6 = col; break;
                                case "CO2": colCO2 = col; break;
                                case "N2": colN2 = col; break;
                                case "H2S": colH2S = col; H2SFlag = true; break;
                                case "NETHVDRY": colNHV = col; break;
                                case "HVSAT": colGHV = col; break;
                                case "SG": colSG = col; break;
                                case "H2O": colWC = col; break;
                                case "UNNORM": colUnnorm = col; break;
                                case "UNNORMMIN": colUnnormMin = col; break;
                                case "UNNORMMAX": colUnnormMax = col; break;
                                case "CHROMAT":  break; //-- ไม่ต้อง import
                                default:
                                    Msg = " - คอลัมน์ที่ " + Utility.ToString(col + 1) + " (" + Utility.ToString(GetCellData(DR, col)) + ") ชื่อคอลัมน์ไม่ถูกต้อง!";
                                    ChkFormat = 0; col = 99; break;
                            }

                        }

                        if ( Msg == "")
                        {
                            if (H2SFlag)
                                ChkFormat = 1;
                            else
                                ChkFormat = 2;
                            ExRow = rowIndex;
                        }
                        break;
                    }

                    rowIndex++;
                    if (rowIndex > 8) break;
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


        //////======================================================================
        //////  SPOT
        //////======================================================================

        protected void gvSpot_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    //create a new row  (SPOT HEADER)-------------------------------------------------------------------

                    GridView gv = sender as GridView;
                    TableCell cell = null;
                    e.Row.TableSection = TableRowSection.TableHeader;
                    GridViewRow extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                    extraHeader.TableSection = TableRowSection.TableHeader;

                    cell = new TableCell(); cell.Text = "";
                    cell.Width = 40; cell.CssClass = "cell-border"; extraHeader.Cells.Add(cell);

                    if (Utility.GetCtrl(hidsH2S_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "SPOT H2S (ppm)";
                        cell.ColumnSpan = 3; cell.Width = 270; cell.CssClass = "cell-center Table-head-orange"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsHG_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "SPOT Hg (ug/cu.m.)";
                        cell.ColumnSpan = 3; cell.Width = 270; cell.CssClass = "cell-center Table-head-success"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsO2_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "SPOT O2 (mole %)";
                        cell.ColumnSpan = 3; cell.Width = 270; cell.CssClass = "cell-center Table-head-primary"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsHC_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "H-C dew point (Deg. F)";
                        cell.ColumnSpan = 3; cell.Width = 270; cell.CssClass = "cell-center Table-head-danger"; extraHeader.Cells.Add(cell);
                    }

                    //add the new row to the gridview
                    gv.Controls[0].Controls.AddAt(0, extraHeader);

                    

                    //create a new row  (SPOT HEADER)-------------------------------------------------------------------
                    extraHeader = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
                    extraHeader.TableSection = TableRowSection.TableHeader;

                    cell = new TableCell(); cell.Text = "Edit";
                    cell.Width = 40; cell.CssClass = "Table-head-gray cell-center"; extraHeader.Cells.Add(cell);

                    if (Utility.GetCtrl(hidsH2S_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "Sampling Point";
                        cell.Width = 110; cell.CssClass = "cell-center Table-head-orange"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Date";
                        cell.Width = 90; cell.CssClass = "cell-center Table-head-orange"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "H2S";
                        cell.Width = 70; cell.CssClass = "cell-center Table-head-orange"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsHG_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "Sampling Point";
                        cell.Width = 110; cell.CssClass = "cell-center Table-head-success"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Date";
                        cell.Width = 90; cell.CssClass = "cell-center Table-head-success"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Hg";
                        cell.Width = 70; cell.CssClass = "cell-center Table-head-success"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsO2_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "Sampling Point";
                        cell.Width = 110; cell.CssClass = "cell-center Table-head-primary"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Date";
                        cell.Width = 90; cell.CssClass = "cell-center Table-head-primary"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "O2";
                        cell.Width = 70; cell.CssClass = "cell-center Table-head-primary"; extraHeader.Cells.Add(cell);
                    }
                    if (Utility.GetCtrl(hidsHC_NAME) != "")
                    {
                        cell = new TableCell(); cell.Text = "Sampling Point";
                        cell.Width = 110; cell.CssClass = "cell-center Table-head-danger"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Date";
                        cell.Width = 90; cell.CssClass = "cell-center Table-head-danger"; extraHeader.Cells.Add(cell);
                        cell = new TableCell(); cell.Text = "Temp";
                        cell.Width = 70; cell.CssClass = "cell-center Table-head-danger"; extraHeader.Cells.Add(cell);
                    }

                    //add the new row to the gridview
                    gv.Controls[0].Controls.AddAt(1, extraHeader);
                    //---------------------------------------------------------------
                }
                else
                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;
               

                    if (ServerAction == "EDIT_SPOT")
                    {
                        //-- edit column 0
                        e.Row.Cells[0].Text = "<li class=\"fa fa-save fa-lg\" style=\"color: #009933;\" onclick=\"DoAction('SAVE_SPOT'," + e.Row.DataItemIndex + ")\"></li> ";

                        e.Row.CssClass = "itemSelect";
                      
                    }

                    //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                    //-- SPOT H2S (ppm) column 1-3
                    if (Utility.GetCtrl(hidsH2S_NAME) != "")
                    {
                        ShowValueSPOT(ref e, 1, 3, dr);
                    }
                    //-- SPOT Hg (ug/cu.m.) column 4-6
                    if (Utility.GetCtrl(hidsHG_NAME) != "")
                    {
                        ShowValueSPOT(ref e, 4, 6, dr);
                    }
                    //-- SPOT O2 (mole %) column 7-9 
                    if (Utility.GetCtrl(hidsO2_NAME) != "")
                    {
                        ShowValueSPOT(ref e, 7, 9, dr);
                    }
                    //-- H-C dew point (Deg. F) column 10-12 
                    if (Utility.GetCtrl(hidsHC_NAME) != "")
                    {
                        ShowValueSPOT(ref e, 10, 12, dr);
                    }
                    
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

        private void ShowValueSPOT(ref GridViewRowEventArgs gRow, int sCol, int eCol, DataRowView gDR)
        {
            String result = "";
            String fd = "";
            String ddl = "";
            String sName = "";
            try
            {
                for (int c = sCol; c <= eCol; c++)
                {
                    result = "";
                    fd = ConfigColSPOT(c);
                    if (fd != "")
                    {
                        if (Utility.ToString(gDR[fd]) != "")
                        {
                            if ( fd.IndexOf("_NAME")> 0)
                            {
                                result = Utility.ToString(gDR[fd]);
                                if (ServerAction == "EDIT_SPOT")
                                {
                                    DataTable DTd = Project.dal.SearchSiteFID(Utility.GetCtrl(hidSITE_ID));
                                    DataRow DRd = Utility.GetDR(ref DTd);

                                    ddl = "";
                                    switch (fd)
                                    {
                                        case "H2S_NAME":
                                            sName = Utility.ToString(DRd["H2S_NAME1"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsH2S_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }
                                            sName = Utility.ToString(DRd["H2S_NAME2"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsH2S_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }

                                            ddl = "<select id=\"ddlSH2S\" name=\"ddlSH2S\" style=\"width: 150px;\" onchange=\"SelectSH2S()\" > " + ddl + "</select>";
                                            break;

                                        case "HG_NAME":
                                            sName = Utility.ToString(DRd["HG_NAME1"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsHG_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }
                                            sName = Utility.ToString(DRd["HG_NAME2"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsHG_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }

                                            ddl = "<select id=\"ddlSHG\" name=\"ddlSHG\" style=\"width: 150px;\" onchange=\"SelectSHG()\" > " + ddl + "</select>";
                                            break;

                                        case "O2_NAME":
                                            sName = Utility.ToString(DRd["O2_NAME1"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsO2_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }
                                            sName = Utility.ToString(DRd["O2_NAME2"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsO2_NAME))
                                                    ddl += "<option value='" + sName + "' selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value='" + sName + "'>" + sName + "</option>";
                                            }

                                            ddl = "<select id=\"ddlSO2\" name=\"ddlSO2\" style=\"width: 150px;\" onchange=\"SelectSO2()\" > " + ddl + "</select>";
                                            break;

                                        case "HC_NAME":
                                            sName = Utility.ToString(DRd["HC_NAME1"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsHC_NAME))
                                                    ddl += "<option value=\"" + sName + "\" selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value=\"" + sName + "\">" + sName + "</option>";
                                            }
                                            sName = Utility.ToString(DRd["HC_NAME2"]);
                                            if (sName != "")
                                            {
                                                if (sName == Utility.GetCtrl(hidsHC_NAME))
                                                    ddl += "<option value=\"" + sName + "\" selected>" + sName + "</option>";
                                                else
                                                    ddl += "<option value=\"" + sName + "\">" + sName + "</option>";
                                            }

                                            ddl = "<select id=\"ddlSHC\" name=\"ddlSHC\"  style=\"width: 150px;\"   onchange=\"SelectSHC()\" > " + ddl + "</select>";
                                            break;

                                    }

                                    gRow.Row.Cells[c].Text = ddl;
                                }
                                else
                                {
                                    gRow.Row.Cells[c].Text = result;
                                }

                            }
                            else if(fd.IndexOf("_DATE") > 0)
                            {
                                result = Utility.AppFormatDate(gDR[fd]);
                                //  กำหนดเป็น textbox ไม่ให้แก้ไข
                                if (ServerAction == "EDIT_SPOT")
                                {
                                    gRow.Row.Cells[c].Text = "<input type=\"text\" class=\"input-center\" readonly=\"readonly\" id=\"txtS" + fd + "\" name=\"txtS" + fd + "\" style=\"width:95px\" MaxLength=\"10\" value=\"" + result + "\"  />";

                                }
                                else
                                {
                                    gRow.Row.Cells[c].Text = result;
                                }
                            }
                            else
                            {
                                //-- format number ต้องตรวจสอบก่อนว่าข้อมูลเป็นตัวเลขหรือไม่ ------
                                if (Utility.IsNumeric(gDR[fd]))
                                {
                                    if (fd =="H2S")
                                        result = Utility.FormatNum(gDR[fd], 2);
                                    else
                                        result = Utility.FormatNum(gDR[fd], 3);
                                }
                                else
                                {
                                    result = Utility.ToString(gDR[fd]);
                                }
                                //  กำหนดเป็น textbox ไม่ให้แก้ไข
                                if (ServerAction == "EDIT_SPOT")
                                {
                                    gRow.Row.Cells[c].Text = "<input type=\"text\" class=\"input-right\" readonly=\"readonly\"  id=\"txtS" + fd + "\" name=\"txtS" + fd + "\" style=\"width:65px\" MaxLength=\"10\" value=\"" + result + "\"  />";

                                }
                                else
                                {
                                    gRow.Row.Cells[c].Text = result;
                                }

                            }
                        }
                    }



                }

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);

            }
        }


        private String ConfigColSPOT(int gCol)
        {
            String result = "";
            try
            {
                switch (gCol)
                {
                    //-- SPOT H2S (ppm) column 1-3
                    case 1: result = "H2S_NAME"; break;
                    case 2: result = "H2S_DATE"; break;
                    case 3: result = "H2S"; break;
                    //-- SPOT Hg (ug/cu.m.) column 4-6
                    case 4: result = "HG_NAME"; break;
                    case 5: result = "HG_DATE"; break;
                    case 6: result = "HG"; break;
                    //-- SPOT O2 (mole %) column 7-9 
                    case 7: result = "O2_NAME"; break;
                    case 8: result = "O2_DATE"; break;
                    case 9: result = "O2"; break;
                    //-- H-C dew point (Deg. F) column 10-12 
                    case 10: result = "HC_NAME"; break;
                    case 11: result = "HC_DATE"; break;
                    case 12: result = "HC"; break;
 
                }

                return result;

            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
                return "";
            }
        }


        private void SaveSPOTData()
        {
            String pH2S = "", pHG = "", pO2 = "", pHC = "";
            try
            {
                //--- จะบันทึกลงที่ O_SPOT_UPDATE

 
                pH2S = Validation.ValidateStr(Request.Form["ddlSH2S"]);
                pHG = Validation.ValidateStr(Request.Form["ddlSHG"]);
                pO2 = Validation.ValidateStr(Request.Form["ddlSO2"]);
                pHC = Validation.ValidateStr(Request.Form["ddlSHC"]);

                Project.dal.MngOSpotUpdate(DBUTIL.opUPDATE, Utility.GetCtrl(hidFID), Utility.GetCtrl(hidfromDate), pH2S, pHG, pO2, pHC);


                LoadData();

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }


        //////======================================================================
        //////======================================================================



    }
}