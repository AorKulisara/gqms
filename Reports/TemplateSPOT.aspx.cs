using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- EDIT 27/06/2019 ---
    public partial class TemplateSPOT : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public String Key;
        public int ItemIndex;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskRptSite, true);

                if (!this.IsPostBack)
                {
                    Key = Validation.GetParamStr("K", IsEncoded: true);
                    if (Utility.IsNumeric(Key))
                    {
                        ServerAction = "LOAD";
                        Utility.SetCtrl(hidTID, Key);
                    }
                    else
                    {

                        ServerAction = "ADD";
                        Utility.SetCtrl(hidTID, "");
                    }
                }
                else
                {
                    Key = Utility.GetCtrl(hidTID);
                    ServerAction = Validation.GetParamStr("ServerAction");
                    ItemIndex = Validation.GetParamInt("ItemIndex");
                }



                switch (ServerAction)
                {
                    case "ADD": AddData(); break;
                    case "LOAD": LoadData();
                        break;
                    case "SAVE": SaveData(); break;
                    case "DELETE": DeleteData(); break;
                    case "EDIT_ITEM":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = ItemIndex; BindData();
                        gvData.FooterRow.Visible = false; break;
                    case "ADD_ITEM": DoItemCommand(ServerAction); LoadData(); break;
                    case "SAVE_ITEM":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = -1; LoadData();
                        gvData.FooterRow.Visible = true;
                        break;
                    case "CANCEL_ITEM":
                        gvData.EditIndex = -1; BindData();
                        gvData.FooterRow.Visible = true;
                        break;
                    case "DELETE_ITEM": DoItemCommand(ServerAction, ItemIndex); LoadData(); break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }




        private void AddData()
        {
            DataTable DT = null;
            try
            {

                Utility.SetCtrl(txtName, "");
                Utility.SetCtrl(hidTID, "");

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
            DataRow DR = null;
            DataTable DT = null;
            DataTable DT1 = null;
            try
            {
                if (Key == "") { Key = Utility.GetCtrl(hidTID); }
                DT = Project.dal.SearchRptFidTemplate("3",Key, "");
                if (DT != null && DT.Rows.Count > 0)
                {
                    DR = Utility.GetDR(ref DT);
                    Utility.SetCtrl(hidTID, Utility.ToString(DR["TID"]));
                    Utility.SetCtrl(txtName, Utility.ToString(DR["T_NAME"]));

                    DT1 = Project.dal.SearchRptFidDetail(Key, "", "");
                    Session["DT1"] = DT1;
                    BindData();

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
                Utility.ClearObject(ref DT1);
            }
        }


        private void BindData()
        {
            DataTable DT = (DataTable)Session["DT1"];
            Utility.BindGVData(ref gvData, Session["DT1"], (DT.Rows.Count == 0));
            gvData.FooterRow.Visible = true;
            gvData.Visible = true;
        }

        private void SaveData()
        {
            DataTable DT = new DataTable();
            int OPs;
            String TName;
            try
            {
                Key = Utility.GetCtrl(hidTID);
                if (Key == "")
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }
                TName = Validation.GetCtrlStr(txtName);

                DT = Project.dal.SearchRptFidTemplate("3","", TName);
                if (DT.Rows.Count > 0 && OPs == DBUTIL.opINSERT)
                {
                    Msg = "Template name already exist!";
                }
                else
                {
                    Project.dal.MngRptFidTemplate(OPs, ref Key, TName,"3");
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

        private void DeleteData()
        {
            try
            {
                if (Utility.GetCtrl(hidTID) == "")
                {
                    Msg = "";
                    PageAction = "Result('C');";
                }
                else
                {

                    Project.dal.MngRptFidTemplate(DBUTIL.opDELETE, ref Key);
                    Msg = ""; PageAction = "Result('D2', LastPage);";

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {
                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    String mxSeq = "1";
                    if (Key != "") mxSeq = Project.dal.GetSQLValue("SELECT NVL(MAX(SEQ),0)+1 FROM O_RPT_FID_DETAIL WHERE TID =" + Key);

                    String txt = "";
                    txt = "<input type='text' class='form-control' name='txtSEQ' id='txtSEQ' size='10'  value='" + mxSeq + "' maxlength='3' onkeypress='return isNumber(event)' />";
                    e.Row.Cells[0].Text = txt;



                    //-- dropdown FID SAMPLING POINT ---
                    String ddl = "";
 
                    String SQL = "";
                    if (Key == "")
                    {
                        SQL = "SELECT DISTINCT FID FROM  " +
                        " (SELECT H2S_NAME AS FID FROM O_DIM_H2S  " +
                        " UNION SELECT HC_NAME AS FID FROM O_DIM_HC  " +
                        " UNION SELECT HG_NAME AS FID FROM O_DIM_HG  " +
                        " UNION SELECT O2_NAME AS FID FROM O_DIM_O2) A  " +
                        " ORDER BY FID ";
                    }
                    else
                    {
                        SQL = "SELECT DISTINCT FID FROM  " +
                       " (SELECT H2S_NAME AS FID FROM O_DIM_H2S  " +
                       " UNION SELECT HC_NAME AS FID FROM O_DIM_HC  " +
                       " UNION SELECT HG_NAME AS FID FROM O_DIM_HG  " +
                       " UNION SELECT O2_NAME AS FID FROM O_DIM_O2) A  " +
                       "WHERE NOT FID IN (SELECT FID FROM O_RPT_FID_DETAIL WHERE TID = " + Key + ") " +
                       " ORDER BY FID ";
                    }

                    ddl += "<option value=\"\"></option>";

                    DataTable DT = Project.dal.QueryData(SQL);
                    foreach (DataRow DR in DT.Rows)
                    {
                        ddl += "<option value='" + Utility.ToString(DR["FID"]) + "'>" + Utility.ToString(DR["FID"]) + "</option>";
                    }

                    ddl = "<select id=\"ddlFID\" name=\"ddlFID\" class=\"form-control select2\" > " + ddl + "</select>";
                    e.Row.Cells[1].Text = ddl;

                    Utility.ClearObject(ref DT);

                }


            }
            catch (Exception ex)
            {

            }

        }


        private void DoItemCommand(string action, int ItemIndex = 0)
        {
            DataTable DT = (DataTable)Session["DT1"];
            string FID = "";
            try
            {
                if (action != "ADD_ITEM")
                {
                    FID = DT.Rows[ItemIndex]["FID"].ToString();
                }

                else if (action == "ADD_ITEM")
                {
                    FID = Validation.ValidateStr(Request.Form["ddlFID"]);
                }

                switch (action)
                {
                    case "ADD_ITEM":
                        DT = Project.dal.SearchRptFidDetail(Key, FID, "");
                        if (DT.Rows.Count != 0)
                        {
                            Msg = "Duplicated key, Please verify the correctness ";
                        }
                        else
                        {
                            Project.dal.MngRptFidDetail(DBUTIL.opINSERT, ref Key, FID, Validation.ValidateStr(Request.Form["txtSEQ"]));
                            //Msg = "Save completed.";
                        }
                        break;
                    case "SAVE_ITEM":
                        Project.dal.MngRptFidDetail(DBUTIL.opUPDATE, ref Key, FID, Validation.ValidateStr(Request.Form["txtSEQEdit"]));
                        //Msg = "Save completed.";
                        break;
                    case "EDIT_ITEM": break;
                    case "DELETE_ITEM":
                        Project.dal.MngRptFidDetail(DBUTIL.opDELETE, ref Key, FID, "");
                        //Msg = "Delete completed.";
                        break;
                    case "CANCEL_ITEM": break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



    }
}