using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- EDIT 21/07/2023 --
    public partial class MngCustomerDetail : System.Web.UI.Page
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
                Security.CheckRole(Security.TaskMDCustomer, true);

                SetCtrl();
                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();

                    Key = Validation.GetParamStr("K", IsEncoded: true);
                    if ( Key != "")
                    {
                        Utility.SetCtrl(hidPERMANENT_CODE, Key);
                        ServerAction = "LOAD";
                    }
                    else
                    {
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
                canEdit = Security.CanDo(Security.TaskMDCustomer, Security.actAdd);
                canDelete = canEdit;
                canAdd = canEdit;

                //-- EDIT 04/08/2023 -- ไม่ต้องเพิ่ม-ลบ เพราะดึงข้อมูลจากระบบ GIS
                // pnlDELETE.Visible = (canDelete) ? true : false;
                pnlDELETE.Visible = false;

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
                //-- OGC FID --
                DT = Project.dal.SearchSiteFID(orderSQL: "FID");
                Utility.LoadList(ref ddlQUALITY_MAIN, DT, "FID_NAME", "FID", true, "");  
                Utility.LoadList(ref ddlQUALITY_SUPPORT1, DT, "FID_NAME", "FID", false, ""); //เพิ่ม blank line จาก บรรทัดบนแล้ว
                Utility.LoadList(ref ddlQUALITY_SUPPORT2, DT, "FID_NAME", "FID", false, "");

                //-- OMA Moisture ---
                DT = Project.dalhs.SearchDimMoisture();
                Utility.LoadList(ref ddlOMA_MAIN, DT, "NAME_TAGNAME", "NAME", true, "");
                Utility.LoadList(ref ddlOMA_SUPPORT1, DT, "NAME_TAGNAME", "NAME", false, ""); //เพิ่ม blank line จาก บรรทัดบนแล้ว

                //-- H2S --
                DT = Project.dal.SearchDimH2S();
                Utility.LoadList(ref ddlH2S, DT, "H2S_NAME", "NAME", true, "");

                //-- HG --
                DT = Project.dal.SearchDimHG();
                Utility.LoadList(ref ddlHG, DT, "HG_NAME", "NAME", true, "");


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
                Utility.SetCtrl(hidPERMANENT_CODE, "");
                Utility.SetCtrl(txtBV_VALVE, "");
                Utility.SetCtrl(txtBV_ZONE, "");
                Utility.SetCtrl(txtPERMANENT_CODE, "");
                Utility.SetCtrl(txtNAME_ABBR , "");
                Utility.SetCtrl(txtNAME_FULL, "");
                Utility.SetCtrl(txtSUB_TYPE, "");
                Utility.SetCtrl(txtSTATUS_CL, "");
                Utility.SetCtrl(ddlH2S, "");
                Utility.SetCtrl(ddlHG, "");
                Utility.SetCtrl(ddlOMA_MAIN, "");
                Utility.SetCtrl(ddlOMA_SUPPORT1, "");
                Utility.SetCtrl(ddlQUALITY_MAIN, "");
                Utility.SetCtrl(ddlQUALITY_SUPPORT1, "");
                Utility.SetCtrl(ddlQUALITY_SUPPORT2, "");
                Utility.SetCtrl(txtREGION, "");
                Utility.SetCtrl(txtREMARK, "");

                Utility.SetCtrl(lblLastUpdated, "");

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

            try
            {
                if (Validation.GetCtrlStr(hidPERMANENT_CODE) != "" )
                {
                    DT = Project.dal.SearchCustomer(Validation.GetCtrlStr(hidPERMANENT_CODE));
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        DR = Utility.GetDR(ref DT);

                        Utility.SetCtrl(txtPERMANENT_CODE, Utility.ToString(DR["PERMANENT_CODE"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNAME_ABBR, Utility.ToString(DR["NAME_ABBR"]), IsReadOnly: true);
                        Utility.SetCtrl(txtNAME_FULL, Utility.ToString(DR["NAME_FULL"]), IsReadOnly: true);
                        Utility.SetCtrl(txtSUB_TYPE , Utility.ToString(DR["SUB_TYPE"]), IsReadOnly: true);
                        Utility.SetCtrl(txtREGION, Utility.ToString(DR["REGION"]), IsReadOnly: true);
                        Utility.SetCtrl(txtBV_VALVE, Utility.ToString(DR["BV_VALVE"]), IsReadOnly: true);
                        Utility.SetCtrl(txtBV_ZONE, Utility.ToString(DR["BV_ZONE"]), IsReadOnly: true);
                        Utility.SetCtrl(txtSTATUS_CL, Utility.ToString(DR["STATUS_CL"]), IsReadOnly: true);

                        Utility.SetCtrl(ddlH2S, Utility.ToString(DR["H2S"]));
                        Utility.SetCtrl(ddlHG, Utility.ToString(DR["HG"]));
                        Utility.SetCtrl(ddlOMA_MAIN, Utility.ToString(DR["OMA_MAIN"]));
                        Utility.SetCtrl(ddlOMA_SUPPORT1, Utility.ToString(DR["OMA_SUPPORT1"]));
                        Utility.SetCtrl(ddlQUALITY_MAIN, Utility.ToString(DR["QUALITY_MAIN"]));
                        Utility.SetCtrl(ddlQUALITY_SUPPORT1, Utility.ToString(DR["QUALITY_SUPPORT1"]));
                        Utility.SetCtrl(ddlQUALITY_SUPPORT2, Utility.ToString(DR["QUALITY_SUPPORT2"]));
                        Utility.SetCtrl(txtREMARK , Utility.ToString(DR["REMARK"]));

                        Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);


                    }
                    else
                    {
                        Msg = "";
                        PageAction = "Result('N', LastPage);";
                    }
                }
                else
                {
                    AddData();
                    Msg = "";
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

        private void SaveData()
        {
            DataTable DT = new DataTable();
           // int OPs;
            try
            {
               

               
                String custID = Validation.GetCtrlStr(txtPERMANENT_CODE).Trim();
                //String custShort = Validation.GetCtrlStr(txtNAME_ABBR).Trim();
                //String custTname = Validation.GetCtrlStr(txtNAME_FULL).Trim();
                //String custType = Validation.GetCtrlStr(txtSUB_TYPE).Trim();
                //String region = Validation.GetCtrlStr(txtREGION).Trim();
                //String bvValve = Validation.GetCtrlStr(txtBV_VALVE).Trim();
                //String bvZone = Validation.GetCtrlStr(txtBV_ZONE).Trim();
       

                String h2s = Validation.GetCtrlStr(ddlH2S).Trim();
                String hg = Validation.GetCtrlStr(ddlHG).Trim();
                String omaMain = Validation.GetCtrlStr(ddlOMA_MAIN).Trim();
                String omaSup1 = Validation.GetCtrlStr(ddlOMA_SUPPORT1).Trim();
                String qtyMain = Validation.GetCtrlStr(ddlQUALITY_MAIN).Trim();
                String qtySup1 = Validation.GetCtrlStr(ddlQUALITY_SUPPORT1).Trim();
                String qtySup2 = Validation.GetCtrlStr(ddlQUALITY_SUPPORT2).Trim();
                String remark = Validation.GetCtrlStr(txtREMARK).Trim();

                //-- EDIT 04/08/2023 -- ไม่ต้องเพิ่ม-ลบ เพราะดึงข้อมูลจากระบบ GIS
                //Key = Utility.GetCtrl(hidPERMANENT_CODE);
                //if (Key == "")
                //{
                //    OPs = DBUTIL.opINSERT;
                //}
                //else
                //{
                //    OPs = DBUTIL.opUPDATE;
                //}

                //if (OPs == DBUTIL.opINSERT)
                //{
                //    DT = Project.dal.SearchCustomer(custID);
                //    if (DT.Rows.Count > 0 )
                //    {
                //        Msg = "Customer ID already exist!";
                //    }
                //}

                Key = Utility.GetCtrl(hidPERMANENT_CODE);

                if (Key != "")
                {
     
                   // Project.dal.MngCustomerTSO(OPs, Key, custShort: custShort, custTname: custTname, custType: custType, region: region, bvZone: bvZone, bvValve: bvValve);
                   // Utility.SetCtrl(hidPERMANENT_CODE, Key);
                    Project.dal.MngCustomerGQMS(DBUTIL.opUPDATE, Key, QualityMain: qtyMain, QualitySupport1: qtySup1, QualitySupport2: qtySup2, omaMain: omaMain, omaSupport1: omaSup1, h2s: h2s, hg: hg, remark: remark);

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

        //-- EDIT 28/05/2024 --
        protected void ddlQUALITY_MAIN_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable DT = null;
            DataRow DR = null;

            try
            {
                //-- clear data --
                Utility.SetCtrl(ddlQUALITY_SUPPORT1, "");
                Utility.SetCtrl(ddlQUALITY_SUPPORT2, "");
                Utility.SetCtrl(ddlOMA_MAIN, "");
                Utility.SetCtrl(ddlOMA_SUPPORT1, "");
                Utility.SetCtrl(ddlH2S, "");
                Utility.SetCtrl(ddlHG, "");


                if (Validation.GetCtrlStr(ddlQUALITY_MAIN) != "")
                {
                    DT = Project.dal.SearchSiteFID(FID: Validation.GetCtrlStr(ddlQUALITY_MAIN));
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        DR = Utility.GetDR(ref DT);
                        if (Utility.ToString(DR["OGC_NAME1"]) == "LAST_GOOD")
                        {
                            Utility.SetCtrl(ddlQUALITY_SUPPORT1, Utility.ToString(DR["OGC_NAME2"]));
                            Utility.SetCtrl(ddlQUALITY_SUPPORT2, Utility.ToString(DR["OGC_NAME3"]));
                        }
                        else
                        {
                            Utility.SetCtrl(ddlQUALITY_SUPPORT1, Utility.ToString(DR["OGC_NAME1"]));
                            Utility.SetCtrl(ddlQUALITY_SUPPORT2, Utility.ToString(DR["OGC_NAME2"]));
                        }

                        Utility.SetCtrl(ddlOMA_MAIN, Utility.ToString(DR["OMA_NAME1"]));
                        Utility.SetCtrl(ddlOMA_SUPPORT1, Utility.ToString(DR["OMA_NAME2"]));
                        Utility.SetCtrl(ddlH2S, Utility.ToString(DR["H2S_NAME1"]));
                        Utility.SetCtrl(ddlHG, Utility.ToString(DR["HG_NAME1"]));

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

        private void DeleteData()
        {
            try
            {
                Key = Utility.GetCtrl(hidPERMANENT_CODE);
                if (Key == "")
                {
                    Msg = ""; PageAction = "Result('C');";
                }
                else
                {

                    Project.dal.MngCustomerGIS(DBUTIL.opDELETE, Key);
                    Project.dal.MngCustomerGQMS(DBUTIL.opDELETE, Key);

                    Msg = ""; PageAction = "Result('D2', LastPage);";

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

    }
}