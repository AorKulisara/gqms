using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- EDIT 26/06/2023 ---

    public partial class MngAnalysisDetail : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public String Key;
        public int ItemIndex;

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
                        Utility.SetCtrl(hidANLMET_ID, Key);
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
                    ItemIndex = Validation.GetParamInt("ItemIndex");
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

                    case "EDIT_ITEM":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = ItemIndex; BindData();
                        gvData.FooterRow.Visible = false; break;
                    case "ADD_ITEM": DoItemCommand(ServerAction);
                        LoadData();
                        break;
                    case "SAVE_ITEM":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = -1;
                        LoadData();
                        if (canAdd) gvData.FooterRow.Visible = true;
                        break;
                    case "CANCEL_ITEM":
                        gvData.EditIndex = -1; BindData();
                        if (canAdd) gvData.FooterRow.Visible = true;
                        break;
                    case "DELETE_ITEM": DoItemCommand(ServerAction, ItemIndex); LoadData(); break;


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

                if (canEdit || canDelete)
                {
                    gvData.Columns[3].Visible = true;
                }
                else
                {
                    gvData.Columns[3].Visible = false;
                }
                if (canAdd)
                {
                    gvData.ShowFooter = true;
                }
                else
                {
                    gvData.ShowFooter = false;
                }

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
                Utility.SetCtrl(hidANLMET_ID, "");
                Utility.SetCtrl(txtANLMET_NAME, "");

                Utility.SetCtrl(lblLastUpdated, "");
                gvData.Visible = false;

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
            DataTable DTi = null;
            String anlmetID = "";
            try
            {

                anlmetID = Validation.GetCtrlInt(hidANLMET_ID).ToString();
                if (anlmetID != "")
                {
                    DT = Project.dal.SearchDimAnalysisMethod(anlmetID);
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        DR = Utility.GetDR(ref DT);
                        Utility.SetCtrl(txtANLMET_NAME, Utility.ToString(DR["ANLMET_NAME"]));

                        Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);

                        //-- กรณียังไม่มี analysis item ให้ใช้ defualt ---
                        DTi = Project.dal.SearchDimAnalysisItem(anlmetID);

                        Session["DT"] = DTi;
                        BindData();
                    }
                    else
                    {
                        AddData();
                        Msg = "";
                        PageAction = "Result('N', LastPage);";
                    }
                }
                else
                {
                    AddData();
                }
               
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
                Utility.ClearObject(ref DTi);
            }
        }

        private void SaveData()
        {
            DataTable DT = new DataTable();
            int OPs;
            String rAnlmetName = "";
            try
            {
                Key = Utility.GetCtrl(hidANLMET_ID);
                if (Key == "")
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }
                rAnlmetName = Validation.GetCtrlStr(txtANLMET_NAME).Trim();

                Project.dal.MngDimAnalysisMethod(OPs, ref Key, rAnlmetName);
                Utility.SetCtrl(hidANLMET_ID, Key);

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


        private void DeleteData()
        {
            try
            {
                Key = Utility.GetCtrl(hidANLMET_ID);
                if (Key == "")
                {
                    Msg = ""; PageAction = "Result('C');";
                }
                else
                {
                    Project.dal.MngDimAnalysisMethod(DBUTIL.opDELETE, ref Key, "");
                    Project.dal.MngDimAnalysisItem(DBUTIL.opDELETE, Key, "", "", "", "");
                    Msg = ""; PageAction = "Result('D2', LastPage);";
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        //========================================

        private void BindData()
        {
            DataTable DT = (DataTable)Session["DT"];
            Utility.BindGVData(ref gvData, Session["DT"], (DT.Rows.Count == 0));
            gvData.Visible = true;

            if (canEdit || canDelete)
            {
                    gvData.Columns[3].Visible = true;
            }

            if (canAdd)
            {
                    gvData.FooterRow.Visible = true;
            }

        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {



            }
            catch (Exception ex)
            {

            }

        }


        private void DoItemCommand(string action, int ItemIndex = 0)
        {
            DataTable DT = (DataTable)Session["DT"];
            string anlmetID = "", seqNo = "";
            try
            {
                anlmetID = Validation.GetCtrlInt(hidANLMET_ID).ToString();

                if (action != "ADD_ITEM")
                {
                    seqNo = DT.Rows[ItemIndex]["SEQ_NO"].ToString();
                }
                else
                {
                    seqNo = Validation.ValidateStr(Request.Form["txtSEQ_NO"]);
                }

                if (anlmetID != "" && seqNo != "")
                {
                    switch (action)
                    {
                        case "ADD_ITEM":
                            DT = Project.dal.SearchDimAnalysisItem(anlmetID, seqNo);
                            if (DT.Rows.Count != 0)
                            {
                                Msg = "Duplicated Sequence, Please verify the correctness ";
                            }
                            else
                            {
                                Project.dal.MngDimAnalysisItem(DBUTIL.opINSERT, anlmetID, seqNo, seqNo, Validation.ValidateStr(Request.Form["txtSTD_HEAD"]), Validation.ValidateStr(Request.Form["txtSTD_REF"]));
                                Msg = "Save completed.";
                            }
                            break;
                        case "SAVE_ITEM":
                            Msg = "";
                            string seqNoEdit = Validation.ValidateStr(Utility.ToNum(Request.Form["txtSEQ_NOEdit"]).ToString());
                            if (seqNo != seqNoEdit )
                            {
                                DT = Project.dal.SearchDimAnalysisItem(anlmetID, seqNoEdit);
                                if (DT.Rows.Count != 0)
                                {
                                    Msg = "Duplicated Sequence, Please verify the correctness ";
                                }
                            }
                            if (Msg == "")
                            {
                                Project.dal.MngDimAnalysisItem(DBUTIL.opUPDATE, anlmetID, seqNo, seqNoEdit, Validation.ValidateStr(Request.Form["txtSTD_HEADEdit"]), Validation.ValidateStr(Request.Form["txtSTD_REFEdit"]));
                                Msg = "Save completed.";
                            }

                            break;
                        case "EDIT_ITEM": break;
                        case "DELETE_ITEM":
                            Project.dal.MngDimAnalysisItem(DBUTIL.opDELETE, anlmetID, seqNo, "","","");
                            Msg = "Delete completed.";                           
                            break;
                        case "CANCEL_ITEM": break;
                    }
                }
                else
                {
                    Msg = "Invalid Sequence.";
                }


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



    }
}