using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 07/07/2018 
    //30/08/2018 เพิ่ม OGC site สำรอง 1,2,3
    // เพิ่ม DAILY ของ report27
    public partial class MngSiteDetail : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public String Key;

        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDSite, true);

                SetCtrl();
                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();

                    Key = Validation.GetParamStr("K", IsEncoded: true);
                    if (Utility.IsNumeric(Key))
                    {
                        Utility.SetCtrl(hidSITE_ID, Key);
                        ServerAction = "LOAD";
                    }
                    else {

                        ServerAction = "ADD";
                    }
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");
                }



                switch (ServerAction)
                {
                    case "ADD": if (canAdd) { AddData(); } break;
                    case "LOAD":
                        LoadData();
                        break;
                    case "SAVE":
                        if (canAdd || canEdit)
                        {
                            if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                            {
                                SaveData();
                                HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                            }
                        }
                        break;
                    case "DELETE":
                        if (canDelete)
                        {
                            DeleteData();
                        }
                        break;

                    case "VIEW_STD":
                        LoadSTDData();
                        break;
                    case "DELETE_STD":
                        //DeleteSTDData();
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
                canEdit = Security.CanDo(Security.TaskMDSite, Security.actAdd);
                canDelete = canEdit;
                canAdd = canEdit;


                pnlDELETE.Visible = (canDelete) ? true : false;
                pnlSAVE.Visible = (canEdit) ? true : false;


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
                DT = Project.dal.SearchDimRegion();
                Utility.LoadList(ref ddlREGION_ID, DT, "REGION_NAME", "REGION_ID", true, "");

                //-- OMA Moisture ---
                DT = Project.dalhs.SearchDimMoisture();
                Utility.LoadList(ref ddlOMA_NAME1, DT, "NAME_TAGNAME", "NAME", true, "none");
                Utility.LoadList(ref ddlOMA_NAME2, DT, "NAME_TAGNAME", "NAME", false, "none"); //เพิ่ม blank line จาก บรรทัดบนแล้ว

                //-- FLOW ---
                DT = Project.dalhs.SearchDimFlowRate();
                Utility.LoadList(ref ddlFLOW_NAME1, DT, "NAME_TAGNAME", "NAME", true, "none");
                Utility.LoadList(ref ddlFLOW_NAME2, DT, "NAME_TAGNAME", "NAME", false, "none"); //เพิ่ม blank line จาก บรรทัดบนแล้ว

                //-- OGC FID --
                DT = Project.dal.SearchSiteFID( orderSQL: "FID");
                //-- edit 19/08/2022 --- เพิ่ม Last Good
                DataRow dr = null;
                dr = DT.NewRow();
                dr["FID"] = "LAST_GOOD";
                dr["FID_NAME"] = "Last Good";
                DT.Rows.InsertAt(dr, 0);

                Utility.LoadList(ref ddlOGC_NAME1, DT, "FID_NAME", "FID", true, "none"); //-- edit 19/07/2019 --- บางครั้งต้องการแสดงเป็น none แต่ value ให้เป็น "" แต่บังเอิญใช้ field เดียวกัน
                Utility.LoadList(ref ddlOGC_NAME2, DT, "FID_NAME", "FID", false, "none"); //เพิ่ม blank line จาก บรรทัดบนแล้ว
                Utility.LoadList(ref ddlOGC_NAME3, DT, "FID_NAME", "FID", false, "none");

                //-- Add by Turk 18/04/2562 --> H2S, HG, O2, H-C dew point --
                //-- H2S --
                DT = Project.dal.SearchDimH2S();
                Utility.LoadList(ref ddlH2S_NAME1, DT, "H2S_NAME", "NAME", true, "none");    //-- edit 19/07/2019 --- บางครั้งต้องการแสดงเป็น none แต่ value ให้เป็น "" แต่บังเอิญใช้ field เดียวกัน
                Utility.LoadList(ref ddlH2S_NAME2, DT, "H2S_NAME", "NAME", false, "none");

                //-- HG --
                DT = Project.dal.SearchDimHG();
                Utility.LoadList(ref ddlHG_NAME1, DT, "HG_NAME", "NAME", true, "none");
                Utility.LoadList(ref ddlHG_NAME2, DT, "HG_NAME", "NAME", false, "none");

                //-- O2 --
                DT = Project.dal.SearchDimO2();
                Utility.LoadList(ref ddlO2_NAME1, DT, "O2_NAME", "NAME", true, "none");
                Utility.LoadList(ref ddlO2_NAME2, DT, "O2_NAME", "NAME", false, "none");

                //-- H-C dew point --
                DT = Project.dal.SearchDimHC();
                Utility.LoadList(ref ddlHC_NAME1, DT, "HC_NAME", "NAME", true, "none");
                Utility.LoadList(ref ddlHC_NAME2, DT, "HC_NAME", "NAME", false, "none");


                Utility.SetCtrl(hidSTD_ID, ""); //-- edit 19/09/2019 -- clear std

                //-- EDIT 28/06/2023 --- Analysis method
                DT = Project.dal.SearchDimAnalysisMethod(); 
                Utility.LoadList(ref ddlANLMET_ID, DT, "ANLMET_NAME", "ANLMET_ID", true, "");

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


        private void AddData()
        {
            DataTable DT = null;
            try
            {
                Utility.SetCtrl(hidSITE_ID, "");
                Utility.SetCtrl(hidRPT_ID1, "");
                Utility.SetCtrl(hidRPT_ID2, "");
                Utility.SetCtrl(hidRPT_ID3, "");
                Utility.SetCtrl(hidSGCSITE_ID, "");
                Utility.SetCtrl(hidSTD_ID, "");


                Utility.SetCtrl(txtFID, "");
                Utility.SetCtrl(txtSITE_NAME, "");
                Utility.SetCtrl(ddlREGION_ID, "");
                Utility.SetCtrl(ddlALERT_FLAG, "N");

                Utility.SetCtrl(ddlISO_FLAG, "N");
                Utility.SetCtrl(ddlH2S_FLAG, "N");
                //-- Report --
                Utility.SetCtrl(txtNGBILL_RPT_NO1, "");
                Utility.SetCtrl(txtGC_RPT_NO1, "");
                Utility.SetCtrl(txtGC_RPT_NAME1, "");
                Utility.SetCtrl(txtNGBILL_RPT_NO2, "");
                Utility.SetCtrl(txtGC_RPT_NO2, "");
                Utility.SetCtrl(txtGC_RPT_NAME2, "");
                Utility.SetCtrl(txtNGBILL_RPT_NO3, "");
                Utility.SetCtrl(txtGC_RPT_NO3, "");
                Utility.SetCtrl(txtGC_RPT_NAME3, "");
                Utility.SetCtrl(txtNGBILL_RPT_NO4, "");
                Utility.SetCtrl(txtGC_RPT_NO4, "");
                Utility.SetCtrl(txtGC_RPT_NAME4, "");

                Utility.SetCtrl(txtNGBILL_RPT_NO5, ""); //-- edit 29/11/2019 เพิ่ม 20 Days ---
                Utility.SetCtrl(txtGC_RPT_NO5, "");
                Utility.SetCtrl(txtGC_RPT_NAME5, "");
                Utility.SetCtrl(txtNGBILL_RPT_NO6, "");
                Utility.SetCtrl(txtGC_RPT_NO6, "");
                Utility.SetCtrl(txtGC_RPT_NAME6, "");

                //-- Gas composition
                Utility.SetCtrl(txtCYLINDER_NO , "");
                Utility.SetCtrl(hidORDER_DATE, ""); //-- edit 19/09/2019 
                Utility.SetCtrl(datORDER_DATE , "");
                Utility.SetCtrl(datEXPIRE_DATE , "");
                Utility.SetCtrl(txtC1 , "");
                Utility.SetCtrl(txtC2 , "");
                Utility.SetCtrl(txtC3 , "");
                Utility.SetCtrl(txtIC4 , "");
                Utility.SetCtrl(txtNC4 , "");
                Utility.SetCtrl(txtIC5 , "");
                Utility.SetCtrl(txtNC5 , "");
                Utility.SetCtrl(txtC6 , "");
                Utility.SetCtrl(txtN2 , "");
                Utility.SetCtrl(txtCO2 , "");
                Utility.SetCtrl(txtH2S , "");
                //-- Add by Turk 18/04/2562 --> HG--
                Utility.SetCtrl(txtHG , "");

                //-- Gas composition ISO MIN /MAX
                Utility.SetCtrl(txtC1_MIN, ""); Utility.SetCtrl(txtC1_MAX, "");
                Utility.SetCtrl(txtC2_MIN, ""); Utility.SetCtrl(txtC2_MAX, "");
                Utility.SetCtrl(txtC3_MIN, ""); Utility.SetCtrl(txtC3_MAX, "");
                Utility.SetCtrl(txtIC4_MIN, ""); Utility.SetCtrl(txtIC4_MAX, "");
                Utility.SetCtrl(txtNC4_MIN, ""); Utility.SetCtrl(txtNC4_MAX, "");
                Utility.SetCtrl(txtIC5_MIN, ""); Utility.SetCtrl(txtIC5_MAX, "");
                Utility.SetCtrl(txtNC5_MIN, ""); Utility.SetCtrl(txtNC5_MAX, "");
                Utility.SetCtrl(txtC6_MIN, ""); Utility.SetCtrl(txtC6_MAX, "");
                Utility.SetCtrl(txtN2_MIN, ""); Utility.SetCtrl(txtN2_MAX, "");
                Utility.SetCtrl(txtCO2_MIN, ""); Utility.SetCtrl(txtCO2_MAX, "");
                Utility.SetCtrl(txtH2S_MIN, ""); Utility.SetCtrl(txtH2S_MAX, "");
                Utility.SetCtrl(txtH2S_MIN, ""); Utility.SetCtrl(txtH2S_MAX, "");



                //-- OMA
                Utility.SetCtrl(ddlOMA_NAME1 , "");
                Utility.SetCtrl(ddlOMA_NAME2 , "");

                //-- Chomat run
                Utility.SetCtrl(txtTOTAL_RUN , "");
                Utility.SetCtrl(txtTOLERANCE_RUN, "");

                //-- Flow
                Utility.SetCtrl(ddlFLOW_NAME1 , "");
                Utility.SetCtrl(ddlFLOW_NAME2, "");

                //-- OGC FID
                Utility.SetCtrl(ddlOGC_NAME1, "");
                Utility.SetCtrl(ddlOGC_NAME2, "");
                Utility.SetCtrl(ddlOGC_NAME3, "");

                //-- Add by Turk 18/04/2562 --> H2S, HG, O2, H-C dew point --
                //-- H2S
                Utility.SetCtrl(ddlH2S_NAME1, "");
                Utility.SetCtrl(ddlH2S_NAME2, "");

                //-- HG
                Utility.SetCtrl(ddlHG_NAME1, "");
                Utility.SetCtrl(ddlHG_NAME2, "");

                //-- O2
                Utility.SetCtrl(ddlO2_NAME1, "");
                Utility.SetCtrl(ddlO2_NAME2, "");

                //-- H-C dew point
                Utility.SetCtrl(ddlHC_NAME1, "");
                Utility.SetCtrl(ddlHC_NAME2, "");

                Utility.SetCtrl(lblLastUpdated, "");

                //-- EDIT 28/06/2023 --- Analysis method
                Utility.SetCtrl(ddlANLMET_ID, "");

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
            DataRow DR = null;
            DataTable DTrpt = null;
            DataRow DRrpt = null;
            DataTable DTgas = null;
            DataRow DRgas = null;
            try
            {

                DT = Project.dal.SearchSiteFID(Validation.GetCtrlInt(hidSITE_ID).ToString());
                if (DT != null && DT.Rows.Count > 0)
                {
                    DR = Utility.GetDR(ref DT);

                    Utility.SetCtrl(txtFID, Utility.ToString(DR["FID"]));
                    Utility.SetCtrl(txtSITE_NAME, Utility.ToString(DR["SITE_NAME"]));
                    Utility.SetCtrl(ddlREGION_ID, Utility.ToString(DR["REGION_ID"]));
                    Utility.SetCtrl(ddlALERT_FLAG, Utility.ToString(DR["ALERT_FLAG"]));
                    Utility.SetCtrl(ddlISO_FLAG, Utility.ToString(DR["ISO_FLAG"]));
                    Utility.SetCtrl(ddlH2S_FLAG, Utility.ToString(DR["H2S_FLAG"]));

                    Utility.SetCtrl(ddlOMA_NAME1, Utility.ToString(DR["OMA_NAME1"]));
                    Utility.SetCtrl(ddlOMA_NAME2, Utility.ToString(DR["OMA_NAME2"]));

                    Utility.SetCtrl(txtTOTAL_RUN, Utility.FormatNum(DR["TOTAL_RUN"]) );
                    Utility.SetCtrl(txtTOLERANCE_RUN, Utility.FormatNum(DR["TOLERANCE_RUN"]));

                    Utility.SetCtrl(ddlFLOW_NAME1, Utility.ToString(DR["FLOW_NAME1"]));
                    Utility.SetCtrl(ddlFLOW_NAME2, Utility.ToString(DR["FLOW_NAME2"]));

                    Utility.SetCtrl(ddlOGC_NAME1, Utility.ToString(DR["OGC_NAME1"]));
                    Utility.SetCtrl(ddlOGC_NAME2, Utility.ToString(DR["OGC_NAME2"]));
                    Utility.SetCtrl(ddlOGC_NAME3, Utility.ToString(DR["OGC_NAME3"]));

                    //-- Add by Turk 18/04/2562 --> H2S, HG, O2, H-C dew point --
                    Utility.SetCtrl(ddlH2S_NAME1, Utility.ToString(DR["H2S_NAME1"]));
                    Utility.SetCtrl(ddlH2S_NAME2, Utility.ToString(DR["H2S_NAME2"]));

                    Utility.SetCtrl(ddlHG_NAME1, Utility.ToString(DR["HG_NAME1"]));
                    Utility.SetCtrl(ddlHG_NAME2, Utility.ToString(DR["HG_NAME2"]));

                    Utility.SetCtrl(ddlO2_NAME1, Utility.ToString(DR["O2_NAME1"]));
                    Utility.SetCtrl(ddlO2_NAME2, Utility.ToString(DR["O2_NAME2"]));

                    Utility.SetCtrl(ddlHC_NAME1, Utility.ToString(DR["HC_NAME1"]));
                    Utility.SetCtrl(ddlHC_NAME2, Utility.ToString(DR["HC_NAME2"]));

                    //-- EDIT 28/06/2023 --- Analysis method
                    Utility.SetCtrl(ddlANLMET_ID, Utility.ToString(DR["ANLMET_ID"]));


                    Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);
                  
                    
                    //==== Reports ==============================================================================
                    //-- DAILY
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE1);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID1, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO1, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO1, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME1, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }

                    //-- DAILY27  30/08/2018 เพิ่ม DAILY ของ report27
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE4);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID4, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO4, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO4, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME4, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }

                    //-- 27DAY
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE2);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID2, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO2, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO2, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME2, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }

                    //-- ENDMTH
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE3);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID3, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO3, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO3, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME3, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }

                    //-- edit 29/11/2019 เพิ่ม 20 Days ---
                    //-- Daily(20)
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE5);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID5, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO5, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO5, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME5, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }
                    //-- 20 Days
                    DTrpt = Project.dal.SearchSiteReport(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), RptType: Project.lkRPT_TYPE6);
                    if (DTrpt != null && DTrpt.Rows.Count > 0)
                    {
                        DRrpt = Utility.GetDR(ref DTrpt);

                        Utility.SetCtrl(hidRPT_ID6, Utility.ToString(DRrpt["RPT_ID"]));
                        Utility.SetCtrl(txtNGBILL_RPT_NO6, Utility.ToString(DRrpt["NGBILL_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NO6, Utility.ToString(DRrpt["GC_RPT_NO"]));
                        Utility.SetCtrl(txtGC_RPT_NAME6, Utility.ToString(DRrpt["GC_RPT_NAME"]));
                    }

                    //==== Gas composition ==============================================================================
                    LoadComponentData();

                    //int cntS = Utility.ToInt(Project.dal.GetSQLValue("SELECT COUNT(*) FROM O_SITE_SGC WHERE  SITE_ID = " + Validation.GetCtrlInt(hidSITE_ID).ToString()));
                    //if (cntS > 1)
                    //    btnFind.Visible = true;
                    //else
                    //    btnFind.Visible = false;

                    //if (Validation.GetCtrlStr(hidSTD_ID) != "" )
                    //    DTgas = Project.dal.SearchSiteSgc(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(),StdID: Validation.GetCtrlStr(hidSTD_ID));
                    //else
                    //    DTgas = Project.dal.SearchSiteSgc(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString());


                    //if (DTgas != null && DTgas.Rows.Count > 0)
                    //{
                    //    DRgas = Utility.GetDR(ref DTgas);

                    //    Utility.SetCtrl(hidSGCSITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                    //    Utility.SetCtrl(hidSTD_ID, Utility.ToString(DRgas["STD_ID"])); //-- edit 19/09/2019 
                    //    Utility.SetCtrl(txtCYLINDER_NO, Utility.ToString(DRgas["CYLINDER_NO"]));
                    //    Utility.SetCtrl(hidORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"])); //-- edit 19/09/2019 
                    //    Utility.SetCtrl(datORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"]));
                    //    Utility.SetCtrl(datEXPIRE_DATE, Utility.AppFormatDate(DRgas["EXPIRE_DATE"]));
                    //    Utility.SetCtrl(txtC1, Utility.ToString(DRgas["C1"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtC2, Utility.ToString(DRgas["C2"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtC3, Utility.ToString(DRgas["C3"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtIC4, Utility.ToString(DRgas["IC4"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtNC4, Utility.ToString(DRgas["NC4"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtIC5, Utility.ToString(DRgas["IC5"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtNC5, Utility.ToString(DRgas["NC5"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtC6, Utility.ToString(DRgas["C6"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtN2, Utility.ToString(DRgas["N2"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtCO2, Utility.ToString(DRgas["CO2"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtH2S, Utility.ToString(DRgas["H2S"]), IsReadOnly: true);
                    //    Utility.SetCtrl(txtHG, Utility.ToString(DRgas["HG"]), IsReadOnly: true);

                    //    //==== Gas composition ISO MIN/MAX==============================================================================
                    //    Utility.SetCtrl(hidTISISITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                    //    Utility.SetCtrl(txtC1_MIN, Utility.ToString(DRgas["C1_MIN"])); Utility.SetCtrl(txtC1_MAX, Utility.ToString(DRgas["C1_MAX"]));
                    //    Utility.SetCtrl(txtC2_MIN, Utility.ToString(DRgas["C2_MIN"])); Utility.SetCtrl(txtC2_MAX, Utility.ToString(DRgas["C2_MAX"]));
                    //    Utility.SetCtrl(txtC3_MIN, Utility.ToString(DRgas["C3_MIN"])); Utility.SetCtrl(txtC3_MAX, Utility.ToString(DRgas["C3_MAX"]));
                    //    Utility.SetCtrl(txtIC4_MIN, Utility.ToString(DRgas["IC4_MIN"])); Utility.SetCtrl(txtIC4_MAX, Utility.ToString(DRgas["IC4_MAX"]));
                    //    Utility.SetCtrl(txtNC4_MIN, Utility.ToString(DRgas["NC4_MIN"])); Utility.SetCtrl(txtNC4_MAX, Utility.ToString(DRgas["NC4_MAX"]));
                    //    Utility.SetCtrl(txtIC5_MIN, Utility.ToString(DRgas["IC5_MIN"])); Utility.SetCtrl(txtIC5_MAX, Utility.ToString(DRgas["IC5_MAX"]));
                    //    Utility.SetCtrl(txtNC5_MIN, Utility.ToString(DRgas["NC5_MIN"])); Utility.SetCtrl(txtNC5_MAX, Utility.ToString(DRgas["NC5_MAX"]));
                    //    Utility.SetCtrl(txtC6_MIN, Utility.ToString(DRgas["C6_MIN"])); Utility.SetCtrl(txtC6_MAX, Utility.ToString(DRgas["C6_MAX"]));
                    //    Utility.SetCtrl(txtN2_MIN, Utility.ToString(DRgas["N2_MIN"])); Utility.SetCtrl(txtN2_MAX, Utility.ToString(DRgas["N2_MAX"]));
                    //    Utility.SetCtrl(txtCO2_MIN, Utility.ToString(DRgas["CO2_MIN"])); Utility.SetCtrl(txtCO2_MAX, Utility.ToString(DRgas["CO2_MAX"]));
                    //    Utility.SetCtrl(txtH2S_MIN, Utility.ToString(DRgas["H2S_MIN"])); Utility.SetCtrl(txtH2S_MAX, Utility.ToString(DRgas["H2S_MAX"]));
                    //    Utility.SetCtrl(txtHG_MIN, Utility.ToString(DRgas["HG_MIN"])); Utility.SetCtrl(txtHG_MAX, Utility.ToString(DRgas["HG_MAX"]));

                    //}




                    if (Validation.GetCtrlStr(ddlISO_FLAG) == "Y")
                    {
                        pnlISO.Visible = true;
                    }
                    else
                    {
                        pnlISO.Visible = false;
                    }

                }
                else
                {
                    AddData();
                    Msg = "";
                    PageAction = "Result('N', LastPage);";
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
                Utility.ClearObject(ref DTrpt);
                Utility.ClearObject(ref DTgas);
            }
        }

        private void LoadComponentData()
        {
            DataTable DTgas = null;
            DataRow DRgas = null;
            try
            {

 
 

                    //==== Gas composition ==============================================================================
                    int cntS = Utility.ToInt(Project.dal.GetSQLValue("SELECT COUNT(*) FROM O_SITE_SGC WHERE  SITE_ID = " + Validation.GetCtrlInt(hidSITE_ID).ToString()));
                    if (cntS > 1)
                        btnFind.Visible = true;
                    else
                        btnFind.Visible = false;

                    if (Validation.GetCtrlStr(hidSTD_ID) != "")
                        DTgas = Project.dal.SearchSiteSgc(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString(), StdID: Validation.GetCtrlStr(hidSTD_ID));
                    else
                        DTgas = Project.dal.SearchSiteSgc(SiteID: Validation.GetCtrlInt(hidSITE_ID).ToString());


                    if (DTgas != null && DTgas.Rows.Count > 0)
                    {
                        DRgas = Utility.GetDR(ref DTgas);

                        Utility.SetCtrl(hidSGCSITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                        Utility.SetCtrl(hidSTD_ID, Utility.ToString(DRgas["STD_ID"])); //-- edit 19/09/2019 
                        Utility.SetCtrl(txtCYLINDER_NO, Utility.ToString(DRgas["CYLINDER_NO"]));
                        Utility.SetCtrl(hidORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"])); //-- edit 19/09/2019 
                        Utility.SetCtrl(datORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"]));
                        Utility.SetCtrl(datEXPIRE_DATE, Utility.AppFormatDate(DRgas["EXPIRE_DATE"]));
                        Utility.SetCtrl(txtC1, Utility.ToString(DRgas["C1"]), IsReadOnly: true);
                        Utility.SetCtrl(txtC2, Utility.ToString(DRgas["C2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtC3, Utility.ToString(DRgas["C3"]), IsReadOnly: true);
                        Utility.SetCtrl(txtIC4, Utility.ToString(DRgas["IC4"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNC4, Utility.ToString(DRgas["NC4"]), IsReadOnly: true);
                        Utility.SetCtrl(txtIC5, Utility.ToString(DRgas["IC5"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNC5, Utility.ToString(DRgas["NC5"]), IsReadOnly: true);
                        Utility.SetCtrl(txtC6, Utility.ToString(DRgas["C6"]), IsReadOnly: true);
                        Utility.SetCtrl(txtN2, Utility.ToString(DRgas["N2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtCO2, Utility.ToString(DRgas["CO2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtH2S, Utility.ToString(DRgas["H2S"]), IsReadOnly: true);
                        Utility.SetCtrl(txtHG, Utility.ToString(DRgas["HG"]), IsReadOnly: true);

                        //==== Gas composition ISO MIN/MAX==============================================================================
                        Utility.SetCtrl(hidTISISITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                        Utility.SetCtrl(txtC1_MIN, Utility.ToString(DRgas["C1_MIN"])); Utility.SetCtrl(txtC1_MAX, Utility.ToString(DRgas["C1_MAX"]));
                        Utility.SetCtrl(txtC2_MIN, Utility.ToString(DRgas["C2_MIN"])); Utility.SetCtrl(txtC2_MAX, Utility.ToString(DRgas["C2_MAX"]));
                        Utility.SetCtrl(txtC3_MIN, Utility.ToString(DRgas["C3_MIN"])); Utility.SetCtrl(txtC3_MAX, Utility.ToString(DRgas["C3_MAX"]));
                        Utility.SetCtrl(txtIC4_MIN, Utility.ToString(DRgas["IC4_MIN"])); Utility.SetCtrl(txtIC4_MAX, Utility.ToString(DRgas["IC4_MAX"]));
                        Utility.SetCtrl(txtNC4_MIN, Utility.ToString(DRgas["NC4_MIN"])); Utility.SetCtrl(txtNC4_MAX, Utility.ToString(DRgas["NC4_MAX"]));
                        Utility.SetCtrl(txtIC5_MIN, Utility.ToString(DRgas["IC5_MIN"])); Utility.SetCtrl(txtIC5_MAX, Utility.ToString(DRgas["IC5_MAX"]));
                        Utility.SetCtrl(txtNC5_MIN, Utility.ToString(DRgas["NC5_MIN"])); Utility.SetCtrl(txtNC5_MAX, Utility.ToString(DRgas["NC5_MAX"]));
                        Utility.SetCtrl(txtC6_MIN, Utility.ToString(DRgas["C6_MIN"])); Utility.SetCtrl(txtC6_MAX, Utility.ToString(DRgas["C6_MAX"]));
                        Utility.SetCtrl(txtN2_MIN, Utility.ToString(DRgas["N2_MIN"])); Utility.SetCtrl(txtN2_MAX, Utility.ToString(DRgas["N2_MAX"]));
                        Utility.SetCtrl(txtCO2_MIN, Utility.ToString(DRgas["CO2_MIN"])); Utility.SetCtrl(txtCO2_MAX, Utility.ToString(DRgas["CO2_MAX"]));
                        Utility.SetCtrl(txtH2S_MIN, Utility.ToString(DRgas["H2S_MIN"])); Utility.SetCtrl(txtH2S_MAX, Utility.ToString(DRgas["H2S_MAX"]));
                        Utility.SetCtrl(txtHG_MIN, Utility.ToString(DRgas["HG_MIN"])); Utility.SetCtrl(txtHG_MAX, Utility.ToString(DRgas["HG_MAX"]));

                    }

 
                 
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DTgas);
            }
        }

        private void SaveData()
        {
            DataTable DT = new DataTable();
            int OPs;
            String rFID = "", rSiteName = "", rRegionID = "";
            String rRptID = "", rNgbillRptNo = "", rGcRptNo = "", rGcRptName = "";
            try
            {
                Key = Utility.GetCtrl(hidSITE_ID);
                if (Key == "")
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }

                rFID = Validation.GetCtrlStr(txtFID).Trim();
                rSiteName = Validation.GetCtrlStr(txtSITE_NAME);
                rRegionID = Validation.GetCtrlStr(ddlREGION_ID);

                DT = Project.dal.SearchSiteFID("", rFID);
                if (DT.Rows.Count > 0 && OPs == DBUTIL.opINSERT)
                {
                    Msg = "Site already exist!";
                }
                else
                {
                    //-- Add by Turk 18/04/2562 --> MngSiteFID (ddlH2S_NAME1, ddlH2S_NAME2, ddlHG_NAME1, ddlHG_NAME2, ddlO2_NAME1, ddlO2_NAME2, ddlHC_NAME1, ddlHC_NAME2) --
                    //-- EDIT 28/06/2023 --
                    Project.dal.MngSiteFID(OPs, ref Key, rFID, rSiteName, rRegionID, Validation.GetCtrlStr(ddlALERT_FLAG), Validation.GetCtrlStr(ddlISO_FLAG), Validation.GetCtrlStr(ddlH2S_FLAG), Validation.GetCtrlStr(ddlOMA_NAME1), Validation.GetCtrlStr(ddlOMA_NAME2), Validation.GetCtrlStr(ddlFLOW_NAME1), Validation.GetCtrlStr(ddlFLOW_NAME2), Validation.GetCtrlStr(txtTOTAL_RUN), Validation.GetCtrlStr(txtTOLERANCE_RUN), Validation.GetCtrlStr(ddlOGC_NAME1), Validation.GetCtrlStr(ddlOGC_NAME2), Validation.GetCtrlStr(ddlOGC_NAME3), Validation.GetCtrlStr(ddlH2S_NAME1), Validation.GetCtrlStr(ddlH2S_NAME2), Validation.GetCtrlStr(ddlHG_NAME1), Validation.GetCtrlStr(ddlHG_NAME2), Validation.GetCtrlStr(ddlO2_NAME1), Validation.GetCtrlStr(ddlO2_NAME2), Validation.GetCtrlStr(ddlHC_NAME1), Validation.GetCtrlStr(ddlHC_NAME2), Validation.GetCtrlStr(ddlANLMET_ID)); 
                    Utility.SetCtrl(hidSITE_ID, Key);

                    //==== Reports ==============================================================================
                    //-- DAILY ------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID1);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO1).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO1).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME1).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE1, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID1, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID1, rRptID);
                        }
                    }

                    //-- DAILY27 ------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID4);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO4).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO4).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME4).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE4, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID4, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID4, rRptID);
                        }
                    }

                    //-- 27DAY ---------------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID2);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO2).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO2).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME2).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE2, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID2, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID2, rRptID);
                        }
                    }


                    //-- ENDMTH --------------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID3);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO3).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO3).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME3).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE3, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID3, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID3, rRptID);
                        }
                    }

                    //-- edit 29/11/2019  เพิ่ม 20Day
                    //-- DAILY20 ------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID5);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO5).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO5).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME5).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE5, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID5, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID5, rRptID);
                        }
                    }

                    //-- 20DAY ---------------------------------------------------------------------------------------
                    rRptID = Utility.GetCtrl(hidRPT_ID6);
                    rNgbillRptNo = Validation.GetCtrlStr(txtNGBILL_RPT_NO6).Trim();
                    rGcRptNo = Validation.GetCtrlStr(txtGC_RPT_NO6).Trim();
                    rGcRptName = Validation.GetCtrlStr(txtGC_RPT_NAME6).Trim();
                    if (rRptID == "")
                    {
                        OPs = DBUTIL.opINSERT;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ไม่ต้องบันทึก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = -99;
                        }
                    }
                    else
                    {
                        OPs = DBUTIL.opUPDATE;
                        //--- ถ้าไม่มีข้อมูล บรรทัดนั้น ก็ให้ลบข้อมูลออก
                        if (rNgbillRptNo == "" && rGcRptNo == "" && rGcRptName == "")
                        {
                            OPs = DBUTIL.opDELETE;
                        }
                    }
                    if (OPs != -99)
                    {
                        Project.dal.MngSiteReport(OPs, ref rRptID, Key, rFID, Project.lkRPT_TYPE6, rNgbillRptNo, rGcRptNo, rGcRptName);
                        if (OPs == DBUTIL.opDELETE)
                        {
                            Utility.SetCtrl(hidRPT_ID6, "");
                        }
                        else
                        {
                            Utility.SetCtrl(hidRPT_ID6, rRptID);
                        }
                    }




                    //==== Gas composition ==============================================================================
                    if (pnlISO.Visible)
                    {
                        if (Utility.GetCtrl(hidSGCSITE_ID) == "")
                        {
                            OPs = DBUTIL.opINSERT;
                        }
                        else
                        {
                            //-- edit 19/09/2019 --- เก็บ standard gas history ด้วย
                            //-- ถ้า order date ไม่เหมือนเดิม ให้เพิ่มใหม่
                            if (Validation.GetCtrlStr(datORDER_DATE) != Validation.GetCtrlStr(hidORDER_DATE))
                            {
                                OPs = DBUTIL.opINSERT;
                            }
                            else
                            {
                                OPs = DBUTIL.opUPDATE;
                            }

                        }
                        string STDid = Validation.GetCtrlStr(hidSTD_ID);
                        //-- edit 27/08/2020 -- standard C1,... ให้เอาจากระบบ OGC Data ซึ่งจะบันทึกข้อมูลลง table ให้เลย
                        //Project.dal.MngSiteSgc(OPs, Key, ref STDid, Validation.GetCtrlStr(txtCYLINDER_NO), Validation.GetCtrlStr(datORDER_DATE), Validation.GetCtrlStr(datEXPIRE_DATE), Validation.GetCtrlStr(txtC1), Validation.GetCtrlStr(txtC2), Validation.GetCtrlStr(txtC3), Validation.GetCtrlStr(txtIC4), Validation.GetCtrlStr(txtNC4), Validation.GetCtrlStr(txtIC5), Validation.GetCtrlStr(txtNC5), Validation.GetCtrlStr(txtC6), Validation.GetCtrlStr(txtN2), Validation.GetCtrlStr(txtCO2), Validation.GetCtrlStr(txtH2S), Validation.GetCtrlStr(txtHG));
                        Project.dal.MngSiteSgc(OPs, Key, ref STDid, Validation.GetCtrlStr(txtCYLINDER_NO), Validation.GetCtrlStr(datORDER_DATE), Validation.GetCtrlStr(datEXPIRE_DATE));

                        Utility.SetCtrl(hidSGCSITE_ID, Key);
                        Utility.SetCtrl(hidSTD_ID, STDid);

                        //-- edit 21/10/2020 -- ตรวจสอบว่ามีข้อมูลอยู่หรือเปล่า
                        string CNT = Project.dal.GetSQLValue("SELECT COUNT(*) FROM O_SITE_TISI WHERE SITE_ID=" + Key + " AND STD_ID=" + STDid);
                        if (CNT == "0" )
                        {
                            OPs = DBUTIL.opINSERT;
                        }
                        else
                        {
                            OPs = DBUTIL.opUPDATE;
                        }

                        Project.dal.MngSiteTisi(OPs, Key, STDid, Validation.GetCtrlStr(txtC1_MIN), Validation.GetCtrlStr(txtC2_MIN), Validation.GetCtrlStr(txtC3_MIN), Validation.GetCtrlStr(txtIC4_MIN), Validation.GetCtrlStr(txtNC4_MIN), Validation.GetCtrlStr(txtIC5_MIN), Validation.GetCtrlStr(txtNC5_MIN), Validation.GetCtrlStr(txtC6_MIN), Validation.GetCtrlStr(txtN2_MIN), Validation.GetCtrlStr(txtCO2_MIN), Validation.GetCtrlStr(txtH2S_MIN), Validation.GetCtrlStr(txtHG_MIN), Validation.GetCtrlStr(txtC1_MAX), Validation.GetCtrlStr(txtC2_MAX), Validation.GetCtrlStr(txtC3_MAX), Validation.GetCtrlStr(txtIC4_MAX), Validation.GetCtrlStr(txtNC4_MAX), Validation.GetCtrlStr(txtIC5_MAX), Validation.GetCtrlStr(txtNC5_MAX), Validation.GetCtrlStr(txtC6_MAX), Validation.GetCtrlStr(txtN2_MAX), Validation.GetCtrlStr(txtCO2_MAX), Validation.GetCtrlStr(txtH2S_MAX), Validation.GetCtrlStr(txtHG_MAX));
                        Utility.SetCtrl(hidTISISITE_ID, Key);
                    }
                    else
                    { //-- delete 
                        string tmp = "";
                        Project.dal.MngSiteSgc(DBUTIL.opDELETE, Key, ref tmp);
                        Utility.SetCtrl(hidSGCSITE_ID, "");
                        Utility.SetCtrl(hidSTD_ID, "");

                        Project.dal.MngSiteTisi(DBUTIL.opDELETE, Key, "");
                        Utility.SetCtrl(hidTISISITE_ID, "");
                    }


                }
                if (Msg == "")
                {

                    LoadData();
                    Msg = ""; PageAction = "Result('S','');";
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

        protected void ddlISO_FLAG_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ( Utility.GetCtrl(ddlISO_FLAG) == "N")
                {
                    pnlISO.Visible = false;
                }
                else
                {
                    pnlISO.Visible = true;
                    //-- edit 21/10/2020 --  

                    string siteID = Validation.GetCtrlInt(hidSITE_ID).ToString();
                    if (siteID != "" && siteID != "0")
                    {
                        string SQL = "";
                        //-- update ค่า standard ที่ระบบ OGC verification ด้วย (table O_SITE_SGC)
                        SQL = "  MERGE INTO O_SITE_SGC O " +
                            " USING ( SELECT rownum rw, CS.*, CF.OSITE_ID FROM C_SITE_STD CS INNER JOIN C_SITE_FID CF ON CS.CSITE_ID=CF.CSITE_ID " +
                            "               WHERE CF.OSITE_ID=" + siteID + ") C " +
                            " ON (O.SITE_ID=C.OSITE_ID AND ((O.ORDER_DATE=C.ORDER_DATE and O.EXPIRE_DATE=C.EXPIRE_DATE) OR O.ORDER_DATE IS NULL ) ) " +
                            " WHEN MATCHED THEN " +
                            "  UPDATE SET O.CYLINDER_NO=C.CYLINDER_NO,O.ORDER_DATE=C.ORDER_DATE,O.EXPIRE_DATE=C.EXPIRE_DATE, " +
                            "  O.C1=to_char(C.C1,'90.9999'), O.C2=to_char(C.C2,'90.9999'), O.C3=to_char(C.C3,'90.9999'), O.IC4=to_char(C.IC4,'90.9999'), O.NC4=to_char(C.NC4,'90.9999'), O.IC5=to_char(C.IC5,'90.9999'), O.NC5=to_char(C.NC5,'90.9999'), " +
                            "  O.C6=to_char(C.C6,'90.9999'), O.N2=to_char(C.N2,'90.9999'), O.CO2=to_char(C.CO2,'90.9999'), O.H2S=to_char(C.H2S,'90.9999'), O.HG=to_char(C.HG,'90.9999'),O.MODIFIED_BY=C.MODIFIED_BY,O.MODIFIED_DATE=C.MODIFIED_DATE " +
                            " WHEN NOT MATCHED THEN " +
                            "  INSERT (SITE_ID,CYLINDER_NO,ORDER_DATE,EXPIRE_DATE,STD_ID, " +
                            "C1,C2,C3,IC4,NC4,IC5,NC5,C6,N2,CO2,H2S,HG, CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE) " +
                            "VALUES " +
                            "(C.OSITE_ID,C.CYLINDER_NO,C.ORDER_DATE,C.EXPIRE_DATE,  (SELECT NVL(MAX(STD_ID),1)+rw-1 FROM O_SITE_SGC WHERE SITE_ID=C.OSITE_ID) , " +
                            "to_char(C.C1,'90.9999'),to_char(C.C2,'90.9999'),to_char(C.C3,'90.9999'),to_char(C.IC4,'90.9999'),to_char(C.NC4,'90.9999'),to_char(C.IC5,'90.9999'),to_char(C.NC5,'90.9999')," +
                            "to_char(C.C6,'90.9999'),to_char(C.N2,'90.9999'),to_char(C.CO2,'90.9999'),to_char(C.H2S,'90.9999'),to_char(C.HG,'90.9999'),C.CREATED_BY,C.CREATED_DATE,C.MODIFIED_BY,C.MODIFIED_DATE)";
                        Project.dal.ExecuteSQL(SQL);

                        LoadComponentData();
                    }

                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void DeleteData()
        {
            try
            {
                Key = Utility.GetCtrl(hidSITE_ID);
                if (Key == "")
                {
                    Msg = ""; PageAction = "Result('C');";
                }
                else
                {
                    //--- ก่อนลบให้ดูว่ามีข้อมูลใน NGBILL_DAILY_UPDATE หรือไม่
                    String SQL = "SELECT COUNT(*) FROM NGBILL_DAILY_UPDATE WHERE FID = '" + Utility.GetCtrl(txtFID).Trim() + "' ";
                    if (Project.dal.GetSQLValue(SQL) == "0")
                    {
                        Project.dal.MngSiteFID(DBUTIL.opDELETE, ref Key);
                        Msg = ""; PageAction = "Result('D2', LastPage);";
                    }
                    else
                    {
                        Msg = "Can not delete this site because it being use by NGBILL";
                    }

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }


        //-- EDIT 19/09/2019 ---
        private void LoadSTDData()
        {
            DataTable DT = null;
            DataRow DR = null;
            DataTable DTgas = null;
            DataRow DRgas = null;
            string siteid = "";
            string STDid = "";

            try
            {
                siteid = Validation.GetCtrlInt(hidSITE_ID).ToString();
                STDid = Validation.GetCtrlStr(hidSELECT_STD);

                if (siteid != "" && STDid != "")
                {
                    //==== Gas composition ==============================================================================
                    DTgas = Project.dal.SearchSiteSgc(SiteID: siteid, StdID: STDid);
                    if (DTgas != null && DTgas.Rows.Count > 0)
                    {
                        DRgas = Utility.GetDR(ref DTgas);

                        Utility.SetCtrl(hidSGCSITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                        Utility.SetCtrl(hidSTD_ID, Utility.ToString(DRgas["STD_ID"]));//-- edit 19/09/2019 
                        Utility.SetCtrl(txtCYLINDER_NO, Utility.ToString(DRgas["CYLINDER_NO"]));
                        Utility.SetCtrl(hidORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"])); //-- edit 19/09/2019 
                        Utility.SetCtrl(datORDER_DATE, Utility.AppFormatDate(DRgas["ORDER_DATE"]));
                        Utility.SetCtrl(datEXPIRE_DATE, Utility.AppFormatDate(DRgas["EXPIRE_DATE"]));
                        Utility.SetCtrl(txtC1, Utility.ToString(DRgas["C1"]),IsReadOnly: true);
                        Utility.SetCtrl(txtC2, Utility.ToString(DRgas["C2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtC3, Utility.ToString(DRgas["C3"]), IsReadOnly: true);
                        Utility.SetCtrl(txtIC4, Utility.ToString(DRgas["IC4"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNC4, Utility.ToString(DRgas["NC4"]), IsReadOnly: true);
                        Utility.SetCtrl(txtIC5, Utility.ToString(DRgas["IC5"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNC5, Utility.ToString(DRgas["NC5"]), IsReadOnly: true);
                        Utility.SetCtrl(txtC6, Utility.ToString(DRgas["C6"]), IsReadOnly: true);
                        Utility.SetCtrl(txtN2, Utility.ToString(DRgas["N2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtCO2, Utility.ToString(DRgas["CO2"]), IsReadOnly: true);
                        Utility.SetCtrl(txtH2S, Utility.ToString(DRgas["H2S"]), IsReadOnly: true);
                        Utility.SetCtrl(txtHG, Utility.ToString(DRgas["HG"]), IsReadOnly: true);

                        //==== Gas composition ISO MIN/MAX==============================================================================
                        Utility.SetCtrl(hidTISISITE_ID, Utility.ToString(DRgas["SITE_ID"]));
                        Utility.SetCtrl(txtC1_MIN, Utility.ToString(DRgas["C1_MIN"])); Utility.SetCtrl(txtC1_MAX, Utility.ToString(DRgas["C1_MAX"]));
                        Utility.SetCtrl(txtC2_MIN, Utility.ToString(DRgas["C2_MIN"])); Utility.SetCtrl(txtC2_MAX, Utility.ToString(DRgas["C2_MAX"]));
                        Utility.SetCtrl(txtC3_MIN, Utility.ToString(DRgas["C3_MIN"])); Utility.SetCtrl(txtC3_MAX, Utility.ToString(DRgas["C3_MAX"]));
                        Utility.SetCtrl(txtIC4_MIN, Utility.ToString(DRgas["IC4_MIN"])); Utility.SetCtrl(txtIC4_MAX, Utility.ToString(DRgas["IC4_MAX"]));
                        Utility.SetCtrl(txtNC4_MIN, Utility.ToString(DRgas["NC4_MIN"])); Utility.SetCtrl(txtNC4_MAX, Utility.ToString(DRgas["NC4_MAX"]));
                        Utility.SetCtrl(txtIC5_MIN, Utility.ToString(DRgas["IC5_MIN"])); Utility.SetCtrl(txtIC5_MAX, Utility.ToString(DRgas["IC5_MAX"]));
                        Utility.SetCtrl(txtNC5_MIN, Utility.ToString(DRgas["NC5_MIN"])); Utility.SetCtrl(txtNC5_MAX, Utility.ToString(DRgas["NC5_MAX"]));
                        Utility.SetCtrl(txtC6_MIN, Utility.ToString(DRgas["C6_MIN"])); Utility.SetCtrl(txtC6_MAX, Utility.ToString(DRgas["C6_MAX"]));
                        Utility.SetCtrl(txtN2_MIN, Utility.ToString(DRgas["N2_MIN"])); Utility.SetCtrl(txtN2_MAX, Utility.ToString(DRgas["N2_MAX"]));
                        Utility.SetCtrl(txtCO2_MIN, Utility.ToString(DRgas["CO2_MIN"])); Utility.SetCtrl(txtCO2_MAX, Utility.ToString(DRgas["CO2_MAX"]));
                        Utility.SetCtrl(txtH2S_MIN, Utility.ToString(DRgas["H2S_MIN"])); Utility.SetCtrl(txtH2S_MAX, Utility.ToString(DRgas["H2S_MAX"]));
                        Utility.SetCtrl(txtHG_MIN, Utility.ToString(DRgas["HG_MIN"])); Utility.SetCtrl(txtHG_MAX, Utility.ToString(DRgas["HG_MAX"]));

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
                Utility.ClearObject(ref DTgas);
            }
        }



    }
}